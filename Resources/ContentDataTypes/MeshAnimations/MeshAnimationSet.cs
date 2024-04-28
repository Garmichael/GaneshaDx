using System.Collections.Generic;

namespace GaneshaDx.Resources.ContentDataTypes.MeshAnimations {
	public class MeshAnimationSet {
		public readonly List<byte> KeyframesHeader = new List<byte>();
		public readonly List<MeshAnimationKeyframe> Keyframes = new List<MeshAnimationKeyframe>();

		public readonly List<byte> MeshInstructionSetsHeader = new List<byte>();
		public readonly List<AnimatedMeshInstructionSet> MeshInstructionSets = new List<AnimatedMeshInstructionSet>();

		public readonly List<byte> MeshPropertiesHeader = new List<byte>();
		public readonly List<AnimatedMeshProperties> MeshProperties = new List<AnimatedMeshProperties>();
		
		public readonly MeshAnimationUnknownData UnknownDataChunk;

		public MeshAnimationSet() {
			KeyframesHeader = new List<byte> { 1, 0, 0, 0, 128, 0, 0, 0 };
			MeshInstructionSetsHeader = new List<byte> { 2, 0, 0, 0, 16, 0, 64, 0 };
			MeshPropertiesHeader = new List<byte> { 3, 0, 0, 0, 64, 0, 0, 0 };

			Keyframes = new List<MeshAnimationKeyframe>();
			const int totalKeyFrames = 128;
			for (int i = 0; i < totalKeyFrames; i++) {
				Keyframes.Add(new MeshAnimationKeyframe());
			}

			MeshInstructionSets = new List<AnimatedMeshInstructionSet>();
			const int totalMeshAnimations = 64;

			for (int i = 0; i < totalMeshAnimations; i++) {
				MeshInstructionSets.Add(new AnimatedMeshInstructionSet());
			}

			MeshProperties = new List<AnimatedMeshProperties>();
			const int totalMeshProperties = 64;
			
			for (int i = 0; i < totalMeshProperties; i++) {
				MeshProperties.Add(new AnimatedMeshProperties());
			}

			const int totalUnknownChunkBytes = 4;
			UnknownDataChunk = new MeshAnimationUnknownData();
			for (int i = 0; i < totalUnknownChunkBytes; i++) {
				UnknownDataChunk.Data.Add(0);
			}
		}

		public MeshAnimationSet(List<byte> rawData) {
			const int headerLength = 8;
			const int totalKeyframes = 128;
			const int keyframePropertiesLength = 80;
			const int totalMeshInstructionSets = 64;
			const int meshAnimationInstructionLength = 64;

			int currentByteIndex = 0;

			while (KeyframesHeader.Count < headerLength) {
				KeyframesHeader.Add(rawData[currentByteIndex]);
				currentByteIndex++;
			}

			for (int instructionIndex = 0; instructionIndex < totalKeyframes; instructionIndex++) {
				List<byte> keyframeProperties = new List<byte>();
				for (int byteIndex = 0; byteIndex < keyframePropertiesLength; byteIndex++) {
					keyframeProperties.Add(rawData[currentByteIndex]);
					currentByteIndex++;
				}

				MeshAnimationKeyframe newKeyframe = new MeshAnimationKeyframe(keyframeProperties);
				Keyframes.Add(newKeyframe);
			}

			while (MeshInstructionSetsHeader.Count < headerLength) {
				MeshInstructionSetsHeader.Add(rawData[currentByteIndex]);
				currentByteIndex++;
			}

			for (int instructionIndex = 0; instructionIndex < totalMeshInstructionSets; instructionIndex++) {
				List<byte> meshInstructionProperties = new List<byte>();
				for (int byteIndex = 0; byteIndex < meshAnimationInstructionLength; byteIndex++) {
					meshInstructionProperties.Add(rawData[currentByteIndex]);
					currentByteIndex++;
				}

				MeshInstructionSets.Add(new AnimatedMeshInstructionSet(meshInstructionProperties));
			}

			while (MeshPropertiesHeader.Count < headerLength) {
				MeshPropertiesHeader.Add(rawData[currentByteIndex]);
				currentByteIndex++;
			}

			while (MeshProperties.Count < 64) {
				MeshProperties.Add(new AnimatedMeshProperties() {
					LinkedParentMesh = rawData[currentByteIndex],
					Unknown1 = rawData[currentByteIndex + 1],
					Unknown2 = rawData[currentByteIndex + 2],
					Unknown3 = rawData[currentByteIndex + 3]
				});

				currentByteIndex += 4;
			}

			UnknownDataChunk = new MeshAnimationUnknownData();
			while (currentByteIndex < rawData.Count) {
				UnknownDataChunk.Data.Add(rawData[currentByteIndex]);
				currentByteIndex++;
			}
		}
	}
}