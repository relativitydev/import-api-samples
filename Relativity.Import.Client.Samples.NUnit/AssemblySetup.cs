// ----------------------------------------------------------------------------
// <copyright file="AssemblySetup.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.Sample.NUnit
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.IO;
	using System.Net;

	using global::NUnit.Framework;

	/// <summary>
	/// Represents a global assembly-wide setup routine that's guaranteed to be executed before ANY NUnit test.
	/// </summary>
	[SetUpFixture]
	public class AssemblySetup
	{
		internal static Relativity.Logging.ILog Logger
		{
			get;
			private set;
		}

		/// <summary>
		/// The main setup method.
		/// </summary>
		[OneTimeSetUp]
		public void Setup()
		{
			TestSettings.RelativityUserName = GetConfigurationStringValue("RelativityUserName");
			TestSettings.RelativityPassword = GetConfigurationStringValue("RelativityPassword");
			TestSettings.RelativityRestApiUrl = GetConfigurationStringValue("RelativityRestApiUrl");
			TestSettings.RelativityWebApiUrl = GetConfigurationStringValue("RelativityWebApiUrl");
			TestSettings.SqlInstanceName = GetConfigurationStringValue("SqlInstanceName");
			TestSettings.SqlAdminUserName = GetConfigurationStringValue("SqlAdminUserName");
			TestSettings.SqlAdminPassword = GetConfigurationStringValue("SqlAdminPassword");
			TestSettings.SqlDropWorkspaceDatabase = bool.Parse(GetConfigurationStringValue("SqlDropWorkspaceDatabase"));

			// Note: don't create the logger until all parameters have been retrieved.
			SetupLogger();
			TestSettings.WorkspaceId = TestHelper.CreateTestWorkspace(
				TestSettings.RelativityWebApiUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				Logger);
		}

		/// <summary>
		/// The main teardown method.
		/// </summary>
		[OneTimeTearDown]
		public void TearDown()
		{
			TestHelper.DeleteTestWorkspace(
				TestSettings.RelativityWebApiUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.WorkspaceId,
				Logger);
			string database = $"EDDS{TestSettings.WorkspaceId}";
			if (TestSettings.SqlDropWorkspaceDatabase && TestSettings.WorkspaceId > 0)
			{
				try
				{
					SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
					{
						DataSource = TestSettings.SqlInstanceName,
						IntegratedSecurity = false,
						UserID = TestSettings.SqlAdminUserName,
						Password = TestSettings.SqlAdminPassword,
						InitialCatalog = string.Empty
					};

					SqlConnection.ClearAllPools();
					using (var connection = new SqlConnection(builder.ConnectionString))
					{
						connection.Open();
						using (var command = connection.CreateCommand())
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

		private static string GetConfigurationStringValue(string key)
		{
			var value = System.Configuration.ConfigurationManager.AppSettings.Get(key);
			if (!string.IsNullOrEmpty(value))
			{
				return value;
			}

			throw new AssertionException($"The '{key}' app.config setting is not specified.");
		}

		private static void SetupLogger()
		{
			Logging.LoggerOptions loggerOptions = new Logging.LoggerOptions
			{
				Application = "8A1A6418-29B3-4067-8C9E-51E296F959DE",
				ConfigurationFileLocation = Path.Combine(TestHelper.GetBasePath(), "LogConfig.xml"),
				System = "Import-API",
				SubSystem = "Samples"
			};

			// Configure the optional SEQ sink to periodically send logs to the local SEQ server for improved debugging.
			// See https://getseq.net/ for more details.
			loggerOptions.AddSinkParameter(
				Logging.Configuration.SeqSinkConfig.ServerUrlSinkParameterKey,
				new Uri("http://localhost:5341"));

			// Configure the optional HTTP sink to periodically send logs to Relativity.
			loggerOptions.AddSinkParameter(
				Logging.Configuration.RelativityHttpSinkConfig.CredentialSinkParameterKey,
				new NetworkCredential(TestSettings.RelativityUserName, TestSettings.RelativityPassword));
			loggerOptions.AddSinkParameter(
				Logging.Configuration.RelativityHttpSinkConfig.InstanceUrlSinkParameterKey,
				TestSettings.RelativityRestApiUrl);
			Logger = Logging.Factory.LogFactory.GetLogger(loggerOptions);

			// Until Import API supports passing a logger instance via constructor, the API
			// internally uses the Logger singleton instance if defined.
			Relativity.Logging.Log.Logger = Logger;
		}
	}
}