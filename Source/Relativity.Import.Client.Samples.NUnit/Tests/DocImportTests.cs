// ----------------------------------------------------------------------------
// <copyright file="DocImportTests.cs" company="Relativity ODA LLC">
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
	/// Represents a test that creates a new workspace, import documents, validates the results, and deletes the workspace.
	/// </summary>
	[TestFixture]
	public class DocImportTests : DocImportTestsBase
	{
		private static IEnumerable<string> TestCases
		{
			get
			{
				yield return "EDRM-Sample1.pdf";
				yield return "EDRM-Sample2.doc";
				yield return "EDRM-Sample3.xlsx";
			}
		}

		[Test]
		[TestCaseSource(nameof(TestCases))]
		public void ShouldImportTheDoc(string fileName)
		{
			// Arrange
			kCura.Relativity.ImportAPI.ImportAPI importApi = CreateImportApiObject();
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job = importApi.NewNativeDocumentImportJob();
			this.ConfigureJobSettings(
				job,
				this.ArtifactTypeId,
				this.IdentifierFieldId,
				FieldFilePath,
				FieldControlNumber);
			this.ConfigureJobEvents(job);
			string file = TestHelper.GetResourceFilePath("Docs", fileName);
			this.DataTable.Columns.AddRange(new[]
			{
				new DataColumn(FieldControlNumber, typeof(string)),
				new DataColumn(FieldFilePath, typeof(string))
			});

			string controlNumber = "REL-" + Guid.NewGuid();
			this.DataTable.Rows.Add(controlNumber, file);
			job.SourceData.SourceData = this.DataTable.CreateDataReader();
			int initialDocumentCount = this.QueryRelativityObjectCount((int) ArtifactType.Document);

			// Act
			job.Execute();

			// Assert - the job completed and the report matches the expected values.
			Assert.That(this.JobCompletedReport, Is.Not.Null);
			Assert.That(this.JobCompletedReport.EndTime, Is.GreaterThan(this.JobCompletedReport.StartTime));
			Assert.That(this.JobCompletedReport.ErrorRowCount, Is.Zero);
			Assert.That(this.JobCompletedReport.FileBytes, Is.Positive);
			Assert.That(this.JobCompletedReport.MetadataBytes, Is.Positive);
			Assert.That(this.JobCompletedReport.StartTime, Is.GreaterThan(this.ImportStartTime));
			Assert.That(this.JobCompletedReport.TotalRows, Is.EqualTo(1));

			// Assert - the events match the expected values.
			Assert.That(this.ErrorEvents.Count, Is.Zero);
			Assert.That(this.FatalExceptionEvent, Is.Null);
			Assert.That(this.MessageEvents.Count, Is.Positive);
			Assert.That(this.ProcessProgressEvents.Count, Is.Positive);
			Assert.That(this.ProgressRowEvents.Count, Is.Positive);

			// Assert - the object count is incremented by 1.
			int expectedDocCount = initialDocumentCount + this.DataTable.Rows.Count;
			int actualDocCount = this.QueryRelativityObjectCount((int)ArtifactType.Document);
			Assert.That(actualDocCount, Is.EqualTo(expectedDocCount));

			// Assert - the imported document exists.
			IList<Relativity.Services.Objects.DataContracts.RelativityObject> docs = 
				this.QueryRelativityObjects(this.ArtifactTypeId, new[] { FieldControlNumber });
			Assert.That(docs, Is.Not.Null);
			Assert.That(docs.Count, Is.EqualTo(expectedDocCount));
			Relativity.Services.Objects.DataContracts.RelativityObject importedObj
				= FindRelativityObject(docs, FieldControlNumber, controlNumber);
			Assert.That(importedObj, Is.Not.Null);
		}
	}
}