using System.Globalization;
using GaneshaDx.Environment;
using GaneshaDx.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GaneshaDx.Common {
	public static class FpsCounter {
		private const double MessageFrequency = 1.0f;
		private static double _frames;
		private static double _elapsed;
		private static double _last;
		private static double _now;
		private static string _message = "";
		private static SpriteFont _font;

		public static void Update() {
			_font ??= Stage.Content.Load<SpriteFont>("DebugFont");

			_now = Stage.GameTime.TotalGameTime.TotalSeconds;
			_elapsed = _now - _last;

			if (_elapsed > MessageFrequency) {
				_message = (_frames / _elapsed).ToString(CultureInfo.InvariantCulture).Substring(0, 4) + "fps";
				_elapsed = 0;
				_frames = 0;
				_last = _now;
			}
		}

		public static void DrawFps() {
			if (MapData.MapIsLoaded && Configuration.Properties.ShowFps) {
				Stage.SpriteBatch.Begin();
				Vector2 fpsDisplayPosition = new Vector2(10, Stage.Height - 25);
				Stage.SpriteBatch.DrawString(_font, _message, fpsDisplayPosition - new Vector2(1, -1), Color.Black);
				Stage.SpriteBatch.DrawString(_font, _message, fpsDisplayPosition, Color.White);
				Stage.SpriteBatch.End();
			}

			_frames++;
		}
	}
}