// ----------------------------------------------------------------------------
// <copyright file="IntegrationTestParameters.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework
{
	using System;

	using Newtonsoft.Json;

	/// <summary>
	/// Represents the parameters used by all integration tests.
	/// </summary>
	public class IntegrationTestParameters
	{
		/// <summary>
		/// The maximum folder length.
		/// </summary>
		public const int MaxFolderLength = 255;

		/// <summary>
		/// Initializes a new instance of the <see cref="IntegrationTestParameters"/> class.
		/// </summary>
		public IntegrationTestParameters()
		{
			this.FileShareUncPath = null;
			this.RelativityPassword = null;
			this.RelativityRestUrl = null;
			this.RelativityServicesUrl = null;
			this.RelativityUrl = null;
			this.RelativityUserName = null;
			this.RelativityWebApiUrl = null;
			this.ServerCertificateValidation = false;
			this.SkipAsperaModeTests = false;
			this.SkipDirectModeTests = false;
			this.SkipIntegrationTests = false;
			this.WorkspaceId = 0;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IntegrationTestParameters"/> class.
		/// </summary>
		/// <param name="copy">
		/// The object to copy.
		/// </param>
		public IntegrationTestParameters(IntegrationTestParameters copy)
		{
			if (copy == null)
			{
				throw new ArgumentNullException(nameof(copy));
			}

			this.FileShareUncPath = copy.FileShareUncPath;
			this.RelativityPassword = copy.RelativityPassword;
			this.RelativityRestUrl = new Uri(copy.RelativityRestUrl.ToString());
			this.RelativityServicesUrl = new Uri(copy.RelativityServicesUrl.ToString());
			this.RelativityUrl = new Uri(copy.RelativityUrl.ToString());
			this.RelativityUserName = copy.RelativityUserName;
			this.RelativityWebApiUrl = new Uri(copy.RelativityWebApiUrl.ToString());
			this.ServerCertificateValidation = copy.ServerCertificateValidation;
			this.SkipAsperaModeTests = copy.SkipAsperaModeTests;
			this.SkipDirectModeTests = copy.SkipDirectModeTests;
			this.SkipIntegrationTests = copy.SkipIntegrationTests;
			this.SqlAdminPassword = copy.SqlAdminPassword;
			this.SqlAdminUserName = copy.SqlAdminUserName;
			this.SqlDropWorkspaceDatabase = copy.SqlDropWorkspaceDatabase;
			this.SqlInstanceName = copy.SqlInstanceName;
			this.WorkspaceId = copy.WorkspaceId;
		}

		/// <summary>
		/// Gets or sets the full UNC path to the file share.
		/// </summary>
		/// <value>
		/// The full path.
		/// </value>
		[JsonProperty("FileShareUncPath")]
		[IntegrationTestParameter(false)]
		public string FileShareUncPath
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the Relativity password used to authenticate.
		/// </summary>
		/// <value>
		/// The password.
		/// </value>
		[JsonProperty("RelativityPassword")]
#if DEBUG
		[IntegrationTestParameter(true, false)]
#else
		[IntegrationTestParameter(true, true)]
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
#endif
		public string RelativityPassword
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the Relativity REST API URL.
		/// </summary>
		/// <value>
		/// The <see cref="Uri"/> instance.
		/// </value>
		[JsonProperty("RelativityRestUrl")]
		[IntegrationTestParameter(true)]
		public Uri RelativityRestUrl
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the Relativity services API URL.
		/// </summary>
		/// <value>
		/// The <see cref="Uri"/> instance.
		/// </value>
		[JsonProperty("RelativityServicesUrl")]
		[IntegrationTestParameter(true)]
		public Uri RelativityServicesUrl
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the Relativity URL.
		/// </summary>
		/// <value>
		/// The <see cref="Uri"/> instance.
		/// </value>
		[JsonProperty("RelativityUrl")]
		[IntegrationTestParameter(true)]
		public Uri RelativityUrl
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the Relativity user name used to authenticate.
		/// </summary>
		/// <value>
		/// The user name.
		/// </value>
		[JsonProperty("RelativityUserName")]
		[IntegrationTestParameter(true)]
		public string RelativityUserName
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the Relativity WebAPI URL.
		/// </summary>
		/// <value>
		/// The <see cref="Uri"/> instance.
		/// </value>
		[JsonProperty("RelativityWebApiUrl")]
		[IntegrationTestParameter(true)]
		public Uri RelativityWebApiUrl
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to enforce server certificate validation errors.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to enforce server certificate validation errors; otherwise, <see langword="false" />.
		/// </value>
		[JsonProperty("ServerCertificateValidation")]
		[IntegrationTestParameter(true)]
		public bool ServerCertificateValidation
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the SQL admin password.
		/// </summary>
		/// <value>
		/// The password.
		/// </value>
		[JsonProperty("SqlAdminPassword")]
#if DEBUG
		[IntegrationTestParameter(true, false)]
#else
		[IntegrationTestParameter(true, true)]
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
#endif
		public string SqlAdminPassword
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the SQL admin user name.
		/// </summary>
		/// <value>
		/// The user name.
		/// </value>
		[JsonProperty("SqlAdminUserName")]
		[IntegrationTestParameter(true)]
		public string SqlAdminUserName
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to drop the test workspace SQL database.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to drop the workspace SQL database; otherwise, <see langword="false" />.
		/// </value>
		[JsonProperty("SqlDropWorkspaceDatabase")]
		[IntegrationTestParameter(true)]
		public bool SqlDropWorkspaceDatabase
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the SQL instance name.
		/// </summary>
		/// <value>
		/// The SQL instance name.
		/// </value>
		[JsonProperty("SqlInstanceName")]
		[IntegrationTestParameter(true)]
		public string SqlInstanceName
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to skip tests that specify the <see cref="Relativity.Transfer.WellKnownTransferClient.Aspera"/> transfer client.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to skip the tests; otherwise, <see langword="false" />.
		/// </value>
		[JsonProperty("SkipAsperaModeTests")]
		[IntegrationTestParameter(true)]
		public bool SkipAsperaModeTests
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to skip tests that specify the <see cref="Relativity.Transfer.WellKnownTransferClient.FileShare"/> transfer client.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to skip the tests; otherwise, <see langword="false" />.
		/// </value>
		[JsonProperty("SkipDirectModeTests")]
		[IntegrationTestParameter(true)]
		public bool SkipDirectModeTests
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to skip integration tests.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to skip the integration tests; otherwise, <see langword="false" />.
		/// </value>
		[JsonProperty("SkipIntegrationTests")]
		[IntegrationTestParameter(true)]
		public bool SkipIntegrationTests
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the test workspace artifact identifier.
		/// </summary>
		/// <value>
		/// The artifact identifier.
		/// </value>
		[JsonProperty("WorkspaceId")]
		[IntegrationTestParameter(false)]
		public int WorkspaceId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name of the workspace template used when creating a test workspace.
		/// </summary>
		/// <value>
		/// The template name.
		/// </value>
		[JsonProperty("WorkspaceTemplate")]
		[IntegrationTestParameter(true)]
		public string WorkspaceTemplate
		{
			get;
			set;
		}

		/// <summary>
		/// Performs a deep copy of this instance.
		/// </summary>
		/// <returns>
		/// The <see cref="IntegrationTestParameters"/> instance.
		/// </returns>
		public IntegrationTestParameters DeepCopy()
		{
			return new IntegrationTestParameters(this);
		}
	}
}