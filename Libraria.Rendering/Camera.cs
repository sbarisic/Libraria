using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace Libraria.Rendering {
	public class Camera {
		static Stack<Camera> Cameras = new Stack<Camera>();

		public static void Push(Camera C) {
			Cameras.Push(C);
		}

		public static Camera Pop() {
			return Cameras.Pop();
		}

		public static void Use(Camera C, Action A) {
			Push(C);
			A();
			Pop();
		}

		public static Camera GetCurrent() {
			if (Cameras.Count == 0)
				return null;
			return Cameras.Peek();
		}

		public Matrix4 Translation, Projection;
		public float MouseSensitivity;

		public Quaternion Rotation;
		public Matrix4 RotationMat {
			get {
				return Matrix4.CreateFromQuaternion(Rotation);
			}
		}

		public float RotationX;
		public float RotationY;

		public Camera() {
			Translation = Projection = Matrix4.Identity;
			Rotation = Quaternion.FromAxisAngle(Vector3.UnitY, 0);
			MouseSensitivity = 1.0f;
		}

		public void Move(Vector3 Delta) {
			Translation *= Matrix4.CreateTranslation(-Delta);
		}

		public void SetPosition(Vector3 Pos) {
			Translation = Matrix4.CreateTranslation(Pos);
		}

		public Vector3 GetForward() {
			return Vector3.TransformNormalInverse(new Vector3(0, 0, -1), RotationMat);
		}

		public Vector3 GetRight() {
			return Vector3.TransformNormalInverse(new Vector3(1, 0, 0), RotationMat);
		}

		public Vector3 GetPosition() {
			return -Translation.ExtractTranslation();
		}

		public void MouseRotate(float T, float MouseDX, float MouseDY) {
			float Accel = MouseSensitivity * T;
			float DX = MouseDX * Accel;
			float DY = MouseDY * Accel;

			if (DX == 0 && DY == 0)
				return;

			RotationX += DX;
			RotationY += DY;
			RotationY = (float)MathHelper.Clamp(RotationY, -MathHelper.PiOver2, MathHelper.PiOver2);
			Rotation = Quaternion.FromAxisAngle(Vector3.UnitX, -RotationY) * Quaternion.FromAxisAngle(Vector3.UnitY, -RotationX);
		}

		public Matrix4 Collapse() {
			return Translation * RotationMat * Projection;
		}
	}
}
