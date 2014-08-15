using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libraria {
	public static partial class Statics {

		/// <summary>
		/// Converts double to float, shorter than cast
		/// </summary>
		/// <param name="D">Double</param>
		/// <returns>Float</returns>
		public static float f(this double D) {
			return (float)D;
		}

		/// <summary>
		/// Casts O to T and returns
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="O">Object</param>
		/// <returns>(T)O</returns>
		public static T To<T>(this object O) {
			return (T)O;
		}
	}
}
