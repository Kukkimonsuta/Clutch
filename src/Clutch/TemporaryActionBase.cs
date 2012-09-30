using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clutch
{
	/// <summary>
	/// Base class for temporary actions.
	/// </summary>
	public abstract class TemporaryActionBase : IDisposable
	{
		public TemporaryActionBase()
		{
			this.applied = true;
		}

		private bool applied;

		/// <summary>
		/// Reverts executed action.
		/// </summary>
		protected abstract void Revert();

		/// <summary>
		/// Disposes object reverting executed action if required.
		/// </summary>
		public void Dispose()
		{
			if (applied)
				Revert();

			applied = false;
		}
	}
}
