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
		protected const string ControlNumberFieldName = "control number";

		/// <summary>
		/// The file path field name.
		/// </summary>
		protected const string FilePathFieldName = "file path";

		/// <summary>
		/// The folder field name.
		/// </summary>
		protected const string FolderFieldName = "folder name";

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
		/// The sample MSG file name that's available for testing within the output directory.
		/// </summary>
		protected const string SampleMsgFileName = "EDRM-Sample4.msg";

		/// <summary>
		/// The sample HTM file name that's available for testing within the output directory.
		/// </summary>
		protected const string SampleHtmFileName = "EDRM-Sample5.htm";

		/// <summary>
		/// The sample EMF file name that's available for testing within the output directory.
		/// </summary>
		protected const string SampleEmfFileName = "EDRM-Sample6.emf";

		/// <summary>
		/// The sample PPT file name that's available for testing within the output directory.
		/// </summary>
		protected const string SamplePptFileName = "EDRM-Sample7.ppt";

		/// <summary>
		/// The sample PNG file name that's available for testing within the output directory.
		/// </summary>
		protected const string SamplePngFileName = "EDRM-Sample8.png";

		/// <summary>
		/// The sample TXT file name that's available for testing within the output directory.
		/// </summary>
		protected const string SampleTxtFileName = "EDRM-Sample9.txt";

		/// <summary>
		/// The sample WMF file name that's available for testing within the output directory.
		/// </summary>
		protected const string SampleWmfFileName = "EDRM-Sample10.wmf";

		/// <summary>
		/// The list of all sample file names available for testing within the output directory.
		/// </summary>
		protected static IEnumerable<string> AllSampleFileNames = new[]
		{
			SamplePdfFileName,
			SampleWordFileName,
			SampleExcelFileName,
			SampleMsgFileName,
			SampleHtmFileName,
			SampleEmfFileName,
			SamplePptFileName,
			SamplePngFileName,
			SampleTxtFileName,
			SampleWmfFileName
		};

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
				string folderName = $"\\{Guid.NewGuid()}-{TestHelper.NextString(20, TestSettings.MaxFolderLength - 36)}";
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
			string file = TestHelper.GetDocsResourceFilePath(fileName);
			DocImportRecord record = new DocImportRecord { ControlNumber = controlNumber, File = file, Folder = folder };
			return this.ArrangeImportJob(new[] { record  });
		}

		protected kCura.Relativity.DataReaderClient.ImportBulkArtifactJob ArrangeImportJob(IEnumerable<DocImportRecord> records)
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
			this.DataSource.Columns.AddRange(new[]
			{
				new DataColumn(ControlNumberFieldName, typeof(string)),
				new DataColumn(FilePathFieldName, typeof(string)),
				new DataColumn(FolderFieldName, typeof(string))
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

		protected override void OnSetup()
		{
			base.OnSetup();
			this.ArtifactTypeId = this.QueryArtifactTypeId(ArtifactTypeName);
			this.IdentifierFieldId = this.QueryIdentifierFieldId(ArtifactTypeName);
		}
	}

	public class DocImportRecord
	{
		public string ControlNumber
		{
			get;
			set;
		}

		public string Folder
		{
			get;
			set;
		}

		public string File
		{
			get;
			set;
		}
	}
}