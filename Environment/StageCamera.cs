using System;
using System.Collections.Generic;
using GaneshaDx.Common;
using GaneshaDx.Resources.ContentDataTypes.Polygons;
using GaneshaDx.Resources.ContentDataTypes.Terrains;
using GaneshaDx.UserInterface;
using GaneshaDx.UserInterface.GuiDefinitions;
using GaneshaDx.UserInterface.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GaneshaDx.Environment {
	public static class StageCamera {
		private const int FrontViewSnapStrength = 5;
		public static Vector3 CamPosition;
		public static Vector3 CamTarget = new Vector3(-200, 0, 200);
		public static double CameraHorizontalAngle = -45f;
		public static double CameraHeightAngle = 45f;
		public static double ZoomLevel = 1;
		public const float TopElevation = 90f - 39.37f;
		public const float BottomElevation = 90f - 26.54f;

		public static void Update() {
			HandleInput();
			SetCameraState();
		}

		public static void FocusOnSelection() {
			if (Gui.SelectedTab == RightPanelTab.Terrain && Selection.SelectedTerrainTiles.Count > 0) {
				List<Vector3> averagePoints = new List<Vector3>();

				foreach (TerrainTile terrainTile in Selection.SelectedTerrainTiles) {
					averagePoints.Add(terrainTile.AveragePoint);
				}

				CamTarget = Utilities.GetAveragePoint(averagePoints);
			} else if (Selection.SelectedPolygons.Count > 0) {
				List<Vector3> averagePoints = new List<Vector3>();

				foreach (Polygon polygon in Selection.SelectedPolygons) {
					averagePoints.Add(polygon.AveragePoint);
				}

				CamTarget = Utilities.GetAveragePoint(averagePoints);
			}
		}

		private static void HandleInput() {
			if (MyraGui.IsActive || !AppInput.MouseIsWithinModelViewport) {
				return;
			}

			if (AppInput.MouseScrolledUp || AppInput.MouseScrolledDown) {
				HandleZooming();
			}

			if (AppInput.ThisMouseState.RightButton == ButtonState.Pressed) {
				HandlePanning();
			}

			if (AppInput.ThisMouseState.MiddleButton == ButtonState.Pressed) {
				HandleRotation();
			}
		}

		private static void HandlePanning() {
			Vector3 facingDirection = CamTarget - CamPosition;
			Vector3 right = Vector3.Cross(facingDirection, Vector3.Up);
			Vector3 up = Vector3.Cross(facingDirection, right);

			facingDirection.Normalize();
			right.Normalize();
			up.Normalize();

			int direction = Configuration.Properties.InvertedPanning
				? 1
				: -1;

			CamTarget += right * AppInput.MousePositionChange.X *
			             Configuration.Properties.PanningSensitivity *
			             (float) ZoomLevel *
			             direction;

			CamTarget += up * AppInput.MousePositionChange.Y *
			             Configuration.Properties.PanningSensitivity *
			             (float) ZoomLevel *
			             direction;
		}

		private static void HandleZooming() {
			if (AppInput.MouseScrolledUp) {
				ZoomLevel *= Configuration.Properties.ZoomStrength;
			}

			if (AppInput.MouseScrolledDown) {
				ZoomLevel /= Configuration.Properties.ZoomStrength;
			}

			if (ZoomLevel < 0) {
				ZoomLevel = 0;
			}

			if (ZoomLevel > 2) {
				ZoomLevel = 2;
			}
		}

		private static void HandleRotation() {
			int direction = Configuration.Properties.InvertedRotation
				? -1
				: 1;

			CameraHorizontalAngle += AppInput.MousePositionChange.X / 2f *
			                         Configuration.Properties.RotationSensitivity *
			                         direction;

			CameraHeightAngle -= AppInput.MousePositionChange.Y / 2f *
			                     Configuration.Properties.RotationSensitivity *
			                     direction;

			if (CameraHeightAngle > 179) {
				CameraHeightAngle = 179;
			}

			if (CameraHeightAngle < 1) {
				CameraHeightAngle = 1;
			}
		}

		private static void SetCameraState() {
			bool shouldSnapHeightAngle = CameraHeightAngle > 90 - FrontViewSnapStrength &&
			                             CameraHeightAngle < 90 + FrontViewSnapStrength;

			double heightAngleRads = shouldSnapHeightAngle
				? MathHelper.ToRadians(90)
				: MathHelper.ToRadians((float) CameraHeightAngle);

			double horizontalAngleRads = MathHelper.ToRadians((float) CameraHorizontalAngle);

			CamPosition = CamTarget + new Vector3(
				(float) (1000 * Math.Cos(horizontalAngleRads) * Math.Sin(heightAngleRads)),
				(float) (1000 * Math.Cos(heightAngleRads)),
				(float) (1000 * Math.Sin(horizontalAngleRads) * Math.Sin(heightAngleRads))
			);

			Stage.ViewMatrix = Matrix.CreateLookAt(CamPosition, CamTarget, Vector3.Up);

			Stage.ProjectionMatrix = Matrix.CreateOrthographic(
				Stage.ModelingViewport.Width * (float) ZoomLevel,
				Stage.ModelingViewport.Height * (float) ZoomLevel,
				1f,
				5000f
			);
		}
	}
}