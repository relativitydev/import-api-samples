// ----------------------------------------------------------------------------
// <copyright file="DocImportConcurrencyTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.Sample.NUnit.Tests
{
	using System.Collections.Generic;
	using System.IO;

	using global::NUnit.Framework;

	/// <summary>
	/// Represents a test that creates a new workspace, concurrently import documents, validates the results, and deletes the workspace.
	/// </summary>
	[TestFixture]
	public class DocImportConcurrencyTests : DocImportTestsBase
	{
		// [Test]
		public void ShouldConcurrentlyImportTheDocs()
		{
			// Arrange
			string folderPath = GenerateFolderPath(10);
			List<DocImportRecord> records = new List<DocImportRecord>();
			foreach (var file in Directory.GetFiles(
				@"Z:\PerfDataSet_60GB\MicrosoftOfficeFiles_Various\10GB-EDRM-Loose\VOL0001\NATIVES\NATIVE00001"))
			{
				records.Add(new DocImportRecord
					{ ControlNumber = GenerateControlNumber(), File = file, Folder = folderPath});
			}

			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job = this.ArrangeImportJob(records);

			// Act
			job.Execute();

			// Assert - the import job is successful.
			this.AssertImportSuccess();
		}
	}
}