using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Libraria {
	public static class StringUtils {
		public static string Substring(this string Str, int StartIndex, int Length, char DefaultChar) {
			if (StartIndex <= Str.Length && StartIndex + Length <= Str.Length)
				return Str.Substring(StartIndex, Length);

			string Sub = "";
			if (StartIndex <= Str.Length)
				Sub = Str.Substring(StartIndex);

			if (Sub.Length < Length)
				Sub += new string(DefaultChar, Length - Sub.Length);
			return Sub;
		}
	}
}