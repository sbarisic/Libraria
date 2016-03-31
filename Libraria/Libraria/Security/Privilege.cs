using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Libraria.Security {
	public class Privilege {
		static Type PrivType = Type.GetType("System.Security.AccessControl.Privilege");
		object Priv;

		public Privilege(string Name) {
			Priv = Activator.CreateInstance(PrivType, Name);
		}

		public void Enable() {
			PrivType.GetMethod("Enable").Invoke(Priv, null);
		}

		public void Disable() {
			PrivType.GetMethod("Revert").Invoke(Priv, null);
		}
	}
}