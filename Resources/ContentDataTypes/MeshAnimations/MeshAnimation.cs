using System.Collections.Generic;

namespace GaneshaDx.Resources.ContentDataTypes.MeshAnimations {
	public class MeshAnimation {
		public List<byte> RawData;

		public MeshAnimation(List<byte> rawData) {
			RawData = rawData;
		}

		public void GenerateRawData() { }
	}
}