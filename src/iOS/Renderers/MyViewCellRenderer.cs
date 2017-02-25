using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Hybird.iOS.Renderers;
using Hybird.Controls;

[assembly: ExportRenderer(typeof(MyViewCell), typeof(MyViewCellRenderer))]
namespace Hybird.iOS.Renderers
{
	public class MyViewCellRenderer : ViewCellRenderer
	{
		public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			var cell = base.GetCell(item, reusableCell, tv);
			cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			return cell;
		}
	}
}
