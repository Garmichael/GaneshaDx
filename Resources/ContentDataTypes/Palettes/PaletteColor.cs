using System;
using GaneshaDx.Common;
using Microsoft.Xna.Framework;

namespace GaneshaDx.Resources.ContentDataTypes.Palettes {
	public class PaletteColor {
		public int Red;
		public int Green;
		public int Blue;
		public bool IsTransparent;

		public PaletteColor(int red, int green, int blue, bool isTransparent) {
			Red = red;
			Green = green;
			Blue = blue;
			IsTransparent = isTransparent;
		}
		
		public PaletteColor(Color color) {
			FromColor(color);
		}

		public void FromColor(Color color) {
			const double ratio = 255f / 31f;
			Red = (int) Math.Round(color.R / ratio);
			Green = (int) Math.Round(color.G / ratio);
			Blue = (int) Math.Round(color.B / ratio);
			IsTransparent = color.A < 255;
		}

		public Color ToColor() {
			const double ratio = 255f / 31f;
			int red = (int) Math.Round(Red * ratio);
			int green = (int) Math.Round(Green * ratio);
			int blue = (int) Math.Round(Blue * ratio);

			return IsTransparent && red == 0 && green == 0 && blue == 0
				? Configuration.Properties.RenderAlphaAsSemiTransparent
					? new Color(128, 128, 128, 32)
					: new Color(0, 0, 0, 0)
				: new Color(red, green, blue, 255);
		}
	}
}