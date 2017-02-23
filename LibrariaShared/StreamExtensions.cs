using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Libraria {
	public static class StreamExtensions {
		public static void WriteBytes(this Stream S, byte[] Bytes) {
			S.Write(Bytes, 0, Bytes.Length);
		}

		public static byte[] ReadBytes(this Stream S, int Len) {
			byte[] Bytes = new byte[Len];
			S.Read(Bytes, 0, Bytes.Length);
			return Bytes;
		}
	}
}
