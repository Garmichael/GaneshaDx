using System.Collections.Generic;
using GaneshaDx.Common;
using Microsoft.Xna.Framework;

namespace GaneshaDx.Resources.ContentDataTypes.Palettes {
	public class Palette {
		public readonly List<PaletteColor> Colors = new List<PaletteColor>();

		public Vector4[] ShaderColors {
			get {
				Vector4[] paletteColors = new Vector4[Colors.Count];

				for (int colorIndex = 0; colorIndex < Colors.Count; colorIndex++) {
					paletteColors[colorIndex] = Colors[colorIndex].ToColor(true).ToVector4();
				}

				return paletteColors;
			}
		}

		public List<byte> GetRawData() {
			List<byte> rawData = new List<byte>();

			foreach (PaletteColor color in Colors) {
				string binary = "";

				binary += color.IsTransparent ? "0" : "1";
				binary += Utilities.GetBinaryFromInt(color.Blue, 5);
				binary += Utilities.GetBinaryFromInt(color.Green, 5);
				binary += Utilities.GetBinaryFromInt(color.Red, 5);

				byte high = (byte) Utilities.GetIntFromBinary(binary.Substring(0, 8));
				byte low = (byte) Utilities.GetIntFromBinary(binary.Substring(8, 8));

				rawData.Add(low);
				rawData.Add(high);
			}

			return rawData;
		}
	}
}