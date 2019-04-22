// ----------------------------------------------------------------------------
// <copyright file="FileDto.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.TestFramework
{
	using System;

	/// <summary>
	/// Represents a file data transfer object.
	/// </summary>
	public sealed class FileDto
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FileDto"/> class.
		/// </summary>
		/// <param name="row">
		/// The row.
		/// </param>
		public FileDto(System.Data.DataRow row)
		{
			if (row == null)
			{
				throw new ArgumentNullException(nameof(row));
			}

			this.DocumentArtifactId = (int)row["DocumentArtifactID"];
			this.FileId = (int)row["FileID"];
			this.FileName = row["Filename"] as string;
			this.FileType = (int)row["Type"];
			this.Identifier = row["Identifier"] as string;
			this.InRepository = (bool)row["InRepository"];
			this.Path = row["Location"] as string;
			this.Size = null;
			object sizeValue = row["Size"];
			if (sizeValue != DBNull.Value)
			{
				this.Size = (long)sizeValue;
			}
		}

		/// <summary>
		/// Gets or sets the document artifact identifier.
		/// </summary>
		/// <value>
		/// The unique identifier.
		/// </value>
		public int DocumentArtifactId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the auto-incrementing file identifier.
		/// </summary>
		/// <value>
		/// The file identifier.
		/// </value>
		public int FileId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the original file name.
		/// </summary>
		/// <value>
		/// The file name.
		/// </value>
		public string FileName
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the file type.
		/// </summary>
		/// <value>
		/// The file type.
		/// </value>
		public int FileType
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the file identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		public string Identifier
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether the file is located within the repository.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if the file is located within the repository; otherwise, <see langword="false" />.
		/// </value>
		public bool InRepository
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the full path to the file.
		/// </summary>
		/// <value>
		/// The full path.
		/// </value>
		public string Path
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the size of the file.
		/// </summary>
		/// <value>
		/// The total number of bytes.
		/// </value>
		public long? Size
		{
			get;
			set;
		}
	}
}