// ----------------------------------------------------------------------------
// <copyright file="WorkspaceHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.TestFramework
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using kCura.Relativity.Client;
	using kCura.Relativity.Client.DTOs;

	/// <summary>
	/// Defines static helper methods to manage workspaces.
	/// </summary>
	public static class WorkspaceHelper
	{
		public static void CreateTestWorkspace(IntegrationTestParameters parameters, Relativity.Logging.ILog logger)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}

			using (IRSAPIClient client = ServiceHelper.GetServiceProxy<IRSAPIClient>(parameters))
			{
				logger.LogInformation(
					"Retrieving the {TemplateName} workspace template...",
					parameters.WorkspaceTemplate);
				client.APIOptions.WorkspaceID = -1;
				QueryResultSet<Workspace> resultSet = QueryWorkspaceTemplate(client, parameters.WorkspaceTemplate);
				if (!resultSet.Success)
				{
					throw new InvalidOperationException(
						$"An error occurred while attempting to create a workspace from template {parameters.WorkspaceTemplate}: {resultSet.Message}");
				}

				if (resultSet.Results.Count == 0)
				{
					throw new InvalidOperationException(
						$"Trying to create a workspace. Template with the following name does not exist: {parameters.WorkspaceTemplate}");
				}

				int templateWorkspaceId = resultSet.Results[0].Artifact.ArtifactID;
				logger.LogInformation(
					"Retrieved the {TemplateName} workspace template. TemplateWorkspaceId={TemplateWorkspaceId}.",
					parameters.WorkspaceTemplate,
					templateWorkspaceId);
				Workspace workspace = new Workspace
				{
					Name = $"Import API Sample Workspace ({DateTime.Now:MM-dd HH.mm.ss.fff})",
					DownloadHandlerApplicationPath = "Relativity.Distributed",
				};

				logger.LogInformation("Creating the {WorkspaceName} workspace...", workspace.Name);
				ProcessOperationResult result =
					client.Repositories.Workspace.CreateAsync(templateWorkspaceId, workspace);
				parameters.WorkspaceId = QueryWorkspaceArtifactId(client, result, logger);
				logger.LogInformation(
					"Created the {WorkspaceName} workspace. Workspace Artifact ID: {WorkspaceId}.",
					workspace.Name,
					parameters.WorkspaceId);
			}
		}

		public static void DeleteTestWorkspace(IntegrationTestParameters parameters, Relativity.Logging.ILog logger)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}

			if (parameters.WorkspaceId != 0)
			{
				using (IRSAPIClient client = ServiceHelper.GetServiceProxy<IRSAPIClient>(parameters))
				{
					client.APIOptions.WorkspaceID = -1;
					logger.LogInformation("Deleting the {WorkspaceId} workspace.", parameters.WorkspaceId);
					client.Repositories.Workspace.DeleteSingle(parameters.WorkspaceId);
					logger.LogInformation("Deleted the {WorkspaceId} workspace.", parameters.WorkspaceId);
				}
			}
			else
			{
				logger.LogInformation("Skipped deleting the {WorkspaceId} workspace.", parameters.WorkspaceId);
			}
		}

		public static IList<string> QueryWorkspaceFolders(IntegrationTestParameters parameters, Relativity.Logging.ILog logger)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}

			using (IRSAPIClient client = ServiceHelper.GetServiceProxy<IRSAPIClient>(parameters))
			{
				client.APIOptions.WorkspaceID = parameters.WorkspaceId;
				logger.LogInformation("Retrieving the {WorkspaceId} workspace folders...", parameters.WorkspaceId);
				Query<Folder> query = new Query<Folder> { Fields = FieldValue.AllFields };
				QueryResultSet<Folder> resultSet = client.Repositories.Folder.Query(query, 0);
				List<string> folders = resultSet.Results.Select(x => x.Artifact.Name).ToList();
				logger.LogInformation(
					"Retrieved {FolderCount} {WorkspaceId} workspace folders.",
					folders.Count,
					parameters.WorkspaceId);
				return folders;
			}
		}

		private static QueryResultSet<Workspace> QueryWorkspaceTemplate(IRSAPIClient client, string templateName)
		{
			Query<Workspace> query = new Query<Workspace>
			{
				Condition = new TextCondition(WorkspaceFieldNames.Name, TextConditionEnum.EqualTo, templateName),
				Fields = FieldValue.AllFields,
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
				logger.LogError(
					"The create process completed with errors. Message: {Message}",
					args.ProcessInformation.Message);
				source.SetResult(args.ProcessInformation);
			};

			client.ProcessFailure += (sender, args) =>
			{
				logger.LogError(
					"The create process failed to complete. Message: {Message}",
					args.ProcessInformation.Message);
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