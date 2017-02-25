using System.Collections.Generic;
using System;
#if __ANDROID__
using Android.Support.Design.Widget;
using Android.Widget;
using Android.Views;
#elif __IOS__
using MaterialControls;
using Hybird.iOS;
#endif
namespace Hybird
{
	public class SnackBarUtils
	{
#if __ANDROID__

		readonly List<Snackbar> _snackBars = new List<Snackbar>();

		View _view;

		public SnackBarUtils(View view)
		{
			_view = view;
		}

		Snackbar ShowSnackBar(string message, string actionText, Action action, int duration = 4000)
		{
			var snackBar = Snackbar.Make(_view, message, duration).SetAction(
				actionText, view => action());
			snackBar.Show();
			_snackBars.Add(snackBar);
			return snackBar;
		}

#elif __IOS__

	    readonly List<MDSnackbar> _snackBars = new List<MDSnackbar>();

		MDSnackbar ShowSnackBar(string message, string actionText, Action action, int duration = 4000)
		{
			var snackBar = new MDSnackbar(message, actionText, duration / 1000.0);
			snackBar.AddActionTouchedHandler(action);
			snackBar.Show();
			_snackBars.Add(snackBar);
			return snackBar;
		}

#endif

		void DismissSnackBars()
		{
			_snackBars.ForEach(snackBar => { snackBar.Dismiss(); snackBar.Dispose(); });
			_snackBars.Clear();
		}
	}
}
