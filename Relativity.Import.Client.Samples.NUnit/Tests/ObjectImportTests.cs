// ----------------------------------------------------------------------------
// <copyright file="ObjectImportTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.Sample.NUnit.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Data;

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;

	using global::NUnit.Framework;

	/// <summary>
	/// Represents a test that creates a new workspace, import objects, validates the results, and deletes the workspace.
	/// </summary>
	[TestFixture]
	public class ObjectImportTests : ImportTestsBase
	{
		private const string ArtifactTypeName = "TransferJob";
		private int _artifactTypeId;
		private int _identifierFieldId;

		private static IEnumerable<string> TestCases
		{
			get
			{
				yield return "Job-Small";
				yield return "Job-Medium";
				yield return "Job-Large";
			}
		}

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			this.CreateObjectType(ArtifactTypeName);
			this._artifactTypeId = this.GetArtifactTypeId(ArtifactTypeName);
			this._identifierFieldId = this.GetIdentifierFieldId(ArtifactTypeName);
		}

		[Test]
		[TestCaseSource(nameof(TestCases))]
		public void ShouldImportTheObject(string name)
		{
			// Arrange
			ImportAPI importApi = CreateImportApiObject();
			ImportBulkArtifactJob job = importApi.NewObjectImportJob(this._artifactTypeId);
			this.ConfigureJobSettings(job);
			this.CatchJobEvents(job);
			this.DataTable.Columns.AddRange(new[]
			{
				new DataColumn("Name", typeof(string))
			});

			int initialObjectCount = this.GetObjectCount(this._artifactTypeId);
			this.DataTable.Rows.Add(name);
			job.SourceData.SourceData = this.DataTable.CreateDataReader();

			// Act
			job.Execute();

			// Assert
			Assert.That(this.FatalException, Is.Null);
			Assert.That(this.JobReport, Is.Not.Null);
			Assert.That(this.JobReport.ErrorRowCount, Is.Zero);
			Assert.That(this.JobReport.TotalRows, Is.EqualTo(1));
			int expectedObjectCount = initialObjectCount + this.DataTable.Rows.Count;
			int actualObjectCount = this.GetObjectCount(this._artifactTypeId);
			Assert.That(actualObjectCount, Is.EqualTo(expectedObjectCount));
		}

		private void ConfigureJobSettings(ImportBulkArtifactJob job)
		{
			Settings settings = job.Settings;
			settings.ArtifactTypeId = this._artifactTypeId;
			settings.Billable = false;
			settings.BulkLoadFileFieldDelimiter = ";";
			settings.CaseArtifactId = TestSettings.WorkspaceId;						
			settings.CopyFilesToDocumentRepository = false;
			settings.DisableControlNumberCompatibilityMode = true;
			settings.DisableExtractedTextEncodingCheck = true;
			settings.DisableExtractedTextFileLocationValidation = true;
			settings.DisableNativeLocationValidation = false;
			settings.DisableNativeValidation = false;
			settings.DisableUserSecurityCheck = true;
			settings.ExtractedTextFieldContainsFilePath = false;
			settings.IdentityFieldId = this._identifierFieldId;
			settings.LoadImportedFullTextFromServer = false;
			settings.MaximumErrorCount = int.MaxValue - 1;
			settings.MoveDocumentsInAppendOverlayMode = false;
			settings.MultiValueDelimiter = ';';
			settings.NativeFileCopyMode = NativeFileCopyModeEnum.DoNotImportNativeFiles;
			settings.OverwriteMode = OverwriteModeEnum.Append;
			settings.SelectedIdentifierFieldName = "Name";
			settings.StartRecordNumber = 0;
		}

		private void CatchJobEvents(ImportBulkArtifactJob job)
		{
			job.OnMessage += status =>
			{
				Console.WriteLine("[Message]: " + status.Message);
			};

			job.OnFatalException += report =>
			{
				this.FatalException = report.FatalException;
				Console.WriteLine("[Fatal Exception]: " + report.FatalException);
			};

			job.OnComplete += report =>
			{
				this.JobReport = report;
				Console.WriteLine("[Complete]");
			};
		}
	}
}