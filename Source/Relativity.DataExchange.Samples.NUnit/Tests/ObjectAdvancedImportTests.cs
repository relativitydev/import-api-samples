// ----------------------------------------------------------------------------
// <copyright file="ObjectAdvancedImportTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Samples.NUnit.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using global::NUnit.Framework;

	using Relativity.DataExchange.TestFramework;

	/// <summary>
	/// Represents a test that imports advanced objects containing single/multi-object fields and validates the results.
	/// </summary>
	[TestFixture]
	public class ObjectAdvancedImportTests : ObjectImportTestsBase
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
					new TestCaseData("Advanced-Transfer-Small-1", "Advanced-Detail-1", "Advanced-DataSourceName-1", true),
					new TestCaseData("Advanced-Transfer-Small-2", "Advanced-Detail-2", "Advanced-DataSourceName-2", false),
					new TestCaseData("Advanced-Transfer-Medium-1", "Advanced-Detail-3", "Advanced-DataSourceName-3", true),
					new TestCaseData("Advanced-Transfer-Medium-2", "Advanced-Detail-4", "Advanced-DataSourceName-4", false),
					new TestCaseData("Advanced-Transfer-Large-1", "Advanced-Detail-5", "Advanced-DataSourceName-5", true),
					new TestCaseData("Advanced-Transfer-Large-2", "Advanced-Detail-6", "Advanced-DataSourceName-6", false),
				};

		[Test]
		[Category(TestCategories.ImportObject)]
		[Category(TestCategories.Integration)]
		[TestCaseSource(nameof(TestCases))]
		public void ShouldImportTheObject(
			string name,
			string detailName,
			string dataSourceName,
			bool importAssociatedObj)
		{
			// Arrange
			if (!importAssociatedObj)
			{
				// This verifies import does NOT fail when the associated object already exists.
				this.CreateAssociatedDetailInstance(detailName);
				this.CreateAssociatedDataSourceInstance(dataSourceName);
			}

			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job = this.CreateImportBulkArtifactJob();
			int initialObjectCount = this.QueryRelativityObjectCount(this.TransferArtifactTypeId);
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
			Assert.That(this.PublishedJobReport.ErrorRowCount, Is.Zero);
			Assert.That(this.PublishedJobReport.FileBytes, Is.Zero);
			Assert.That(this.PublishedJobReport.MetadataBytes, Is.Positive);
			Assert.That(this.PublishedJobReport.StartTime, Is.GreaterThan(this.StartTime));
			Assert.That(this.PublishedJobReport.TotalRows, Is.EqualTo(1));

			// Assert - the events match the expected values.
			Assert.That(this.PublishedErrors.Count, Is.Zero);
			Assert.That(this.PublishedFatalException, Is.Null);
			Assert.That(this.PublishedMessages.Count, Is.Positive);
			Assert.That(this.PublishedProcessProgress.Count, Is.Positive);
			Assert.That(this.PublishedProgressRows.Count, Is.Positive);

			// Assert - the object count is incremented by 1.
			int expectedObjectCount = initialObjectCount + this.DataSource.Rows.Count;
			IList<Relativity.Services.Objects.DataContracts.RelativityObject> transfers =
				this.QueryRelativityObjects(
					this.TransferArtifactTypeId,
					TransferFields);
			Assert.That(transfers, Is.Not.Null);
			Assert.That(transfers.Count, Is.EqualTo(expectedObjectCount));

			// Assert - the imported object exists.
			Relativity.Services.Objects.DataContracts.RelativityObject importedTransfers
				= SearchRelativityObject(transfers, TransferFieldName, name);
			Assert.That(importedTransfers, Is.Not.Null);

			// Assert - all standard field values matches the expected values.
			string descriptionFieldValue = GetStringFieldValue(importedTransfers, TransferFieldDescription);
			Assert.That(descriptionFieldValue, Is.EqualTo(description));
			decimal requestBytesFieldValue = GetDecimalFieldValue(importedTransfers, TransferFieldRequestBytes);
			Assert.That(requestBytesFieldValue, Is.EqualTo(requestBytes));
			decimal requestFilesFieldValue = GetDecimalFieldValue(importedTransfers, TransferFieldRequestFiles);
			Assert.That(requestFilesFieldValue, Is.EqualTo(requestFiles));
			DateTime requestDateFieldValue = GetDateFieldValue(importedTransfers, TransferFieldRequestDate);
			Assert.That(requestDateFieldValue, Is.EqualTo(requestDate).Within(5).Seconds);
			Relativity.Services.Objects.DataContracts.RelativityObjectValue transferDetailFieldValue
				= GetSingleObjectFieldValue(importedTransfers, TransferFieldDetailId);
			Assert.That(transferDetailFieldValue, Is.Not.Null);
			Assert.That(transferDetailFieldValue.ArtifactID, Is.Positive);

			// Assert - the associated single-object field is imported.
			Relativity.Services.Objects.DataContracts.RelativityObject transferDetail =
				this.ReadRelativityObject(
					transferDetailFieldValue.ArtifactID,
					TransferDetailFields);
			Assert.That(transferDetail, Is.Not.Null);
			string transferDetailNameFieldValue = GetStringFieldValue(transferDetail, TransferDetailFieldName);
			Assert.That(transferDetailNameFieldValue, Is.EqualTo(detailName));

			// Assert - the associated multi-object field is imported.
			List<Relativity.Services.Objects.DataContracts.RelativityObjectValue> transferDataSourceFieldValues
				= GetMultiObjectFieldValues(importedTransfers, TransferFieldDataSourceId);
			Assert.That(transferDataSourceFieldValues, Is.Not.Null);
			Assert.That(transferDataSourceFieldValues.Count, Is.EqualTo(1));
			Relativity.Services.Objects.DataContracts.RelativityObjectValue transferDataSourceFieldValue
				= transferDataSourceFieldValues.Single();
			Relativity.Services.Objects.DataContracts.RelativityObject transferDataSource =
				this.ReadRelativityObject(
					transferDataSourceFieldValue.ArtifactID,
					TransferDataSourceFields);
			Assert.That(transferDataSource, Is.Not.Null);
			string transferDataSourceNameFieldValue =
				GetStringFieldValue(transferDataSource, TransferDataSourceFieldName);
			Assert.That(transferDataSourceNameFieldValue, Is.EqualTo(dataSourceName));
		}
	}
}