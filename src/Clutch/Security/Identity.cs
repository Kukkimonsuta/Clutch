
namespace Clutch.Security
{
	/// <summary>
	/// Base identity.
	/// </summary>
	public abstract class Identity : System.Security.Principal.IIdentity
	{
		public Identity()
		{
		}

		public abstract string Name { get; }
		public abstract bool IsAuthenticated { get; }
		public abstract Role Role { get; }

		#region IIdentity

		string System.Security.Principal.IIdentity.AuthenticationType
		{
			get { return "Clutch.Security"; }
		}

		bool System.Security.Principal.IIdentity.IsAuthenticated
		{
			get { return IsAuthenticated; }
		}

		string System.Security.Principal.IIdentity.Name
		{
			get { return Name; }
		}

		#endregion
	}
}
