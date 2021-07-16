using System.Collections.Generic;
using GaneshaDx.Common;
using Microsoft.Xna.Framework;

namespace GaneshaDx.Resources.ContentDataTypes.MeshAnimations {
	public class MeshAnimationKeyFrame {
		public readonly List<int> Properties = new List<int>();

		public readonly List<double> Position = new List<double>();
		public readonly List<double> Rotation = new List<double>();
		public readonly List<double> Scale = new List<double>();
		public readonly List<MeshAnimationTweenType> PositionTweenTypes = new List<MeshAnimationTweenType>();
		public readonly List<MeshAnimationTweenType> RotationTweenTypes = new List<MeshAnimationTweenType>();
		public readonly List<MeshAnimationTweenType> ScaleTweenTypes = new List<MeshAnimationTweenType>();
		public readonly List<double> PositionStartPercents = new List<double>();
		public readonly List<double> RotationStartPercents = new List<double>();
		public readonly List<double> ScaleStartPercents = new List<double>();
		public readonly List<double> PositionEndPercents = new List<double>();
		public readonly List<double> RotationEndPercents = new List<double>();
		public readonly List<double> ScaleEndPercents = new List<double>();

		public MeshAnimationKeyFrame(List<byte> rawData) {
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

			RotationTweenTypes.Add(ConvertTweenType(Properties[30]));
			RotationTweenTypes.Add(ConvertTweenType(Properties[31]));
			RotationTweenTypes.Add(ConvertTweenType(Properties[32]));

			PositionTweenTypes.Add(ConvertTweenType(Properties[33]));
			PositionTweenTypes.Add(ConvertTweenType(Properties[34]));
			PositionTweenTypes.Add(ConvertTweenType(Properties[35]));

			ScaleTweenTypes.Add(ConvertTweenType(Properties[36]));
			ScaleTweenTypes.Add(ConvertTweenType(Properties[37]));
			ScaleTweenTypes.Add(ConvertTweenType(Properties[38]));
		}

		public List<byte> GetRawData() {
			return new List<byte> {
				Utilities.GetLittleEndianFromInt((int) (Rotation[0] / 360f * 4096f)).high,
				Utilities.GetLittleEndianFromInt((int) (Rotation[0] / 360f * 4096f)).low,
				Utilities.GetLittleEndianFromInt((int) (Rotation[1] / 360f * 4096f)).high,
				Utilities.GetLittleEndianFromInt((int) (Rotation[1] / 360f * 4096f)).low,
				Utilities.GetLittleEndianFromInt((int) (Rotation[2] / 360f * 4096f)).high,
				Utilities.GetLittleEndianFromInt((int) (Rotation[2] / 360f * 4096f)).low,

				0, 0,

				Utilities.GetLittleEndianFromInt((int) Position[0]).high,
				Utilities.GetLittleEndianFromInt((int) Position[0]).low,
				Utilities.GetLittleEndianFromInt((int) -Position[1]).high,
				Utilities.GetLittleEndianFromInt((int) -Position[1]).low,
				Utilities.GetLittleEndianFromInt((int) Position[2]).high,
				Utilities.GetLittleEndianFromInt((int) Position[2]).low,

				0, 0,

				Utilities.GetLittleEndianFromInt((int) (Scale[0] * 4096f)).high,
				Utilities.GetLittleEndianFromInt((int) (Scale[0] * 4096f)).low,
				Utilities.GetLittleEndianFromInt((int) (Scale[1] * 4096f)).high,
				Utilities.GetLittleEndianFromInt((int) (Scale[1] * 4096f)).low,
				Utilities.GetLittleEndianFromInt((int) (Scale[2] * 4096f)).high,
				Utilities.GetLittleEndianFromInt((int) (Scale[2] * 4096f)).low,

				0, 0,

				Utilities.GetLittleEndianFromInt((int) (RotationStartPercents[0] / 100f * 4096f)).high,
				Utilities.GetLittleEndianFromInt((int) (RotationStartPercents[0] / 100f * 4096f)).low,
				Utilities.GetLittleEndianFromInt((int) (RotationStartPercents[1] / 100f * 4096f)).high,
				Utilities.GetLittleEndianFromInt((int) (RotationStartPercents[1] / 100f * 4096f)).low,
				Utilities.GetLittleEndianFromInt((int) (RotationStartPercents[2] / 100f * 4096f)).high,
				Utilities.GetLittleEndianFromInt((int) (RotationStartPercents[2] / 100f * 4096f)).low,

				Utilities.GetLittleEndianFromInt((int) (PositionStartPercents[0] / 100f * 4096f)).high,
				Utilities.GetLittleEndianFromInt((int) (PositionStartPercents[0] / 100f * 4096f)).low,
				Utilities.GetLittleEndianFromInt((int) (PositionStartPercents[1] / 100f * 4096f)).high,
				Utilities.GetLittleEndianFromInt((int) (PositionStartPercents[1] / 100f * 4096f)).low,
				Utilities.GetLittleEndianFromInt((int) (PositionStartPercents[2] / 100f * 4096f)).high,
				Utilities.GetLittleEndianFromInt((int) (PositionStartPercents[2] / 100f * 4096f)).low,

				Utilities.GetLittleEndianFromInt((int) (ScaleStartPercents[0] / 100f * 4096f)).high,
				Utilities.GetLittleEndianFromInt((int) (ScaleStartPercents[0] / 100f * 4096f)).low,
				Utilities.GetLittleEndianFromInt((int) (ScaleStartPercents[1] / 100f * 4096f)).high,
				Utilities.GetLittleEndianFromInt((int) (ScaleStartPercents[1] / 100f * 4096f)).low,
				Utilities.GetLittleEndianFromInt((int) (ScaleStartPercents[2] / 100f * 4096f)).high,
				Utilities.GetLittleEndianFromInt((int) (ScaleStartPercents[2] / 100f * 4096f)).low,

				Utilities.GetLittleEndianFromInt((int) (RotationEndPercents[0] / 100f * 4096f)).high,
				Utilities.GetLittleEndianFromInt((int) (RotationEndPercents[0] / 100f * 4096f)).low,
				Utilities.GetLittleEndianFromInt((int) (RotationEndPercents[1] / 100f * 4096f)).high,
				Utilities.GetLittleEndianFromInt((int) (RotationEndPercents[1] / 100f * 4096f)).low,
				Utilities.GetLittleEndianFromInt((int) (RotationEndPercents[2] / 100f * 4096f)).high,
				Utilities.GetLittleEndianFromInt((int) (RotationEndPercents[2] / 100f * 4096f)).low,

				Utilities.GetLittleEndianFromInt((int) (PositionEndPercents[0] / 100f * 4096f)).high,
				Utilities.GetLittleEndianFromInt((int) (PositionEndPercents[0] / 100f * 4096f)).low,
				Utilities.GetLittleEndianFromInt((int) (PositionEndPercents[1] / 100f * 4096f)).high,
				Utilities.GetLittleEndianFromInt((int) (PositionEndPercents[1] / 100f * 4096f)).low,
				Utilities.GetLittleEndianFromInt((int) (PositionEndPercents[2] / 100f * 4096f)).high,
				Utilities.GetLittleEndianFromInt((int) (PositionEndPercents[2] / 100f * 4096f)).low,

				Utilities.GetLittleEndianFromInt((int) (ScaleEndPercents[0] / 100f * 4096f)).high,
				Utilities.GetLittleEndianFromInt((int) (ScaleEndPercents[0] / 100f * 4096f)).low,
				Utilities.GetLittleEndianFromInt((int) (ScaleEndPercents[1] / 100f * 4096f)).high,
				Utilities.GetLittleEndianFromInt((int) (ScaleEndPercents[1] / 100f * 4096f)).low,
				Utilities.GetLittleEndianFromInt((int) (ScaleEndPercents[2] / 100f * 4096f)).high,
				Utilities.GetLittleEndianFromInt((int) (ScaleEndPercents[2] / 100f * 4096f)).low,

				Utilities.GetLittleEndianFromInt(ConvertTweenType(RotationTweenTypes[0])).high,
				Utilities.GetLittleEndianFromInt(ConvertTweenType(RotationTweenTypes[0])).low,
				Utilities.GetLittleEndianFromInt(ConvertTweenType(RotationTweenTypes[1])).high,
				Utilities.GetLittleEndianFromInt(ConvertTweenType(RotationTweenTypes[1])).low,
				Utilities.GetLittleEndianFromInt(ConvertTweenType(RotationTweenTypes[2])).high,
				Utilities.GetLittleEndianFromInt(ConvertTweenType(RotationTweenTypes[2])).low,

				Utilities.GetLittleEndianFromInt(ConvertTweenType(PositionTweenTypes[0])).high,
				Utilities.GetLittleEndianFromInt(ConvertTweenType(PositionTweenTypes[0])).low,
				Utilities.GetLittleEndianFromInt(ConvertTweenType(PositionTweenTypes[1])).high,
				Utilities.GetLittleEndianFromInt(ConvertTweenType(PositionTweenTypes[1])).low,
				Utilities.GetLittleEndianFromInt(ConvertTweenType(PositionTweenTypes[2])).high,
				Utilities.GetLittleEndianFromInt(ConvertTweenType(PositionTweenTypes[2])).low,

				Utilities.GetLittleEndianFromInt(ConvertTweenType(ScaleTweenTypes[0])).high,
				Utilities.GetLittleEndianFromInt(ConvertTweenType(ScaleTweenTypes[0])).low,
				Utilities.GetLittleEndianFromInt(ConvertTweenType(ScaleTweenTypes[1])).high,
				Utilities.GetLittleEndianFromInt(ConvertTweenType(ScaleTweenTypes[1])).low,
				Utilities.GetLittleEndianFromInt(ConvertTweenType(ScaleTweenTypes[2])).high,
				Utilities.GetLittleEndianFromInt(ConvertTweenType(ScaleTweenTypes[2])).low,

				0, 0
			};
		}

		private MeshAnimationTweenType ConvertTweenType(int value) {
			MeshAnimationTweenType type = value switch {
				0 => MeshAnimationTweenType.Unused,
				5 => MeshAnimationTweenType.TweenTo,
				6 => MeshAnimationTweenType.TweenBy,
				9 => MeshAnimationTweenType.Ignore,
				10 => MeshAnimationTweenType.Oscillate,
				17 => MeshAnimationTweenType.SnapTo,
				18 => MeshAnimationTweenType.OscillateOffset,
				_ => MeshAnimationTweenType.Unknown
			};

			return type;
		}

		private int ConvertTweenType(MeshAnimationTweenType value) {
			int type = value switch {
				MeshAnimationTweenType.Unused => 0,
				MeshAnimationTweenType.TweenTo => 5,
				MeshAnimationTweenType.TweenBy => 6,
				MeshAnimationTweenType.Ignore => 9,
				MeshAnimationTweenType.Oscillate => 10,
				MeshAnimationTweenType.SnapTo => 17,
				MeshAnimationTweenType.OscillateOffset => 18,
				_ => 99
			};

			return type;
		}
	}
}