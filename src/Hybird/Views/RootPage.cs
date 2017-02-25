using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.Generic;
using Hybird.ViewModels;

namespace Hybird.Views
{
	public class RootPage : MasterDetailPage
	{
		public static bool IsUwpDesktop { get; set; }
		Dictionary<MenuType, NavigationPage> Pages { get; set; }

		public RootPage()
		{
			Pages = new Dictionary<MenuType, NavigationPage>();
			Master = new MenuPage(this);
			BindingContext = new BaseViewModel
			{
				Title = "Hybird Sample",
				Subtitle = "Hybird Sample",
				Icon = "slideout.png"
			};

			//setup home page
			NavigateAsync(MenuType.Home);

			InvalidateMeasure();
		}

		T AddPage<T>(MenuType id, T page)
			where T : ContentPage
		{
			Pages.Add(id, new HybirdNavigationPage(page));
			return page;
		}

		public async Task NavigateAsync(MenuType id)
		{
			if (!Pages.ContainsKey(id))
			{
				switch (id)
				{
					case MenuType.Home:
						AddPage(id, new HomePage());
						break;
				}
			}

			var newPage = Pages[id] as HybirdNavigationPage;
			if (newPage == Detail)
			{
				IsPresented = false;
				return;
			}
			if (newPage == null)
				return;

			//pop to root for Windows Phone
			if (Detail != null && Device.OS == TargetPlatform.WinPhone)
			{
				await Detail.Navigation.PopToRootAsync();
			}

			Detail = newPage;
			var basePage = newPage.Page as BasePage;
			if (basePage != null)
			{
				basePage.OnLoad();
			}

			if (!IsUwpDesktop)
			{
				IsPresented = false;
			}
		}

	}
}

