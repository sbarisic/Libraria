using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libraria.Maths;

namespace Libraria {
	public static class IEnumerableExtensions {
		public static T ElementAtLooping<T>(this IEnumerable<T> E, int Idx) {
			return E.ElementAt(MathFuncs.Loop(0, E.Count() - 1, Idx));
		}
	}
}
