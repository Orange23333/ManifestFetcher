using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ManifestFetcher
{
	public class ManifestFile
	{
		public ManifestItem[] manifest { get; set; }

		public static ManifestFile LoadFromFile(string path)
		{
			YamlDotNet.Serialization.Deserializer deserializer = new YamlDotNet.Serialization.Deserializer();
			ManifestFile r = deserializer.Deserialize<ManifestFile>(File.ReadAllText(path));
			return r;
		}

		public void SaveTo(string path)
		{
			YamlDotNet.Serialization.Serializer serializer = new YamlDotNet.Serialization.Serializer();
			FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
			StreamWriter sw = new StreamWriter(fs);

			serializer.Serialize(sw, this, typeof(ManifestFile));
			sw.Flush();
			fs.Close();
		}
	}
}
