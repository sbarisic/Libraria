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
using Vector3 = OpenTK.Vector3;
using Vector2 = OpenTK.Vector2;
using System.Linq.Expressions;
using System.Reflection;
using OpenTK.Input;
using LibTech.Other;

namespace ClientLib {
	static class VoxelHelper {
		public static void GenerateFace(List<Vector3> Verts, List<Vector3> Norms, Vector3 Normal, Vector3 A, Vector3 B, Vector3 C, Vector3 D) {
			for (int i = 0; i < 6; i++)
				Norms.Add(Normal);

			Verts.Add(A);
			Verts.Add(B);
			Verts.Add(C);
			Verts.Add(A);
			Verts.Add(C);
			Verts.Add(D);
		}
	}

	class Chunk {
		const float BlockSize = 10;
		public int Size;
		public int[] Blocks;

		public ShaderProgram ChunkShader;
		RenderObject RObj;

		public Chunk() {
			Size = 16;
			Blocks = new int[Size * Size * Size];

			List<Vector3> Vertices = new List<Vector3>();
			List<Vector3> Normals = new List<Vector3>();
			List<Vector2> UVs = new List<Vector2>();
			List<uint> Indices = new List<uint>();

			/*Random R = new Random();
			for (int i = 0; i < Blocks.Length; i++)
				if (R.Next(0, 100) > 60)
					Blocks[i] = 1;

			for (int x = 0; x < Size; x++)
				for (int y = 0; y < Size; y++)
					for (int z = 0; z < Size; z++)
						GenerateBlock(Vertices, Normals, UVs, new Vector3(x, y, z));*/

			//ObjTriangle[] Triangles = ObjLoader.Load("basegame\\objmdl\\dragon\\dragon.objmdl"); float Scale = 100;
			//ObjTriangle[] Triangles = ObjLoader.Load("basegame\\objmdl\\rugholt\\house.objmdl");
			//ObjTriangle[] Triangles = ObjLoader.Load("basegame\\objmdl\\lostempire\\lost_empire.objmdl"); float Scale = 10;
			ObjTriangle[] Triangles = ObjLoader.Load("basegame\\objmdl\\cube\\cube.objmdl"); float Scale = 50;
			
			foreach (var T in Triangles) {
				Vertices.Add(T.A * Scale);
				Vertices.Add(T.B * Scale);
				Vertices.Add(T.C * Scale);

				UVs.Add(T.A_UV);
				UVs.Add(T.B_UV);
				UVs.Add(T.C_UV);
			}

			ChunkShader = ShaderProgram.CreateProgram("basegame\\shaders\\chunk");

			RObj = new RenderObject(DrawPrimitiveType.Triangles);
			//RObj.SetTexture(Texture2D.FromFile("basegame\\objmdl\\rugholt\\house-RGB.png"));

			RObj.BindShader(ChunkShader);
			RObj.BindBuffer("vert_Vertex", () => new VertexBuffer().SetData(Vertices.ToArray()));

			if (Normals.Count > 0)
				RObj.BindBuffer("vert_Normal", () => DataBuffer.CreateFromData(Normals));

			if (UVs.Count > 0)
				RObj.BindBuffer("vert_UV", () => DataBuffer.CreateFromData(UVs));
			
			if (Indices.Count > 0)
				RObj.BindIndexBuffer(new IndexBuffer().SetData(Indices.ToArray()));
			else
				RObj.BindIndexBuffer(new IndexBuffer().SetSequence((uint)Vertices.Count));
		}

		void GenerateBlock(List<Vector3> Verts, List<Vector3> Norms, List<Vector2> UVs, Vector3 Pos) {
			if (GetBlock(Pos) == 0)
				return;


			Vector3 HBS = new Vector3(BlockSize / 2);
			Vector3 Origin = Pos * BlockSize - HBS;

			// Front face
			VoxelHelper.GenerateFace(Verts, Norms, new Vector3(0, 0, 1),
				Origin + new Vector3(1, 0, 1) * BlockSize,
				Origin + new Vector3(1, 1, 1) * BlockSize,
				Origin + new Vector3(0, 1, 1) * BlockSize,
				Origin + new Vector3(0, 0, 1) * BlockSize);

			// Back face
			VoxelHelper.GenerateFace(Verts, Norms, new Vector3(0, 0, -1),
				Origin + new Vector3(0, 1, 0) * BlockSize,
				Origin + new Vector3(1, 1, 0) * BlockSize,
				Origin + new Vector3(1, 0, 0) * BlockSize,
				Origin + new Vector3(0, 0, 0) * BlockSize);

			// Left face
			VoxelHelper.GenerateFace(Verts, Norms, new Vector3(-1, 0, 0),
				Origin + new Vector3(0, 0, 0) * BlockSize,
				Origin + new Vector3(0, 0, 1) * BlockSize,
				Origin + new Vector3(0, 1, 1) * BlockSize,
				Origin + new Vector3(0, 1, 0) * BlockSize);

			// Right face
			VoxelHelper.GenerateFace(Verts, Norms, new Vector3(1, 0, 0),
				Origin + new Vector3(1, 0, 0) * BlockSize,
				Origin + new Vector3(1, 1, 0) * BlockSize,
				Origin + new Vector3(1, 1, 1) * BlockSize,
				Origin + new Vector3(1, 0, 1) * BlockSize);

			// Top face
			VoxelHelper.GenerateFace(Verts, Norms, new Vector3(0, 1, 0),
				Origin + new Vector3(0, 1, 1) * BlockSize,
				Origin + new Vector3(1, 1, 1) * BlockSize,
				Origin + new Vector3(1, 1, 0) * BlockSize,
				Origin + new Vector3(0, 1, 0) * BlockSize);

			// Bottom face
			VoxelHelper.GenerateFace(Verts, Norms, new Vector3(0, -1, 0),
				Origin + new Vector3(1, 0, 0) * BlockSize,
				Origin + new Vector3(1, 0, 1) * BlockSize,
				Origin + new Vector3(0, 0, 1) * BlockSize,
				Origin + new Vector3(0, 0, 0) * BlockSize);
		}

		public void SetBlock(int X, int Y, int Z, int Block) {
			Blocks[X + Y * Size + Z * Size * Size] = Block;
		}

		public int GetBlock(Vector3 Pos) {
			return GetBlock((int)Pos.X, (int)Pos.Y, (int)Pos.Z);
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

		public void EnterWorld() {
			Engine.Print("Entered world");
		}

		public void ExitWorld() {
			Engine.Print("Exited world");
		}

		public void Update(float Dt) {
		}

		public void Render(float Dt) {
			if (Test == null)
				Test = new Chunk();

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