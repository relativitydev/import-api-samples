// ----------------------------------------------------------------------------
// <copyright file="AssemblySetup.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Samples.NUnit
{
	using Relativity.DataExchange.TestFramework;

	using global::NUnit.Framework;

	/// <summary>
	/// Represents a global assembly-wide setup routine that's guaranteed to be executed before ANY NUnit test.
	/// </summary>
	[SetUpFixture]
	public class AssemblySetup
	{
		/// <summary>
		/// Gets the test parameters used by all integration tests within the current assembly.
		/// </summary>
		/// <value>
		/// The <see cref="IntegrationTestParameters"/> instance.
		/// </value>
		public static IntegrationTestParameters TestParameters
		{
			get;
			private set;
		}

		/// <summary>
		/// The main setup method.
		/// </summary>
		[OneTimeSetUp]
		public void Setup()
		{
			TestParameters = IntegrationTestHelper.Create();
		}

		/// <summary>
		/// The main teardown method.
		/// </summary>
		[OneTimeTearDown]
		public void TearDown()
		{
			IntegrationTestHelper.Destroy(TestParameters);
		}
	}
}