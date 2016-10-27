using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Numerics;

namespace Libraria.Maths {
	public static partial class MathFuncs {
		public static float Angle(this Vector2 A, Vector2 B) {
			return (float)Math.Atan2(B.Y - A.Y, B.X - A.X);
		}

		public static float Cross(this Vector2 A, Vector2 B) {
			return (A.X * B.Y) - (A.Y * B.X);
		}
	}
}