using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Libraria;

namespace Libraria.Native {
	public static class MachineCode {
		public static IntPtr AllocMem(IntPtr BaseAddr, int Size) {
			return Kernel32.VirtualAlloc(BaseAddr, (IntPtr)Size, AllocationType.COMMIT, MemoryProtection.EXECUTE_READWRITE);
		}

		public static IntPtr ToFunction(byte[] Code) {
			IntPtr Ptr = AllocMem(IntPtr.Zero, Code.Length);
			Marshal.Copy(Code, 0, Ptr, Code.Length);
			return Ptr;
		}

		public static T ToDelegate<T>(byte[] Code, out IntPtr FuncPtr) where T : class {
			FuncPtr = ToFunction(Code);
			return Marshal.GetDelegateForFunctionPointer<T>(FuncPtr);
		}

		public static T ToDelegate<T>(byte[] Code) where T : class {
			IntPtr FuncPtr;
			return ToDelegate<T>(Code, out FuncPtr);
		}

		public static void Link(IntPtr Addr, Delegate Func) {
			IntPtr FuncPtr = Marshal.GetFunctionPointerForDelegate(Func);

			byte[] LinkCode = Asmblr.CreateByteArray(
				(byte)0x50, (byte)0x50,											// push RAX; push RAX
				(byte)0x48, (byte)0xB8, FuncPtr.ToInt64(),						// movabs RAX, FuncPtr
				(byte)0x48, (byte)0x89, (byte)0x44, (byte)0x24, (byte)0x08,     // mov QWORD PTR [RSP + 0x8], RAX
				(byte)0x58, (byte)0xC3											// pop RAX; ret
				);

			Marshal.Copy(LinkCode, 0, Addr, LinkCode.Length);
		}

		public static void Link(int Addr, Delegate Func) {
			Link((IntPtr)Addr, Func);
		}

		public static void Link(int Addr, Action Func) {
			Link((IntPtr)Addr, Func);
		}
	}
}
