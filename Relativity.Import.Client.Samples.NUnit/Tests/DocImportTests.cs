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

	using global::NUnit.Framework;

	/// <summary>
	/// Represents a test that creates a new workspace, import documents, validates the results, and deletes the workspace.
	/// </summary>
	[TestFixture]
	public class DocImportTests : ImportTestsBase
	{
		private const string ArtifactTypeName = "Document";
		private const string FieldControlNumber = "Control Number";
		private const string FieldFilePath = "File Path";
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
			this._artifactTypeId = this.QueryArtifactTypeId(ArtifactTypeName);
			this._identifierFieldId = this.QueryIdentifierFieldId(ArtifactTypeName);
		}

		[Test]
		[TestCaseSource(nameof(TestCases))]
		public void ShouldImportTheDoc(string fileName)
		{
			// Arrange
			kCura.Relativity.ImportAPI.ImportAPI importApi = CreateImportApiObject();
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job = importApi.NewNativeDocumentImportJob();
			this.ConfigureJobSettings(job);
			this.CatchJobEvents(job);
			string file = TestHelper.GetResourceFilePath("Docs", fileName);
			this.DataTable.Columns.AddRange(new[]
			{
				new DataColumn(FieldControlNumber, typeof(string)),
				new DataColumn(FieldFilePath, typeof(string))
			});

			string controlNumber = "REL-" + Guid.NewGuid();
			this.DataTable.Rows.Add(controlNumber, file);
			job.SourceData.SourceData = this.DataTable.CreateDataReader();
			int initialDocumentCount = this.QueryRelativityObjectCount((int) ArtifactType.Document);

			// Act
			job.Execute();

			// Assert - the job completed and the report matches the expected values.
			Assert.That(this.JobCompletedReport, Is.Not.Null);
			Assert.That(this.JobCompletedReport.EndTime, Is.GreaterThan(this.JobCompletedReport.StartTime));
			Assert.That(this.JobCompletedReport.ErrorRowCount, Is.Zero);
			Assert.That(this.JobCompletedReport.FileBytes, Is.GreaterThan(0));
			Assert.That(this.JobCompletedReport.MetadataBytes, Is.GreaterThan(0));
			Assert.That(this.JobCompletedReport.StartTime, Is.GreaterThan(this.ImportStartTime));
			Assert.That(this.JobCompletedReport.TotalRows, Is.EqualTo(1));

			// Assert - the events match the expected values.
			Assert.That(this.ErrorEvents.Count, Is.EqualTo(0));
			Assert.That(this.FatalExceptionEvent, Is.Null);
			Assert.That(this.MessageEvents.Count, Is.GreaterThan(0));
			Assert.That(this.ProcessProgressEvents.Count, Is.GreaterThan(0));
			Assert.That(this.ProgressRowEvents.Count, Is.GreaterThan(0));

			// Assert - the object count is incremented by 1.
			int expectedDocCount = initialDocumentCount + this.DataTable.Rows.Count;
			int actualDocCount = this.QueryRelativityObjectCount((int)ArtifactType.Document);
			Assert.That(actualDocCount, Is.EqualTo(expectedDocCount));

			// Assert - the imported document exists.
			IList<Relativity.Services.Objects.DataContracts.RelativityObject> docs = 
				this.QueryRelativityObjects(this._artifactTypeId, new[] { FieldControlNumber });
			Assert.That(docs, Is.Not.Null);
			Assert.That(docs.Count, Is.EqualTo(expectedDocCount));
			Relativity.Services.Objects.DataContracts.RelativityObject importedObj
				= FindRelativityObject(docs, FieldControlNumber, controlNumber);
			Assert.That(importedObj, Is.Not.Null);
		}

		private void ConfigureJobSettings(kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job)
		{
			kCura.Relativity.DataReaderClient.Settings settings = job.Settings;
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
			settings.NativeFileCopyMode = kCura.Relativity.DataReaderClient.NativeFileCopyModeEnum.CopyFiles;
			settings.NativeFilePathSourceFieldName = FieldFilePath;
			settings.OIFileIdColumnName = "OutsideInFileId";
			settings.OIFileIdMapped = true;
			settings.OIFileTypeColumnName = "OutsideInFileType";
			settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Append;
			settings.SelectedIdentifierFieldName = FieldControlNumber;
		}

		private void CatchJobEvents(kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job)
		{
			job.OnComplete += report =>
			{
				this.JobCompletedReport = report;
				Console.WriteLine("[Job Complete]");
			};

			job.OnError += row =>
			{
				this.ErrorEvents.Add(row);
			};

			job.OnFatalException += report =>
			{
				this.FatalExceptionEvent = report.FatalException;
				Console.WriteLine("[Job Fatal Exception]: " + report.FatalException);
			};

			job.OnMessage += status =>
			{
				this.MessageEvents.Add(status.Message);
				Console.WriteLine("[Job Message]: " + status.Message);
			};

			job.OnProcessProgress += status =>
			{
				this.ProcessProgressEvents.Add(status);
			};

			job.OnProgress += row =>
			{
				this.ProgressRowEvents.Add(row);
			};
		}
	}
}