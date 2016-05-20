using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace Libraria.Physics {
	public class PhysicsBody {
		public float Mass { get { return 1.0f / InverseMass; } set { InverseMass = 1.0f / value; } }
		public float Restitution;
		public float ElectricCharge;
		public float ElectricResistance;

		public Vector2 Position;
		public Vector2 Velocity;
		public float AngularVelocity;
		public float InverseMass;
	}
}
