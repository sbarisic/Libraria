using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using LibTech;
using Libraria.Rendering;
using OpenTK;

namespace UI {
	public class Entry : IModule {
		IModule Client;

		RenderObject Triangle;

		public void Init(params object[] Args) {
			Client = Args[0] as IModule;

			ShaderProgram TestShader = ShaderProgram.CreateProgram("testgame\\shaders\\test");

			Matrix4 ViewMatrix = Matrix4.CreateOrthographicOffCenter(0, Engine.RenderWindow.AspectRatio, 0, 1, -100, 100);
			TestShader.SetUniform("ViewMatrix", ViewMatrix);

			VertexBuffer Verts = new VertexBuffer();
			Verts.SetData(new Vector3[] {
				new Vector3(0.0f, 0.0f, 0), // 0, 0
				new Vector3(0.0f, 1.0f, 0), // 0, 1
				new Vector3(Engine.RenderWindow.AspectRatio, 0.0f, 0), // 1, 0
				new Vector3(Engine.RenderWindow.AspectRatio, 1.0f, 0), // 1, 1
			});

			DataBuffer Clrs = new DataBuffer(Vector3.SizeInBytes);
			Clrs.SetData(new Vector3[] {
				new Vector3(1.0f, 0.0f, 0.0f),
				new Vector3(0.0f, 1.0f, 0.0f),
				new Vector3(0.0f, 0.0f, 1.0f),
				new Vector3(1.0f, 1.0f, 0.0f)
			});

			DataBuffer UVs = new DataBuffer(Vector2.SizeInBytes);
			UVs.SetData(new Vector2[] {
				new Vector2(0.0f, 1.0f),
				new Vector2(0.0f, 0.0f),
				new Vector2(1.0f, 1.0f),
				new Vector2(1.0f, 0.0f)
			});

			IndexBuffer Inds = new IndexBuffer();
			Inds.SetData(new uint[] { 0, 1, 3, 2 });

			Triangle = new RenderObject(DrawPrimitiveType.Quads);
			Triangle.BindShader(TestShader);
			Triangle.BindBuffer("vert_Vertex", Verts);
			Triangle.BindBuffer("vert_Color", Clrs);
			Triangle.BindBuffer("vert_UV", UVs);
			Triangle.BindIndexBuffer(Inds);

			Triangle.SetTexture(Texture2D.FromFile("testgame\\textures\\mask.png"));

			Triangle.SetTexture(Texture2D.FromFile("testgame\\textures\\a.jpg"), 1);
			Triangle.SetTexture(Texture2D.FromFile("testgame\\textures\\b.jpg"), 2);
			Triangle.SetTexture(Texture2D.FromFile("testgame\\textures\\c.jpg"), 3);
			Triangle.SetTexture(Texture2D.FromFile("testgame\\textures\\d.jpg"), 4);
		}

		public void Event(ModuleEvent Evt, params object[] Args) {
			if (Evt == ModuleEvent.RENDER) {
				Triangle.Draw();
			}
		}
	}
}