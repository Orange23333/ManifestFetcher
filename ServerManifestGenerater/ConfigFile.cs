using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using YamlDotNet;

namespace ManifestFetcher.Server
{
	public class ConfigFile
	{
		public string? digest_algorithm { get; set; }
		public string? manifest_client_local { get; set; }
		public string? manifest_remote_url { get; set; }

		public static ConfigFile LoadFromFile(string path)
		{
			YamlDotNet.Serialization.Deserializer deserializer = new YamlDotNet.Serialization.Deserializer();
			ConfigFile r = deserializer.Deserialize<ConfigFile>(File.ReadAllText(path));
			return r;
		}
	}
}
