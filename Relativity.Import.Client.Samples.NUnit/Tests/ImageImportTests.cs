// ----------------------------------------------------------------------------
// <copyright file="ImageImportTests.cs" company="Relativity ODA LLC">
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
	/// Represents a test that creates a new workspace, import images, validates the results, and deletes the workspace.
	/// </summary>
	[TestFixture]
	public class ImageImportTests : ImportTestsBase
	{
		private const string ArtifactTypeName = "Document";
		private const string FieldBatesNumber = "Bates Number";
		private const string FieldControlNumber = "Control Number";
		private const string FieldFileLocation = "File Location";
		private int _artifactTypeId;
		private int _identifierFieldId;

		private static IEnumerable<string> TestCases
		{
			get
			{
				yield return "EDRM-Sample1.tif";
				yield return "EDRM-Sample2.tif";
				yield return "EDRM-Sample3.tif";
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
		public void ShouldImportTheImage(string fileName)
		{
			// Arrange
			kCura.Relativity.ImportAPI.ImportAPI importApi = CreateImportApiObject();
			kCura.Relativity.DataReaderClient.ImageImportBulkArtifactJob job = importApi.NewImageImportJob();
			this.ConfigureJobSettings(job);
			this.CatchJobEvents(job);
			string file = TestHelper.GetResourceFilePath("Images", fileName);
			this.DataTable.Columns.AddRange(new[]
			{
				new DataColumn(FieldBatesNumber, typeof(string)),
				new DataColumn(FieldControlNumber, typeof(string)),
				new DataColumn(FieldFileLocation, typeof(string))
			});

			int initialDocumentCount = this.QueryRelativityObjectCount((int)ArtifactType.Document);
			string batesNumber = $"BATES-{Guid.NewGuid()}";
			string controlNumber = "REL-" + Guid.NewGuid();
			if (initialDocumentCount == 0)
			{
				// The Bates field for the first image in a set must be identical to the doc identifier.
				batesNumber = controlNumber;
			}

			this.DataTable.Rows.Add(batesNumber, controlNumber, file);
			job.SourceData.SourceData = this.DataTable;

			// Act
			job.Execute();

			// Assert - the job completed and the report matches the expected values.
			Assert.That(this.JobCompletedReport, Is.Not.Null);
			Assert.That(this.JobCompletedReport.EndTime, Is.GreaterThan(this.JobCompletedReport.StartTime));
			Assert.That(this.JobCompletedReport.ErrorRowCount, Is.Zero);

			// Note: Importing images does NOT currently update FileBytes/MetadataBytes.
			Assert.That(this.JobCompletedReport.FileBytes, Is.EqualTo(0));
			Assert.That(this.JobCompletedReport.MetadataBytes, Is.EqualTo(0));
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
		}

		private void ConfigureJobSettings(kCura.Relativity.DataReaderClient.ImageImportBulkArtifactJob job)
		{
			kCura.Relativity.DataReaderClient.ImageSettings settings = job.Settings;
			settings.ArtifactTypeId = this._artifactTypeId;
			settings.AutoNumberImages = true;
			settings.BatesNumberField = FieldBatesNumber;
			settings.CaseArtifactId = TestSettings.WorkspaceId;
			settings.CopyFilesToDocumentRepository = true;
			settings.DisableImageLocationValidation = false;
			settings.DisableImageTypeValidation = false;
			settings.DocumentIdentifierField = FieldControlNumber;
			settings.ExtractedTextEncoding = Encoding.Unicode;
			settings.ExtractedTextFieldContainsFilePath = false;
			settings.FileLocationField = FieldFileLocation;
			settings.IdentityFieldId = this._identifierFieldId;
			settings.LoadImportedFullTextFromServer = false;
			settings.NativeFileCopyMode = kCura.Relativity.DataReaderClient.NativeFileCopyModeEnum.CopyFiles;
			settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Append;
			settings.SelectedIdentifierFieldName = FieldControlNumber;
		}

		private void CatchJobEvents(kCura.Relativity.DataReaderClient.ImageImportBulkArtifactJob job)
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