// ----------------------------------------------------------------------------
// <copyright file="ObjectNegativeImportTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Samples.NUnit.Tests
{
	using System;
	using System.Collections.Generic;

	using global::NUnit.Framework;

	using Relativity.DataExchange.TestFramework;

	/// <summary>
	/// Represents tests that fails to import objects and validates the results.
	/// </summary>
	[TestFixture]
	public class ObjectNegativeImportTests : ObjectImportTestsBase
	{
		/// <summary>
		/// Gets the test case data.
		/// </summary>
		/// <value>
		/// The <see cref="TestCaseData"/> instances.
		/// </value>
		private static IEnumerable<TestCaseData> TestCases =>
			new List<TestCaseData>
				{
					new TestCaseData("Negative-Transfer-1", "Negative-Detail-1", "Negative-DataSourceName-1"),
				};

		[Test]
		[Category(TestCategories.ImportObject)]
		[Category(TestCategories.Integration)]
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
			string description = RandomHelper.NextString(50, 450);
			decimal requestBytes = RandomHelper.NextDecimal(10, 1000000);
			DateTime requestDate = DateTime.Now;
			decimal requestFiles = RandomHelper.NextDecimal(1000, 10000);
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
		[Category(TestCategories.ImportObject)]
		[Category(TestCategories.Integration)]
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
			string description = RandomHelper.NextString(50, 450);
			decimal requestBytes = RandomHelper.NextDecimal(10, 1000000);
			DateTime requestDate = DateTime.Now;
			decimal requestFiles = RandomHelper.NextDecimal(1000, 10000);
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