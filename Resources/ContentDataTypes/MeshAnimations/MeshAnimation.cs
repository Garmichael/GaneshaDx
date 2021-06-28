using System.Collections.Generic;

namespace GaneshaDx.Resources.ContentDataTypes.MeshAnimations {
	public class MeshAnimation {
		public readonly List<byte> InstructionsHeader = new List<byte>();
		public readonly List<MeshAnimationInstruction> Instructions = new List<MeshAnimationInstruction>();
		
		public readonly List<byte> LinksHeader = new List<byte>();
		public readonly List<MeshAnimationLink> Links = new List<MeshAnimationLink>();
		
		public readonly List<byte> UnknownChunkHeader = new List<byte>();
		public readonly MeshAnimationUnknown UnknownChunk;
		
		public MeshAnimation(List<byte> rawData) {
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

				MeshAnimationInstruction newInstruction = new MeshAnimationInstruction(instructionData);
				Instructions.Add(newInstruction);
			}

			while (LinksHeader.Count < headerLength) {
				LinksHeader.Add(rawData[currentByteIndex]);
				currentByteIndex++;
			}

			for (int linkIndex = 0; linkIndex < totalLinks; linkIndex++) {
				List<byte> data = new List<byte>();
				for (int byteIndex = 0; byteIndex < linkLength; byteIndex++) {
					data.Add(rawData[currentByteIndex]);
					currentByteIndex++;
				}

				Links.Add(new MeshAnimationLink(data));
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