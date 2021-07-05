using System.Collections.Generic;
using GaneshaDx.Common;

namespace GaneshaDx.Resources.ContentDataTypes.MeshAnimations {
	public class MeshAnimation {
		public readonly List<byte> RawData;
		public readonly List<MeshAnimationFrame> Frames = new List<MeshAnimationFrame>();

		public MeshAnimation(List<byte> rawData) {
			RawData = rawData;
			int currentBye = 0;
			for (int frameIndex = 0; frameIndex < 16; frameIndex++) {
				Frames.Add(new MeshAnimationFrame {
					FrameStateId = rawData[currentBye],
					NextFrameId = rawData[currentBye + 1],
					Duration = Utilities.GetIntFromLittleEndian(rawData[currentBye + 2], rawData[currentBye + 3])
				});
				currentBye += 4;
			}
		}

		public void GenerateRawData() { }
	}
}