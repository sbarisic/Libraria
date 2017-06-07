using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libraria.Rendering;
using LibTech;
using LibTech.Networking;
using Shared;

namespace ClientLib {
	public class Entry : ModuleBase {
		Client NetClient;

		public override void Open() {
			NetClient = new Client();
			NetClient.Connect("127.0.0.1");
		}

		bool Printed = false;
		public override void Update(float Dt) {
			NetClient.Update();

			Entity E = NetClient.GetObject(0) as Entity;
			if (!Printed && E != null) {
				Printed = true;

				Engine.Print("Integer: ", E.Integer);
				Engine.Print("Stringe: ", E.Stringe);
			}
		}
	}
}