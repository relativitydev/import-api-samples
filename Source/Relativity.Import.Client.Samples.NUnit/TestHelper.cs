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

	using FizzWare.NBuilder;

	using kCura.Relativity.Client;
	using kCura.Relativity.Client.DTOs;

	using Relativity.Services.Objects;
	using Relativity.Services.Objects.DataContracts;
	using Relativity.Services.ServiceProxy;

	public static class TestHelper
	{
		/// <summary>
		/// The random instance.
		/// </summary>
		private static readonly Random RandomInstance = new Random();

		/// <summary>
		/// The random generator instance.
		/// </summary>
		private static readonly RandomGenerator RandomGeneratorInstance = new RandomGenerator();

		public static void CreateField(
			Uri relativityRestUrl,
			Uri relativityServicesUrl,
			string userName,
			string password,
			int workspaceId,
			int workspaceObjectTypeId,
			kCura.Relativity.Client.DTOs.Field field)
		{
			using (IRSAPIClient client = GetProxy<IRSAPIClient>(relativityRestUrl, relativityServicesUrl, userName, password))
			{
				client.APIOptions.WorkspaceID = workspaceId;
				List<kCura.Relativity.Client.DTOs.Field>
					fieldsToCreate = new List<kCura.Relativity.Client.DTOs.Field>();
				field.ObjectType = new kCura.Relativity.Client.DTOs.ObjectType
				{
					DescriptorArtifactTypeID = workspaceObjectTypeId
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

		public static int CreateObjectType(
			Uri relativityRestUrl,
			Uri relativityServicesUrl,
			string userName,
			string password,
			int workspaceId,
			string objectTypeName)
		{
			using (IRSAPIClient client = GetProxy<IRSAPIClient>(relativityRestUrl, relativityServicesUrl, userName, password))
			{
				client.APIOptions.WorkspaceID = workspaceId;
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
					CopyInstancesOnWorkspaceCreation = false,
					Sampling = true,
					PersistentLists = false,
					CopyInstancesOnParentCopy = false
				};

				int artifactId = client.Repositories.ObjectType.CreateSingle(objectTypeDto);
				return artifactId;
			}
		}

		public static int CreateObjectTypeInstance(
			Uri relativityRestUrl,
			Uri relativityServicesUrl,
			string userName,
			string password,
			int workspaceId,
			int artifactTypeId,
			IDictionary<string, object> fields)
		{
			using (IObjectManager objectManager = GetProxy<IObjectManager>(relativityRestUrl, relativityServicesUrl, userName, password))
			{
				CreateRequest request = new CreateRequest
				{
					ObjectType = new ObjectTypeRef {ArtifactTypeID = artifactTypeId},
					FieldValues = fields.Keys.Select(key => new FieldRefValuePair
						{Field = new FieldRef {Name = key}, Value = fields[key]})
				};

				Services.Objects.DataContracts.CreateResult result
					= objectManager.CreateAsync(workspaceId, request).GetAwaiter().GetResult();
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

		public static int CreateTestWorkspace(
			Uri relativityRestUrl,
			Uri relativityServicesUrl,
			string userName,
			string password,
			string workspaceTemplate,
			Relativity.Logging.ILog logger)
		{
			using (IRSAPIClient client = GetProxy<IRSAPIClient>(relativityRestUrl, relativityServicesUrl, userName, password))
			{
				logger.LogInformation("Retrieving the {TemplateName} workspace template...", workspaceTemplate);
				client.APIOptions.WorkspaceID = -1;
				QueryResultSet<Workspace> resultSet = QueryWorkspaceTemplate(client, workspaceTemplate);
				if (!resultSet.Success)
				{
					throw new InvalidOperationException(
						$"An error occurred while attempting to create a workspace from template {workspaceTemplate}: {resultSet.Message}");
				}

				if (resultSet.Results.Count == 0)
				{
					throw new InvalidOperationException(
						$"Trying to create a workspace. Template with the following name does not exist: {workspaceTemplate}");
				}

				int templateWorkspaceId = resultSet.Results[0].Artifact.ArtifactID;
				logger.LogInformation(
					"Retrieved the {TemplateName} workspace template. TemplateWorkspaceId={TemplateWorkspaceId}.",
					workspaceTemplate,
					templateWorkspaceId);
				Workspace workspace = new Workspace
				{
					Name = $"Import API Sample Workspace ({DateTime.Now:MM-dd HH.mm.ss.fff})",
					DownloadHandlerApplicationPath = "Relativity.Distributed"
				};

				logger.LogInformation("Creating the {WorkspaceName} workspace...", workspace.Name);
				ProcessOperationResult result =
					client.Repositories.Workspace.CreateAsync(templateWorkspaceId, workspace);
				int workspaceArtifactId = QueryWorkspaceArtifactId(client, result, logger);
				logger.LogInformation("Created the {WorkspaceName} workspace. Workspace Artifact ID: {WorkspaceId}.",
					workspace.Name, workspaceArtifactId);
				return workspaceArtifactId;
			}
		}

		public static void DeleteObject(
			Uri relativityRestUrl,
			Uri relativityServicesUrl,
			string userName,
			string password,
			int workspaceId,
			int artifactId)
		{
			using (IObjectManager objectManager = GetProxy<IObjectManager>(relativityRestUrl, relativityServicesUrl, userName, password))
			{
				DeleteRequest request = new DeleteRequest
				{
					Object = new RelativityObjectRef {ArtifactID = artifactId}
				};

				Services.Objects.DataContracts.DeleteResult result
					= objectManager.DeleteAsync(workspaceId, request).GetAwaiter().GetResult();
				if (result.Report.DeletedItems.Count == 0)
				{
					throw new InvalidOperationException($"Failed to delete the {artifactId} object.");
				}
			}
		}

		public static void DeleteTestWorkspace(
			Uri relativityRestUrl,
			Uri relativityServicesUrl,
			string userName,
			string password,
			int workspaceId,
			Relativity.Logging.ILog logger)
		{
			if (workspaceId != 0)
			{
				using (IRSAPIClient client = GetProxy<IRSAPIClient>(relativityRestUrl, relativityServicesUrl, userName, password))
				{
					logger.LogInformation("Deleting the {WorkspaceId} workspace.", workspaceId);
					client.Repositories.Workspace.DeleteSingle(workspaceId);
					logger.LogInformation("Deleted the {WorkspaceId} workspace.", workspaceId);
				}
			}
			else
			{
				logger.LogInformation("Skipped deleting the {WorkspaceId} workspace.", workspaceId);
			}
		}

		public static IList<string> QueryWorkspaceFolders(
			Uri relativityRestUrl,
			Uri relativityServicesUrl,
			string userName,
			string password,
			int workspaceId,
			Relativity.Logging.ILog logger)
		{
			using (IRSAPIClient client =
				GetProxy<IRSAPIClient>(relativityRestUrl, relativityServicesUrl, userName, password))
			{
				logger.LogInformation("Retrieving the {WorkspaceId} workspace folders...", workspaceId);
				client.APIOptions.WorkspaceID = workspaceId;
				Query<Folder> query = new Query<Folder>
				{
					Fields = FieldValue.AllFields
				};

				QueryResultSet<Folder> resultSet = client.Repositories.Folder.Query(query, 0);
				List<string> folders = resultSet.Results.Select(x => x.Artifact.Name).ToList();
				logger.LogInformation("Retrieved {FolderCount} {WorkspaceId} workspace folders.",
					folders.Count,
					workspaceId);
				return folders;
			}
		}

		public static string GetBasePath()
		{
			string basePath = System.IO.Path.GetDirectoryName(typeof(TestHelper).Assembly.Location);
			return basePath;
		}

		public static string GetDocsResourceFilePath(string fileName)
		{
			return GetResourceFilePath("Docs", fileName);
		}

		public static string GetImagesResourceFilePath(string fileName)
		{
			return GetResourceFilePath("Images", fileName);
		}

		public static string GetResourceFilePath(string folder, string fileName)
		{
			string basePath = System.IO.Path.GetDirectoryName(typeof(TestHelper).Assembly.Location);
			string sourceFile = System.IO.Path.Combine(System.IO.Path.Combine(System.IO.Path.Combine(basePath, "Resources"), folder), fileName);
			return sourceFile;
		}

		/// <summary>
		/// Gets the next random string value between <paramref name="minValue"/> and <paramref name="maxValue"/>.
		/// </summary>
		/// <param name="minValue">
		/// The minimum value.
		/// </param>
		/// <param name="maxValue">
		/// The maximum value.
		/// </param>
		/// <returns>
		/// The random string value.
		/// </returns>
		public static string NextString(int minValue, int maxValue)
		{
			return RandomGeneratorInstance.NextString(minValue, maxValue);
		}

		/// <summary>
		/// Gets the next random integer value between <paramref name="minValue"/> and <paramref name="maxValue"/>.
		/// </summary>
		/// <param name="minValue">
		/// The minimum value.
		/// </param>
		/// <param name="maxValue">
		/// The maximum value.
		/// </param>
		/// <returns>
		/// The random integer value.
		/// </returns>
		public static int NextInt(int minValue, int maxValue)
		{
			return RandomInstance.Next(minValue, maxValue);
		}

		/// <summary>
		/// Gets the next random double value between <paramref name="minValue"/> and <paramref name="maxValue"/>.
		/// </summary>
		/// <param name="minValue">
		/// The minimum value.
		/// </param>
		/// <param name="maxValue">
		/// The maximum value.
		/// </param>
		/// <returns>
		/// The random integer value.
		/// </returns>
		public static double NextDouble(int minValue, int maxValue)
		{
			double value = NextInt(minValue, maxValue);
			return value;
		}

		/// <summary>
		/// Gets the next random double value between <paramref name="minValue"/> and <paramref name="maxValue"/>.
		/// </summary>
		/// <param name="minValue">
		/// The minimum value.
		/// </param>
		/// <param name="maxValue">
		/// The maximum value.
		/// </param>
		/// <returns>
		/// The random integer value.
		/// </returns>
		public static decimal NextDecimal(int minValue, int maxValue)
		{
			decimal value = NextInt(minValue, maxValue);
			return value;
		}

		public static int QueryArtifactTypeId(
			Uri relativityRestUrl,
			Uri relativityServicesUrl,
			string userName,
			string password,
			int workspaceId,
			string objectTypeName)
		{
			using (IObjectManager objectManager = GetProxy<IObjectManager>(relativityRestUrl, relativityServicesUrl, userName, password))
			{
				QueryRequest queryRequest = new QueryRequest
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
					return 0;
				}

				return (int)result.Objects.Single().FieldValues.Single().Value;
			}
		}

		public static int QueryIdentifierFieldId(
			Uri relativityRestUrl,
			Uri relativityServicesUrl,
			string userName,
			string password,
			int workspaceId,
			string artifactTypeName)
		{
			using (IObjectManager client = GetProxy<IObjectManager>(relativityRestUrl, relativityServicesUrl, userName, password))
			{
				QueryRequest queryRequest = new QueryRequest
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

		public static int QueryRelativityObjectCount(
			Uri relativityRestUrl,
			Uri relativityServicesUrl,
			string userName,
			string password,
			int workspaceId,
			int artifactTypeId)
		{
			using (IObjectManager client = GetProxy<IObjectManager>(relativityRestUrl, relativityServicesUrl, userName, password))
			{
				QueryRequest queryRequest = new QueryRequest
				{
					ObjectType = new ObjectTypeRef { ArtifactTypeID = artifactTypeId }
				};

				const int maxItemsToFetch = 10;
				Services.Objects.DataContracts.QueryResult result =
					client.QueryAsync(workspaceId, queryRequest, 1, maxItemsToFetch).GetAwaiter().GetResult();
				return result.TotalCount;
			}
		}

		public static int QueryObjectType(
			Uri relativityRestUrl,
			Uri relativityServicesUrl,
			string userName,
			string password,
			int workspaceId,
			string objectTypeName)
		{
			using (IRSAPIClient client = GetProxy<IRSAPIClient>(relativityRestUrl, relativityServicesUrl, userName, password))
			{
				client.APIOptions.WorkspaceID = workspaceId;
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
			Uri relativityRestUrl,
			Uri relativityServicesUrl,
			string userName,
			string password,
			int workspaceId,
			int artifactTypeId,
			IEnumerable<string> fields)
		{
			using (IObjectManager client = GetProxy<IObjectManager>(relativityRestUrl, relativityServicesUrl, userName, password))
			{
				QueryRequest queryRequest = new QueryRequest
				{
					Fields = fields.Select(x => new FieldRef { Name = x }),
					ObjectType = new ObjectTypeRef { ArtifactTypeID = artifactTypeId }
				};

				const int maxItemsToFetch = 50;
				Services.Objects.DataContracts.QueryResult result =
					client.QueryAsync(workspaceId, queryRequest, 1, maxItemsToFetch).GetAwaiter().GetResult();
				return result.Objects;
			}
		}

		public static int QueryWorkspaceObjectTypeDescriptorId(
			Uri relativityRestUrl,
			Uri relativityServicesUrl,
			string userName,
			string password,
			int workspaceId,
			int artifactId)
		{
			kCura.Relativity.Client.DTOs.ObjectType objectType = new kCura.Relativity.Client.DTOs.ObjectType(artifactId)
				{ Fields = FieldValue.AllFields };
			ResultSet<kCura.Relativity.Client.DTOs.ObjectType> resultSet;
			using (IRSAPIClient client = GetProxy<IRSAPIClient>(relativityRestUrl, relativityServicesUrl, userName, password))
			{
				client.APIOptions.WorkspaceID = workspaceId;
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
			Uri relativityRestUrl,
			Uri relativityServicesUrl,
			string userName,
			string password,
			int workspaceId,
			int artifactId,
			IEnumerable<string> fields)
		{
			using (IObjectManager client = GetProxy<IObjectManager>(relativityRestUrl, relativityServicesUrl, userName, password))
			{
				ReadRequest readRequest = new ReadRequest
				{
					Fields = fields.Select(x => new FieldRef { Name = x }),
					Object = new RelativityObjectRef {  ArtifactID = artifactId }
				};

				Services.Objects.DataContracts.ReadResult result = client.ReadAsync(workspaceId, readRequest).GetAwaiter().GetResult();
				return result.Object;
			}
		}

		private static T GetProxy<T>(
			Uri relativityRestUrl,
			Uri relativityServicesUrl,
			string username,
			string password) where T : class, IDisposable
		{
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			ServiceFactorySettings serviceFactorySettings = new ServiceFactorySettings(relativityServicesUrl, relativityRestUrl, new Relativity.Services.ServiceProxy.UsernamePasswordCredentials(username, password))
			{
				ProtocolVersion = Relativity.Services.Pipeline.WireProtocolVersion.V2
			};

			Relativity.Services.ServiceProxy.ServiceFactory serviceFactory = new Relativity.Services.ServiceProxy.ServiceFactory(serviceFactorySettings);
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			T proxy = serviceFactory.CreateProxy<T>();
			return proxy;
		}

		private static QueryResultSet<Workspace> QueryWorkspaceTemplate(IRSAPIClient client, string templateName)
		{
			Query<Workspace> query = new Query<Workspace>
			{
				Condition = new TextCondition(WorkspaceFieldNames.Name, TextConditionEnum.EqualTo, templateName),
				Fields = FieldValue.AllFields
			};

			QueryResultSet<Workspace> resultSet = client.Repositories.Workspace.Query(query, 0);
			return resultSet;
		}

		private static int QueryWorkspaceArtifactId(
			IRSAPIClient client,
			ProcessOperationResult processResult,
			Relativity.Logging.ILog logger)
		{
			if (processResult.Message != null)
			{
				logger.LogError("Failed to create the workspace. Message: {Message}", processResult.Message);
				throw new InvalidOperationException(processResult.Message);
			}

			TaskCompletionSource<ProcessInformation> source = new TaskCompletionSource<ProcessInformation>();
			client.ProcessComplete += (sender, args) =>
			{
				logger.LogInformation("Completed the create workspace process.");
				source.SetResult(args.ProcessInformation);
			};

			client.ProcessCompleteWithError += (sender, args) => 
			{
				logger.LogError("The create process completed with errors. Message: {Message}", args.ProcessInformation.Message);
				source.SetResult(args.ProcessInformation);
			};

			client.ProcessFailure += (sender, args) =>
			{
				logger.LogError("The create process failed to complete. Message: {Message}", args.ProcessInformation.Message);
				source.SetResult(args.ProcessInformation);
			};

			client.MonitorProcessState(client.APIOptions, processResult.ProcessID);
			ProcessInformation processInfo = source.Task.GetAwaiter().GetResult();
			if (processInfo.OperationArtifactIDs.Any() && processInfo.OperationArtifactIDs[0] != null)
			{
				return processInfo.OperationArtifactIDs.FirstOrDefault().Value;
			}

			logger.LogError("The create process failed. Message: {Message}", processResult.Message);
			throw new InvalidOperationException(processResult.Message);
		}
	}
}