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

namespace LibTests {
	public class Program {
		static void Main(string[] args) {
			CRAPI.HookJIT();
			Console.Title = "LibTests";
			new TestClass();
			Console.WriteLine("Complete");
			Console.ReadLine();
		}
	}

	public class TestClass {
		public TestClass() {
			Test(2, 3);
			Console.WriteLine("0x{0}", typeof(TestClass).GetMethod("Test").MethodHandle.Value.ToString("x"));
			Test(2, 3);
		}

		public int Test(int A, int B) {
			Console.WriteLine("A + B = {0}", A + B);
			return A + B;
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