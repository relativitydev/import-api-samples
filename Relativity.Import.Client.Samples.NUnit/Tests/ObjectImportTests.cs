// ----------------------------------------------------------------------------
// <copyright file="ObjectImportTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.Sample.NUnit.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Data;

	using global::NUnit.Framework;

	/// <summary>
	/// Represents a test that creates a new workspace, import objects, validates the results, and deletes the workspace.
	/// </summary>
	[TestFixture]
	public class ObjectImportTests : ImportTestsBase
	{
		private const string ArtifactTypeName = "TransferJob";
		private const string FieldCompleted = "Completed";
		private const string FieldDescription = "Description";
		private const string FieldName = "Name";
		private const string FieldSize = "Size";

		private int _artifactTypeId;
		private int _identifierFieldId;

		private static IEnumerable<string> TestCases
		{
			get
			{
				yield return "Job-Small";
				yield return "Job-Medium";
				yield return "Job-Large";
			}
		}

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			int artifactTypeId = this.CreateObjectType(ArtifactTypeName);
			int workspaceObjectTypeId = this.QueryWorkspaceObjectTypeDescriptorId(artifactTypeId);
			this.CreateFixedLengthTextField(workspaceObjectTypeId, FieldDescription, 100);
			this.CreateDecimalField(workspaceObjectTypeId, FieldSize);
			this.CreateDateField(workspaceObjectTypeId, FieldCompleted);
			this._artifactTypeId = this.QueryArtifactTypeId(ArtifactTypeName);
			this._identifierFieldId = this.QueryIdentifierFieldId(ArtifactTypeName);
		}

		[Test]
		[TestCaseSource(nameof(TestCases))]
		public void ShouldImportTheObject(string name)
		{
			// Arrange
			kCura.Relativity.ImportAPI.ImportAPI importApi = CreateImportApiObject();
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job = importApi.NewObjectImportJob(this._artifactTypeId);
			this.ConfigureJobSettings(job);
			this.CatchJobEvents(job);
			this.DataTable.Columns.AddRange(new[]
			{
				new DataColumn(FieldName, typeof(string)),
				new DataColumn(FieldDescription, typeof(string)),
				new DataColumn(FieldSize, typeof(decimal)),
				new DataColumn(FieldCompleted, typeof(DateTime))
			});

			int initialObjectCount = this.QueryRelativityObjectCount(this._artifactTypeId);
			decimal jobSize = TestHelper.NextDecimal(10, 100000);
			string description = TestHelper.NextString(50, 100);
			DateTime completed = DateTime.Now;
			this.DataTable.Rows.Add(name, description, jobSize, completed);
			job.SourceData.SourceData = this.DataTable.CreateDataReader();

			// Act
			job.Execute();

			// Assert
			Assert.That(this.FatalException, Is.Null);
			Assert.That(this.JobReport, Is.Not.Null);
			Assert.That(this.JobReport.ErrorRowCount, Is.Zero);
			Assert.That(this.JobReport.TotalRows, Is.EqualTo(1));
			int expectedObjectCount = initialObjectCount + this.DataTable.Rows.Count;
			IList<Relativity.Services.Objects.DataContracts.RelativityObject> objects =
				this.QueryRelativityObjects(this._artifactTypeId,
					new[] { FieldName, FieldDescription, FieldSize, FieldCompleted });
			Assert.That(objects, Is.Not.Null);
			Assert.That(objects.Count, Is.EqualTo(expectedObjectCount));
			Relativity.Services.Objects.DataContracts.RelativityObject importedObj
				= GetRelativityObject(objects, FieldName, name);
			Assert.That(importedObj, Is.Not.Null);
			string descriptionFieldValue = GetStringFieldValue(importedObj, FieldDescription);
			Assert.That(descriptionFieldValue, Is.EqualTo(description));
			DateTime completeFieldValue = GetDateFieldValue(importedObj, FieldCompleted);
			Assert.That(completeFieldValue, Is.EqualTo(completed).Within(5).Seconds);
			decimal sizeFieldValue = GetDecimalFieldValue(importedObj, FieldSize);
			Assert.That(sizeFieldValue, Is.EqualTo(jobSize));
		}

		private void ConfigureJobSettings(kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job)
		{
			kCura.Relativity.DataReaderClient.Settings settings = job.Settings;
			settings.ArtifactTypeId = this._artifactTypeId;
			settings.Billable = false;
			settings.BulkLoadFileFieldDelimiter = ";";
			settings.CaseArtifactId = TestSettings.WorkspaceId;						
			settings.CopyFilesToDocumentRepository = false;
			settings.DisableControlNumberCompatibilityMode = true;
			settings.DisableExtractedTextEncodingCheck = true;
			settings.DisableExtractedTextFileLocationValidation = true;
			settings.DisableNativeLocationValidation = false;
			settings.DisableNativeValidation = false;
			settings.DisableUserSecurityCheck = true;
			settings.ExtractedTextFieldContainsFilePath = false;
			settings.IdentityFieldId = this._identifierFieldId;
			settings.LoadImportedFullTextFromServer = false;
			settings.MaximumErrorCount = int.MaxValue - 1;
			settings.MoveDocumentsInAppendOverlayMode = false;
			settings.MultiValueDelimiter = ';';
			settings.NativeFileCopyMode = kCura.Relativity.DataReaderClient.NativeFileCopyModeEnum.DoNotImportNativeFiles;
			settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Append;
			settings.SelectedIdentifierFieldName = "Name";
			settings.StartRecordNumber = 0;
		}

		private void CatchJobEvents(kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job)
		{
			job.OnMessage += status =>
			{
				Console.WriteLine("[Message]: " + status.Message);
			};

			job.OnFatalException += report =>
			{
				this.FatalException = report.FatalException;
				Console.WriteLine("[Fatal Exception]: " + report.FatalException);
			};

			job.OnComplete += report =>
			{
				this.JobReport = report;
				Console.WriteLine("[Complete]");
			};
		}
	}
}