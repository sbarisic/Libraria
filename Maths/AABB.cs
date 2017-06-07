using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace Libraria.Maths {
	public struct AABB {
		public Vector2 Min, Max;

		// Clockwise starting with top-left corner (min)
		public Vector2 CornerA { get { return Min; } }
		public Vector2 CornerB { get { return new Vector2(Max.X, Min.Y); } }
		public Vector2 CornerC { get { return Max; } }
		public Vector2 CornerD { get { return new Vector2(Min.X, Max.Y); } }

		public AABB(float X, float Y, float W, float H) {
			Min = new Vector2(X, Y);
			Max = new Vector2(X + W, Y + H);
		}

		public float Width
		{
			get
			{
				return Max.X - Min.X;
			}
		}

		public float Height
		{
			get
			{
				return Max.Y - Min.Y;
			}
		}

		public bool Collide(float X, float Y) {
			return X < Max.X && X > Min.X && Y < Max.Y && Y > Min.Y;
		}

		public bool Collide(AABB Other) {
			if (Max.X < Other.Min.X || Min.X > Other.Max.X)
				return false;
			if (Max.Y < Other.Min.Y || Min.Y > Other.Max.Y)
				return false;
			return true;
		}

		public bool Collide(Circle2D Other) {
			return Other.Collide(CornerA) || Other.Collide(CornerB) || Other.Collide(CornerC) || Other.Collide(CornerD);
		}
	}
}