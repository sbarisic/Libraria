using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;

namespace Libraria {
	public static partial class Lib {
		public static void GetScreenResolution(out int W, out int H, float Mult = 1) {
			Rectangle Sz = Screen.PrimaryScreen.Bounds;
			W = Sz.Width;
			H = Sz.Height;

			if (Mult != 1) {
				W = (int)(W * Mult);
				H = (int)(H * Mult);
			}
		}
	}
}