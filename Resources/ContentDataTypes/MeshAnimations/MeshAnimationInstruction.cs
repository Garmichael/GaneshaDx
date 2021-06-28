using System.Collections.Generic;
using GaneshaDx.Common;

namespace GaneshaDx.Resources.ContentDataTypes.MeshAnimations {
	public class MeshAnimationInstruction {
		public readonly List<int> Properties = new List<int>();
		
		public MeshAnimationInstruction(List<byte> rawData) {
			for (int byteIndex = 0; byteIndex < rawData.Count; byteIndex += 2) {
				Properties.Add(Utilities.GetIntFromLittleEndian(rawData[byteIndex], rawData[byteIndex + 1]));
			}
		}
	}
}