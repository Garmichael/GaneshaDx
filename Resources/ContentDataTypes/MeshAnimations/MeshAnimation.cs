﻿using System.Collections.Generic;
using GaneshaDx.Common;

namespace GaneshaDx.Resources.ContentDataTypes.MeshAnimations {
	public class MeshAnimation {
		public readonly List<byte> RawData;
		public readonly List<MeshAnimationFrame> Frames = new List<MeshAnimationFrame>();

		public MeshAnimation() {
			const int totalFrames = 16;

			for (int i = 0; i < totalFrames; i++) {
				Frames.Add(new MeshAnimationFrame());
			}
		}

		public MeshAnimation(List<byte> rawData) {
			RawData = rawData;
			int currentBye = 0;
			for (int frameIndex = 0; frameIndex < 16; frameIndex++) {
				Frames.Add(new MeshAnimationFrame {
					FrameStateId = rawData[currentBye],
					NextFrameId = rawData[currentBye + 1],
					Duration = Utilities.GetInt16FromLittleEndian(rawData[currentBye + 2], rawData[currentBye + 3])
				});
				currentBye += 4;
			}
		}

		public List<byte> GetRawData() {
			List<byte> rawData = new List<byte>();

			foreach (MeshAnimationFrame frame in Frames) {
				rawData.Add((byte)frame.FrameStateId);
				rawData.Add((byte)frame.NextFrameId);
				rawData.Add(Utilities.GetLittleEndianFromInt16(frame.Duration).high);
				rawData.Add(Utilities.GetLittleEndianFromInt16(frame.Duration).low);
			}

			return rawData;
		}
	}
}