using Xamarin.Forms;

namespace Hybird.Views
{
	public class HybirdNavigationPage : NavigationPage
	{
		public Page Page { get; private set; }

		public HybirdNavigationPage(Page root) : base(root)
		{
			Page = root;
			Init();
		}

		public HybirdNavigationPage()
		{
			Init();
		}

		void Init()
		{
			BarBackgroundColor = Color.FromHex("#03A9F4");
			BarTextColor = Color.White;
		}
	}
}

