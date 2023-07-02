using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ManifestFetcher
{
	public class DigestFile
	{
		public string algorithm { get; set; }
#warning 没有检查重复项
		public DigestItem[] files { get; set; }

		public bool Contains(string filename)
		{
			return (from val in files
					where val.file == filename
					select val).Any();
		}

		public bool TryGetValue(string filename, out DigestItem result)
		{
			var q = from val in files
					where val.file == filename
					select val;
			if(q.Any())
			{
				result = q.First();
				return true;
			}
			result = null;
			return false;
		}

		public static DigestFile LoadFromFile(string path)
		{
			YamlDotNet.Serialization.Deserializer deserializer = new YamlDotNet.Serialization.Deserializer();
			DigestFile r = deserializer.Deserialize<DigestFile>(File.ReadAllText(path));
			return r;
		}
		public void SaveTo(string path)
		{
			YamlDotNet.Serialization.Serializer serializer = new YamlDotNet.Serialization.Serializer();
			FileStream fs= new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
			StreamWriter sw = new StreamWriter(fs);

#warning Remeber to flush the stream!!!
			serializer.Serialize(sw, this, typeof(DigestFile));
			sw.Flush();
			fs.Flush();
		}
	}
}
