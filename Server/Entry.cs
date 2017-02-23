using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibTech;
using LibTech.Networking;
using Shared;

namespace ServerLib {
	public class Entry : ModuleBase {
		Server NetServer;

		public override void Open(ModuleBase ClientLib, ModuleBase ServerLib, ModuleBase UILib) {
			NetServer = new Server();

			Entity E = NetServer.CreateNetworked(new Entity());
			E.Integer = 666;
			E.Stringe = "Hello World!";
		}

		public override void Update(float Dt) {
			NetServer.Update();
		}
	}
}