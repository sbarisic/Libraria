using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Libraria.Native {
	public unsafe static class NtDll {
		const string Lib = "ntdll";
		const CharSet CSet = CharSet.Ansi;
		const CallingConvention CConv = CallingConvention.Winapi;

		[DllImport(Lib, SetLastError = true, CharSet = CSet)]
		public static extern int NtResumeProcess(IntPtr Proc);

		[DllImport(Lib, SetLastError = true, CharSet = CSet)]
		public static extern int NtSuspendProcess(IntPtr Proc);

		[DllImport(Lib, SetLastError = true, CharSet = CSet)]
		public static extern int NtResumeThread(IntPtr Thread, IntPtr SuspendCount);

		public static int NtResumeThread(IntPtr Thread) {
			return NtResumeThread(Thread, IntPtr.Zero);
		}

		[DllImport(Lib, SetLastError = true, CharSet = CSet)]
		public static extern int NtSuspendThread(IntPtr Thread, IntPtr SuspendCount);

		public static int NtSuspendThread(IntPtr Thread) {
			return NtSuspendThread(Thread, IntPtr.Zero);
		}

		[DllImport(Lib, SetLastError = true, CharSet = CSet)]
		public static extern int LdrUnloadDll(IntPtr Module);

		[DllImport(Lib, SetLastError = true, CallingConvention = CConv)]
		public static extern CloneStatus RtlCloneUserProcess(CloneProcessFlags Flags, IntPtr ProcSecDesc, IntPtr ThreadSecDesc,
			IntPtr DebugPort, ProcessInfo* ProcessInfo);

		public static CloneStatus RtlCloneUserProcess(CloneProcessFlags Flags, ProcessInfo* ProcessInfo) {
			return RtlCloneUserProcess(Flags, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, ProcessInfo);
		}

		[DllImport(Lib, SetLastError = true, CallingConvention = CConv)]
		public static extern bool RtlCreateUserThread(IntPtr Proc, IntPtr SecDesc, bool CreateSuspended, uint StackZeroBits,
			uint StackReserved, uint StackCommit, IntPtr StartAddr, IntPtr StartParam, IntPtr Thread, CLIENT_ID* Result);

		public static bool RtlCreateUserThread(IntPtr P, IntPtr Fnc, IntPtr Data, out IntPtr Thread) {
			Thread = IntPtr.Zero;
			fixed (IntPtr* ThrdPtr = &Thread)
				return RtlCreateUserThread(P, IntPtr.Zero, false, 0, 0, 0, Fnc, Data, new IntPtr(ThrdPtr), (CLIENT_ID*)0);
		}

		public static bool RtlCreateUserThread(Process P, IntPtr Fnc, IntPtr Data, out IntPtr Thread) {
			IntPtr Proc = Kernel32.OpenProcess(ProcessAccess.AllAccess, false, P.Id);
			bool Ret = RtlCreateUserThread(Proc, Fnc, Data, out Thread);
			Kernel32.CloseHandle(Proc);
			return Ret;
		}

		[DllImport(Lib, SetLastError = true, CallingConvention = CConv)]
		public static extern void RtlExitUserThread(int Status);

		[DllImport(Lib, SetLastError = true, CallingConvention = CConv)]
		public static extern void RtlExitUserProcess(int Status);

		[DllImport(Lib, SetLastError = true, CallingConvention = CConv)]
		public static extern uint ZwAllocateVirtualMemory(IntPtr Proc, ref IntPtr Addr, int ZeroBits, ref IntPtr RegionSize,
			AllocType AType = AllocType.Commit | AllocType.Reserve, MemProtection Prot = MemProtection.ReadWrite);

		[DllImport(Lib, SetLastError = true, CallingConvention = CConv)]
		public static extern void CsrClientCallServer(CSRMsg* Msg, int A = 0, int B = 0x10000, int C = 0x24);

		public static void CsrClientCallServer(IntPtr Process, IntPtr Thread, int PID, int TID) {
			CsrClientCallServer(Process, Thread, (uint)PID, (uint)TID);
		}

		public static void CsrClientCallServer(IntPtr Process, IntPtr Thread, uint PID, uint TID) {
			CSRMsg CSRMessage = new CSRMsg();
			CSRMessage.ProcessInfo = new PROCESS_INFORMATION(Process, Thread, PID, TID);
			CsrClientCallServer(&CSRMessage);
		}

		[DllImport(Lib, SetLastError = true, CallingConvention = CConv)]
		public static extern int NtQueryInformationProcess(IntPtr processHandle, ProcessInformationClasss Clss,
			IntPtr processInformation, int processInformationLength, out int returnLength);
	}
}