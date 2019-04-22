// -----------------------------------------------------------------------------------------------------
// <copyright file="QueryFieldFactory.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.TestFramework
{
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;

	using Relativity.Import.Export.Services;

	public class QueryFieldFactory
	{
		private DataTable table;

		public kCura.WinEDDS.ViewFieldInfo GenerateQueryField(
			int fieldArtifactID,
			int avfId,
			FieldCategory fieldCategory,
			FieldType fieldType,
			int artifactTypeId,
			string artifactTypeTableName,
			int fieldCodeTypeID,
			bool fieldIsArtifactBaseField,
			string displayName,
			string avfColumnName,
			string avfHeaderName,
			string formatStr,
			int associativeArtifactTypeID,
			bool isUnicodeEnabled,
			bool allowHtml,
			string allowFieldName,
			int sourceFieldArtifactID,
			string sourceFieldArtifactTypeTableName,
			string sourceFieldDisplayName,
			int sourceFieldArtifactTypeID,
			int connectorFieldArtifactID,
			string connectorFieldName,
			int connectorFieldCategoryId,
			int parentFileFieldArtifactID,
			string parentFileFieldDisplayName,
			bool isLinked,
			string columnSource,
			string dataSource,
			string relationalTableName,
			string relationalTableColumnName,
			string relationalTableColumnName2,
			ParentReflectionType parentReflectionType,
			string reflectedFieldArtifactTypeTableName,
			string reflectedFieldIdentifierColumnName,
			string reflectedFieldConnectorFieldName,
			string reflectedConnectorIdentifierColumnName,
			bool enableDataGrid)
		{
			var row = this.GenerateRow();
			row["FieldArtifactId"] = fieldArtifactID;
			row["AvfId"] = avfId;
			row["FieldCategoryID"] = (int)fieldCategory;
			row["FieldTypeID"] = (int)fieldType;
			row["ArtifactTypeID"] = artifactTypeId;
			row["ArtifactTypeTableName"] = artifactTypeTableName;
			row["FieldCodeTypeID"] = fieldCodeTypeID;
			row["FieldIsArtifactBaseField"] = fieldIsArtifactBaseField;
			row["DisplayName"] = displayName;
			row["AvfColumnName"] = avfColumnName;
			row["AvfHeaderName"] = avfHeaderName;
			row["FormatString"] = formatStr;
			row["AssociativeArtifactTypeID"] = associativeArtifactTypeID;
			row["IsUnicodeEnabled"] = isUnicodeEnabled;
			row["AllowHtml"] = allowHtml;
			row["AllowFieldName"] = allowFieldName;
			row["SourceFieldArtifactID"] = sourceFieldArtifactID;
			row["SourceFieldArtifactTypeTableName"] = sourceFieldArtifactTypeTableName;
			row["SourceFieldDisplayName"] = sourceFieldDisplayName;
			row["SourceFieldArtifactTypeID"] = sourceFieldArtifactTypeID;
			row["ConnectorFieldArtifactID"] = connectorFieldArtifactID;
			row["ConnectorFieldName"] = connectorFieldName;
			row["ConnectorFieldCategoryID"] = connectorFieldCategoryId;
			row["ParentFileFieldArtifactID"] = parentFileFieldArtifactID;
			row["ParentFileFieldDisplayName"] = parentFileFieldDisplayName;
			row["IsLinked"] = isLinked;
			row["ColumnSource"] = columnSource;
			row["DataSource"] = dataSource;
			row["RelationalTableName"] = relationalTableName;
			row["RelationalTableColumnName"] = relationalTableColumnName;
			row["RelationalTableColumnName2"] = relationalTableColumnName2;
			row["ParentReflectionType"] = (int)parentReflectionType;
			row["ReflectedFieldArtifactTypeTableName"] = reflectedFieldArtifactTypeTableName;
			row["ReflectedFieldArtifactTypeIdentifierColumnName"] = reflectedFieldIdentifierColumnName;
			row["ReflectedFieldArtifactTypeConnectorFieldName"] = reflectedFieldConnectorFieldName;
			row["ReflectedConnectorArtifactTypeIdentifierColumnName"] = reflectedConnectorIdentifierColumnName;
			row["EnableDataGrid"] = enableDataGrid;
			return new kCura.WinEDDS.ViewFieldInfo(row);
		}

		public kCura.WinEDDS.ViewFieldInfo GetIdentifierQueryField()
		{
			return this.GenerateQueryField(
				5,
				1000186,
				FieldCategory.Identifier,
				FieldType.Varchar,
				10,
				"Document",
				-1,
				false,
				"Identifier",
				"Identifier",
				"Identifier",
				string.Empty,
				-1,
				true,
				false,
				string.Empty,
				-1,
				string.Empty,
				"Identifier",
				-1,
				-1,
				string.Empty,
				-1,
				-1,
				string.Empty,
				true,
				"MainTable",
				"Document",
				string.Empty,
				string.Empty,
				string.Empty,
				ParentReflectionType.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				false);
		}

		public kCura.WinEDDS.ViewFieldInfo GetExtractedTextField()
		{
			return this.GenerateQueryField(
				6,
				1000187,
				FieldCategory.FullText,
				FieldType.Text,
				10,
				"Document",
				-1,
				false,
				"Extracted Text",
				"ExtractedText",
				"Extracted Text",
				string.Empty,
				-1,
				true,
				false,
				string.Empty,
				-1,
				string.Empty,
				"Extracted Text",
				-1,
				-1,
				string.Empty,
				-1,
				-1,
				string.Empty,
				false,
				"MainTable",
				"Document",
				string.Empty,
				string.Empty,
				string.Empty,
				ParentReflectionType.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				false);
		}

		public kCura.WinEDDS.ViewFieldInfo GetGenericLongTextField()
		{
			return this.GenerateQueryField(
				6,
				1001440,
				FieldCategory.FullText,
				FieldType.Text,
				10,
				"Document",
				-1,
				false,
				"Long Text",
				"LongText",
				"Long Text",
				string.Empty,
				-1,
				true,
				false,
				string.Empty,
				-1,
				string.Empty,
				"Long Text",
				-1,
				-1,
				string.Empty,
				-1,
				-1,
				string.Empty,
				false,
				"MainTable",
				"Document",
				string.Empty,
				string.Empty,
				string.Empty,
				ParentReflectionType.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				false);
		}

		public kCura.WinEDDS.ViewFieldInfo GetRenamedGenericLongTextField()
		{
			return this.GenerateQueryField(
				6,
				1001440,
				FieldCategory.FullText,
				FieldType.Text,
				10,
				"Document",
				-1,
				false,
				"OtherLong Text",
				"OtherLongText",
				"OtherLong Text",
				string.Empty,
				-1,
				true,
				false,
				string.Empty,
				-1,
				string.Empty,
				"OtherLong Text",
				-1,
				-1,
				string.Empty,
				-1,
				-1,
				string.Empty,
				false,
				"MainTable",
				"Document",
				string.Empty,
				string.Empty,
				string.Empty,
				ParentReflectionType.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				false);
		}

		public kCura.WinEDDS.ViewFieldInfo GetGenericBooleanField()
		{
			return this.GenerateQueryField(
				14497,
				1000357,
				FieldCategory.Generic,
				FieldType.Boolean,
				10,
				"Document",
				-1,
				false,
				"Bool",
				"Bool",
				"Bool",
				string.Empty,
				-1,
				false,
				false,
				string.Empty,
				-1,
				string.Empty,
				"Bool",
				-1,
				-1,
				string.Empty,
				-1,
				-1,
				string.Empty,
				false,
				"MainTable",
				"Document",
				string.Empty,
				string.Empty,
				string.Empty,
				ParentReflectionType.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				false);
		}

		public kCura.WinEDDS.ViewFieldInfo GetGenericSingleCodeField()
		{
			return this.GenerateQueryField(
				391085,
				1001118,
				FieldCategory.Generic,
				FieldType.Code,
				10,
				"Document",
				1000097,
				false,
				"Single Choice",
				"SingleChoice",
				"Single Choice",
				string.Empty,
				-1,
				true,
				false,
				string.Empty,
				-1,
				string.Empty,
				"Single Choice",
				-1,
				-1,
				string.Empty,
				-1,
				-1,
				string.Empty,
				false,
				"MainTable",
				"Document",
				string.Empty,
				string.Empty,
				string.Empty,
				ParentReflectionType.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				false);
		}

		public kCura.WinEDDS.ViewFieldInfo GetGenericMultiCodeField()
		{
			return this.GenerateQueryField(
				391081,
				1001117,
				FieldCategory.Generic,
				FieldType.MultiCode,
				10,
				"Document",
				1000096,
				false,
				"MCode",
				"MCode",
				"MCode",
				string.Empty,
				-1,
				false,
				false,
				string.Empty,
				-1,
				string.Empty,
				"MCode",
				-1,
				-1,
				string.Empty,
				-1,
				-1,
				string.Empty,
				false,
				"MainTable",
				"Document",
				string.Empty,
				string.Empty,
				string.Empty,
				ParentReflectionType.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				false);
		}

		public kCura.WinEDDS.ViewFieldInfo GetArtifactIdField()
		{
			return this.GenerateQueryField(
				13090,
				1000248,
				FieldCategory.Generic,
				FieldType.Integer,
				10,
				"Document",
				-1,
				true,
				"Artifact ID",
				"ArtifactID",
				"ArtifactID",
				string.Empty,
				-1,
				false,
				false,
				string.Empty,
				-1,
				string.Empty,
				"ArtifactID",
				-1,
				-1,
				string.Empty,
				-1,
				-1,
				string.Empty,
				false,
				"MainTable",
				"Document",
				string.Empty,
				string.Empty,
				string.Empty,
				ParentReflectionType.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				false);
		}

		public kCura.WinEDDS.ViewFieldInfo GetGenericNumericField(FieldType type)
		{
			return this.GenerateQueryField(
				13090,
				1000248,
				FieldCategory.Generic,
				type,
				10,
				"Document",
				-1,
				false,
				"Decimal",
				"Decimal",
				"Decimal",
				string.Empty,
				-1,
				false,
				false,
				string.Empty,
				-1,
				string.Empty,
				"Decimal",
				-1,
				-1,
				string.Empty,
				-1,
				-1,
				string.Empty,
				false,
				"MainTable",
				"Document",
				string.Empty,
				string.Empty,
				string.Empty,
				ParentReflectionType.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				false);
		}

		public kCura.WinEDDS.ViewFieldInfo GetGenericUserField()
		{
			return this.GenerateQueryField(
				14490,
				1000353,
				FieldCategory.Generic,
				FieldType.User,
				10,
				"Document",
				-1,
				false,
				"User Field",
				"UserField",
				"User Field",
				string.Empty,
				-1,
				false,
				false,
				string.Empty,
				-1,
				string.Empty,
				"User Field",
				-1,
				-1,
				string.Empty,
				-1,
				-1,
				string.Empty,
				false,
				"MainTable",
				"Document",
				string.Empty,
				string.Empty,
				string.Empty,
				ParentReflectionType.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				false);
		}

		public kCura.WinEDDS.ViewFieldInfo GetGenericObjectField()
		{
			return this.GenerateQueryField(
				504869,
				1001267,
				FieldCategory.Generic,
				FieldType.Object,
				10,
				"Document",
				-1,
				false,
				"WSP",
				"WSP",
				"WSP",
				string.Empty,
				1000047,
				false,
				false,
				string.Empty,
				-1,
				"WithSpace",
				"Name",
				-1,
				-1,
				string.Empty,
				-1,
				-1,
				string.Empty,
				false,
				"MainTable",
				"o1000047_f504869",
				string.Empty,
				string.Empty,
				string.Empty,
				ParentReflectionType.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				false);
		}

		public kCura.WinEDDS.ViewFieldInfo GetGenericObjectsField()
		{
			return this.GenerateQueryField(
				505158,
				1001278,
				FieldCategory.Generic,
				FieldType.Objects,
				10,
				"Document",
				-1,
				false,
				"DcMon",
				"DcMon",
				"DcMon",
				string.Empty,
				1000045,
				false,
				false,
				string.Empty,
				-1,
				"Cmon",
				"Name",
				-1,
				-1,
				"Name",
				-1,
				-1,
				string.Empty,
				false,
				"MainTable",
				"Document",
				"f505158f505168",
				"f505158ArtifactID",
				"f505168ArtifactID",
				ParentReflectionType.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				false);
		}

		public kCura.WinEDDS.ViewFieldInfo GenerateMultiReflectedUserField()
		{
			return this.GenerateQueryField(
				505162,
				1001282,
				FieldCategory.MultiReflected,
				FieldType.User,
				10,
				"Document",
				-1,
				false,
				"DcMon::User",
				"DcMonUser",
				"DcMon::User",
				string.Empty,
				1000045,
				false,
				false,
				string.Empty,
				504867,
				"Cmon",
				"User",
				1000045,
				505158,
				"Name",
				0,
				-1,
				string.Empty,
				false,
				"MainTable",
				"Cmon",
				"f505158f505168",
				"f505158ArtifactID",
				"f505168ArtifactID",
				ParentReflectionType.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				false);
		}

		public kCura.WinEDDS.ViewFieldInfo GetGenericDateField()
		{
			return this.GenerateQueryField(
				504869,
				1001267,
				FieldCategory.Generic,
				FieldType.Date,
				10,
				"Document",
				-1,
				false,
				"Date",
				"Date",
				"Date",
				"d",
				-1,
				false,
				false,
				string.Empty,
				-1,
				string.Empty,
				"Date",
				-1,
				-1,
				string.Empty,
				-1,
				-1,
				string.Empty,
				false,
				"MainTable",
				"Document",
				string.Empty,
				string.Empty,
				string.Empty,
				ParentReflectionType.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				false);
		}

		public kCura.WinEDDS.ViewFieldInfo GetGenericDateTimeField()
		{
			return this.GenerateQueryField(
				504869,
				1001267,
				FieldCategory.Generic,
				FieldType.Date,
				10,
				"Document",
				-1,
				false,
				"Date",
				"Date",
				"Date",
				"g",
				-1,
				false,
				false,
				string.Empty,
				-1,
				string.Empty,
				"Date",
				-1,
				-1,
				string.Empty,
				-1,
				-1,
				string.Empty,
				false,
				"MainTable",
				"Document",
				string.Empty,
				string.Empty,
				string.Empty,
				ParentReflectionType.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				false);
		}

		public kCura.WinEDDS.ViewFieldInfo GetDocumentFolderNameField()
		{
			return this.GenerateQueryField(
				15,
				1000196,
				FieldCategory.FolderName,
				FieldType.Varchar,
				10,
				"Document",
				-1,
				false,
				"Folder Name",
				"FolderName",
				"Folder Name",
				string.Empty,
				-1,
				false,
				false,
				string.Empty,
				-1,
				string.Empty,
				"Name",
				-1,
				-1,
				string.Empty,
				-1,
				-1,
				string.Empty,
				false,
				"Artifact",
				"Folder",
				string.Empty,
				string.Empty,
				string.Empty,
				ParentReflectionType.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				false);
		}

		public IEnumerable<kCura.WinEDDS.ViewFieldInfo> GetAllDocumentFields()
		{
			return new List<kCura.WinEDDS.ViewFieldInfo>
			{
				this.GetIdentifierQueryField(),
				this.GetExtractedTextField(),
				this.GetGenericBooleanField(),
				this.GetGenericSingleCodeField(),
				this.GetGenericMultiCodeField(),
				this.GetArtifactIdField(),
				this.GetGenericNumericField(FieldType.Currency),
				this.GetGenericNumericField(FieldType.Decimal),
				this.GetGenericNumericField(FieldType.Integer),
				this.GetGenericUserField(),
				this.GetGenericObjectField(),
				this.GetGenericObjectsField(),
				this.GenerateMultiReflectedUserField(),
				this.GetGenericDateField(),
				this.GetGenericDateTimeField(),
				this.GetDocumentFolderNameField(),
			};
		}

		private DataRow GenerateRow()
		{
			if (this.table == null)
			{
				this.table = new DataTable("Test-Data") { Locale = CultureInfo.InvariantCulture };
				this.table.Columns.Add("FieldArtifactID", typeof(int));
				this.table.Columns.Add("AvfId", typeof(int));
				this.table.Columns.Add("FieldCategoryID", typeof(int));
				this.table.Columns.Add("FieldTypeID", typeof(int));
				this.table.Columns.Add("ArtifactTypeID", typeof(int));
				this.table.Columns.Add("ArtifactTypeTableName", typeof(string));
				this.table.Columns.Add("FieldCodeTypeID", typeof(int));
				this.table.Columns.Add("FieldIsArtifactBaseField", typeof(bool));
				this.table.Columns.Add("DisplayName", typeof(string));
				this.table.Columns.Add("AvfColumnName", typeof(string));
				this.table.Columns.Add("AvfHeaderName", typeof(string));
				this.table.Columns.Add("FormatString", typeof(string));
				this.table.Columns.Add("AssociativeArtifactTypeID", typeof(int));
				this.table.Columns.Add("IsUnicodeEnabled", typeof(bool));
				this.table.Columns.Add("AllowHtml", typeof(bool));
				this.table.Columns.Add("AllowFieldName", typeof(string));
				this.table.Columns.Add("SourceFieldArtifactID", typeof(int));
				this.table.Columns.Add("SourceFieldArtifactTypeTableName", typeof(string));
				this.table.Columns.Add("SourceFieldDisplayName", typeof(string));
				this.table.Columns.Add("SourceFieldArtifactTypeID", typeof(int));
				this.table.Columns.Add("ConnectorFieldArtifactID", typeof(int));
				this.table.Columns.Add("ConnectorFieldName", typeof(string));
				this.table.Columns.Add("ConnectorFieldCategoryID", typeof(int));
				this.table.Columns.Add("ParentFileFieldArtifactID", typeof(int));
				this.table.Columns.Add("ParentFileFieldDisplayName", typeof(string));
				this.table.Columns.Add("IsLinked", typeof(bool));
				this.table.Columns.Add("ColumnSource", typeof(string));
				this.table.Columns.Add("DataSource", typeof(string));
				this.table.Columns.Add("RelationalTableName", typeof(string));
				this.table.Columns.Add("RelationalTableColumnName", typeof(string));
				this.table.Columns.Add("RelationalTableColumnName2", typeof(string));
				this.table.Columns.Add("ParentReflectionType", typeof(int));
				this.table.Columns.Add("ReflectedFieldArtifactTypeTableName", typeof(string));
				this.table.Columns.Add("ReflectedFieldArtifactTypeIdentifierColumnName", typeof(string));
				this.table.Columns.Add("ReflectedFieldArtifactTypeConnectorFieldName", typeof(string));
				this.table.Columns.Add("ReflectedConnectorArtifactTypeIdentifierColumnName", typeof(string));
				this.table.Columns.Add("EnableDataGrid", typeof(bool));
			}

			return this.table.NewRow();
		}
	}
}