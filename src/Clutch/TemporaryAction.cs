using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clutch
{
	/// <summary>
	/// Generic temporary action.
	/// </summary>
	public class TemporaryAction : TemporaryActionBase
	{
		public TemporaryAction(Action doAction, Action undoAction)
		{
			if (doAction == null)
				throw new ArgumentNullException("doAction");
			if (undoAction == null)
				throw new ArgumentNullException("undoAction");

			this.undoAction = undoAction;

			doAction();
		}

		private Action undoAction;

		/// <summary>
		/// Reverts executed action.
		/// </summary>
		protected override void Revert()
		{
			undoAction();
		}
	}
}
