using System.Collections.Generic;
using GaneshaDx.Common;

namespace GaneshaDx.Resources.ContentDataTypes.MeshAnimations {
	public class AnimatedMeshInstructionSet {
		public readonly List<byte> RawData;
		public readonly List<AnimatedMeshInstruction> Instructions = new List<AnimatedMeshInstruction>();

		public AnimatedMeshInstructionSet() {
			const int totalInstructions = 16;

			for (int i = 0; i < totalInstructions; i++) {
				Instructions.Add(new AnimatedMeshInstruction());
			}
		}

		public AnimatedMeshInstructionSet(List<byte> rawData) {
			RawData = rawData;
			int currentBye = 0;
			for (int frameIndex = 0; frameIndex < 16; frameIndex++) {
				Instructions.Add(new AnimatedMeshInstruction {
					FrameStateId = rawData[currentBye],
					NextFrameId = rawData[currentBye + 1],
					Duration = Utilities.GetInt16FromLittleEndian(rawData[currentBye + 2], rawData[currentBye + 3])
				});
				currentBye += 4;
			}
		}

		public List<byte> GetRawData() {
			List<byte> rawData = new List<byte>();

			foreach (AnimatedMeshInstruction frame in Instructions) {
				rawData.Add((byte)frame.FrameStateId);
				rawData.Add((byte)frame.NextFrameId);
				rawData.Add(Utilities.GetLittleEndianFromInt16(frame.Duration).high);
				rawData.Add(Utilities.GetLittleEndianFromInt16(frame.Duration).low);
			}

			return rawData;
		}
	}
}