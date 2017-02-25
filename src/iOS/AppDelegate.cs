using System;

using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using UIKit;
using static UIKit.UINavigationBar;
using Xamarin.Forms;
using XLabs.Forms;
using Refractored.XamForms.PullToRefresh.iOS;

namespace Hybird.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : XFormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			Appearance.BarTintColor = UIColor.FromRGB(43, 132, 211); //bar background
			Appearance.TintColor = UIColor.White; //Tint color of button items
			Appearance.SetTitleTextAttributes(new UITextAttributes()
			{
				Font = UIFont.FromName("HelveticaNeue-Light", (nfloat)20f),
				TextColor = UIColor.White
			});
			Forms.Init();
			ZXing.Net.Mobile.Forms.iOS.Platform.Init();
			ImageCircleRenderer.Init();
			PullToRefreshLayoutRenderer.Init();
			LoadApplication(new App());

			return base.FinishedLaunching(app, options);
		}
	}
}
