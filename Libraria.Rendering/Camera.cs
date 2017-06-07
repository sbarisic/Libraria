using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace Libraria.Rendering {
	public class Camera {
		public Matrix4 Translation, Zoom, Projection;
		public float MouseSensitivity;

		public Quaternion Rotation;

		public float RotationX;
		public float RotationY;

		public Camera() {
			Translation = Zoom = Projection = Matrix4.Identity;
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
			Matrix4 M = Matrix4.CreateFromQuaternion(Rotation);
			return Vector3.TransformNormalInverse(new Vector3(0, 0, -1), M);
		}

		public Vector3 GetRight() {
			Matrix4 M = Matrix4.CreateFromQuaternion(Rotation);
			return Vector3.TransformNormalInverse(new Vector3(1, 0, 0), M);
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
			Rotation = Quaternion.FromAxisAngle(Vector3.UnitX, -RotationY) *
				Quaternion.FromAxisAngle(Vector3.UnitY, -RotationX);
		}

		public Matrix4 Collapse() {
			return Translation * Matrix4.CreateFromQuaternion(Rotation) * Zoom * Projection;
		}
	}
}
