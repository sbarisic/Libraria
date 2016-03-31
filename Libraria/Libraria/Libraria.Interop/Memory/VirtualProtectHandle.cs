using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Libraria.Native;

namespace Libraria.Interop.Memory {
	public class VirtualProtectHandle : IDisposable {
		bool Disposed = false;

		/// <summary>
		/// The address of the first byte that had its protection changed.
		/// </summary>
		public IntPtr Address
		{
			get;
			private set;
		}

		/// <summary>
		/// The size of the memory range with changed protection.
		/// </summary>
		public int Size
		{
			get;
			private set;
		}

		/// <summary>
		/// The previous protection setting.
		/// </summary>
		public MemProtection OldProtection
		{
			get;
			private set;
		}

		/// <summary>
		/// Creates a new <see cref="VirtualProtectHandle"/> instance with the given parameters.
		/// </summary>
		/// <param name="address">The address of the first byte that had its protection changed.</param>
		/// <param name="size">The size of the memory range with changed protection.</param>
		/// <param name="oldProtection">The previous protection setting.</param>
		public VirtualProtectHandle(IntPtr address, int size, MemProtection oldProtection) {
			Address = address;
			Size = size;
			OldProtection = oldProtection;
		}

		/// <summary>
		/// Resets the memory protection to its previous value.
		/// </summary>
		public void Dispose() {
			if (Disposed)
				return;
			Disposed = true;
			Kernel32.VirtualProtect(Address, Size, OldProtection);
		}

		public static VirtualProtectHandle Protect(IntPtr address, int size, MemProtection newProtection) {
			MemProtection oldProtection;
			if (!Kernel32.VirtualProtect(address, size, newProtection, out oldProtection))
				throw new Win32Exception();
			return new VirtualProtectHandle(address, size, oldProtection);
		}
	}
}