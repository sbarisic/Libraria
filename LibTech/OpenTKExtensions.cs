using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using NVector3 = System.Numerics.Vector3;

namespace LibTech {
	public static class OpenTKExtensions {
		public static NVector3 ToVec3(this Vector3 V) {
			return new NVector3(V.X, V.Y, V.Z);
		}

		public static Vector3 ToVec3(this NVector3 V) {
			return new Vector3(V.X, V.Y, V.Z);
		}
	}
}
