using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Libraria.Native {
	public class NativeUtils {
		static Dictionary<string, IntPtr> LibCache = new Dictionary<string, IntPtr>();

		public static IntPtr LoadLibrary(string Pth) {
			if (LibCache.ContainsKey(Pth)) // TODO: Handle extensions
				return LibCache[Pth];

			IntPtr Handle = Kernel32.GetModuleHandle(Pth);
			if (Handle == IntPtr.Zero)
				Handle = Kernel32.LoadLibrary(Pth);
			if (Handle == IntPtr.Zero)
				return IntPtr.Zero;

			LibCache.Add(Pth, Handle);
			return Handle;
		}

		public static IntPtr GetAddress(IntPtr Module, string Name) {
			return Kernel32.GetProcAddress(Module, Name);
		}

		public static IntPtr GetAddress<T>(IntPtr Module, string Name) where T : struct {
			return GetAddress(Module, Name) - Marshal.SizeOf<T>();
		}
	}
}
