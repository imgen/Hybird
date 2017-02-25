namespace Hybird
{
	public enum JsToNativeCommandId
	{
		ScanQRCode = 1,
		UploadPhoto
	}

	public enum NativeToJsCommandId
	{
		ShowToast = 1
	}

	public class CallResult
	{
		public string Status { get; set; }
		public string Message { get; set; }
		public int? Code { get; set; }
		public object Data { get; set; }
	}

	public class NativeToJsCommandReturn
	{
		public int CallId { get; set; }
		public CallResult Result { get; set; }
	}

	public class JsToNativeCommand
	{
		public int CallId { get; set; }
		public object Data { get; set; }
	}
}
