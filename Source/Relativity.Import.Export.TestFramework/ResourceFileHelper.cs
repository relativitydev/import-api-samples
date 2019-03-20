// ----------------------------------------------------------------------------
// <copyright file="ResourceFileHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.TestFramework
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	/// <summary>
	/// Defines static helper methods to manage resource test files.
	/// </summary>
	public static class ResourceFileHelper
	{
        public static string GetBasePath()
        {
            string basePath = System.IO.Path.GetDirectoryName(typeof(FieldHelper).Assembly.Location);
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

        public static string GetResourceFolderDirectory(string folder)
        {
	        string basePath = System.IO.Path.GetDirectoryName(typeof(FieldHelper).Assembly.Location);
	        string folderPath =
		        System.IO.Path.Combine(System.IO.Path.Combine(basePath, "Resources"), folder);
	        return folderPath;
        }

        public static IList<string> GetResourceFolderFiles(string folder)
        {
	        return System.IO.Directory.GetFiles(GetResourceFolderDirectory(folder), "*", SearchOption.AllDirectories)
		        .ToList();
        }

		public static string GetResourceFilePath(string folder, string fileName)
		{
			string sourceFile = System.IO.Path.Combine(GetResourceFolderDirectory(folder), fileName);
            return sourceFile;
        }
    }
}