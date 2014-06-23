using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libraria {
	public interface IIsValid {
		bool IsValid();
	}

	public static partial class Statics {

		/// <summary>
		/// Returns false if Val null else Val.IsValid() or true
		/// </summary>
		/// <typeparam name="T">Object</typeparam>
		/// <param name="Val">Value</param>
		/// <returns>False if Val is null else Val.IsValid() or true</returns>
		public static bool IsValid<T>(this T Val) {
			if (Val is IIsValid && Val != null)
				return ((IIsValid)Val).IsValid();

			if (Val == null)
				return false;

			return true;
		}
	}
}
