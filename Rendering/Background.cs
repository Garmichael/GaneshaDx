using System.Collections.Generic;
using GaneshaDx.Environment;
using GaneshaDx.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GaneshaDx.Rendering;

public static class Background {
	public static bool UseScreenshotBackground = false;
	public static int ScreenshotBackgroundIndex = 0;
	private static Texture2D _background;
	private static readonly Texture2D GaneshaLogo;
	private static readonly Texture2D StormGardenStudioLogo;
	private static SpriteFont _font;
	
	public static readonly Dictionary<string, Color> ScreenshotBackgroundColors = new() {
		{ "Magenta", Color.Magenta },
		{ "Lime Green", Color.Lime },
		{ "Yellow", Color.Yellow },
		{ "White", Color.White },
		{ "Black", Color.Black },
		{ "Sky Blue", Color.LightBlue },
	};
	
	private static readonly List<Texture2D> ScreenshotBackgroundTextures = new();

	static Background() {
		GaneshaLogo = Stage.Content.Load<Texture2D>("GaneshaLogo");
		StormGardenStudioLogo = Stage.Content.Load<Texture2D>("StormGardenStudioLogo");
		CreateScreenshotBackgrounds();
	}

	public static void Render() {
		Stage.SpriteBatch.Begin();

		Stage.SpriteBatch.Draw(
			UseScreenshotBackground
				? ScreenshotBackgroundTextures[ScreenshotBackgroundIndex]
				: _background,
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
		_font ??= Stage.Content.Load<SpriteFont>("OverlayFont");
		float versionNumberBrightness = .2f;
		Stage.SpriteBatch.DrawString(
			_font,
			"Ver. " + Program.Version,
			new Vector2(15, Stage.WholeViewport.Height - 30),
			new Color(versionNumberBrightness, versionNumberBrightness, versionNumberBrightness, 1)
		);

		Stage.SpriteBatch.Draw(
			GaneshaLogo,
			new Vector2(
				Stage.WholeViewport.Width / 2f - GaneshaLogo.Width / 2f,
				Stage.WholeViewport.Height / 3f - GaneshaLogo.Height / 3f
			),
			GaneshaLogo.Bounds,
			Color.White
		);

		Stage.SpriteBatch.Draw(
			StormGardenStudioLogo,
			new Vector2(
				Stage.WholeViewport.Width / 2f - StormGardenStudioLogo.Width / 2f,
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

		Texture2D backgroundTex = new(Stage.GraphicsDevice, width, height);
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
		Texture2D backgroundTex = new(Stage.GraphicsDevice, 10, 10);
		Color[] backgroundColors = new Color[100];

		for (int i = 0; i < backgroundColors.Length; i++) {
			backgroundColors[i] = backgroundColor;
		}

		backgroundTex.SetData(backgroundColors);
		_background = backgroundTex;
	}

	private static void CreateScreenshotBackgrounds() {
		foreach (KeyValuePair<string, Color> background in ScreenshotBackgroundColors) {
			Texture2D backgroundTex = new(Stage.GraphicsDevice, 10, 10);
			Color[] backgroundColors = new Color[100];

			for (int i = 0; i < backgroundColors.Length; i++) {
				backgroundColors[i] = background.Value;
			}

			backgroundTex.SetData(backgroundColors);
			ScreenshotBackgroundTextures.Add(backgroundTex);
		}
	}

	public static void SetAsTexture(Texture2D texture2D) {
		_background = texture2D;
	}
}