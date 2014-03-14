using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Clutch.Runtime
{
	public static class Bootstrap
	{
		static Bootstrap()
		{
			initialized = false;
			rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
			assemblyInformation = Enumerable.Empty<BootstrapAssemblyInformation>();
		}

		private static bool initialized;
		private static ReaderWriterLockSlim rwLock;
		private static IEnumerable<BootstrapAssemblyInformation> assemblyInformation;

		#region Assembly scanning

		private static string GetPublicKeyToken(AssemblyName assemblyName)
		{
			var publicKeyToken = assemblyName.GetPublicKeyToken();

			if (publicKeyToken.Length <= 0)
				return null;

			var publicKeyTokenString = new StringBuilder();

			foreach (var b in publicKeyToken)
				publicKeyTokenString.AppendFormat("{0:x2}", b);

			return publicKeyTokenString.ToString();
		}

		private static bool ShouldScanAssemblyName(AssemblyName assemblyName, Assembly assembly = null)
		{
			// check assembly name
			if (!AllowedAssemblyNames.Any(n => n.Equals(assemblyName.FullName, StringComparison.OrdinalIgnoreCase)) && !AllowedAssemblyPublicKeys.Contains(GetPublicKeyToken(assemblyName)))
				return false;

			// check assembly
			if (assembly == null)
				return true;

			if (assembly.IsDynamic || assembly.GetCustomAttributes(typeof(GeneratedCodeAttribute), false).Cast<GeneratedCodeAttribute>().Any(a => IgnoredCodeGeneratorTools.Contains(a.Tool)))
				return false;

			return true;
		}

		private static void ScanAssembly(Assembly assembly)
		{
			// check whether we should scan this assembly
			if (!ShouldScanAssemblyName(assembly.GetName(), assembly))
				return;

			var references = assembly.GetReferencedAssemblies();

			// find all bootstrap subscribers
			var subscriberType = typeof(BootstrapSubscriber);
			var subscribers = new List<BootstrapSubscriber>();

			foreach (var type in assembly.GetTypes().Where(t => !t.IsAbstract && subscriberType.IsAssignableFrom(t)))
			{
				subscribers.Add((BootstrapSubscriber)Activator.CreateInstance(type));
			}

			// register all boostrap subscribers and sort them
			if (subscribers.Any())
			{
				var information = new BootstrapAssemblyInformation()
				{
					Assembly = assembly,
					AssemblyName = assembly.GetName(),
					References = references,
					Subscribers = subscribers
				};

				var oldCollection = assemblyInformation.Concat(new[] { information });
				var newCollection = new List<BootstrapAssemblyInformation>();

				while (oldCollection.Any())
				{
					var topLevel = oldCollection.Where(a => !oldCollection.Any(o => a.References.Any(r => r == o.AssemblyName))).ToArray();
					newCollection.AddRange(topLevel);
					oldCollection = oldCollection.Except(topLevel).ToArray();
				}

				assemblyInformation = newCollection;
			}

			// force-load all referenced assemblies that are allowed to be scanned
			foreach (var reference in references.Where(r => ShouldScanAssemblyName(r)))
				Assembly.Load(reference);
		}

		private static void ScanAssemblies()
		{
			AppDomain.CurrentDomain.AssemblyLoad += (sender, args) =>
			{
				rwLock.EnterWriteLock();
				try
				{
					ScanAssembly(args.LoadedAssembly);
				}
				finally
				{
					rwLock.ExitWriteLock();
				}
			};

			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in assemblies)
			{
				ScanAssembly(assembly);
			}
		}

		#endregion

		#region Public interface

		public static void Startup()
		{
			rwLock.EnterWriteLock();
			try
			{
				if (initialized)
					throw new InvalidOperationException("Bootstrap is already initialized");

				ScanAssemblies();

				initialized = true;

				foreach (var info in assemblyInformation.SelectMany(i => i.Subscribers))
					info.Startup();
			}
			finally
			{
				rwLock.ExitWriteLock();
			}
		}

		public static IDisposable OpenScope()
		{
			rwLock.EnterReadLock();
			try
			{
				if (!initialized)
					throw new InvalidOperationException("Bootstrap wasn't initialized");

				var result = new ExecutionScope();
				try
				{
					foreach (var info in assemblyInformation.SelectMany(i => i.Subscribers))
						info.OpenScope();

					return result;
				}
				catch
				{
					result.Dispose();
					throw;
				}
			}
			finally
			{
				rwLock.ExitReadLock();
			}
		}

		public static void CloseScope()
		{
			rwLock.EnterReadLock();
			try
			{
				if (!initialized)
					throw new InvalidOperationException("Bootstrap wasn't initialized");

				var scope = ExecutionScope.CurrentScope as ExecutionScope;
				if (scope == null)
					throw new InvalidOperationException("Attempting to dispose nonexistant scope");

				foreach (var info in assemblyInformation.SelectMany(i => i.Subscribers))
					info.CloseScope();

				scope.Dispose();
			}
			finally
			{
				rwLock.ExitReadLock();
			}
		}

		public static void Shutdown()
		{
			rwLock.EnterWriteLock();
			try
			{
				if (!initialized)
					throw new InvalidOperationException("Bootstrap wasn't initialized");

				foreach (var info in assemblyInformation.SelectMany(i => i.Subscribers))
					info.Shutdown();
			}
			finally
			{
				rwLock.ExitWriteLock();
			}
		}

		#endregion

		#region Configuration

		public static readonly IList<string> AllowedAssemblyNames = new List<string>() { };
		public static readonly IList<string> AllowedAssemblyPublicKeys = new List<string>() { null, "3f1ee76c0d32d23b" };
		public static readonly IList<string> IgnoredCodeGeneratorTools = new List<string> { "ASP.NET" };

		#endregion

		#region Nested type: BootstrapAssemblyInformation

		private class BootstrapAssemblyInformation
		{
			public Assembly Assembly { get; set; }
			public AssemblyName AssemblyName { get; set; }
			public IEnumerable<AssemblyName> References { get; set; }
			public IEnumerable<BootstrapSubscriber> Subscribers { get; set; }
		}

		#endregion
	}

	public class AssemblyReference
	{
		public AssemblyReference Register<T>()
		{
			return this;
		}
	}
}
