using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace Libraria {
	public struct Circle2D {
		public Vector2 Position;
		public float Radius;

		public bool Collide(Vector2 Point) {
			return (Radius * Radius) < Vector2.DistanceSquared(Position, Point);
		}

		public bool Collide(Circle2D Other) {
			float RadSq = Radius + Other.Radius;
			RadSq *= RadSq;
			return RadSq < Vector2.DistanceSquared(Position, Other.Position);
		}

		public bool Collide(AABB Other) {
			return Other.Collide(this);
		}
	}
}