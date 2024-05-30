using System.Collections.Generic;

namespace GaneshaDx.Resources.GnsData;

public class Gns {
	public readonly List<byte> RawData;

	private readonly List<int> _resourceRowIndicators = new() { 34, 48, 112 };
	private const int ResourceRowLength = 20;
	private const int UnknownRowLength = 28;

	private readonly List<GnsResourceRow> _resourceRows = new();
	private GnsUnknownRow _gnsUnknownRow;

	public Gns(List<byte> rawData) {
		RawData = rawData;
		// ProcessData();
	}

	private void ProcessData() {
		for (int currentByteIndex = 0; currentByteIndex < RawData.Count;) {
			bool isResourceRow = _resourceRowIndicators.Contains(RawData[currentByteIndex]);

			if (isResourceRow) {
				_resourceRows.Add(new GnsResourceRow(RawData.GetRange(currentByteIndex, ResourceRowLength)));
				currentByteIndex += ResourceRowLength;
			} else {
				_gnsUnknownRow = new GnsUnknownRow(RawData.GetRange(currentByteIndex, UnknownRowLength));
				currentByteIndex += UnknownRowLength;
			}
		}
	}
}