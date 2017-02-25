using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using ImageCircle.Forms.Plugin.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XLabs.Forms;
using ZXing.Net.Mobile.Forms.Android;
using Java.Lang;
using Refractored.XamForms.PullToRefresh.Droid;
using Plugin.Permissions;

namespace Hybird.Droid
{
	[Activity(Label = "Hybird Sample", 
	          Icon = "@drawable/hybird", 
	          Theme = "@style/MyTheme", 
	          MainLauncher = false,
	          LaunchMode = LaunchMode.SingleTop,
	          ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			XFormsApplicationDroid forms = null;
			TabLayoutResource = Resource.Layout.tabs;
			ToolbarResource = Resource.Layout.toolbar;

			base.OnCreate(bundle);

			Forms.Init(this, bundle);
			ImageCircleRenderer.Init();
			ZXing.Net.Mobile.Forms.Android.Platform.Init();
			PullToRefreshLayoutRenderer.Init();
			LoadApplication(new App());
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}

		public override void OnBackPressed()
		{
			var app = (App)Xamarin.Forms.Application.Current;
			if (app.CanGoBack)
			{
				base.OnBackPressed();
				if (app.IsRootPage)
				{
					JavaSystem.Exit(0);
				}
			}
		}

		//
		// Properties
		//
		public EventHandler<EventArgs> Destroy
		{
			get;
			set;
		}

		public EventHandler<EventArgs> Pause
		{
			get;
			set;
		}

		public EventHandler<EventArgs> Restart
		{
			get;
			set;
		}

		public EventHandler<EventArgs> Resume
		{
			get;
			set;
		}

		public EventHandler<EventArgs> Start
		{
			get;
			set;
		}

		public EventHandler<EventArgs> Stop
		{
			get;
			set;
		}

		//
		// Methods
		//
		protected override void OnDestroy()
		{
			EventHandler<EventArgs> destroy = this.Destroy;
			if (destroy != null)
			{
				destroy(this, new EventArgs());
			}
			base.OnDestroy();
		}

		protected override void OnPause()
		{
			EventHandler<EventArgs> pause = this.Pause;
			if (pause != null)
			{
				pause(this, new EventArgs());
			}
			base.OnPause();
		}

		protected override void OnRestart()
		{
			EventHandler<EventArgs> restart = this.Restart;
			if (restart != null)
			{
				restart(this, new EventArgs());
			}
			base.OnRestart();
		}

		protected override void OnResume()
		{
			EventHandler<EventArgs> resume = this.Resume;
			if (resume != null)
			{
				resume(this, new EventArgs());
			}
			base.OnResume();
		}

		protected override void OnStart()
		{
			EventHandler<EventArgs> start = this.Start;
			if (start != null)
			{
				start(this, new EventArgs());
			}
			base.OnStart();
		}

		protected override void OnStop()
		{
			EventHandler<EventArgs> stop = this.Stop;
			if (stop != null)
			{
				stop(this, new EventArgs());
			}
			base.OnStop();
		}
	}
}
