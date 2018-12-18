// ----------------------------------------------------------------------------
// <copyright file="DocImportTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.Sample.NUnit.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Text;

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;

	using global::NUnit.Framework;

	/// <summary>
	/// Represents a test that creates a new workspace, import documents, validates the results, and deletes the workspace.
	/// </summary>
	[TestFixture]
	public class DocImportTests : ImportTestsBase
	{
		private const string ArtifactTypeName = "Document";
		private int _artifactTypeId;
		private int _identifierFieldId;

		private static IEnumerable<string> TestCases
		{
			get
			{
				yield return "EDRM-Sample1.pdf";
				yield return "EDRM-Sample2.doc";
				yield return "EDRM-Sample3.xlsx";
			}
		}

		protected override void OnSetup()
		{
			base.OnSetup();
			this._artifactTypeId = this.GetArtifactTypeId(ArtifactTypeName);
			this._identifierFieldId = this.GetIdentifierFieldId(ArtifactTypeName);
		}

		[Test]
		[TestCaseSource(nameof(TestCases))]
		public void ShouldImportTheDoc(string fileName)
		{
			// Arrange
			ImportAPI importApi = CreateImportApiObject();
			ImportBulkArtifactJob job = importApi.NewNativeDocumentImportJob();
			this.ConfigureJobSettings(job);
			this.CatchJobEvents(job);
			string file = GetResourceFilePath("Docs", fileName);
			this.DataTable.Columns.AddRange(new[]
			{
				new DataColumn("Control Number", typeof(string)),
				new DataColumn("File Path", typeof(string))
			});

			this.DataTable.Rows.Add("REL-" + Guid.NewGuid(), file);
			job.SourceData.SourceData = this.DataTable.CreateDataReader();
			int initialDocumentCount = this.GetObjectCount((int) ArtifactType.Document);

			// Act
			job.Execute();

			// Assert
			Assert.That(this.FatalException, Is.Null);
			Assert.That(this.JobReport, Is.Not.Null);
			Assert.That(this.JobReport.ErrorRowCount, Is.Zero);
			Assert.That(this.JobReport.TotalRows, Is.EqualTo(1));
			int expectedDocCount = initialDocumentCount + this.DataTable.Rows.Count;
			int actualDocCount = this.GetObjectCount((int)ArtifactType.Document);
			Assert.That(actualDocCount, Is.EqualTo(expectedDocCount));
		}

		private void ConfigureJobSettings(ImportBulkArtifactJob job)
		{
			Settings settings = job.Settings;
			settings.ArtifactTypeId = this._artifactTypeId;
			settings.BulkLoadFileFieldDelimiter = ";";
			settings.CaseArtifactId = TestSettings.WorkspaceId;
			settings.CopyFilesToDocumentRepository = true;
			settings.DisableControlNumberCompatibilityMode = true;
			settings.DisableExtractedTextFileLocationValidation = false;
			settings.DisableNativeLocationValidation = false;
			settings.DisableNativeValidation = false;
			settings.ExtractedTextEncoding = Encoding.Unicode;
			settings.ExtractedTextFieldContainsFilePath = false;
			settings.FileSizeColumn = "NativeFileSize";
			settings.FileSizeMapped = true;
			settings.IdentityFieldId = this._identifierFieldId;
			settings.LoadImportedFullTextFromServer = false;
			settings.NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles;
			settings.NativeFilePathSourceFieldName = "File Path";
			settings.OIFileIdColumnName = "OutsideInFileId";
			settings.OIFileIdMapped = true;
			settings.OIFileTypeColumnName = "OutsideInFileType";
			settings.OverwriteMode = OverwriteModeEnum.Append;
			settings.SelectedIdentifierFieldName = "Control Number";
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