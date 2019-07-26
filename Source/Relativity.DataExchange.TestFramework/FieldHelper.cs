// ----------------------------------------------------------------------------
// <copyright file="FieldHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using kCura.Relativity.Client;
	using kCura.Relativity.Client.DTOs;

	using Relativity.Services.Objects;
	using Relativity.Services.Objects.DataContracts;

	/// <summary>
	/// Defines static helper methods to manage fields.
	/// </summary>
	public static class FieldHelper
	{
		public static void CreateField(
			IntegrationTestParameters parameters,
			int workspaceObjectTypeId,
			kCura.Relativity.Client.DTOs.Field field)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			if (field == null)
			{
				throw new ArgumentNullException(nameof(field));
			}

			using (IRSAPIClient client = ServiceHelper.GetServiceProxy<IRSAPIClient>(parameters))
			{
				client.APIOptions.WorkspaceID = parameters.WorkspaceId;
				List<kCura.Relativity.Client.DTOs.Field>
					fieldsToCreate = new List<kCura.Relativity.Client.DTOs.Field>();
				field.ObjectType = new kCura.Relativity.Client.DTOs.ObjectType
				{
					DescriptorArtifactTypeID = workspaceObjectTypeId,
				};
				kCura.Relativity.Client.DTOs.WriteResultSet<kCura.Relativity.Client.DTOs.Field> resultSet =
					client.Repositories.Field.Create(fieldsToCreate);
				resultSet = client.Repositories.Field.Create(field);
				if (resultSet.Success)
				{
					return;
				}

				List<Exception> innerExceptions = new List<Exception>();
				foreach (Result<kCura.Relativity.Client.DTOs.Field> result in resultSet.Results.Where(x => !x.Success))
				{
					innerExceptions.Add(new InvalidOperationException(result.Message));
				}

				throw new AggregateException(
					$"Failed to create the {field.Name} field. Error: {resultSet.Message}", innerExceptions);
			}
		}

		public static int QueryIdentifierFieldId(IntegrationTestParameters parameters, string artifactTypeName)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			RelativityObject ro = QueryIdentifierRelativityObject(parameters, artifactTypeName);
			return ro.ArtifactID;
		}

		public static string QueryIdentifierFieldName(IntegrationTestParameters parameters, string artifactTypeName)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			RelativityObject ro = QueryIdentifierRelativityObject(parameters, artifactTypeName);
			return ro.FieldValues[0].Value as string;
		}

		private static RelativityObject QueryIdentifierRelativityObject(IntegrationTestParameters parameters, string artifactTypeName)
		{
			using (IObjectManager client = ServiceHelper.GetServiceProxy<IObjectManager>(parameters))
			{
				QueryRequest queryRequest = new QueryRequest
					                            {
						                            Condition =
							                            $"'{ArtifactTypeNames.ObjectType}' == '{artifactTypeName}' AND '{FieldFieldNames.IsIdentifier}' == true",
						                            Fields = new List<FieldRef>
							                                     {
								                                     new FieldRef { Name = FieldFieldNames.Name },
							                                     },
						                            ObjectType = new ObjectTypeRef { ArtifactTypeID = (int)ArtifactType.Field },
					                            };
				Relativity.Services.Objects.DataContracts.QueryResult result = client.QueryAsync(
					parameters.WorkspaceId,
					queryRequest,
					1,
					ServiceHelper.MaxItemsToFetch).GetAwaiter().GetResult();
				if (result.TotalCount != 1)
				{
					throw new InvalidOperationException(
						$"Failed to retrieve the identifier Relativity object for the '{artifactTypeName}' artifact type.");
				}

				return result.Objects[0];
			}
		}
	}
}