using GaneshaDx.Environment;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GaneshaDx.Common {
	public static class UniversalTextures {
		public static readonly Texture2D GreyTexture;
		public static readonly Texture2D BlackTexture;
		public static readonly Texture2D HoveredBlackTexture;
		public static readonly Texture2D SelectedBlackTexture;
		public static readonly Texture2D GreenTexture;
		public static readonly Texture2D WhiteTexture;
		public static readonly Texture2D RedTexture;

		static UniversalTextures() {
			GreyTexture = MakeTexture(new Color(0.5f, 0.5f, 0.5f, 1));
			BlackTexture = MakeTexture(Color.Black);
			HoveredBlackTexture = MakeTexture(new Color(0.50f, 0.50f, 0.50f, 1f));
			SelectedBlackTexture = MakeTexture(new Color(0.75f, 0.75f, 0.75f, 1f));
			WhiteTexture = MakeTexture(Color.White);
			GreenTexture = MakeTexture(Color.LightGreen);
			RedTexture = MakeTexture(Color.Red);
		}

		private static Texture2D MakeTexture(Color color, int width = 1, int height = 1) {
			Texture2D texture2D = new Texture2D(Stage.GraphicsDevice, width, height);
			Color[] textureColor = new Color[width * height];
			for (int i = 0; i < width * height; i++) {
				textureColor[i] = color;
			}

			texture2D.SetData(textureColor);
			return texture2D;
		}
	}
}