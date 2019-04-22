// ----------------------------------------------------------------------------
// <copyright file="TestCategories.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.TestFramework
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents all import and export API test categories.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class TestCategories
    {
		/// <summary>
		/// The export test category.
		/// </summary>
		public const string Export = "Export";

		/// <summary>
		/// The ExtensionMethods test category.
		/// </summary>
		public const string ExtensionMethods = "ExtensionMethods";

		/// <summary>
		/// The file system test category.
		/// </summary>
		public const string FileSystem = "FileSystem";

		/// <summary>
		/// The folder test category.
		/// </summary>
		public const string Folder = "Folder";

		/// <summary>
		/// The framework test category.
		/// </summary>
		public const string Framework = "Framework";

		/// <summary>
		/// The import document test category.
		/// </summary>
		public const string ImportDoc = "ImportDoc";

		/// <summary>
		/// The import image test category.
		/// </summary>
		public const string ImportImage = "ImportImage";

		/// <summary>
		/// The import object test category.
		/// </summary>
		public const string ImportObject = "ImportObject";

		/// <summary>
		/// The import production test category.
		/// </summary>
		public const string ImportProduction = "ImportProduction";

		/// <summary>
		/// The integration test category.
		/// </summary>
		public const string Integration = "Integration";

		/// <summary>
		/// The Outside In test category.
		/// </summary>
		public const string OutsideIn = "OutsideIn";

		/// <summary>
		/// The separate domain test category.
		/// </summary>
		public const string SeparateDomain = "SeparateDomain";

		/// <summary>
		/// The test framework test category.
		/// </summary>
		public const string TestFramework = "TestFramework";

		/// <summary>
		/// The transfer API test category.
		/// </summary>
		public const string TransferApi = "TransferApi";

		/// <summary>
		/// The web-service test category.
		/// </summary>
		public const string WebService = "WebService";
	}
}