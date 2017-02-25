using Hybird.Controls;
using Hybird.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using MaterialControls;
using UIKit;
using System;
using System.ComponentModel;
using Foundation;
using CoreGraphics;
using System.Threading.Tasks;

[assembly: ExportRenderer(typeof(MaterialButton), typeof(MaterialButtonRenderer))]
namespace Hybird.iOS.Renderers
{
	class MaterialButtonRenderer : ViewRenderer<MaterialButton, MDButton>
	{
		UIColor _buttonTextColorDefaultDisabled;
		UIColor _buttonTextColorDefaultHighlighted;
		UIColor _buttonTextColorDefaultNormal;
		bool _titleChanged;
		CGSize _titleSize;

		// This looks like it should be a const under iOS Classic,
		// but that doesn't work under iOS 
		// ReSharper disable once BuiltInTypeReferenceStyle
		// Under iOS Classic Resharper wants to suggest this use the built-in type ref
		// but under iOS that suggestion won't work
		readonly nfloat _minimumButtonHeight = 44; // Apple docs

		public override CGSize SizeThatFits(CGSize size)
		{
			var result = base.SizeThatFits(size);

			if (result.Height < _minimumButtonHeight)
			{
				result.Height = _minimumButtonHeight;
			}

			return result;
		}

		protected override void Dispose(bool disposing)
		{
			if (Control != null)
				Control.TouchUpInside -= OnButtonTouchUpInside;

			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<MaterialButton> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var button = new MDButton();
					button.SetTitleColor(UIColor.LightGray, UIControlState.Disabled);
					SetNativeControl(button);

					_buttonTextColorDefaultNormal = Control.TitleColor(UIControlState.Normal);
					_buttonTextColorDefaultHighlighted = Control.TitleColor(UIControlState.Highlighted);
					_buttonTextColorDefaultDisabled = Control.TitleColor(UIControlState.Disabled);

					Control.TouchUpInside += OnButtonTouchUpInside;
				}

				UpdateText();
				UpdateFont();
				UpdateBorder();
				UpdateImage();
				UpdateTextColor();
				UpdateEnableDisable();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Button.TextProperty.PropertyName)
				UpdateText();
			else if (e.PropertyName == Button.TextColorProperty.PropertyName)
				UpdateTextColor();
			else if (e.PropertyName == Button.FontProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == Button.BorderWidthProperty.PropertyName || e.PropertyName == Button.BorderRadiusProperty.PropertyName || e.PropertyName == Button.BorderColorProperty.PropertyName)
				UpdateBorder();
			else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName) { }
			else if (e.PropertyName == Button.ImageProperty.PropertyName)
				UpdateImage();
			else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
			{
				UpdateEnableDisable();
			}
		}

		void UpdateEnableDisable()
		{
			if (Control.Enabled != Element.IsEnabled)
			{
				Control.Enabled = Element.IsEnabled;
			}
		}

		void OnButtonTouchUpInside(object sender, EventArgs eventArgs)
		{
			((IButtonController)Element)?.SendClicked();
		}

		void UpdateBorder()
		{
			var uiButton = Control;
			var button = Element;

			if (button.BorderColor != Color.Default)
				uiButton.Layer.BorderColor = button.BorderColor.ToCGColor();

			uiButton.Layer.BorderWidth = (float)button.BorderWidth;
			uiButton.Layer.CornerRadius = button.BorderRadius;
		}

		void UpdateFont()
		{
			Control.TitleLabel.Font = Element.Font.ToUIFont();
		}

		async Task UpdateImage()
		{
			IImageSourceHandler handler = null;
			FileImageSource source = Element.Image;
			if (source != null && (handler = Registrar.Registered.GetHandler<IImageSourceHandler>(source.GetType())) != null)
			{
				UIImage uiimage;
				try
				{
					uiimage = await handler.LoadImageAsync(source, scale: (float)UIScreen.MainScreen.Scale);
				}
				catch (OperationCanceledException)
				{
					uiimage = null;
				}
				UIButton button = Control;
				if (button != null && uiimage != null)
				{
					button.SetImage(uiimage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);

					button.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

					ComputeEdgeInsets(Control, Element.ContentLayout);
				}
			}
			else
			{
				Control.SetImage(null, UIControlState.Normal);
				ClearEdgeInsets(Control);
			}
			((IVisualElementController)Element).NativeSizeChanged();
		}

		void UpdateText()
		{
			var newText = Element.Text;

			if (Control.Title(UIControlState.Normal) != newText)
			{
				Control.SetTitle(Element.Text, UIControlState.Normal);
				_titleChanged = true;
			}
		}

		void UpdateTextColor()
		{
			if (Element.TextColor == Color.Default)
			{
				Control.SetTitleColor(_buttonTextColorDefaultNormal, UIControlState.Normal);
				Control.SetTitleColor(_buttonTextColorDefaultHighlighted, UIControlState.Highlighted);
				Control.SetTitleColor(_buttonTextColorDefaultDisabled, UIControlState.Disabled);
			}
			else
			{
				Control.SetTitleColor(Element.TextColor.ToUIColor(), UIControlState.Normal);
				Control.SetTitleColor(Element.TextColor.ToUIColor(), UIControlState.Highlighted);
				Control.SetTitleColor(_buttonTextColorDefaultDisabled, UIControlState.Disabled);
				Control.TintColor = Element.TextColor.ToUIColor();
			}
		}

		void ClearEdgeInsets(UIButton button)
		{
			if (button == null)
			{
				return;
			}

			Control.ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, 0);
			Control.TitleEdgeInsets = new UIEdgeInsets(0, 0, 0, 0);
			Control.ContentEdgeInsets = new UIEdgeInsets(0, 0, 0, 0);
		}

		void ComputeEdgeInsets(UIButton button, Button.ButtonContentLayout layout)
		{
			if (button?.ImageView?.Image == null || string.IsNullOrEmpty(button.TitleLabel?.Text))
			{
				return;
			}

			var position = layout.Position;
			var spacing = (nfloat)(layout.Spacing / 2);

			if (position == Button.ButtonContentLayout.ImagePosition.Left)
			{
				button.ImageEdgeInsets = new UIEdgeInsets(0, -spacing, 0, spacing);
				button.TitleEdgeInsets = new UIEdgeInsets(0, spacing, 0, -spacing);
				button.ContentEdgeInsets = new UIEdgeInsets(0, 2 * spacing, 0, 2 * spacing);
				return;
			}

			if (_titleChanged)
			{
				var stringToMeasure = new NSString(button.TitleLabel.Text);
				UIStringAttributes attribs = new UIStringAttributes { Font = button.TitleLabel.Font };
				_titleSize = stringToMeasure.GetSizeUsingAttributes(attribs);
				_titleChanged = false;
			}

			var labelWidth = _titleSize.Width;
			var imageWidth = button.ImageView.Image.Size.Width;

			if (position == Button.ButtonContentLayout.ImagePosition.Right)
			{
				button.ImageEdgeInsets = new UIEdgeInsets(0, labelWidth + spacing, 0, -labelWidth - spacing);
				button.TitleEdgeInsets = new UIEdgeInsets(0, -imageWidth - spacing, 0, imageWidth + spacing);
				button.ContentEdgeInsets = new UIEdgeInsets(0, 2 * spacing, 0, 2 * spacing);
				return;
			}

			var imageVertOffset = (_titleSize.Height / 2);
			var titleVertOffset = (button.ImageView.Image.Size.Height / 2);

			var edgeOffset = (float)Math.Min(imageVertOffset, titleVertOffset);

			button.ContentEdgeInsets = new UIEdgeInsets(edgeOffset, 0, edgeOffset, 0);

			var horizontalImageOffset = labelWidth / 2;
			var horizontalTitleOffset = imageWidth / 2;

			if (position == Button.ButtonContentLayout.ImagePosition.Bottom)
			{
				imageVertOffset = -imageVertOffset;
				titleVertOffset = -titleVertOffset;
			}

			button.ImageEdgeInsets = new UIEdgeInsets(-imageVertOffset, horizontalImageOffset, imageVertOffset, -horizontalImageOffset);
			button.TitleEdgeInsets = new UIEdgeInsets(titleVertOffset, -horizontalTitleOffset, -titleVertOffset, horizontalTitleOffset);
		}
	}
}