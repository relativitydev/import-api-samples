// ----------------------------------------------------------------------------
// <copyright file="ImportTestsBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.Samples.NUnit.Tests
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using System.Net;

	using global::NUnit.Framework;

	using Relativity.Import.Export.TestFramework;

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
        /// The default data source table name constant.
        /// </summary>
        protected const string DefaultDataSourceTableName = "Input Data";

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportTestsBase"/> class.
        /// </summary>
        protected ImportTestsBase()
			: this(IntegrationTestHelper.Logger)
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
		/// Gets the list of all sample document file names available for testing within the output directory.
		/// </summary>
		/// <value>
		/// The file names.
		/// </value>
		protected static IReadOnlyList<string> AllSampleDocFileNames =>
			new List<string>
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
		/// Gets the list of all sample image file names available for testing within the output directory.
		/// </summary>
		/// <value>
		/// The file names.
		/// </value>
		protected static IReadOnlyList<string> AllSampleImageFileNames =>
			new List<string>
				{
					SampleImage1FileName,
					SampleImage2FileName,
					SampleImage3FileName,
					SampleProductionImage1FileName
				};

		/// <summary>
		/// Gets or sets the artifact type identifier.
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
        /// Gets or sets the identifier field unique identifier.
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
        /// Gets or sets the identifier field name.
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
		protected IList<IDictionary> PublishedErrors
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
			private set;
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

		/// <summary>
		/// Gets integration test parameters.
		/// </summary>
		/// <value>
		/// The <see cref="IntegrationTestParameters"/> instance.
		/// </value>
		protected IntegrationTestParameters TestParameters
		{
			get;
			private set;
		}

		[SetUp]
		public void Setup()
		{
			ServicePointManager.SecurityProtocol =
				SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11
				| SecurityProtocolType.Tls12;
			this.AssignTestSettings();
			Assert.That(
				this.TestParameters.WorkspaceId,
				Is.Positive,
				() => "The test workspace must be created or specified in order to run this sample test.");
			this.DataSource = new DataTable(DefaultDataSourceTableName);
			this.PublishedErrors = new List<IDictionary>();
			this.PublishedFatalException = null;
			this.StartTime = DateTime.Now;
			this.PublishedJobReport = null;
			this.PublishedMessages = new List<string>();
			this.PublishedProgressRows = new List<long>();
			this.PublishedProcessProgress = new List<kCura.Relativity.DataReaderClient.FullStatus>();
            this.ArtifactTypeId = this.QueryArtifactTypeId(WellKnownArtifactTypes.DocumentArtifactTypeName);
            this.IdentifierFieldId = this.QueryIdentifierFieldId(WellKnownArtifactTypes.DocumentArtifactTypeName);
            this.IdentifierFieldName = this.QueryIdentifierFieldName(WellKnownArtifactTypes.DocumentArtifactTypeName);
            SetWinEddsConfigValue(false, "CreateFoldersInWebAPI", true);
			this.OnSetup();
		}

		[TearDown]
		public void Teardown()
		{
			this.DataSource?.Dispose();
			SetWinEddsConfigValue(false, "CreateFoldersInWebAPI", true);
			this.OnTearDown();
		}

		/// <summary>
		/// Gets the boolean field value for the supplied RDO.
		/// </summary>
		/// <param name="relativityObject">
		/// The relativity object.
		/// </param>
		/// <param name="name">
		/// The field name to search.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/> value.
		/// </returns>
		protected static bool GetBooleanFieldValue(Relativity.Services.Objects.DataContracts.RelativityObject relativityObject, string name)
		{
			object field = GetObjectFieldValue(relativityObject, name);
			if (field == null)
			{
				throw new InvalidOperationException($"The field '{name}' is expected to be a boolean field but was null.");
			}

			return Convert.ToBoolean(field);
		}

		/// <summary>
		/// Gets the choice field value for the supplied RDO.
		/// </summary>
		/// <param name="relativityObject">
		/// The relativity object.
		/// </param>
		/// <param name="name">
		/// The field name to search.
		/// </param>
		/// <returns>
		/// The <see cref="Relativity.Services.Objects.DataContracts.Choice"/> instance.
		/// </returns>
		protected static Relativity.Services.Objects.DataContracts.Choice GetChoiceField(Relativity.Services.Objects.DataContracts.RelativityObject relativityObject, string name)
		{
			object value = GetObjectFieldValue(relativityObject, name);
			if (value == null)
			{
				throw new InvalidOperationException($"The field '{name}' is expected to be a choice field but is null.");
			}

			return value as Relativity.Services.Objects.DataContracts.Choice;
		}

		/// <summary>
		/// Gets the date field value for the supplied RDO.
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
		protected static DateTime GetDateFieldValue(Relativity.Services.Objects.DataContracts.RelativityObject relativityObject, string name)
		{
			object value = GetObjectFieldValue(relativityObject, name);
			if (value == null)
			{
				throw new InvalidOperationException($"The field '{name}' is expected to be a Date-Time field but is null.");
			}

			return Convert.ToDateTime(value);
		}

		/// <summary>
		/// Gets the decimal field value for the supplied RDO.
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
		protected static decimal GetDecimalFieldValue(Relativity.Services.Objects.DataContracts.RelativityObject relativityObject, string name)
		{
			object value = GetObjectFieldValue(relativityObject, name);
			if (value == null)
			{
				throw new InvalidOperationException($"The field '{name}' is expected to be a decimal field but is null.");
			}

			return Convert.ToDecimal(value);
		}

		/// <summary>
		/// Gets the 32-bit integer field value for the supplied RDO.
		/// </summary>
		/// <param name="relativityObject">
		/// The relativity object.
		/// </param>
		/// <param name="name">
		/// The field name to search.
		/// </param>
		/// <returns>
		/// The nullable <see cref="int"/> value.
		/// </returns>
		protected static int? GetInt32FieldValue(Relativity.Services.Objects.DataContracts.RelativityObject relativityObject, string name)
		{
			object field = GetObjectFieldValue(relativityObject, name);
			if (field == null)
			{
				return null;
			}

			return Convert.ToInt32(field);
		}

		/// <summary>
		/// Gets the multi-object field value for the supplied RDO.
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
		protected static List<Relativity.Services.Objects.DataContracts.RelativityObjectValue> GetMultiObjectFieldValues(Relativity.Services.Objects.DataContracts.RelativityObject relativityObject, string name)
		{
			object value = GetObjectFieldValue(relativityObject, name);
			if (value == null)
			{
				throw new InvalidOperationException($"The field '{name}' is expected to be a multi-object field but is null.");
			}

			return value as List<Relativity.Services.Objects.DataContracts.RelativityObjectValue>;
		}

		/// <summary>
		/// Gets the object field value for the supplied RDO.
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
		protected static object GetObjectFieldValue(Relativity.Services.Objects.DataContracts.RelativityObject relativityObject, string name)
		{
			Relativity.Services.Objects.DataContracts.FieldValuePair pair = relativityObject.FieldValues.FirstOrDefault(x => x.Field.Name == name);
			return pair?.Value;
		}

		/// <summary>
		/// Gets the single-object field value for the supplied RDO.
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
		protected static Relativity.Services.Objects.DataContracts.RelativityObjectValue GetSingleObjectFieldValue(Relativity.Services.Objects.DataContracts.RelativityObject relativityObject, string name)
		{
			object value = GetObjectFieldValue(relativityObject, name);
			if (value == null)
			{
				throw new InvalidOperationException($"The field '{name}' is expected to be a single-object field but is null.");
			}

			return value as Relativity.Services.Objects.DataContracts.RelativityObjectValue;
		}

		/// <summary>
		/// Gets the string field value for the supplied RDO.
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
		protected static string GetStringFieldValue(Relativity.Services.Objects.DataContracts.RelativityObject relativityObject, string name)
		{
			// This can be null.
			return GetObjectFieldValue(relativityObject, name) as string;
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

		/// <summary>
		/// Searches the list of RDO's for the object whose field name and value match the specified parameters.
		/// </summary>
		/// <param name="objects">
		/// The relativity object.
		/// </param>
		/// <param name="fieldName">
		/// The field name to search.
		/// </param>
		/// <param name="fieldValue">
		/// The field value to search.
		/// </param>
		/// <returns>
		/// The <see cref="Relativity.Services.Objects.DataContracts.RelativityObject"/> instance.
		/// </returns>
		protected static Relativity.Services.Objects.DataContracts.RelativityObject SearchRelativityObject(
			IList<Relativity.Services.Objects.DataContracts.RelativityObject> objects,
			string fieldName,
			string fieldValue)
		{
			return (from obj in objects
			        from pair in obj.FieldValues
			        where pair.Field.Name == fieldName && pair.Value.ToString() == fieldValue
			        select obj).FirstOrDefault();
		}

		/// <summary>
		/// Assign the test parameters. This should always be called from methods with <see cref="SetUpAttribute"/> or <see cref="OneTimeSetUpAttribute"/>.
		/// </summary>
		protected void AssignTestSettings()
		{
			if (this.TestParameters == null)
			{
				this.TestParameters = AssemblySetup.TestParameters.DeepCopy();
			}
		}

		protected void ConfigureDocumentJobSettings(kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job)
        {
            this.ConfigureJobSettings(
                job,
				WellKnownArtifactTypes.DocumentArtifactTypeId,
				WellKnownFields.ControlNumberId,
				WellKnownFields.FilePath,
                WellKnownFields.ControlNumber,
                null);
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
            settings.Billable = false;
            settings.BulkLoadFileFieldDelimiter = ";";
            settings.CaseArtifactId = this.TestParameters.WorkspaceId;
            settings.CopyFilesToDocumentRepository = true;
            settings.DisableControlNumberCompatibilityMode = true;
            settings.DisableExtractedTextFileLocationValidation = false;
            settings.DisableNativeLocationValidation = false;
            settings.DisableNativeValidation = false;
            settings.ExtractedTextEncoding = System.Text.Encoding.Unicode;
            settings.ExtractedTextFieldContainsFilePath = false;
            settings.FileSizeColumn = WellKnownFields.NativeFileSize;
            settings.FileSizeMapped = true;
            settings.FolderPathSourceFieldName = folderFieldName;
            settings.IdentityFieldId = identityFieldId;
            settings.LoadImportedFullTextFromServer = false;
            settings.MaximumErrorCount = int.MaxValue - 1;
            settings.MoveDocumentsInAppendOverlayMode = false;
            settings.NativeFileCopyMode = kCura.Relativity.DataReaderClient.NativeFileCopyModeEnum.CopyFiles;
            settings.NativeFilePathSourceFieldName = nativeFilePathSourceFieldName;
            settings.OIFileIdColumnName = WellKnownFields.OutsideInFileId;
            settings.OIFileIdMapped = true;
            settings.OIFileTypeColumnName = WellKnownFields.OutsideInFileType;
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

			FieldHelper.CreateField(this.TestParameters, workspaceObjectTypeId, field);
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

			FieldHelper.CreateField(this.TestParameters, workspaceObjectTypeId, field);
		}

		/// <summary>
		/// Creates the export search manager.
		/// </summary>
		/// <returns>
		/// The <see cref="kCura.WinEDDS.Service.Export.ISearchManager"/> instance.
		/// </returns>
		/// <remarks>
		/// The <see cref="kCura.WinEDDS.Service.Export.ISearchManager"/> service is not officially supported and should NEVER be used in production code.
		/// </remarks>
		protected kCura.WinEDDS.Service.Export.ISearchManager CreateExportSearchManager()
		{
			var credentials = new NetworkCredential(this.TestParameters.RelativityUserName, this.TestParameters.RelativityPassword);
			return new kCura.WinEDDS.Service.SearchManager(credentials, new CookieContainer());
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
				Width = string.Empty,
				Wrapping = false
			};

			FieldHelper.CreateField(this.TestParameters, workspaceObjectTypeId, field);
		}

		/// <summary>
		/// Creates the import API object using the app config parameters for authentication and WebAPI URL.
		/// </summary>
		/// <returns>
		/// The <see cref="kCura.Relativity.ImportAPI.ImportAPI"/> instance.
		/// </returns>
		protected kCura.Relativity.ImportAPI.ImportAPI CreateImportApiObject()
		{
			return new kCura.Relativity.ImportAPI.ImportAPI(
				this.TestParameters.RelativityUserName,
				this.TestParameters.RelativityPassword,
				this.TestParameters.RelativityWebApiUrl.ToString());
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

			FieldHelper.CreateField(this.TestParameters, workspaceObjectTypeId, field);
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

			FieldHelper.CreateField(this.TestParameters, workspaceObjectTypeId, field);
		}

		protected int CreateObjectType(string objectTypeName)
		{
			int artifactId = RdoHelper.CreateObjectType(this.TestParameters, objectTypeName);
			this.Logger.LogInformation(
				"Successfully created object type '{ObjectTypeName}' - {ArtifactId}.",
				objectTypeName,
				artifactId);
			return artifactId;
		}

		protected int CreateObjectTypeInstance(int artifactTypeId, IDictionary<string, object> fields)
		{
			int artifactId = RdoHelper.CreateObjectTypeInstance(this.TestParameters, artifactTypeId, fields);
			this.Logger.LogInformation(
				"Successfully created instance {ArtifactId} of object type {ArtifactTypeId}.",
				artifactId,
				artifactTypeId);
			return artifactId;
		}

        protected int CreateProduction(string productionName, string batesPrefix)
        {
            int artifactId = ProductionHelper.CreateProduction(this.TestParameters, productionName, batesPrefix, this.Logger);
            this.Logger.LogInformation(
	            "Successfully created production {ProductionName} - {ArtifactId}.",
                productionName,
	            artifactId);
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
			RdoHelper.DeleteObject(this.TestParameters, artifactId);
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
            kCura.Relativity.ImportAPI.ImportAPI importApi = this.CreateImportApiObject();
            kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job = importApi.NewNativeDocumentImportJob();
            this.ConfigureDocumentJobSettings(job);
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
			return RdoHelper.QueryArtifactTypeId(this.TestParameters, objectTypeName);
		}

		protected int QueryIdentifierFieldId(string artifactTypeName)
		{
			return FieldHelper.QueryIdentifierFieldId(this.TestParameters, artifactTypeName);
		}

        protected string GetDocumentIdentifierFieldName()
        {
            return this.QueryIdentifierFieldName(WellKnownArtifactTypes.DocumentArtifactTypeName);
        }

        protected string QueryIdentifierFieldName(string artifactTypeName)
        {
            return FieldHelper.QueryIdentifierFieldName(this.TestParameters, artifactTypeName);
        }

		/// <summary>
		/// Queries for the list of file information associated with the specified imported image.
		/// </summary>
		/// <param name="artifactId">
		/// The imported image artifact unique identifier.
		/// </param>
		/// <returns>
		/// The <see cref="FileDto"/> instances.
		/// </returns>
		/// <remarks>
		/// The <see cref="kCura.WinEDDS.Service.Export.ISearchManager"/> service is not officially supported and should NEVER be used in production code.
		/// </remarks>
		protected IEnumerable<FileDto> QueryImageFileInfo(int artifactId)
        {
			using (kCura.WinEDDS.Service.Export.ISearchManager searchManager = this.CreateExportSearchManager())
			{
				var ds = searchManager.RetrieveImagesForDocuments(
					this.TestParameters.WorkspaceId,
					new[] { artifactId });
		        if (ds == null || ds.Tables.Count == 0)
		        {
			        return new List<FileDto>();
		        }

		        DataTable table = ds.Tables[0];
				return table.Rows.Cast<DataRow>().Select(x => new FileDto(x));
	        }
        }

		/// <summary>
		/// Queries for the file information associated with the specified native document.
		/// </summary>
		/// <param name="artifactId">
		/// The imported document artifact unique identifier.
		/// </param>
		/// <returns>
		/// The <see cref="FileDto"/> instance.
		/// </returns>
		/// <remarks>
		/// The <see cref="kCura.WinEDDS.Service.Export.ISearchManager"/> service is not officially supported and should NEVER be used in production code.
		/// </remarks>
		protected FileDto QueryNativeFileInfo(int artifactId)
		{
			using (kCura.WinEDDS.Service.Export.ISearchManager searchManager = this.CreateExportSearchManager())
			{
				var ds = searchManager.RetrieveNativesForSearch(this.TestParameters.WorkspaceId, artifactId.ToString());
				if (ds == null || ds.Tables.Count == 0)
				{
					return null;
				}

				DataTable table = ds.Tables[0];
				if (table.Rows.Count != 1)
				{
					throw new InvalidOperationException("The search manager natives endpoint was expected to return exactly 1 row.");
				}

				return table.Rows.Cast<DataRow>().Select(x => new FileDto(x)).Single();
			}
		}

		/// <summary>
		/// Queries for a structure that provides the first and last Bates numbers for the specified production.
		/// </summary>
		/// <param name="productionId">
		/// The production artifact identifier.
		/// </param>
		/// <returns>
		/// The tuple.
		/// </returns>
		protected Tuple<string, string> QueryProductionBatesNumbers(int productionId)
        {
            var production = ProductionHelper.QueryProduction(this.TestParameters, productionId);
            Tuple<string, string> batesNumbers =
                new Tuple<string, string>(production.Details.FirstBatesValue, production.Details.LastBatesValue);
            return batesNumbers;
        }

		/// <summary>
		/// Queries for the total number of objects for the specified RDO type.
		/// </summary>
		/// <param name="artifactTypeId">
		/// The RDO artifact type identifier.
		/// </param>
		/// <returns>
		/// The total number of objects.
		/// </returns>
		protected int QueryRelativityObjectCount(int artifactTypeId)
		{
			return RdoHelper.QueryRelativityObjectCount(this.TestParameters, artifactTypeId);
		}

		/// <summary>
		/// Queries for the list of document RDO's and includes the standard well-known fields.
		/// </summary>
		/// <returns>
		/// The <see cref="Relativity.Services.Objects.DataContracts.RelativityObject"/> instance.
		/// </returns>
		protected IList<Relativity.Services.Objects.DataContracts.RelativityObject> QueryDocuments()
		{
			return this.QueryDocuments(
				new string[]
					{
						WellKnownFields.ArtifactId, WellKnownFields.ControlNumber, WellKnownFields.HasImages,
						WellKnownFields.HasNative, WellKnownFields.BatesNumber, WellKnownFields.RelativityImageCount
					});
		}

		/// <summary>
		/// Queries for the list of document RDO's and includes the specified list of fields.
		/// </summary>
		/// <param name="fields">The fields.</param>
		/// <returns>
		/// The <see cref="Relativity.Services.Objects.DataContracts.RelativityObject"/> instance.
		/// </returns>
		protected IList<Relativity.Services.Objects.DataContracts.RelativityObject> QueryDocuments(IEnumerable<string> fields)
		{
			return this.QueryRelativityObjects(WellKnownArtifactTypes.DocumentArtifactTypeId, fields);
		}

		/// <summary>
		/// Queries for the list of RDO's of the specified type and includes the specified list of fields.
		/// </summary>
		/// <param name="artifactTypeId">
		/// The artifact type identifier that specifies the RDO type.
		/// </param>
		/// <param name="fields">The fields.</param>
		/// <returns>
		/// The <see cref="Relativity.Services.Objects.DataContracts.RelativityObject"/> instance.
		/// </returns>
		protected IList<Relativity.Services.Objects.DataContracts.RelativityObject> QueryRelativityObjects(int artifactTypeId, IEnumerable<string> fields)
		{
			return RdoHelper.QueryRelativityObjects(this.TestParameters, artifactTypeId, fields);
		}

		protected IList<string> QueryWorkspaceFolders()
		{
			return WorkspaceHelper.QueryWorkspaceFolders(this.TestParameters, this.Logger);
		}

		protected int QueryWorkspaceObjectTypeDescriptorId(int artifactId)
		{
			return RdoHelper.QueryWorkspaceObjectTypeDescriptorId(this.TestParameters, artifactId);
		}

		protected Relativity.Services.Objects.DataContracts.RelativityObject ReadRelativityObject(
			int artifactId,
			IEnumerable<string> fields)
		{
			return RdoHelper.ReadRelativityObject(this.TestParameters, artifactId, fields);
		}

		protected virtual void OnSetup()
		{
		}

		protected virtual void OnTearDown()
		{
		}
	}
}