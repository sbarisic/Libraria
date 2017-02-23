using LibTech.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared {
	public class EntityShared {
	}

	public class Entity : EntityShared {
		[Networked]
		public int Integer { get; set; }
		
		[Networked]
		public string Stringe { get; set; }
	}
}
