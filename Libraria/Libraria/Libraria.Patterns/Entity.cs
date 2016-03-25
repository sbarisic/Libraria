using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Libraria.Patterns {
	public class Entity {
		Dictionary<Type, object> Components = new Dictionary<Type, object>();

		public T As<T>() where T : class {
			return (T)As(typeof(T));
		}

		public object As(Type T) {
			if (Is(T))
				return Components[T];
			throw new Exception("Component type not found: " + T);
		}

		public void Set(params object[] Vals) {
			for (int i = 0; i < Vals.Length; i++) {
				Type T = Vals[i].GetType();
				if (!T.IsClass)
					throw new Exception("Type is not class " + T);
				if (Is(T))
					throw new Exception("Entity already contains " + T);
				Components.Add(T, Vals[i]);
			}
		}

		public bool Is<T>() where T : class {
			return Is(typeof(T));
		}

		public bool Is(Type T) {
			return Components.ContainsKey(T);
		}
	}

	public static class EntityUtils {
		public static IEnumerable<Entity> Get(this IEnumerable<Entity> Ents, Type T) {
			foreach (var E in Ents)
				if (E.Is(T))
					yield return E;
		}
	}
}