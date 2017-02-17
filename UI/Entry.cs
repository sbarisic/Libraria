using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using LibTech;
using LibTech.Rendering;
using Libraria.Rendering;
using Libraria.NanoVG;
using OpenTK;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using Libraria.Collections;

namespace UI {
	class MainMenuState : GameState {
		public override void Render(float Dt) {
			const float Offset = 20;
			
			NanoVG.DrawRectOutline(Color.DarkGray, 6.0f, Offset, Offset, NanoVG.Width - Offset * 2, NanoVG.Height - Offset * 2);

			NanoVG.SetFont("clacon", 24);
			NanoVG.DrawParagraph("^FF0000Red^00FF00Green^0000FFBlue^FFFFFFWhite\nRedGreenBlueWhite", 100, 100, 100);
		}
	}

	public class Entry : IModule {
		GameStateManager StateMgr;
		
		public void Open(IModule Client, IModule Server, IModule UI) {
			StateMgr = new GameStateManager();
			StateMgr.Push(new MainMenuState());
		}

		public void Close() {
		}

		public void Update(float Dt) {
			StateMgr.Update(Dt);
		}

		public void Render(float Dt) {
			NanoVG.BeginFrame();
			StateMgr.Render(Dt);
			NanoVG.EndFrame();
		}
	}
}