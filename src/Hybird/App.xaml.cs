using System;
using System.Threading.Tasks;
using Hybird.Views;
using Xamarin.Forms;

namespace Hybird
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = new RootPage();
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}

		bool _quitTimerOn;
		public bool CanGoBack
		{
			get
			{
				var mainPage = MainPage as MasterDetailPage;

				if (mainPage != null)
				{
					var navigation = mainPage.Detail.Navigation;

					if (navigation.NavigationStack.Count == 1)
					{
						if (_quitTimerOn)
						{
							return true;
						}
						Statics.ShowToast("请再次按返回键退出");
						_quitTimerOn = true;
						Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(_ => _quitTimerOn = false);
						return false;
					}

					return true;
				}
				return true;
			}
		}

		public bool IsRootPage
		{
			get
			{
				var mainPage = MainPage as MasterDetailPage;

				if (mainPage != null)
				{
					var navigation = mainPage.Detail.Navigation;
					return navigation.NavigationStack.Count == 1;
				}
				return false;
			}
		}
	}
}
