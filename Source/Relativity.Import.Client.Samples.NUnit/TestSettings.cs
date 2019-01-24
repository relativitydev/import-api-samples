// ----------------------------------------------------------------------------
// <copyright file="TestSettings.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.Sample.NUnit
{
	using System;

	/// <summary>
	/// Represents static test settings used throughout the sample.
	/// </summary>
	public class TestSettings
	{
		/// <summary>
		/// The maximum folder length.
		/// </summary>
		public const int MaxFolderLength = 255;

		/// <summary>
		/// The maximum folder depth.
		/// </summary>
		public const int MaxFolderDepth = 100;

		/// <summary>
		/// Gets or sets the Relativity user name used to authenticate.
		/// </summary>
		/// <value>
		/// The user name.
		/// </value>
		public static string RelativityUserName
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
		public static string RelativityPassword
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
		public static string SqlInstanceName
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
		public static string SqlAdminUserName
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
		public static string SqlAdminPassword
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
		public static bool SqlDropWorkspaceDatabase
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
		public static Uri RelativityUrl
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
		public static Uri RelativityRestUrl
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
		public static Uri RelativityServicesUrl
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
		public static Uri RelativityWebApiUrl
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
		public static int WorkspaceId
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
		public static string WorkspaceTemplate
		{
			get;
			set;
		}
	}
}