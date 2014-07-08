using System.Collections.ObjectModel;
using System.Linq;

namespace Clutch.Security
{
	/// <summary>
	/// Class representing role of a user.
	/// </summary>
	public sealed class Role
	{
		public Role(string name, string displayName, params Permission[] permissions)
		{
			Name = name;
			DisplayName = displayName;

			Permissions = new ReadOnlyCollection<Permission>(permissions.ToList());
		}

		/// <summary>
		/// Unique role name.
		/// </summary>
		public string Name { get; private set; }
		/// <summary>
		/// Display name.
		/// </summary>
		public string DisplayName { get; private set; }
		/// <summary>
		/// List of granted permissions.
		/// </summary>
		public ReadOnlyCollection<Permission> Permissions { get; private set; }

		/// <summary>
		/// Returns whether role has given permission.
		/// </summary>
		public bool Has(string permissionName)
		{
			return Permissions.Any(p => p.Name == permissionName);
		}
		/// <summary>
		/// Returns whether role has given permission.
		/// </summary>
		public bool Has(Permission permission)
		{
			return Permissions.Any(p => p == permission);
		}
	}
}
