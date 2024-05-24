using GaneshaDx.Environment;
using GaneshaDx.Resources;
using Microsoft.Xna.Framework;

namespace GaneshaDx.Common;

public static class GameViewOverlay {
	public static void Render() {
		if (MapData.MapIsLoaded && Configuration.Properties.ShowGameViewOverlay) {
			Vector2 screenDimensions = new(256, 190);
			const int borderWidth = 1;
			Color borderColor = Color.White;

			float orthoDistortion = Configuration.Properties.RenderFftOrtho
				? 1
				: 5f / 4f;

			Vector2 center = new(Stage.ModelingViewport.Width / 2f, Stage.ModelingViewport.Height / 2f);
			float width = screenDimensions.X / (float) StageCamera.ZoomLevel;
			float height = screenDimensions.Y / (float) StageCamera.ZoomLevel * orthoDistortion;

			Rectangle screenRect = new() {
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
				new Rectangle(0, screenRect.Bottom, Stage.ModelingViewport.Width, Stage.ModelingViewport.Height),
				Color.White
			);

			Stage.SpriteBatch.Draw(
				UniversalTextures.WhiteTexture,
				new Rectangle(screenRect.Left - borderWidth, screenRect.Top - borderWidth, (int) width + borderWidth * 2, borderWidth),
				borderColor
			);

			Stage.SpriteBatch.Draw(
				UniversalTextures.WhiteTexture,
				new Rectangle(screenRect.Left - borderWidth, screenRect.Top - borderWidth, borderWidth, (int) height + borderWidth * 2),
				borderColor
			);
				
			Stage.SpriteBatch.Draw(
				UniversalTextures.WhiteTexture,
				new Rectangle(screenRect.Right, screenRect.Top - borderWidth, borderWidth, (int) height + borderWidth * 2),
				borderColor
			);

			Stage.SpriteBatch.Draw(
				UniversalTextures.WhiteTexture,
				new Rectangle(screenRect.Left - borderWidth, screenRect.Bottom, (int) width + borderWidth * 2, borderWidth),
				borderColor
			);
				
			Stage.SpriteBatch.End();
		}
	}
}