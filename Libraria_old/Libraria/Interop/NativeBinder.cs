using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Dynamic;

using Libraria.Native;
using Libraria.Reflection;

namespace Libraria.Interop {
	public class NativeBinder : DynamicObject {
		CallingConvention DefaultCConv;
		List<LibraryBinder> LibraryBinders;

		public NativeBinder(CallingConvention CConv = CallingConvention.Cdecl) {
			LibraryBinders = new List<LibraryBinder>();
			DefaultCConv = CConv;
		}

		LibraryBinder GetBinder(string Name) {
			foreach (var B in LibraryBinders)
				if (B.LibraryName == Name)
					return B;
			LibraryBinder LB = new LibraryBinder(Name, DefaultCConv);
			LibraryBinders.Add(LB);
			return LB;
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result) {
			result = GetBinder(binder.Name);
			return true;
		}
	}

	public class LibraryBinder : DynamicObject {
		public string LibraryName;
		public IntPtr Module;
		CallingConvention CConv;
		Dictionary<string, Delegate> Functions;

		public LibraryBinder(string LibraryName, CallingConvention CConv = CallingConvention.Cdecl) {
			this.CConv = CConv;
			this.LibraryName = LibraryName;
			Module = Kernel32.LoadLibrary(LibraryName);
			Functions = new Dictionary<string, Delegate>();
		}

		~LibraryBinder() {
			Kernel32.FreeLibrary(Module);
		}

		Delegate GetFunction(string Name, Type ReturnType, Type[] ArgTypes) {
			if (Functions.ContainsKey(Name))
				return Functions[Name];

			if (ReturnType == typeof(object))
				ReturnType = typeof(void);

			IntPtr ProcAddr = Kernel32.GetProcAddress(Module, Name);
			if (ProcAddr == IntPtr.Zero)
				throw new Exception(string.Format("Function '{0}' not found in {1}", Name, LibraryName));

			Delegate D = Marshal.GetDelegateForFunctionPointer(ProcAddr, ReflectionRuntime.CreateDelegateType(ReturnType, ArgTypes, CConv));
			Functions.Add(Name, D);
			return D;
		}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result) {
			Type[] ArgTypes = new Type[args.Length];
			for (int i = 0; i < ArgTypes.Length; i++)
				ArgTypes[i] = args[i].GetType();

			result = GetFunction(binder.Name, binder.ReturnType, ArgTypes).DynamicInvoke(args);
			return true;
		}
	}
}