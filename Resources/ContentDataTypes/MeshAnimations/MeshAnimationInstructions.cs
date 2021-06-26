using System.Collections.Generic;

namespace GaneshaDx.Resources.ContentDataTypes.MeshAnimations {
	public class MeshAnimationInstructions {
		public readonly List<byte> InstructionsHeader = new List<byte>();
		public readonly List<MeshAnimationInstruction> Instructions = new List<MeshAnimationInstruction>();
		
		public readonly List<byte> LinksHeader = new List<byte>();
		public readonly List<MeshAnimationLink> Links = new List<MeshAnimationLink>();
		
		public readonly List<byte> UnknownChunkHeader = new List<byte>();
		public MeshAnimationUnknown UnknownChunk;
		
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
				MeshAnimationInstruction newInstruction = new MeshAnimationInstruction();
				for (int byteIndex = 0; byteIndex < instructionLength; byteIndex++) {
					newInstruction.Data.Add(rawData[currentByteIndex]);
					currentByteIndex++;
				}

				Instructions.Add(newInstruction);
			}

			while (LinksHeader.Count < headerLength) {
				LinksHeader.Add(rawData[currentByteIndex]);
				currentByteIndex++;
			}

			for (int linkIndex = 0; linkIndex < totalLinks; linkIndex++) {
				MeshAnimationLink newLink = new MeshAnimationLink();
				for (int byteIndex = 0; byteIndex < linkLength; byteIndex++) {
					newLink.Data.Add(rawData[currentByteIndex]);
					currentByteIndex++;
				}

				Links.Add(newLink);
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