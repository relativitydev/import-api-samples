// ----------------------------------------------------------------------------
// <copyright file="DocImportTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.Sample.NUnit.Tests
{
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;

	using global::NUnit.Framework;

	/// <summary>
	/// Represents a test that creates a new workspace, import documents, validates the results, and deletes the workspace.
	/// </summary>
	[TestFixture]
	public class DocImportTests : DocImportTestsBase
	{
		private static IEnumerable<TestCaseData> TestCases
		{
			get
			{
				// Ensure that duplicate folders never cause failures.
				yield return new TestCaseData(SamplePdfFileName, null);
				yield return new TestCaseData(SampleWordFileName, string.Empty);
				yield return new TestCaseData(SampleExcelFileName, "\\doc-import-root1");
				yield return new TestCaseData(SampleMsgFileName, "\\doc-import-root1");
				yield return new TestCaseData(SampleHtmFileName, "\\doc-import-root1\\doc-import-root2");
				yield return new TestCaseData(SampleEmfFileName, "\\doc-import-root1\\doc-import-root2");
				yield return new TestCaseData(SamplePptFileName, "\\doc-import-root1\\doc-import-root2\\doc-import-root3");
				yield return new TestCaseData(SamplePngFileName, "\\doc-import-root1\\doc-import-root2\\doc-import-root3");
				yield return new TestCaseData(SampleTxtFileName, "\\doc-import-root1\\doc-import-root2\\doc-import-root3\\doc-import-root4");
				yield return new TestCaseData(SampleWmfFileName, "\\doc-import-root1\\doc-import-root2\\doc-import-root3\\doc-import-root4");
			}
		}

		[Test]
		[TestCaseSource(nameof(TestCases))]
		public void ShouldImportTheDoc(string fileName, string folderPath)
		{
			// Arrange
			int initialDocumentCount = this.QueryRelativityObjectCount((int)kCura.Relativity.Client.ArtifactType.Document);
			string controlNumber = GenerateControlNumber();
			kCura.Relativity.ImportAPI.ImportAPI importApi = CreateImportApiObject();
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job = importApi.NewNativeDocumentImportJob();
			this.ConfigureJobSettings(
				job,
				this.ArtifactTypeId,
				this.IdentifierFieldId,
				FilePathFieldName,
				ControlNumberFieldName,
				FolderFieldName);
			this.ConfigureJobEvents(job);

			// Setup the data source.
			this.DataSource.Columns.AddRange(new[]
			{
				new DataColumn(ControlNumberFieldName, typeof(string)),
				new DataColumn(FilePathFieldName, typeof(string)),
				new DataColumn(FolderFieldName, typeof(string))
			});

			// Add the file to the data source.
			string file = TestHelper.GetDocsResourceFilePath(fileName);
			this.DataSource.Rows.Add(controlNumber, file, folderPath);
			job.SourceData.SourceData = this.DataSource.CreateDataReader();

			// Act
			job.Execute();

			// Assert - the import job is successful.
			this.AssertImportSuccess();

			// Assert - the object count is incremented by 1.
			int expectedDocCount = initialDocumentCount + this.DataSource.Rows.Count;
			int actualDocCount = this.QueryRelativityObjectCount((int)kCura.Relativity.Client.ArtifactType.Document);
			Assert.That(actualDocCount, Is.EqualTo(expectedDocCount));

			// Assert - the imported document exists.
			IList<Relativity.Services.Objects.DataContracts.RelativityObject> docs = 
				this.QueryRelativityObjects(this.ArtifactTypeId, new[] { ControlNumberFieldName });
			Assert.That(docs, Is.Not.Null);
			Assert.That(docs.Count, Is.EqualTo(expectedDocCount));
			Relativity.Services.Objects.DataContracts.RelativityObject importedObj
				= FindRelativityObject(docs, ControlNumberFieldName, controlNumber);
			Assert.That(importedObj, Is.Not.Null);

			// Assert - the workspace doesn't include duplicate folders.
			if (!string.IsNullOrEmpty(folderPath))
			{
				IEnumerable<string> folders = SplitFolderPath(folderPath);
				this.AssertDistinctFolders(folders.ToArray());
			}
		}
	}
}