// ----------------------------------------------------------------------------
// <copyright file="DocImportFolderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.Sample.NUnit.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using global::NUnit.Framework;

	/// <summary>
	/// Represents a test that creates a new workspace, import documents with folders, validates the results, and deletes the workspace.
	/// </summary>
	[TestFixture(true)]
	[TestFixture(false)]
	public class DocImportFolderTests : DocImportTestsBase
	{
		/// <summary>
		/// The client-side folder API flag for maximum test coverage.
		/// </summary>
		private readonly bool clientSideFolders;

		/// <summary>
		/// Initializes a new instance of the <see cref="DocImportFolderTests"/> class.
		/// </summary>
		/// <param name="clientSideFolders">
		/// <see langword="true" /> to create all folders via client-side API; otherwise, <see langword="false" /> to create all folders via server-side WebPI.
		/// </param>
		public DocImportFolderTests(bool clientSideFolders)
		{
			this.clientSideFolders = clientSideFolders;
		}

		protected override void OnSetup()
		{
			base.OnSetup();
			SetWinEddsConfigValue("CreateFoldersInWebAPI", this.clientSideFolders);
		}

		[Test]
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
			string folder = $"\\{invalidFolder}-{this.clientSideFolders}";
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job =
				this.ArrangeImportJob(controlNumber, folder, SamplePdfFileName);

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
				this.ArrangeImportJob(controlNumber, folder, SamplePdfFileName);

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
		[TestCase(10)]
		[TestCase(100)]
		[TestCase(500)]
		[TestCase(1000)]
		public void ShouldSupportTheMaxFolderDepth(int maxDepth)
		{
			// Arrange
			string controlNumber = GenerateControlNumber();
			StringBuilder sb = new StringBuilder();
			for (var i = 0; i < maxDepth; i++)
			{
				string folderName = $"\\{Guid.NewGuid()}-{TestHelper.NextString(20, TestSettings.MaxFolderLength - 36)}";
				sb.Append(folderName);
			}

			string folderPath = sb.ToString();
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job =
				this.ArrangeImportJob(controlNumber, folderPath, SamplePdfFileName);

			// Act
			job.Execute();

			// Assert - the import job is successful.
			this.AssertImportSuccess();

			// Assert - all max depth folders were created.
			IEnumerable<string> folders = SplitFolderPath(folderPath);
			this.AssertDistinctFolders(folders.ToArray());
		}
	}
}