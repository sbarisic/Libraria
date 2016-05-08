using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Libraria.Serialization {
	public static class Binary {
		public static Stream Serialize(object Obj) {
			BinaryFormatter BF = new BinaryFormatter();
			MemoryStream MS = new MemoryStream();
			BF.Serialize(MS, Obj);
			MS.Seek(0, SeekOrigin.Begin);
			return MS;
		}

		public static object DeserializeFile(string Pth) {
			object Ret = null;
			using (FileStream FS = File.OpenRead(Pth)) {
				Ret = Deserialize(FS);
			}
			return Ret;
		}

		public static object Deserialize(Stream S) {
			BinaryFormatter BF = new BinaryFormatter();
			return BF.Deserialize(S);
		}

		public static T Deserialize<T>(Stream S) {
			return (T)Deserialize(S);
		}
	}
}
