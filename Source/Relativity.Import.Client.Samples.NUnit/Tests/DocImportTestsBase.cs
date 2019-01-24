// ----------------------------------------------------------------------------
// <copyright file="DocImportTestsBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.Sample.NUnit.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Text;

	using global::NUnit.Framework;

	/// <summary>
	/// Represents an abstract test class object that creates a new workspace, import documents, validates the results, and deletes the workspace.
	/// </summary>
	public abstract class DocImportTestsBase : ImportTestsBase
	{
		/// <summary>
		/// The document artifact type name.
		/// </summary>
		protected const string ArtifactTypeName = "Document";

		/// <summary>
		/// The control number field name.
		/// </summary>
		protected const string ControlNumberFieldName = "Control Number";

		/// <summary>
		/// The file path field name.
		/// </summary>
		protected const string FilePathFieldName = "File Path";

		/// <summary>
		/// The folder field name.
		/// </summary>
		protected const string FolderFieldName = "Folder";

		/// <summary>
		/// The sample PDF file name that's available for testing within the output directory.
		/// </summary>
		protected const string SamplePdfFileName = "EDRM-Sample1.pdf";

		/// <summary>
		/// The sample Word doc file name that's available for testing within the output directory.
		/// </summary>
		protected const string SampleWordFileName = "EDRM-Sample2.doc";

		/// <summary>
		/// The sample Excel file name that's available for testing within the output directory.
		/// </summary>
		protected const string SampleExcelFileName = "EDRM-Sample3.xlsx";

		/// <summary>
		/// Initializes a new instance of the <see cref="DocImportTestsBase"/> class.
		/// </summary>
		protected DocImportTestsBase()
			: base(AssemblySetup.Logger)
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

		protected int ArtifactTypeId
		{
			get;
			private set;
		}

		protected int IdentifierFieldId
		{
			get;
			private set;
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
		protected IEnumerable<string> SplitFolderPath(string folderPath)
		{
			return folderPath.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
		}

		protected kCura.Relativity.DataReaderClient.ImportBulkArtifactJob ArrangeImportJob(
			string controlNumber,
			string folder,
			string fileName)
		{
			// Arrange
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
			return job;
		}

		protected void AssertImportSuccess()
		{
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
			Assert.That(this.JobCompletedReport, Is.Not.Null);
			Assert.That(this.FatalExceptionEvent, Is.Null);
			Assert.That(this.ErrorEvents.Count, Is.EqualTo(expectedErrorEvents));
		}

		protected void AssertError(int errorIndex, int expectedLineNumber, string expectedControlNumber, string expectedMessageSubstring)
		{
			Assert.That(this.ErrorEvents[errorIndex]["Line Number"], Is.EqualTo(expectedLineNumber));
			Assert.That(this.ErrorEvents[errorIndex]["Identifier"], Is.EqualTo(expectedControlNumber));
			Assert.That(this.ErrorEvents[errorIndex]["Message"], Contains.Substring(expectedMessageSubstring));
		}

		protected void ConfigureJobSettings(
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job,
			int artifactTypeId,
			int identityFieldId,
			string nativeFilePathSourceFieldName,
			string identifierFieldName,
			string folderFieldName)
		{
			kCura.Relativity.DataReaderClient.Settings settings = job.Settings;
			settings.ArtifactTypeId = artifactTypeId;
			settings.BulkLoadFileFieldDelimiter = ";";
			settings.CaseArtifactId = TestSettings.WorkspaceId;
			settings.CopyFilesToDocumentRepository = true;
			settings.DisableControlNumberCompatibilityMode = true;
			settings.DisableExtractedTextFileLocationValidation = false;
			settings.DisableNativeLocationValidation = false;
			settings.DisableNativeValidation = false;
			settings.ExtractedTextEncoding = Encoding.Unicode;
			settings.ExtractedTextFieldContainsFilePath = false;
			settings.FileSizeColumn = "NativeFileSize";
			settings.FileSizeMapped = true;
			settings.FolderPathSourceFieldName = folderFieldName;
			settings.IdentityFieldId = identityFieldId;
			settings.LoadImportedFullTextFromServer = false;
			settings.NativeFileCopyMode = kCura.Relativity.DataReaderClient.NativeFileCopyModeEnum.CopyFiles;
			settings.NativeFilePathSourceFieldName = nativeFilePathSourceFieldName;
			settings.OIFileIdColumnName = "OutsideInFileId";
			settings.OIFileIdMapped = true;
			settings.OIFileTypeColumnName = "OutsideInFileType";
			settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Append;
			settings.SelectedIdentifierFieldName = identifierFieldName;
		}

		protected void ConfigureJobEvents(kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job)
		{
			job.OnComplete += report =>
			{
				this.JobCompletedReport = report;
				Console.WriteLine("[Job Complete]");
			};

			job.OnError += row =>
			{
				this.ErrorEvents.Add(row);
			};

			job.OnFatalException += report =>
			{
				this.FatalExceptionEvent = report.FatalException;
				Console.WriteLine("[Job Fatal Exception]: " + report.FatalException);
			};

			job.OnMessage += status =>
			{
				this.MessageEvents.Add(status.Message);
				Console.WriteLine("[Job Message]: " + status.Message);
			};

			job.OnProcessProgress += status =>
			{
				this.ProcessProgressEvents.Add(status);
			};

			job.OnProgress += row =>
			{
				this.ProgressRowEvents.Add(row);
			};
		}

		protected override void OnSetup()
		{
			base.OnSetup();
			this.ArtifactTypeId = this.QueryArtifactTypeId(ArtifactTypeName);
			this.IdentifierFieldId = this.QueryIdentifierFieldId(ArtifactTypeName);
		}
	}
}