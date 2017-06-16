using LibTech;
using Libraria.Rendering;
using System;
using Libraria;
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
using Console = LibTech.Console;

using INDEX_TYPE = System.Single;

namespace ClientLib {
	static class VoxelHelper {
		public static void GenerateFace(List<Vector3> Verts, List<Vector2> UVs, List<INDEX_TYPE> TexIDs, INDEX_TYPE TexID,
			//Vector3 Normal,
			Vector3 A, Vector2 UVA,
			Vector3 B, Vector2 UVB,
			Vector3 C, Vector2 UVC,
			Vector3 D, Vector2 UVD) {

			for (int i = 0; i < 6; i++) {
				//Norms.Add(Normal);
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

	struct JSONVoxels {
		public struct DimensionElement {
			public int width;
			public int height;
			public int depth;
		}

		public struct VoxelElement {
			public string id;
			public int x;
			public int y;
			public int z;
		}

		public DimensionElement[] dimension;
		public VoxelElement[] voxels;
	}

	class Chunk {
		const float BlockSize = 10;
		public int XSize, YSize, ZSize;

		public int[] Blocks;

		public ShaderProgram ChunkShader;
		RenderObject RObj;

		List<Texture2D> TexList = new List<Texture2D>();

		public Chunk() {
			List<Vector3> Vertices = new List<Vector3>();
			List<Vector2> UVs = new List<Vector2>();
			List<uint> Indices = new List<uint>();
			List<INDEX_TYPE> TexIDs = new List<INDEX_TYPE>();

			JSONVoxels Torus = Libraria.Serialization.JSON.Deserialize<JSONVoxels>(Files.ReadAllText("torus.json"));
			XSize = Torus.dimension[0].width + 1;
			YSize = Torus.dimension[0].height + 1;
			ZSize = Torus.dimension[0].depth + 1;

			//XSize = YSize = ZSize = 24;
			Blocks = new int[XSize * YSize * ZSize];

			for (int i = 0; i < Torus.voxels.Length; i++) {
				JSONVoxels.VoxelElement E = Torus.voxels[i];
				SetBlock(E.x, E.y, E.z, 24);
			}

			/*FastNoise Noise = new FastNoise();
			Noise.SetNoiseType(FastNoise.NoiseType.Simplex);
			Noise.SetFrequency(0.01f);

			XSize = YSize = ZSize = 60;
			Blocks = new int[XSize * YSize * ZSize];

			for (int X = 0; X < XSize; X++) {
				for (int Y = 0; Y < YSize; Y++) {
					for (int Z = 0; Z < ZSize; Z++) {
						float Weight = ((float)Y / YSize) * 2 - 1;

						SetBlock(X, Y, Z, Noise.GetNoise(X, Z, Y) > Weight ? 24 : 0);
					}
				}
			}*/

			///////////////////////

			int C = 0;
			for (int i = 0; i < Blocks.Length; i++) {
				if (Blocks[i] != 0) C++;
			}
			Console.WriteLine("{0} blocks {1} vertices {2} triangles", C, 6 * 6 * C, (6 * 6 * C) / 3);

			for (int x = 0; x < XSize; x++)
				for (int y = 0; y < YSize; y++)
					for (int z = 0; z < ZSize; z++)
						GenerateBlock(Vertices, UVs, TexIDs, new Vector3(x, y, z));

			ChunkShader = ShaderProgram.CreateProgram("basegame\\shaders\\chunk");


			string[] TextureFiles = Files.GetFilesInDirectory("textures\\blocks", SearchPattern: "*.png");
			int I = 0;

			foreach (var TFile in TextureFiles) {
				Texture2D Tex = Texture2D.FromFile(Files.GetFullPath(TFile), GenerateMipmap: false, UseSRGBA: true);
				Tex.SetFilterMode(TexFilterMode.Nearest, TexFilterMode.LinearMipmapLinear);
				Tex.GenerateMipmap();
				Tex.Resident = true;
				TexList.Add(Tex);

				ChunkShader.SetUniform("Textures", I++, Tex.TextureHandle);
			}
			
			RObj = new RenderObject(DrawPrimitiveType.Triangles);
			//RObj.SetTexture(Texture2D.FromFile("basegame\\objmdl\\rugholt\\house-RGB.png"));

			RObj.BindShader(ChunkShader);
			if (!RObj.BindBuffer("in_Vertex", () => new VertexBuffer().SetData(Vertices.ToArray())))
				throw new Exception();

			/*if (!RObj.BindBuffer("in_Normal", () => DataBuffer.CreateFromData(Normals)))
				throw new Exception();*/

			if (!RObj.BindBuffer("in_UV", () => DataBuffer.CreateFromData(UVs)))
				throw new Exception();

			if (!RObj.BindBuffer("in_TextureID", () => DataBuffer.CreateFromData(TexIDs)))
				throw new Exception();

			if (Indices.Count > 0)
				RObj.BindIndexBuffer(new IndexBuffer().SetData(Indices.ToArray()));
			else
				RObj.BindIndexBuffer(new IndexBuffer().SetSequence((uint)Vertices.Count));
		}

		static Random Rnd = new Random();

		void GenerateBlock(List<Vector3> Verts, List<Vector2> UVs, List<INDEX_TYPE> TexIDs, Vector3 Pos) {
			int BlockType = 0;
			if ((BlockType = GetBlock(Pos)) == 0)
				return;


			/*INDEX_TYPE T_Left = 0;
			INDEX_TYPE T_Right = 1;
			INDEX_TYPE T_Front = 2;
			INDEX_TYPE T_Back = 3;
			INDEX_TYPE T_Top = 4;
			INDEX_TYPE T_Bottom = 5;*/

			INDEX_TYPE T_Left = BlockType;
			INDEX_TYPE T_Right = BlockType;
			INDEX_TYPE T_Front = BlockType;
			INDEX_TYPE T_Back = BlockType;
			INDEX_TYPE T_Top = BlockType;
			INDEX_TYPE T_Bottom = BlockType;

			Vector3 HBS = new Vector3(BlockSize / 2);
			Vector3 Origin = Pos * BlockSize - HBS;

			// Front face
			VoxelHelper.GenerateFace(Verts, UVs, TexIDs, T_Front,
				Origin + new Vector3(0, 1, 0) * BlockSize, new Vector2(1, 0),
				Origin + new Vector3(1, 1, 0) * BlockSize, new Vector2(0, 0),
				Origin + new Vector3(1, 0, 0) * BlockSize, new Vector2(0, 1),
				Origin + new Vector3(0, 0, 0) * BlockSize, new Vector2(1, 1));

			// Back face
			VoxelHelper.GenerateFace(Verts, UVs, TexIDs, T_Back,
				Origin + new Vector3(1, 0, 1) * BlockSize, new Vector2(1, 1),
				Origin + new Vector3(1, 1, 1) * BlockSize, new Vector2(1, 0),
				Origin + new Vector3(0, 1, 1) * BlockSize, new Vector2(0, 0),
				Origin + new Vector3(0, 0, 1) * BlockSize, new Vector2(0, 1));

			// Left face
			VoxelHelper.GenerateFace(Verts, UVs, TexIDs, T_Left,
				Origin + new Vector3(0, 0, 0) * BlockSize, new Vector2(0, 1),
				Origin + new Vector3(0, 0, 1) * BlockSize, new Vector2(1, 1),
				Origin + new Vector3(0, 1, 1) * BlockSize, new Vector2(1, 0),
				Origin + new Vector3(0, 1, 0) * BlockSize, new Vector2(0, 0));

			// Right face
			VoxelHelper.GenerateFace(Verts, UVs, TexIDs, T_Right,
				Origin + new Vector3(1, 0, 0) * BlockSize, new Vector2(1, 1),
				Origin + new Vector3(1, 1, 0) * BlockSize, new Vector2(1, 0),
				Origin + new Vector3(1, 1, 1) * BlockSize, new Vector2(0, 0),
				Origin + new Vector3(1, 0, 1) * BlockSize, new Vector2(0, 1));

			// Top face
			VoxelHelper.GenerateFace(Verts, UVs, TexIDs, T_Top,
				Origin + new Vector3(0, 1, 1) * BlockSize, new Vector2(0, 1),
				Origin + new Vector3(1, 1, 1) * BlockSize, new Vector2(1, 1),
				Origin + new Vector3(1, 1, 0) * BlockSize, new Vector2(1, 0),
				Origin + new Vector3(0, 1, 0) * BlockSize, new Vector2(0, 0));

			// Bottom face
			VoxelHelper.GenerateFace(Verts, UVs, TexIDs, T_Bottom,
				Origin + new Vector3(1, 0, 0) * BlockSize, new Vector2(1, 1),
				Origin + new Vector3(1, 0, 1) * BlockSize, new Vector2(1, 0),
				Origin + new Vector3(0, 0, 1) * BlockSize, new Vector2(0, 0),
				Origin + new Vector3(0, 0, 0) * BlockSize, new Vector2(0, 1));
		}

		public int XYZToIdx(int X, int Y, int Z) {
			if (X >= XSize || X < 0) throw new Exception("X index invalid");
			if (Y >= YSize || Y < 0) throw new Exception("Y index invalid");
			if (Z >= ZSize || Z < 0) throw new Exception("Z index invalid");

			return X + XSize * (Y + YSize * Z);
		}

		public void IdxToXYZ(int Idx, out int X, out int Y, out int Z) {
			Z = Idx / (XSize * YSize);
			Idx -= (Z * XSize * YSize);
			Y = Idx / XSize;
			X = Idx % XSize;
		}

		public void SetBlock(int X, int Y, int Z, int Block) {
			Blocks[XYZToIdx(X, Y, Z)] = Block;
		}

		public int GetBlock(Vector3 Pos) {
			return GetBlock((int)Pos.X, (int)Pos.Y, (int)Pos.Z);
		}

		public int GetBlock(int X, int Y, int Z) {
			return Blocks[XYZToIdx(X, Y, Z)];
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