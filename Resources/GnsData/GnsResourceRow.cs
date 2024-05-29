using System.Collections.Generic;

namespace GaneshaDx.Resources.GnsData;

public class GnsResourceRow {
	public List<byte> RawData;

	public GnsResourceRow(List<byte> rawData) {
		RawData = rawData;
		ProcessRawData();
	}

	private void ProcessRawData() {
		
	}
}