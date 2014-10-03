using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libraria {
	// Because fuck you too, compiler. That's why.
	public static partial class Static {
		public static bool Is<T>(this object O, bool CompareRaw = false) {
			return O.Is(typeof(T), CompareRaw);
		}

		public static bool Is(this object O, Type T, bool CompareRaw = false) {
			if (!CompareRaw)
				return O.GetType() == T;
			else
				if (T.IsAssignableFrom(O.GetType())) // FUCK YOU C#
					return true;
				else
					return O.Is(T);
		}

		public static T As<T>(this object O) {
			return (T)(object)O;
		}
	}
}