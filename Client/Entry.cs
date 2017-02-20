using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libraria.Rendering;
using LibTech;

namespace Client {
	class Entity {
		[Networked]
		public int Property1 { get; set; }

		[Networked]
		public int Property2 { get; set; }

		[Networked]
		public string Property3 { get; set; }

		public Entity() {
			NetworkManager.Network(this);
		}
	}

	public class Entry : ModuleBase {
		public override void Open(ModuleBase Client, ModuleBase Server, ModuleBase UI) {
			Entity E = new Entity();
			E.Property1 = 50;
			E.Property2 = 60;
			E.Property3 = "70";
			E.Property1 = E.Property1 + 50;
		}
	}
}