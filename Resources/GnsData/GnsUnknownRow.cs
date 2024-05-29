using System.Collections.Generic;

namespace GaneshaDx.Resources.GnsData;

public class GnsUnknownRow {
	public List<byte> RawData;

	public GnsUnknownRow(List<byte> rawData) {
		RawData = rawData;
	}
}