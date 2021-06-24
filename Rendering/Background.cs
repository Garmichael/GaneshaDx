using GaneshaDx.Environment;
using GaneshaDx.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GaneshaDx.Rendering {
	public static class Background {
		private static Texture2D _background;
		private static readonly Texture2D GaneshaLogo;
		private static readonly Texture2D StormGardenStudioLogo;

		static Background() {
			GaneshaLogo = Stage.Content.Load<Texture2D>("GaneshaLogo");
			StormGardenStudioLogo = Stage.Content.Load<Texture2D>("StormGardenStudioLogo");
		}

		public static void Render() {
			Stage.SpriteBatch.Begin();
			
			Stage.SpriteBatch.Draw(
				_background,
				MapData.MapIsLoaded
					? Stage.ModelingViewport.Bounds
					: Stage.GraphicsDevice.PresentationParameters.Bounds,
				Color.White
			);

			if (!MapData.MapIsLoaded) {
				DrawTitleScreen();
			}

			Stage.SpriteBatch.End();
		}

		private static void DrawTitleScreen() {
			Stage.SpriteBatch.Draw(
				GaneshaLogo,
				new Vector2(
					Stage.WholeViewport.Width / 2 - GaneshaLogo.Width / 2,
					Stage.WholeViewport.Height / 3 - GaneshaLogo.Height / 3
				),
				GaneshaLogo.Bounds,
				Color.White
			);

			Stage.SpriteBatch.Draw(
				StormGardenStudioLogo,
				new Vector2(
					Stage.WholeViewport.Width / 2 - StormGardenStudioLogo.Width / 2,
					Stage.WholeViewport.Height - StormGardenStudioLogo.Height - 10
				),
				GaneshaLogo.Bounds,
				Color.White
			);
		}

		public static void SetAsGradient(Color startColor, Color endColor) {
			int width = Stage.GraphicsDevice.PresentationParameters.Bounds.Width;
			int height = Stage.GraphicsDevice.PresentationParameters.Bounds.Height;
			int totalPixels = width * height;

			Texture2D backgroundTex = new Texture2D(Stage.GraphicsDevice, width, height);
			Color[] backgroundColors = new Color[height * width];

			for (int i = 0; i < backgroundColors.Length; i++) {
				double distanceDown = i == 0
					? 0
					: (double) i / totalPixels;

				Color gradientColor = Color.Lerp(startColor, endColor, (float) distanceDown);

				backgroundColors[i] = gradientColor;
			}

			backgroundTex.SetData(backgroundColors);
			_background = backgroundTex;
		}

		public static void SetAsSolid(Color backgroundColor) {
			Texture2D backgroundTex = new Texture2D(Stage.GraphicsDevice, 10, 10);
			Color[] backgroundColors = new Color[100];

			for (int i = 0; i < backgroundColors.Length; i++) {
				backgroundColors[i] = backgroundColor;
			}

			backgroundTex.SetData(backgroundColors);
			_background = backgroundTex;
		}

		public static void SetAsTexture(Texture2D texture2D) {
			_background = texture2D;
		}
	}
}