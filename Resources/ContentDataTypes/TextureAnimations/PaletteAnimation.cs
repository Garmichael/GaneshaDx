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

		public List<byte> GetRawData() {
			string binary = Utilities.GetBinaryFromInt(OverriddenPaletteId, 4) + "0000";

			return new List<byte> {
				(byte) Utilities.GetIntFromBinary(binary),
				0,
				224,
				1,
				0,
				0,
				0,
				0,
				(byte) AnimationStartIndex,
				0,
				0,
				0,
				0,
				0,
				3,
				(byte) FrameCount,
				0,
				(byte) FrameDuration,
				0,
				0
			};
		}
	}
}