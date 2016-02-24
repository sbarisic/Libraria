using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Libraria.Patterns {
	public class Entity {
		private Dictionary<Type, object> Components;

		public Entity() {
			Components = new Dictionary<Type, object>();
		}

		public virtual void SetComponent(object Comp) {
			if (Comp == null)
				throw new Exception("Component cannot be null");

			Type T = Comp.GetType();
			if (Components.ContainsKey(T))
				Components.Remove(T);
			Components.Add(T, Comp);
		}

		public virtual T GetComponent<T>() {
			object Ret = GetComponent(typeof(T));
			if (Ret == null)
				return default(T);
			return (T)Ret;
		}

		public virtual object GetComponent(Type T) {
			if (Components.ContainsKey(T))
				return Components[T];

			foreach (var C in Components)
				if (T.IsAssignableFrom(C.Key))
					return C.Value;

			return null;
		}

		public void RemoveComponent<T>(bool AssignableTo = false) {
			RemoveComponent(typeof(T), AssignableTo);
		}

		public virtual void RemoveComponent(Type T, bool AssignableTo = false) {
			if (AssignableTo) {
				Type Key;
				if (HasComponent(T, out Key))
					Components.Remove(Key);
			} else if (Components.ContainsKey(T))
				Components.Remove(T);
		}

		public bool HasComponent<T>() {
			return HasComponent(typeof(T));
		}

		public bool HasComponent(params Type[] Types) {
			for (int i = 0; i < Types.Length; i++)
				if (!HasComponent(Types[i]))
					return false;
			return true;
		}

		public bool HasComponent(Type T) {
			Type Key;
			return HasComponent(T, out Key);
		}

		public virtual bool HasComponent(Type T, out Type Key) {
			if (Components.ContainsKey(T)) {
				Key = T;
				return true;
			}

			foreach (var C in Components)
				if (T.IsAssignableFrom(C.Key)) {
					Key = C.Key;
					return true;
				}

			Key = typeof(void);
			return false;
		}

		//////////////////////////////////////////////////////////////////////////////

		public static IEnumerable<Entity> GetEntities(IEnumerable<Entity> Ents, params Type[] Components) {
			foreach (var Ent in Ents)
				if (Ent.HasComponent(Components))
					yield return Ent;
		}
	}

	public class EntitySystem {
		Type[] ComponentTypes;

		public EntitySystem(params Type[] ComponentTypes) {
			this.ComponentTypes = ComponentTypes;
		}

		public void Update(IEnumerable<Entity> Entities) {
			IEnumerable<Entity> Ents = Entity.GetEntities(Entities, ComponentTypes);
			foreach (var E in Ents)
				Update(E);
		}

		public virtual void Update(Entity E) {
		}
	}
}