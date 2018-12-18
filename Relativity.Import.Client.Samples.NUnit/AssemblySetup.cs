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

	using global::NUnit.Framework;

	/// <summary>
	/// Represents a global assembly-wide setup routine that's guaranteed to be executed before ANY NUnit test.
	/// </summary>
	[SetUpFixture]
	public class AssemblySetup
	{
		/// <summary>
		/// The main setup method.
		/// </summary>
		[OneTimeSetUp]
		public void Setup()
		{
			TestSettings.RelativityUserName = GetConfigurationStringValue("RelativityUserName");
			TestSettings.RelativityPassword = GetConfigurationStringValue("RelativityPassword");
			TestSettings.RelativityWebApiUrl = GetConfigurationStringValue("RelativityWebApiUrl");
			TestSettings.SqlInstanceName = GetConfigurationStringValue("SqlInstanceName");
			TestSettings.SqlAdminUserName = GetConfigurationStringValue("SqlAdminUserName");
			TestSettings.SqlAdminPassword = GetConfigurationStringValue("SqlAdminPassword");
			TestSettings.SqlDropWorkspaceDatabase = bool.Parse(GetConfigurationStringValue("SqlDropWorkspaceDatabase"));
			TestSettings.WorkspaceId = TestHelper.CreateTestWorkspace(
				TestSettings.RelativityWebApiUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword);
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
				TestSettings.WorkspaceId);
			if (TestSettings.SqlDropWorkspaceDatabase && TestSettings.WorkspaceId > 0)
			{
				string database = $"EDDS{TestSettings.WorkspaceId}";

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
							Console.WriteLine($"Successfully dropped the {database} SQL workspace database.");
						}
					}
				}
				catch (Exception e)
				{
					Console.WriteLine($"Failed to drop the {database} SQL workspace database. Exception: " + e);
				}
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
	}
}