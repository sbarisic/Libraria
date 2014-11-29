using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Libraria.Reflection;

namespace Libraria {
	public interface IKeyVal<TK, TV> {
		TK Key {
			get;
		}
		TV Value {
			get;
		}
	}

	public static class Libraria {
		/// <summary>
		/// Returns a list with Arg elements
		/// </summary>
		/// <typeparam name="T">Type of list</typeparam>
		/// <param name="Args">Elements</param>
		/// <returns></returns>
		public static List<T> List<T>(params T[] Args) {
			List<T> Ret = new List<T>();
			for (int i = 0; i < Args.Length; i++)
				Ret.Add(Args[i]);
			return Ret;
		}

		/// <summary>
		/// Returns a Dictionary from list of anonymous objects, they must contain Key and Value properties of TK and TV types
		/// </summary>
		/// <typeparam name="TK">Type of Key</typeparam>
		/// <typeparam name="TV">Type of Value</typeparam>
		/// <param name="KVs">Anonymous objects</param>
		/// <returns></returns>
		public static Dictionary<TK, TV> Dict<TK, TV>(params object[] KVs) {
			Dictionary<TK, TV> Dict = new Dictionary<TK, TV>();
			for (int i = 0; i < KVs.Length; i++) {
				KeyValuePair<TK, TV> KV = ToKeyValuePair<TK, TV>(KVs[i]);
				Dict.Add(KV.Key, KV.Value);
			}
			return Dict;
		}

		/// <summary>
		/// Returns a range of integers
		/// </summary>
		/// <param name="Start">Start integer, can be bigger than End</param>
		/// <param name="End">End integer</param>
		/// <param name="Step">Step, if smaller than 0 then reverses Start and End</param>
		/// <returns></returns>
		public static int[] Range(int Start, int End, int Step = 1) {
			if (Start == End)
				return new int[] { };

			if (Step == 0)
				throw new Exception("Step cannot be 0");
			else if (Step < 0) {
				Step *= -1;
				int Tmp = Start;
				Start = End;
				End = Tmp;
			}

			if (Start < End) {
				int[] Ret = new int[(End - Start) / Step];
				for (int j = Start, i = 0; j < End; j += Step, i++)
					Ret[i] = j;
				return Ret;
			} else {
				int[] Ret = new int[(Start - End) / Step];
				for (int j = Start, i = 0; j > End; j -= Step, i++)
					Ret[i] = j;
				return Ret;
			}
		}

		/// <summary>
		/// Maps anonymous type AnonObj that contins Key, Value properties of types TK and TV to a KeyValuePair
		/// </summary>
		/// <typeparam name="TK">Type of key</typeparam>
		/// <typeparam name="TV">Type of value</typeparam>
		/// <param name="AnonObj">Anonymous object</param>
		/// <returns></returns>
		public static KeyValuePair<TK, TV> ToKeyValuePair<TK, TV>(object AnonObj) {
			PropertyInfo[] AllProps = AnonObj.GetType().GetProperties();
			PropertyInfo[] Props = new PropertyInfo[2];

			for (int i = 0; i < AllProps.Length; i++)
				if (AllProps[i].Name == "Key")
					Props[0] = AllProps[i];
				else if (AllProps[i].Name == "Value")
					Props[1] = AllProps[i];

			if (Props[0] == null || Props[1] == null)
				throw new InvalidOperationException("Anonymous type does not contain Key and Value");
			else if (Props[0].GetMethod.ReturnType != typeof(TK))
				throw new InvalidOperationException("Key of invalid type");
			else if (Props[1].GetMethod.ReturnType != typeof(TV))
				throw new InvalidOperationException("Value of invalid type");

			Delegate GetKey = Props[0].CreateGetDelegate();
			Delegate GetValue = Props[1].CreateGetDelegate();
			return new KeyValuePair<TK, TV>((TK)GetKey.DynamicInvoke(AnonObj), (TV)GetValue.DynamicInvoke(AnonObj));
		}
	}
}