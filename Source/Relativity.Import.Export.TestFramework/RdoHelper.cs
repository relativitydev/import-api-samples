// ----------------------------------------------------------------------------
// <copyright file="RdoHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.TestFramework
{
	using System;
	using System.Collections.Generic;
    using System.Linq;

	using kCura.Relativity.Client;
	using kCura.Relativity.Client.DTOs;

	using Relativity.Services.Objects;
	using Relativity.Services.Objects.DataContracts;

	/// <summary>
	/// Defines static helper methods to manage Relativity Dynamic Objects.
	/// </summary>
	public static class RdoHelper
	{
        public static int CreateObjectType(IntegrationTestParameters parameters, string objectTypeName)
        {
	        if (parameters == null)
	        {
		        throw new ArgumentNullException(nameof(parameters));
	        }

			using (IRSAPIClient client = ServiceHelper.GetServiceProxy<IRSAPIClient>(parameters))
            {
                client.APIOptions.WorkspaceID = parameters.WorkspaceId;
                Result<kCura.Relativity.Client.DTOs.ObjectType> objectType = client.Repositories.ObjectType.Query(
	                new Query<kCura.Relativity.Client.DTOs.ObjectType>
		                {
			                Condition = new TextCondition("Name", TextConditionEnum.EqualTo, objectTypeName),
			                Fields = FieldValue.AllFields
		                }).Results.FirstOrDefault();
                if (objectType != null)
                {
                    return objectType.Artifact.ArtifactID;
                }

                kCura.Relativity.Client.DTOs.ObjectType objectTypeDto = new kCura.Relativity.Client.DTOs.ObjectType
	                                                                        {
		                                                                        Name = objectTypeName,
		                                                                        ParentArtifactTypeID = 8,
		                                                                        SnapshotAuditingEnabledOnDelete = true,
		                                                                        Pivot = true,
		                                                                        CopyInstancesOnWorkspaceCreation =
			                                                                        false,
		                                                                        Sampling = true,
		                                                                        PersistentLists = false,
		                                                                        CopyInstancesOnParentCopy = false
	                                                                        };
                int artifactId = client.Repositories.ObjectType.CreateSingle(objectTypeDto);
                return artifactId;
            }
        }

        public static int CreateObjectTypeInstance(
	        IntegrationTestParameters parameters,
            int artifactTypeId,
            IDictionary<string, object> fields)
        {
	        if (parameters == null)
	        {
		        throw new ArgumentNullException(nameof(parameters));
	        }

			using (IObjectManager objectManager = ServiceHelper.GetServiceProxy<IObjectManager>(parameters))
			{
				CreateRequest request = new CreateRequest
					                        {
						                        ObjectType = new ObjectTypeRef { ArtifactTypeID = artifactTypeId },
						                        FieldValues = fields.Keys.Select(
							                        key => new FieldRefValuePair
								                               {
									                               Field = new FieldRef { Name = key },
									                               Value = fields[key]
								                               })
					                        };
                Services.Objects.DataContracts.CreateResult result =
	                objectManager.CreateAsync(parameters.WorkspaceId, request).GetAwaiter().GetResult();
                List<InvalidOperationException> innerExceptions = result.EventHandlerStatuses.Where(x => !x.Success)
	                .Select(status => new InvalidOperationException(status.Message)).ToList();
                if (innerExceptions.Count == 0)
                {
                    return result.Object.ArtifactID;
                }

                throw new AggregateException(
                    $"Failed to create a new instance for {artifactTypeId} artifact type.", innerExceptions);
            }
        }

        public static void DeleteObject(IntegrationTestParameters parameters, int artifactId)
        {
	        if (parameters == null)
	        {
		        throw new ArgumentNullException(nameof(parameters));
	        }

			using (IObjectManager objectManager = ServiceHelper.GetServiceProxy<IObjectManager>(parameters))
            {
                DeleteRequest request = new DeleteRequest
                {
                    Object = new RelativityObjectRef { ArtifactID = artifactId }
                };

                Services.Objects.DataContracts.DeleteResult result =
	                objectManager.DeleteAsync(parameters.WorkspaceId, request).GetAwaiter().GetResult();
                if (result.Report.DeletedItems.Count == 0)
                {
                    throw new InvalidOperationException($"Failed to delete the {artifactId} object.");
                }
            }
        }

        public static int QueryArtifactTypeId(IntegrationTestParameters parameters, string objectTypeName)
        {
	        if (parameters == null)
	        {
		        throw new ArgumentNullException(nameof(parameters));
	        }

	        using (IObjectManager objectManager = ServiceHelper.GetServiceProxy<IObjectManager>(parameters))
	        {
		        QueryRequest queryRequest = new QueryRequest
			                                    {
				                                    ObjectType = new ObjectTypeRef { Name = "Object Type" },
				                                    Fields = new[] { new FieldRef { Name = "Artifact Type ID" } },
				                                    Condition = $"'Name' == '{objectTypeName}'"
			                                    };

		        Services.Objects.DataContracts.QueryResult result = objectManager
			        .QueryAsync(parameters.WorkspaceId, queryRequest, 0, 1).GetAwaiter().GetResult();
		        if (result.TotalCount != 1)
		        {
			        return 0;
		        }

		        return (int)result.Objects.Single().FieldValues.Single().Value;
	        }
        }

        public static int QueryRelativityObjectCount(IntegrationTestParameters parameters, int artifactTypeId)
        {
	        if (parameters == null)
	        {
		        throw new ArgumentNullException(nameof(parameters));
	        }

			using (IObjectManager client = ServiceHelper.GetServiceProxy<IObjectManager>(parameters))
			{
				QueryRequest queryRequest = new QueryRequest
					                            {
						                            ObjectType = new ObjectTypeRef { ArtifactTypeID = artifactTypeId }
					                            };
                Services.Objects.DataContracts.QueryResult result = client.QueryAsync(
	                parameters.WorkspaceId,
	                queryRequest,
	                1,
	                ServiceHelper.MaxItemsToFetch).GetAwaiter().GetResult();
                return result.TotalCount;
            }
        }

        public static int QueryObjectType(IntegrationTestParameters parameters, string objectTypeName)
        {
	        if (parameters == null)
	        {
		        throw new ArgumentNullException(nameof(parameters));
	        }

	        using (IRSAPIClient client = ServiceHelper.GetServiceProxy<IRSAPIClient>(parameters))
	        {
		        client.APIOptions.WorkspaceID = parameters.WorkspaceId;
		        Result<kCura.Relativity.Client.DTOs.ObjectType> objectType = client.Repositories.ObjectType.Query(
			        new Query<kCura.Relativity.Client.DTOs.ObjectType>
				        {
					        Condition = new TextCondition("Name", TextConditionEnum.EqualTo, objectTypeName),
					        Fields = FieldValue.AllFields
				        }).Results.FirstOrDefault();
		        return objectType?.Artifact?.ArtifactTypeID ?? 0;
	        }
        }

        public static IList<RelativityObject> QueryRelativityObjects(
	        IntegrationTestParameters parameters,
            int artifactTypeId,
            IEnumerable<string> fields)
        {
	        if (parameters == null)
	        {
		        throw new ArgumentNullException(nameof(parameters));
	        }

			using (IObjectManager client = ServiceHelper.GetServiceProxy<IObjectManager>(parameters))
			{
				QueryRequest queryRequest = new QueryRequest
					                            {
						                            Fields = fields?.Select(x => new FieldRef { Name = x }),
						                            ObjectType = new ObjectTypeRef { ArtifactTypeID = artifactTypeId }
					                            };
                Services.Objects.DataContracts.QueryResult result = client.QueryAsync(
	                parameters.WorkspaceId,
	                queryRequest,
	                1,
	                ServiceHelper.MaxItemsToFetch).GetAwaiter().GetResult();
                return result.Objects;
            }
        }

        public static int QueryWorkspaceObjectTypeDescriptorId(IntegrationTestParameters parameters, int artifactId)
        {
	        if (parameters == null)
	        {
		        throw new ArgumentNullException(nameof(parameters));
	        }

	        kCura.Relativity.Client.DTOs.ObjectType objectType =
		        new kCura.Relativity.Client.DTOs.ObjectType(artifactId) { Fields = FieldValue.AllFields };
            ResultSet<kCura.Relativity.Client.DTOs.ObjectType> resultSet;
            using (IRSAPIClient client = ServiceHelper.GetServiceProxy<IRSAPIClient>(parameters))
            {
	            client.APIOptions.WorkspaceID = parameters.WorkspaceId;
	            resultSet = client.Repositories.ObjectType.Read(objectType);
            }

            int? descriptorArtifactTypeId = null;
            if (resultSet.Success && resultSet.Results.Any())
            {
                descriptorArtifactTypeId = resultSet.Results.First().Artifact.DescriptorArtifactTypeID;
            }

            if (!descriptorArtifactTypeId.HasValue)
            {
                throw new InvalidOperationException(
                    "Failed to retrieve Object Type descriptor artifact type identifier.");
            }

            return descriptorArtifactTypeId.Value;
        }

        public static RelativityObject ReadRelativityObject(
	        IntegrationTestParameters parameters,
            int artifactId,
            IEnumerable<string> fields)
        {
	        if (parameters == null)
	        {
		        throw new ArgumentNullException(nameof(parameters));
	        }

			using (IObjectManager client = ServiceHelper.GetServiceProxy<IObjectManager>(parameters))
			{
				ReadRequest readRequest = new ReadRequest
					                          {
						                          Fields = fields.Select(x => new FieldRef { Name = x }),
						                          Object = new RelativityObjectRef { ArtifactID = artifactId }
					                          };

				Services.Objects.DataContracts.ReadResult result = client.ReadAsync(parameters.WorkspaceId, readRequest)
					.GetAwaiter().GetResult();
                return result.Object;
			}
        }
    }
}