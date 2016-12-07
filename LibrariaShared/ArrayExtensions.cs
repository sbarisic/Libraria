using System;
using System.Collections.Generic;
using System.Text;

namespace Libraria {
	public static class ArrayExtensions {
		public static T[] Append<T>(this T[] Arr, T[] Arr2) {
			T[] NewArr = new T[Arr.Length + Arr2.Length];
			if (Arr.Length > 0)
				Array.Copy(Arr, NewArr, Arr.Length);
			if (Arr2.Length > 0)
				Array.Copy(Arr2, 0, NewArr, Arr.Length, Arr2.Length);
			return NewArr;
		}
	}
}
