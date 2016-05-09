using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

using SharpBgfx;

namespace Libraria.Rendering {
	public class RenderWindow : Form {
		Thread RenderThread;
		public int RenderWidth, RenderHeight;
		public ResetFlags ResetFlags;
		public bool IsRunning { get; private set; }

		public RenderWindow(string Title, int W, int H, ResetFlags ResetFlags = ResetFlags.Vsync) {
			this.ResetFlags = ResetFlags;
			Text = Title;
			RenderWidth = W;
			RenderHeight = H;
			ClientSize = new Size(W, H);
			IsRunning = true;
		}

		public void Reset() {
			Reset(ClientSize.Width, ClientSize.Height);
		}

		public void Reset(int W, int H) {
			RenderWidth = W;
			RenderHeight = H;
			Bgfx.Reset(W, H, ResetFlags);
		}

		public void BgfxSetWindowHandle() {
			Bgfx.SetWindowHandle(Handle);
		}

		public void Run(Action<RenderWindow> RenderAction) {
			BgfxSetWindowHandle();
			RenderThread = new Thread(() => RenderAction(this));
			RenderThread.Start();
			Application.Run(this);
		}

		protected override void OnFormClosing(FormClosingEventArgs e) {
			IsRunning = false;
			RenderThread.Join();
			base.OnFormClosing(e);
		}
	}
}