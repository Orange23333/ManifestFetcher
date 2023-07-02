using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ManifestFetcher.Client
{
	public class Download
	{
		public static void Donwload(string url, string savePath)
		{
			//url = url.Replace('\\', '/');
			Console.Write($"Downloading \"{url}\"...");

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "GET";
			request.UserAgent = $"ManifestFetcher Client ${Assembly.GetExecutingAssembly().GetName().Version}";

			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			Stream stream = response.GetResponseStream();

#warning 可以异步
			FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.WriteThrough);
			byte[] buffer = new byte[4096];
			int n;
			long i = 0;

			while ((n = stream.Read(buffer, 0, buffer.Length)) > 0)
			{
				fileStream.Write(buffer, 0, n);
				i += n;
			}
			stream.Close();
			fileStream.Close();

			Console.WriteLine("Done.");
		}
	}
}
