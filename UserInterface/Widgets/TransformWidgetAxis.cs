using System.Collections.Generic;
using GaneshaDx.Common;
using GaneshaDx.Environment;
using GaneshaDx.Resources;
using GaneshaDx.Resources.ContentDataTypes.Polygons;
using GaneshaDx.UserInterface.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GaneshaDx.UserInterface.Widgets {
	public class TransformWidgetAxis {
		private readonly Axis _axis;
		public CameraRayResults HoveredAxisResults;
		public bool IsHovered => HoveredAxisResults is {HasHit: true};
		public bool IsActive;
		public Vector3 DraggingStartPosition;
		public readonly List<Vector3> SelectionStartPositions = new List<Vector3>();
		public double DistanceToCamera => Vector3.Distance(StageCamera.CamPosition, AveragePoint);

		private Vector3 _center;
		private readonly DepthStencilState _depthStencilState;
		private VertexPositionColorTexture[] _axisRenderVertices;
		private readonly List<Vector3> _axisVertices = new List<Vector3>();
		private readonly List<Vector3> _planeVertices = new List<Vector3>();
		private readonly List<Vector3> _arrowDefinition;
		private readonly List<Vector3> _axisPlaneDefinition;

		private const float ArrowWidth = 4;
		private const float ArrowLength = 60;
		private const float DistanceFromCenter = 10;
		private const float ArrowPointLength = 20;
		private const float PLaneSize = 8000;

		private Color NormalColor {
			get {
				return _axis switch {
					Axis.X => Color.DarkRed,
					Axis.Y => Color.DarkGreen,
					_ => Color.DarkBlue
				};
			}
		}

		private Color HoverColor {
			get {
				return _axis switch {
					Axis.X => Color.Coral,
					Axis.Y => Color.PaleGreen,
					_ => Color.CornflowerBlue
				};
			}
		}

		private Vector3 AveragePoint => Utilities.GetAveragePoint(_axisVertices);

		public TransformWidgetAxis(Axis axis) {
			_depthStencilState = new DepthStencilState {
				DepthBufferEnable = false
			};

			_axis = axis;

			_arrowDefinition = GetAxisArrowDefinition();
			_axisPlaneDefinition = GetAxisPlaneDefinition();
		}

		public void Update() {
			_center = Gui.Widget switch {
				WidgetSelectionMode.PolygonVertexTranslate => Selection.SelectedPolygons[0]
					.Vertices[TransformWidget.SelectedVertexIndices[0]]
					.AnimationAdjustedPosition,
				WidgetSelectionMode.PolygonEdgeTranslate => Utilities.GetAveragePoint(new List<Vector3> {
					Selection.SelectedPolygons[0].Vertices[TransformWidget.SelectedVertexIndices[0]]
						.AnimationAdjustedPosition,
					Selection.SelectedPolygons[0].Vertices[TransformWidget.SelectedVertexIndices[1]]
						.AnimationAdjustedPosition,
				}),
				_ => Selection.CenterOfSelection
			};

			BuildAxisVertices();
			
			if (!MyraGui.LockModeling) {
				HandleInput();
			}

			if (IsActive) {
				if (Gui.Widget == WidgetSelectionMode.PolygonTranslate) {
					TranslatePolySelection();
				} else if (Gui.Widget == WidgetSelectionMode.PolygonVertexTranslate) {
					TranslateVertex();
				} else if (Gui.Widget == WidgetSelectionMode.PolygonEdgeTranslate) {
					TranslateEdge();
				}
			}
		}

		private void HandleInput() {
			HoveredAxisResults = GetHoveredAxisResults();

			if (HoveredAxisResults.HasHit && AppInput.LeftMouseClicked) {
				if (AppInput.ShiftHeld) {
					CurrentMapState.CloneSelection();
				}

				IsActive = true;
				DraggingStartPosition = GetHitPointOnPlane();

				SelectionStartPositions.Clear();
				foreach (Polygon polygon in Selection.SelectedPolygons) {
					SelectionStartPositions.Add(polygon.Vertices[0].Position);
				}
			}

			if (AppInput.LeftMouseReleased) {
				IsActive = false;
			}
		}

		private void TranslatePolySelection() {
			Vector3 pointOnPlane = GetHitPointOnPlane();
			Vector3 difference = pointOnPlane - DraggingStartPosition;

			if (AppInput.ControlHeld) {
				difference *= Configuration.Properties.TranslateAxisControlSpeed;
			}

			difference = AppInput.AltHeld
				? Utilities.SnapVector3ToGrid(difference, _axis == Axis.Y ? 12 : 28)
				: Utilities.RoundVector3ToInt(difference);

			if (_axis == Axis.X && difference.X != 0) {
				DraggingStartPosition = pointOnPlane;

				for (int polygonIndex = 0; polygonIndex < Selection.SelectedPolygons.Count; polygonIndex++) {
					Selection.SelectedPolygons[polygonIndex].SetNewPosition(
						new Vector3(
							SelectionStartPositions[polygonIndex].X + difference.X,
							Selection.SelectedPolygons[polygonIndex].Vertices[0].Position.Y,
							Selection.SelectedPolygons[polygonIndex].Vertices[0].Position.Z
						));
				}
			} else if (_axis == Axis.Y && difference.Y != 0) {
				DraggingStartPosition = pointOnPlane;

				for (int polygonIndex = 0; polygonIndex < Selection.SelectedPolygons.Count; polygonIndex++) {
					Selection.SelectedPolygons[polygonIndex].SetNewPosition(
						new Vector3(
							Selection.SelectedPolygons[polygonIndex].Vertices[0].Position.X,
							SelectionStartPositions[polygonIndex].Y + difference.Y,
							Selection.SelectedPolygons[polygonIndex].Vertices[0].Position.Z
						));
				}
			} else if (_axis == Axis.Z && difference.Z != 0) {
				DraggingStartPosition = pointOnPlane;

				for (int polygonIndex = 0; polygonIndex < Selection.SelectedPolygons.Count; polygonIndex++) {
					Selection.SelectedPolygons[polygonIndex].SetNewPosition(
						new Vector3(
							Selection.SelectedPolygons[polygonIndex].Vertices[0].Position.X,
							Selection.SelectedPolygons[polygonIndex].Vertices[0].Position.Y,
							SelectionStartPositions[polygonIndex].Z + difference.Z
						));
				}
			}

			SelectionStartPositions.Clear();
			foreach (Polygon polygon in Selection.SelectedPolygons) {
				SelectionStartPositions.Add(polygon.Vertices[0].Position);
			}
		}

		private void TranslateVertex() {
			int selectedVertIndex = TransformWidget.SelectedVertexIndices[0];
			Vector3 pointOnPlane = GetHitPointOnPlane();
			Vector3 difference = pointOnPlane - DraggingStartPosition;

			if (AppInput.ControlHeld) {
				difference *= Configuration.Properties.TranslateAxisControlSpeed;
			}

			difference = AppInput.AltHeld
				? Utilities.SnapVector3ToGrid(difference, _axis == Axis.Y ? 12 : 28)
				: Utilities.RoundVector3ToInt(difference);

			if (_axis == Axis.X && difference.X != 0) {
				DraggingStartPosition = pointOnPlane;

				Selection.SelectedPolygons[0].SetNewVertexPosition(
					selectedVertIndex,
					new Vector3(
						SelectionStartPositions[0].X + difference.X,
						Selection.SelectedPolygons[0].Vertices[selectedVertIndex].Position.Y,
						Selection.SelectedPolygons[0].Vertices[selectedVertIndex].Position.Z
					));
			} else if (_axis == Axis.Y && difference.Y != 0) {
				DraggingStartPosition = pointOnPlane;

				Selection.SelectedPolygons[0].SetNewVertexPosition(
					selectedVertIndex,
					new Vector3(
						Selection.SelectedPolygons[0].Vertices[selectedVertIndex].Position.X,
						SelectionStartPositions[0].Y + difference.Y,
						Selection.SelectedPolygons[0].Vertices[selectedVertIndex].Position.Z
					));
			} else if (_axis == Axis.Z && difference.Z != 0) {
				DraggingStartPosition = pointOnPlane;

				Selection.SelectedPolygons[0].SetNewVertexPosition(
					selectedVertIndex,
					new Vector3(
						Selection.SelectedPolygons[0].Vertices[selectedVertIndex].Position.X,
						Selection.SelectedPolygons[0].Vertices[selectedVertIndex].Position.Y,
						SelectionStartPositions[0].Z + difference.Z
					));
			}

			SelectionStartPositions.Clear();
			SelectionStartPositions.Add(Selection.SelectedPolygons[0].Vertices[selectedVertIndex].Position);
		}

		private void TranslateEdge() {
			Vector3 pointOnPlane = GetHitPointOnPlane();
			Vector3 difference = pointOnPlane - DraggingStartPosition;

			if (AppInput.ControlHeld) {
				difference *= Configuration.Properties.TranslateAxisControlSpeed;
			}

			difference = AppInput.AltHeld
				? Utilities.SnapVector3ToGrid(difference, _axis == Axis.Y ? 12 : 28)
				: Utilities.RoundVector3ToInt(difference);

			if (_axis == Axis.X && difference.X != 0) {
				DraggingStartPosition = pointOnPlane;
				int selectedVertIndex = TransformWidget.SelectedVertexIndices[0];

				Selection.SelectedPolygons[0].SetNewVertexPosition(
					selectedVertIndex,
					new Vector3(
						SelectionStartPositions[0].X + difference.X,
						Selection.SelectedPolygons[0].Vertices[selectedVertIndex].Position.Y,
						Selection.SelectedPolygons[0].Vertices[selectedVertIndex].Position.Z
					));

				selectedVertIndex = TransformWidget.SelectedVertexIndices[1];

				Selection.SelectedPolygons[0].SetNewVertexPosition(
					selectedVertIndex,
					new Vector3(
						SelectionStartPositions[1].X + difference.X,
						Selection.SelectedPolygons[0].Vertices[selectedVertIndex].Position.Y,
						Selection.SelectedPolygons[0].Vertices[selectedVertIndex].Position.Z
					));
			} else if (_axis == Axis.Y && difference.Y != 0) {
				DraggingStartPosition = pointOnPlane;
				int selectedVertIndex = TransformWidget.SelectedVertexIndices[0];

				Selection.SelectedPolygons[0].SetNewVertexPosition(
					selectedVertIndex,
					new Vector3(
						Selection.SelectedPolygons[0].Vertices[selectedVertIndex].Position.X,
						SelectionStartPositions[0].Y + difference.Y,
						Selection.SelectedPolygons[0].Vertices[selectedVertIndex].Position.Z
					));

				selectedVertIndex = TransformWidget.SelectedVertexIndices[1];

				Selection.SelectedPolygons[0].SetNewVertexPosition(
					selectedVertIndex,
					new Vector3(
						Selection.SelectedPolygons[0].Vertices[selectedVertIndex].Position.X,
						SelectionStartPositions[1].Y + difference.Y,
						Selection.SelectedPolygons[0].Vertices[selectedVertIndex].Position.Z
					));
			} else if (_axis == Axis.Z && difference.Z != 0) {
				DraggingStartPosition = pointOnPlane;
				int selectedVertIndex = TransformWidget.SelectedVertexIndices[0];

				Selection.SelectedPolygons[0].SetNewVertexPosition(
					selectedVertIndex,
					new Vector3(
						Selection.SelectedPolygons[0].Vertices[selectedVertIndex].Position.X,
						Selection.SelectedPolygons[0].Vertices[selectedVertIndex].Position.Y,
						SelectionStartPositions[0].Z + difference.Z
					));

				selectedVertIndex = TransformWidget.SelectedVertexIndices[1];

				Selection.SelectedPolygons[0].SetNewVertexPosition(
					selectedVertIndex,
					new Vector3(
						Selection.SelectedPolygons[0].Vertices[selectedVertIndex].Position.X,
						Selection.SelectedPolygons[0].Vertices[selectedVertIndex].Position.Y,
						SelectionStartPositions[1].Z + difference.Z
					));
			}

			SelectionStartPositions.Clear();
			foreach (Polygon polygon in Selection.SelectedPolygons) {
				SelectionStartPositions.Add(polygon.Vertices[TransformWidget.SelectedVertexIndices[0]].Position);
				SelectionStartPositions.Add(polygon.Vertices[TransformWidget.SelectedVertexIndices[1]].Position);
			}
		}

		private Vector3 GetHitPointOnPlane() {
			for (int vertexIndex = 0; vertexIndex < _planeVertices.Count; vertexIndex += 3) {
				CameraRayResults cameraRayResults = CameraRay.GetResults(
					new List<Vector3> {
						_planeVertices[vertexIndex] + _center,
						_planeVertices[vertexIndex + 1] + _center,
						_planeVertices[vertexIndex + 2] + _center
					},
					true
				);

				if (cameraRayResults.HasHit) {
					return cameraRayResults.HitPoint;
				}
			}

			return new Vector3(-1000, -1000, -1000);
		}

		private CameraRayResults GetHoveredAxisResults() {
			for (int vertexIndex = 0; vertexIndex < _axisVertices.Count; vertexIndex += 3) {
				CameraRayResults cameraRayResults = CameraRay.GetResults(
					new List<Vector3> {
						_axisVertices[vertexIndex] + _center,
						_axisVertices[vertexIndex + 1] + _center,
						_axisVertices[vertexIndex + 2] + _center
					}
				);

				if (cameraRayResults.HasHit
				) {
					return cameraRayResults;
				}
			}

			return new CameraRayResults {HasHit = false};
		}

		public void Render() {
			BuildAxisVertices();
			BuildPlaneVertices();

			Stage.UntexturedVertexBuffer.SetData(_axisRenderVertices);
			Stage.GraphicsDevice.SetVertexBuffer(Stage.UntexturedVertexBuffer);
			Stage.GraphicsDevice.DepthStencilState = _depthStencilState;
			Stage.GraphicsDevice.Viewport = Stage.ModelingViewport;

			Stage.BasicEffect.Alpha = 1f;
			Stage.BasicEffect.TextureEnabled = false;
			Stage.BasicEffect.VertexColorEnabled = true;

			foreach (EffectPass pass in Stage.BasicEffect.CurrentTechnique.Passes) {
				pass.Apply();
				Stage.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 16);
			}
		}

		private void BuildAxisVertices() {
			float zoomAdjustedRadius = (float) StageCamera.ZoomLevel;
			List<VertexPositionColorTexture> arrowVerts = new List<VertexPositionColorTexture>();

			if (_axis == Axis.X) {
				float directionAdjustment = StageCamera.CamPosition.X < _center.X ? 0 : 180;

				Matrix viewRotation = Matrix.CreateRotationY(
					MathHelper.ToRadians(directionAdjustment)
				);

				_axisVertices.Clear();
				foreach (Vector3 vert in _arrowDefinition) {
					_axisVertices.Add(Vector3.Transform(vert * zoomAdjustedRadius, viewRotation));
				}
			} else if (_axis == Axis.Y) {
				Matrix rotate = Matrix.CreateRotationZ(MathHelper.ToRadians(-90));
				_axisVertices.Clear();
				foreach (Vector3 vert in _arrowDefinition) {
					_axisVertices.Add(Vector3.Transform(vert * zoomAdjustedRadius, rotate));
				}
			} else if (_axis == Axis.Z) {
				float directionAdjustment = StageCamera.CamPosition.Z > _center.Z ? 0 : 180;
				Matrix rotate = Matrix.CreateRotationY(MathHelper.ToRadians(90 + directionAdjustment));
				_axisVertices.Clear();
				foreach (Vector3 vert in _arrowDefinition) {
					_axisVertices.Add(Vector3.Transform(vert * zoomAdjustedRadius, rotate));
				}
			}

			foreach (Vector3 vert in _axisVertices) {
				arrowVerts.Add(new VertexPositionColorTexture(
					_center + vert,
					IsHovered || IsActive ? HoverColor : NormalColor,
					Vector2.Zero
				));
			}


			_axisRenderVertices = arrowVerts.ToArray();
		}

		private void BuildPlaneVertices() {
			float zoomAdjustedRadius = (float) StageCamera.ZoomLevel;

			if (_axis == Axis.X) {
				Matrix rotateUp = Matrix.CreateRotationX(MathHelper.ToRadians(90));
				Matrix rotateDown = Matrix.CreateRotationX(MathHelper.ToRadians(0));
				_planeVertices.Clear();

				foreach (Vector3 vert in _axisPlaneDefinition) {
					Vector3 transformedPositionUp = Vector3.Transform(vert * zoomAdjustedRadius, rotateUp);
					_planeVertices.Add(transformedPositionUp);
				}

				foreach (Vector3 vert in _axisPlaneDefinition) {
					Vector3 transformedPositionDown = Vector3.Transform(vert * zoomAdjustedRadius, rotateDown);
					_planeVertices.Add(transformedPositionDown);
				}
			} else if (_axis == Axis.Z) {
				Matrix rotateUp = Matrix.CreateRotationZ(MathHelper.ToRadians(90));
				Matrix rotateDown = Matrix.CreateRotationZ(MathHelper.ToRadians(0));
				_planeVertices.Clear();

				foreach (Vector3 vert in _axisPlaneDefinition) {
					Vector3 transformedPositionUp = Vector3.Transform(vert * zoomAdjustedRadius, rotateUp);
					_planeVertices.Add(transformedPositionUp);
				}

				foreach (Vector3 vert in _axisPlaneDefinition) {
					Vector3 transformedPositionDown = Vector3.Transform(vert * zoomAdjustedRadius, rotateDown);
					_planeVertices.Add(transformedPositionDown);
				}
			} else if (_axis == Axis.Y) {
				Matrix rotate = Matrix.CreateRotationZ(MathHelper.ToRadians(-90));
				_planeVertices.Clear();

				foreach (Vector3 vert in _axisPlaneDefinition) {
					Vector3 transformedPosition = Vector3.Transform(vert * zoomAdjustedRadius, rotate);
					float angle = MathHelper.ToRadians((float) -StageCamera.CameraHorizontalAngle);
					Matrix faceCamera = Matrix.CreateRotationY(angle);
					transformedPosition = Vector3.Transform(transformedPosition, faceCamera);

					_planeVertices.Add(transformedPosition);
				}
			}
		}

		private List<Vector3> GetAxisArrowDefinition() {
			return new List<Vector3> {
				new Vector3(-DistanceFromCenter, -ArrowWidth, ArrowWidth),
				new Vector3(-DistanceFromCenter, ArrowWidth, ArrowWidth),
				new Vector3(-DistanceFromCenter, ArrowWidth, -ArrowWidth),

				new Vector3(-DistanceFromCenter, ArrowWidth, -ArrowWidth),
				new Vector3(-DistanceFromCenter, -ArrowWidth, -ArrowWidth),
				new Vector3(-DistanceFromCenter, -ArrowWidth, ArrowWidth),

				new Vector3(-DistanceFromCenter - ArrowLength, ArrowWidth, ArrowWidth),
				new Vector3(-DistanceFromCenter - ArrowLength, ArrowWidth, -ArrowWidth),
				new Vector3(-DistanceFromCenter, ArrowWidth, -ArrowWidth),

				new Vector3(-DistanceFromCenter, ArrowWidth, -ArrowWidth),
				new Vector3(-DistanceFromCenter, ArrowWidth, ArrowWidth),
				new Vector3(-DistanceFromCenter - ArrowLength, ArrowWidth, ArrowWidth),

				new Vector3(-DistanceFromCenter, ArrowWidth, -ArrowWidth),
				new Vector3(-DistanceFromCenter - ArrowLength, ArrowWidth, -ArrowWidth),
				new Vector3(-DistanceFromCenter, -ArrowWidth, -ArrowWidth),

				new Vector3(-DistanceFromCenter, -ArrowWidth, -ArrowWidth),
				new Vector3(-DistanceFromCenter - ArrowLength, ArrowWidth, -ArrowWidth),
				new Vector3(-DistanceFromCenter - ArrowLength, -ArrowWidth, -ArrowWidth),

				new Vector3(-DistanceFromCenter, -ArrowWidth, -ArrowWidth),
				new Vector3(-DistanceFromCenter - ArrowLength, -ArrowWidth, ArrowWidth),
				new Vector3(-DistanceFromCenter, -ArrowWidth, ArrowWidth),

				new Vector3(-DistanceFromCenter - ArrowLength, -ArrowWidth, -ArrowWidth),
				new Vector3(-DistanceFromCenter - ArrowLength, -ArrowWidth, ArrowWidth),
				new Vector3(-DistanceFromCenter, -ArrowWidth, -ArrowWidth),

				new Vector3(-DistanceFromCenter, -ArrowWidth, ArrowWidth),
				new Vector3(-DistanceFromCenter - ArrowLength, -ArrowWidth, ArrowWidth),
				new Vector3(-DistanceFromCenter - ArrowLength, ArrowWidth, ArrowWidth),

				new Vector3(-DistanceFromCenter - ArrowLength, ArrowWidth, ArrowWidth),
				new Vector3(-DistanceFromCenter, ArrowWidth, ArrowWidth),
				new Vector3(-DistanceFromCenter, -ArrowWidth, ArrowWidth),

				new Vector3(-DistanceFromCenter - ArrowLength, -ArrowWidth * 2, ArrowWidth * 2),
				new Vector3(-DistanceFromCenter - ArrowLength, ArrowWidth * 2, ArrowWidth * 2),
				new Vector3(-DistanceFromCenter - ArrowLength, ArrowWidth * 2, -ArrowWidth * 2),

				new Vector3(-DistanceFromCenter - ArrowLength, ArrowWidth * 2, -ArrowWidth * 2),
				new Vector3(-DistanceFromCenter - ArrowLength, -ArrowWidth * 2, -ArrowWidth * 2),
				new Vector3(-DistanceFromCenter - ArrowLength, -ArrowWidth * 2, ArrowWidth * 2),

				new Vector3(-DistanceFromCenter - ArrowLength, ArrowWidth * 2, ArrowWidth * 2),
				new Vector3(-DistanceFromCenter - ArrowLength - ArrowPointLength, 0, 0),
				new Vector3(-DistanceFromCenter - ArrowLength, ArrowWidth * 2, -ArrowWidth * 2),

				new Vector3(-DistanceFromCenter - ArrowLength, ArrowWidth * 2, -ArrowWidth * 2),
				new Vector3(-DistanceFromCenter - ArrowLength - ArrowPointLength, 0, 0),
				new Vector3(-DistanceFromCenter - ArrowLength, -ArrowWidth * 2, -ArrowWidth * 2),

				new Vector3(-DistanceFromCenter - ArrowLength, -ArrowWidth * 2, -ArrowWidth * 2),
				new Vector3(-DistanceFromCenter - ArrowLength - ArrowPointLength, 0, 0),
				new Vector3(-DistanceFromCenter - ArrowLength, -ArrowWidth * 2, ArrowWidth * 2),

				new Vector3(-DistanceFromCenter - ArrowLength, -ArrowWidth * 2, ArrowWidth * 2),
				new Vector3(-DistanceFromCenter - ArrowLength - ArrowPointLength, 0, 0),
				new Vector3(-DistanceFromCenter - ArrowLength, ArrowWidth * 2, ArrowWidth * 2),
			};
		}

		private List<Vector3> GetAxisPlaneDefinition() {
			return new List<Vector3> {
				new Vector3(PLaneSize, 0, -PLaneSize),
				new Vector3(PLaneSize, 0, PLaneSize),
				new Vector3(-PLaneSize, 0, -PLaneSize),

				new Vector3(PLaneSize, 0, PLaneSize),
				new Vector3(-PLaneSize, 0, PLaneSize),
				new Vector3(-PLaneSize, 0, -PLaneSize),
			};
		}
	}
}