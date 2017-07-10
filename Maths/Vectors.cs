using System;
using System.Collections.Generic;
using System.Text;

namespace Libraria.Maths {
	public struct Vector2<T> where T : struct {
		public T X;
		public T Y;

		public Vector2(T X, T Y) {
			this.X = X;
			this.Y = Y;
		}
	}

	public struct Vector3<T> where T : struct {
		public T X;
		public T Y;
		public T Z;

		public Vector3(T X, T Y, T Z) {
			this.X = X;
			this.Y = Y;
			this.Z = Z;
		}
	}

	public struct Vector4<T> where T : struct {
		public T X;
		public T Y;
		public T Z;
		public T W;

		public Vector4(T X, T Y, T Z, T W) {
			this.X = X;
			this.Y = Y;
			this.Z = Z;
			this.W = W;
		}
	}
}