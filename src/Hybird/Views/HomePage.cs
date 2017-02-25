using System;
using System.Threading.Tasks;
using Hybird.Controls;
using Hybird.ViewModels;
using Refractored.XamForms.PullToRefresh;
using Xamarin.Forms;

namespace Hybird.Views
{
	public class HomePage : HybridWebViewPage
	{
		public HomeViewModel ViewModel => BindingContext as HomeViewModel;

		public HomePage()
		{
			BindingContext = new HomeViewModel(WebView);
			var pullRefreshLayout = new PullToRefreshLayout
			{
				Content = WebView
			};
			pullRefreshLayout.SetBinding<HomeViewModel>(
				PullToRefreshLayout.IsRefreshingProperty, vm => vm.IsBusy);
			pullRefreshLayout.SetBinding<HomeViewModel>(
				PullToRefreshLayout.RefreshCommandProperty, vm => vm.RefreshPageCommand);
			
			Content = pullRefreshLayout;

			new ExtendedToolbarItem(ToolbarItems)
			{
				Icon = "refresh.png",
				Text = "Refresh",
				Command = new Command(async () => await ViewModel.RefreshPage(false, true))
			};
		}

		bool _loaded = false;
		public async override void OnLoad()
		{
			base.OnLoad();
#if __ANDROID__
			await Task.Delay(TimeSpan.FromSeconds(0.1));
#endif
			if (!_loaded)
			{
				ViewModel.RefreshPage();
				_loaded = true;
			}
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			Console.WriteLine("Disappearing");
		}

		protected async override void OnWebViewLoadFinished()
		{
			base.OnWebViewLoadFinished();
			await Task.Delay(TimeSpan.FromSeconds(2));
			InvokeNativeToJsCommand(NativeToJsCommandId.ShowToast,
									"Hello from Native Xamarin Code to JS", result =>
									{
										Console.WriteLine("The result is a " + result.Status);
									});
		}
	}
}

