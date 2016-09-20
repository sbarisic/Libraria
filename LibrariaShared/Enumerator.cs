using System;
using System.Collections.Generic;
using System.Text;

namespace Libraria {
	public static class Enumerator {
		public static T Parse<T>(string Value) {
			return (T)Enum.Parse(typeof(T), Value);
		}

		public static IEnumerable<T> GetValues<T>() {
			Array A = Enum.GetValues(typeof(T));
			foreach (object Element in A)
				yield return (T)Element;
		}
	}
}