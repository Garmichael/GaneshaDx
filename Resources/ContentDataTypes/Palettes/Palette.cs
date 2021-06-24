using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GaneshaDx.Resources.ContentDataTypes.Palettes {
	public class Palette {
		public Vector4[] ShaderColors {
			get {
				Vector4[] paletteColors = new Vector4[Colors.Count];

				for (int colorIndex = 0; colorIndex < Colors.Count; colorIndex++) {
					paletteColors[colorIndex] = Colors[colorIndex].ToColor().ToVector4();
				}

				return paletteColors;
			}
		}
		public readonly List<PaletteColor> Colors = new List<PaletteColor>();
	}
}