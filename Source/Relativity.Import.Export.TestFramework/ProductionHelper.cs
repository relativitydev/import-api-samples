// ----------------------------------------------------------------------------
// <copyright file="ProductionHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.TestFramework
{
	using System;

	/// <summary>
	/// Defines static helper methods to manage productions.
	/// </summary>
	public static class ProductionHelper
	{
		public static int CreateProduction(
			IntegrationTestParameters parameters,
			string productionName,
			string batesPrefix,
			Relativity.Logging.ILog logger)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}

			const int productionFontSize = 10;
			const int numberOfDigits = 7;
			using (Relativity.Productions.Services.IProductionManager client = ServiceHelper.GetServiceProxy<Relativity.Productions.Services.IProductionManager>(parameters))
			{
				var production = new Relativity.Productions.Services.Production
				{
					Details = new Relativity.Productions.Services.ProductionDetails
					{
						BrandingFontSize = productionFontSize,
						ScaleBrandingFont = false,
					},
					Name = productionName,
					Numbering = new Relativity.Productions.Services.DocumentLevelNumbering
					{
						NumberingType = Relativity.Productions.Services.NumberingType.DocumentLevel,
						BatesPrefix = batesPrefix,
						BatesStartNumber = 0,
						NumberOfDigitsForDocumentNumbering = numberOfDigits,
						IncludePageNumbers = false,
					},
				};
				return client.CreateSingleAsync(parameters.WorkspaceId, production).ConfigureAwait(false).GetAwaiter()
					.GetResult();
			}
		}

		public static Relativity.Productions.Services.Production QueryProduction(IntegrationTestParameters parameters, int productionId)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			using (Relativity.Productions.Services.IProductionManager client =
				ServiceHelper.GetServiceProxy<Relativity.Productions.Services.IProductionManager>(parameters))
			{
				Relativity.Productions.Services.Production production = client
					.ReadSingleAsync(parameters.WorkspaceId, productionId).ConfigureAwait(false).GetAwaiter().GetResult();
				if (production == null)
				{
					throw new InvalidOperationException($"The production {productionId} does not exist.");
				}

				return production;
			}
		}
	}
}