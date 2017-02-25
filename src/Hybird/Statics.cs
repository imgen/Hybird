using Xamarin.Forms;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;

#if __ANDROID__
using Android.App;
using Android.Widget;
using Android.Views;
using Plugin.CurrentActivity;
using AndroidHUD;
#elif __IOS__
using MaterialControls;
using BigTed;
using static BigTed.ProgressHUD;
#endif

namespace Hybird
{
	static class Statics
	{
		public const string AppName = "Hybird Sample";

#if __ANDROID__
		public static Activity CurrentActivity => CrossCurrentActivity.Current.Activity;
		public static ViewGroup RootView => (ViewGroup)CurrentActivity.Window.DecorView.RootView;
#endif

		public static void Init()
		{
		}

		public static void SetBackButtonTitle(this BindableObject page, string title = "返回")
		{
			NavigationPage.SetBackButtonTitle(page, title);
		}

		static Color _separatorColor = Color.Default;
		public static Color SeparatorColor =>
			_separatorColor == Color.Default ?
								(_separatorColor = GetResource<Color>("SeparatorColor")) : _separatorColor;

		public static void ShowToast(string message)
		{
#if __ANDROID__
			Toast.MakeText(CurrentActivity, message, ToastLength.Short).Show();
#elif __IOS__
			var toast = new MDToast(message, MDToast.DurationShort);
			toast.Show();
#endif
		}

		public static T GetResource<T>(string index)
		{
			return (T)Xamarin.Forms.Application.Current.Resources[index];
		}

		public static async Task UploadStream(string url, Stream stream, string formName, string fileName = null)
		{
			using (var client = new HttpClient())
			{
				using (var content = new MultipartFormDataContent())
				{
					content.Add(new StreamContent(stream), formName, fileName);
					var response = await client.PostAsync(url, content);
					response.EnsureSuccessStatusCode();
				}
			}
		}

		static readonly TimeSpan HUDTimeout = TimeSpan.FromSeconds(2);

#if __ANDROID__

		public static void ShowHUD(string status)
		{
			AndHUD.Shared.Show(CurrentActivity, status, maskType: MaskType.Black);
		}

		public static void ShowSuccessHUDWithStatus(string status)
		{
			AndHUD.Shared.ShowSuccessWithStatus(CurrentActivity, status, MaskType.Black, HUDTimeout);
		}

		public static void ShowErrorHUDWithStatus(string status)
		{
			AndHUD.Shared.ShowErrorWithStatus(CurrentActivity, status, MaskType.Black, HUDTimeout);
		}

#elif __IOS__

		public static void ShowHUD(string status)
		{
			BTProgressHUD.Show(status, maskType: MaskType.Black);
		}

		public static void ShowSuccessHUDWithStatus(string status)
		{
			BTProgressHUD.ShowSuccessWithStatus(status, HUDTimeout.TotalMilliseconds);
		}

		public static void ShowErrorHUDWithStatus(string status)
		{
			BTProgressHUD.ShowErrorWithStatus(status, HUDTimeout.TotalMilliseconds);
		}

#endif
	}
}
