using System;
using System.Collections.Generic;
using System.Text;

#warning Group, Requirement

namespace ManifestFetcher
{
	public class ManifestItem
	{
		public static readonly Comparer SharedCompare = new Comparer();

		public string local { get; set; }
		public string remote { get; set; }

		// 应当先下载解压可执行文件，再执行解压命令。或者修改本地的manifest，来执行解压。
		// null (none), zip, bz, gzip, xz, 7z. <format>$<password>
		//public string extract { get; set; }

		public string[] files { get; set; }

		public static bool Equals(ManifestItem[] x, ManifestItem[] y)
		{
			if(x.Length != y.Length)
			{
				return false;
			}

			ManifestItem[] a = (ManifestItem[])x.Clone();
			ManifestItem[] b = (ManifestItem[])y.Clone();
			Array.Sort(a, SharedCompare);
			Array.Sort(b, SharedCompare);

			for(int i = 0; i < a.Length; i++)
			{
				if (SharedCompare.Compare(a[i], b[i]) != 0)
				{
					return false;
				}
			}

			return true;
		}

		public class Comparer:IComparer<ManifestItem>
		{
			public int Compare(ManifestItem x, ManifestItem y)
			{
				int r;
				if((r = x.remote.CompareTo(y.remote)) != 0)
				{
					return r;
				}
				if((r = x.local.CompareTo(y.local)) != 0)
				{
					return r;
				}
				if ((r = (x.files.Length.CompareTo(y.files.Length))) != 0)
				{
					return r;
				}
#warning 当前Manifest时无序的！
				string[] a = (string[])x.files.Clone();
				string[] b = (string[])y.files.Clone();
				Array.Sort(a);
				Array.Sort(b);
				for(int i = 0; i < a.Length; i++)
				{
					if ((r = a[i].CompareTo(b[i])) != 0)
					{
						return r;
					}
				}
				return 0;
			}
		}
	}
}
