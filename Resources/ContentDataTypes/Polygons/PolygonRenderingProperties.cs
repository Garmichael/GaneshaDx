using System.Collections.Generic;
using GaneshaDx.Common;

namespace GaneshaDx.Resources.ContentDataTypes.Polygons {
	public class PolygonRenderingProperties {
		public readonly string RawData;
		public bool InvisibleNorthwest;
		public bool InvisibleNorthNorthwest;
		public bool InvisibleNorthNortheast;
		public bool InvisibleNortheast;
		public bool InvisibleWestNorthWest;
		public bool InvisibleEastNortheast;
		public bool InvisibleWestSouthwest;
		public bool InvisibleEastSoutheast;
		public bool InvisibleSouthwest;
		public bool InvisibleSouthSouthwest;
		public bool InvisibleSouthSoutheast;
		public bool InvisibleSoutheast;
		public bool LitTexture;
		public bool Unknown1;
		public bool Unknown14;
		public bool Unknown15;

		public PolygonRenderingProperties() {
			RawData = "0000000000000000";
			LitTexture = true;
		}

		public PolygonRenderingProperties(string rawData) {
			RawData = rawData;
			LitTexture = RawData.Substring(0, 1) == "1";
			Unknown1 = RawData.Substring(1, 1) == "1";
			InvisibleNortheast = RawData.Substring(2, 1) == "1";
			InvisibleSoutheast = RawData.Substring(3, 1) == "1";
			InvisibleSouthwest = RawData.Substring(4, 1) == "1";
			InvisibleNorthwest = RawData.Substring(5, 1) == "1";
			InvisibleNorthNortheast = RawData.Substring(6, 1) == "1";
			InvisibleEastNortheast = RawData.Substring(7, 1) == "1";
			InvisibleEastSoutheast = RawData.Substring(8, 1) == "1";
			InvisibleSouthSoutheast = RawData.Substring(9, 1) == "1";
			InvisibleSouthSouthwest = RawData.Substring(10, 1) == "1";
			InvisibleWestSouthwest = RawData.Substring(11, 1) == "1";
			InvisibleWestNorthWest = RawData.Substring(12, 1) == "1";
			InvisibleNorthNorthwest = RawData.Substring(13, 1) == "1";
			Unknown14 = RawData.Substring(14, 1) == "1";
			Unknown15 = RawData.Substring(15, 1) == "1";
		}

		public List<byte> GetRawData() {
			string highBinary = (InvisibleEastSoutheast ? "1" : "0") +
			                    (InvisibleSouthSoutheast ? "1" : "0") +
			                    (InvisibleSouthSouthwest ? "1" : "0") +
			                    (InvisibleWestSouthwest ? "1" : "0") +
			                    (InvisibleWestNorthWest ? "1" : "0") +
			                    (InvisibleNorthNorthwest ? "1" : "0") +
			                    (Unknown14 ? "1" : "0") +
			                    (Unknown15 ? "1" : "0");

			string lowBinary = (LitTexture ? "1" : "0") +
			                   (Unknown1 ? "1" : "0") +
			                   (InvisibleNortheast ? "1" : "0") +
			                   (InvisibleSoutheast ? "1" : "0") +
			                   (InvisibleSouthwest ? "1" : "0") +
			                   (InvisibleNorthwest ? "1" : "0") +
			                   (InvisibleNorthNortheast ? "1" : "0") +
			                   (InvisibleEastNortheast ? "1" : "0");

			return new List<byte> {
				(byte) Utilities.GetIntFromBinary(highBinary),
				(byte) Utilities.GetIntFromBinary(lowBinary)
			};
		}

		public PolygonRenderingProperties Clone() {
			return new PolygonRenderingProperties {
				InvisibleNorthwest = InvisibleNorthwest,
				InvisibleNorthNorthwest = InvisibleNorthNorthwest,
				InvisibleNorthNortheast = InvisibleNorthNortheast,
				InvisibleNortheast = InvisibleNortheast,
				InvisibleWestNorthWest = InvisibleWestNorthWest,
				InvisibleEastNortheast = InvisibleEastNortheast,
				InvisibleWestSouthwest = InvisibleWestSouthwest,
				InvisibleEastSoutheast = InvisibleEastSoutheast,
				InvisibleSouthwest = InvisibleSouthwest,
				InvisibleSouthSouthwest = InvisibleSouthSouthwest,
				InvisibleSouthSoutheast = InvisibleSouthSoutheast,
				InvisibleSoutheast = InvisibleSoutheast,
				LitTexture = LitTexture,
				Unknown1 = Unknown1,
				Unknown14 = Unknown14,
				Unknown15 = Unknown15
			};
		}
	}
}