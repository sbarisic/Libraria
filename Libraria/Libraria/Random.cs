using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libraria {
	public static class Rand {
		internal static Random R = new Random();

		public static void Reseed(int Seed) {
			R = new Random(Seed);
		}

		public static void Reseed() {
			R = new Random();
		}

		public static int Next(int Min = int.MinValue, int Max = int.MaxValue) {
			return R.Next(Min, Max);
		}

		public static float NextF() {
			return (float)R.NextDouble();
		}

		public static double NextD() {
			return R.NextDouble();
		}

		public static byte NextB(byte Min = byte.MinValue, byte Max = byte.MaxValue) {
			return (byte)R.Next(Min, Max);
		}

		public static T NextElement<T>(this T[] Arr) {
			return Arr[Next(0, Arr.Length)];
		}

		public static T NextType<T>(T Min, T Max) {
			if (typeof(T).Is<int>())
				return Next(Min.As<int>(), Max.As<int>()).As<T>();

			if (typeof(T).Is<float>())
				return NextF().As<T>();

			if (typeof(T).Is<double>())
				return NextD().As<T>();

			if (typeof(T).Is<byte>())
				return NextB(Min.As<byte>(), Max.As<byte>()).As<T>();

			throw new Exception("Unsupported type " + typeof(T).ToString());
		}
	}
}