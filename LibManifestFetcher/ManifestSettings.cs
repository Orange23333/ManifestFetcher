using System;
using System.Collections.Generic;
using System.Text;

namespace ManifestFetcher
{
	public class ManifestSettings
	{
		public bool allow_auto_update { get; set; } = false;
		public string auto_update_url { get; set; } = null;
	}
}
