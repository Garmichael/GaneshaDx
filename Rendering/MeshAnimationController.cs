using System.Collections.Generic;
using GaneshaDx.Common;
using GaneshaDx.Resources;
using GaneshaDx.Resources.ContentDataTypes.MeshAnimations;
using Microsoft.Xna.Framework;

namespace GaneshaDx.Rendering {
	public static class MeshAnimationController {
		public static bool AnimationsPlaying = true;

		private static readonly List<MeshType> AnimatedMeshTypes = new List<MeshType> {
			MeshType.AnimatedMesh1, MeshType.AnimatedMesh2, MeshType.AnimatedMesh3, MeshType.AnimatedMesh4,
			MeshType.AnimatedMesh5, MeshType.AnimatedMesh6, MeshType.AnimatedMesh7, MeshType.AnimatedMesh8
		};

		private static readonly Dictionary<MeshType, MeshAnimationRoutine> Animations =
			new Dictionary<MeshType, MeshAnimationRoutine> {
				{MeshType.AnimatedMesh1, null},
				{MeshType.AnimatedMesh2, null},
				{MeshType.AnimatedMesh3, null},
				{MeshType.AnimatedMesh4, null},
				{MeshType.AnimatedMesh5, null},
				{MeshType.AnimatedMesh6, null},
				{MeshType.AnimatedMesh7, null},
				{MeshType.AnimatedMesh8, null},
			};

		public static void PlayAnimations() {
			Reset();

			if (CurrentMapState.StateData.MeshAnimationInstructions == null) {
				return;
			}

			AnimationsPlaying = true;

			foreach (MeshType meshType in AnimatedMeshTypes) {
				List<MeshAnimation> meshAnimations = CurrentMapState.StateData.MeshAnimationInstructions.MeshAnimations;
				int meshAnimationIndex = (int) meshType - 1;
				bool isValidAnimationFrame = meshAnimations[meshAnimationIndex].Frames[0].FrameStateId > 0;

				if (isValidAnimationFrame) {
					Animations[meshType] ??= new MeshAnimationRoutine(
						meshAnimations[meshAnimationIndex].Frames[0],
						Vector3.Zero,
						Vector3.Zero,
						Vector3.Zero,
						true
					);
				}
			}
		}

		public static void StopAnimations() {
			Reset();
			AnimationsPlaying = false;
		}

		public static void Update() {
			if (!MapData.MapIsLoaded || CurrentMapState.StateData.MeshAnimationInstructions == null) {
				return;
			}

			foreach (MeshType meshType in AnimatedMeshTypes) {
				if (Animations[meshType] != null && Animations[meshType].IsFinished()) {
					MeshAnimationInstructions allInstructions = CurrentMapState.StateData.MeshAnimationInstructions;
					MeshAnimation thisMeshAnimation = allInstructions.MeshAnimations[(int) meshType - 1];

					MeshAnimationRoutine currentRoutine = Animations[meshType];
					int nextFrameId = currentRoutine.CurrentFrame.NextFrameId - 1;

					Animations[meshType] = new MeshAnimationRoutine(
						nextFrameId >= 0 ? thisMeshAnimation.Frames[nextFrameId] : null,
						currentRoutine.CurrentPosition,
						currentRoutine.CurrentRotation,
						currentRoutine.CurrentScale,
						false
					);
				}

				if (Animations[meshType] != null) {
					Animations[meshType].Animate();
				}
			}
		}

		public static Vector3 GetAnimatedVertexOffset(MeshType meshType, Vector3 vertexPosition) {
			if (Animations[meshType] == null) {
				return vertexPosition;
			}

			Matrix rotationX = Matrix.CreateRotationX(MathHelper.ToRadians(Animations[meshType].CurrentRotation.X));
			Matrix rotationY = Matrix.CreateRotationY(MathHelper.ToRadians(Animations[meshType].CurrentRotation.Y));
			Matrix rotationZ = Matrix.CreateRotationZ(MathHelper.ToRadians(Animations[meshType].CurrentRotation.Z));

			vertexPosition = Vector3.Transform(vertexPosition, rotationX * rotationY * rotationZ);

			Matrix scale = Matrix.CreateScale(
				Animations[meshType].CurrentScale.X,
				Animations[meshType].CurrentScale.Y,
				Animations[meshType].CurrentScale.Z
			);

			vertexPosition = Vector3.Transform(vertexPosition, scale);

			vertexPosition += new Vector3(
				-Animations[meshType].CurrentPosition.X,
				Animations[meshType].CurrentPosition.Y,
				Animations[meshType].CurrentPosition.Z
			);

			return vertexPosition;
		}

		private static void Reset() {
			Animations[MeshType.AnimatedMesh1] = null;
			Animations[MeshType.AnimatedMesh2] = null;
			Animations[MeshType.AnimatedMesh3] = null;
			Animations[MeshType.AnimatedMesh4] = null;
			Animations[MeshType.AnimatedMesh5] = null;
			Animations[MeshType.AnimatedMesh6] = null;
			Animations[MeshType.AnimatedMesh7] = null;
			Animations[MeshType.AnimatedMesh8] = null;
		}
	}
}