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
			this.Logger = log ?? throw new ArgumentNullException(nameof(log));
			Assert.That(this.Logger, Is.Not.Null);
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
			this.DataSource = new DataTable();
			this.PublishedErrors = new List<IDictionary>();
			this.PublishedFatalException = null;
			this.StartTime = DateTime.Now;
			this.PublishedJobReport = null;
			this.PublishedMessages = new List<string>();
			this.PublishedProgressRows = new List<long>();
			this.PublishedProcessProgress = new List<kCura.Relativity.DataReaderClient.FullStatus>();
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
		/// Generates a unique bates number with a BATES prefix.
		/// </summary>
		/// <returns>
		/// The bates number.
		/// </returns>
		protected static string GenerateBatesNumber()
		{
			return $"BATES-{Guid.NewGuid()}";
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