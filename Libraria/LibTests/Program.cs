using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Reflection.Emit;

using Xunit;
using Xunit.Extensions;
using Libraria;

using Lib = Libraria.Libraria;

namespace LibTests {
	public class Program {
		static void Main(string[] args) {
			Console.Title = "LibTests";

			Dictionary<string, int> Dict = Lib.Dict<string, int>(new {
				Key = "SomeKey",
				Value = 1337,
				Nigger = 23123
			}, new {
				Key = "31",
				Value = 69
			});

			foreach (var KVPair in Dict)
				Console.WriteLine("Key: {0}, Value: {1}", KVPair.Key, KVPair.Value);

			int[] Rng = Lib.Range(10, 20, -1);
			for (int i = 0; i < Rng.Length; i++)
				Console.Write("{0} ", Rng[i]);
			Console.WriteLine();


			Console.WriteLine("Complete");
			Console.ReadLine();
		}
	}
		

	public class Tests {
		[Theory]
		[InlineData(true, 1, typeof(int), false)]
		[InlineData(true, 1.0f, typeof(float), false)]
		[InlineData(true, 1.0d, typeof(double), false)]
		[InlineData(true, typeof(int), typeof(Type), true)]
		[InlineData(true, typeof(float), typeof(Type), true)]
		[InlineData(true, typeof(double), typeof(Type), true)]
		[InlineData(false, 1, typeof(Type), true)]
		[InlineData(false, 1.0f, typeof(Type), true)]
		[InlineData(false, 1.0d, typeof(Type), true)]
		public static void Comparator_Is(bool Expected, object O, Type T, bool Raw) {
			Assert.Equal(Expected, O.Is(T, Raw));
		}

		[Fact]
		public static void Comparator_As() {
			Assert.DoesNotThrow(() => {
				(1).As<int>();
				(1f).As<float>();
				(1d).As<double>();
				new object().As<object>();
			});

			Assert.ThrowsAny<Exception>(() => new object().As<int>());
			Assert.ThrowsAny<Exception>(() => new object().As<float>());
			Assert.ThrowsAny<Exception>(() => (1).As<float>());
			Assert.ThrowsAny<Exception>(() => (1f).As<int>());
		}
	}
}