// ----------------------------------------------------------------------------
// <copyright file="DocImportRecord.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.Samples.NUnit.Tests
{
	/// <summary>
	/// Represents a single document import record.
	/// </summary>
	public class DocImportRecord
	{
		/// <summary>
		/// Gets or sets the control number.
		/// </summary>
		/// <value>
		/// The control number.
		/// </value>
		public string ControlNumber
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the folder name.
		/// </summary>
		/// <value>
		/// The folder name.
		/// </value>
		public string Folder
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the full path to the import file.
		/// </summary>
		/// <value>
		/// The full path.
		/// </value>
		public string File
		{
			get;
			set;
		}
	}
}