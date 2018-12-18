// ----------------------------------------------------------------------------
// <copyright file="TestHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.Sample.NUnit
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Threading.Tasks;

	using kCura.Relativity.Client;
	using kCura.Relativity.Client.DTOs;

	using Relativity.Services.Objects;
	using Relativity.Services.Objects.DataContracts;
	using Relativity.Services.ServiceProxy;
	
	public static class TestHelper
	{
		public static int GetObjectCount(string webApiUrl, string userName, string password, int workspaceId, int artifactTypeId)
		{
			using (var client = GetProxy<IObjectManager>(webApiUrl, userName, password))
			{
				var queryRequest = new QueryRequest
				{
					ObjectType = new ObjectTypeRef { ArtifactTypeID = artifactTypeId }
				};

				const int maxItemsToFetch = 10;
				var result = client.QueryAsync(workspaceId, queryRequest, 1, maxItemsToFetch).GetAwaiter().GetResult();
				return result.TotalCount;
			}
		}

		public static kCura.Relativity.Client.DTOs.ObjectType CreateObjectType(string webApiUrl, string userName, string password, int workspaceId, string objectTypeName)
		{
			using (IRSAPIClient client = GetProxy<IRSAPIClient>(webApiUrl, userName, password))
			{
				client.APIOptions.WorkspaceID = workspaceId;
				kCura.Relativity.Client.DTOs.ObjectType objectTypeDto = new kCura.Relativity.Client.DTOs.ObjectType
				{
					Name = objectTypeName,
					ParentArtifactTypeID = 8,
					SnapshotAuditingEnabledOnDelete = true,
					Pivot = true,
					CopyInstancesOnWorkspaceCreation = false,
					Sampling = true,
					PersistentLists = false,
					CopyInstancesOnParentCopy = false
				};

				int artifactTypeID = client.Repositories.ObjectType.CreateSingle(objectTypeDto);
				objectTypeDto.DescriptorArtifactTypeID = artifactTypeID;
				return objectTypeDto;
			}
		}

		public static void CreateFixedLengthTextField(string webApiUrl, string userName, string password, int workspaceId, int descriptorArtifactTypeId, string fieldName, int length)
		{
			using (IRSAPIClient client = GetProxy<IRSAPIClient>(webApiUrl, userName, password))
			{
				client.APIOptions.WorkspaceID = workspaceId;
				List<kCura.Relativity.Client.DTOs.Field> fieldsToCreate = new List<kCura.Relativity.Client.DTOs.Field>();
				kCura.Relativity.Client.DTOs.Field field = new kCura.Relativity.Client.DTOs.Field();
				field.Name = fieldName;
				field.ObjectType = new kCura.Relativity.Client.DTOs.ObjectType
				{
					DescriptorArtifactTypeID = descriptorArtifactTypeId
				};

				field.FieldTypeID = kCura.Relativity.Client.FieldType.FixedLengthText;
				field.IsRequired = false;
				field.Unicode = false;
				field.AvailableInFieldTree = false;
				field.OpenToAssociations = false;
				field.Linked = false;
				field.AllowSortTally = true;
				field.Wrapping = false;
				field.AllowGroupBy = false;
				field.AllowPivot = false;
				field.IgnoreWarnings = true;
				field.Length = length;
				field.Width = "";
				kCura.Relativity.Client.DTOs.WriteResultSet<kCura.Relativity.Client.DTOs.Field> resultSet =
					client.Repositories.Field.Create(fieldsToCreate);
				resultSet = client.Repositories.Field.Create(field);
				if (!resultSet.Success)
				{
					throw new InvalidOperationException($"Failed to create the {fieldName} field. Error: {resultSet.Message}");
				}
			}
		}

		public static int CreateTestWorkspace(string webApiUrl, string userName, string password)
		{
			using (IRSAPIClient client = GetProxy<IRSAPIClient>(webApiUrl, userName, password))
			{
				const string TemplateName = "Relativity Starter Template";
				client.APIOptions.WorkspaceID = -1;
				QueryResultSet<Workspace> resultSet = GetWorkspaceTemplate(client, TemplateName);
				if (!resultSet.Success)
				{
					throw new InvalidOperationException($"An error occurred while attempting to create a workspace from template {TemplateName}: {resultSet.Message}");
				}

				if (resultSet.Results.Count == 0)
				{
					throw new InvalidOperationException(
						$"Trying to create a workspace. Template with the following name does not exist: {TemplateName}");
				}

				int templateWorkspaceId = resultSet.Results[0].Artifact.ArtifactID;
				Workspace workspace = new Workspace();
				workspace.Name = $"Import API Sample Workspace ({DateTime.Now.ToString("MM-dd HH.mm.ss.fff")})";
				workspace.DownloadHandlerApplicationPath = "Relativity.Distributed";
				ProcessOperationResult result = client.Repositories.Workspace.CreateAsync(templateWorkspaceId, workspace);
				return GetWorkspaceArtifactId(client, result);
			}
		}

		public static void DeleteTestWorkspace(string webApiUrl, string userName, string password, int workspaceId)
		{
			if (workspaceId != 0)
			{
				using (IRSAPIClient client = GetProxy<IRSAPIClient>(webApiUrl, userName, password))
				{
					client.Repositories.Workspace.DeleteSingle(workspaceId);
				}				
			}
		}

		public static int GetArtifactTypeId(string webApiUrl, string userName, string password, int workspaceId, string objectTypeName)
		{
			using (var objectManager = GetProxy<IObjectManager>(webApiUrl, userName, password))
			{
				var queryRequest = new QueryRequest
				{
					ObjectType = new ObjectTypeRef
					{
						Name = "Object Type"
					},

					Fields = new[]
					{
						new FieldRef
						{
							Name = "Artifact Type ID"
						}
					},

					Condition = $"'Name' == '{objectTypeName}'"
				};

				Services.Objects.DataContracts.QueryResult result = objectManager.QueryAsync(workspaceId, queryRequest, 0, 1).GetAwaiter().GetResult();
				if (result.TotalCount != 1)
				{
					throw new InvalidOperationException($"Failed to retrieve the artifact type id for the '{objectTypeName}' object type.");
				}

				return (int)result.Objects.Single().FieldValues.Single().Value;
			}
		}

		public static int GetIdentifierFieldId(string webApiUrl, string userName, string password, int workspaceId, string artifactTypeName)
		{
			using (var client = GetProxy<IObjectManager>(webApiUrl, userName, password))
			{
				var queryRequest = new QueryRequest
				{
					Condition = $"'{ArtifactTypeNames.ObjectType}' == '{artifactTypeName}' AND '{FieldFieldNames.IsIdentifier}' == true",
					ObjectType = new ObjectTypeRef { ArtifactTypeID = (int)ArtifactType.Field }
				};

				const int maxItemsToFetch = 2;
				Services.Objects.DataContracts.QueryResult result = client.QueryAsync(workspaceId, queryRequest, 1, maxItemsToFetch).GetAwaiter().GetResult();
				if (result.TotalCount != 1)
				{
					throw new InvalidOperationException($"Failed to retrieve the identifier field id for the '{artifactTypeName}' artifact type.");
				}

				return result.Objects[0].ArtifactID;
			}
		}

		private static QueryResultSet<Workspace> GetWorkspaceTemplate(IRSAPIClient client, string templateName)
		{
			Query<Workspace> query = new Query<Workspace>
			{
				Condition = new TextCondition(WorkspaceFieldNames.Name, TextConditionEnum.EqualTo, templateName),
				Fields = FieldValue.AllFields
			};

			QueryResultSet<Workspace> resultSet = client.Repositories.Workspace.Query(query, 0);
			return resultSet;
		}

		private static int GetWorkspaceArtifactId(IRSAPIClient client, ProcessOperationResult processResult)
		{
			if (processResult.Message != null)
			{
				throw new InvalidOperationException(processResult.Message);
			}

			TaskCompletionSource<ProcessInformation> source = new TaskCompletionSource<ProcessInformation>();
			client.ProcessComplete += (sender, args) =>
			{
				source.SetResult(args.ProcessInformation);
			};

			client.ProcessCompleteWithError += (sender, args) => 
			{
				source.SetResult(args.ProcessInformation);
			};

			client.ProcessFailure += (sender, args) =>
			{
				source.SetResult(args.ProcessInformation);
			};

			client.MonitorProcessState(client.APIOptions, processResult.ProcessID);
			var processInfo = source.Task.GetAwaiter().GetResult();
			if (processInfo.OperationArtifactIDs.Any() && processInfo.OperationArtifactIDs[0] != null)
			{
				return processInfo.OperationArtifactIDs.FirstOrDefault().Value;
			}

			throw new InvalidOperationException(processResult.Message);
		}

		private static T GetProxy<T>(string webApiUrl, string username, string password) where T : class, IDisposable
		{
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			Uri keplerUri = GetKeplerUrl(webApiUrl);
			Uri servicesUri = GetServicesUrl(webApiUrl);
			ServiceFactorySettings serviceFactorySettings = new ServiceFactorySettings(servicesUri, keplerUri, new Relativity.Services.ServiceProxy.UsernamePasswordCredentials(username, password))
			{
				ProtocolVersion = Relativity.Services.Pipeline.WireProtocolVersion.V2
			};

			Relativity.Services.ServiceProxy.ServiceFactory serviceFactory = new Relativity.Services.ServiceProxy.ServiceFactory(serviceFactorySettings);
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			T proxy = serviceFactory.CreateProxy<T>();
			return proxy;
		}

		private static Uri GetKeplerUrl(string webApiUrl)
		{
			var baseUri = new Uri(webApiUrl);
			var host = new Uri(baseUri.GetLeftPart(UriPartial.Authority));
			Uri keplerUri = new Uri(host, "relativity.rest/api");
			return keplerUri;
		}

		private static Uri GetServicesUrl(string webApiUrl)
		{
			var baseUri = new Uri(webApiUrl);
			var host = new Uri(baseUri.GetLeftPart(UriPartial.Authority));
			Uri servicesUri = new Uri(host, "relativity.services");
			return servicesUri;
		}
	}
}