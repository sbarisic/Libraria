using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LibTech;

namespace Server {
	public class Entry : ModuleBase {
		public override void Open(ModuleBase Client, ModuleBase Server, ModuleBase UI) {
			Engine.Print("Hello Server!");
		}
	}
}