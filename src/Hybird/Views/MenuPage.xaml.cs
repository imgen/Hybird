using Hybird.ViewModels;
using System.Collections.Generic;

namespace Hybird.Views
{
	public enum MenuType
	{
		Home,
		About
	}

	public partial class MenuPage
	{
		public class HomeMenuItemViewModel : BaseViewModel
		{
			public HomeMenuItemViewModel()
			{
				MenuType = MenuType.About;
			}
			public MenuType MenuType { get; set; }

			bool _isSelected;
			public bool IsSelected
			{
				get { return _isSelected; }
				set { SetProperty(ref _isSelected, value); }
			}
		}

		RootPage root;
		List<HomeMenuItemViewModel> menuItems;
		public MenuPage(RootPage root)
		{
			InitializeComponent();
			this.root = root;

			BindingContext = new BaseViewModel
			{
				Title = "Hybird Sample",
				Subtitle = "Hybird Sample",
				Icon = "slideout.png"
			};

			menuListView.ItemsSource = menuItems = new List<HomeMenuItemViewModel>
				{
					new HomeMenuItemViewModel
					{
						Title = "Home",
						MenuType = MenuType.Home,
						Icon ="article.png",
						IsSelected = false
					},
					new HomeMenuItemViewModel
					{
						Title = "About",
						MenuType = MenuType.About,
						Icon ="about.png",
						IsSelected = false
					}
				};

			menuListView.ItemSelected += async (sender, e) =>
				{
					if (e.SelectedItem == null)
						return;
					menuListView.SelectedItem = null;

					var menuItem = (HomeMenuItemViewModel)(e.SelectedItem);
					//menuItems.ForEach(item => item.IsSelected = item.MenuType == menuItem.MenuType);
					switch (menuItem.MenuType)
					{
						case MenuType.Home:
							await root.NavigateAsync(menuItem.MenuType);
							break;

						case MenuType.About:
							root.IsPresented = false;
							var homePage = (root.Detail as HybirdNavigationPage).Page;
							await homePage.Navigation.PushAsync(new AboutPage());
							break;
					}
					
				};
		}
	}
}

