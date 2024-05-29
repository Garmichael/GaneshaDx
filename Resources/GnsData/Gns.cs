using System.Collections.Generic;

namespace GaneshaDx.Resources.GnsData;

public class Gns {
	public readonly List<byte> RawData;

	private const int ResourceRow = 34;
	private const int ResourceRowLength = 20;
	private const int UnknownRowLength = 28;

	private readonly List<GnsResourceRow> _resourceRows = new();
	private GnsUnknownRow _gnsUnknownRow;

	public Gns(List<byte> rawData) {
		RawData = rawData;
		// ProcessData();
	}

	private void ProcessData() {
		for (int currentByteIndex = 0; currentByteIndex < RawData.Count; currentByteIndex++) {
			bool isResourceRow = RawData[currentByteIndex] == ResourceRow;

			if (isResourceRow) {
				List<byte> resourceRowData = new();
				resourceRowData.AddRange(RawData.GetRange(currentByteIndex, ResourceRowLength));
				_resourceRows.Add(new GnsResourceRow(resourceRowData));
				currentByteIndex += ResourceRowLength;
			} else {
				_gnsUnknownRow = new GnsUnknownRow(RawData.GetRange(currentByteIndex, UnknownRowLength));
				currentByteIndex += UnknownRowLength;
			}
		}
	}
}