// ----------------------------------------------------------------------------
// <copyright file="CryptoHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.TestFramework
{
	using System;
	using System.IO;
	using System.Security.Cryptography;
	using System.Text;

	/// <summary>
	/// Provides static methods to perform simple cryptography for test data.
	/// </summary>
	/// <remarks>
	/// This is only intended to provide basic obfuscation for test environment parameters.
	/// </remarks>
	internal static class CryptoHelper
	{
		/// <summary>
		/// The vector value.
		/// </summary>
		private const string Vector = "7F6D86EAA5F84D17";

		/// <summary>
		/// The salt value.
		/// </summary>
		private const string Salt = "0216F04EC95A49C7";

		/// <summary>
		/// The hash value.
		/// </summary>
		private const string Hash = "SHA1";

		/// <summary>
		/// The password value.
		/// </summary>
		private const string Pwd = "891802791973";

		/// <summary>
		/// The total number of iterations.
		/// </summary>
		private const int Iterations = 2;

		/// <summary>
		/// The key value.
		/// </summary>
		private const int Key = 256 / 8;

		/// <summary>
		/// Decrypts the value.
		/// </summary>
		/// <param name="value">
		/// The encrypted value.
		/// </param>
		/// <returns>
		/// The decrypted value.
		/// </returns>
		public static string Decrypt(string value)
		{
			using (var algorithm = new AesManaged())
			{
				var vectorBytes = Encoding.ASCII.GetBytes(Vector);
				var saltBytes = Encoding.ASCII.GetBytes(Salt);
				var valueBytes = Convert.FromBase64String(value);
				using (var passwordBytes = new PasswordDeriveBytes(Pwd, saltBytes, Hash, Iterations))
				{
#pragma warning disable 618
					var keyBytes = passwordBytes.GetBytes(Key);
#pragma warning restore 618
					algorithm.Mode = CipherMode.CBC;
					using (var decryptor = algorithm.CreateDecryptor(keyBytes, vectorBytes))
					using (var ms = new MemoryStream(valueBytes))
					{
						var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
						var decrypted = new byte[valueBytes.Length];
						var decryptedByteCount = cs.Read(decrypted, 0, decrypted.Length);
						return Encoding.UTF8.GetString(decrypted, 0, decryptedByteCount);
					}
				}
			}
		}

		/// <summary>
		/// Encrypts the value.
		/// </summary>
		/// <param name="value">
		/// The plain text value.
		/// </param>
		/// <returns>
		/// The encrypts value.
		/// </returns>
		public static string Encrypt(string value)
		{
			using (var algorithm = new AesManaged())
			{
				var vectorBytes = Encoding.ASCII.GetBytes(Vector);
				var saltBytes = Encoding.ASCII.GetBytes(Salt);
				var valueBytes = Encoding.UTF8.GetBytes(value);
				using (var passwordBytes = new PasswordDeriveBytes(Pwd, saltBytes, Hash, Iterations))
				{
#pragma warning disable 618
					var keyBytes = passwordBytes.GetBytes(Key);
#pragma warning restore 618
					algorithm.Mode = CipherMode.CBC;
					using (var encryptor = algorithm.CreateEncryptor(keyBytes, vectorBytes))
					using (var ms = new MemoryStream())
					{
						var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
						cs.Write(valueBytes, 0, valueBytes.Length);
						cs.FlushFinalBlock();
						var result = Convert.ToBase64String(ms.ToArray());
						return result;
					}
				}
			}
		}
	}
}