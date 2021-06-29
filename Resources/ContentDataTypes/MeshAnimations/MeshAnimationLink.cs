using System.Collections.Generic;

namespace GaneshaDx.Resources.ContentDataTypes.MeshAnimations {
	public class MeshAnimationLink {
		public List<byte> RawData;

		public MeshAnimationLink(List<byte> rawData) {
			RawData = rawData;
		}

		public void GenerateRawData() { }
	}
}