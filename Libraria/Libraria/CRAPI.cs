using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Reflection.Emit;
using CCnv = System.Runtime.InteropServices.CallingConvention;

using EasyHook;
using Libraria.Reflection;

namespace Libraria {
	[UnmanagedFunctionPointer(CCnv.StdCall, SetLastError = true)]
	public unsafe delegate
	ulong CompileMethodSig(IntPtr This, IntPtr JITInfo, MthdInfo* MthdInf, uint Flgs, byte** ILCode, ulong* ILCodeSize);

	internal enum JITResult : byte {
		OK,
		BadCode,
		OutOfMemory,
		InternalError,
		Skipped
	}

	[Flags]
	public enum InfoOptions : uint {
		OptInitLocals = 0x00000010,
		GenericsCTXTFromThis = 0x00000020,
		GenericsCTXTFromParamTypeArg = 0x00000040,
		GenericsCTXTMask = (GenericsCTXTFromThis | GenericsCTXTFromParamTypeArg),
		GenericsCTXTKeepAlive = 0x00000080,
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct MthdInfo {
		public IntPtr Func;
		public IntPtr Scope;
		public byte* ILCode;
		public uint ILCodeSize;
		public ushort MaxStack;
		public ushort EHCount;
		public InfoOptions CorInfoOptions;
	}

	public static unsafe class CRAPI {
		[DllImport("clrjit.dll", CallingConvention = CCnv.StdCall, EntryPoint = "getJit", SetLastError = true)]
		public static extern IntPtr GetJIT();

		public static IntPtr[] GetVTblAddresses(IntPtr Ptr, int MethodCount) {
			List<IntPtr> vtblAddresses = new List<IntPtr>();
			IntPtr vTable = Marshal.ReadIntPtr(Ptr);

			for (int i = 0; i < MethodCount; i++) {
				vtblAddresses.Add(Marshal.ReadIntPtr(vTable, i * IntPtr.Size));
			}

			return vtblAddresses.ToArray();
		}

		public static MethodHook<CompileMethodSig> HookJIT() {
			IntPtr CompileMethodPtr = CRAPI.GetVTblAddresses(GetJIT(), 1)[0];
			CompileMethodSig CompileMethod = CompileMethodPtr.ToDelegate<CompileMethodSig>();

			return new MethodHook<CompileMethodSig>(CompileMethodPtr, (This, JITInfo, MthdInf, Flags, Entry, ILCodeSize) => {
				//MthdInfo I = MthdInf.ToStruct<MthdInfo>();
				Console.WriteLine("Compile Method");



				return CompileMethod(This, JITInfo, MthdInf, Flags, Entry, ILCodeSize);
			});
		}
	}

	public class MethodHook<T> {
		public LocalHook Hook;

		public MethodHook(IntPtr Method, T Hook) {
			this.Hook = LocalHook.Create(Method, Hook.As<Delegate>(), this);
			this.Hook.ThreadACL.SetExclusiveACL(new int[1]);
		}
	}
}