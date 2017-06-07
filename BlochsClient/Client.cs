using LibTech;
using Libraria.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibTech.Networking;
using System.Numerics;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using Matrix4 = OpenTK.Matrix4;
using Vec3 = OpenTK.Vector3;
using System.Linq.Expressions;
using System.Reflection;
using OpenTK.Input;

namespace ClientLib {
	class Chunk {
		const float BlockSize = 10;
		public int Size;
		public int[] Blocks;

		public ShaderProgram ChunkShader;
		RenderObject RObj;
		VertexBuffer Verts;
		IndexBuffer Inds;

		public Chunk() {
			Size = 24;
			Blocks = new int[Size * Size * Size];

			List<Vector3> Vertices = new List<Vector3>();
			List<uint> Indices = new List<uint>();

			GenerateBlock(Vertices, Indices, new Vector3(0, 0, 0));

			Verts = new VertexBuffer();
			Inds = new IndexBuffer();
			ChunkShader = ShaderProgram.CreateProgram("basegame\\shaders\\chunk");

			RObj = new RenderObject(DrawPrimitiveType.Triangles);
			RObj.BindShader(ChunkShader);
			RObj.BindBuffer("vert_Vertex", Verts.SetData(Vertices.ToArray(), VertexUsageHint.DynamicDraw));
			RObj.BindIndexBuffer(Inds.SetData(Indices.ToArray(), VertexUsageHint.DynamicDraw));
		}

		void GenerateBlock(List<Vector3> Verts, List<uint> Inds, Vector3 Pos) {
			// Front face
			Verts.Add(new Vector3(0, BlockSize, 0));
			Verts.Add(new Vector3(BlockSize, BlockSize, 0));
			Verts.Add(new Vector3(BlockSize, 0, 0));
			Verts.Add(new Vector3(0, 0, 0));

			Inds.AddRange(new uint[] { 0, 1, 2, 2, 3, 0 });

			/*// Back face
			Verts.Add(new Vector3(0, BlockSize, BlockSize));
			Verts.Add(new Vector3(BlockSize, BlockSize, BlockSize));
			Verts.Add(new Vector3(BlockSize, 0, BlockSize));
			Verts.Add(new Vector3(0, 0, BlockSize));*/

			/*// Left face
			Verts.Add(new Vector3(BlockSize, BlockSize, BlockSize));
			Verts.Add(new Vector3(BlockSize, BlockSize, 0));
			Verts.Add(new Vector3(BlockSize, 0, 0));
			Verts.Add(new Vector3(BlockSize, 0, BlockSize));*/
		}

		public void SetBlock(int X, int Y, int Z, int Block) {
			Blocks[X + Y * Size + Z * Size * Size] = Block;
		}

		public int GetBlock(int X, int Y, int Z) {
			return Blocks[X + Y * Size + Z * Size * Size];
		}

		public void Render() {
			RObj.Draw();
		}
	}

	class ClientGameWorld {
		Chunk Test;
		//Camera Cam;

		public ClientGameWorld() {
			Test = new Chunk();
			/*Cam = new Camera();
			Cam.Projection = Matrix4.CreatePerspectiveFieldOfView(90.0f / 180.0f * (float)Math.PI, Engine.RenderWindow.AspectRatio, 0.1f, 1000.0f);*/
		}

		public void EnterWorld() {
			Engine.Print("Entered world");
		}

		public void ExitWorld() {
			Engine.Print("Exited world");
		}

		public void Update(float Dt) {
			//Cam.MouseRotate(Dt, 1, 0);
		}

		public void Render(float Dt) {
			Test.ChunkShader.SetUniform("ViewMatrix", Engine.Camera.Collapse());
			Test.Render();
		}
	}

	public class Entry : ModuleBase {
		ClientGameWorld GameWorld = new ClientGameWorld();

		public override void Message(string MessageName, params object[] Args) {
			if (MessageName == nameof(ClientGameWorld.EnterWorld))
				GameWorld.EnterWorld();
			else if (MessageName == nameof(ClientGameWorld.ExitWorld))
				GameWorld.ExitWorld();

			base.Message(MessageName, Args);
		}

		public override void Open() {
		}

		public override void Update(float Dt) {
			const float MoveSpeed = 50;

			if (Input.Keyboard[Key.W]) {
				Engine.Camera.Move(Engine.Camera.GetForward() * MoveSpeed * Dt);
			}
			if (Input.Keyboard[Key.S]) {
				Engine.Camera.Move(-Engine.Camera.GetForward() * MoveSpeed * Dt);
			}
			if (Input.Keyboard[Key.A]) {
				Engine.Camera.Move(-Engine.Camera.GetRight() * MoveSpeed * Dt);
			}
			if (Input.Keyboard[Key.D]) {
				Engine.Camera.Move(Engine.Camera.GetRight() * MoveSpeed * Dt);
			}

			GameWorld.Update(Dt);
		}

		public override void Render(float Dt) {
			GameWorld.Render(Dt);
		}
	}
}