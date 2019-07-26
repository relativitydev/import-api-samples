// ----------------------------------------------------------------------------
// <copyright file="DocNegativeImportTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Samples.NUnit.Tests
{
	using global::NUnit.Framework;

	using Relativity.DataExchange.TestFramework;

	/// <summary>
	/// Represents tests that fails to import documents and validates the results.
	/// </summary>
	[TestFixture]
	public class DocNegativeImportTests : DocImportTestsBase
	{
		[Test]
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		public void ShouldNotImportWhenTheFolderExceedsTheMaxLength()
		{
			// Arrange
			string controlNumber = GenerateControlNumber();
			string folder = "\\" + new string('x', IntegrationTestParameters.MaxFolderLength + 1);
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job =
				this.ArrangeImportJob(controlNumber, folder, SampleDocPdfFileName);

			// Act
			job.Execute();

			// Assert - the job failed with a single non-fatal exception.
			this.AssertImportFailed(1);

			// Assert - exceeding the max folder length yields a doc-level error.
			this.AssertError(0, 1, controlNumber, IntegrationTestParameters.MaxFolderLength.ToString());
		}
	}
}