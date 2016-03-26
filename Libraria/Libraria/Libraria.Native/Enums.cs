using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Libraria.Native {
	[Flags]
	public enum MemProtection : uint {
		NoAccess = 0x01,
		ReadOnly = 0x02,
		ReadWrite = 0x04,
		WriteCopy = 0x08,
		Exec = 0x10,
		ExecRead = 0x20,
		ExecReadWrite = 0x40,
		ExecWriteCopy = 0x80,
		PageGuard = 0x100,
		NoCache = 0x200,
		WriteCombine = 0x400
	}

	[Flags]
	public enum AllocType : uint {
		Commit = 0x1000,
		Reserve = 0x2000,
		Reset = 0x80000,
		LargePages = 0x20000000,
		Physical = 0x400000,
		TopDown = 0x100000,
		WriteWatch = 0x200000
	}

	[Flags]
	public enum ProcessAccess : uint {
		AllAccess = 0x1F0FFF
	}

	[Flags]
	public enum ModuleHandleFlags : uint {
		Pin = 0x1,
		UnchangedRefCount = 0x2,
		FromAddress = 0x4,
	}

	[Flags]
	public enum ThreadAccess : int {
		TERMINATE = (0x0001),
		SUSPEND_RESUME = (0x0002),
		GET_CONTEXT = (0x0008),
		SET_CONTEXT = (0x0010),
		SET_INFORMATION = (0x0020),
		QUERY_INFORMATION = (0x0040),
		SET_THREAD_TOKEN = (0x0080),
		IMPERSONATE = (0x0100),
		DIRECT_IMPERSONATION = (0x0200)
	}

	[Flags]
	public enum ProcessCreationFlags : uint {
		ZERO_FLAG = 0x00000000,
		CREATE_BREAKAWAY_FROM_JOB = 0x01000000,
		CREATE_DEFAULT_ERROR_MODE = 0x04000000,
		CREATE_NEW_CONSOLE = 0x00000010,
		CREATE_NEW_PROCESS_GROUP = 0x00000200,
		CREATE_NO_WINDOW = 0x08000000,
		CREATE_PROTECTED_PROCESS = 0x00040000,
		CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x02000000,
		CREATE_SEPARATE_WOW_VDM = 0x00001000,
		CREATE_SHARED_WOW_VDM = 0x00001000,
		CREATE_SUSPENDED = 0x00000004,
		CREATE_UNICODE_ENVIRONMENT = 0x00000400,
		DEBUG_ONLY_THIS_PROCESS = 0x00000002,
		DEBUG_PROCESS = 0x00000001,
		DETACHED_PROCESS = 0x00000008,
		EXTENDED_STARTUPINFO_PRESENT = 0x00080000,
		INHERIT_PARENT_AFFINITY = 0x00010000
	}

	[Flags]
	public enum CloneProcessFlags : uint {
		CreateSuspended = 0x1,
		InheritHandles = 0x2,
		NoSync = 0x4,
	}

	public enum CloneStatus : int {
		Parent = 0,
		Child = 297
	}

	public enum ProcessInformationClasss : int {
		ProcessBasicInformation = 0,
		ProcessDebugPort = 7,
		ProcessWow64Information = 26,
		ProcessImageFileName = 27,
		ProcessBreakOnTermination = 29,
	}
}