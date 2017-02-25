using System.Collections.Generic;
using Xamarin.Forms;

namespace Hybird.Controls
{
	public class ExtendedToolbarItem : ToolbarItem
	{
		public IList<ToolbarItem> ToolbarItems { get; set; }
		FileImageSource _enableIcon, _disableIcon;

		public ExtendedToolbarItem(IList<ToolbarItem> toolbarItems,
								   FileImageSource enableIcon = null,
								   FileImageSource disableIcon = null,
								   bool enabled = true,
								   bool visible = true)
		{
			ToolbarItems = toolbarItems;
			_enableIcon = enableIcon;
			_disableIcon = disableIcon;
			Icon = enabled ? _enableIcon : _disableIcon;
			IsVisible = visible;
		}

		public int Index { get; set; } = -1;

		public bool IsVisible
		{
			get
			{
				return ToolbarItems.Contains(this);
			}
			set
			{
				if (IsVisible == value)
				{
					return;
				}

				if (value)
				{
					if (Index >= 0 && Index < ToolbarItems.Count)
					{
						ToolbarItems.Insert(Index, this);
					}
					else
					{
						var index = 0;
						foreach (ExtendedToolbarItem item in ToolbarItems)
						{
							index++;
							if (item.Index > Index)
							{
								ToolbarItems.Insert(index, this);
								return;
							}
						}
						ToolbarItems.Add(this);
					}
				}
				else
				{
					ToolbarItems.Remove(this);
				}
			}
		}

		public bool IsEnabled
		{
			get
			{
				return Command.CanExecute(null);
			}
			set
			{
				if (Device.OS == TargetPlatform.iOS || _enableIcon == null || _disableIcon == null)
				{
					return;
				}

				Icon = value ? _enableIcon : _disableIcon;
			}
		}
	}
}
