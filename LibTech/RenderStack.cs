using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTech {
	public interface IRenderable {
		void Render(float Dt);
	}

	public static class RenderStack {

		internal static void RenderAll(float Dt) {
		}
	}
}
