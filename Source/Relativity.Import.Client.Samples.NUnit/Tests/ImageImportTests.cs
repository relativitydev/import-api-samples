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

	using global::NUnit.Framework;

	/// <summary>
	/// Represents a test that creates a new workspace, import images, validates the results, and deletes the workspace.
	/// </summary>
	[TestFixture]
	public class ImageImportTests : ImageImportTestsBase
	{
		private static IEnumerable<string> TestCases
		{
			get
			{
				yield return "EDRM-Sample1.tif";
				yield return "EDRM-Sample2.tif";
				yield return "EDRM-Sample3.tif";
			}
		}

		[Test]
		[TestCaseSource(nameof(TestCases))]
		public void ShouldImportTheImage(string fileName)
		{
			// Arrange
			kCura.Relativity.ImportAPI.ImportAPI importApi = CreateImportApiObject();
			kCura.Relativity.DataReaderClient.ImageImportBulkArtifactJob job = importApi.NewImageImportJob();
			this.ConfigureJobSettings(job);
			this.ConfigureJobEvents(job);
			string file = TestHelper.GetImagesResourceFilePath(fileName);
			this.DataSource.Columns.AddRange(new[]
			{
				new DataColumn(FieldBatesNumber, typeof(string)),
				new DataColumn(FieldControlNumber, typeof(string)),
				new DataColumn(FieldFileLocation, typeof(string))
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
			Assert.That(this.PublishedJobReport.TotalRows, Is.EqualTo(1));

			// Assert - the events match the expected values.
			Assert.That(this.PublishedErrors.Count, Is.Zero);
			Assert.That(this.PublishedFatalException, Is.Null);
			Assert.That(this.PublishedMessages.Count, Is.Positive);
			Assert.That(this.PublishedProcessProgress.Count, Is.Positive);
			Assert.That(this.PublishedProgressRows.Count, Is.Positive);

			// Assert - the object count is incremented by 1.
			int expectedDocCount = initialDocumentCount + this.DataSource.Rows.Count;
			int actualDocCount = this.QueryRelativityObjectCount((int)kCura.Relativity.Client.ArtifactType.Document);
			Assert.That(actualDocCount, Is.EqualTo(expectedDocCount));

			// Assert - the imported document exists.
			IList<Relativity.Services.Objects.DataContracts.RelativityObject> docs =
				this.QueryRelativityObjects(this.ArtifactTypeId, new[] { FieldControlNumber });
			Assert.That(docs, Is.Not.Null);
			Assert.That(docs.Count, Is.EqualTo(expectedDocCount));
		}
	}
}