using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libraria {
	public interface IValid {
		bool Valid();
	}

	public static partial class Statics {

		/// <summary>
		/// Returns false if Val null else Val.Valid() or true
		/// </summary>
		/// <typeparam name="T">Object</typeparam>
		/// <param name="Val">Value</param>
		/// <returns>False if Val is null else Val.Valid() or true</returns>
		public static bool Valid<T>(this T Val) {
			if (Val is IValid && Val != null)
				return ((IValid)Val).Valid();

			if (Val == null)
				return false;

			return true;
		}
	}
}
