using System;
using System.Collections.Generic;
using System.Text;

namespace Libraria.Native {
	[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
	class InterfaceVersionAttribute : Attribute {
		public string Identifier { get; set; }

		public InterfaceVersionAttribute(string versionIdentifier) {
			Identifier = versionIdentifier;
		}
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class VTableSlotAttribute : Attribute {
		public int Slot { get; set; }

		public VTableSlotAttribute(int s) {
			Slot = s;
		}
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class VTableOffsetAttribute : Attribute {
		public int Offset { get; set; }

		public VTableOffsetAttribute(int s) {
			Offset = s;
		}
	}
}
