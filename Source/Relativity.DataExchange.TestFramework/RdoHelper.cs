// ----------------------------------------------------------------------------
// <copyright file="RdoHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Relativity.Services.Interfaces.ObjectType;
	using Relativity.Services.Interfaces.ObjectType.Models;
	using Relativity.Services.Interfaces.Shared;
	using Relativity.Services.Interfaces.Shared.Models;

	using kCura.Relativity.Client;
	using kCura.Relativity.Client.DTOs;

	using Relativity.Services.Objects;
	using Relativity.Services.Objects.DataContracts;

	/// <summary>
	/// Defines static helper methods to manage Relativity Dynamic Objects.
	/// </summary>
	public static class RdoHelper
	{
		private const int WorkspaceArtifactTypeId = 8;

		public static int CreateObjectType(IntegrationTestParameters parameters, string objectTypeName)
		{
			using (var objectManager = ServiceHelper.GetServiceProxy<IObjectManager>(parameters))
			{
				QueryRequest queryObjectTypeRequest = new QueryRequest
				{
					ObjectType = new ObjectTypeRef { Name = "Object Type" },
					Fields = new[] { new FieldRef { Name = "Artifact Type ID" } },
					Condition = $"'Name' == '{objectTypeName}'",
				};
				Services.Objects.DataContracts.QueryResult result = objectManager.QueryAsync(parameters.WorkspaceId, queryObjectTypeRequest, 0, 1).GetAwaiter().GetResult();

				if (result.TotalCount > 0)
				{
					return (int)result.Objects.Single().FieldValues.Single().Value;
				}
			}

			using (var objectTypeManager = ServiceHelper.GetServiceProxy<IObjectTypeManager>(parameters))
			{
				var request = new ObjectTypeRequest
				{
					Name = objectTypeName,
					ParentObjectType = new Securable<ObjectTypeIdentifier>(new ObjectTypeIdentifier { ArtifactTypeID = WorkspaceArtifactTypeId }),
					EnableSnapshotAuditingOnDelete = true,
					PivotEnabled = true,
					CopyInstancesOnCaseCreation = false,
					SamplingEnabled = true,
					PersistentListsEnabled = false,
					CopyInstancesOnParentCopy = false,
				};

				int artifactId = objectTypeManager.CreateAsync(parameters.WorkspaceId, request).GetAwaiter().GetResult();
				ObjectTypeResponse objectTypeResponse = objectTypeManager.ReadAsync(parameters.WorkspaceId, artifactId).GetAwaiter().GetResult();
				return objectTypeResponse.ArtifactTypeID;
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
														Value = fields[key],
													}),
				};
				Relativity.Services.Objects.DataContracts.CreateResult result =
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
					Object = new RelativityObjectRef { ArtifactID = artifactId },
				};

				Relativity.Services.Objects.DataContracts.DeleteResult result =
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
					Condition = $"'Name' == '{objectTypeName}'",
				};

				Relativity.Services.Objects.DataContracts.QueryResult result = objectManager
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
					ObjectType = new ObjectTypeRef { ArtifactTypeID = artifactTypeId },
				};
				Relativity.Services.Objects.DataContracts.QueryResult result = client.QueryAsync(
					parameters.WorkspaceId,
					queryRequest,
					1,
					ServiceHelper.MaxItemsToFetch).GetAwaiter().GetResult();
				return result.TotalCount;
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
					ObjectType = new ObjectTypeRef { ArtifactTypeID = artifactTypeId },
				};
				Relativity.Services.Objects.DataContracts.QueryResult result = client.QueryAsync(
					parameters.WorkspaceId,
					queryRequest,
					1,
					ServiceHelper.MaxItemsToFetch).GetAwaiter().GetResult();
				return result.Objects;
			}
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
					Object = new RelativityObjectRef { ArtifactID = artifactId },
				};

				Relativity.Services.Objects.DataContracts.ReadResult result = client.ReadAsync(parameters.WorkspaceId, readRequest)
					.GetAwaiter().GetResult();
				return result.Object;
			}
		}
	}
}