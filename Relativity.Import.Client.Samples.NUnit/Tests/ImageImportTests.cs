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

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;

	using global::NUnit.Framework;

	/// <summary>
	/// Represents a test that creates a new workspace, import images, validates the results, and deletes the workspace.
	/// </summary>
	[TestFixture]
	public class ImageImportTests : ImportTestsBase
	{
		private const string ArtifactTypeName = "Document";
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
			this._artifactTypeId = this.GetArtifactTypeId(ArtifactTypeName);
			this._identifierFieldId = this.GetIdentifierFieldId(ArtifactTypeName);
		}

		[Test]
		[TestCaseSource(nameof(TestCases))]
		public void ShouldImportTheImage(string fileName)
		{
			// Arrange
			ImportAPI importApi = CreateImportApiObject();
			ImageImportBulkArtifactJob job = importApi.NewImageImportJob();
			this.ConfigureJobSettings(job);
			this.CatchJobEvents(job);
			string file = GetResourceFilePath("Images", fileName);
			this.DataTable.Columns.AddRange(new[]
			{
				new DataColumn("Bates Number", typeof(string)),
				new DataColumn("Control Number", typeof(string)),
				new DataColumn("File Location", typeof(string))
				
			});

			int initialDocumentCount = this.GetObjectCount((int)ArtifactType.Document);
			if (initialDocumentCount == 0)
			{
				// The Bates field for the first image in a set must be identical to the doc identifier.
				var identicalVal = $"FIRST-{Guid.NewGuid()}";
				this.DataTable.Rows.Add(identicalVal, identicalVal, file);
			}
			else
			{
				this.DataTable.Rows.Add($"BATES-{Guid.NewGuid()}", $"CONTROL-{Guid.NewGuid()}", file);
			}
			
			job.SourceData.SourceData = this.DataTable;

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

		private void ConfigureJobSettings(ImageImportBulkArtifactJob job)
		{
			ImageSettings settings = job.Settings;
			settings.ArtifactTypeId = this._artifactTypeId;
			settings.AutoNumberImages = true;
			settings.BatesNumberField = "Bates Number";
			settings.CaseArtifactId = TestSettings.WorkspaceId;
			settings.CopyFilesToDocumentRepository = true;
			settings.DisableImageLocationValidation = false;
			settings.DisableImageTypeValidation = false;
			settings.DocumentIdentifierField = "Control Number";
			settings.ExtractedTextEncoding = Encoding.Unicode;
			settings.ExtractedTextFieldContainsFilePath = false;
			settings.FileLocationField = "File Location";
			settings.IdentityFieldId = this._identifierFieldId;
			settings.LoadImportedFullTextFromServer = false;
			settings.NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles;
			settings.OverwriteMode = OverwriteModeEnum.Append;
			settings.SelectedIdentifierFieldName = "Control Number";
		}

		private void CatchJobEvents(ImageImportBulkArtifactJob job)
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