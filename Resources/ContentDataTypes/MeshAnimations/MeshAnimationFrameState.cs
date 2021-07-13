using System.Collections.Generic;
using GaneshaDx.Common;
using Microsoft.Xna.Framework;

namespace GaneshaDx.Resources.ContentDataTypes.MeshAnimations {
	public class MeshAnimationFrameState {
		public readonly List<int> Properties = new List<int>();

		public readonly List<double> Position = new List<double>();
		public readonly List<double> Rotation = new List<double>();
		public readonly List<double> Scale = new List<double>();
		public readonly List<MeshAnimationKeyFrameType> PositionKeyFrameTypes = new List<MeshAnimationKeyFrameType>();
		public readonly List<MeshAnimationKeyFrameType> RotationKeyFrameTypes = new List<MeshAnimationKeyFrameType>();
		public readonly List<MeshAnimationKeyFrameType> ScaleKeyFrameTypes = new List<MeshAnimationKeyFrameType>();
		public readonly List<double> PositionStartPercents = new List<double>();
		public readonly List<double> RotationStartPercents = new List<double>();
		public readonly List<double> ScaleStartPercents = new List<double>();
		public readonly List<double> PositionEndPercents = new List<double>();
		public readonly List<double> RotationEndPercents = new List<double>();
		public readonly List<double> ScaleEndPercents = new List<double>();

		public MeshAnimationFrameState(List<byte> rawData) {
			for (int byteIndex = 0; byteIndex < rawData.Count; byteIndex += 2) {
				Properties.Add(Utilities.GetIntFromLittleEndian(rawData[byteIndex], rawData[byteIndex + 1]));
			}

			Rotation.Add(Properties[0] / 4096f * 360);
			Rotation.Add(Properties[1] / 4096f * 360);
			Rotation.Add(Properties[2] / 4096f * 360);

			Position.Add(Properties[4]);
			Position.Add(-Properties[5]);
			Position.Add(Properties[6]);

			Scale.Add(Properties[8] / 4096f);
			Scale.Add(Properties[9] / 4096f);
			Scale.Add(Properties[10] / 4096f);

			PositionStartPercents.Add(Properties[15] / 4096f * 100f);
			PositionStartPercents.Add(Properties[16] / 4096f * 100f);
			PositionStartPercents.Add(Properties[17] / 4096f * 100f);

			RotationStartPercents.Add(Properties[12] / 4096f * 100f);
			RotationStartPercents.Add(Properties[13] / 4096f * 100f);
			RotationStartPercents.Add(Properties[14] / 4096f * 100f);

			ScaleStartPercents.Add(Properties[18] / 4096f * 100f);
			ScaleStartPercents.Add(Properties[19] / 4096f * 100f);
			ScaleStartPercents.Add(Properties[20] / 4096f * 100f);

			PositionEndPercents.Add(Properties[24] / 4096f * 100f);
			PositionEndPercents.Add(Properties[25] / 4096f * 100f);
			PositionEndPercents.Add(Properties[26] / 4096f * 100f);

			RotationEndPercents.Add(Properties[21] / 4096f * 100f);
			RotationEndPercents.Add(Properties[22] / 4096f * 100f);
			RotationEndPercents.Add(Properties[23] / 4096f * 100f);

			ScaleEndPercents.Add(Properties[27] / 4096f * 100f);
			ScaleEndPercents.Add(Properties[28] / 4096f * 100f);
			ScaleEndPercents.Add(Properties[29] / 4096f * 100f);

			RotationKeyFrameTypes.Add(ConvertKeyFrameType(Properties[30]));
			RotationKeyFrameTypes.Add(ConvertKeyFrameType(Properties[31]));
			RotationKeyFrameTypes.Add(ConvertKeyFrameType(Properties[32]));

			PositionKeyFrameTypes.Add(ConvertKeyFrameType(Properties[33]));
			PositionKeyFrameTypes.Add(ConvertKeyFrameType(Properties[34]));
			PositionKeyFrameTypes.Add(ConvertKeyFrameType(Properties[35]));

			ScaleKeyFrameTypes.Add(ConvertKeyFrameType(Properties[36]));
			ScaleKeyFrameTypes.Add(ConvertKeyFrameType(Properties[37]));
			ScaleKeyFrameTypes.Add(ConvertKeyFrameType(Properties[38]));
		}

		public List<byte> GetRawData() {
			List<byte> rawData = new List<byte>();

			return rawData;
		}

		private MeshAnimationKeyFrameType ConvertKeyFrameType(int value) {
			MeshAnimationKeyFrameType type = value switch {
				0 => MeshAnimationKeyFrameType.Unused,
				5 => MeshAnimationKeyFrameType.TweenTo,
				6 => MeshAnimationKeyFrameType.TweenBy,
				9 => MeshAnimationKeyFrameType.Ignore,
				10 => MeshAnimationKeyFrameType.Oscillate,
				17 => MeshAnimationKeyFrameType.SnapTo,
				18 => MeshAnimationKeyFrameType.OscillateOffset,
				_ => MeshAnimationKeyFrameType.Unknown
			};

			return type;
		}

		private int ConvertKeyFrameType(MeshAnimationKeyFrameType value) {
			int type = value switch {
				MeshAnimationKeyFrameType.Unused => 0,
				MeshAnimationKeyFrameType.TweenTo => 5,
				MeshAnimationKeyFrameType.TweenBy => 6,
				MeshAnimationKeyFrameType.Ignore => 9,
				MeshAnimationKeyFrameType.Oscillate => 10,
				MeshAnimationKeyFrameType.SnapTo => 17,
				MeshAnimationKeyFrameType.OscillateOffset => 18,
				_ => 99
			};

			return type;
		}
	}
}