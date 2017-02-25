using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace Hybird
{
	static class ResourceLoader
	{
#if __IOS__
        const string resourcePrefix = "Hybird.iOS.";
#endif
#if __ANDROID__
		const string resourcePrefix = "Hybird.Droid.";
#endif

		public static readonly Dictionary<string, string> Cache = new Dictionary<string, string>();

		public static string Load(string fileName)
		{
			if (Cache.ContainsKey(fileName))
			{
				return Cache[fileName];
			}
			
			var assembly = typeof(ResourceLoader).GetTypeInfo().Assembly;
			var stream = assembly.GetManifestResourceStream(resourcePrefix + fileName);
			using (var reader = new StreamReader(stream))
			{
				var content = reader.ReadToEnd();
				Cache.Add(fileName, content);
				return content;
			}
		}
	}
}
