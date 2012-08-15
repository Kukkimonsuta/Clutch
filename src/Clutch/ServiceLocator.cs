using System;
using System.Collections.Generic;
using System.Linq;

namespace Clutch
{
    public static class ServiceLocator
    {
        static ServiceLocator()
        {
            getInstanceProviders = new List<Func<Type, object>>();
            getInstancesProviders = new List<Func<Type, IEnumerable<object>>>();
        }

        private static List<Func<Type, object>> getInstanceProviders;
        private static List<Func<Type, IEnumerable<object>>> getInstancesProviders;

        public static object GetInstance(Type type, bool required = true)
        {
            if (required)
                return getInstanceProviders.Select(p => p(type)).Where(r => r != null).FirstOrDefault();
            else
                return getInstancesProviders.SelectMany(p => p(type)).FirstOrDefault();
        }

        public static T GetInstance<T>(bool required = true)
        {
            return (T)GetInstance(typeof(T), required: required);
        }

        public static IEnumerable<object> GetInstances(Type type)
        {
            return getInstancesProviders.SelectMany(p => p(type)).ToArray();
        }

        public static IEnumerable<T> GetInstances<T>()
        {
            return (IEnumerable<T>)GetInstances(typeof(T));
        }

        public static void RegisterProvider(Func<Type, object> getServiceProvider, Func<Type, IEnumerable<object>> getServicesProvider)
        {
            if (getServiceProvider != null)
                getInstanceProviders.Add(getServiceProvider);

            if (getServicesProvider != null)
                getInstancesProviders.Add(getServicesProvider);
        }
    }
}
