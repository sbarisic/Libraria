using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Libraria.Net {
	public static class Webs {
		public static string DownloadString(string Url) {
			using (WebClient WC = new WebClient()) {
				return WC.DownloadString(Url);
			}
		}

		public static byte[] DownloadData(string Url) {
			using (WebClient WC = new WebClient()) {
				return WC.DownloadData(Url);
			}
		}

		public static int DownloadInt(string Url) {
			return int.Parse(DownloadString(Url));
		}
	}
}
