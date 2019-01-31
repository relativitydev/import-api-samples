// ----------------------------------------------------------------------------
// <copyright file="ImportTestsBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.Sample.NUnit.Tests
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;

	using global::NUnit.Framework;

	/// <summary>
	/// Represents an abstract base class object to provide common functionality and helper methods.
	/// </summary>
	public abstract class ImportTestsBase
	{
        /// <summary>
        /// The default bates prefix constant.
        /// </summary>
        protected const string BatesPrefix = "BATES";

        /// <summary>
        /// The control number field name constant.
        /// </summary>
        protected const string ControlNumberFieldName = "control number";

        /// <summary>
        /// The document artifact type name constant.
        /// </summary>
        protected const string DocumentArtifactTypeName = "Document";

        /// <summary>
        /// The file path field name.
        /// </summary>
        protected const string FilePathFieldName = "file path";

        /// <summary>
        /// The file location field name.
        /// </summary>
        protected const string FileLocationFieldName = "file location";

        /// <summary>
        /// The folder field name.
        /// </summary>
        protected const string FolderFieldName = "folder name";

        /// <summary>
        /// The control number field name.
        /// </summary>
        protected const string BatesNumberFieldName = "bates number";

        /// <summary>
		/// The sample PDF file name that's available for testing within the output directory.
		/// </summary>
		protected const string SampleDocPdfFileName = "EDRM-Sample1.pdf";

        /// <summary>
        /// The sample Word doc file name that's available for testing within the output directory.
        /// </summary>
        protected const string SampleDocWordFileName = "EDRM-Sample2.doc";

        /// <summary>
        /// The sample Excel file name that's available for testing within the output directory.
        /// </summary>
        protected const string SampleDocExcelFileName = "EDRM-Sample3.xlsx";

        /// <summary>
        /// The sample MSG file name that's available for testing within the output directory.
        /// </summary>
        protected const string SampleDocMsgFileName = "EDRM-Sample4.msg";

        /// <summary>
        /// The sample HTM file name that's available for testing within the output directory.
        /// </summary>
        protected const string SampleDocHtmFileName = "EDRM-Sample5.htm";

        /// <summary>
        /// The sample EMF file name that's available for testing within the output directory.
        /// </summary>
        protected const string SampleDocEmfFileName = "EDRM-Sample6.emf";

        /// <summary>
        /// The sample PPT file name that's available for testing within the output directory.
        /// </summary>
        protected const string SampleDocPptFileName = "EDRM-Sample7.ppt";

        /// <summary>
        /// The sample PNG file name that's available for testing within the output directory.
        /// </summary>
        protected const string SampleDocPngFileName = "EDRM-Sample8.png";

        /// <summary>
        /// The sample TXT file name that's available for testing within the output directory.
        /// </summary>
        protected const string SampleDocTxtFileName = "EDRM-Sample9.txt";

        /// <summary>
        /// The sample WMF file name that's available for testing within the output directory.
        /// </summary>
        protected const string SampleDocWmfFileName = "EDRM-Sample10.wmf";

        /// <summary>
        /// The sample TIFF file name that's available for testing within the output directory.
        /// </summary>
        protected const string SampleImage1FileName = "EDRM-Sample1.tif";

        /// <summary>
        /// The sample TIFF file name that's available for testing within the output directory.
        /// </summary>
        protected const string SampleImage2FileName = "EDRM-Sample2.tif";

        /// <summary>
        /// The sample TIFF file name that's available for testing within the output directory.
        /// </summary>
        protected const string SampleImage3FileName = "EDRM-Sample3.tif";

        /// <summary>
        /// The sample production image file name.
        /// </summary>
        protected const string SampleProductionImage1FileName = "EDRM-Sample-000001.tif";

        /// <summary>
        /// The well-known document artifact type identifier constant.
        /// </summary>
        protected const int WellKnownDocumentArtifactTypeId = 10;

        /// <summary>
        /// The well-known control number field identifier constant.
        /// </summary>
        protected const int WellKnownControlNumberFieldId = 1003667;

        /// <summary>
        /// The default data source table name constant.
        /// </summary>
        protected const string DefaultDataSourceTableName = "Input Data";

        /// <summary>
        /// The list of all sample document file names available for testing within the output directory.
        /// </summary>
        protected static IEnumerable<string> AllSampleDocFileNames = new[]
        {
            SampleDocPdfFileName,
            SampleDocWordFileName,
            SampleDocExcelFileName,
            SampleDocMsgFileName,
            SampleDocHtmFileName,
            SampleDocEmfFileName,
            SampleDocPptFileName,
            SampleDocPngFileName,
            SampleDocTxtFileName,
            SampleDocWmfFileName
        };

        /// <summary>
        /// The list of all sample image file names available for testing within the output directory.
        /// </summary>
        protected static IEnumerable<string> AllSampleImageFileNames = new[]
        {
            SampleImage1FileName,
            SampleImage2FileName,
            SampleImage3FileName,
            SampleProductionImage1FileName
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportTestsBase"/> class.
        /// </summary>
        protected ImportTestsBase()
			: this(AssemblySetup.Logger)
		{
			// Assume that AssemblySetup has already setup the singleton.
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImportTestsBase"/> class.
		/// </summary>
		/// <param name="log">
		/// The Relativity logger.
		/// </param>
		protected ImportTestsBase(Relativity.Logging.ILog log)
		{
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }

			this.Logger = log;
			Assert.That(this.Logger, Is.Not.Null);
		}

        /// <summary>
        /// Gets the artifact type identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        protected int ArtifactTypeId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the identifier field unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        protected int IdentifierFieldId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the identifier field name.
        /// </summary>
        /// <value>
        /// The field name.
        /// </value>
        protected string IdentifierFieldName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the import data source.
        /// </summary>
        /// <value>
        /// The <see cref="DataTable"/> instance.
        /// </value>
        protected DataTable DataSource
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the Relativity logger.
		/// </summary>
		/// <value>
		/// The <see cref="Relativity.Logging.ILog"/> value.
		/// </value>
		protected Relativity.Logging.ILog Logger
		{
			get;
		}

		/// <summary>
		/// Gets the published errors.
		/// </summary>
		/// <value>
		/// The <see cref="IDictionary"/> instances.
		/// </value>
		public IList<IDictionary> PublishedErrors
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the published fatal exception event.
		/// </summary>
		/// <value>
		/// The <see cref="Exception"/> instance.
		/// </value>
		protected Exception PublishedFatalException
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the published job report.
		/// </summary>
		/// <value>
		/// The <see cref="JobReport"/> value.
		/// </value>
		protected JobReport PublishedJobReport
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the list of published messages.
		/// </summary>
		/// <value>
		/// The messages.
		/// </value>
		protected IList<string> PublishedMessages
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the list of published process progress.
		/// </summary>
		/// <value>
		/// The <see cref="kCura.Relativity.DataReaderClient.FullStatus"/> instances.
		/// </value>
		protected IList<kCura.Relativity.DataReaderClient.FullStatus> PublishedProcessProgress
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the list of published progress row numbers.
		/// </summary>
		/// <value>
		/// The row numbers.
		/// </value>
		protected IList<long> PublishedProgressRows
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the import start time.
		/// </summary>
		/// <value>
		/// The <see cref="DateTime"/> value.
		/// </value>
		protected DateTime StartTime
		{
			get;
			private set;
		}

		[SetUp]
		public void Setup()
		{
			Assert.That(TestSettings.WorkspaceId, Is.Positive);
			this.DataSource = new DataTable(DefaultDataSourceTableName);
			this.PublishedErrors = new List<IDictionary>();
			this.PublishedFatalException = null;
			this.StartTime = DateTime.Now;
			this.PublishedJobReport = null;
			this.PublishedMessages = new List<string>();
			this.PublishedProgressRows = new List<long>();
			this.PublishedProcessProgress = new List<kCura.Relativity.DataReaderClient.FullStatus>();
            this.ArtifactTypeId = this.QueryArtifactTypeId(DocumentArtifactTypeName);
            this.IdentifierFieldId = this.QueryIdentifierFieldId(DocumentArtifactTypeName);
            this.IdentifierFieldName = this.QueryIdentifierFieldName(DocumentArtifactTypeName);
            SetWinEddsConfigValue(false, "CreateFoldersInWebAPI", true);
			this.OnSetup();
		}

		[TearDown]
		public void Teardown()
		{
			DataSource?.Dispose();
			SetWinEddsConfigValue(false, "CreateFoldersInWebAPI", true);
			this.OnTearDown();
		}

		/// <summary>
		/// Creates the import API object using the app config settings for authentication and WebAPI URL.
		/// </summary>
		/// <returns>
		/// The <see cref="kCura.Relativity.ImportAPI.ImportAPI"/> instance.
		/// </returns>
		protected static kCura.Relativity.ImportAPI.ImportAPI CreateImportApiObject()
		{
			return new kCura.Relativity.ImportAPI.ImportAPI(
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.RelativityWebApiUrl.ToString());
		}

		/// <summary>
		/// Finds the date field value within the supplied Relativity object.
		/// </summary>
		/// <param name="relativityObject">
		/// The relativity object.
		/// </param>
		/// <param name="name">
		/// The field name to search.
		/// </param>
		/// <returns>
		/// The <see cref="DateTime"/> value.
		/// </returns>
		protected static DateTime FindDateFieldValue(Relativity.Services.Objects.DataContracts.RelativityObject relativityObject, string name)
		{
			return (DateTime)FindFieldValue(relativityObject, name);
		}

		/// <summary>
		/// Finds the decimal field value within the supplied Relativity object.
		/// </summary>
		/// <param name="relativityObject">
		/// The relativity object.
		/// </param>
		/// <param name="name">
		/// The field name to search.
		/// </param>
		/// <returns>
		/// The <see cref="decimal"/> value.
		/// </returns>
		protected static decimal FindDecimalFieldValue(Relativity.Services.Objects.DataContracts.RelativityObject relativityObject, string name)
		{
			return (decimal)FindFieldValue(relativityObject, name);
		}

		/// <summary>
		/// Finds the single-object field value within the supplied Relativity object.
		/// </summary>
		/// <param name="relativityObject">
		/// The relativity object.
		/// </param>
		/// <param name="name">
		/// The field name to search.
		/// </param>
		/// <returns>
		/// The <see cref="Relativity.Services.Objects.DataContracts.RelativityObjectValue"/> instance.
		/// </returns>
		protected static Relativity.Services.Objects.DataContracts.RelativityObjectValue FindSingleObjectFieldValue(Relativity.Services.Objects.DataContracts.RelativityObject relativityObject, string name)
		{
			return FindFieldValue(relativityObject, name) as Relativity.Services.Objects.DataContracts.RelativityObjectValue;
		}

		/// <summary>
		/// Finds the multi-object field values within the supplied Relativity object.
		/// </summary>
		/// <param name="relativityObject">
		/// The relativity object.
		/// </param>
		/// <param name="name">
		/// The field name to search.
		/// </param>
		/// <returns>
		/// The <see cref="Relativity.Services.Objects.DataContracts.RelativityObjectValue"/> instances.
		/// </returns>
		protected static List<Relativity.Services.Objects.DataContracts.RelativityObjectValue> FindMultiObjectFieldValues(Relativity.Services.Objects.DataContracts.RelativityObject relativityObject, string name)
		{
			return FindFieldValue(relativityObject, name) as List<Relativity.Services.Objects.DataContracts.RelativityObjectValue>;
		}

		/// <summary>
		/// Finds the object field value within the supplied Relativity object.
		/// </summary>
		/// <param name="relativityObject">
		/// The relativity object.
		/// </param>
		/// <param name="name">
		/// The field name to search.
		/// </param>
		/// <returns>
		/// The field value.
		/// </returns>
		protected static object FindFieldValue(Relativity.Services.Objects.DataContracts.RelativityObject relativityObject, string name)
		{
			Relativity.Services.Objects.DataContracts.FieldValuePair pair = relativityObject.FieldValues.FirstOrDefault(x => x.Field.Name == name);
			return pair?.Value;
		}

		/// <summary>
		/// Finds the object whose identity name and value match the specified values.
		/// </summary>
		/// <param name="objects">
		/// The relativity object.
		/// </param>
		/// <param name="identityFieldName">
		/// The identity field name to search.
		/// </param>
		/// <param name="identityFieldValue">
		/// The identity field value to search.
		/// </param>
		/// <returns>
		/// The <see cref="Relativity.Services.Objects.DataContracts.RelativityObject"/> instance.
		/// </returns>
		protected static Relativity.Services.Objects.DataContracts.RelativityObject FindRelativityObject(IList<Relativity.Services.Objects.DataContracts.RelativityObject> objects, string identityFieldName, string identityFieldValue)
		{
			return (from obj in objects
				from pair in obj.FieldValues
				where pair.Field.Name == identityFieldName && pair.Value.ToString() == identityFieldValue
				select obj).FirstOrDefault();
		}

		/// <summary>
		/// Finds the string field value within the supplied Relativity object.
		/// </summary>
		/// <param name="relativityObject">
		/// The relativity object.
		/// </param>
		/// <param name="name">
		/// The field name.
		/// </param>
		/// <returns>
		/// The field value.
		/// </returns>
		protected static string FindStringFieldValue(Relativity.Services.Objects.DataContracts.RelativityObject relativityObject, string name)
		{
			return FindFieldValue(relativityObject, name) as string;
		}

        /// <summary>
        /// Generates a unique production set name.
        /// </summary>
        /// <returns>
        /// The name.
        /// </returns>
        protected static string GenerateProductionSetName()
        {
            return $"ProductionSet-{Guid.NewGuid()}";
        }

        /// <summary>
        /// Generates a unique bates number with a BATES prefix.
        /// </summary>
        /// <returns>
        /// The bates number.
        /// </returns>
        protected static string GenerateBatesNumber()
		{
			return $"{BatesPrefix}-{Guid.NewGuid()}";
		}

		/// <summary>
		/// Generates a unique control number with a REL prefix.
		/// </summary>
		/// <returns>
		/// The control number.
		/// </returns>
		protected static string GenerateControlNumber()
		{
			return $"REL-{Guid.NewGuid()}";
		}

		/// <summary>
		/// Sets a WinEDDS-based configuration value.
		/// </summary>
		/// <param name="log">
		/// Specify whether to log the configuration assignment.
		/// </param>
		/// <param name="key">
		/// The configuration key name.
		/// </param>
		/// <param name="value">
		/// The configuration value name.
		/// </param>
		protected static void SetWinEddsConfigValue(bool log, string key, object value)
		{
			System.Collections.IDictionary configDictionary = kCura.WinEDDS.Config.ConfigSettings;
			if (configDictionary.Contains(key))
			{
				configDictionary[key] = value;
			}
			else
			{
				configDictionary.Add(key, value);
			}

			if (log)
			{
				System.Console.WriteLine($"{key}={value}");
			}
		}

        protected static void ConfigureDocumentJobSettings(kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job)
        {
            ConfigureJobSettings(
                job,
                WellKnownDocumentArtifactTypeId,
                WellKnownControlNumberFieldId,
                FilePathFieldName,
                ControlNumberFieldName,
                null);
        }

        protected static void ConfigureJobSettings(
            kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job,
            int artifactTypeId,
            int identityFieldId,
            string nativeFilePathSourceFieldName,
            string identifierFieldName,
            string folderFieldName)
        {
            kCura.Relativity.DataReaderClient.Settings settings = job.Settings;
            settings.ArtifactTypeId = artifactTypeId;
            settings.Billable = false;
            settings.BulkLoadFileFieldDelimiter = ";";
            settings.CaseArtifactId = TestSettings.WorkspaceId;
            settings.CopyFilesToDocumentRepository = true;
            settings.DisableControlNumberCompatibilityMode = true;
            settings.DisableExtractedTextFileLocationValidation = false;
            settings.DisableNativeLocationValidation = false;
            settings.DisableNativeValidation = false;
            settings.ExtractedTextEncoding = System.Text.Encoding.Unicode;
            settings.ExtractedTextFieldContainsFilePath = false;
            settings.FileSizeColumn = "NativeFileSize";
            settings.FileSizeMapped = true;
            settings.FolderPathSourceFieldName = folderFieldName;
            settings.IdentityFieldId = identityFieldId;
            settings.LoadImportedFullTextFromServer = false;
            settings.MaximumErrorCount = int.MaxValue - 1;
            settings.MoveDocumentsInAppendOverlayMode = false;
            settings.NativeFileCopyMode = kCura.Relativity.DataReaderClient.NativeFileCopyModeEnum.CopyFiles;
            settings.NativeFilePathSourceFieldName = nativeFilePathSourceFieldName;
            settings.OIFileIdColumnName = "OutsideInFileId";
            settings.OIFileIdMapped = true;
            settings.OIFileTypeColumnName = "OutsideInFileType";
            settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Append;
            settings.SelectedIdentifierFieldName = identifierFieldName;
            settings.StartRecordNumber = 0;
        }

        protected void CreateDateField(int workspaceObjectTypeId, string fieldName)
		{
			kCura.Relativity.Client.DTOs.Field field = new kCura.Relativity.Client.DTOs.Field
			{
				AllowGroupBy = false,
				AllowPivot = false,
				AllowSortTally = false,
				FieldTypeID = kCura.Relativity.Client.FieldType.Date,
				IgnoreWarnings = true,
				IsRequired = false,
				Linked = false,
				Name = fieldName,
				OpenToAssociations = false,
				Width = "12",
				Wrapping = true
			};

			TestHelper.CreateField(
				TestSettings.RelativityRestUrl,
				TestSettings.RelativityServicesUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.WorkspaceId,
				workspaceObjectTypeId,
				field);
		}

		protected void CreateDecimalField(int workspaceObjectTypeId, string fieldName)
		{
			kCura.Relativity.Client.DTOs.Field field = new kCura.Relativity.Client.DTOs.Field
			{
				AllowGroupBy = false,
				AllowPivot = false,
				AllowSortTally = false,
				FieldTypeID = kCura.Relativity.Client.FieldType.Decimal,
				IgnoreWarnings = true,
				IsRequired = false,
				Linked = false,
				Name = fieldName,
				OpenToAssociations = false,
				Width = "12",
				Wrapping = true
			};

			TestHelper.CreateField(
				TestSettings.RelativityRestUrl,
				TestSettings.RelativityServicesUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.WorkspaceId,
				workspaceObjectTypeId,
				field);
		}

		protected void CreateFixedLengthTextField(int workspaceObjectTypeId, string fieldName, int length)
		{
			kCura.Relativity.Client.DTOs.Field field = new kCura.Relativity.Client.DTOs.Field
			{
				AllowGroupBy = false,
				AllowHTML = false,
				AllowPivot = false,
				AllowSortTally = false,
				FieldTypeID = kCura.Relativity.Client.FieldType.FixedLengthText,
				Length = length,
				IgnoreWarnings = true,
				IncludeInTextIndex = true,
				IsRequired = false,
				Linked = false,
				Name = fieldName,
				OpenToAssociations = false,
				Unicode = false,
				Width = "",
				Wrapping = false
			};

			TestHelper.CreateField(
				TestSettings.RelativityRestUrl,
				TestSettings.RelativityServicesUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.WorkspaceId,
				workspaceObjectTypeId,
				field);
		}

		protected void CreateSingleObjectField(int workspaceObjectTypeId, int descriptorArtifactTypeId, string fieldName)
		{
			kCura.Relativity.Client.DTOs.Field field = new kCura.Relativity.Client.DTOs.Field
			{
				AllowGroupBy = false,
				AllowPivot = false,
				AllowSortTally = false,
				AssociativeObjectType = new kCura.Relativity.Client.DTOs.ObjectType { DescriptorArtifactTypeID = descriptorArtifactTypeId },
				FieldTypeID = kCura.Relativity.Client.FieldType.SingleObject,
				IgnoreWarnings = true,
				IsRequired = false,
				Linked = false,
				Name = fieldName,
				OpenToAssociations = false,
				Width = "12",
				Wrapping = false
			};

			TestHelper.CreateField(
				TestSettings.RelativityRestUrl,
				TestSettings.RelativityServicesUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.WorkspaceId,
				workspaceObjectTypeId,
				field);
		}

		protected void CreateMultiObjectField(int workspaceObjectTypeId, int descriptorArtifactTypeId, string fieldName)
		{
			kCura.Relativity.Client.DTOs.Field field = new kCura.Relativity.Client.DTOs.Field
			{
				AllowGroupBy = false,
				AllowPivot = false,
				AssociativeObjectType = new kCura.Relativity.Client.DTOs.ObjectType { DescriptorArtifactTypeID = descriptorArtifactTypeId },
				FieldTypeID = kCura.Relativity.Client.FieldType.MultipleObject,
				IgnoreWarnings = true,
				IsRequired = false,
				Name = fieldName,
				Width = "12"
			};

			TestHelper.CreateField(
				TestSettings.RelativityRestUrl,
				TestSettings.RelativityServicesUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.WorkspaceId,
				workspaceObjectTypeId,
				field);
		}

		protected int CreateObjectType(string objectTypeName)
		{
			int artifactId = TestHelper.CreateObjectType(
				TestSettings.RelativityRestUrl,
				TestSettings.RelativityServicesUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.WorkspaceId,
				objectTypeName);
			this.Logger.LogInformation(
				"Successfully created object type '{ObjectTypeName}' - {ArtifactId}.",
				objectTypeName, artifactId);
			return artifactId;
		}

		protected int CreateObjectTypeInstance(int artifactTypeId, IDictionary<string, object> fields)
		{
			int artifactId = TestHelper.CreateObjectTypeInstance(
				TestSettings.RelativityRestUrl,
				TestSettings.RelativityServicesUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.WorkspaceId,
				artifactTypeId,
				fields);
			this.Logger.LogInformation("Successfully created instance {ArtifactId} of object type {ArtifactTypeId}.",
				artifactId, artifactTypeId);
			return artifactId;
		}

        protected int CreateProduction(string productionName, string batesPrefix)
        {
            int artifactId = TestHelper.CreateProduction(
                TestSettings.RelativityRestUrl,
                TestSettings.RelativityServicesUrl,
                TestSettings.RelativityUserName,
                TestSettings.RelativityPassword,
                TestSettings.WorkspaceId,
                productionName,
                batesPrefix,
                this.Logger);
            this.Logger.LogInformation("Successfully created production {ProductionName} - {ArtifactId}.",
                productionName, artifactId);
            return artifactId;
        }

        protected void DeleteObjects(IList<int> artifacts)
		{
			foreach (int artifactId in artifacts.ToList())
			{
				this.DeleteObject(artifactId);
				artifacts.Remove(artifactId);
			}
		}

		protected void DeleteObject(int artifactId)
		{
			TestHelper.DeleteObject(
				TestSettings.RelativityRestUrl,
				TestSettings.RelativityServicesUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.WorkspaceId,
				artifactId);
		}

        /// <summary>
        /// Imports a list of documents without copying native files.
        /// </summary>
        /// <param name="controlNumbers">
        /// The list of control numbers to import.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// The exception thrown when the import fails.
        /// </exception>
        protected void ImportDocuments(IEnumerable<string> controlNumbers)
        {
            kCura.Relativity.ImportAPI.ImportAPI importApi = CreateImportApiObject();
            kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job = importApi.NewNativeDocumentImportJob();
            ConfigureDocumentJobSettings(job);
            job.Settings.CopyFilesToDocumentRepository = false;
            job.Settings.NativeFileCopyMode = kCura.Relativity.DataReaderClient.NativeFileCopyModeEnum.DoNotImportNativeFiles;
            job.Settings.NativeFilePathSourceFieldName = null;
            var dt = new DataTable(DefaultDataSourceTableName);
            dt.Columns.Add(this.IdentifierFieldName);
            foreach (var controlNumber in controlNumbers)
            {
                DataRow dr = dt.NewRow();
                dr[this.IdentifierFieldName] = controlNumber;
                dt.Rows.Add(dr);
            }

            job.SourceData.SourceData = dt.CreateDataReader();
            job.OnFatalException += report => throw report.FatalException;
            job.OnComplete += report =>
            {
                if (report.FatalException != null)
                {
                    throw report.FatalException;
                }

                if (report.ErrorRowCount > 0)
                {
                    IEnumerable<string> errors = report.ErrorRows.Select(x => $"{x.Identifier} - {x.Message}");
                    throw new InvalidOperationException(string.Join("\n", errors));
                }
            };

            job.Execute();
        }

		protected int QueryArtifactTypeId(string objectTypeName)
		{
			return TestHelper.QueryArtifactTypeId(
				TestSettings.RelativityRestUrl,
				TestSettings.RelativityServicesUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.WorkspaceId,
				objectTypeName);
		}

		protected int QueryIdentifierFieldId(string artifactTypeName)
		{
			return TestHelper.QueryIdentifierFieldId(
				TestSettings.RelativityRestUrl,
				TestSettings.RelativityServicesUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.WorkspaceId,
				artifactTypeName);
		}

        protected string GetDocumentIdentifierFieldName()
        {
            return this.QueryIdentifierFieldName(DocumentArtifactTypeName);
        }

        protected string QueryIdentifierFieldName(string artifactTypeName)
        {
            return TestHelper.QueryIdentifierFieldName(
                TestSettings.RelativityRestUrl,
                TestSettings.RelativityServicesUrl,
                TestSettings.RelativityUserName,
                TestSettings.RelativityPassword,
                TestSettings.WorkspaceId,
                artifactTypeName);
        }

        protected Tuple<string, string> QueryProductionBatesNumbers(int productionId)
        {
            var production = TestHelper.QueryProduction(
                TestSettings.RelativityRestUrl,
                TestSettings.RelativityServicesUrl,
                TestSettings.RelativityUserName,
                TestSettings.RelativityPassword,
                TestSettings.WorkspaceId,
                productionId);
            Tuple<string, string> batesNumbers =
                new Tuple<string, string>(production.Details.FirstBatesValue, production.Details.LastBatesValue);
            return batesNumbers;
        }

        protected int QueryRelativityObjectCount(int artifactTypeId)
		{
			return TestHelper.QueryRelativityObjectCount(
				TestSettings.RelativityRestUrl,
				TestSettings.RelativityServicesUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword, TestSettings.WorkspaceId,
				artifactTypeId);
		}

		protected IList<Relativity.Services.Objects.DataContracts.RelativityObject> QueryRelativityObjects(int artifactTypeId, IEnumerable<string> fields)
		{
			return TestHelper.QueryRelativityObjects(
				TestSettings.RelativityRestUrl,
				TestSettings.RelativityServicesUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.WorkspaceId,
				artifactTypeId,
				fields);
		}

		protected IList<string> QueryWorkspaceFolders()
		{
			return TestHelper.QueryWorkspaceFolders(
				TestSettings.RelativityRestUrl,
				TestSettings.RelativityServicesUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.WorkspaceId,
				this.Logger);
		}

		protected int QueryWorkspaceObjectTypeDescriptorId(int artifactId)
		{
			return TestHelper.QueryWorkspaceObjectTypeDescriptorId(
				TestSettings.RelativityRestUrl,
				TestSettings.RelativityServicesUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.WorkspaceId,
				artifactId);
		}

		protected Relativity.Services.Objects.DataContracts.RelativityObject ReadRelativityObject(int artifactId,
			IEnumerable<string> fields)
		{
			return TestHelper.ReadRelativityObject(
				TestSettings.RelativityRestUrl,
				TestSettings.RelativityServicesUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.WorkspaceId,
				artifactId,
				fields);
		}

		protected virtual void OnSetup()
		{
		}

		protected virtual void OnTearDown()
		{
		}
	}
}