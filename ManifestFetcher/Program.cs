using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using ManifestFetcher.Client.Properties;

namespace ManifestFetcher.Client
{
#warning 代办：允许自动更新manifest。
	internal class Program
	{
		static void Main(string[] args)
		{
			//先检测manifest中文是否更新
			//再检测manifest中是否有digest中没有的文件。
			//提示新manifest中已经删除的文件，记录到manifest.deleted中。

			ManifestFile manifestFile = ManifestFile.LoadFromFile(ManifestFetcher.Constants.ManifestFilePath);
			ManifestItem[] manifestItems = manifestFile.manifest;

			//获取PGPKey
			Console.Write($"Fetching public key ({manifestFile.settings.security.key_id}) from {manifestFile.settings.security.key_server}...");
			string publicKeyString = HKPSClient.Query(manifestFile.settings.security.key_server, manifestFile.settings.security.key_id);
			if (publicKeyString == null)
			{
				Console.WriteLine("Failed.");
				throw new Exception("Can't fetch pgp public key.");
			}
			Console.WriteLine("Done");

			int i = 0;
			foreach (ManifestItem manifestItem in manifestItems)
			{
				Console.WriteLine($"Fetch manifest[{i}]");

				string baseUrl = manifestItem.remote;
				string digestFileUrl = Path.Combine(baseUrl, ManifestFetcher.Constants.RemoteDigestFile);
				string digestSignatureFileUrl = Path.Combine(baseUrl, ManifestFetcher.Constants.RemoteDigestSignatureFile);

				string baseLocalPath = manifestItem.local;
				string digestFilePath = Path.Combine(baseLocalPath, ManifestFetcher.Constants.RemoteDigestFile);
				string digestSignatureFilePath = Path.Combine(baseLocalPath, ManifestFetcher.Constants.RemoteDigestSignatureFile);

				Download.Donwload(digestFileUrl, digestFilePath);
				Download.Donwload(digestSignatureFileUrl, digestSignatureFilePath);

				File.ReadAllBytes(digestFilePath)

				Utility.PGPSignatureTools.PGPSignature.Verify()

				i++;
			}
		}
	}
}
