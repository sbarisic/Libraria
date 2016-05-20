using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace Libraria.Physics {
	public class PhysicsWorld {
		public HashSet<PhysicsBody> Bodies;

		public PhysicsWorld() {
			Bodies = new HashSet<PhysicsBody>();
		}

		public void Update(float Delta) {

		}
	}
}