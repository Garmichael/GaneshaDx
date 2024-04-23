using System.Collections.Generic;
using GaneshaDx.Common;

namespace GaneshaDx.Resources.ContentDataTypes.TextureAnimations {
	public class PaletteAnimation : TextureAnimation {
		public int OverriddenPaletteId;
		public int AnimationStartIndex;
		public int FrameCount = 2;
		public int FrameDuration = 10;
		public PaletteAnimationMode AnimationMode = PaletteAnimationMode.ForwardLooping;
		
		public int Unknown0A;
		public int Unknown1;
		public int Unknown2 = 224;
		public int Unknown3 = 1;
		public int Unknown4;
		public int Unknown5;
		public int Unknown6;
		public int Unknown7;
		public int Unknown9;
		public int Unknown10;
		public int Unknown11;
		public int Unknown12;
		public int Unknown13;
		public int Unknown16;
		public int Unknown18;
		public int Unknown19;

		public PaletteAnimation() { }

		public PaletteAnimation(List<byte> rawData) {
			string binary = Utilities.GetBinaryFromInt(rawData[0]);
			OverriddenPaletteId = Utilities.GetIntFromBinary(binary.Substring(0, 4));
			Unknown0A = Utilities.GetIntFromBinary(binary.Substring(4, 4));
			Unknown1 = rawData[1];
			Unknown2 = rawData[2];
			Unknown3 = rawData[3];
			Unknown4 = rawData[4];
			Unknown5 = rawData[5];
			Unknown6 = rawData[6];
			Unknown7 = rawData[7];
			AnimationStartIndex = rawData[8];
			Unknown9 = rawData[9];
			Unknown10 = rawData[10];
			Unknown11 = rawData[11];
			Unknown12 = rawData[12];
			Unknown13 = rawData[13];
						
			AnimationMode = rawData[14] switch {
				3 => PaletteAnimationMode.ForwardLooping,
				4 => PaletteAnimationMode.ForwardAndReverseLooping,
				13 => PaletteAnimationMode.ForwardOnceOnTrigger,
				0 => PaletteAnimationMode.ForwardLoopingOnTrigger,
				_ => PaletteAnimationMode.Unknown
			};

			FrameCount = rawData[15];
			Unknown16 = rawData[16];
			FrameDuration = rawData[17];
			Unknown18 = rawData[18];
			Unknown19 = rawData[19];
		}

		public List<byte> GetRawData() {
			int animationMode = AnimationMode switch {
				PaletteAnimationMode.ForwardLooping => 3,
				PaletteAnimationMode.ForwardAndReverseLooping => 4,
				PaletteAnimationMode.ForwardOnceOnTrigger => 13,
				PaletteAnimationMode.ForwardLoopingOnTrigger => 0,
				_ => 0
			};
			
			string binary = Utilities.GetBinaryFromInt(OverriddenPaletteId, 4) + Utilities.GetBinaryFromInt(Unknown0A, 4);

			return new List<byte> {
				(byte) Utilities.GetIntFromBinary(binary),
				(byte) Unknown1,
				(byte) Unknown2,
				(byte) Unknown3,
				(byte) Unknown4,
				(byte) Unknown5,
				(byte) Unknown6,
				(byte) Unknown7,
				(byte) AnimationStartIndex,
				(byte) Unknown9,
				(byte) Unknown10,
				(byte) Unknown11,
				(byte) Unknown12,
				(byte) Unknown13,
				(byte) animationMode,
				(byte) FrameCount,
				(byte) Unknown16,
				(byte) FrameDuration,
				(byte) Unknown18,
				(byte) Unknown19,
			};
		}
	}
}