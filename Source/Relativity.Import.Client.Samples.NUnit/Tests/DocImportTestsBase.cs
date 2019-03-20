// ----------------------------------------------------------------------------
// <copyright file="DocImportTestsBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.Samples.NUnit.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Text;

	using global::NUnit.Framework;

    using Relativity.Import.Export.TestFramework;

    /// <summary>
    /// Represents an abstract test class object that imports native documents and validates the results.
    /// </summary>
    public abstract class DocImportTestsBase : ImportTestsBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DocImportTestsBase"/> class.
		/// </summary>
		protected DocImportTestsBase()
			: base(IntegrationTestHelper.Logger)
		{
			// Assume that AssemblySetup has already setup the singleton.
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DocImportTestsBase"/> class.
		/// </summary>
		/// <param name="log">
		/// The Relativity logger.
		/// </param>
		protected DocImportTestsBase(Relativity.Logging.ILog log)
			: base(log)
		{
		}

        /// <summary>
        /// Splits the folder path into one or more individual folders.
        /// </summary>
        /// <param name="folderPath">
        /// The folder path.
        /// </param>
        /// <returns>
        /// The list of folders.
        /// </returns>
        protected static IEnumerable<string> SplitFolderPath(string folderPath)
		{
			return folderPath.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
		}

		/// <summary>
		/// Generates a unique folder path.
		/// </summary>
		/// <param name="maxDepth">
		/// The maximum depth.
		/// </param>
		/// <returns>
		/// The folder path.
		/// </returns>
		protected static string GenerateFolderPath(int maxDepth)
		{
			StringBuilder sb = new StringBuilder();
			for (var i = 0; i < maxDepth; i++)
			{
				string folderName = $"\\{Guid.NewGuid()}-{RandomHelper.NextString(20, IntegrationTestParameters.MaxFolderLength - 36)}";
				sb.Append(folderName);
			}

			string folderPath = sb.ToString();
			return folderPath;
		}

		protected kCura.Relativity.DataReaderClient.ImportBulkArtifactJob ArrangeImportJob(
			string controlNumber,
			string folder,
			string fileName)
		{
			string file = ResourceFileHelper.GetDocsResourceFilePath(fileName);
			DocImportRecord record = new DocImportRecord { ControlNumber = controlNumber, File = file, Folder = folder };
			return this.ArrangeImportJob(new[] { record });
		}

		protected kCura.Relativity.DataReaderClient.ImportBulkArtifactJob ArrangeImportJob(IEnumerable<DocImportRecord> records)
		{
			// Arrange
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
				new DataColumn(WellKnownFields.FolderName, typeof(string))
			});

			// Add the file to the data source.
			foreach (DocImportRecord record in records)
			{
				this.DataSource.Rows.Add(record.ControlNumber, record.File, record.Folder);
				job.SourceData.SourceData = this.DataSource.CreateDataReader();
			}

			return job;
		}

		protected void AssertImportSuccess()
		{
			// Assert - the job completed and the report matches the expected values.
			Assert.That(this.PublishedJobReport, Is.Not.Null);
			Assert.That(this.PublishedJobReport.EndTime, Is.GreaterThan(this.PublishedJobReport.StartTime));
			Assert.That(this.PublishedJobReport.ErrorRowCount, Is.Zero);
			Assert.That(this.PublishedJobReport.FileBytes, Is.Positive);
			Assert.That(this.PublishedJobReport.MetadataBytes, Is.Positive);
			Assert.That(this.PublishedJobReport.StartTime, Is.GreaterThan(this.StartTime));
			Assert.That(this.PublishedJobReport.TotalRows, Is.EqualTo(this.DataSource.Rows.Count));

			// Assert - the events match the expected values.
			Assert.That(this.PublishedErrors.Count, Is.Zero);
			Assert.That(this.PublishedFatalException, Is.Null);
			Assert.That(this.PublishedMessages.Count, Is.Positive);
			Assert.That(this.PublishedProcessProgress.Count, Is.Positive);
			Assert.That(this.PublishedProgressRows.Count, Is.Positive);
		}

		/// <summary>
		/// Asserts that the supplied folder names aren't duplicated.
		/// </summary>
		/// <param name="folderNames">
		/// The list of folder names.
		/// </param>
		protected void AssertDistinctFolders(params string[] folderNames)
		{
			// Assert - SQL collation is case-insensitive.
			IList<string> actualFolders = this.QueryWorkspaceFolders();
			int actualMatches = 0;
			foreach (string folder in actualFolders)
			{
				foreach (string expectedFolderName in folderNames)
				{
					if (folder.IndexOf(expectedFolderName, 0, StringComparison.OrdinalIgnoreCase) != -1)
					{
						actualMatches++;
					}
				}
			}

			Assert.That(actualMatches, Is.EqualTo(folderNames.Length));
		}

		protected void AssertImportFailed(int expectedErrorEvents)
		{
			// Assert - the job failed with a non-fatal exception.
			Assert.That(this.PublishedJobReport, Is.Not.Null);
			Assert.That(this.PublishedFatalException, Is.Null);
			Assert.That(this.PublishedErrors.Count, Is.EqualTo(expectedErrorEvents));
		}

		protected void AssertError(int errorIndex, int expectedLineNumber, string expectedControlNumber, string expectedMessageSubstring)
		{
			Assert.That(this.PublishedErrors[errorIndex]["Line Number"], Is.EqualTo(expectedLineNumber));
			Assert.That(this.PublishedErrors[errorIndex]["Identifier"], Is.EqualTo(expectedControlNumber));
			Assert.That(this.PublishedErrors[errorIndex]["Message"], Contains.Substring(expectedMessageSubstring));
		}

		protected void ConfigureJobEvents(kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job)
		{
			job.OnComplete += report =>
			{
				this.PublishedJobReport = report;
				Console.WriteLine("[Job Complete]");
			};

			job.OnError += row =>
			{
				this.PublishedErrors.Add(row);
			};

			job.OnFatalException += report =>
			{
				this.PublishedFatalException = report.FatalException;
				Console.WriteLine("[Job Fatal Exception]: " + report.FatalException);
			};

			job.OnMessage += status =>
			{
				this.PublishedMessages.Add(status.Message);
				Console.WriteLine("[Job Message]: " + status.Message);
			};

			job.OnProcessProgress += status =>
			{
				this.PublishedProcessProgress.Add(status);
			};

			job.OnProgress += row =>
			{
				this.PublishedProgressRows.Add(row);
			};
		}
	}
}