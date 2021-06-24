using System.Collections.Generic;
using GaneshaDx.Common;

namespace GaneshaDx.Resources.ContentDataTypes.TextureAnimations {
	public class PaletteAnimation : TextureAnimation {
		public int OverriddenPaletteId;
		public int AnimationStartIndex;
		public int FrameCount = 2;
		public int FrameDuration = 10;

		public PaletteAnimation() { }

		public PaletteAnimation(List<byte> rawData) {
			string binary = Utilities.GetBinaryFromInt(rawData[0]);
			binary = binary.Substring(0, 4);
			OverriddenPaletteId = Utilities.GetIntFromBinary(binary);
			AnimationStartIndex = rawData[8];
			FrameCount = rawData[15];
			FrameDuration = rawData[17];
		}
	}
}