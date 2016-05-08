using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Libraria.Native;

namespace Libraria.Interop {
	public struct Symbol {
		public string Name;
		public ulong RelativeAddress;
		public uint Size;

		public Symbol(string Name, ulong RelativeAddress, uint Size) {
			this.Name = Name;
			this.RelativeAddress = RelativeAddress;
			this.Size = Size;
		}

		public override string ToString() {
			return Name;
		}
	}

	public static class DebugHelp {
		public static Symbol[] GetSymbols(string DllFile) {
			IntPtr CurProc = Process.GetCurrentProcess().Handle;
			DbgHelp.SymInitialize(CurProc, null, false);

			if (!File.Exists(DllFile))
				throw new FileNotFoundException("File not found", DllFile);
			List<Symbol> Symbols = new List<Symbol>();

			ulong DllBase = DbgHelp.SymLoadModuleEx(CurProc, IntPtr.Zero, DllFile, null, 0, 0, IntPtr.Zero, 0);
			DbgHelp.SymEnumerateSymbols64(CurProc, DllBase, (Name, Addr, Size, Ctx) => {
				Symbols.Add(new Symbol(Name, DllBase - Addr, Size));
				return true;
			}, IntPtr.Zero);

			DbgHelp.SymCleanup(CurProc);
			return Symbols.ToArray();
		}

		public static string Demangle(string Symbol) {
			//IntPtr CurProc = Process.GetCurrentProcess().Handle;
			StringBuilder SB = new StringBuilder(4069);

			//DbgHelp.SymInitialize(CurProc, null, false);
			DbgHelp.UnDecorateSymbolName(Symbol, SB, SB.Capacity, UndnameFlags.UNDNAME_COMPLETE);
			//DbgHelp.SymCleanup(CurProc);

			return SB.ToString().Trim();
		}
	}
}