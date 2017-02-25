using System;
using MaterialControls;

namespace Hybird.iOS
{
	public static class ViewExtensions
	{
		public static MDSnackbar AddActionTouchedHandler(this MDSnackbar snackBar, Action action)
		{
			snackBar.AddDelegate(new SnackBarActionDelegate(action));
			return snackBar;
		}

		public static MDSnackbar AddActionTouchedHandler(this MDSnackbar snackBar, Action<MDSnackbar> action)
		{
			snackBar.AddActionTouchedHandler(() => action(snackBar));
			return snackBar;
		}
	}

	class SnackBarActionDelegate : MDSnackbarDelegate
	{
		readonly Action _action;

		public SnackBarActionDelegate(Action action)
		{
			_action = action;
		}

		public override void ActionTouched(MDSnackbar snackbar)
		{
			_action?.Invoke();
		}
	}
}
