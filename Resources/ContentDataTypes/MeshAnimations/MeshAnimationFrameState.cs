using System.Collections.Generic;
using GaneshaDx.Common;
using Microsoft.Xna.Framework;

namespace GaneshaDx.Resources.ContentDataTypes.MeshAnimations {
	public class MeshAnimationFrameState {
		public readonly List<int> Properties = new List<int>();

		public Vector3 Position;
		public Vector3 Rotation;
		public Vector3 Scale;
		public readonly List<MeshAnimationKeyFrameType> PositionKeyFrameTypes = new List<MeshAnimationKeyFrameType>();
		public readonly List<MeshAnimationKeyFrameType> RotationKeyFrameTypes = new List<MeshAnimationKeyFrameType>();
		public readonly List<MeshAnimationKeyFrameType> ScaleKeyFrameTypes = new List<MeshAnimationKeyFrameType>();

		public MeshAnimationFrameState(List<byte> rawData) {
			for (int byteIndex = 0; byteIndex < rawData.Count; byteIndex += 2) {
				Properties.Add(Utilities.GetIntFromLittleEndian(rawData[byteIndex], rawData[byteIndex + 1]));
			}

			Rotation = new Vector3(Properties[0] / 4096f, Properties[1] / 4096f, Properties[2] / 4096f);
			Position = new Vector3(Properties[4] / 4096f, Properties[5] / 4096f, Properties[6] / 4096f);
			Scale = new Vector3(Properties[8] / 4096f, Properties[9] / 4096f, Properties[10] / 4096f);

			RotationKeyFrameTypes.Add(GetKeyFrameType(Properties[30]));
			RotationKeyFrameTypes.Add(GetKeyFrameType(Properties[31]));
			RotationKeyFrameTypes.Add(GetKeyFrameType(Properties[32]));

			PositionKeyFrameTypes.Add(GetKeyFrameType(Properties[33]));
			PositionKeyFrameTypes.Add(GetKeyFrameType(Properties[34]));
			PositionKeyFrameTypes.Add(GetKeyFrameType(Properties[35]));

			ScaleKeyFrameTypes.Add(GetKeyFrameType(Properties[36]));
			ScaleKeyFrameTypes.Add(GetKeyFrameType(Properties[37]));
			ScaleKeyFrameTypes.Add(GetKeyFrameType(Properties[38]));
		}

		private MeshAnimationKeyFrameType GetKeyFrameType(int value) {
			MeshAnimationKeyFrameType type = value switch {
				5 => MeshAnimationKeyFrameType.ChangeTo,
				6 => MeshAnimationKeyFrameType.ChangeBy,
				9 => MeshAnimationKeyFrameType.Unknown9,
				10 => MeshAnimationKeyFrameType.Unknown10,
				17 => MeshAnimationKeyFrameType.Unknown17,
				18 => MeshAnimationKeyFrameType.Unknown18,
				_ => MeshAnimationKeyFrameType.Other
			};

			return type;
		}
	}
}