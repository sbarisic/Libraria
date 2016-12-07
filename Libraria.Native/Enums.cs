using System;
using System.Collections.Generic;
using System.Text;

namespace Libraria.Native {
	[Flags]
	public enum AllocationType : uint {
		COMMIT = 0x1000,
		RESERVE = 0x2000,
		DECOMMIT = 0x4000,
		RELEASE = 0x8000,
		RESET = 0x80000,
		LARGE_PAGES = 0x20000000,
		PHYSICAL = 0x400000,
		TOP_DOWN = 0x100000,
		WRITE_WATCH = 0x200000
	}

	[Flags]
	public enum MemoryProtection : uint {
		EXECUTE = 0x10,
		EXECUTE_READ = 0x20,
		EXECUTE_READWRITE = 0x40,
		EXECUTE_WRITECOPY = 0x80,
		NOACCESS = 0x01,
		READONLY = 0x02,
		READWRITE = 0x04,
		WRITECOPY = 0x08,
		GUARD_Modifierflag = 0x100,
		NOCACHE_Modifierflag = 0x200,
		WRITECOMBINE_Modifierflag = 0x400
	}
}
