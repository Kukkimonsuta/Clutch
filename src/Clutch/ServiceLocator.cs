using System;
using System.Collections.Generic;
using System.Linq;

namespace Clutch
{
	/// <summary>
	/// Service locator.
	/// </summary>
    public static class ServiceLocator
    {
        static ServiceLocator()
        {
            getInstanceProviders = new List<Func<Type, object>>();
            getInstancesProviders = new List<Func<Type, IEnumerable<object>>>();
        }

        private static List<Func<Type, object>> getInstanceProviders;
        private static List<Func<Type, IEnumerable<object>>> getInstancesProviders;

		/// <summary>
		/// Gets single instance of requested service.
		/// </summary>
		/// <param name="type">Requested service type.</param>
		/// <param name="required">If true, exception will be thrown when instance is not available.</param>
		/// <returns>Requested service instance.</returns>
        public static object GetInstance(Type type, bool required = true)
        {
			object service;

            if (required)
				service = getInstanceProviders.Select(p => p(type)).Where(r => r != null).FirstOrDefault();
            else
				service = getInstancesProviders.SelectMany(p => p(type)).Where(r => r != null).FirstOrDefault();

			if (required && service == null)
				throw new InvalidOperationException(string.Format("Service '{0}' not found", type.FullName));

			return service;
        }

		/// <summary>
		/// Gets single instance of requested service.
		/// </summary>
		/// <typeparam name="TService">Requested service type.</typeparam>
		/// <param name="required">If true, exception will be thrown when instance is not available.</param>
		/// <returns>Requested service instance.</returns>
        public static TService GetInstance<TService>(bool required = true)
        {
            return (TService)GetInstance(typeof(TService), required: required);
        }

		/// <summary>
		/// Gets all instances of requested service.
		/// </summary>
		/// <param name="type">Requested service type.</param>
		/// <returns>Requested service instances.</returns>
        public static IEnumerable<object> GetInstances(Type type)
        {
            return getInstancesProviders.SelectMany(p => p(type)).ToArray();
        }

		/// <summary>
		/// Gets all instances of requested service.
		/// </summary>
		/// <typeparam name="TService">Requested service type.</typeparam>
		/// <returns>Requested service instances.</returns>
        public static IEnumerable<TService> GetInstances<TService>()
        {
            return (IEnumerable<TService>)GetInstances(typeof(TService));
        }

		/// <summary>
		/// Registers provider for single or multiple instances.
		/// </summary>
		/// <param name="getServiceProvider">New provider for single instance service resolution.</param>
		/// <param name="getServicesProvider">New provider for multiple instances service resolution.</param>
        public static void RegisterProvider(Func<Type, object> getServiceProvider, Func<Type, IEnumerable<object>> getServicesProvider)
        {
            if (getServiceProvider != null)
                getInstanceProviders.Add(getServiceProvider);

            if (getServicesProvider != null)
                getInstancesProviders.Add(getServicesProvider);
        }
    }
}
