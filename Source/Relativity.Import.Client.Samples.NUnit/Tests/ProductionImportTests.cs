// ----------------------------------------------------------------------------
// <copyright file="ProductionImportTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.Sample.NUnit.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Data;
    using System.Linq;

    using global::NUnit.Framework;

	/// <summary>
	/// Represents a test that imports production images and validates the results.
	/// </summary>
	/// <remarks>
	/// This test requires the Relativity.Productions.Client package but hasn't yet been published to nuget.org.
	/// </remarks>
	[TestFixture]
	public class ProductionImportTests : ImageImportTestsBase
    {
        /// <summary>
        /// The first document control number.
        /// </summary>
        private const string FirstDocumentControlNumber = "EDRM-Sample-000001";

        /// <summary>
        /// The second document control number.
        /// </summary>
        private const string SecondDocumentControlNumber = "EDRM-Sample-001002";

        /// <summary>
        /// The total number of images for the first imported document.
        /// </summary>
        /// <remarks>
        /// The last digits in <see cref="SecondDocumentControlNumber"/> must be updated if this value is changed.
        /// </remarks>
        private const int TotalImagesForFirstDocument = 1001;

        [Test]
		public void ShouldImportTheProductionImages()
		{
            // Arrange
            IList<string> controlNumbers = new List<string> { FirstDocumentControlNumber, SecondDocumentControlNumber };
            this.ImportDocuments(controlNumbers);
            string productionSetName = GenerateProductionSetName();
            int productionId = this.CreateProduction(productionSetName, BatesPrefix);

            // Act
            this.ImportProduction(productionId);

            // Assert - the job completed and the report matches the expected values.
            Assert.That(this.PublishedJobReport, Is.Not.Null);
			Assert.That(this.PublishedJobReport.EndTime, Is.GreaterThan(this.PublishedJobReport.StartTime));
			Assert.That(this.PublishedJobReport.ErrorRowCount, Is.Zero);

			// Note: Importing images do NOT currently update FileBytes/MetadataBytes.
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

            // Assert - the first and last bates numbers match the expected values.
            Tuple<string, string> batesNumbers = this.QueryProductionBatesNumbers(productionId);
            string expectedFirstBatesValue = FirstDocumentControlNumber;
            string expectedLastBatesValue = SecondDocumentControlNumber;
            Assert.That(batesNumbers.Item1, Is.EqualTo(expectedFirstBatesValue));
            Assert.That(batesNumbers.Item2, Is.EqualTo(expectedLastBatesValue));
        }

        private void ImportProduction(int productionId)
        {
            kCura.Relativity.ImportAPI.ImportAPI importApi = CreateImportApiObject();
            IEnumerable<kCura.Relativity.ImportAPI.Data.ProductionSet> productionSets =
                importApi.GetProductionSets(TestSettings.WorkspaceId).ToList();
            Assert.That(productionSets.Count, Is.GreaterThan(0));
            kCura.Relativity.ImportAPI.Data.ProductionSet productionSet =
                productionSets.FirstOrDefault(x => x.ArtifactID == productionId);
            Assert.That(productionSet, Is.Not.Null);
            kCura.Relativity.DataReaderClient.ImageImportBulkArtifactJob job =
                importApi.NewProductionImportJob(productionSet.ArtifactID);
            this.ConfigureJobSettings(job);
            job.Settings.NativeFileCopyMode = kCura.Relativity.DataReaderClient.NativeFileCopyModeEnum.DoNotImportNativeFiles;
            this.ConfigureJobEvents(job);
            this.DataSource.Columns.AddRange(new[]
            {
                new DataColumn(this.IdentifierFieldName, typeof(string)),
                new DataColumn(BatesNumberFieldName, typeof(string)),
                new DataColumn(FileLocationFieldName, typeof(string))
            });

            DataRow row;
            for (int i = 1; i <= TotalImagesForFirstDocument; i++)
            {
                row = this.DataSource.NewRow();
                row[this.IdentifierFieldName] = FirstDocumentControlNumber;
                row[BatesNumberFieldName] = $"EDRM-Sample-{i:D6}";
                row[FileLocationFieldName] = TestHelper.GetImagesResourceFilePath(SampleProductionImage1FileName);
                this.DataSource.Rows.Add(row);
            }

            row = this.DataSource.NewRow();
            row[this.IdentifierFieldName] = SecondDocumentControlNumber;
            row[BatesNumberFieldName] = SecondDocumentControlNumber;
            row[FileLocationFieldName] = TestHelper.GetImagesResourceFilePath(SampleProductionImage1FileName);
            this.DataSource.Rows.Add(row);
            job.SourceData.SourceData = this.DataSource;
            job.Execute();
        }
	}
}