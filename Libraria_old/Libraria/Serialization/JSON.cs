using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web.Script.Serialization;

namespace Libraria.Serialization {
	public static class JSON {
		static JavaScriptSerializer Serializer = new JavaScriptSerializer() { MaxJsonLength = int.MaxValue };

		public static string Serialize(object Obj) {
			return Serializer.Serialize(Obj);
		}

		public static T Deserialize<T>(string JsonString) {
			return Serializer.Deserialize<T>(JsonString);
		}

		public static object Deserialize(string JsonString, Type T) {
			return Serializer.Deserialize(JsonString, T);
		}

		public static dynamic Deserialize(string JsonString) {
			return Serializer.DeserializeObject(JsonString);
		}
	}
}
