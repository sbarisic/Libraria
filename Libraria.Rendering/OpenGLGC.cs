using System;
using System.Collections.Generic;
using System.Text;

namespace Libraria.Rendering {
	public static class OpenGLGC {
		public interface Destructable {
			void Destroy();
		}

		static Queue<Destructable> Finalized = new Queue<Destructable>();

		public static void Enqueue(Destructable Obj, ref bool WasFinalized) {
			if (!WasFinalized) {
				WasFinalized = true;
				lock (Finalized)
					Finalized.Enqueue(Obj);
				GC.ReRegisterForFinalize(Obj);
			}
		}

		public static int CollectAll() {
			lock (Finalized) {
				int Cnt = Finalized.Count;
				while (Finalized.Count > 0)
					Finalized.Dequeue().Destroy();
				return Cnt;
			}
		}
	}
}