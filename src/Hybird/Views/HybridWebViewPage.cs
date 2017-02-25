using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using ZXing.Mobile;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;
using Plugin.Media;
using System.Linq;
using Newtonsoft.Json.Linq;
using static Hybird.Statics;
using System.IO;
#if __ANDROID__
using Android.Content;
using Android.Provider;
using Android.Bluetooth;
#endif

namespace Hybird.Views
{
	public class HybridWebViewPage : BasePage
	{
		protected HybridWebView WebView { get; set; }

		protected delegate void JsToNativeCommandHandler(int callId, object data);

		protected delegate void NativeToJsCallResultHandler(CallResult result);

		const string DefaultPhotoUploadFormName = "photo";

		public WebViewSource Source
		{
			get
			{
				return WebView.Source;
			}
			set
			{
				WebView.Source = value;
			}
		}

		public HybridWebViewPage()
		{
			WebView = new HybridWebView(new XLabs.Serialization.JsonNET.JsonSerializer())
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Margin = new Thickness(8)
			};
			WebView.LoadFinished += delegate
			{
				OnWebViewLoadFinished();
			};
#if __ANDROID__
			// Initialize the scanner first so it can track the current context
			MobileBarcodeScanner.Initialize(CurrentActivity.Application);
#endif
			RegisterCallbacksAndCommands();
			CrossMedia.Current.Initialize();
		}

		protected Dictionary<JsToNativeCommandId, JsToNativeCommandHandler> NativeCommandHandlers
		{
			get
			{
				return _nativeCommandHandlers;
			}
			set
			{
				_nativeCommandHandlers = value;
			}
		}

		Dictionary<int, NativeToJsCallResultHandler> _jsCommandCallResultHandlers = new Dictionary<int, NativeToJsCallResultHandler>();
		Queue<NativeToJsCallResultHandler> _jsCommandCallResultHandlerQueue = new Queue<NativeToJsCallResultHandler>();
		Dictionary<JsToNativeCommandId, JsToNativeCommandHandler> _nativeCommandHandlers = new Dictionary<JsToNativeCommandId, JsToNativeCommandHandler>();

		readonly object _locker = new object();

		protected virtual void OnWebViewLoadFinished()
		{
		}

		void RegisterCallbacksAndCommands()
		{
			RegisterNativeCallback<NativeToJsCommandReturn>("SetNativeToJsCallResult", ret =>
			{
				int callId = ret.CallId;
				if (_jsCommandCallResultHandlers.ContainsKey(callId))
				{
					var handler = _jsCommandCallResultHandlers[callId];
					lock (_locker)
					{
						_jsCommandCallResultHandlers.Remove(callId);
					}
					handler(ret.Result);
				}
			});

			RegisterNativeCallback<int>(
				"SetNativeToJsCallId",
				callId =>
				{
					lock (_locker)
					{
						var handler = _jsCommandCallResultHandlerQueue.Dequeue();
						if (handler != null)
						{
							_jsCommandCallResultHandlers[callId] = handler;
						}
					}
				});

			RegisterJsToNativeCommand(JsToNativeCommandId.ScanQRCode, async (callId, data) =>
			{
				var scanner = new MobileBarcodeScanner();

				var result = await scanner.Scan();

				if (result != null)
				{
					SetSuccessJsToNativeCallResult(callId, result);
				}
			});

			RegisterJsToNativeCommand(JsToNativeCommandId.UploadPhoto, async(callId, data) =>
			{
				var json = data as JObject;
				var url = (string)json["url"];
				var formName = (string)json["form_name"];
				formName = formName ?? DefaultPhotoUploadFormName;
				var fileName = (string)json["file_name"];

				var photo = await PickPhotoAsync();
				if (photo == null)
				{
					return;
				}
				fileName = string.IsNullOrEmpty(fileName) ? 
				                 Path.GetFileName(photo.Path) : 
				                 fileName + "." + Path.GetExtension(photo.Path);
				var stream = photo.GetStream();
				Device.BeginInvokeOnMainThread(() =>
				{
					ShowHUD("Uploading photo");
				});
				try
				{
					await UploadStream(url, stream, formName, fileName);
					SetSuccessJsToNativeCallResult(callId, "Successfully uploaded photo");
					Device.BeginInvokeOnMainThread(() =>
					{
						ShowSuccessHUDWithStatus("Upload photo successfully");
					});
				}
				catch (Exception ex)
				{
					ex = ex.InnerException ?? ex;
					var message = $"Unable to upload file {photo.Path} to {url}, error: {ex}";
					Console.Error.WriteLine(message);
					SetErrorJsToNativeCallResult(callId, message);
					Device.BeginInvokeOnMainThread(() =>
					{
						ShowErrorHUDWithStatus("Upload photo failed, please check connectivity and retry");
					});
				}
			});
		}

		async Task<MediaFile> TakeBackPhotoAsync()
		{
			return await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
			{
				PhotoSize = PhotoSize.Medium,
				Directory = "Sample",
				Name = "test.jpg"
			});
		}

		async Task<MediaFile> PickPhotoAsync()
		{
			return await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
			{
				PhotoSize = PhotoSize.Full
			});
		}

		protected void LoadContent(string html, string baseUri = null)
		{
			WebView.LoadContent(html, baseUri);
		}

		protected void LoadFromContent(string contentFullName, string baseUri = null)
		{
			WebView.LoadFromContent(contentFullName, baseUri);
		}

		protected void InvokeNativeToJsCommand(NativeToJsCommandId commandId, object data = null, NativeToJsCallResultHandler resultHandler = null)
		{
			lock (_locker)
			{
				_jsCommandCallResultHandlerQueue.Enqueue(resultHandler);
			}
			WebView.CallJsFunction("executeNativeToJsCommand", commandId, data);
		}

		protected void RegisterJsToNativeCommand(JsToNativeCommandId commandId, JsToNativeCommandHandler handler)
		{
			NativeCommandHandlers[commandId] = handler;
			RegisterNativeCallback<JsToNativeCommand>(commandId.ToString(),
								  command => handler(command.CallId, command.Data));
		}

		public void RegisterNativeCallback<T>(string name, Action<T> callback)
		{
			WebView.RegisterCallback(name, json =>
			{
				var data = JsonConvert.DeserializeObject<T>(json);
				callback(data);
			});
		}

		public void SetJsToNativeCallResult(int callId, CallResult result)
		{
			WebView.CallJsFunction("setJsToNativeCallResult", callId, result);
		}

		public void SetSuccessJsToNativeCallResult<T>(int callId, T data)
		{
			WebView.CallJsFunction("setSuccessJsToNativeCallResult", callId, data);
		}

		public void SetErrorJsToNativeCallResult<T>(int callId, T data)
		{
			WebView.CallJsFunction("setErrorJsToNativeCallResult", callId, data);
		}

		public void SetFailJsToNativeCallResult<T>(int callId, T data)
		{
			WebView.CallJsFunction("setFailJsToNativeCallResult", callId, data);
		}
	}
}
