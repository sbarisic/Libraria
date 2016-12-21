using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace Libraria.Maths {
	public struct Rect {
		public float X, Y, W, H;

		public Rect(float X, float Y, float W, float H) {
			this.X = X;
			this.Y = Y;
			this.W = W;
			this.H = H;
		}

		public void SetPos(float X, float Y) {
			this.X = X;
			this.Y = Y;
		}

		public void SetSize(float W, float H) {
			this.W = W;
			this.H = H;
		}
	}
}