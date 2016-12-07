using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Libraria.Native {
	public static class Asmblr {
		public static byte[] CreateByteArray(params object[] Args) {
			byte[] Ret = new byte[] { };

			for (int i = 0; i < Args.Length; i++) {
				Type ArgType = Args[i].GetType();

				if (ArgType == typeof(byte))
					Ret = Ret.Append(new byte[] { (byte)Args[i] });
				else
					Ret = Ret.Append((byte[])typeof(BitConverter).GetMethod("GetBytes", new Type[] { Args[i].GetType() })
						.Invoke(null, new object[] { Args[i] }));
			}

			return Ret;
		}
	}
}
