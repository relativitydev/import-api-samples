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
		private const string Level1Folder = "doc-import-root1";
		private const string Level2Folder = "doc-import-root2";
		private const string Level3Folder = "doc-import-root3";

		private static IEnumerable<TestCaseData> TestCases
		{
			get
			{
				// Ensure that duplicate folders never cause failures.
				yield return new TestCaseData(SamplePdfFileName, $"\\{Level1Folder}");
				yield return new TestCaseData(SampleWordFileName, $"\\{Level1Folder}");
				yield return new TestCaseData(SampleExcelFileName, $"\\{Level1Folder}");
				yield return new TestCaseData(SamplePdfFileName, $"\\{Level1Folder}\\{Level2Folder}");
				yield return new TestCaseData(SampleWordFileName, $"\\{Level1Folder}\\{Level2Folder}");
				yield return new TestCaseData(SampleExcelFileName, $"\\{Level1Folder}\\{Level2Folder}");
				yield return new TestCaseData(SamplePdfFileName, $"\\{Level1Folder}\\{Level2Folder}\\{Level3Folder}");
				yield return new TestCaseData(SampleWordFileName, $"\\{Level1Folder}\\{Level2Folder}\\{Level3Folder}");
				yield return new TestCaseData(SampleExcelFileName, $"\\{Level1Folder}\\{Level2Folder}\\{Level3Folder}");
			}
		}

		[Test]
		[TestCaseSource(nameof(TestCases))]
		public void ShouldImportTheDoc(string fileName, string folder)
		{
			// Arrange
			int initialDocumentCount = this.QueryRelativityObjectCount((int)kCura.Relativity.Client.ArtifactType.Document);
			string controlNumber = "REL-" + Guid.NewGuid();
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
			this.DataTable.Columns.AddRange(new[]
			{
				new DataColumn(ControlNumberFieldName, typeof(string)),
				new DataColumn(FilePathFieldName, typeof(string)),
				new DataColumn(FolderFieldName, typeof(string))
			});

			// Add the file to the data source.
			string file = TestHelper.GetDocsResourceFilePath(fileName);
			this.DataTable.Rows.Add(controlNumber, file, folder);
			job.SourceData.SourceData = this.DataTable.CreateDataReader();

			// Act
			job.Execute();

			// Assert - the import job is successful.
			this.AssertImportSuccess();

			// Assert - the object count is incremented by 1.
			int expectedDocCount = initialDocumentCount + this.DataTable.Rows.Count;
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
			string[] folders = folder.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
			this.AssertDistinctFolders(folders);
		}
	}
}