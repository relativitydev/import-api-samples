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
		// The custom object type under test.
		private const string TransferJobArtifactTypeName = "TransferJob";
		private const string TransferJobFieldDescription = "Description";
		private const string TransferJobFieldName = "Name";
		private const string TransferJobFieldDetailId = "TransferJobDetailId";
		private const string TransferJobFieldDataSourceId = "TransferJobDataSourceId";
		private const string TransferJobFieldRequestBytes = "RequestBytes";
		private const string TransferJobFieldRequestFiles = "RequestFiles";
		private const string TransferJobFieldRequestDate = "RequestDate";

		// To verify importing a single-object field.
		private const string TransferJobDetailArtifactTypeName = "TransferJobDetail";
		private const string TransferJobDetailFieldName = "Name";
		private const string TransferJobDetailFieldTransferredBytes = "TransferredBytes";
		private const string TransferJobDetailFieldTransferredFiles = "TransferredFiles";
		private const string TransferJobDetailFieldStartDate = "StartDate";
		private const string TransferJobDetailFieldEndDate = "EndDate";

		// To verify importing a multi-object field.
		private const string TransferJobDataSourceArtifactTypeName = "TransferJobDataSource";
		private const string TransferJobDataSourceFieldName = "Name";
		private const string TransferJobDataSourceFieldNumber = "Number";
		private const string TransferJobDataSourceFieldConnectionString = "ConnectionString";

		private int _transferJobDetailArtifactTypeId;
		private int _transferJobDetailWorkspaceObjectTypeId;
		private int _transferJobArtifactTypeId;
		private int _transferJobIdentifierFieldId;
		private int _transferJobDataSourceArtifactTypeId;
		private int _transferJobDataSourceWorkspaceObjectTypeId;
		private int _transferJobWorkspaceObjectTypeId;

		private static IEnumerable<TestCaseData> TestCases
		{
			get
			{
				yield return new TestCaseData("Job-Small-1", "Detail-1", "DataSourceName-1", true);
				yield return new TestCaseData("Job-Small-2", "Detail-2", "DataSourceName-2", false);
				yield return new TestCaseData("Job-Medium-1", "Detail-3", "DataSourceName-3", true);
				yield return new TestCaseData("Job-Medium-2", "Detail-4", "DataSourceName-4", false);
				yield return new TestCaseData("Job-Large-1", "Detail-5", "DataSourceName-5", true);
				yield return new TestCaseData("Job-Large-2", "Detail-6", "DataSourceName-6", false);
			}
		}

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			this.CreateTransferJobDetailObjectType();
			this.CreateTransferJobDataSourceObjectType();
			this.CreateTransferJobObjectType();
		}

		[Test]
		[TestCaseSource(nameof(TestCases))]
		public void ShouldImportTheObject(string name, string detailName, string dataSourceName, bool importAssociatedObj)
		{
			// Arrange
			if (!importAssociatedObj)
			{
				// This verifies import does NOT fail when the associated object already exists.
				this.CreateAssociatedDetailInstance(detailName);
				this.CreateAssociatedDataSourceInstance(dataSourceName);
			}

			kCura.Relativity.ImportAPI.ImportAPI importApi = CreateImportApiObject();
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job = importApi.NewObjectImportJob(this._transferJobArtifactTypeId);
			this.ConfigureJobSettings(job);
			this.CatchJobEvents(job);
			this.DataTable.Columns.AddRange(new[]
			{
				new DataColumn(TransferJobFieldName, typeof(string)),
				new DataColumn(TransferJobFieldDescription, typeof(string)),
				new DataColumn(TransferJobFieldRequestBytes, typeof(decimal)),
				new DataColumn(TransferJobFieldRequestFiles, typeof(decimal)),
				new DataColumn(TransferJobFieldRequestDate, typeof(DateTime)),
				new DataColumn(TransferJobFieldDetailId, typeof(string)),
				new DataColumn(TransferJobFieldDataSourceId, typeof(string))
			});

			int initialObjectCount = this.QueryRelativityObjectCount(this._transferJobArtifactTypeId);
			decimal requestBytes = TestHelper.NextDecimal(10, 1000000);
			decimal requestFiles = TestHelper.NextDecimal(1000, 10000);
			string description = TestHelper.NextString(50, 450);
			DateTime requestDate = DateTime.Now;
			this.DataTable.Rows.Add(
				name,
				description,
				requestBytes,
				requestFiles,
				requestDate,
				detailName,
				dataSourceName);
			job.SourceData.SourceData = this.DataTable.CreateDataReader();

			// Act
			job.Execute();

			// Assert - the job completed and the report matches the expected values.
			Assert.That(this.JobCompletedReport, Is.Not.Null);
			Assert.That(this.JobCompletedReport.EndTime, Is.GreaterThan(this.JobCompletedReport.StartTime));
			Assert.That(this.JobCompletedReport.ErrorRowCount, Is.Zero);
			Assert.That(this.JobCompletedReport.FileBytes, Is.EqualTo(0));
			Assert.That(this.JobCompletedReport.MetadataBytes, Is.GreaterThan(0));
			Assert.That(this.JobCompletedReport.StartTime, Is.GreaterThan(this.ImportStartTime));
			Assert.That(this.JobCompletedReport.TotalRows, Is.EqualTo(1));
			
			// Assert - the events match the expected values.
			Assert.That(this.ErrorEvents.Count, Is.EqualTo(0));
			Assert.That(this.FatalExceptionEvent, Is.Null);
			Assert.That(this.MessageEvents.Count, Is.GreaterThan(0));
			Assert.That(this.ProcessProgressEvents.Count, Is.GreaterThan(0));
			Assert.That(this.ProgressRowEvents.Count, Is.GreaterThan(0));

			// Assert - the object count is incremented by 1.
			int expectedObjectCount = initialObjectCount + this.DataTable.Rows.Count;
			IList<Relativity.Services.Objects.DataContracts.RelativityObject> transferJobs =
				this.QueryRelativityObjects(
					this._transferJobArtifactTypeId,
					new[]
					{
						TransferJobFieldName,
						TransferJobFieldDescription,
						TransferJobFieldRequestBytes,
						TransferJobFieldRequestFiles,
						TransferJobFieldRequestDate
					});
			Assert.That(transferJobs, Is.Not.Null);
			Assert.That(transferJobs.Count, Is.EqualTo(expectedObjectCount));

			// Assert - the imported object exists and all field values matches the expected values.
			Relativity.Services.Objects.DataContracts.RelativityObject importedTransferJob
				= FindRelativityObject(transferJobs, TransferJobFieldName, name);
			Assert.That(importedTransferJob, Is.Not.Null);
			string descriptionFieldValue = FindStringFieldValue(importedTransferJob, TransferJobFieldDescription);
			Assert.That(descriptionFieldValue, Is.EqualTo(description));
			decimal requestBytesFieldValue = FindDecimalFieldValue(importedTransferJob, TransferJobFieldRequestBytes);
			Assert.That(requestBytesFieldValue, Is.EqualTo(requestBytes));
			decimal requestFilesFileValue = FindDecimalFieldValue(importedTransferJob, TransferJobFieldRequestFiles);
			Assert.That(requestFilesFileValue, Is.EqualTo(requestFiles));
			DateTime requestDateFieldValue = FindDateFieldValue(importedTransferJob, TransferJobFieldRequestDate);
			Assert.That(requestDateFieldValue, Is.EqualTo(requestDate).Within(5).Seconds);
		}

		private void ConfigureJobSettings(kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job)
		{
			kCura.Relativity.DataReaderClient.Settings settings = job.Settings;
			settings.ArtifactTypeId = this._transferJobArtifactTypeId;
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
			settings.IdentityFieldId = this._transferJobIdentifierFieldId;
			settings.LoadImportedFullTextFromServer = false;
			settings.MaximumErrorCount = int.MaxValue - 1;
			settings.MoveDocumentsInAppendOverlayMode = false;
			settings.MultiValueDelimiter = ';';
			settings.NativeFileCopyMode = kCura.Relativity.DataReaderClient.NativeFileCopyModeEnum.DoNotImportNativeFiles;
			settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Append;
			settings.SelectedIdentifierFieldName = TransferJobFieldName;
			settings.StartRecordNumber = 0;
		}

		private void CatchJobEvents(kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job)
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

		private int CreateAssociatedDetailInstance(string name)
		{
			return this.CreateObjectTypeInstance(
				this._transferJobDetailArtifactTypeId, 
				new Dictionary<string, object>
				{
					{ TransferJobDetailFieldName, name },
					{ TransferJobDetailFieldTransferredBytes, TestHelper.NextDecimal(100000, 1000000) },
					{ TransferJobDetailFieldTransferredFiles, TestHelper.NextDecimal(1000, 500000) },
					{ TransferJobDetailFieldStartDate, DateTime.Now },
					{ TransferJobDetailFieldEndDate, DateTime.Now.AddDays(3) },
				});
		}

		private int CreateAssociatedDataSourceInstance(string name)
		{
			return this.CreateObjectTypeInstance(
				this._transferJobDataSourceArtifactTypeId,
				new Dictionary<string, object>
				{
					{ TransferJobDataSourceFieldName, name },
					{ TransferJobDataSourceFieldNumber, TestHelper.NextDecimal(1, 100) },
					{ TransferJobDataSourceFieldConnectionString, TestHelper.NextString(50, 450) }
				});
		}

		private void CreateTransferJobDetailObjectType()
		{
			// This is a 1-to-1 relationship.
			int transferJobDetailsArtifactTypeId = this.CreateObjectType(TransferJobDetailArtifactTypeName);
			this._transferJobDetailWorkspaceObjectTypeId = this.QueryWorkspaceObjectTypeDescriptorId(transferJobDetailsArtifactTypeId);
			this.CreateDecimalField(this._transferJobDetailWorkspaceObjectTypeId, TransferJobDetailFieldTransferredBytes);
			this.CreateDecimalField(this._transferJobDetailWorkspaceObjectTypeId, TransferJobDetailFieldTransferredFiles);
			this.CreateDateField(this._transferJobDetailWorkspaceObjectTypeId, TransferJobDetailFieldStartDate);
			this.CreateDateField(this._transferJobDetailWorkspaceObjectTypeId, TransferJobDetailFieldEndDate);
			this._transferJobDetailArtifactTypeId = this.QueryArtifactTypeId(TransferJobDetailArtifactTypeName);
		}

		private void CreateTransferJobDataSourceObjectType()
		{
			// This is a many-to-many relationship.
			int transferJobErrorArtifactTypeId = this.CreateObjectType(TransferJobDataSourceArtifactTypeName);
			this._transferJobDataSourceWorkspaceObjectTypeId = this.QueryWorkspaceObjectTypeDescriptorId(transferJobErrorArtifactTypeId);
			this.CreateDecimalField(this._transferJobDataSourceWorkspaceObjectTypeId, TransferJobDataSourceFieldNumber);
			this.CreateFixedLengthTextField(this._transferJobDataSourceWorkspaceObjectTypeId, TransferJobDataSourceFieldConnectionString, 500);
			this._transferJobDataSourceArtifactTypeId = this.QueryArtifactTypeId(TransferJobDataSourceArtifactTypeName);
		}

		private void CreateTransferJobObjectType()
		{
			int transferJobArtifactTypeId = this.CreateObjectType(TransferJobArtifactTypeName);
			this._transferJobWorkspaceObjectTypeId = this.QueryWorkspaceObjectTypeDescriptorId(transferJobArtifactTypeId);
			this.CreateFixedLengthTextField(this._transferJobWorkspaceObjectTypeId, TransferJobFieldDescription, 500);
			this.CreateSingleObjectField(
				this._transferJobWorkspaceObjectTypeId,
				this._transferJobDetailWorkspaceObjectTypeId,
				TransferJobFieldDetailId);
			this.CreateMultiObjectField(
				this._transferJobWorkspaceObjectTypeId,
				this._transferJobDataSourceWorkspaceObjectTypeId,
				TransferJobFieldDataSourceId);
			this.CreateDecimalField(this._transferJobWorkspaceObjectTypeId, TransferJobFieldRequestBytes);
			this.CreateDecimalField(this._transferJobWorkspaceObjectTypeId, TransferJobFieldRequestFiles);
			this.CreateDateField(this._transferJobWorkspaceObjectTypeId, TransferJobFieldRequestDate);			
			this._transferJobArtifactTypeId = this.QueryArtifactTypeId(TransferJobArtifactTypeName);
			this._transferJobIdentifierFieldId = this.QueryIdentifierFieldId(TransferJobArtifactTypeName);
		}
	}
}