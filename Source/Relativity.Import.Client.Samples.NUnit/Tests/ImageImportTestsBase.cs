// ----------------------------------------------------------------------------
// <copyright file="ImageImportTestsBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.Sample.NUnit.Tests
{
	using System;
	using System.Text;

	/// <summary>
	/// Represents an abstract test class object that creates a new workspace, import images, validates the results, and deletes the workspace.
	/// </summary>
	public abstract class ImageImportTestsBase : ImportTestsBase
	{
		protected const string ArtifactTypeName = "Document";
		protected const string FieldBatesNumber = "Bates Number";
		protected const string FieldControlNumber = "Control Number";
		protected const string FieldFileLocation = "File Location";
		protected int ArtifactTypeId;
		protected int IdentifierFieldId;

		protected override void OnSetup()
		{
			base.OnSetup();
			this.ArtifactTypeId = this.QueryArtifactTypeId(ArtifactTypeName);
			this.IdentifierFieldId = this.QueryIdentifierFieldId(ArtifactTypeName);
		}

		protected void ConfigureJobSettings(kCura.Relativity.DataReaderClient.ImageImportBulkArtifactJob job)
		{
			kCura.Relativity.DataReaderClient.ImageSettings settings = job.Settings;
			settings.ArtifactTypeId = this.ArtifactTypeId;
			settings.AutoNumberImages = true;
			settings.BatesNumberField = FieldBatesNumber;
			settings.CaseArtifactId = TestSettings.WorkspaceId;
			settings.CopyFilesToDocumentRepository = true;
			settings.DisableImageLocationValidation = false;
			settings.DisableImageTypeValidation = false;
			settings.DocumentIdentifierField = FieldControlNumber;
			settings.ExtractedTextEncoding = Encoding.Unicode;
			settings.ExtractedTextFieldContainsFilePath = false;
			settings.FileLocationField = FieldFileLocation;
			settings.IdentityFieldId = this.IdentifierFieldId;
			settings.LoadImportedFullTextFromServer = false;
			settings.NativeFileCopyMode = kCura.Relativity.DataReaderClient.NativeFileCopyModeEnum.CopyFiles;
			settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Append;
			settings.SelectedIdentifierFieldName = FieldControlNumber;
		}

		protected void ConfigureJobEvents(kCura.Relativity.DataReaderClient.ImageImportBulkArtifactJob job)
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
	}
}