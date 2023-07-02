using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using BCDigests = Org.BouncyCastle.Crypto.Digests;
using IDigest = Org.BouncyCastle.Crypto.IDigest;

using ManifestFetcher.Client;

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

			Console.WriteLine("注意使用本软件在更新文件时，默认会覆盖源文件内容，请谨慎操作！（按任意键继续）");
			Console.ReadKey();

			ManifestFile manifestFile = ManifestFile.LoadFromFile(ManifestFetcher.Constants.ManifestFilePath);
			ManifestItem[] manifestItems = manifestFile.manifest;

			int i = 0;
			foreach (ManifestItem manifestItem in manifestItems)
			{
				Console.WriteLine($"Fetch manifest[{i}]");

				string baseUrl = manifestItem.remote;
				string digestFileUrl = Path.Combine(baseUrl, ManifestFetcher.Constants.RemoteDigestFile);

				string baseLocalPath = manifestItem.local;
				string digestFilePath = Path.Combine(baseLocalPath, ManifestFetcher.Constants.RemoteDigestFile);

				Download.Donwload(digestFileUrl, digestFilePath);

#warning 文件操作存在间隙，本地攻击可能。
				DigestFile digestFile = DigestFile.LoadFromFile(digestFilePath);
				
				DirectoryInfo directoryInfo = new DirectoryInfo(baseLocalPath);
				IDigest digestAlgorithm = Digest.GetDigest(digestFile.algorithm);
				foreach(string filename in manifestItem.files)
				{
					if (digestFile.TryGetValue(filename, out DigestItem digestItem))
					{
						string fileUrl = Path.Combine(baseUrl, filename);
						string filePath = Path.Combine(baseLocalPath, filename);
						Download.Donwload(fileUrl, filePath);

#warning 文件操作存在间隙，本地攻击可能。
						string digest = Digest.GetFileHashHexString(digestAlgorithm, filePath).ToLower();
						if (String.CompareOrdinal(digest, digestItem.digest.ToLower()) != 0)
						{
							FileInfo fileInfo = new FileInfo(filePath);
							fileInfo.Delete();
							fileInfo = new FileInfo(filePath);
							Console.WriteLine($"Warning: File digest of \"{filename}\"  isn't correct.");
						}
						else
						{
							Console.WriteLine($"Downloaded \"{fileUrl}\" -> \"{filePath}\".");
						}
					}
					else
					{
						Console.WriteLine($"Warning: Skipped file \"{filename}\", because it isn't has a digest in digest file.");
					}
				}

				i++;
			}
		}
	}
}
