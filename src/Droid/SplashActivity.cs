using Android.App;
using Android.Support.V7.App;

namespace Hybird.Droid
{
	[Activity(
		Label="Hybird Sample",
		Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
	public class SplashActivity : AppCompatActivity
	{
		protected override void OnResume()
		{
			base.OnResume();

			StartActivity(typeof(MainActivity));
		}
	}
}