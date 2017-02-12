using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Libraria.Reflection.CustomMarshals {
	public unsafe class UTF8StringMarshal : ICustomMarshaler {
		public static ICustomMarshaler GetInstance(string cookie) {
			return new UTF8StringMarshal();
		}

		public void CleanUpManagedData(object ManagedObj) {
		}

		public void CleanUpNativeData(IntPtr pNativeData) {
			if (pNativeData == IntPtr.Zero)
				return;
			Marshal.FreeHGlobal(pNativeData);
		}

		public int GetNativeDataSize() {
			return -1;
		}

		public IntPtr MarshalManagedToNative(object ManagedObj) {
			if (ManagedObj == null || !(ManagedObj is string))
				return IntPtr.Zero;

			byte[] Bytes = Encoding.UTF8.GetBytes((string)ManagedObj);
			IntPtr Native = Marshal.AllocHGlobal(Bytes.Length + 1);
			Marshal.Copy(Bytes, 0, Native, Bytes.Length);
			Marshal.WriteByte(Native, Bytes.Length, 0);
			return Native;
		}

		public object MarshalNativeToManaged(IntPtr pNativeData) {
			if (pNativeData == IntPtr.Zero)
				return null;

			int Len = strlen(pNativeData);
			byte[] Bytes = new byte[Len];
			Marshal.Copy(pNativeData, Bytes, 0, Len);
			return Encoding.UTF8.GetString(Bytes);
		}

		static int strlen(IntPtr Ptr) {
			int Len = -1;
			while (Marshal.ReadByte(Ptr, Len++) != 0)
				;
			return Len;
		}
	}
}