// ----------------------------------------------------------------------------
// <copyright file="ResourceFileHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.TestFramework
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Reflection;

	/// <summary>
	/// Defines static helper methods to manage resource test files.
	/// </summary>
	public static class ResourceFileHelper
	{
		/// <summary>
		/// Extracts the specified resource to the specified file.
		/// </summary>
		/// <param name="assembly">
		/// The assembly containing the resource.
		/// </param>
		/// <param name="name">
		/// The resource name.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Thrown when the resource <paramref name="name"/> does not exist within the assembly.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="assembly"/> or <paramref name="name"/> is <see langword="null"/> or empty.
		/// </exception>
		/// <exception cref="IOException">
		/// Thrown if a serious error occurs writing the embedded contents out to a file.
		/// </exception>
		/// <returns>
		/// A <stream cref="Stream" /> containing the file contents.
		/// </returns>
		public static Stream ExtractToStream(Assembly assembly, string name)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException(nameof(assembly));
			}

			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name));
			}

			var resourceStream = assembly.GetManifestResourceStream(name);
			if (resourceStream == null)
			{
				var message = string.Format(
					CultureInfo.InvariantCulture,
					"The embedded resource '{0}' does not exist in this assembly.\n\nAvailable resources: {1}",
					name,
					string.Join(",", assembly.GetManifestResourceNames()));
				throw new ArgumentException(message, name);
			}

			return resourceStream;
		}

		public static string GetBasePath()
		{
			string basePath = System.IO.Path.GetDirectoryName(typeof(FieldHelper).Assembly.Location);
			return basePath;
		}

		public static string GetBaseFilePath(string fileName)
		{
			return System.IO.Path.Combine(GetBasePath(), fileName);
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