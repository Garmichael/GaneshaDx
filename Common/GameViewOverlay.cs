using GaneshaDx.Environment;
using GaneshaDx.Resources;
using Microsoft.Xna.Framework;

namespace GaneshaDx.Common {
	public static class GameViewOverlay {
		public static void Render() {
			if (MapData.MapIsLoaded && Configuration.Properties.ShowGameViewOverlay) {
				float orthoDistortion = Configuration.Properties.RenderFftOrtho
					? 1
					: 5f / 4f;
				
				Vector2 center = new Vector2(Stage.ModelingViewport.Width / 2f, Stage.ModelingViewport.Height / 2f);
				float width = 256 / (float) StageCamera.ZoomLevel;
				float height = 190 / (float) StageCamera.ZoomLevel * orthoDistortion;

				Rectangle screenRect = new Rectangle {
					X = (int) (center.X - width / 2),
					Y = (int) (center.Y - height / 2),
					Width = (int) width,
					Height = (int) height
				};

				Stage.SpriteBatch.Begin();
				
				Stage.SpriteBatch.Draw(
					UniversalTextures.SemiTransparentBlackTexture,
					new Rectangle(0, 0, Stage.ModelingViewport.Width, screenRect.Top)
					, Color.White
				);
				Stage.SpriteBatch.Draw(
					UniversalTextures.SemiTransparentBlackTexture,
					new Rectangle(0, screenRect.Top, screenRect.Left, (int) height),
					Color.White
				);
				Stage.SpriteBatch.Draw(
					UniversalTextures.SemiTransparentBlackTexture,
					new Rectangle(screenRect.Right, screenRect.Top, Stage.ModelingViewport.Width, (int) height),
					Color.White
				);
				Stage.SpriteBatch.Draw(
					UniversalTextures.SemiTransparentBlackTexture,
					new Rectangle(0, screenRect.Bottom, Stage.ModelingViewport.Width, (int) Stage.ModelingViewport.Height),
					Color.White
				);

				Stage.SpriteBatch.End();
			}
		}
	}
}