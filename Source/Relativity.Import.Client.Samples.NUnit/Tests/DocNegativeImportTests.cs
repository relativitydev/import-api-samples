// ----------------------------------------------------------------------------
// <copyright file="DocNegativeImportTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.Sample.NUnit.Tests
{
	using System;

	using global::NUnit.Framework;

	/// <summary>
	/// Represents tests that create a new workspace, fail to import documents, validates the results, and deletes the workspace.
	/// </summary>
	[TestFixture]
	public class DocNegativeImportTests : DocImportTestsBase
	{
		[Test]
		public void ShouldNotImportWhenTheFolderExceedsTheMaxLength()
		{
			// Arrange
			const int MaxFolderLength = 255;
			string controlNumber = "REL-" + Guid.NewGuid();
			string folder = "\\" + new string('x', MaxFolderLength + 1);
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job =
				this.ArrangeImportJob(controlNumber, folder, SamplePdfFileName);

			// Act
			job.Execute();

			// Assert - the job failed with a single non-fatal exception.
			this.AssertImportFailed(1);

			// Assert - exceeding the max folder length yields a doc-level error.
			this.AssertError(0, 1, controlNumber, MaxFolderLength.ToString());
		}
	}
}