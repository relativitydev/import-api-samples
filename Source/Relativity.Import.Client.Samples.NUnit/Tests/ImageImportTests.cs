// ----------------------------------------------------------------------------
// <copyright file="ImageImportTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Samples.NUnit.Tests
{
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.TestFramework;

	/// <summary>
	/// Represents a test that imports images and validates the results.
	/// </summary>
	[TestFixture]
	public class ImageImportTests : ImageImportTestsBase
	{
		[Test]
		[Category(TestCategories.ImportImage)]
		[Category(TestCategories.Integration)]
		[TestCaseSource(nameof(AllSampleImageFileNames))]
		public void ShouldImportTheImage(string fileName)
		{
			// Arrange
			kCura.Relativity.ImportAPI.ImportAPI importApi = this.CreateImportApiObject();
			kCura.Relativity.DataReaderClient.ImageImportBulkArtifactJob job = importApi.NewImageImportJob();
			this.ConfigureJobSettings(job);
			this.ConfigureJobEvents(job);
			string file = ResourceFileHelper.GetImagesResourceFilePath(fileName);
			this.DataSource.Columns.AddRange(new[]
			{
				new DataColumn(WellKnownFields.BatesNumber, typeof(string)),
				new DataColumn(WellKnownFields.ControlNumber, typeof(string)),
				new DataColumn(WellKnownFields.FileLocation, typeof(string)),
			});

			int initialDocumentCount = this.QueryRelativityObjectCount((int)kCura.Relativity.Client.ArtifactType.Document);
			string batesNumber = GenerateBatesNumber();
			string controlNumber = GenerateControlNumber();
			if (initialDocumentCount == 0)
			{
				// The Bates field for the first image in a set must be identical to the doc identifier.
				batesNumber = controlNumber;
			}

			this.DataSource.Rows.Add(batesNumber, controlNumber, file);
			job.SourceData.SourceData = this.DataSource;

			// Act
			job.Execute();

			// Assert - the job completed and the report matches the expected values.
			Assert.That(this.PublishedJobReport, Is.Not.Null);
			Assert.That(this.PublishedJobReport.EndTime, Is.GreaterThan(this.PublishedJobReport.StartTime));
			Assert.That(this.PublishedJobReport.ErrorRowCount, Is.Zero);

			// Note: Importing images does NOT currently update FileBytes/MetadataBytes.
			Assert.That(this.PublishedJobReport.FileBytes, Is.Zero);
			Assert.That(this.PublishedJobReport.MetadataBytes, Is.Zero);
			Assert.That(this.PublishedJobReport.StartTime, Is.GreaterThan(this.StartTime));
			Assert.That(this.PublishedJobReport.TotalRows, Is.EqualTo(this.DataSource.Rows.Count));

			// Assert - the events match the expected values.
			Assert.That(this.PublishedErrors.Count, Is.Zero);
			Assert.That(this.PublishedFatalException, Is.Null);
			Assert.That(this.PublishedMessages.Count, Is.Positive);
			Assert.That(this.PublishedProcessProgress.Count, Is.Positive);
			Assert.That(this.PublishedProgressRows.Count, Is.Positive);

			// Assert - the object count is incremented by the number of imported documents.
			int expectedDocCount = initialDocumentCount + this.DataSource.Rows.Count;
			int actualDocCount = this.QueryRelativityObjectCount((int)kCura.Relativity.Client.ArtifactType.Document);
			Assert.That(actualDocCount, Is.EqualTo(expectedDocCount));

			// Assert - the imported document exists.
			IList<Relativity.Services.Objects.DataContracts.RelativityObject> docs = this.QueryDocuments();
			Assert.That(docs, Is.Not.Null);
			Assert.That(docs.Count, Is.EqualTo(expectedDocCount));

			// Assert the field values match the expected values.
			Relativity.Services.Objects.DataContracts.RelativityObject document =
				SearchRelativityObject(docs, WellKnownFields.ControlNumber, controlNumber) ?? SearchRelativityObject(
					docs,
					WellKnownFields.ControlNumber,
					batesNumber);
			Assert.That(document, Is.Not.Null);
			Relativity.Services.Objects.DataContracts.Choice hasImagesField = GetChoiceField(document, WellKnownFields.HasImages);
			Assert.That(hasImagesField, Is.Not.Null);
			Assert.That(hasImagesField.Name, Is.Not.Null);
			Assert.That(hasImagesField.Name, Is.EqualTo("Yes"));
			bool hasNativeField = GetBooleanFieldValue(document, WellKnownFields.HasNative);
			Assert.That(hasNativeField, Is.False);
			int? relativityImageCount = GetInt32FieldValue(document, WellKnownFields.RelativityImageCount);
			Assert.That(relativityImageCount, Is.Positive);

			// Assert that importing adds a file record and all properties match the expected values.
			IList<FileDto> documentImages = this.QueryImageFileInfo(document.ArtifactID).ToList();
			Assert.That(documentImages, Is.Not.Null);
			Assert.That(documentImages.Count, Is.EqualTo(1));
			FileDto imageFile = documentImages[0];
			Assert.That(imageFile.DocumentArtifactId, Is.EqualTo(document.ArtifactID));
			Assert.That(imageFile.FileId, Is.Positive);
			Assert.That(imageFile.FileName, Is.EqualTo(fileName));
			Assert.That(imageFile.FileType, Is.EqualTo((int)FileType.Tif));
			Assert.That(imageFile.Identifier, Is.EqualTo(controlNumber).Or.EqualTo(batesNumber));
			Assert.That(imageFile.InRepository, Is.True);
			Assert.That(imageFile.Path, Is.Not.Null.Or.Empty);
			Assert.That(imageFile.Size, Is.Positive);
		}
	}
}