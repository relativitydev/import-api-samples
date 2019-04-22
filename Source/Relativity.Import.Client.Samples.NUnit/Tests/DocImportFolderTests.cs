// ----------------------------------------------------------------------------
// <copyright file="DocImportFolderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.Samples.NUnit.Tests
{
	using System.Collections.Generic;
	using System.Linq;

	using global::NUnit.Framework;

	using Relativity.Import.Export;
	using Relativity.Import.Export.TestFramework;

	/// <summary>
	/// Represents a test that imports native documents with folders and validates the results.
	/// </summary>
	/// <remarks>
	/// Due to poor performance, disabling client-side implementation by default.
	/// </remarks>
	[TestFixture(false)]
	[TestFixture(true)]
	public class DocImportFolderTests : DocImportTestsBase
	{
		/// <summary>
		/// The flag that indicates whether to create folders server-side or client-side.
		/// </summary>
		private readonly bool serverSideFolders;

		/// <summary>
		/// Initializes a new instance of the <see cref="DocImportFolderTests"/> class.
		/// </summary>
		/// <param name="serverSideFolders">
		/// <see langword="true" /> to create all folders via server-side WebPI; otherwise, <see langword="false" /> to create all folders via client-side API.
		/// </param>
		public DocImportFolderTests(bool serverSideFolders)
		{
			this.serverSideFolders = serverSideFolders;
		}

		[Test]
		[Category(TestCategories.Folder)]
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[TestCase("00-te/st")]
		[TestCase("01-te:st")]
		[TestCase("02-te?st")]
		[TestCase("03-te<st")]
		[TestCase("04-te>st")]
		[TestCase("05-te\"st")]
		[TestCase("06-te|st")]
		[TestCase("07-te*st")]
		public void ShouldImportTheDocWhenTheFolderContainsInvalidChars(string invalidFolder)
		{
			// Arrange
			IList<string> initialFolders = this.QueryWorkspaceFolders();
			string controlNumber = GenerateControlNumber();
			string folder = $"\\{invalidFolder}-{this.serverSideFolders}";
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job =
				this.ArrangeImportJob(controlNumber, folder, SampleDocPdfFileName);

			// Act
			job.Execute();

			// Assert - the invalid folders were scrubbed and the import job is successful.
			this.AssertImportSuccess();

			// Assert - a new folder is added to the workspace.
			int expectedDocCount = initialFolders.Count + 1;
			IList<string> actualFolders = this.QueryWorkspaceFolders();
			Assert.That(actualFolders.Count, Is.EqualTo(expectedDocCount));
		}

		[Test]
		[Category(TestCategories.Folder)]
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[TestCase("\\case-root1")]
		[TestCase("\\case-root1\\")]
		[TestCase("\\case-root1\\case-root2")]
		[TestCase("\\case-root1\\case-Root2")]
		[TestCase("\\case-ROOT1\\case-root2")]
		[TestCase("\\case-ROOT1\\case-Root2")]
		[TestCase("\\case-ROOT1\\case-ROOT2")]
		public void ShouldNotDuplicateFoldersDueToCaseSensitivity(string folder)
		{
			// Arrange
			string controlNumber = GenerateControlNumber();
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job =
				this.ArrangeImportJob(controlNumber, folder, SampleDocPdfFileName);

			// Act
			job.Execute();

			// Assert - the invalid folders were scrubbed and the import job is successful.
			this.AssertImportSuccess();

			// Assert - SQL collation is case-insensitive.
			int separators = folder.TrimEnd('\\').Count(x => x == '\\');
			if (separators == 1)
			{
				this.AssertDistinctFolders("case-root1");
			}
			else
			{
				this.AssertDistinctFolders("case-root1", "case-root2");
			}
		}

		[Test]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.ImportDoc)]
		[TestCase(10)]
		[TestCase(25)]
		[TestCase(50)]
		public void ShouldSupportTheMaxFolderDepth(int maxDepth)
		{
			// Arrange
			string folderPath = GenerateFolderPath(maxDepth);
			List<DocImportRecord> records = AllSampleDocFileNames.Select(fileName => new DocImportRecord
			{
				ControlNumber = GenerateControlNumber(),
				File = ResourceFileHelper.GetDocsResourceFilePath(fileName),
				Folder = folderPath,
			}).ToList();
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job = this.ArrangeImportJob(records);

			// Act
			job.Execute();

			// Assert - the import job is successful.
			this.AssertImportSuccess();

			// Assert - all max depth folders were created.
			IEnumerable<string> folders = SplitFolderPath(folderPath);
			this.AssertDistinctFolders(folders.ToArray());
		}

		protected override void OnSetup()
		{
			base.OnSetup();
			AppSettings.Instance.CreateFoldersInWebApi = this.serverSideFolders;
		}
	}
}