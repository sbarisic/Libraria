using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading.Tasks;
using Libraria.Native;

namespace Libraria.Interop {
	public static unsafe class Dll {
		public static Symbol[] GetExports(IntPtr Handle) {
			List<Symbol> Exports = new List<Symbol>();

			IMAGE_DOS_HEADER* DOS = (IMAGE_DOS_HEADER*)Handle;
			IMAGE_NT_HEADERS* NT = (IMAGE_NT_HEADERS*)((byte*)DOS + DOS->LFaNew);
			IMAGE_OPTIONAL_HEADER* Optional = &NT->OptionalHeader;
			IMAGE_EXPORT_DIRECTORY* ExportsDir = (IMAGE_EXPORT_DIRECTORY*)((byte*)Handle + Optional->ExportTable.VirtualAddress);

			uint* AddrOfNames = (uint*)((byte*)Handle + ExportsDir->AddressOfNames);
			uint* AddrOfFuncs = (uint*)((byte*)Handle + ExportsDir->AddressOfNames);

			for (int j = 0; j < ExportsDir->NumberOfNames; j++) {
				string Name = Marshal.PtrToStringAnsi((IntPtr)((uint)Handle + AddrOfNames[j]));
				long Func = AddrOfFuncs[j];
				Exports.Add(new Symbol(Name, (ulong)Func, 0));
			}

			return Exports.ToArray();
		}
	}
}