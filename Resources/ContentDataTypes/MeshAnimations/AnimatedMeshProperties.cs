using System.Collections.Generic;

namespace GaneshaDx.Resources.ContentDataTypes.MeshAnimations {
	public class AnimatedMeshProperties {
		public int LinkedParentMesh = 0;
		public int Unknown1;
		public int Unknown2;
		public int Unknown3;
		
		public List<byte> GetRawData() {
			return new List<byte> {
				(byte) LinkedParentMesh,
				(byte) Unknown1,
				(byte) Unknown2,
				(byte) Unknown3
			};
		}
	}
}