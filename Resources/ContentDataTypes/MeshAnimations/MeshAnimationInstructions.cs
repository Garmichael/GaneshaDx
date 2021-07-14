using System.Collections.Generic;

namespace GaneshaDx.Resources.ContentDataTypes.MeshAnimations {
	public class MeshAnimationInstructions {
		public readonly List<byte> InstructionsHeader = new List<byte>();
		public readonly List<MeshAnimationFrameState> FrameStates = new List<MeshAnimationFrameState>();
		
		public readonly List<byte> MeshAnimationsHeader = new List<byte>();
		public readonly List<MeshAnimation> MeshAnimations = new List<MeshAnimation>();
		
		public readonly List<byte> UnknownChunkHeader = new List<byte>();
		public readonly MeshAnimationUnknown UnknownChunk;
		
		public MeshAnimationInstructions(List<byte> rawData) {
			const int headerLength = 8;
			const int totalInstructions = 128;
			const int instructionLength = 80;
			const int totalLinks = 64;
			const int linkLength = 64;

			int currentByteIndex = 0;

			while (InstructionsHeader.Count < headerLength) {
				InstructionsHeader.Add(rawData[currentByteIndex]);
				currentByteIndex++;
			}

			for (int instructionIndex = 0; instructionIndex < totalInstructions; instructionIndex++) {
				List<byte> instructionData = new List<byte>();
				for (int byteIndex = 0; byteIndex < instructionLength; byteIndex++) {
					instructionData.Add(rawData[currentByteIndex]);
					currentByteIndex++;
				}

				MeshAnimationFrameState newFrameState = new MeshAnimationFrameState(instructionData);
				FrameStates.Add(newFrameState);
			}

			while (MeshAnimationsHeader.Count < headerLength) {
				MeshAnimationsHeader.Add(rawData[currentByteIndex]);
				currentByteIndex++;
			}

			for (int linkIndex = 0; linkIndex < totalLinks; linkIndex++) {
				List<byte> data = new List<byte>();
				for (int byteIndex = 0; byteIndex < linkLength; byteIndex++) {
					data.Add(rawData[currentByteIndex]);
					currentByteIndex++;
				}

				MeshAnimations.Add(new MeshAnimation(data));
			}

			while (UnknownChunkHeader.Count < headerLength) {
				UnknownChunkHeader.Add(rawData[currentByteIndex]);
				currentByteIndex++;
			}

			UnknownChunk = new MeshAnimationUnknown();
			while (currentByteIndex < rawData.Count) {
				UnknownChunk.Data.Add(rawData[currentByteIndex]);
				currentByteIndex++;
			}
		}
	}
}