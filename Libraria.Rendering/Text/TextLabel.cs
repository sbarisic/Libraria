using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace Libraria.Rendering {
	public class TextLabel {
		bool Dirty;
		RenderObject RObj;

		List<Vector3> Verts, Colors;
		List<Vector2> UVs;
		List<uint> Inds;
		List<char> Characters;

		VertexBuffer VertsBuffer;
		DataBuffer ColorsBuffer;
		DataBuffer UVsBuffer;
		IndexBuffer IndsBuffer;

		GfxFont _Font;
		public GfxFont Font {
			get {
				return _Font;
			}
			set {
				_Font = value;
				Dirty = true;
			}
		}

		public TextLabel(GfxFont Fnt, ShaderProgram FontShader) {
			RObj = new RenderObject(DrawPrimitiveType.Quads);
			RObj.BindShader(FontShader);
			Font = Fnt;

			Verts = new List<Vector3>();
			Colors = new List<Vector3>();
			UVs = new List<Vector2>();
			Inds = new List<uint>();
			Characters = new List<char>();

			VertsBuffer = new VertexBuffer();
			ColorsBuffer = new DataBuffer(3, typeof(float));
			UVsBuffer = new DataBuffer(2, typeof(float));
			IndsBuffer = new IndexBuffer();
		}

		public void ClearText() {
			Characters.Clear();
			Verts.Clear();
			Colors.Clear();
			UVs.Clear();
			Inds.Clear();

			Dirty = true;
		}

		public void DrawText(Vector2 Position, string Text, Vector3 Color) {
			_Font.LoadChar(Text);

			if (_Font.Update()) { // Rebuild previous glyph UVs because the font may have relocated them on the atlas
				for (int i = 0; i < Characters.Count; i++) {
					char Chr = Characters[i];

					int UVIdx = i * 4;
					Vector2 GlyphUV;
					Vector2 GlyphSize;
					_Font.GetCharAtlasLocation(Chr, out GlyphUV, out GlyphSize);

					UVs[UVIdx] = GlyphUV + new Vector2(0, GlyphSize.Y);
					UVs[UVIdx + 1] = GlyphUV;
					UVs[UVIdx + 2] = GlyphUV + GlyphSize;
					UVs[UVIdx + 3] = GlyphUV + new Vector2(GlyphSize.X, 0);
				}
			}

			int Line = 0;
			float dPosX = 0;
			char C = (char)0;
			char CNext = (char)0;

			for (int i = 0; i < Text.Length; i++) {
				C = Text[i];
				if (i < Text.Length - 1)
					CNext = Text[i + 1];
				else
					CNext = (char)0;

				float X = dPosX - _Font.HalfPadding;
				float Y = Line * _Font.LineHeight - _Font.GetDescent(C) - _Font.HalfPadding;

				float W = _Font.GetCharWidth(C);
				float H = _Font.GetCharHeight(C);

				X = Position.X + X;
				Y = Position.Y - Y;

				Vector2 GlyphUV;
				Vector2 GlyphSize;
				_Font.GetCharAtlasLocation(C, out GlyphUV, out GlyphSize);

				{
					Characters.Add(C);

					Verts.AddRange(new Vector3[] {
						new Vector3(X, Y, 0),
						new Vector3(X, Y + H, 0),
						new Vector3(X + W, Y, 0),
						new Vector3(X + W, Y + H, 0),
					});

					Colors.AddRange(new Vector3[] { Color, Color, Color, Color });

					UVs.AddRange(new Vector2[] {
						GlyphUV + new Vector2(0, GlyphSize.Y),
						GlyphUV,
						GlyphUV + GlyphSize,
						GlyphUV + new Vector2(GlyphSize.X, 0),
					});

					int j = Inds.Count;
					Inds.AddRange(new uint[] { (uint)(j), (uint)(j + 1), (uint)(j + 3), (uint)(j + 2) });
				}

				if (C == '\n') {
					Line++;
					dPosX = 0;
				} else
					dPosX += W + _Font.GetKerning(C, CNext) - _Font.HalfPadding * 2;
			}

			Dirty = true;
		}

		void Update() {
			if (!Dirty)
				return;
			Dirty = false;

			RObj.Bind();
			RObj.BindBuffer("vert_Vertex", VertsBuffer.SetData(Verts.ToArray(), VertexUsageHint.DynamicDraw));
			RObj.BindBuffer("vert_Color", () => ColorsBuffer.SetData(Colors.ToArray(), VertexUsageHint.DynamicDraw));
			RObj.BindBuffer("vert_UV", () => UVsBuffer.SetData(UVs.ToArray(), VertexUsageHint.DynamicDraw));
			RObj.BindIndexBuffer(IndsBuffer.SetData(Inds.ToArray(), VertexUsageHint.DynamicDraw));

			RObj.SetTexture(_Font.FontAtlas);
		}

		public void Draw() {
			Update();
			RObj.Draw();
		}
	}
}