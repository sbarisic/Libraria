using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;

namespace Libraria.Graphics {
	public static class Resolution {
		public static void GetDesktop(out int Width, out int Height) {
			Rectangle WorkingArea = Screen.PrimaryScreen.Bounds;
			Width = WorkingArea.Width;
			Height = WorkingArea.Height;
		}

		public static void GetDesktop(out uint Width, out uint Height) {
			int W, H;
			GetDesktop(out W, out H);
			Width = (uint)W;
			Height = (uint)H;
		}
	}
}
