// ----------------------------------------------------------------------------
// <copyright file="ObjectNegativeImportTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.Sample.NUnit.Tests
{
	using System;
	using System.Collections.Generic;

	using global::NUnit.Framework;

	/// <summary>
	/// Represents tests that create a new workspace, fail to import objects, validates the results, and deletes the workspace.
	/// </summary>
	[TestFixture]
	public class ObjectNegativeImportTests : ObjectImportTestsBase
	{
		private static IEnumerable<TestCaseData> TestCases
		{
			get
			{
				yield return new TestCaseData("Negative-Transfer-1", "Negative-Detail-1", "Negative-DataSourceName-1");
			}
		}

		[Test]
		[TestCaseSource(nameof(TestCases))]
		public void ShouldNotImportDuplicateSingleObjectFields(
			string name,
			string detailName,
			string dataSourceName)
		{
			// Arrange
			this.CreateAssociatedDetailInstance(detailName);
			this.CreateAssociatedDetailInstance(detailName);
			this.CreateAssociatedDataSourceInstance(dataSourceName);
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job = this.CreateImportBulkArtifactJob();
			string description = TestHelper.NextString(50, 450);
			decimal requestBytes = TestHelper.NextDecimal(10, 1000000);
			DateTime requestDate = DateTime.Now;
			decimal requestFiles = TestHelper.NextDecimal(1000, 10000);
			this.DataSource.Rows.Add(
				name,
				description,
				requestBytes,
				requestFiles,
				requestDate,
				detailName,
				dataSourceName);
			job.SourceData.SourceData = this.DataSource.CreateDataReader();

			// Act
			job.Execute();

			// Assert - the job completed and the report matches the expected values.
			Assert.That(this.PublishedJobReport, Is.Not.Null);
			Assert.That(this.PublishedJobReport.EndTime, Is.GreaterThan(this.PublishedJobReport.StartTime));

			// Assert - duplicate single-object field yields a job-level error.
			Assert.That(this.PublishedJobReport.ErrorRowCount, Is.Positive);
			Assert.That(this.PublishedJobReport.FatalException, Is.Null);
			Assert.That(this.PublishedJobReport.TotalRows, Is.EqualTo(1));

			// Assert - the events match the expected values.
			Assert.That(this.PublishedErrors.Count, Is.Positive);
			Assert.That(this.PublishedFatalException, Is.Null);
			Assert.That(this.PublishedMessages.Count, Is.Positive);
			Assert.That(this.PublishedProcessProgress.Count, Is.Positive);
			Assert.That(this.PublishedProgressRows.Count, Is.Positive);
		}

		[Test]
		[TestCaseSource(nameof(TestCases))]
		public void ShouldNotImportDuplicateMultiObjectFields(
			string name,
			string detailName,
			string dataSourceName)
		{
			// Arrange
			this.CreateAssociatedDetailInstance(detailName);
			this.CreateAssociatedDataSourceInstance(dataSourceName);
			this.CreateAssociatedDataSourceInstance(dataSourceName);
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job = this.CreateImportBulkArtifactJob();
			string description = TestHelper.NextString(50, 450);
			decimal requestBytes = TestHelper.NextDecimal(10, 1000000);
			DateTime requestDate = DateTime.Now;
			decimal requestFiles = TestHelper.NextDecimal(1000, 10000);
			this.DataSource.Rows.Add(
				name,
				description,
				requestBytes,
				requestFiles,
				requestDate,
				detailName,
				dataSourceName);
			job.SourceData.SourceData = this.DataSource.CreateDataReader();

			// Act
			job.Execute();

			// Assert - the job completed and the report matches the expected values.
			Assert.That(this.PublishedJobReport, Is.Not.Null);
			Assert.That(this.PublishedJobReport.EndTime, Is.GreaterThan(this.PublishedJobReport.StartTime));

			// Assert - duplicate multi-object field currently yields a fatal error.
			Assert.That(this.PublishedJobReport.ErrorRowCount, Is.Zero);
			Assert.That(this.PublishedJobReport.FatalException, Is.Not.Null);
			Assert.That(this.PublishedJobReport.TotalRows, Is.EqualTo(1));

			// Assert - the events match the expected values.
			Assert.That(this.PublishedErrors.Count, Is.Zero);
			Assert.That(this.PublishedFatalException, Is.Not.Null);
			Assert.That(this.PublishedMessages.Count, Is.Positive);
			Assert.That(this.PublishedProcessProgress.Count, Is.Positive);
			Assert.That(this.PublishedProgressRows.Count, Is.Positive);
		}
	}
}