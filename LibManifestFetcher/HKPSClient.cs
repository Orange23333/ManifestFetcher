using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;

using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Bcpg;

namespace ManifestFetcher
{
	// ref: https://github.com/openpgpjs/hkp-client/blob/master/src/hkp.js
	public class HKPSClient
	{
		public static string Query(string baseUrl, string keyId)
		{
			if (!baseUrl.EndsWith("/"))
			{
				baseUrl += "/";
			}
			string queryUrl = $"{baseUrl}pks/lookup?op=get&search=0x{keyId}";

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(queryUrl);
			request.Method = "GET";
			request.UserAgent = $"LibManifestFetcher {Assembly.GetExecutingAssembly().GetName().Version}";

			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			StreamReader streamReader = new StreamReader(response.GetResponseStream());
			string result = streamReader.ReadToEnd();

			Regex regex = new Regex("(-----BEGIN PGP PUBLIC KEY BLOCK-----.*?-----END PGP PUBLIC KEY BLOCK-----)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
			Match match = regex.Match(result);
			if (!match.Success)
			{
				return null;
			}

			string pgpPublicKeyString = match.Groups[1].Value;
			return pgpPublicKeyString;
		}

		public static PgpPublicKey ToPgpPublicKey(string armoredPgpPublicKeyString)
		{
			MemoryStream s = new MemoryStream();
			StreamWriter sw = new StreamWriter(s);
			sw.Write(armoredPgpPublicKeyString);
			sw.Flush();

			Stream ds = PgpUtilities.GetDecoderStream(s);
			byte[] buffer = new byte[ds.Length];
#warning long -> int
			ds.Read(buffer, 0, (int)ds.Length);

			PgpPublicKeyRing pgpPublicKeyRing = new PgpPublicKeyRing(ds);
			PgpPublicKey pgpPublicKey = pgpPublicKeyRing.GetPublicKey();
			return pgpPublicKey;
		}
	}
}
