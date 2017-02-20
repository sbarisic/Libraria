using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTech {
	public static class ContentManager {
		public static string[] GetAllFonts() {
			return Files.GetFilesInDirectory("fonts", SearchPattern: "*.ttf", IncludeChildDirectories: true);
		}
	}
}
