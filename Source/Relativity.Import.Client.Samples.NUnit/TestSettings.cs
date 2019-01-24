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
		public static string RelativityUserName
		{
			get;
			set;
		}

		public static string RelativityPassword
		{
			get;
			set;
		}

		public static string SqlInstanceName
		{
			get;
			set;
		}

		public static string SqlAdminUserName
		{
			get;
			set;
		}

		public static string SqlAdminPassword
		{
			get;
			set;
		}

		public static bool SqlDropWorkspaceDatabase
		{
			get;
			set;
		}

		public static Uri RelativityUrl
		{
			get;
			set;
		}

		public static Uri RelativityRestUrl
		{
			get;
			set;
		}

		public static Uri RelativityServicesUrl
		{
			get;
			set;
		}

		public static string RelativityWebApiUrl
		{
			get;
			set;
		}

		public static int WorkspaceId
		{
			get;
			set;
		}

		public static string WorkspaceTemplate
		{
			get;
			set;
		}
	}
}