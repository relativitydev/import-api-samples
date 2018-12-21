// ----------------------------------------------------------------------------
// <copyright file="ImportTestsBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

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
		protected DataTable DataTable
		{
			get;
			private set;
		}

		protected Exception FatalException
		{
			get;
			set;
		}

		protected JobReport JobReport
		{
			get;
			set;
		}

		[SetUp]
		public void Setup()
		{
			Assert.That(TestSettings.WorkspaceId, Is.GreaterThan(0));
			this.DataTable = new DataTable();
			this.FatalException = null;
			this.JobReport = null;
			this.OnSetup();
		}

		[TearDown]
		public void Teardown()
		{
			DataTable?.Dispose();
		}

		protected static DateTime GetDateFieldValue(Relativity.Services.Objects.DataContracts.RelativityObject relativityObject, string name)
		{
			return (DateTime)GetFieldValue(relativityObject, name);
		}

		protected static decimal GetDecimalFieldValue(Relativity.Services.Objects.DataContracts.RelativityObject relativityObject, string name)
		{
			return (decimal)GetFieldValue(relativityObject, name);
		}

		protected static object GetFieldValue(Relativity.Services.Objects.DataContracts.RelativityObject relativityObject, string name)
		{
			Relativity.Services.Objects.DataContracts.FieldValuePair pair = relativityObject.FieldValues.FirstOrDefault(x => x.Field.Name == name);
			return pair?.Value;
		}

		protected static string GetStringFieldValue(Relativity.Services.Objects.DataContracts.RelativityObject relativityObject, string name)
		{
			return GetFieldValue(relativityObject, name) as string;
		}

		protected static Relativity.Services.Objects.DataContracts.RelativityObject GetRelativityObject(IList<Relativity.Services.Objects.DataContracts.RelativityObject> objects, string identityFieldName, string identityFieldValue)
		{
			return (from obj in objects
				from pair in obj.FieldValues
				where pair.Field.Name == identityFieldName && pair.Value.ToString() == identityFieldValue
				select obj).FirstOrDefault();
		}

		protected int CreateObjectType(string objectTypeName)
		{
			return TestHelper.CreateObjectType(
				TestSettings.RelativityWebApiUrl,
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.WorkspaceId,
				objectTypeName);
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

		protected kCura.Relativity.ImportAPI.ImportAPI CreateImportApiObject()
		{
			return new kCura.Relativity.ImportAPI.ImportAPI(
				TestSettings.RelativityUserName,
				TestSettings.RelativityPassword,
				TestSettings.RelativityWebApiUrl);
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