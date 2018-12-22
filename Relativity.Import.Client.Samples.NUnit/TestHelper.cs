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
			string webApiUrl,
			string userName,
			string password,
			int workspaceId,
			int workspaceObjectTypeId,
			kCura.Relativity.Client.DTOs.Field field)
		{
			using (IRSAPIClient client = GetProxy<IRSAPIClient>(webApiUrl, userName, password))
			{
				client.APIOptions.WorkspaceID = workspaceId;
				List<kCura.Relativity.Client.DTOs.Field> fieldsToCreate = new List<kCura.Relativity.Client.DTOs.Field>();
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

				var innerExceptions = new List<Exception>();
				foreach (var result in resultSet.Results.Where(x => !x.Success))
				{
					innerExceptions.Add(new InvalidOperationException(result.Message));
				}

				throw new AggregateException(
					$"Failed to create the {field.Name} field. Error: {resultSet.Message}", innerExceptions);
			}
		}

		public static int CreateObjectType(
			string webApiUrl,
			string userName,
			string password,
			int workspaceId,
			string objectTypeName)
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

				int artifactTypeId = client.Repositories.ObjectType.CreateSingle(objectTypeDto);
				return artifactTypeId;
			}
		}

		public static int CreateObjectTypeInstance(
			string webApiUrl,
			string userName,
			string password,
			int workspaceId,
			int artifactTypeId,
			IDictionary<string, object> fields)
		{
			using (var objectManager = GetProxy<IObjectManager>(webApiUrl, userName, password))
			{
				var request = new CreateRequest
				{
					ObjectType = new ObjectTypeRef { ArtifactTypeID = artifactTypeId },
					FieldValues = fields.Keys.Select(key => new FieldRefValuePair { Field = new FieldRef { Name = key }, Value = fields[key] })
				};

				Services.Objects.DataContracts.CreateResult result
					= objectManager.CreateAsync(workspaceId, request).GetAwaiter().GetResult();
				var innerExceptions = result.EventHandlerStatuses.Where(x => !x.Success)
					.Select(status => new InvalidOperationException(status.Message)).Cast<Exception>().ToList();
				if (innerExceptions.Count == 0)
				{
					return result.Object.ArtifactID;
				}

				throw new AggregateException(
					$"Failed to create a new instance for {artifactTypeId} artifact type.", innerExceptions);
			}
		}

		public static int CreateTestWorkspace(
			string webApiUrl,
			string userName,
			string password,
			Relativity.Logging.ILog logger)
		{
			using (IRSAPIClient client = GetProxy<IRSAPIClient>(webApiUrl, userName, password))
			{
				const string TemplateName = "Relativity Starter Template";
				logger.LogInformation("Retrieving the {TemplateName} workspace template...", TemplateName);
				client.APIOptions.WorkspaceID = -1;
				QueryResultSet<Workspace> resultSet = QueryWorkspaceTemplate(client, TemplateName);
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
				logger.LogInformation("Retrieved the {TemplateName} workspace template. TemplateWorkspaceId={TemplateWorkspaceId}.",
					TemplateName,
					templateWorkspaceId);
				Workspace workspace = new Workspace
				{
					Name = $"Import API Sample Workspace ({DateTime.Now:MM-dd HH.mm.ss.fff})",
					DownloadHandlerApplicationPath = "Relativity.Distributed"
				};

				logger.LogInformation("Creating the {WorkspaceName} workspace...", workspace.Name);
				ProcessOperationResult result = client.Repositories.Workspace.CreateAsync(templateWorkspaceId, workspace);
				int workspaceArtifactId = QueryWorkspaceArtifactId(client, result, logger);
				logger.LogInformation("Created the {WorkspaceName} workspace. Workspace Artifact ID: {WorkspaceId}.",
					workspace.Name, workspaceArtifactId);
				return workspaceArtifactId;
			}
		}

		public static void DeleteTestWorkspace(
			string webApiUrl,
			string userName,
			string password,
			int workspaceId,
			Relativity.Logging.ILog logger)
		{
			if (workspaceId != 0)
			{
				using (IRSAPIClient client = GetProxy<IRSAPIClient>(webApiUrl, userName, password))
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

		public static string GetBasePath()
		{
			string basePath = System.IO.Path.GetDirectoryName(typeof(TestHelper).Assembly.Location);
			return basePath;
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

		public static int QueryArtifactTypeId(string webApiUrl, string userName, string password, int workspaceId, string objectTypeName)
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

		public static int QueryIdentifierFieldId(string webApiUrl, string userName, string password, int workspaceId, string artifactTypeName)
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

		public static int QueryRelativityObjectCount(string webApiUrl, string userName, string password, int workspaceId, int artifactTypeId)
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

		public static IList<RelativityObject> QueryRelativityObjects(
			string webApiUrl,
			string userName,
			string password,
			int workspaceId,
			int artifactTypeId,
			IEnumerable<string> fields)
		{
			using (var client = GetProxy<IObjectManager>(webApiUrl, userName, password))
			{
				var queryRequest = new QueryRequest
				{
					Fields = fields.Select(x => new FieldRef { Name = x }),
					ObjectType = new ObjectTypeRef { ArtifactTypeID = artifactTypeId }
				};

				const int maxItemsToFetch = 50;
				var result = client.QueryAsync(workspaceId, queryRequest, 1, maxItemsToFetch).GetAwaiter().GetResult();
				return result.Objects;
			}
		}

		public static int QueryWorkspaceObjectTypeDescriptorId(
			string webApiUrl,
			string userName,
			string password,
			int workspaceId,
			int workspaceObjectTypeId)
		{
			var objectType = new kCura.Relativity.Client.DTOs.ObjectType(workspaceObjectTypeId)
				{Fields = FieldValue.AllFields};
			ResultSet<kCura.Relativity.Client.DTOs.ObjectType> resultSet;
			using (IRSAPIClient client = GetProxy<IRSAPIClient>(webApiUrl, userName, password))
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

		private static Uri GetKeplerUrl(string webApiUrl)
		{
			var baseUri = new Uri(webApiUrl);
			var host = new Uri(baseUri.GetLeftPart(UriPartial.Authority));
			Uri keplerUri = new Uri(host, "relativity.rest/api");
			return keplerUri;
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

		private static Uri GetServicesUrl(string webApiUrl)
		{
			var baseUri = new Uri(webApiUrl);
			var host = new Uri(baseUri.GetLeftPart(UriPartial.Authority));
			Uri servicesUri = new Uri(host, "relativity.services");
			return servicesUri;
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
			var processInfo = source.Task.GetAwaiter().GetResult();
			if (processInfo.OperationArtifactIDs.Any() && processInfo.OperationArtifactIDs[0] != null)
			{
				return processInfo.OperationArtifactIDs.FirstOrDefault().Value;
			}

			logger.LogError("The create process failed. Message: {Message}", processResult.Message);
			throw new InvalidOperationException(processResult.Message);
		}
	}
}