using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Libraria.Native {
	public struct STARTUPINFO {
		public uint cb;
		public string lpReserved;
		public string lpDesktop;
		public string lpTitle;
		public uint dwX;
		public uint dwY;
		public uint dwXSize;
		public uint dwYSize;
		public uint dwXCountChars;
		public uint dwYCountChars;
		public uint dwFillAttribute;
		public uint dwFlags;
		public short wShowWindow;
		public short cbReserved2;
		public IntPtr lpReserved2;
		public IntPtr hStdInput;
		public IntPtr hStdOutput;
		public IntPtr hStdError;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct CLIENT_ID {
		public uint ProcessID;
		public uint ThreadID;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct SectionImageInfo {
		public IntPtr EntryPoint;
		public uint StackZeroBits;
		public uint StackReserved;
		public uint StackCommit;
		public uint ImageSubsystem;
		public ushort SubSysVerLow;
		public ushort SubSysVerHigh;
		public uint U1;
		public uint ImageStats;
		public uint ImageMachineType;
		public fixed uint U2[3];
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct PROCESS_INFORMATION {
		public IntPtr Process;
		public IntPtr Thread;
		public CLIENT_ID ClientID;

		public PROCESS_INFORMATION(IntPtr Process, IntPtr Thread, uint PID, uint TID) {
			this.Process = Process;
			this.Thread = Thread;
			this.ClientID = new CLIENT_ID();
			this.ClientID.ProcessID = PID;
			this.ClientID.ThreadID = TID;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ProcessInfo {
		public uint Size;
		public PROCESS_INFORMATION ProcInfo;
		public SectionImageInfo ImageInfo;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct PROCESS_BASIC_INFORMATION {
		public IntPtr Reserved1;
		public IntPtr PebBaseAddress;
		public IntPtr Reserved2_0;
		public IntPtr Reesrved2_1;
		public IntPtr UniqueProcessId;
		public IntPtr Reserved3;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct CSRSS_MESSAGE {
		public uint Unknown1;
		public uint Opcode;
		public uint Status;
		public uint Unknown2;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct PORT_MESSAGE {
		public uint Unknown1;
		public uint Unknown2;
		public CLIENT_ID ClientID;
		public uint MessageID;
		public uint CallbackID;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct CSRMsg {
		public PORT_MESSAGE PortMsg;
		public CSRSS_MESSAGE CSRSSMsg;
		public PROCESS_INFORMATION ProcessInfo;
		public CLIENT_ID CID;
		public uint CreationFlags;
		public fixed uint VdmInfo[2];
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct IMAGE_NT_HEADERS {
		public short Signature;
		public IMAGE_FILE_HEADER FileHeader;
		public IMAGE_OPTIONAL_HEADER OptionalHeader;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct IMAGE_FILE_HEADER {
		public short Machine;
		public short NumberOfSections;
		public int TimeDateStamp;
		public int PointerToSymbolTable;
		public int NumberOfSymbols;
		public short SizeOfOptionalHeader;
		public short Characteristics;
	}

	[StructLayout(LayoutKind.Explicit)]
	public unsafe struct IMAGE_BASE {
		[FieldOffset(0)]
		public int BaseOfData;
		[FieldOffset(sizeof(int))]
		public int ImageBase32;
		[FieldOffset(0)]
		public long ImageBase64;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct IMAGE_OPTIONAL_HEADER {
		public short Magic;
		public byte MajorLinkerVersion;
		public byte MinorLinkerVersion;
		public int SizeOfCode;
		public int SizeOfInitializedData;
		public int SizeOfUninitializedData;
		public int AddressOfEntryPoint;
		public int BaseOfCode;
		public IMAGE_BASE ImageBase;
		public int SectionAlignment;
		public int FileAlignment;
		public short MajorOperatingSystemVersion;
		public short MinorOperatingSystemVersion;
		public short MajorImageVersion;
		public short MinorImageVersion;
		public short MajorSubsystemVersion;
		public short MinorSubsystemVersion;
		public int Win32VersionValue;
		public int SizeOfImage;
		public int SizeOfHeaders;
		public int CheckSum;
		public short Subsystem;
		public short DllCharacteristics;
		public IntPtr SizeOfStackReserve;
		public IntPtr SizeOfStackCommit;
		public IntPtr SizeOfHeapReserve;
		public IntPtr SizeOfHeapCommit;
		public int LoaderFlags;
		public int NumberOfRvaAndSizes;
		public IMAGE_DATA_DIRECTORY ExportTable;
		public IMAGE_DATA_DIRECTORY ImportTable;
		public IMAGE_DATA_DIRECTORY ResourceTable;
		public IMAGE_DATA_DIRECTORY ExceptionTable;
		public IMAGE_DATA_DIRECTORY CertificateTable;
		public IMAGE_DATA_DIRECTORY BaseRelocationTable;
		public IMAGE_DATA_DIRECTORY Debug;
		public IMAGE_DATA_DIRECTORY Arch;
		public IMAGE_DATA_DIRECTORY GlobalPtr;
		public IMAGE_DATA_DIRECTORY TLSTable;
		public IMAGE_DATA_DIRECTORY LoadConfigTable;
		public IMAGE_DATA_DIRECTORY BoundImport;
		public IMAGE_DATA_DIRECTORY IAT;
		public IMAGE_DATA_DIRECTORY DelayImportDescriptor;
		public IMAGE_DATA_DIRECTORY CLRRuntimeHeader;
		public IMAGE_DATA_DIRECTORY Reserved;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct IMAGE_DATA_DIRECTORY {
		public int VirtualAddress;
		public int Size;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct IMAGE_EXPORT_DIRECTORY {
		public int Characteristics;
		public int TimeDateStamp;
		public short MajorVersion;
		public short MinorVersion;
		public int Name;
		public int Base;
		public int NumberOfFunctions;
		public int NumberOfNames;
		public int AddressOfFunctions;     // RVA from base of image
		public int AddressOfNames;     // RVA from base of image
		public int AddressOfNameOrdinals;  // RVA from base of image
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct IMAGE_DOS_HEADER {
		public ushort Magic;
		public ushort CBLP;
		public ushort CP;
		public ushort CRLc;
		public ushort CPARHdr;
		public ushort MinAlloc;
		public ushort MaxAlloc;
		public ushort SS;
		public ushort SP;
		public ushort CSum;
		public ushort IP;
		public ushort CS;
		public ushort LFarLc;
		public ushort OVNO;
		public fixed ushort Res[4];
		public ushort OEMId;
		public ushort OEMInfo;
		public fixed ushort Res2[10];
		public uint LFaNew;
	}
}