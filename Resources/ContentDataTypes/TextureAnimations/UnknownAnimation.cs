using System.Collections.Generic;
using GaneshaDx.Common;

namespace GaneshaDx.Resources.ContentDataTypes.TextureAnimations {
	public class UnknownAnimation : TextureAnimation {
		public int Unknown0;
		public int Unknown1;
		public int Unknown2;
		public int Unknown3;
		public int Unknown4;
		public int Unknown5;
		public int Unknown6;
		public int Unknown7;
		public int Unknown8;
		public int Unknown9;
		public int Unknown10;
		public int Unknown11;
		public int Unknown12;
		public int Unknown13;
		public int Unknown14;
		public int Unknown15;
		public int Unknown16;
		public int Unknown17;
		public int Unknown18;
		public int Unknown19;

		public UnknownAnimation() { }

		public UnknownAnimation(List<byte> rawData) {
			Unknown0 = rawData[0];
			Unknown1 = rawData[1];
			Unknown2 = rawData[2];
			Unknown3 = rawData[3];
			Unknown4 = rawData[4];
			Unknown5 = rawData[5];
			Unknown6 = rawData[6];
			Unknown7 = rawData[7];
			Unknown8 = rawData[8];
			Unknown9 = rawData[9];
			Unknown10 = rawData[10];
			Unknown11 = rawData[11];
			Unknown12 = rawData[12];
			Unknown13 = rawData[13];
			Unknown14 = rawData[14];
			Unknown15 = rawData[15];
			Unknown16 = rawData[16];
			Unknown17 = rawData[17];
			Unknown18 = rawData[18];
			Unknown19 = rawData[19];
		}
		
		public List<byte> GetRawData() {
			return new List<byte> {
				(byte) Unknown0,
				(byte) Unknown1,
				(byte) Unknown2,
				(byte) Unknown3,
				(byte) Unknown4,
				(byte) Unknown5,
				(byte) Unknown6,
				(byte) Unknown7,
				(byte) Unknown8,
				(byte) Unknown9,
				(byte) Unknown10,
				(byte) Unknown11,
				(byte) Unknown12,
				(byte) Unknown13,
				(byte) Unknown14,
				(byte) Unknown15,
				(byte) Unknown16,
				(byte) Unknown17,
				(byte) Unknown18,
				(byte) Unknown19
			};
		}
	}
}