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

	public interface Animal {
		void Eat();
	}

	public interface Mammal : Animal {
		void Breathe();
	}

	public interface WingedAnimal : Animal {
		void Flap();
	}

	public interface Bat : Mammal, WingedAnimal {
	}


	unsafe class Program {
		[DllImport("NativeTest")]
		[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(NativeClassMarshal<Bat>))]
		static extern Bat GetPointer();

		static void Main(string[] Args) {
			//IntPtr ClassInstancePtr = GetPointer();

			Bat B = GetPointer();
			B.Eat();
			B.Breathe();
			B.Flap();

			//Console.WriteLine("Done!");
			Console.ReadLine();
		}

		static void PrintStr(IntPtr Str) {
			Console.WriteLine(Encoding.UTF8.GetString((byte*)Str, 32));

			//Console.WriteLine(Encoding.ASCII.GetString((byte*)Str, 40));
			//Console.WriteLine(Encoding.Unicode.GetString((byte*)Str, 40));
			//Console.WriteLine(Encoding.UTF8.GetString((byte*)Str, 40));
		}
	}

	struct STRUCTE {
		public int val;
	}

	unsafe class TESTTEST {
		IntPtr Instance;

		IntPtr OffsetThisPtr(int Offset) {
			return Instance + Offset;
		}

		public STRUCTE TestInt {
			get {
				STRUCTE* P = (STRUCTE*)OffsetThisPtr(42);
				return *P;
			}

			set {
				STRUCTE* P = (STRUCTE*)OffsetThisPtr(42);
				*P = value;
			}
		}
	}
}