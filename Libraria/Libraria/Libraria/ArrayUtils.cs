using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Libraria {
	public static class ArrayUtils {
		public static T[] Append<T>(this T[] A, T[] B) {
			T[] New = new T[A.Length + B.Length];
			Array.Copy(A, New, A.Length);
			Array.Copy(B, 0, New, A.Length, B.Length);
			return New;
		}

		public static T[] Sub<T>(this T[] A, int Idx, int Len) {
			T[] New = new T[Len];
			Array.Copy(A, Idx, New, 0, Len);
			return New;
		}

		public static T[] Sub<T>(this T[] A, int Len) {
			return A.Sub(0, Len);
		}

		public static T[] Sub<T>(this T[] A, long Len) {
			return A.Sub((int)Len);
		}
	}
}