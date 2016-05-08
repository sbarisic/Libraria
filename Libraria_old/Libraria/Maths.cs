using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libraria {
	public static class Maths {
		public static int Loop(int Min, int Max, int Val) {
			int Range = Max - Min;
			while (Val < Min)
				Val += Range;
			while (Val > Max)
				Val -= Range;
			return Val;
		}
	}
}
