using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Libraria {
	public static class Screen {
		public static int Wi {
			get {
				return System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
			}
		}

		public static int Hi {
			get {
				return System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
			}
		}

		public static float Wf {
			get {
				return Wi;
			}
		}

		public static float Hf {
			get {
				return Hi;
			}
		}

		internal static Point? _Center;
		public static Point Center {
			get {
				if (_Center != null)
					return _Center.Value;
				return (_Center = new Point(Wi / 2, Hi / 2)).Value;
			}
		}

		public static T W<T>() {
			object R = Wi;
			return (T)R;
		}

		public static T H<T>() {
			object R = Hi;
			return (T)R;
		}
	}
}