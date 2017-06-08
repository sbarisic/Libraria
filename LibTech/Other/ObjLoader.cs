using OpenTK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTech.Other {
	public struct ObjTriangle {
		public Vector3 A, B, C;
		public Vector2 A_UV, B_UV, C_UV;

		public static ObjTriangle operator +(ObjTriangle A, Vector3 B) {
			A.A += B;
			A.B += B;
			A.C += B;
			return A;
		}

		public static ObjTriangle operator *(ObjTriangle A, Vector3 B) {
			A.A *= B;
			A.B *= B;
			A.C *= B;
			return A;
		}

	}

	public static class ObjLoader {
		static void ParseFaceElement(string Element, out int VertInd, out int UVInd) {
			string[] ElementTokens = Element.Trim().Split('/');

			VertInd = int.Parse(ElementTokens[0]) - 1;

			UVInd = 0;
			if (ElementTokens[1].Length != 0)
				UVInd = int.Parse(ElementTokens[1]) - 1;
		}

		static void ParseFace(string[] Tokens, out int[] VertInds, out int[] UVInds) {
			VertInds = new int[Tokens.Length];
			UVInds = new int[Tokens.Length];

			for (int i = 0; i < VertInds.Length; i++)
				ParseFaceElement(Tokens[i], out VertInds[i], out UVInds[i]);
		}

		static float ParseFloat(string Str) {
			return float.Parse(Str, CultureInfo.InvariantCulture);
		}

		public static ObjTriangle[] Load(string[] Lines) {
			List<Vector3> Verts = new List<Vector3>();
			List<Vector2> UVs = new List<Vector2>();

			List<ObjTriangle> Tris = new List<ObjTriangle>();

			for (int i = 0; i < Lines.Length; i++) {
				string L = Lines[i].ToLower().Trim();

				while (L.Contains("  "))
					L = L.Replace("  ", " ");

				if (L.Length == 0 || L.StartsWith("#") || L == "\0")
					continue;

				string[] Tokens = L.Split(' ');


				switch (Tokens[0]) {
					case "v": {
							Verts.Add(new Vector3(ParseFloat(Tokens[1]), ParseFloat(Tokens[2]), ParseFloat(Tokens[3])));
							break;
						}

					case "vt": { // Texture coords
							UVs.Add(new Vector2(ParseFloat(Tokens[1]), ParseFloat(Tokens[2])));
							break;
						}

					case "vn": { // Vertex normals
							break;
						}

					case "f": { // Face
							int[] VertInds;
							int[] UVInds;

							ParseFace(Tokens.Skip(1).ToArray(), out VertInds, out UVInds);

							bool GenerateUVs = UVs.Count > 0;

							for (int j = 0; j < VertInds.Length; j++)
								if (VertInds[j] < 0) VertInds[j] = Verts.Count + VertInds[j] + 1;

							for (int j = 0; j < UVInds.Length; j++)
								if (UVInds[j] < 0) UVInds[j] = UVs.Count + UVInds[j] + 1;

							/*Tris.Add(Verts[VertInds[0] - 1]);
							Tris.Add(Verts[VertInds[1] - 1]);
							Tris.Add(Verts[VertInds[2] - 1]);*/

							if (VertInds.Length == 3) { // Triangles
								ObjTriangle T = new ObjTriangle();
								T.A = Verts[VertInds[0]];
								T.B = Verts[VertInds[1]];
								T.C = Verts[VertInds[2]];

								if (GenerateUVs) {
									T.A_UV = UVs[UVInds[0]];
									T.B_UV = UVs[UVInds[1]];
									T.C_UV = UVs[UVInds[2]];
								}
								Tris.Add(T);
							} else if (VertInds.Length == 4) { // Quads
								ObjTriangle T1 = new ObjTriangle();
								T1.A = Verts[VertInds[0]];
								T1.B = Verts[VertInds[1]];
								T1.C = Verts[VertInds[2]];

								if (GenerateUVs) {
									T1.A_UV = UVs[UVInds[0]];
									T1.B_UV = UVs[UVInds[1]];
									T1.C_UV = UVs[UVInds[2]];
								}
								Tris.Add(T1);

								ObjTriangle T2 = new ObjTriangle();
								T2.A = Verts[VertInds[2]];
								T2.B = Verts[VertInds[3]];
								T2.C = Verts[VertInds[0]];

								if (GenerateUVs) {
									T2.A_UV = UVs[UVInds[2]];
									T2.B_UV = UVs[UVInds[3]];
									T2.C_UV = UVs[UVInds[0]];
								}
								Tris.Add(T2);
							} else
								throw new NotImplementedException();
							break;
						}

					case "mtllib": {
							// TODO
							break;
						}

					case "usemtl": {
							break;
						}

					case "g": {
							break;
						}

					default:
						//Console.WriteLine("Unknown obj type: {0}", Tokens[0]);
						break;
				}
			}

			return Tris.ToArray();
		}

		public static ObjTriangle[] Load(string Path) {
			return Load(File.ReadAllLines(Path));
		}
	}
}
