// ----------------------------------------------------------------------------
// <copyright file="ServiceHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.TestFramework
{
	using System;
	using System.Net;

	using Relativity.Services.ServiceProxy;

	/// <summary>
	/// Defines static helper methods to retrieve Relativity services.
	/// </summary>
	public static class ServiceHelper
	{
		/// <summary>
		/// The maximum number of query manager items to fetch.
		/// </summary>
		public const int MaxItemsToFetch = 5000;

		/// <summary>
		/// Retrieve a service proxy.
		/// </summary>
		/// <typeparam name="T">
		/// The type of service to retrieve.
		/// </typeparam>
		/// <param name="parameters">
		/// The integration test parameters.
		/// </param>
		/// <returns>
		/// The <typeparamref name="T"/> instance.
		/// </returns>
		public static T GetServiceProxy<T>(IntegrationTestParameters parameters)
			where T : class, IDisposable
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls
			                                                                            | SecurityProtocolType.Tls11
			                                                                            | SecurityProtocolType.Tls12;
			Credentials credentials = new UsernamePasswordCredentials(parameters.RelativityUserName, parameters.RelativityPassword);
			ServiceFactorySettings serviceFactorySettings = new ServiceFactorySettings(
				                                                parameters.RelativityServicesUrl,
				                                                parameters.RelativityRestUrl,
				                                                credentials)
				                                                {
					                                                ProtocolVersion = Relativity.Services.Pipeline
						                                                .WireProtocolVersion.V2
				                                                };
			Relativity.Services.ServiceProxy.ServiceFactory serviceFactory =
				new Relativity.Services.ServiceProxy.ServiceFactory(serviceFactorySettings);
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls
			                                                                            | SecurityProtocolType.Tls11
			                                                                            | SecurityProtocolType.Tls12;
			T proxy = serviceFactory.CreateProxy<T>();
			return proxy;
		}
	}
}