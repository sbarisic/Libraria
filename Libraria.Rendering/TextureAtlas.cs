using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Libraria.Rendering {
	public class TextureAtlas {
		public static TextureAtlas CreateFrom(string[] ImageFiles) {
			/*List<Image> Images = new List<Image>();

			for (int i = 0; i < ImageFiles.Length; i++) {
				Image I = Image.FromFile(ImageFiles[i]);

				if (I.Width != 32 || I.Height != 32)
					I.Dispose();
				else
					Images.Add(I);
			}

			int CW = Images.Count / 2;
			int CH = Images.Count / 2;

			//Bitmap AtlasImage = new Bitmap()*/



			TextureAtlas Atlas = new TextureAtlas();
			return Atlas;
		}

		public TextureAtlas() {

		}
	}
}