using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Libraria.Native {
	public unsafe static class PsAPI {
		const string Lib = "psapi";
		const CharSet CSet = CharSet.Ansi;
		const CallingConvention CConv = CallingConvention.Winapi;

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern bool EnumProcessModules(IntPtr Proc, IntPtr Modules, uint Size, out uint SizeNeeded);

		public static bool EnumProcessModules(IntPtr Proc, IntPtr Modules, uint Size) {
			uint SizeNeeded;
			return EnumProcessModules(Proc, Modules, Size, out SizeNeeded);
		}

		public static bool EnumProcessModules(IntPtr Proc, IntPtr[] Modules, out uint SizeNeeded) {
			fixed (IntPtr* ModulesPtr = Modules)
			{
				return EnumProcessModules(Proc, new IntPtr(ModulesPtr), (uint)(Modules.Length * IntPtr.Size), out SizeNeeded);
			}
		}

		public static bool EnumProcessModules(IntPtr Proc, IntPtr[] Modules) {
			uint SizeNeeded;
			return EnumProcessModules(Proc, Modules, out SizeNeeded);
		}

		[DllImport(Lib, SetLastError = true, CharSet = CSet, CallingConvention = CConv)]
		public static extern uint GetModuleFileNameEx(IntPtr Proc, IntPtr Mod, StringBuilder FileName, int Size = 80);
	}
}