using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Libraria.Maths {
	public static partial class MathFuncs {
		public static int Loop(int Min, int Max, int Val) {
			int Range = Max - Min;
			while (Val < Min)
				Val += Range;
			while (Val > Max)
				Val -= Range;
			return Val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static float FastInvSqrt(float Num) {
			long I = 0x5f375a86 - ((*(long*)&Num) >> 1);
			float Y = *(float*)&I;
			return Y * (1.5f - (Num * .5f * Y * Y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float FastSqrt(float Num) {
			return 1.0f / FastInvSqrt(Num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Lerp(float A, float B, float F) {
			return A + F * (B - A);
		}
	}
}