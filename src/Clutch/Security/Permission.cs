using System;
using System.Threading;

namespace Clutch.Security
{
	/// <summary>
	/// Class representing a permission for given action.
	/// </summary>
	public sealed class Permission
	{
		public Permission(string name, string displayName)
		{
			Name = name;
			DisplayName = displayName;
		}

		/// <summary>
		/// Unique permission name.
		/// </summary>
		public string Name { get; private set; }
		/// <summary>
		/// Display name.
		/// </summary>
		public string DisplayName { get; private set; }

		/// <summary>
		/// Returns whether current principal has granted this permission.
		/// </summary>
		public bool IsGranted()
		{
			return Thread.CurrentPrincipal.HasPermission(this);
		}

		/// <summary>
		/// Ensures that current principal has granted this permission.
		/// </summary>
		public void Demand()
		{
			Thread.CurrentPrincipal.DemandPermission(this);
		}
	}
}
