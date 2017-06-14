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

using INDEX_TYPE = System.Single;

namespace ClientLib {
	static class VoxelHelper {
		public static void GenerateFace(List<Vector3> Verts, List<Vector3> Norms, List<Vector2> UVs, List<INDEX_TYPE> TexIDs, INDEX_TYPE TexID,
			Vector3 Normal,
			Vector3 A, Vector2 UVA,
			Vector3 B, Vector2 UVB,
			Vector3 C, Vector2 UVC,
			Vector3 D, Vector2 UVD) {

			for (int i = 0; i < 6; i++) {
				Norms.Add(Normal);
				TexIDs.Add(TexID);
			}

			Verts.Add(A);
			Verts.Add(B);
			Verts.Add(C);
			Verts.Add(A);
			Verts.Add(C);
			Verts.Add(D);

			UVs.Add(UVA);
			UVs.Add(UVB);
			UVs.Add(UVC);
			UVs.Add(UVA);
			UVs.Add(UVC);
			UVs.Add(UVD);
		}
	}

	class Chunk {
		const float BlockSize = 10;
		public int Size;
		public int[] Blocks;

		public ShaderProgram ChunkShader;
		RenderObject RObj;

		List<Texture2D> TexList = new List<Texture2D>();

		public Chunk() {
			Size = 24;
			Blocks = new int[Size * Size * Size];

			List<Vector3> Vertices = new List<Vector3>();
			List<Vector3> Normals = new List<Vector3>();
			List<Vector2> UVs = new List<Vector2>();
			List<uint> Indices = new List<uint>();
			List<INDEX_TYPE> TexIDs = new List<INDEX_TYPE>();

			Random R = new Random();
			for (int i = 0; i < Blocks.Length; i++)
				if (R.Next(0, 100) > 60)
					Blocks[i] = 1;

			int C = 0;
			for (int i = 0; i < Blocks.Length; i++) {
				if (Blocks[i] != 0) C++;
			}
			LibTech.Console.WriteLine("{0} blocks {1} vertices {2} normals", C, 6 * 6 * C, 4 * 6 * C);

			for (int x = 0; x < Size; x++)
				for (int y = 0; y < Size; y++)
					for (int z = 0; z < Size; z++)
						GenerateBlock(Vertices, Normals, UVs, TexIDs, new Vector3(x, y, z));

			ChunkShader = ShaderProgram.CreateProgram("basegame\\shaders\\chunk");
			for (int i = 0; i < 16; i++) {
				Texture2D Tex = Texture2D.FromFile("basegame\\textures\\blocks\\cloth_" + i + ".png", TexFilterMode.Nearest);
				Tex.Resident = true;
				ChunkShader.SetUniform("Textures", i, Tex.TextureHandle);

				TexList.Add(Tex);
			}

			RObj = new RenderObject(DrawPrimitiveType.Triangles);
			//RObj.SetTexture(Texture2D.FromFile("basegame\\objmdl\\rugholt\\house-RGB.png"));

			RObj.BindShader(ChunkShader);
			if (!RObj.BindBuffer("vert_Vertex", () => new VertexBuffer().SetData(Vertices.ToArray())))
				throw new Exception();

			if (!RObj.BindBuffer("vert_Normal", () => DataBuffer.CreateFromData(Normals)))
				throw new Exception();

			if (!RObj.BindBuffer("vert_UV", () => DataBuffer.CreateFromData(UVs)))
				throw new Exception();

			if (!RObj.BindBuffer("vert_TextureID", () => DataBuffer.CreateFromData(TexIDs)))
				throw new Exception();

			if (Indices.Count > 0)
				RObj.BindIndexBuffer(new IndexBuffer().SetData(Indices.ToArray()));
			else
				RObj.BindIndexBuffer(new IndexBuffer().SetSequence((uint)Vertices.Count));
		}

		static Random Rnd = new Random();

		void GenerateBlock(List<Vector3> Verts, List<Vector3> Norms, List<Vector2> UVs, List<INDEX_TYPE> TexIDs, Vector3 Pos) {
			if (GetBlock(Pos) == 0)
				return;

			INDEX_TYPE T_Front = Rnd.Next(0, 16);
			INDEX_TYPE T_Back = Rnd.Next(0, 16);
			INDEX_TYPE T_Left = Rnd.Next(0, 16);
			INDEX_TYPE T_Right = Rnd.Next(0, 16);
			INDEX_TYPE T_Top = Rnd.Next(0, 16);
			INDEX_TYPE T_Bottom = Rnd.Next(0, 16);

			Vector3 HBS = new Vector3(BlockSize / 2);
			Vector3 Origin = Pos * BlockSize - HBS;

			// Front face
			VoxelHelper.GenerateFace(Verts, Norms, UVs, TexIDs, T_Front, new Vector3(0, 0, 1),
				Origin + new Vector3(1, 0, 1) * BlockSize, new Vector2(0, 0),
				Origin + new Vector3(1, 1, 1) * BlockSize, new Vector2(1, 0),
				Origin + new Vector3(0, 1, 1) * BlockSize, new Vector2(1, 1),
				Origin + new Vector3(0, 0, 1) * BlockSize, new Vector2(0, 1));

			// Back face
			VoxelHelper.GenerateFace(Verts, Norms, UVs, TexIDs, T_Back, new Vector3(0, 0, -1),
				Origin + new Vector3(0, 1, 0) * BlockSize, new Vector2(0, 0),
				Origin + new Vector3(1, 1, 0) * BlockSize, new Vector2(1, 0),
				Origin + new Vector3(1, 0, 0) * BlockSize, new Vector2(1, 1),
				Origin + new Vector3(0, 0, 0) * BlockSize, new Vector2(0, 1));

			// Left face
			VoxelHelper.GenerateFace(Verts, Norms, UVs, TexIDs, T_Left, new Vector3(-1, 0, 0),
				Origin + new Vector3(0, 0, 0) * BlockSize, new Vector2(0, 0),
				Origin + new Vector3(0, 0, 1) * BlockSize, new Vector2(1, 0),
				Origin + new Vector3(0, 1, 1) * BlockSize, new Vector2(1, 1),
				Origin + new Vector3(0, 1, 0) * BlockSize, new Vector2(0, 1));

			// Right face
			VoxelHelper.GenerateFace(Verts, Norms, UVs, TexIDs, T_Right, new Vector3(1, 0, 0),
				Origin + new Vector3(1, 0, 0) * BlockSize, new Vector2(0, 0),
				Origin + new Vector3(1, 1, 0) * BlockSize, new Vector2(1, 0),
				Origin + new Vector3(1, 1, 1) * BlockSize, new Vector2(1, 1),
				Origin + new Vector3(1, 0, 1) * BlockSize, new Vector2(0, 1));

			// Top face
			VoxelHelper.GenerateFace(Verts, Norms, UVs, TexIDs, T_Top, new Vector3(0, 1, 0),
				Origin + new Vector3(0, 1, 1) * BlockSize, new Vector2(0, 0),
				Origin + new Vector3(1, 1, 1) * BlockSize, new Vector2(1, 0),
				Origin + new Vector3(1, 1, 0) * BlockSize, new Vector2(1, 1),
				Origin + new Vector3(0, 1, 0) * BlockSize, new Vector2(0, 1));

			// Bottom face
			VoxelHelper.GenerateFace(Verts, Norms, UVs, TexIDs, T_Bottom, new Vector3(0, -1, 0),
				Origin + new Vector3(1, 0, 0) * BlockSize, new Vector2(0, 0),
				Origin + new Vector3(1, 0, 1) * BlockSize, new Vector2(1, 0),
				Origin + new Vector3(0, 0, 1) * BlockSize, new Vector2(1, 1),
				Origin + new Vector3(0, 0, 0) * BlockSize, new Vector2(0, 1));
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