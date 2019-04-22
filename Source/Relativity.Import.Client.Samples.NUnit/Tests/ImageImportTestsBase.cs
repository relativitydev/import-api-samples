// ----------------------------------------------------------------------------
// <copyright file="ImageImportTestsBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.Samples.NUnit.Tests
{
	using System;
	using System.Text;

	using kCura.EDDS.WebAPI.BulkImportManagerBase;

	using Relativity.Import.Export.TestFramework;

	/// <summary>
	/// Represents an abstract test class object that imports images and validates the results.
	/// </summary>
	public abstract class ImageImportTestsBase : ImportTestsBase
	{
		protected void ConfigureJobSettings(kCura.Relativity.DataReaderClient.ImageImportBulkArtifactJob job)
		{
			kCura.Relativity.DataReaderClient.ImageSettings settings = job.Settings;
			settings.ArtifactTypeId = this.ArtifactTypeId;
			settings.AutoNumberImages = false;
			settings.BatesNumberField = WellKnownFields.BatesNumber;
			settings.Billable = false;
			settings.CaseArtifactId = this.TestParameters.WorkspaceId;
			settings.CopyFilesToDocumentRepository = true;
			settings.DisableExtractedTextEncodingCheck = true;
			settings.DisableImageLocationValidation = false;
			settings.DisableImageTypeValidation = false;
			settings.DisableUserSecurityCheck = true;
			settings.DocumentIdentifierField = this.IdentifierFieldName;
			settings.ExtractedTextEncoding = Encoding.Unicode;
			settings.ExtractedTextFieldContainsFilePath = false;
			settings.FileLocationField = WellKnownFields.FileLocation;
			settings.FolderPathSourceFieldName = null;
			settings.IdentityFieldId = this.IdentifierFieldId;
			settings.ImageFilePathSourceFieldName = WellKnownFields.FileLocation;
			settings.LoadImportedFullTextFromServer = false;
			settings.MaximumErrorCount = int.MaxValue - 1;
			settings.MoveDocumentsInAppendOverlayMode = false;
			settings.NativeFileCopyMode = kCura.Relativity.DataReaderClient.NativeFileCopyModeEnum.CopyFiles;
			settings.OverlayBehavior = OverlayBehavior.MergeAll;
			settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Append;
			settings.SelectedIdentifierFieldName = this.IdentifierFieldName;
			settings.StartRecordNumber = 0;

			// Note: production related parameters are automatically set by ImportAPI.
			////parameters.ForProduction = true;
			////parameters.ProductionArtifactID = 1;
		}

		protected void ConfigureJobEvents(kCura.Relativity.DataReaderClient.ImageImportBulkArtifactJob job)
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