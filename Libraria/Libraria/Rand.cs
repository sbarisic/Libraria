using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Libraria {
	public static class Rand {
		static Random Rnd;

		static Rand() {
			Rnd = new Random();
		}

		public static Random Random
		{
			get
			{
				return Rnd;
			}
		}

		public static byte[] Bytes(int Len) {
			byte[] Bytes = new byte[Len];
			Rnd.NextBytes(Bytes);
			return Bytes;
		}

		public static byte Byte() {
			return (byte)Rnd.Next(256);
		}

		public static int Int() {
			return BitConverter.ToInt32(Bytes(sizeof(int)), 0);
		}

		public static long Long() {
			return BitConverter.ToInt64(Bytes(sizeof(long)), 0);
		}

		public static double Double() {
			double Mantissa = (Rnd.NextDouble() * 2) - 1;
			double Exp = Math.Pow(2, Rnd.Next(-126, 128));
			return Mantissa * Exp;
		}

		public static double Chance() {
			return Rnd.NextDouble();
		}

		public static float Float() {
			return (float)Double();
		}
	}
}