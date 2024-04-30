using System.Collections.Generic;
using GaneshaDx.Common;
using GaneshaDx.Resources;
using GaneshaDx.Resources.ContentDataTypes.MeshAnimations;
using GaneshaDx.Resources.ContentDataTypes.Polygons;
using Microsoft.Xna.Framework;

namespace GaneshaDx.Rendering {
	public static class MeshAnimationController {
		public static bool AnimationsPlaying = true;
		public static int ActivePlayingState;

		private static readonly List<MeshType> AnimatedMeshTypes = new List<MeshType> {
			MeshType.AnimatedMesh1, MeshType.AnimatedMesh2, MeshType.AnimatedMesh3, MeshType.AnimatedMesh4,
			MeshType.AnimatedMesh5, MeshType.AnimatedMesh6, MeshType.AnimatedMesh7, MeshType.AnimatedMesh8
		};

		public static readonly Dictionary<MeshType, MeshAnimationRoutine> Animations =
			new Dictionary<MeshType, MeshAnimationRoutine> {
				{ MeshType.AnimatedMesh1, null },
				{ MeshType.AnimatedMesh2, null },
				{ MeshType.AnimatedMesh3, null },
				{ MeshType.AnimatedMesh4, null },
				{ MeshType.AnimatedMesh5, null },
				{ MeshType.AnimatedMesh6, null },
				{ MeshType.AnimatedMesh7, null },
				{ MeshType.AnimatedMesh8, null },
			};

		private static readonly Dictionary<MeshType, AnimatedMeshProperties> MeshProperties =
			new Dictionary<MeshType, AnimatedMeshProperties> {
				{ MeshType.AnimatedMesh1, null },
				{ MeshType.AnimatedMesh2, null },
				{ MeshType.AnimatedMesh3, null },
				{ MeshType.AnimatedMesh4, null },
				{ MeshType.AnimatedMesh5, null },
				{ MeshType.AnimatedMesh6, null },
				{ MeshType.AnimatedMesh7, null },
				{ MeshType.AnimatedMesh8, null },
			};

		public static void PlayAnimations(int stateToPlay = 0) {
			Reset();

			if (CurrentMapState.StateData.MeshAnimationSet == null) {
				return;
			}

			ActivePlayingState = stateToPlay;
			AnimationsPlaying = true;

			foreach (MeshType meshType in AnimatedMeshTypes) {
				List<AnimatedMeshInstructionSet> meshAnimations = CurrentMapState.StateData.MeshAnimationSet.MeshInstructionSets;
				int meshAnimationIndex = (int) (meshType - 1) + ActivePlayingState * 8;

				bool isValidAnimationFrame = meshAnimations[meshAnimationIndex].Instructions[0].FrameStateId > 0;

				if (isValidAnimationFrame) {
					Animations[meshType] ??= new MeshAnimationRoutine(
						meshAnimations[meshAnimationIndex].Instructions[0],
						Vector3.Zero,
						Vector3.Zero,
						Vector3.One,
						true,
						meshAnimations[meshAnimationIndex]
					);
				}

				MeshProperties[meshType] = CurrentMapState.StateData.MeshAnimationSet.MeshProperties[meshAnimationIndex];
			}
		}

		public static void StopAnimations() {
			Reset();
			AnimationsPlaying = false;

			foreach (MeshType meshType in AnimatedMeshTypes) {
				foreach (KeyValuePair<PolygonType, List<Polygon>> polygons in CurrentMapState.StateData.PolygonCollection[meshType]) {
					foreach (Polygon polygon in polygons.Value) {
						foreach (Vertex vertex in polygon.Vertices) {
							vertex.LastAnimatedStartPosition.X = vertex.Position.X;
							vertex.LastAnimatedStartPosition.Y = vertex.Position.Y;
							vertex.LastAnimatedStartPosition.Z = vertex.Position.Z;
						}
					}
				}
			}
		}

		public static void Update() {
			if (!MapData.MapIsLoaded || CurrentMapState.StateData.MeshAnimationSet == null) {
				return;
			}

			foreach (MeshType meshType in AnimatedMeshTypes) {
				if (Animations[meshType] != null && Animations[meshType].IsFinished()) {
					List<AnimatedMeshInstructionSet> meshAnimations = CurrentMapState.StateData.MeshAnimationSet.MeshInstructionSets;
					int meshAnimationIndex = (int) (meshType - 1) + ActivePlayingState * 8;
					AnimatedMeshInstructionSet thisAnimatedMeshInstructionSet = meshAnimations[meshAnimationIndex];

					MeshAnimationRoutine currentRoutine = Animations[meshType];
					int nextFrameId = currentRoutine.AnimatedMeshInstruction.NextFrameId - 1;

					Animations[meshType] = new MeshAnimationRoutine(
						nextFrameId >= 0 ? thisAnimatedMeshInstructionSet.Instructions[nextFrameId] : null,
						currentRoutine.CurrentPosition,
						currentRoutine.CurrentRotation,
						currentRoutine.CurrentScale,
						false,
						thisAnimatedMeshInstructionSet
					);

					foreach (
						KeyValuePair<PolygonType, List<Polygon>> polygonBucket in
						CurrentMapState.StateData.PolygonCollection[meshType]
					) {
						foreach (Polygon polygon in polygonBucket.Value) {
							polygon.SetLastAnimatedPositionsForVertices(currentRoutine.CurrentRotation);
						}
					}
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

			if (MeshProperties[meshType].LinkedParentMesh > 0) {
				vertexPosition = GetAnimatedVertexOffset(
					AnimatedMeshTypes[MeshProperties[meshType].LinkedParentMesh - 1],
					vertexPosition
				);
			}

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