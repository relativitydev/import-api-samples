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
			this.DataTable.Rows.Add(
				name,
				description,
				requestBytes,
				requestFiles,
				requestDate,
				detailName,
				dataSourceName);
			job.SourceData.SourceData = this.DataTable.CreateDataReader();

			// Act
			job.Execute();

			// Assert - the job completed and the report matches the expected values.
			Assert.That(this.JobCompletedReport, Is.Not.Null);
			Assert.That(this.JobCompletedReport.EndTime, Is.GreaterThan(this.JobCompletedReport.StartTime));

			// Assert - duplicate single-object field yields a job-level error.
			Assert.That(this.JobCompletedReport.ErrorRowCount, Is.Positive);
			Assert.That(this.JobCompletedReport.FatalException, Is.Null);
			Assert.That(this.JobCompletedReport.TotalRows, Is.EqualTo(1));

			// Assert - the events match the expected values.
			Assert.That(this.ErrorEvents.Count, Is.Positive);
			Assert.That(this.FatalExceptionEvent, Is.Null);
			Assert.That(this.MessageEvents.Count, Is.Positive);
			Assert.That(this.ProcessProgressEvents.Count, Is.Positive);
			Assert.That(this.ProgressRowEvents.Count, Is.Positive);
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
			this.DataTable.Rows.Add(
				name,
				description,
				requestBytes,
				requestFiles,
				requestDate,
				detailName,
				dataSourceName);
			job.SourceData.SourceData = this.DataTable.CreateDataReader();

			// Act
			job.Execute();

			// Assert - the job completed and the report matches the expected values.
			Assert.That(this.JobCompletedReport, Is.Not.Null);
			Assert.That(this.JobCompletedReport.EndTime, Is.GreaterThan(this.JobCompletedReport.StartTime));

			// Assert - duplicate multi-object field currently yields a fatal error.
			Assert.That(this.JobCompletedReport.ErrorRowCount, Is.Zero);
			Assert.That(this.JobCompletedReport.FatalException, Is.Not.Null);
			Assert.That(this.JobCompletedReport.TotalRows, Is.EqualTo(1));

			// Assert - the events match the expected values.
			Assert.That(this.ErrorEvents.Count, Is.Zero);
			Assert.That(this.FatalExceptionEvent, Is.Not.Null);
			Assert.That(this.MessageEvents.Count, Is.Positive);
			Assert.That(this.ProcessProgressEvents.Count, Is.Positive);
			Assert.That(this.ProgressRowEvents.Count, Is.Positive);
		}
	}
}