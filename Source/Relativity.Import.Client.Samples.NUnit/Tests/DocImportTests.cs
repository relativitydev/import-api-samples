// ----------------------------------------------------------------------------
// <copyright file="DocImportTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.Samples.NUnit.Tests
{
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;

	using global::NUnit.Framework;

	using Relativity.Import.Export.Services;
	using Relativity.Import.Export.TestFramework;

	/// <summary>
	/// Represents a test that imports native documents and validates the results.
	/// </summary>
	[TestFixture]
	public class DocImportTests : DocImportTestsBase
	{
		/// <summary>
		/// Gets the test case data.
		/// </summary>
		/// <value>
		/// The <see cref="TestCaseData"/> instances.
		/// </value>
		/// <remarks>
		/// Ensure that duplicate folders never cause failures.
		/// </remarks>
		private static IEnumerable<TestCaseData> TestCases =>
			new List<TestCaseData>
				{
					new TestCaseData(SampleDocPdfFileName, null),
					new TestCaseData(SampleDocWordFileName, string.Empty),
					new TestCaseData(SampleDocExcelFileName, "\\doc-import-root1"),
					new TestCaseData(SampleDocMsgFileName, "\\doc-import-root1"),
					new TestCaseData(SampleDocHtmFileName, "\\doc-import-root1\\doc-import-root2"),
					new TestCaseData(SampleDocEmfFileName, "\\doc-import-root1\\doc-import-root2"),
					new TestCaseData(SampleDocPptFileName, "\\doc-import-root1\\doc-import-root2\\doc-import-root3"),
					new TestCaseData(SampleDocPngFileName, "\\doc-import-root1\\doc-import-root2\\doc-import-root3"),
					new TestCaseData(SampleDocTxtFileName, "\\doc-import-root1\\doc-import-root2\\doc-import-root3\\doc-import-root4"),
					new TestCaseData(SampleDocWmfFileName, "\\doc-import-root1\\doc-import-root2\\doc-import-root3\\doc-import-root4"),
				};

		[Test]
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[TestCaseSource(nameof(TestCases))]
		public void ShouldImportTheDoc(string fileName, string folderPath)
		{
			// Arrange
			int initialDocumentCount = this.QueryRelativityObjectCount((int)kCura.Relativity.Client.ArtifactType.Document);
			string controlNumber = GenerateControlNumber();
			kCura.Relativity.ImportAPI.ImportAPI importApi = this.CreateImportApiObject();
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job = importApi.NewNativeDocumentImportJob();
			this.ConfigureJobSettings(
				job,
				this.ArtifactTypeId,
				this.IdentifierFieldId,
				WellKnownFields.FilePath,
				WellKnownFields.ControlNumber,
				WellKnownFields.FolderName);
			this.ConfigureJobEvents(job);

			// Setup the data source.
			this.DataSource.Columns.AddRange(new[]
			{
				new DataColumn(WellKnownFields.ControlNumber, typeof(string)),
				new DataColumn(WellKnownFields.FilePath, typeof(string)),
				new DataColumn(WellKnownFields.FolderName, typeof(string)),
			});

			// Add the file to the data source.
			string file = ResourceFileHelper.GetDocsResourceFilePath(fileName);
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
			IList<Relativity.Services.Objects.DataContracts.RelativityObject> docs = this.QueryDocuments();
			Assert.That(docs, Is.Not.Null);
			Assert.That(docs.Count, Is.EqualTo(expectedDocCount));
			Relativity.Services.Objects.DataContracts.RelativityObject importedObj
				= SearchRelativityObject(docs, WellKnownFields.ControlNumber, controlNumber);
			Assert.That(importedObj, Is.Not.Null);

			// Assert - the workspace doesn't include duplicate folders.
			if (!string.IsNullOrEmpty(folderPath))
			{
				IEnumerable<string> folders = SplitFolderPath(folderPath);
				this.AssertDistinctFolders(folders.ToArray());
			}

			// Assert the field values match the expected values.
			Relativity.Services.Objects.DataContracts.RelativityObject document = SearchRelativityObject(
				docs,
				WellKnownFields.ControlNumber,
				controlNumber);
			Assert.That(document, Is.Not.Null);
			Relativity.Services.Objects.DataContracts.Choice hasImagesField = GetChoiceField(document, WellKnownFields.HasImages);
			Assert.That(hasImagesField, Is.Not.Null);
			Assert.That(hasImagesField.Name, Is.Not.Null);
			Assert.That(hasImagesField.Name, Is.EqualTo("No"));
			bool hasNativeField = GetBooleanFieldValue(document, WellKnownFields.HasNative);
			Assert.That(hasNativeField, Is.True);
			int? relativityImageCount = GetInt32FieldValue(document, WellKnownFields.RelativityImageCount);
			Assert.That(relativityImageCount, Is.Null);

			// Assert that importing adds a file record and all properties match the expected values.
			FileDto documentFile = this.QueryNativeFileInfo(document.ArtifactID);
			Assert.That(documentFile, Is.Not.Null);
			Assert.That(documentFile.DocumentArtifactId, Is.EqualTo(document.ArtifactID));
			Assert.That(documentFile.FileId, Is.Positive);
			Assert.That(documentFile.FileName, Is.EqualTo(fileName));
			Assert.That(documentFile.FileType, Is.EqualTo((int)FileType.Native));
			Assert.That(documentFile.Identifier, Is.Not.Null.Or.Empty);
			Assert.That(documentFile.InRepository, Is.True);
			Assert.That(documentFile.Path, Is.Not.Null.Or.Empty);
			Assert.That(documentFile.Size, Is.Positive);
		}
	}
}