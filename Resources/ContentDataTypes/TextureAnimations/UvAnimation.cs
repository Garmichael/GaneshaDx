using System.Collections.Generic;
using GaneshaDx.Common;

namespace GaneshaDx.Resources.ContentDataTypes.TextureAnimations {
	public class UvAnimation : TextureAnimation {
		public int CanvasX = 8;
		public int CanvasY = 8;
		public int CanvasTexturePage;
		public int SizeWidth = 20;
		public int SizeHeight = 20;

		public int FirstFrameX = 28;
		public int FirstFrameY = 8;
		public int FirstFrameTexturePage;

		public int FrameCount = 2;
		public int FrameDuration = 10;

		public UvAnimationMode UvAnimationMode = UvAnimationMode.ForwardLooping;

		private int _unknownB;
		private int _unknownC;
		private int _unknownD;
		private int _unknownF;
		private int _unknownG;
		private int _unknownH;
		private int _unknownI;
		private int _unknownJ;
		private int _unknownK;

		public int PreviousFramesFrameId = 0;

		public UvAnimation() { }

		public UvAnimation(List<byte> rawData) {
			CanvasX = rawData[0] * 4;
			CanvasY = rawData[2];
			_unknownB = rawData[3];
			SizeWidth = rawData[4] * 4;
			_unknownC = rawData[5];
			SizeHeight = rawData[6];
			_unknownD = rawData[7];
			FirstFrameX = rawData[8] * 4;
			FirstFrameY = rawData[10];
			_unknownF = rawData[11];
			_unknownG = rawData[12];
			_unknownH = rawData[13];

			UvAnimationMode = rawData[14] switch {
				1 => UvAnimationMode.ForwardLooping,
				2 => UvAnimationMode.ForwardAndReverseLooping,
				5 => UvAnimationMode.ForwardOnceOnTrigger,
				21 => UvAnimationMode.ReverseOnceOnTrigger,
				_ => UvAnimationMode.Unknown
			};

			FrameCount = rawData[15];
			_unknownI = rawData[16];
			FrameDuration = rawData[17];
			_unknownJ = rawData[18];
			_unknownK = rawData[19];

			while (CanvasX >= 256) {
				CanvasX -= 256;
				CanvasTexturePage++;
			}

			while (FirstFrameX >= 256) {
				FirstFrameX -= 256;
				FirstFrameTexturePage++;
			}
		}
	}
}