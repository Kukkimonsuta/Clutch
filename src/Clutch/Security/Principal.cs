using System;

namespace Clutch.Security
{
	/// <summary>
	/// Principal used by Clutch.Security.
	/// </summary>
	public sealed class Principal : System.Security.Principal.IPrincipal
	{
		public Principal(Identity identity)
		{
			if (identity == null)
				throw new ArgumentNullException("identity");

			m_identity = identity;
		}

		private Identity m_identity;

		#region IPrincipal

		System.Security.Principal.IIdentity System.Security.Principal.IPrincipal.Identity
		{
			get { return m_identity; }
		}

		bool System.Security.Principal.IPrincipal.IsInRole(string role)
		{
			return m_identity.Role.Name == role;
		}

		#endregion
	}
}
