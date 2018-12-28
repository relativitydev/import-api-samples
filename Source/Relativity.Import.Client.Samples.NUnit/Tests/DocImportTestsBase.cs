// ----------------------------------------------------------------------------
// <copyright file="DocImportTestsBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.Sample.NUnit.Tests
{
	using System;
	using System.Text;

	/// <summary>
	/// Represents an abstract test class object that creates a new workspace, import documents, validates the results, and deletes the workspace.
	/// </summary>
	public abstract class DocImportTestsBase : ImportTestsBase
	{
		protected const string ArtifactTypeName = "Document";
		protected const string FieldControlNumber = "Control Number";
		protected const string FieldFilePath = "File Path";

		protected DocImportTestsBase()
			: base(AssemblySetup.Logger)
		{
			// Assume that AssemblySetup has already setup the singleton.
		}

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

		protected void ConfigureJobSettings(
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job,
			int artifactTypeId,
			int identityFieldId,
			string nativeFilePathSourceFieldName,
			string identifierFieldName)
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