using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Diagnostics;
using Libraria.IO;
using Libraria.Net;
using Libraria.Reflection;
using Libraria.Native;

namespace Test {
	/*public interface A : I, J {
		void A_A();
	}

	public interface B : A, E, G {
		void B_A();
	}

	public interface C {
		void C_A();
	}

	public interface D : F, H {
		void D_A();
	}

	public interface E {
		void E_A();
	}

	public interface F {
		void F_A();
	}

	public interface G : K {
		void G_A();
	}
	public interface H {
		void H_A();
	}

	public interface I {
		void I_A();
	}

	public interface J {
		void J_A();
	}

	public interface K {
		void K_A();
	}

	public interface Test : B, C, D {
		void Test_A();
	}*/

	public interface A {
		int SomeInt { get; set; }
		int SomeOtherInt { get; set; }

		void FuncA();
		void FuncB();
	}

	public interface B {
		void FuncC();
	}

	public interface Test : A, B {
		void SetOtherInt(int I);
		void FuncE();
	}

	delegate void FuncInt(IntPtr This, int I);

	class Program {
		[DllImport("NativeTest")]
		static extern IntPtr GetPointer();

		static void Main(string[] Args) {
			IntPtr ClassInstancePtr = GetPointer();
			NativeClassInfo Info = VTable.CalculateClassLayout(typeof(Test));

			IntPtr SomeOtherIntPtr = Info.FindVariable(typeof(A).GetProperty("SomeOtherInt")).GetAddress(ClassInstancePtr);
			Console.WriteLine("SomeOtherInt = {0}", Marshal.ReadInt32(SomeOtherIntPtr));

			Info.Find(typeof(Test).GetMethod("SetOtherInt")).GetDelegate<FuncInt>(ClassInstancePtr)(ClassInstancePtr, 42);
			
			Console.WriteLine("SomeOtherInt = {0}", Marshal.ReadInt32(SomeOtherIntPtr));

			//Console.WriteLine("Done!");
			Console.ReadLine();
		}
	}
}