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
		private const int VerticalSnapStrength = 5;
		private const int HorizontalSnapStrength = 5;
		public static Vector3 CamPosition;
		public static Vector3 CamTarget = new Vector3(-200, 0, 200);
		public static double CameraHorizontalAngle = -45f;
		public static double CameraHeightAngle = 45f;
		public static double ZoomLevel = 1;
		public const float TopElevation = 90f - 39.37f;
		public const float BottomElevation = 90f - 26.54f;
		public const float CameraSpinSpeed = 10;
		private static bool _cameraIsSpinningHorizontally;
		private static bool _cameraIsSpinningVertically;
		private static CameraView _cameraSpinningDestinationAngle;
		private static CameraElevation _cameraSpinningDestinationElevation;
		private static RotationDirection _cameraSpinningRotationDirection;

		public static CameraView FacingDirection {
			get {
				const int cornerWidth = 30;
				if (Math.Abs(CameraHorizontalAngle - CameraValues[CameraView.Southwest]) < cornerWidth) {
					return CameraView.Southwest;
				}

				if (Math.Abs(CameraHorizontalAngle - CameraValues[CameraView.Northwest]) < cornerWidth) {
					return CameraView.Northwest;
				}

				if (Math.Abs(CameraHorizontalAngle - CameraValues[CameraView.Southeast]) < cornerWidth) {
					return CameraView.Southeast;
				}

				if (Math.Abs(CameraHorizontalAngle - CameraValues[CameraView.Northeast]) < cornerWidth) {
					return CameraView.Northeast;
				}

				return CameraView.Unset;
			}
		}

		private static readonly Dictionary<CameraView, float> CameraValues = new Dictionary<CameraView, float> {
			{ CameraView.Northwest, 225 },
			{ CameraView.Northeast, 315 },
			{ CameraView.Southwest, 135 },
			{ CameraView.Southeast, 45 },
		};

		public enum CameraView {
			Northwest,
			Northeast,
			Southwest,
			Southeast,
			Unset
		}

		public enum CameraElevation {
			Top,
			Bottom
		}

		public enum RotationDirection {
			Left,
			Right
		}

		public static void Update() {
			AnimateSpinningCamera();
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

		public static void SpinCameraTo(CameraView cameraView, CameraElevation cameraElevation) {
			SpinCameraTo(cameraView);
			SpinCameraTo(cameraElevation);
		}

		public static void SpinCameraTo(CameraView cameraView) {
			_cameraSpinningDestinationAngle = cameraView;
			_cameraIsSpinningHorizontally = true;
			_cameraSpinningRotationDirection = cameraView switch {
				CameraView.Northwest => CameraHorizontalAngle > CameraValues[CameraView.Southeast] &&
				                        CameraHorizontalAngle < CameraValues[CameraView.Northwest]
					? RotationDirection.Left
					: RotationDirection.Right,
				CameraView.Northeast => CameraHorizontalAngle > CameraValues[CameraView.Southwest] &&
				                        CameraHorizontalAngle < CameraValues[CameraView.Northeast]
					? RotationDirection.Left
					: RotationDirection.Right,
				CameraView.Southwest => CameraHorizontalAngle > CameraValues[CameraView.Northeast] ||
				                        CameraHorizontalAngle < CameraValues[CameraView.Southwest]
					? RotationDirection.Left
					: RotationDirection.Right,
				CameraView.Southeast => CameraHorizontalAngle > CameraValues[CameraView.Northwest] ||
				                        CameraHorizontalAngle < CameraValues[CameraView.Southeast]
					? RotationDirection.Left
					: RotationDirection.Right,
				_ => _cameraSpinningRotationDirection
			};
		}

		public static void SpinCameraTo(CameraElevation cameraElevation) {
			_cameraSpinningDestinationElevation = cameraElevation;
			_cameraIsSpinningVertically = true;
		}

		private static void AnimateSpinningCamera() {
			while (CameraHorizontalAngle > 360) {
				CameraHorizontalAngle -= 360;
			}

			while (CameraHorizontalAngle < 0) {
				CameraHorizontalAngle += 360;
			}

			if (_cameraIsSpinningHorizontally) {
				double distanceToHorizontalDestination =
					Math.Abs(CameraHorizontalAngle - CameraValues[_cameraSpinningDestinationAngle]);

				if (_cameraSpinningRotationDirection == RotationDirection.Left) {
					CameraHorizontalAngle += CameraSpinSpeed;
					if (distanceToHorizontalDestination <= CameraSpinSpeed) {
						CameraHorizontalAngle = CameraValues[_cameraSpinningDestinationAngle];
						_cameraIsSpinningHorizontally = false;
					}
				} else {
					CameraHorizontalAngle -= CameraSpinSpeed;
					if (distanceToHorizontalDestination <= CameraSpinSpeed) {
						CameraHorizontalAngle = CameraValues[_cameraSpinningDestinationAngle];
						_cameraIsSpinningHorizontally = false;
					}
				}
			}

			if (_cameraIsSpinningVertically) {
				float destinationElevation = _cameraSpinningDestinationElevation == CameraElevation.Top
					? TopElevation
					: BottomElevation;
				double distanceToVerticalDestination = Math.Abs(CameraHeightAngle - destinationElevation);

				if (distanceToVerticalDestination <= CameraSpinSpeed) {
					CameraHeightAngle = destinationElevation;
					_cameraIsSpinningVertically = false;
				} else if (CameraHeightAngle < destinationElevation) {
					CameraHeightAngle += CameraSpinSpeed;
				} else {
					CameraHeightAngle -= CameraSpinSpeed;
				}
			}
		}

		private static void HandleInput() {
			if (!Stage.ScreenshotMode && (MyraGui.IsActive || !AppInput.MouseIsWithinModelViewport)) {
				return;
			}

			if (AppInput.MouseScrolledUp || AppInput.MouseScrolledDown) {
				HandleZooming();
			}

			bool swappedControls = Configuration.Properties.SwapCameraControls;

			bool handleRotation = !swappedControls && AppInput.MiddleMouseHeld ||
			                      swappedControls && AppInput.RightMouseHeld ||
			                      AppInput.AltHeld && !swappedControls && AppInput.RightMouseHeld ||
			                      AppInput.AltHeld && swappedControls && AppInput.MiddleMouseHeld;

			bool handlePanning = !swappedControls && AppInput.RightMouseHeld ||
			                     swappedControls && AppInput.MiddleMouseHeld ||
			                     AppInput.AltHeld && !swappedControls && AppInput.MiddleMouseHeld ||
			                     AppInput.AltHeld && swappedControls && AppInput.RightMouseHeld;

			if (handleRotation) {
				HandleRotation();
			} else if (handlePanning) {
				HandlePanning();
			}

			if (AppInput.KeyJustPressed(Keys.Up)) {
				SpinCameraTo(CameraElevation.Top);
			}

			if (AppInput.KeyJustPressed(Keys.Down)) {
				SpinCameraTo(CameraElevation.Bottom);
			}

			if (AppInput.KeyJustPressed(Keys.Left)) {
				if (CameraHorizontalAngle >= CameraValues[CameraView.Southwest] &&
				    CameraHorizontalAngle < CameraValues[CameraView.Northwest]
				) {
					SpinCameraTo(CameraView.Northwest);
				} else if (CameraHorizontalAngle >= CameraValues[CameraView.Northwest] &&
				           CameraHorizontalAngle < CameraValues[CameraView.Northeast]
				) {
					SpinCameraTo(CameraView.Northeast);
				} else if (CameraHorizontalAngle >= CameraValues[CameraView.Northeast] ||
				           CameraHorizontalAngle < CameraValues[CameraView.Southeast]
				) {
					SpinCameraTo(CameraView.Southeast);
				} else {
					SpinCameraTo(CameraView.Southwest);
				}
			}

			if (AppInput.KeyJustPressed(Keys.Right)) {
				if (CameraHorizontalAngle <= CameraValues[CameraView.Northwest] &&
				    CameraHorizontalAngle > CameraValues[CameraView.Southwest]
				) {
					SpinCameraTo(CameraView.Southwest);
				} else if (CameraHorizontalAngle <= CameraValues[CameraView.Southwest] &&
				           CameraHorizontalAngle > CameraValues[CameraView.Southeast]
				) {
					SpinCameraTo(CameraView.Southeast);
				} else if (CameraHorizontalAngle <= CameraValues[CameraView.Southeast] ||
				           CameraHorizontalAngle > CameraValues[CameraView.Northeast]
				) {
					SpinCameraTo(CameraView.Northeast);
				} else {
					SpinCameraTo(CameraView.Northwest);
				}
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
			_cameraIsSpinningHorizontally = false;
			_cameraIsSpinningVertically = false;

			int direction = Configuration.Properties.InvertedRotation
				? -1
				: 1;

			CameraHorizontalAngle += AppInput.MousePositionChange.X / 2f *
			                         Configuration.Properties.RotationSensitivity *
			                         direction;

			CameraHeightAngle -= AppInput.MousePositionChange.Y / 2f *
			                     Configuration.Properties.RotationSensitivity *
			                     direction;

			if (CameraHeightAngle > 180) {
				CameraHeightAngle = 180;
			}

			if (CameraHeightAngle < 1) {
				CameraHeightAngle = 1;
			}
		}

		private static void SetCameraState() {
			bool shouldSnapHeightAngle = Math.Abs(CameraHeightAngle - 90) < VerticalSnapStrength;

			double heightAngleRads = shouldSnapHeightAngle
				? MathHelper.ToRadians(90)
				: MathHelper.ToRadians((float) CameraHeightAngle);


			double horizontalAngleRads;

			if (Math.Abs(CameraHorizontalAngle - 90) < HorizontalSnapStrength) {
				horizontalAngleRads = MathHelper.ToRadians(90);
			} else if (Math.Abs(CameraHorizontalAngle - 270) < HorizontalSnapStrength) {
				horizontalAngleRads = MathHelper.ToRadians(270);
			} else if (Math.Abs(CameraHorizontalAngle - 180) < HorizontalSnapStrength) {
				horizontalAngleRads = MathHelper.ToRadians(180);
			} else if (CameraHorizontalAngle > 360 - HorizontalSnapStrength / 2f ||
			           CameraHorizontalAngle < HorizontalSnapStrength / 2f
			) {
				horizontalAngleRads = MathHelper.ToRadians(0);
			} else {
				horizontalAngleRads = MathHelper.ToRadians((float) CameraHorizontalAngle);
			}

			CamPosition = CamTarget + new Vector3(
				(float) (1000 * Math.Cos(horizontalAngleRads) * Math.Sin(heightAngleRads)),
				(float) (1000 * Math.Cos(heightAngleRads)),
				(float) (1000 * Math.Sin(horizontalAngleRads) * Math.Sin(heightAngleRads))
			);

			Stage.ViewMatrix = Matrix.CreateLookAt(CamPosition, CamTarget, Vector3.Up);

			float orthoDistortion = Configuration.Properties.RenderFFTOrtho
				? 5f / 4f
				: 1;

			Stage.ProjectionMatrix = Matrix.CreateOrthographic(
				Stage.ModelingViewport.Width * (float) ZoomLevel,
				Stage.ModelingViewport.Height * (float) ZoomLevel * orthoDistortion,
				1f,
				5000f
			);
		}
	}
}