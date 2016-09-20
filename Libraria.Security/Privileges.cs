using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;
using System.Linq;

namespace Libraria.Security {
	public static class Privileges {
		public static bool IsInRole(WindowsBuiltInRole Role) {
			WindowsIdentity Ident = WindowsIdentity.GetCurrent();
			WindowsPrincipal Principal = new WindowsPrincipal(Ident);
			return Principal.IsInRole(Role);
		}

		public static bool IsAdministrator() {
			return IsInRole(WindowsBuiltInRole.Administrator);
		}

		public static IEnumerable<WindowsBuiltInRole> GetRoles() {
			return Enumerator.GetValues<WindowsBuiltInRole>().Where(IsInRole);
		}
	}
}
