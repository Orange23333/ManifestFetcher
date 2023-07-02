using System;
using System.Collections.Generic;
using System.Text;

#warning Group, Requirement

namespace ManifestFetcher
{
	public class ManifestItem
	{
		public string local { get; set; }
		public string remote { get; set; }

		// 应当先下载解压可执行文件，再执行解压命令。或者修改本地的manifest，来执行解压。
		// null (none), zip, bz, gzip, xz, 7z. <format>$<password>
		//public string extract { get; set; }

		public string[] files { get; set; }
	}
}
