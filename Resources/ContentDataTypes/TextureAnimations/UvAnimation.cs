using System.Collections.Generic;
using GaneshaDx.Common;

namespace GaneshaDx.Resources.ContentDataTypes.TextureAnimations;

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

	public int Unknown1 = 3;
	public int Unknown3;
	public int Unknown5;
	public int Unknown7;
	public int Unknown9 = 3;
	public int Unknown11;
	public int Unknown12;
	public int Unknown13;
	public int Unknown16;
	public int Unknown18;
	public int Unknown19;

	public UvAnimation() { }

	public UvAnimation(List<byte> rawData) {
		CanvasX = rawData[0] * 4;
		Unknown1 = rawData[1];
		CanvasY = rawData[2];
		Unknown3 = rawData[3];
		SizeWidth = rawData[4] * 4;
		Unknown5 = rawData[5];
		SizeHeight = rawData[6];
		Unknown7 = rawData[7];
		FirstFrameX = rawData[8] * 4;
		Unknown9 = rawData[9];
		FirstFrameY = rawData[10];
		Unknown11 = rawData[11];
		Unknown12 = rawData[12];
		Unknown13 = rawData[13];

		UvAnimationMode = rawData[14] switch {
			1 => UvAnimationMode.ForwardLooping,
			2 => UvAnimationMode.ForwardAndReverseLooping,
			5 => UvAnimationMode.ForwardOnceOnTrigger,
			6 => UvAnimationMode.ForwardOnceOnTrigger,
			7 => UvAnimationMode.ForwardOnceOnTrigger,
			8 => UvAnimationMode.ForwardOnceOnTrigger,
			21 => UvAnimationMode.ReverseOnceOnTrigger,
			23 => UvAnimationMode.ReverseOnceOnTrigger,
			24 => UvAnimationMode.ReverseOnceOnTrigger,
			0 => UvAnimationMode.Disabled,
			_ => UvAnimationMode.Unknown
		};

		FrameCount = rawData[15];
		Unknown16 = rawData[16];
		FrameDuration = rawData[17];
		Unknown18 = rawData[18];
		Unknown19 = rawData[19];

		while (CanvasX >= 256) {
			CanvasX -= 256;
			CanvasTexturePage++;
		}

		while (FirstFrameX >= 256) {
			FirstFrameX -= 256;
			FirstFrameTexturePage++;
		}
	}

	public List<byte> GetRawData() {
		int animationMode = UvAnimationMode switch {
			UvAnimationMode.ForwardLooping => 1,
			UvAnimationMode.ForwardAndReverseLooping => 2,
			UvAnimationMode.ForwardOnceOnTrigger => 5,
			UvAnimationMode.ReverseOnceOnTrigger => 21,
			_ => 0
		};

		return new List<byte> {
			(byte) ((CanvasX + CanvasTexturePage * 256) / 4f),
			(byte) Unknown1,
			(byte) CanvasY,
			(byte) Unknown3,
			(byte) (SizeWidth / 4f),
			(byte) Unknown5,
			(byte) SizeHeight,
			(byte) Unknown7,
			(byte) ((FirstFrameX + FirstFrameTexturePage * 256) / 4f),
			(byte) Unknown9,
			(byte) FirstFrameY,
			(byte) Unknown11,
			(byte) Unknown12,
			(byte) Unknown13,
			(byte) animationMode,
			(byte) FrameCount,
			(byte) Unknown16,
			(byte) FrameDuration,
			(byte) Unknown18,
			(byte) Unknown19
		};
	}
}