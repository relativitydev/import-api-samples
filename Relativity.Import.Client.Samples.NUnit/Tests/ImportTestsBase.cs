// ----------------------------------------------------------------------------
// <copyright file="ImportTestsBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

using System.Collections;
using kCura.Relativity.DataReaderClient;

namespace Relativity.Import.Client.Sample.NUnit.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;

	using global::NUnit.Framework;

	/// <summary>
	/// Represents an abstract base class to provide common functionality and helper methods.
	/// </summary>
	public abstract class ImportTestsBase
	{
		protected ImportTestsBase()
			: this(AssemblySetup.Logger)
		{
			// Assume that AssemblySetup has already setup the singleton.
		}

		protected ImportTestsBase(Relativity.Logging.ILog log)
		{
			this.Logger = log ?? throw new ArgumentNullException(nameof(log));
			Assert.That(this.Logger, Is.Not.Null);
		}

		protected Relativity.Logging.ILog Logger
		{
			get;
		}

		protected DataTable DataTable
		{
			get;
			private set;
		}

		protected DateTime ImportStartTime
		{
			get;
			private set;
		}

		public IList<IDictionary> ErrorEvents
		{
			get;
			set;
		}

		protected Exception FatalExceptionEvent
		{
			get;
			set;
		}

		protected JobReport JobCompletedReport
		{
			get;
			set;
		}

		protected IList<string> MessageEvents
		{
			get;
			set;
		}

		protected IList<long> ProgressRowEvents
		{
			get;
			set;
		}

		protected IList<FullStatus> ProcessProgressEvents
		{
			get;
			set;
		}

		[SetUp]
		public void Setup()
		{
			Assert.That(TestSettings.WorkspaceId, Is.GreaterThan(0));
			this.DataTable = new DataTable();
			this.ErrorEvents = new List<IDictionary>();
			this.FatalExceptionEvent = null;
			this.ImportStartTime = DateTime.Now;
			this.JobCompletedReport = null;
			this.MessageEvents = new List<string>();
			this.ProgressRowEvents = new List<long>();
			this.ProcessProgressEvents = new List<FullStatus>();
			this.OnSetup();
		}

		[TearDown]
		public void Teardown()
		{
			DataTable?.Dispose();
		}

		protected static DateTime FindDateFieldValue(Relativity.Services.Objects.DataContracts.RelativityObject relativityObject, string name)
		{
			return (DateTime)FindFieldValue(relativityObject, name);
		}

		protected static decimal FindDecimalFieldValue(Relativity.Services.Objects.DataContracts.RelativityObject relativityObject, string name)
		{
			return (decimal)FindFieldValue(relativityObject, name);
		}

		protected static object FindFieldValue(Relativity.Services.Objects.DataContracts.RelativityObject relativityObject, string name)
		{
			Relativity.Services.Objects.DataContracts.FieldValuePair pair = relativityObject.FieldValues.FirstOrDefault(x => x.Field.Name == name);
			return pair?.Value;
		}

		protected static Relativity.Services.Objects.DataContracts.RelativityObject FindRelativityObject(IList<Relativity.Services.Objects.DataContracts.RelativityObject> objects, string identityFieldName, string identityFieldValue)
		{
			return (from obj in objects
				from pair in obj.FieldValues
				where pair.Field.Name == identityFieldName && pair.Value.ToString() == identityFieldValue
				select obj).FirstOrDefault();
		}

		protected static string FindStringFieldValue(Relativity.Services.Objects.DataContracts.RelativityObject relativityObject, string name)
		{
			return FindFieldValue(relativityObject, name) as string;
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
				TestSettings.RelativityWebApiUrl,
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
				TestSettings.RelativityWebApiUrl,
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
				TestSettings.RelativityWebApiUrl,
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
				TestSettings.RelativityWebApiUrl,
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
				TestSettings.RelativityWebApiUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.WorkspaceId,
				workspaceObjectTypeId,
				field);
		}

		protected kCura.Relativity.ImportAPI.ImportAPI CreateImportApiObject()
		{
			return new kCura.Relativity.ImportAPI.ImportAPI(
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.RelativityWebApiUrl);
		}

		protected int CreateObjectType(string objectTypeName)
		{
			int artifactTypeId = TestHelper.CreateObjectType(
				TestSettings.RelativityWebApiUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.WorkspaceId,
				objectTypeName);
			this.Logger.LogInformation(
				"Successfully created object type '{ObjectTypeName}' - {ArtifactTypeId}.",
				objectTypeName, artifactTypeId);
			return artifactTypeId;
		}

		protected int CreateObjectTypeInstance(int artifactTypeId, IDictionary<string, object> fields)
		{
			int artifactId = TestHelper.CreateObjectTypeInstance(
				TestSettings.RelativityWebApiUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.WorkspaceId,
				artifactTypeId,
				fields);
			this.Logger.LogInformation("Successfully created instance {ArtifactId} of object type {ArtifactTypeId}.",
				artifactId, artifactTypeId);
			return artifactId;
		}

		protected int QueryArtifactTypeId(string objectTypeName)
		{
			return TestHelper.QueryArtifactTypeId(
				TestSettings.RelativityWebApiUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.WorkspaceId,
				objectTypeName);
		}

		protected int QueryIdentifierFieldId(string artifactTypeName)
		{
			return TestHelper.QueryIdentifierFieldId(
				TestSettings.RelativityWebApiUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.WorkspaceId,
				artifactTypeName);
		}

		protected int QueryRelativityObjectCount(int artifactTypeId)
		{
			return TestHelper.QueryRelativityObjectCount(
				TestSettings.RelativityWebApiUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword, TestSettings.WorkspaceId,
				artifactTypeId);
		}

		protected IList<Relativity.Services.Objects.DataContracts.RelativityObject> QueryRelativityObjects(int artifactTypeId, IEnumerable<string> fields)
		{
			return TestHelper.QueryRelativityObjects(
				TestSettings.RelativityWebApiUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.WorkspaceId,
				artifactTypeId,
				fields);
		}

		protected int QueryWorkspaceObjectTypeDescriptorId(int workspaceObjectTypeId)
		{
			return TestHelper.QueryWorkspaceObjectTypeDescriptorId(
				TestSettings.RelativityWebApiUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.WorkspaceId,
				workspaceObjectTypeId);
		}

		protected virtual void OnSetup()
		{
		}
	}
}