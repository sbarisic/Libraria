using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

using LibTech;
using Libraria.Rendering;
using OpenTK;

namespace UI {
	public class Entry : IModule {
		IModule Client;

		//RenderObject Obj;
		TextLabel Obj;

		public void Init(params object[] Args) {
			Client = Args[0] as IModule;

			/*Obj = InitTriangle();
			Obj.SetTexture(Texture2D.Mask_Tex0);
			Obj.SetTexture(TestFont.FontAtlas, 1);*/

			ShaderProgram FontShader = new ShaderProgram("testgame\\shaders\\default.vert", "testgame\\shaders\\text.frag");

			Matrix4 ViewMatrix = Matrix4.CreateOrthographicOffCenter(0, 600, 0, 600 / Engine.RenderWindow.AspectRatio, -100, 100);
			FontShader.SetUniform("ViewMatrix", ViewMatrix);

			//GfxFont TestFont = new GfxFont("testgame\\fonts\\clacon.ttf");
			GfxFont TestFont = new GfxFont("C:\\Windows\\Fonts\\consola.ttf");

			Obj = new TextLabel(TestFont, FontShader);
			Obj.DrawText(new Vector2(10, 250), "I hate font rendering\nTest one two one two", Vector3.One);
			Obj.DrawText(new Vector2(10, 200), "Lazy potatoes", new Vector3(1, 0, 0));
			Obj.DrawText(new Vector2(10, 150), "The slow green snake", new Vector3(0, 1, 0));
		}

		public void Event(ModuleEvent Evt, params object[] Args) {
			if (Evt == ModuleEvent.RENDER) {
				Obj.Draw();
			}
		}

		RenderObject InitText(string Txt) { /////// aaaaaaaaaaaaaaah
			ShaderProgram FontShader = new ShaderProgram("testgame\\shaders\\default.vert", "testgame\\shaders\\msdf.frag");

			Matrix4 ViewMatrix = Matrix4.CreateOrthographicOffCenter(0, 600, 0, 600, -100, 100);
			FontShader.SetUniform("ViewMatrix", ViewMatrix);

			Dictionary<char, int> CharPos = new Dictionary<char, int>();
			{
				string[] FontDefs = File.ReadAllText("testgame\\fonts\\clacon.txt").Split('\n');

				for (int i = 0; i < FontDefs.Length; i++) {
					string[] FontDef = FontDefs[i].Trim().Split('=');
					if (FontDef.Length != 2)
						continue;

					CharPos.Add((char)int.Parse(FontDef[0]), int.Parse(FontDef[1]));
				}
			}

			RenderObject TextObj = new RenderObject(DrawPrimitiveType.Quads);
			TextObj.BindShader(FontShader);

			//TextObj.SetTexture(Texture2D.Mask_Tex0);
			//TextObj.SetTexture(Texture2D.FromFile("testgame\\fonts\\clacon.png"), 1);

			Texture2D FontAtlas = Texture2D.FromFile("testgame\\fonts\\clacon.png", TexFilterMode.Nearest, TexWrapMode.ClampToEdge, false);
			TextObj.SetTexture(FontAtlas);

			float CharSz = FontAtlas.Height;
			float CharW = CharSz / FontAtlas.Width;

			float PosX = 0;
			float PosY = 5;

			Vector3[] Verts = new Vector3[Txt.Length * 4];
			Vector3[] Cols = new Vector3[Verts.Length];
			Vector2[] UVs = new Vector2[Verts.Length];
			uint[] Inds = new uint[Verts.Length];

			for (int i = 0; i < Txt.Length; i++) {
				if (Txt[i] == ' ')
					PosX++;
				if (!CharPos.ContainsKey(Txt[i]))
					continue;

				Verts[i * 4 + 0] = new Vector3(PosX * CharSz, PosY * CharSz, 0);
				Verts[i * 4 + 1] = new Vector3(PosX * CharSz, PosY * CharSz + CharSz, 0);
				Verts[i * 4 + 2] = new Vector3(PosX * CharSz + CharSz, PosY * CharSz, 0);
				Verts[i * 4 + 3] = new Vector3(PosX * CharSz + CharSz, PosY * CharSz + CharSz, 0);

				Cols[i * 4 + 0] = new Vector3(1.0f, 0.0f, 0.0f);
				Cols[i * 4 + 1] = new Vector3(0.0f, 1.0f, 0.0f);
				Cols[i * 4 + 2] = new Vector3(0.0f, 0.0f, 1.0f);
				Cols[i * 4 + 3] = new Vector3(1.0f, 1.0f, 0.0f);

				float XOffs = (CharPos[Txt[i]] * CharSz) / FontAtlas.Width;

				UVs[i * 4 + 0] = new Vector2(XOffs, 1.0f);
				UVs[i * 4 + 1] = new Vector2(XOffs, 0.0f);
				UVs[i * 4 + 2] = new Vector2(XOffs + CharW, 1.0f);
				UVs[i * 4 + 3] = new Vector2(XOffs + CharW, 0.0f);

				Inds[i * 4 + 0] = (uint)(i * 4 + 0);
				Inds[i * 4 + 1] = (uint)(i * 4 + 1);
				Inds[i * 4 + 2] = (uint)(i * 4 + 3);
				Inds[i * 4 + 3] = (uint)(i * 4 + 2);

				PosX++;
			}

			TextObj.BindBuffer("vert_Vertex", new VertexBuffer().SetData(Verts));
			TextObj.BindBuffer("vert_Color", () => new DataBuffer(Vector3.SizeInBytes).SetData(Cols));
			TextObj.BindBuffer("vert_UV", () => new DataBuffer(Vector2.SizeInBytes).SetData(UVs));

			TextObj.BindIndexBuffer(new IndexBuffer().SetData(Inds));

			return TextObj;
		}

		RenderObject InitTriangle() {
			ShaderProgram TestShader = ShaderProgram.CreateProgram("testgame\\shaders\\default");

			Matrix4 ViewMatrix = Matrix4.CreateOrthographicOffCenter(0, Engine.RenderWindow.AspectRatio, 0, 1, -100, 100);
			TestShader.SetUniform("ViewMatrix", ViewMatrix);

			/*Camera Cam = new Camera();
			TestShader.SetUniform("ViewMatrix", Cam.Collapse());*/

			RenderObject Triangle = new RenderObject(DrawPrimitiveType.Quads);
			Triangle.BindShader(TestShader);

			Triangle.BindBuffer("vert_Vertex", new VertexBuffer().SetData(new Vector3[] {
				new Vector3(0.0f, 0.0f, 0), // 0, 0
				new Vector3(0.0f, 1.0f, 0), // 0, 1
				new Vector3(Engine.RenderWindow.AspectRatio, 0.0f, 0), // 1, 0
				new Vector3(Engine.RenderWindow.AspectRatio, 1.0f, 0), // 1, 1
			}));

			Triangle.BindBuffer("vert_Color", () => new DataBuffer(Vector3.SizeInBytes).SetData(new Vector3[] {
				new Vector3(1.0f, 0.0f, 0.0f),
				new Vector3(0.0f, 1.0f, 0.0f),
				new Vector3(0.0f, 0.0f, 1.0f),
				new Vector3(1.0f, 1.0f, 0.0f)
			}));

			Triangle.BindBuffer("vert_UV", () => new DataBuffer(Vector2.SizeInBytes).SetData(new Vector2[] {
				new Vector2(0.0f, 1.0f),
				new Vector2(0.0f, 0.0f),
				new Vector2(1.0f, 1.0f),
				new Vector2(1.0f, 0.0f)
			}));

			Triangle.BindIndexBuffer(new IndexBuffer().SetData(new uint[] { 0, 1, 3, 2 }));

			Triangle.SetTexture(Texture2D.FromFile("testgame\\textures\\mask.png"));
			Triangle.SetTexture(Texture2D.FromFile("testgame\\textures\\a.jpg"), 1);
			Triangle.SetTexture(Texture2D.FromFile("testgame\\textures\\b.jpg"), 2);
			Triangle.SetTexture(Texture2D.FromFile("testgame\\textures\\c.jpg"), 3);
			Triangle.SetTexture(Texture2D.FromFile("testgame\\textures\\d.jpg"), 4);

			return Triangle;
		}
	}
}