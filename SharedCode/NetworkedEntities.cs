using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared {
	public static class EntityManager {
		// TODO: Network
		static List<EntityShared> Entities;

		static EntityManager() {
			Entities = new List<EntityShared>();
		}

		public static T Get<T>(int Idx) where T : EntityShared {
			if (Idx > 0 && Idx < Entities.Count)
				return Entities[Idx] as T;
			return null;
		}
	}
}
