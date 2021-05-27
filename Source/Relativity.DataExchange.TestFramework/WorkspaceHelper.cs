// ----------------------------------------------------------------------------
// <copyright file="WorkspaceHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Relativity.Services.Folder;
	using Relativity.Services.Interfaces.Shared;
	using Relativity.Services.Interfaces.Shared.Models;
	using Relativity.Services.Interfaces.Workspace;
	using Relativity.Services.Interfaces.Workspace.Models;
	using Relativity.Services.Objects.DataContracts;

	/// <summary>
	/// Defines static helper methods to manage workspaces.
	/// </summary>
	public static class WorkspaceHelper
	{
		public static void CreateTestWorkspace(IntegrationTestParameters parameters, Relativity.Logging.ILog logger)
		{
			parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
			logger = logger ?? throw new ArgumentNullException(nameof(logger));

			int templateWorkspaceId = RetrieveWorkspaceId(parameters, logger, parameters.WorkspaceTemplate);

			// pasted:
			using (IWorkspaceManager workspaceManager = ServiceHelper.GetServiceProxy<IWorkspaceManager>(parameters))
			{
				WorkspaceResponse readResponse = workspaceManager.ReadAsync(templateWorkspaceId).GetAwaiter().GetResult();

				var createRequest = new WorkspaceRequest(readResponse)
				{
					Name = $"Import API Sample Workspace ({DateTime.Now:MM-dd HH.mm.ss.fff})",
					Template = new Securable<ObjectIdentifier>(new ObjectIdentifier { ArtifactID = templateWorkspaceId }),
				};

				logger.LogInformation("Creating the {WorkspaceName} workspace...", createRequest.Name);

				WorkspaceResponse createResponse = workspaceManager.CreateAsync(createRequest).GetAwaiter().GetResult();
				logger.LogInformation("Completed the create workspace process.");
				parameters.WorkspaceId = createResponse.ArtifactID;
			}
		}


		private static int RetrieveWorkspaceId(IntegrationTestParameters parameters, Relativity.Logging.ILog logger, string workspaceName)
		{
			logger.LogInformation("Retrieving the {workspaceName} workspace...", workspaceName);
			var queryRequest = new QueryRequest { Condition = $"'Name' == '{workspaceName}'" };

			QueryResultSlim queryResponse;
			using (IWorkspaceManager workspaceManager = ServiceHelper.GetServiceProxy<IWorkspaceManager>(parameters))
			{
				queryResponse = workspaceManager.QueryEligibleTemplatesAsync(queryRequest, 0, 2).GetAwaiter().GetResult();
			}

			switch (queryResponse.Objects.Count)
			{
				case 0:
					throw new InvalidOperationException($"Workspace with the following name does not exist: {workspaceName}");
				case 1:
					int workspaceId = queryResponse.Objects[0].ArtifactID;
					logger.LogInformation($"Retrieved the {workspaceName} workspace. workspaceId={workspaceId}.", workspaceName, workspaceId);
					return workspaceId;
				default:
					throw new InvalidOperationException($"More then one Workspace with the following name exists: {workspaceName}");
			}
		}

		public static void DeleteTestWorkspace(IntegrationTestParameters parameters, Relativity.Logging.ILog logger)
		{
			parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
			logger = logger ?? throw new ArgumentNullException(nameof(logger));

			if (parameters.WorkspaceId != 0)
			{
				using (IWorkspaceManager workspaceManager = ServiceHelper.GetServiceProxy<IWorkspaceManager>(parameters))
				{
					logger.LogInformation("Deleting the {WorkspaceId} workspace.", parameters.WorkspaceId);
					workspaceManager.DeleteAsync(parameters.WorkspaceId).GetAwaiter().GetResult();
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
			parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
			logger = logger ?? throw new ArgumentNullException(nameof(logger));

			using (IFolderManager folderManager = ServiceHelper.GetServiceProxy<IFolderManager>(parameters))
			{
				var folderQuery = new Services.Query();
				var queryResponse = folderManager.QueryAsync(parameters.WorkspaceId, folderQuery).GetAwaiter().GetResult();
				List<string> folders = queryResponse.Results.Select(x => x.Artifact.Name).ToList();
				logger.LogInformation(
					"Retrieved {FolderCount} {WorkspaceId} workspace folders.",
					folders.Count,
					parameters.WorkspaceId);
				return folders;
			}
		}
	}
}