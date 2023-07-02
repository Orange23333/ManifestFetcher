using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ManifestFetcher
{
	public class DigestFile
	{
		public string algorithm { get; set; }
		public DigestItem[] files { get; set; }

		public static bool Verify(string targetDirectory)
		{
			JetBrains.DownloadPgpVerifier.PgpSignaturesVerifier.Verify()
		}
		
		private static void Sign(string targetDirectory)
		{
			Utility.PGPSignatureTools.PGPSignature.Sign();
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

			fs.Position = 0;
			StreamReader sr= new StreamReader(fs);
			byte[] bytes = Encoding.ASCII.GetBytes(sr.ReadToEnd());

			Utility.PGPSignatureTools.PGPSignature.Sign(bytes, )
		}
	}
}
