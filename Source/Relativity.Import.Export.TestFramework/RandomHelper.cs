// -----------------------------------------------------------------------------------------------------
// <copyright file="RandomHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Security.Cryptography;

	using FizzWare.NBuilder;

	/// <summary>
	/// Represents static helper methods to create random test files and data.
	/// </summary>
	public static class RandomHelper
	{
		/// <summary>
		/// The random instance.
		/// </summary>
		private static readonly Random RandomInstance = new Random();

		/// <summary>
		/// The random generator instance.
		/// </summary>
		private static readonly RandomGenerator RandomGeneratorInstance = new RandomGenerator();

		/// <summary>
		/// Creates a new binary file whose file size is between <paramref name="minLength"/> and <paramref name="maxLength"/>.
		/// </summary>
		/// <param name="minLength">
		/// The minimum file length.
		/// </param>
		/// <param name="maxLength">
		/// The maximum file length.
		/// </param>
		/// <param name="directory">
		/// The directory to create the file.
		/// </param>
		/// <returns>
		/// The file.
		/// </returns>
		public static string NextBinaryFile(long minLength, long maxLength, string directory)
		{
			var fileName = "IXApi_TestFile_" + DateTime.Now.Ticks + "_" + Guid.NewGuid().ToString("D");
			return NextBinaryFile(minLength, maxLength, directory, fileName);
		}

		/// <summary>
		/// Creates a new binary file whose file size is between <paramref name="minLength"/> and <paramref name="maxLength"/>.
		/// </summary>
		/// <param name="minLength">
		/// The minimum file length.
		/// </param>
		/// <param name="maxLength">
		/// The maximum file length.
		/// </param>
		/// <param name="directory">
		/// The directory to create the file.
		/// </param>
		/// <param name="fileName">
		/// The file name.
		/// </param>
		/// <returns>
		/// The file.
		/// </returns>
		public static string NextBinaryFile(long minLength, long maxLength, string directory, string fileName)
		{
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var file = Path.Combine(directory, fileName);
			using (var fileStream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				var length = RandomGeneratorInstance.Next(minLength, maxLength);
				fileStream.SetLength(length);
				return file;
			}
		}

		/// <summary>
		/// Gets the next random integer value between <paramref name="minValue"/> and <paramref name="maxValue"/>.
		/// </summary>
		/// <param name="minValue">
		/// The minimum value.
		/// </param>
		/// <param name="maxValue">
		/// The maximum value.
		/// </param>
		/// <returns>
		/// The random integer value.
		/// </returns>
		public static int NextInt(int minValue, int maxValue)
		{
			return RandomInstance.Next(minValue, maxValue);
		}

		/// <summary>
		/// Gets the next random double value between <paramref name="minValue"/> and <paramref name="maxValue"/>.
		/// </summary>
		/// <param name="minValue">
		/// The minimum value.
		/// </param>
		/// <param name="maxValue">
		/// The maximum value.
		/// </param>
		/// <returns>
		/// The random integer value.
		/// </returns>
		public static double NextDouble(int minValue, int maxValue)
		{
			double value = NextInt(minValue, maxValue);
			return value;
		}

		/// <summary>
		/// Gets the next random double value between <paramref name="minValue"/> and <paramref name="maxValue"/>.
		/// </summary>
		/// <param name="minValue">
		/// The minimum value.
		/// </param>
		/// <param name="maxValue">
		/// The maximum value.
		/// </param>
		/// <returns>
		/// The random integer value.
		/// </returns>
		public static decimal NextDecimal(int minValue, int maxValue)
		{
			decimal value = NextInt(minValue, maxValue);
			return value;
		}

		/// <summary>
		/// Creates a new random text file whose file size is between <paramref name="minValue"/> and <paramref name="maxValue"/>.
		/// </summary>
		/// <param name="minValue">
		/// The minimum value.
		/// </param>
		/// <param name="maxValue">
		/// The maximum value.
		/// </param>
		/// <param name="file">
		/// The full path to the file to create.
		/// </param>
		public static void NextTextFile(int minValue, int maxValue, string file)
		{
			string directory = System.IO.Path.GetDirectoryName(file);
			if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var text = NextString(minValue, maxValue);
			File.WriteAllText(file, text);
		}

		/// <summary>
		/// Creates a new random text file whose file size is between <paramref name="minLength"/> and <paramref name="maxLength"/>.
		/// </summary>
		/// <param name="minLength">
		/// The minimum file length.
		/// </param>
		/// <param name="maxLength">
		/// The maximum file length.
		/// </param>
		/// <param name="directory">
		/// The directory to create the file.
		/// </param>
		/// <param name="readOnly">
		/// Specify whether to set the file read-only attribute.
		/// </param>
		/// <returns>
		/// The file.
		/// </returns>
		public static string NextTextFile(int minLength, int maxLength, string directory, bool readOnly)
		{
			var fileName = "Iapi_TestFile_" + DateTime.Now.Ticks + "_" + Guid.NewGuid().ToString("D");
			var file = NextTextFile(minLength, maxLength, directory, fileName);
			if (!readOnly)
			{
				return file;
			}

			var fileAttributes = File.GetAttributes(file);
			File.SetAttributes(file, fileAttributes | FileAttributes.ReadOnly);
			return file;
		}

		/// <summary>
		/// Creates a new random text file whose file size is between <paramref name="minValue"/> and <paramref name="maxValue"/>.
		/// </summary>
		/// <param name="minValue">
		/// The minimum value.
		/// </param>
		/// <param name="maxValue">
		/// The maximum value.
		/// </param>
		/// <param name="directory">
		/// The directory to create the file.
		/// </param>
		/// <param name="fileName">
		/// The file name.
		/// </param>
		/// <returns>
		/// The file.
		/// </returns>
		public static string NextTextFile(int minValue, int maxValue, string directory, string fileName)
		{
			checked
			{
				if (!Directory.Exists(directory))
				{
					Directory.CreateDirectory(directory);
				}

				if (string.IsNullOrEmpty(Path.GetExtension(fileName)))
				{
					fileName = Path.ChangeExtension(fileName, ".txt");
				}

				var text = NextString(minValue, maxValue);
				var file = Path.Combine(directory, fileName);
				File.WriteAllText(file, text);
				return file;
			}
		}

		/// <summary>
		/// Gets the next random boolean value.
		/// </summary>
		/// <returns>
		/// The random boolean value.
		/// </returns>
		public static bool NextBoolean()
		{
			return RandomInstance.NextDouble() >= 0.5;
		}

		/// <summary>
		/// Gets the next random string value between <paramref name="minValue"/> and <paramref name="maxValue"/>.
		/// </summary>
		/// <param name="minValue">
		/// The minimum value.
		/// </param>
		/// <param name="maxValue">
		/// The maximum value.
		/// </param>
		/// <returns>
		/// The random string value.
		/// </returns>
		public static string NextString(int minValue, int maxValue)
		{
			return RandomGeneratorInstance.NextString(minValue, maxValue);
		}

		/// <summary>
		/// Gets the next random Uri.
		/// </summary>
		/// <returns>
		/// The random <see cref="Uri"/> instance.
		/// </returns>
		public static Uri NextUri()
		{
			const string availableChars =
				"0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
			using (var generator = new RNGCryptoServiceProvider())
			{
				var bytes = new byte[16];
				generator.GetBytes(bytes);
				var chars = bytes
					.Select(b => availableChars[b % availableChars.Length]);
				var token = new string(chars.ToArray());
				return new Uri($"https://www.{token}.com");
			}
		}

		/// <summary>
		/// Gets the next random enumeration value.
		/// </summary>
		/// <param name="enumType">
		/// The enumeration type.
		/// </param>
		/// <returns>
		/// The random enumeration value.
		/// </returns>
		public static int NextEnum(Type enumType)
		{
			Array enumValues = Enum.GetValues(enumType);
			int randomValue = (int)enumValues.GetValue(RandomInstance.Next(enumValues.Length));
			return randomValue;
		}

		/// <summary>
		/// Gets the next random integer value between <paramref name="minValue"/> and <paramref name="maxValue"/>.
		/// </summary>
		/// <param name="minValue">
		/// The minimum value.
		/// </param>
		/// <param name="maxValue">
		/// The maximum value.
		/// </param>
		/// <returns>
		/// The random integer value.
		/// </returns>
		public static int NextInt32(int minValue, int maxValue)
		{
			return RandomInstance.Next(minValue, maxValue);
		}

		/// <summary>
		/// Gets the next random long value between <paramref name="minValue"/> and <paramref name="maxValue"/>.
		/// </summary>
		/// <param name="minValue">
		/// The minimum value.
		/// </param>
		/// <param name="maxValue">
		/// The maximum value.
		/// </param>
		/// <returns>
		/// The random long value.
		/// </returns>
		public static long NextInt64(long minValue, long maxValue)
		{
			long value = minValue + (long)(RandomInstance.NextDouble() * (maxValue - minValue));
			return value;
		}

		/// <summary>
		/// Gets a random list using the specified constraints.
		/// </summary>
		/// <param name="min">
		/// The minimum value.
		/// </param>
		/// <param name="max">
		/// The maximum value.
		/// </param>
		/// <param name="targetSum">
		/// The target sum.
		/// </param>
		/// <param name="totalNumbers">
		/// The total number of randomly generated numbers to create.
		/// </param>
		/// <returns>
		/// The numbers.
		/// </returns>
		public static IEnumerable<int> GetRandomNumbers(int min, int max, int targetSum, int totalNumbers)
		{
			var ret = new List<int>(totalNumbers);
			var random = new Random();
			var remainingSum = targetSum;
			for (var i = 1; i <= totalNumbers; i++)
			{
				var localMax = remainingSum - ((totalNumbers - i) * min);
				if (localMax > max)
				{
					localMax = max;
				}

				var localMin = remainingSum - ((totalNumbers - i) * max);
				if (localMin > min || localMin < 0)
				{
					localMin = min;
				}

				if (i + 1 <= totalNumbers)
				{
					var nextDigit = random.Next(localMin, localMax);
					ret.Add(nextDigit);
					remainingSum -= nextDigit;
				}
				else
				{
					ret.Add(remainingSum);
				}
			}

			return ret;
		}
	}
}