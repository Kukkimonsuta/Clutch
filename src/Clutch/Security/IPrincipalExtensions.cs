using System;
using System.Security;
using System.Security.Principal;

namespace Clutch.Security
{
	public static class IPrincipalExtensions
	{
		/// <summary>
		/// Returns Identity of given principal
		/// </summary>
		public static Identity GetIdentity(this IPrincipal principal)
		{
			if (principal == null)
				throw new ArgumentNullException("principal");

			return (Identity)principal.Identity;
		}

		/// <summary>
		/// Returns Identity of given principal
		/// </summary>
		public static T GetIdentity<T>(this IPrincipal principal)
			where T : Identity
		{
			if (principal == null)
				throw new ArgumentNullException("principal");

			return (T)principal.Identity;
		}

		/// <summary>
		/// Returns Role of given principal
		/// </summary>
		public static Role GetRole(this IPrincipal principal)
		{
			if (principal == null)
				throw new ArgumentNullException("principal");

			return principal.GetIdentity().Role;
		}

		/// <summary>
		/// Returns whether given principal has permission with given name
		/// </summary>
		public static bool HasPermission(this IPrincipal principal, string permissionName)
		{
			if (principal == null)
				throw new ArgumentNullException("principal");
			if (permissionName == null)
				throw new ArgumentNullException("permissionName");

			return principal.GetRole().Has(permissionName);
		}

		/// <summary>
		/// Returns whether given principal has given permission
		/// </summary>
		public static bool HasPermission(this IPrincipal principal, Permission permission)
		{
			if (principal == null)
				throw new ArgumentNullException("principal");
			if (permission == null)
				throw new ArgumentNullException("permission");

			return principal.GetRole().Has(permission);
		}

		/// <summary>
		/// Throws SecurityException if given principal doesn't have permission with given name
		/// </summary>
		public static void DemandPermission(this IPrincipal principal, string permissionName)
		{
			if (principal == null)
				throw new ArgumentNullException("principal");
			if (permissionName == null)
				throw new ArgumentNullException("permissionName");

			if (!principal.HasPermission(permissionName))
				throw new SecurityException("Principal '" + principal.Identity.Name + "' doesn't have permission with name '" + permissionName + "'");
		}

		/// <summary>
		/// Throws SecurityException if given principal doesn't have given permission
		/// </summary>
		public static void DemandPermission(this IPrincipal principal, Permission permission)
		{
			if (principal == null)
				throw new ArgumentNullException("principal");
			if (permission == null)
				throw new ArgumentNullException("permission");

			if (!principal.HasPermission(permission))
				throw new SecurityException("Principal '" + principal.Identity.Name + "' doesn't have permission '" + permission.Name + "'");
		}
	}
}
