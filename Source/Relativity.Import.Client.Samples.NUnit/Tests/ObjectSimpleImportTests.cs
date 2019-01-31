// ----------------------------------------------------------------------------
// <copyright file="ObjectSimpleImportTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.Sample.NUnit.Tests
{
	using System;
	using System.Collections.Generic;

	using global::NUnit.Framework;

	/// <summary>
	/// Represents a test that imports simple objects and validates the results.
	/// </summary>
	[TestFixture]
	public class ObjectSimpleImportTests : ObjectImportTestsBase
	{
		private static IEnumerable<TestCaseData> TestCases
		{
			get
			{
				yield return new TestCaseData("Simple-Transfer-Small-1");
				yield return new TestCaseData("Simple-Transfer-Small-2");
				yield return new TestCaseData("Simple-Transfer-Medium-1");
				yield return new TestCaseData("Simple-Transfer-Medium-2");
				yield return new TestCaseData("Simple-Transfer-Large-1");
				yield return new TestCaseData("Simple-Transfer-Large-2");
			}
		}

		[Test]
		[TestCaseSource(nameof(TestCases))]
		public void ShouldImportTheObject(string name)
		{
			// Arrange
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job = this.CreateImportBulkArtifactJob();
			int initialObjectCount = this.QueryRelativityObjectCount(this.TransferArtifactTypeId);
			string description = TestHelper.NextString(50, 450);
			decimal requestBytes = TestHelper.NextDecimal(10, 1000000);
			DateTime requestDate = DateTime.Now;
			decimal requestFiles = TestHelper.NextDecimal(1000, 10000);
			this.DataSource.Rows.Add(
				name,
				description,
				requestBytes,
				requestFiles,
				requestDate);
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
					new[]
					{
						TransferFieldName,
						TransferFieldDescription,
						TransferFieldRequestBytes,
						TransferFieldRequestFiles,
						TransferFieldRequestDate
					});
			Assert.That(transfers, Is.Not.Null);
			Assert.That(transfers.Count, Is.EqualTo(expectedObjectCount));

			// Assert - the imported object exists.
			Relativity.Services.Objects.DataContracts.RelativityObject importedTransfer
				= FindRelativityObject(transfers, TransferFieldName, name);
			Assert.That(importedTransfer, Is.Not.Null);

			// Assert - all standard field values matches the expected values.
			string descriptionFieldValue = FindStringFieldValue(importedTransfer, TransferFieldDescription);
			Assert.That(descriptionFieldValue, Is.EqualTo(description));
			decimal requestBytesFieldValue = FindDecimalFieldValue(importedTransfer, TransferFieldRequestBytes);
			Assert.That(requestBytesFieldValue, Is.EqualTo(requestBytes));
			decimal requestFilesFieldValue = FindDecimalFieldValue(importedTransfer, TransferFieldRequestFiles);
			Assert.That(requestFilesFieldValue, Is.EqualTo(requestFiles));
			DateTime requestDateFieldValue = FindDateFieldValue(importedTransfer, TransferFieldRequestDate);
			Assert.That(requestDateFieldValue, Is.EqualTo(requestDate).Within(5).Seconds);
		}
	}
}