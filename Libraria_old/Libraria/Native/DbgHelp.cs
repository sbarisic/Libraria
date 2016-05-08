using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Libraria.Native {
	[Flags]
	public enum UndnameFlags : int {
		UNDNAME_32_BIT_DECODE = 0x0800,
		UNDNAME_COMPLETE = 0x0000,
		UNDNAME_NAME_ONLY = 0x1000,
		UNDNAME_NO_ACCESS_SPECIFIERS = 0x0080,
		UNDNAME_NO_ALLOCATION_LANGUAGE = 0x0010,
		UNDNAME_NO_ALLOCATION_MODEL = 0x0008,
		UNDNAME_NO_ARGUMENTS = 0x2000,
		UNDNAME_NO_CV_THISTYPE = 0x0040,
		UNDNAME_NO_FUNCTION_RETURNS = 0x0004,
		UNDNAME_NO_LEADING_UNDERSCORES = 0x0001,
		UNDNAME_NO_MEMBER_TYPE = 0x0200,
		UNDNAME_NO_MS_KEYWORDS = 0x0002,
		UNDNAME_NO_MS_THISTYPE = 0x0020,
		UNDNAME_NO_RETURN_UDT_MODEL = 0x0400,
		UNDNAME_NO_SPECIAL_SYMS = 0x4000,
		UNDNAME_NO_THISTYPE = 0x0060,
		UNDNAME_NO_THROW_SIGNATURES = 0x0100,
	}

	public delegate bool SymEnumerateSymbolsProc64(string SymbolName, ulong SymbolAddress, uint SymbolSize, IntPtr UserContext);

	public unsafe static class DbgHelp {
		const string Lib = "Dbghelp";
		const CharSet CSet = CharSet.Unicode;
		const CallingConvention CConv = CallingConvention.Winapi;

		[DllImport(Lib, SetLastError = true, CharSet = CSet)]
		public static extern int UnDecorateSymbolName(string DecName, StringBuilder UndName, int UndLen, UndnameFlags Fags);

		[DllImport(Lib, SetLastError = true, CharSet = CSet)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SymInitialize(IntPtr Proc, string UserSearchPath, [MarshalAs(UnmanagedType.Bool)]bool fInvadeProcess);

		[DllImport(Lib, SetLastError = true, CharSet = CSet)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SymCleanup(IntPtr Proc);

		[DllImport(Lib, SetLastError = true, CharSet = CSet)]
		public static extern ulong SymLoadModuleEx(IntPtr Proc, IntPtr File, string ImageName, string ModuleName, long BaseOfDll, int DllSize, IntPtr Data, int Flags);

		[DllImport(Lib, SetLastError = true, CharSet = CSet)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SymEnumerateSymbols64(IntPtr Proc, ulong BaseOfDll, SymEnumerateSymbolsProc64 EnumSymbolsCallback, IntPtr UserContext);
	}
}