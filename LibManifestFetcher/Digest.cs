using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;

using BCDigests = Org.BouncyCastle.Crypto.Digests;
using IDigest = Org.BouncyCastle.Crypto.IDigest;

namespace ManifestFetcher
{
	public static class Digest
	{
		public static readonly IDigest MD5 = new BCDigests.MD5Digest();
		public static readonly IDigest SHA1 = new BCDigests.Sha1Digest();
		public static readonly IDigest SHA256 = new BCDigests.Sha256Digest();
		public static readonly IDigest SHA512 = new BCDigests.Sha512Digest();
		public static readonly IDigest SM3 = new BCDigests.SM3Digest();

		public static readonly Dictionary<string, IDigest> HMACs = new Dictionary<string, IDigest>()
		{
			{ "md5", MD5},
			{ "sha1", SHA1},
			{ "sha256", SHA256},
			{ "sha2-256", SHA256},
			{ "sha512", SHA512},
			{ "sha2-512", SHA512},
			{ "sm3", SM3 }
		};

		public static IDigest GetDigest(string algorithm)
		{
			if(HMACs.TryGetValue(algorithm.ToLower(), out var r))
			{
				return r;
			}
			throw new ArgumentException("Unknown digest algorithm.", nameof(algorithm));
		}

		private static string ToHexString(byte[] bytes)
		{
			StringBuilder sb = new StringBuilder(bytes.Length*2);
			//foreach(byte b in bytes)
			//{
			//	sb.Append(ToHexChar((byte)((b & 0xf0) >> 4)));
			//	sb.Append(ToHexChar((byte)(b & 0x0f)));
			//}
			for (int i = 0; i < bytes.Length; i++)
			{
				sb.Append($"{bytes[i]:x2}");
			}

			return sb.ToString();
		}
		//private static char ToHexChar(byte halfByte)
		//{
		//	if(halfByte <= 9)
		//	{
		//		return (char)('0' + halfByte);
		//	}
		//	else if (halfByte <= 15)
		//	{
		//		return (char)('a' + halfByte - 10);
		//	}
		//	throw new ArgumentOutOfRangeException(nameof(halfByte));
		//}
		public static byte[] GetFileHash(IDigest digest, string path)
		{
#warning 可以改成异步的
			FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
			byte[] buffer = new byte[4096];
			int n;
			long i = 0;

#warning 不知道为什么用.NET自带的库计算出来的值不一样，所以这里用BC的代替。
			digest.Reset();
			while ((n = fileStream.Read(buffer, 0, buffer.Length)) > 0)
			{
				digest.BlockUpdate(buffer, 0, n);
				i += n;
			}
			fileStream.Close();

			buffer = new byte[digest.GetDigestSize()];
			digest.DoFinal(buffer, 0);

			return buffer;
		}
		public static string GetFileHashHexString(IDigest digest, string path)
		{
			return ToHexString(GetFileHash(digest, path));
		}
	}
}
