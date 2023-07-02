using System;

namespace ManifestFetcher
{
#warning 将CLient或者Server特有的类移出Lib。
	public static class Constants
	{
		public static readonly string ManifestFilePath = "manifest.mf.yml";
		public static readonly string RemoteConfigFile = "config.mf.yml";
		public static readonly string RemoteDigestFile = "digest.mf.yml";
		public static readonly string RemoteDigestSignatureFile = "digest.mf.yml.sign";
	}
}
