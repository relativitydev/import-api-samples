// ----------------------------------------------------------------------------
// <copyright file="TestDirectory.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework
{
	using System;
	using System.Diagnostics;
	using System.IO;

	/// <summary>
	/// Represents a disposable temporary directory class object.
	/// </summary>
	public class TestDirectory : IDisposable
	{
		/// <summary>
		/// The disposed backing.
		/// </summary>
		private bool disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="TestDirectory" /> class.
		/// </summary>
		public TestDirectory()
		{
			this.ClearReadOnlyAttributes = false;
			this.DeleteDirectory = true;
			this.Directory = Path.Combine(Path.GetTempPath(), "RelativityTmpDir_" + DateTime.Now.Ticks + "_" + Guid.NewGuid());
		}

		/// <summary>
		/// Gets or sets a value indicating whether to clear all read-only attributes within the temp directory once this instance is disposed.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to clear all read-only attributes; otherwise, <see langword="false" />.
		/// </value>
		public bool ClearReadOnlyAttributes
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to delete the directory once this instance is disposed.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to delete the directory; otherwise, <see langword="false" />.
		/// </value>
		public bool DeleteDirectory
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the directory.
		/// </summary>
		/// <value>
		/// The directory.
		/// </value>
		public string Directory
		{
			get;
			private set;
		}

		/// <summary>
		/// Creates the directory.
		/// </summary>
		public void Create()
		{
			if (!System.IO.Directory.Exists(this.Directory))
			{
				System.IO.Directory.CreateDirectory(this.Directory);
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Determines whether the directory exists on disk.
		/// </summary>
		/// <returns>
		/// <see langword="true" /> if the directory exists; otherwise, <see langword="false" />.
		/// </returns>
		public bool Exists()
		{
			return System.IO.Directory.Exists(this.Directory);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing">
		/// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
		/// </param>
		private void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}

			if (disposing)
			{
				if (this.DeleteDirectory && !string.IsNullOrEmpty(this.Directory)
					&& System.IO.Directory.Exists(this.Directory))
				{
					try
					{
						if (this.ClearReadOnlyAttributes)
						{
							var files = System.IO.Directory.GetFiles(this.Directory, "*", SearchOption.AllDirectories);
							foreach (var file in files)
							{
								var attributes = File.GetAttributes(file);
								File.SetAttributes(file, attributes & ~FileAttributes.ReadOnly);
							}
						}

						System.IO.Directory.Delete(this.Directory, true);
					}
					catch (IOException e)
					{
						System.Diagnostics.Trace.WriteLine(
							$"Failed to tear down the '{this.Directory}' temp directory due to an I/O issue. Exception: "
							+ e);
					}
					catch (UnauthorizedAccessException e)
					{
						Debug.WriteLine(
							$"Failed to tear down the '{this.Directory}' temp directory due to unauthorized access. Exception: "
							+ e);
					}
				}

				this.Directory = null;
			}

			this.disposed = true;
		}
	}
}