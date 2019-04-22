// ----------------------------------------------------------------------------
// <copyright file="IntegrationTestHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.TestFramework
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Data;
	using System.Data.SqlClient;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Reflection;

	using Newtonsoft.Json;

	/// <summary>
	/// Defines static methods to setup and teardown integration tests.
	/// </summary>
	public static class IntegrationTestHelper
	{
		/// <summary>
		/// Gets or sets a value indicating whether environment variables are enabled.
		/// </summary>
		/// <value>
		/// <see langword="true" /> when environment variables are enabled; otherwise, <see langword="false" />.
		/// </value>
		public static bool EnvironmentVariablesEnabled { get; set; } = true;

		/// <summary>
		/// Gets the Relativity logging instance.
		/// </summary>
		/// <value>
		/// The <see cref="Relativity.Logging.ILog"/> instance.
		/// </value>
		public static Relativity.Logging.ILog Logger
		{
			get;
			private set;
		}

		/// <summary>
		/// Create the integration test environment with a new test workspace and returns the test parameters.
		/// </summary>
		/// <returns>
		/// The <see cref="IntegrationTestParameters"/> instance.
		/// </returns>
		public static IntegrationTestParameters Create()
		{
			// Note: don't create the logger until all parameters have been retrieved.
			IntegrationTestParameters parameters = GetIntegrationTestParameters();
			SetupLogger(parameters);
			SetupServerCertificateValidation(parameters);
			if (parameters.SkipIntegrationTests)
			{
				Console.WriteLine("Skipping test workspace creation.");
				return parameters;
			}

			Console.WriteLine("Creating a test workspace...");
			WorkspaceHelper.CreateTestWorkspace(parameters, Logger);
			kCura.Relativity.ImportAPI.ImportAPI iapi = new kCura.Relativity.ImportAPI.ImportAPI(
				parameters.RelativityUserName,
				parameters.RelativityPassword,
				parameters.RelativityWebApiUrl.ToString());
			IEnumerable<kCura.Relativity.ImportAPI.Data.Workspace> workspaces = iapi.Workspaces();
			kCura.Relativity.ImportAPI.Data.Workspace workspace =
				workspaces.FirstOrDefault(x => x.ArtifactID == parameters.WorkspaceId);
			if (workspace == null)
			{
				throw new InvalidOperationException(
					$"This operation cannot be performed because the workspace {parameters.WorkspaceId} that was just created doesn't exist.");
			}

			parameters.FileShareUncPath = workspace.DocumentPath;
			Console.WriteLine($"Created {parameters.WorkspaceId} test workspace.");
			return parameters;
		}

		/// <summary>
		/// Destroy the integration test environment that was previously created.
		/// </summary>
		/// <param name="parameters">
		/// The integration test parameters.
		/// </param>
		public static void Destroy(IntegrationTestParameters parameters)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			if (parameters.SkipIntegrationTests)
			{
				Console.WriteLine("Skipping test workspace teardown.");
				return;
			}

			WorkspaceHelper.DeleteTestWorkspace(parameters, Logger);
			string database = $"EDDS{parameters.WorkspaceId}";
			if (parameters.SqlDropWorkspaceDatabase && parameters.WorkspaceId > 0)
			{
				try
				{
					SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
					{
						DataSource = parameters.SqlInstanceName,
						IntegratedSecurity = false,
						UserID = parameters.SqlAdminUserName,
						Password = parameters.SqlAdminPassword,
						InitialCatalog = string.Empty,
					};

					SqlConnection.ClearAllPools();
					using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
					{
						connection.Open();
						using (SqlCommand command = connection.CreateCommand())
						{
							command.CommandText = $@"
IF EXISTS(SELECT name FROM sys.databases WHERE name = '{database}')
BEGIN
	ALTER DATABASE [{database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
	DROP DATABASE [{database}]
END";
							command.CommandType = CommandType.Text;
							command.ExecuteNonQuery();
							Logger.LogInformation("Successfully dropped the {DatabaseName} SQL workspace database.", database);
							Console.WriteLine($"Successfully dropped the {database} SQL workspace database.");
						}
					}
				}
				catch (Exception e)
				{
					Logger.LogError(e, "Failed to drop the {DatabaseName} SQL workspace database.", database);
					Console.WriteLine($"Failed to drop the {database} SQL workspace database. Exception: " + e);
				}
			}
			else
			{
				Logger.LogInformation("Skipped dropping the {DatabaseName} SQL workspace database.", database);
			}
		}

		private static IntegrationTestParameters GetIntegrationTestParameters()
		{
			Console.WriteLine("Retrieving and dumping all integration test parameters...");
			bool decryptParameters = false;
			IntegrationTestParameters parameters = new IntegrationTestParameters();
			string testEnvironment = GetEnvironmentVariable("IAPI_INTEGRATION_TEST_ENV");
			string jsonFile = GetEnvironmentVariable("IAPI_INTEGRATION_TEST_JSON_FILE");
			if (!string.IsNullOrWhiteSpace(testEnvironment))
			{
				string resourceFile;
				switch (testEnvironment.ToUpperInvariant())
				{
					case "HYPERV":
						resourceFile = "test-parameters-hyperv.json";
						break;

					case "E2E":
						resourceFile = "test-parameters-e2e.json";
						break;

					default:
						throw new InvalidOperationException($"The test environment '{testEnvironment}' is not recognized or supported.");
				}

				using (Stream stream = ResourceFileHelper.ExtractToStream(
					Assembly.GetExecutingAssembly(),
					$"Relativity.Import.Export.TestFramework.Resources.{resourceFile}"))
				{
					StreamReader reader = new StreamReader(stream);
					JsonSerializer serializer = new JsonSerializer();
					parameters = serializer.Deserialize<IntegrationTestParameters>(new JsonTextReader(reader));
					decryptParameters = true;
				}
			}
			else if (!string.IsNullOrWhiteSpace(jsonFile))
			{
				parameters = JsonConvert.DeserializeObject<IntegrationTestParameters>(File.ReadAllText(jsonFile));
				decryptParameters = true;
			}
			else
			{
				foreach (var prop in parameters.GetType().GetProperties())
				{
					IntegrationTestParameterAttribute attribute =
						prop.GetCustomAttribute<IntegrationTestParameterAttribute>();
					if (attribute == null || !attribute.IsMapped)
					{
						continue;
					}

					string value = GetConfigurationStringValue(prop.Name);
					if (prop.PropertyType == typeof(string))
					{
						prop.SetValue(parameters, value);
					}
					else if (prop.PropertyType == typeof(bool))
					{
						prop.SetValue(parameters, bool.Parse(value));
					}
					else if (prop.PropertyType == typeof(Uri))
					{
						prop.SetValue(parameters, new Uri(value));
					}
					else
					{
						string message =
							$"The integration test parameter '{prop.Name}' of type '{prop.PropertyType}' isn't supported by the integration test helper.";
						throw new ConfigurationErrorsException(message);
					}
				}
			}

			if (decryptParameters)
			{
				DecryptTestParameters(parameters);
			}

			foreach (var prop in parameters.GetType().GetProperties())
			{
				IntegrationTestParameterAttribute attribute =
					prop.GetCustomAttribute<IntegrationTestParameterAttribute>();
				if (attribute == null || !attribute.IsMapped)
				{
					continue;
				}

				if (attribute.IsSecret)
				{
					Console.WriteLine("{0}=[Obfuscated]", prop.Name);
				}
				else
				{
					Console.WriteLine("{0}={1}", prop.Name, prop.GetValue(parameters, null));
				}
			}

			Console.WriteLine("Retrieved and dumped all integration test parameters.");
			return parameters;
		}

		private static void DecryptTestParameters(IntegrationTestParameters parameters)
		{
			if (!string.IsNullOrWhiteSpace(parameters.RelativityPassword))
			{
				parameters.RelativityPassword = CryptoHelper.Decrypt(parameters.RelativityPassword);
			}

			if (!string.IsNullOrWhiteSpace(parameters.RelativityUserName))
			{
				parameters.RelativityUserName = CryptoHelper.Decrypt(parameters.RelativityUserName);
			}

			if (!string.IsNullOrWhiteSpace(parameters.SqlAdminPassword))
			{
				parameters.SqlAdminPassword = CryptoHelper.Decrypt(parameters.SqlAdminPassword);
			}

			if (!string.IsNullOrWhiteSpace(parameters.SqlAdminUserName))
			{
				parameters.SqlAdminUserName = CryptoHelper.Decrypt(parameters.SqlAdminUserName);
			}
		}

		private static string GetConfigurationStringValue(string key)
		{
			string envVariable = $"IAPI_INTEGRATION_{key.ToUpperInvariant()}";
			string value = GetEnvironmentVariable(envVariable);
			if (!string.IsNullOrWhiteSpace(value))
			{
				return value;
			}

			value = System.Configuration.ConfigurationManager.AppSettings.Get(key);
			if (!string.IsNullOrEmpty(value))
			{
				return value;
			}

			throw new InvalidOperationException($"The '{key}' app.config setting or '{envVariable}' environment variable is not specified.");
		}

		private static string GetEnvironmentVariable(string envVariable)
		{
			if (EnvironmentVariablesEnabled)
			{
				// Note: these targets are intentionally ordered to favor process vars!
				IEnumerable<EnvironmentVariableTarget> targets = new[]
					                                                 {
						                                                 EnvironmentVariableTarget.Process,
						                                                 EnvironmentVariableTarget.User,
						                                                 EnvironmentVariableTarget.Machine,
					                                                 };
				foreach (EnvironmentVariableTarget target in targets)
				{
					string envValue = Environment.GetEnvironmentVariable(envVariable, target);
					if (!string.IsNullOrEmpty(envValue))
					{
						return envValue;
					}
				}
			}

			return string.Empty;
		}

		private static void SetupServerCertificateValidation(IntegrationTestParameters parameters)
		{
			if (!parameters.ServerCertificateValidation)
			{
				ServicePointManager.ServerCertificateValidationCallback +=
					(sender, certificate, chain, sslPolicyErrors) => true;
			}
		}

		private static void SetupLogger(IntegrationTestParameters parameters)
		{
			Logging.LoggerOptions loggerOptions = new Logging.LoggerOptions
			{
				Application = "8A1A6418-29B3-4067-8C9E-51E296F959DE",
				ConfigurationFileLocation = Path.Combine(ResourceFileHelper.GetBasePath(), "LogConfig.xml"),
				System = "Import-API",
				SubSystem = "Samples",
			};

			// Configure the optional SEQ sink to periodically send logs to the local SEQ server for improved debugging.
			// See https://getseq.net/ for more details.
			loggerOptions.AddSinkParameter(
				Logging.Configuration.SeqSinkConfig.ServerUrlSinkParameterKey,
				new Uri("http://localhost:5341"));

			// Configure the optional HTTP sink to periodically send logs to Relativity.
			loggerOptions.AddSinkParameter(
				Logging.Configuration.RelativityHttpSinkConfig.CredentialSinkParameterKey,
				new NetworkCredential(parameters.RelativityUserName, parameters.RelativityPassword));
			loggerOptions.AddSinkParameter(
				Logging.Configuration.RelativityHttpSinkConfig.InstanceUrlSinkParameterKey,
				parameters.RelativityUrl);
			Logger = Logging.Factory.LogFactory.GetLogger(loggerOptions);

			// Until Import API supports passing a logger instance via constructor, the API
			// internally uses the Logger singleton instance if defined.
			Relativity.Logging.Log.Logger = Logger;
		}
	}
}