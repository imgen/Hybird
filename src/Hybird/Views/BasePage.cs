#if __ANDROID__
using Android.App;
using Android.Views;
#endif
using Hybird.ViewModels;
using Xamarin.Forms;
using System.Linq;

namespace Hybird.Views
{
	public class BasePage : ContentPage
	{
		public bool IsAndroid => Device.OS == TargetPlatform.Android;
		public bool IsIOS => Device.OS == TargetPlatform.iOS;

		public bool IsTopPage => Navigation.NavigationStack.Count == 1;

		public bool IsCurrentPage => Navigation.NavigationStack.LastOrDefault() == this || Navigation.ModalStack.LastOrDefault() == this;

#if __ANDROID__
		public Activity CurrentActivity => Statics.CurrentActivity;

		public ViewGroup RootView => Statics.RootView;
#endif

		public virtual void OnLoad()
		{
		}

		public BasePage()
		{
			SetBinding(Page.TitleProperty, new Binding(BaseViewModel.TitlePropertyName));
			SetBinding(Page.IconProperty, new Binding(BaseViewModel.IconPropertyName));
		}
	}
}

