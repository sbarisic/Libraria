using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Libraria {
	public static partial class Statics {
		/// <summary>
		/// Returns empty string in case supplied string is null
		/// </summary>
		/// <param name="Str">String</param>
		/// <returns></returns>
		public static String Val(this String Str) {
			if (Str == null)
				return "";
			return Str;
		}

		/// <summary>
		/// Returns either Obj or default value in case Obj is null
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="Obj">Object</param>
		/// <param name="DefaultValue">Default value</param>
		/// <returns></returns>
		public static T Val<T>(this T Obj, T DefaultValue) {
			if (Obj == null)
				return DefaultValue;

			return Obj;
		}

		/// <summary>
		/// Returns either Obj or default value in case Obj is null
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="Obj">Object</param>
		/// <param name="DefaultValue">Default value</param>
		/// <returns></returns>
		public static T Val<T>(this T Obj, object DefaultValue) {
			if (Obj == null)
				return (T)DefaultValue;

			return Obj;
		}

		/// <summary>
		/// Returns either Obj or default value in case Obj is null
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="Obj">Object</param>
		/// <param name="DefaultValue">Default value</param>
		/// <returns></returns>
		public static object Val(this object Obj, object DefaultValue) {
			if (Obj == null)
				return DefaultValue;

			return Obj;
		}

		/// <summary>
		/// Return either Obj or initialized or ununitialized default value of Obj
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="Obj">Object</param>
		/// <param name="Allocate">Initialize the default value?</param>
		/// <returns>Initialized or uninitialized value of Obj</returns>
		public static T Val<T>(this T Obj, bool Allocate = true) {
			if (Obj != null)
				return Obj;

			T Ret = Obj;

			if (Ret == null)
				Ret = default(T);

			if (Obj == null)
				if (!Allocate)
					Ret = (T)FormatterServices.GetUninitializedObject(typeof(T));
				else
					Ret = Activator.CreateInstance<T>();

			return Ret;
		}
	}
}