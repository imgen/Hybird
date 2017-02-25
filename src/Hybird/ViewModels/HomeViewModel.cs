using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace Hybird.ViewModels
{
	public class HomeViewModel: BaseViewModel
	{
		Command _refreshPageCommand;
		public Command RefreshPageCommand => _refreshPageCommand ??
		(_refreshPageCommand = new Command(async () => await RefreshPage(true, true)));


		HybridWebView WebView { get; }

		public HomeViewModel(HybridWebView webView)
		{
			Title = Statics.AppName;
			WebView = webView;
		}

		public async Task RefreshPage(bool delay = false, bool showHud = false)
		{
			if (showHud)
			{
				Statics.ShowHUD("正在刷新");
			}
			IsBusy = true;
			var html = await Task.Run(() => ResourceLoader.Load("hybrid_test.html"));
			WebView.LoadContent(html);
			if (delay)
			{
				await Task.Delay(TimeSpan.FromSeconds(1));
			}
			IsBusy = false;
			if (showHud)
			{
				Statics.ShowSuccessHUDWithStatus("刷新完成");
			}
		}
	}
}
