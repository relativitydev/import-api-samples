// ----------------------------------------------------------------------------
// <copyright file="ImportTestsBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.Sample.NUnit.Tests
{
	using System;
	using System.Data;
	using System.IO;

	using kCura.Relativity.ImportAPI;

	using global::NUnit.Framework;

	/// <summary>
	/// Represents an abstract base class to provide common functionality and helper methods.
	/// </summary>
	public abstract class ImportTestsBase
	{
		protected DataTable DataTable
		{
			get;
			private set;
		}

		protected Exception FatalException
		{
			get;
			set;
		}

		protected JobReport JobReport
		{
			get;
			set;
		}

		[SetUp]
		public void Setup()
		{
			Assert.That(TestSettings.WorkspaceId, Is.GreaterThan(0));
			this.DataTable = new DataTable();
			this.FatalException = null;
			this.JobReport = null;
			this.OnSetup();
		}

		[TearDown]
		public void Teardown()
		{
			DataTable?.Dispose();
		}

		protected virtual void OnSetup()
		{
		}

		protected void CreateObjectType(string objectTypeName)
		{
			TestHelper.CreateObjectType(
				TestSettings.RelativityWebApiUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.WorkspaceId,
				objectTypeName);
		}

		protected void CreateFixedLengthTextField(int descriptorArtifactTypeId, string fieldName, int length)
		{
			TestHelper.CreateFixedLengthTextField(
				TestSettings.RelativityWebApiUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.WorkspaceId,
				descriptorArtifactTypeId,
				fieldName,
				length);
		}

		protected int GetArtifactTypeId(string objectTypeName)
		{
			return TestHelper.GetArtifactTypeId(
				TestSettings.RelativityWebApiUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.WorkspaceId,
				objectTypeName);
		}

		protected int GetIdentifierFieldId(string artifactTypeName)
		{
			return TestHelper.GetIdentifierFieldId(
				TestSettings.RelativityWebApiUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.WorkspaceId,
				artifactTypeName);
		}

		protected int GetObjectCount(int artifactTypeId)
		{
			return TestHelper.GetObjectCount(
				TestSettings.RelativityWebApiUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword, TestSettings.WorkspaceId,
				artifactTypeId);
		}

		protected ImportAPI CreateImportApiObject()
		{
			return new ImportAPI(
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.RelativityWebApiUrl);
		}

		protected static string GetResourceFilePath(string folder, string fileName)
		{
			string basePath = Path.GetDirectoryName(typeof(ImportTestsBase).Assembly.Location);
			string sourceFile = Path.Combine(Path.Combine(Path.Combine(basePath, "Resources"), folder), fileName);
			return sourceFile;
		}
	}
}