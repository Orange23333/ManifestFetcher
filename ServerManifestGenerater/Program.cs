using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Crypto;

using BCDigests = Org.BouncyCastle.Crypto.Digests;
using IDigest = Org.BouncyCastle.Crypto.IDigest;
using System.Reflection;

namespace ManifestFetcher.Server
{
	public class Program
	{
		public static readonly string[] ignoreFiles = new string[]
		{
			ManifestFetcher.Constants.ManifestFilePath,
			ManifestFetcher.Constants.RemoteConfigFile,
			ManifestFetcher.Constants.RemoteDigestFile
		};

		public static ConfigFile? ConfigFile = null;

		static void Main(string[] args)
		{
			string targetDirectory = args[0];
			ConfigFile = ConfigFile.LoadFromFile(Path.Combine(targetDirectory, ManifestFetcher.Constants.RemoteConfigFile));

			Console.WriteLine($"Target directory is \"{targetDirectory}\", without recursion option.");

			DirectoryInfo directoryInfo = new DirectoryInfo(targetDirectory);
			FileInfo[] files = (from val in directoryInfo.GetFiles()
								where !ignoreFiles.Contains(val.Name)
								select val).ToArray();

			GenerateManifest(targetDirectory, files);
			GenerateDigest(targetDirectory, files);

			Console.WriteLine("Finished!");
		}

		public static void GenerateManifest(string targetDirectory, FileInfo[] fileInfos)
		{

			ManifestItem manifestItem = new ManifestItem()
			{
				local = ConfigFile.manifest_client_local ?? throw new NullReferenceException(nameof(ConfigFile.manifest_client_local)),
				remote = ConfigFile.manifest_remote_url ?? throw new NullReferenceException(nameof(ConfigFile.manifest_remote_url))
			};
			ManifestFile manifestFile = new ManifestFile()
			{
				manifest = new ManifestItem[]
				{
					manifestItem
				}
			};
			List<string> files = new List<string>();

			foreach (FileInfo fileInfo in fileInfos)
			{
				if (!ignoreFiles.Contains(fileInfo.Name))
				{
					files.Add(fileInfo.Name);
				}
			};
			manifestItem.files = files.ToArray();
			manifestFile.SaveTo(Path.Combine(targetDirectory, ManifestFetcher.Constants.ManifestFilePath));
		}

		public static void GenerateDigest(string targetDirectory, FileInfo[] fileInfos)
		{
			DigestFile digestFile = new DigestFile()
			{
				algorithm = ConfigFile.digest_algorithm ?? throw new NullReferenceException(nameof(ConfigFile.digest_algorithm))
			};
			List<DigestItem> digests = new List<DigestItem>();
			IDigest digest = Digest.GetDigest(ConfigFile.digest_algorithm);

			foreach (FileInfo fileInfo in fileInfos)
			{
				if (!ignoreFiles.Contains(fileInfo.Name))
				{
					string filename = fileInfo.FullName;

					digests.Add(new DigestItem()
					{
						file = fileInfo.Name,
						digest = Digest.GetFileHashHexString(digest, filename)
					});
				}
			}
			digestFile.files = digests.ToArray();

			digestFile.SaveTo(Path.Combine(targetDirectory, ManifestFetcher.Constants.RemoteDigestFile));
		}
	}
}