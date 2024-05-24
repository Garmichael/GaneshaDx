using System;
using System.Collections.Generic;
using GaneshaDx.Common;
using GaneshaDx.Environment;
using GaneshaDx.Resources.ContentDataTypes.Polygons;
using GaneshaDx.UserInterface.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GaneshaDx.UserInterface.Widgets {
	public class RotationWidgetAxis {
		private readonly Axis _axis;
		private Vector3 _center;
		private readonly DepthStencilState _depthStencilState;

		private readonly List<Vector3> _arrowDefinition;
		private readonly List<Vector3> _axisVertices = new List<Vector3>();
		private VertexPositionColorTexture[] _axisPoleRenderVertices = { };

		private readonly List<Vector3> _rotationButtonDefinition;
		private readonly List<Vector3> _leftButtonVertices = new List<Vector3>();
		private readonly List<Vector3> _rightButtonVertices = new List<Vector3>();
		private VertexPositionColorTexture[] _leftButtonRenderVertices = { };
		private VertexPositionColorTexture[] _rightButtonRenderVertices = { };

		public CameraRayResults LeftButtonHoveredResults;
		public CameraRayResults RightButtonHoveredResults;
		public bool IsHovered => LeftButtonHovered || RightButtonHovered;
		private bool LeftButtonHovered => LeftButtonHoveredResults is { HasHit: true };
		private bool RightButtonHovered => RightButtonHoveredResults is { HasHit: true };
		private bool _leftButtonIsClockwise = true;
		private Vector3 AveragePoint => Utilities.GetAveragePoint(_axisVertices);
		public double DistanceToCamera => Vector3.Distance(StageCamera.CamPosition, AveragePoint);

		private const float ArrowWidth = 1;
		private const float ArrowLength = 60;
		private const float DistanceFromCenter = 10;
		private const float RotationButtonWidth = 15;
		private const float RotationButtonHeight = 20;
		private const float RotationButtonDistanceFromCenter = 18;
		private const float RotationButtonDepth = 6;

		private readonly Color _axisPoleColor = Color.LightGray;

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

		public RotationWidgetAxis(Axis axis) {
			_depthStencilState = new DepthStencilState {
				DepthBufferEnable = false
			};

			_axis = axis;

			_arrowDefinition = GetAxisArrowDefinition();
			_rotationButtonDefinition = GetRotationButtonDefinition();
		}

		public void Update() {
			SetCenter();

			BuildAxisPoleVertices();
			BuildRotationButtonVertices();

			if (!Gui.LockModeling) {
				HandleInput();
			}
		}

		private void SetCenter() {
			_center = Selection.CenterOfSelection;

			if (RotationWidget.AnchoredToVertex) {
				if (RotationWidget.AnchoredVertex > Selection.SelectedPolygons[0].Vertices.Count - 1) {
					RotationWidget.AnchoredVertex = 0;
				}

				_center = Selection.SelectedPolygons[0].Vertices[RotationWidget.AnchoredVertex].Position;
			}
		}

		private void BuildAxisPoleVertices() {
			float zoomAdjustedRadius = (float) StageCamera.ZoomLevel;

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

			if (_axisPoleRenderVertices.Length != _axisVertices.Count) {
				_axisPoleRenderVertices = new VertexPositionColorTexture[_axisVertices.Count];
			}

			for (int vertexIndex = 0; vertexIndex < _axisVertices.Count; vertexIndex++) {
				_axisPoleRenderVertices[vertexIndex] = new VertexPositionColorTexture(
					_center + _axisVertices[vertexIndex],
					_axisPoleColor,
					Vector2.Zero
				);
			}
		}

		private void BuildRotationButtonVertices() {
			float zoomAdjustedRadius = (float) StageCamera.ZoomLevel;
			_leftButtonVertices.Clear();
			_rightButtonVertices.Clear();

			switch (_axis) {
				case Axis.X: {
					_leftButtonIsClockwise = StageCamera.CamPosition.X < _center.X;
					float directionAdjustment = _leftButtonIsClockwise ? 0 : 180;
					Matrix viewRotation = Matrix.CreateRotationY(MathHelper.ToRadians(directionAdjustment));

					foreach (Vector3 vert in _rotationButtonDefinition) {
						Vector3 translated = vert + new Vector3(0, 0, RotationButtonDistanceFromCenter);
						_rightButtonVertices.Add(Vector3.Transform(translated * zoomAdjustedRadius, viewRotation));

						translated = vert - new Vector3(0, 0, RotationButtonDistanceFromCenter);
						Vector3 vertexPosition = Vector3.Transform(translated * zoomAdjustedRadius, viewRotation);
						_leftButtonVertices.Add(vertexPosition);
					}

					break;
				}
				case Axis.Z: {
					_leftButtonIsClockwise = StageCamera.CamPosition.Z <= _center.Z;
					float directionAdjustment = _leftButtonIsClockwise ? 180 : 0;
					Matrix viewRotation = Matrix.CreateRotationY(MathHelper.ToRadians(90 + directionAdjustment));

					foreach (Vector3 vert in _rotationButtonDefinition) {
						Vector3 translated = vert - new Vector3(0, 0, RotationButtonDistanceFromCenter);
						_leftButtonVertices.Add(Vector3.Transform(translated * zoomAdjustedRadius, viewRotation));

						translated = vert + new Vector3(0, 0, RotationButtonDistanceFromCenter);
						Vector3 vertexPosition = Vector3.Transform(translated * zoomAdjustedRadius, viewRotation);
						_rightButtonVertices.Add(vertexPosition);
					}

					break;
				}
				case Axis.Y: {
					_leftButtonIsClockwise = true;
					Matrix viewRotation = Matrix.CreateRotationZ(MathHelper.ToRadians(-90));

					foreach (Vector3 vert in _rotationButtonDefinition) {
						Vector3 translated = vert + new Vector3(0, 0, RotationButtonDistanceFromCenter);
						_rightButtonVertices.Add(Vector3.Transform(translated * zoomAdjustedRadius, viewRotation));

						translated = vert - new Vector3(0, 0, RotationButtonDistanceFromCenter);
						Vector3 vertexPosition = Vector3.Transform(translated * zoomAdjustedRadius, viewRotation);
						_leftButtonVertices.Add(vertexPosition);
					}

					break;
				}
				default:
					throw new ArgumentOutOfRangeException();
			}

			if (_leftButtonRenderVertices.Length != _leftButtonVertices.Count) {
				_leftButtonRenderVertices = new VertexPositionColorTexture[_leftButtonVertices.Count];
			}

			if (_rightButtonRenderVertices.Length != _rightButtonVertices.Count) {
				_rightButtonRenderVertices = new VertexPositionColorTexture[_rightButtonVertices.Count];
			}

			for (int vertexIndex = 0; vertexIndex < _leftButtonVertices.Count; vertexIndex++) {
				_leftButtonRenderVertices[vertexIndex] = new VertexPositionColorTexture(
					_center + _leftButtonVertices[vertexIndex],
					LeftButtonHovered ? HoverColor : NormalColor,
					Vector2.Zero
				);
			}

			for (int vertexIndex = 0; vertexIndex < _rightButtonVertices.Count; vertexIndex++) {
				_rightButtonRenderVertices[vertexIndex] = new VertexPositionColorTexture(
					_center + _rightButtonVertices[vertexIndex],
					RightButtonHovered ? HoverColor : NormalColor,
					Vector2.Zero
				);
			}
		}

		public void Render() {
			Stage.GraphicsDevice.DepthStencilState = _depthStencilState;
			Stage.GraphicsDevice.Viewport = Stage.ModelingViewport;
			Stage.BasicEffect.Alpha = 1f;
			Stage.BasicEffect.TextureEnabled = false;
			Stage.BasicEffect.VertexColorEnabled = true;

			if (_axisPoleRenderVertices.Length > 0 &&
			    _leftButtonVertices.Count > 0 &&
			    _rightButtonVertices.Count > 0
			   ) {
				RenderAxisPole();
				RenderButtons();
			}
		}

		private void RenderAxisPole() {
			Stage.UntexturedVertexBuffer.SetData(_axisPoleRenderVertices);
			Stage.GraphicsDevice.SetVertexBuffer(Stage.UntexturedVertexBuffer);

			foreach (EffectPass pass in Stage.BasicEffect.CurrentTechnique.Passes) {
				pass.Apply();
				Stage.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 12);
			}
		}

		private void RenderButtons() {
			float distanceToLeftButton = Vector3.Distance(
				StageCamera.CamPosition,
				Utilities.GetAveragePoint(_leftButtonVertices)
			);

			float distanceToRightButton = Vector3.Distance(
				StageCamera.CamPosition,
				Utilities.GetAveragePoint(_rightButtonVertices)
			);

			if (distanceToLeftButton > distanceToRightButton) {
				RenderLeftButton();
				RenderRightButton();
			} else {
				RenderRightButton();
				RenderLeftButton();
			}
		}

		private void RenderLeftButton() {
			Stage.UntexturedVertexBuffer.SetData(_leftButtonRenderVertices);
			Stage.GraphicsDevice.SetVertexBuffer(Stage.UntexturedVertexBuffer);

			foreach (EffectPass pass in Stage.BasicEffect.CurrentTechnique.Passes) {
				pass.Apply();
				Stage.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 8);
			}
		}

		private void RenderRightButton() {
			Stage.UntexturedVertexBuffer.SetData(_rightButtonRenderVertices);
			Stage.GraphicsDevice.SetVertexBuffer(Stage.UntexturedVertexBuffer);

			foreach (EffectPass pass in Stage.BasicEffect.CurrentTechnique.Passes) {
				pass.Apply();
				Stage.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 8);
			}
		}

		private void HandleInput() {
			LeftButtonHoveredResults = GetLeftButtonHoveredResults();
			RightButtonHoveredResults = GetRightButtonHoveredResults();

			if (LeftButtonHoveredResults.HasHit && RightButtonHoveredResults.HasHit) {
				float distanceToLeftButton = Vector3.Distance(
					StageCamera.CamPosition,
					Utilities.GetAveragePoint(_leftButtonVertices)
				);

				float distanceToRightButton = Vector3.Distance(
					StageCamera.CamPosition,
					Utilities.GetAveragePoint(_rightButtonVertices)
				);

				if (distanceToLeftButton < distanceToRightButton) {
					RightButtonHoveredResults = new CameraRayResults { HasHit = false };
				} else {
					LeftButtonHoveredResults = new CameraRayResults { HasHit = false };
				}
			}

			bool buttonIsHovered = LeftButtonHoveredResults.HasHit || RightButtonHoveredResults.HasHit;
			bool clicked = AppInput.LeftMouseClicked && buttonIsHovered;

			bool regularClicked = clicked && !AppInput.ShiftHeld && !AppInput.AltHeld;
			bool shiftClicked = clicked && AppInput.ShiftHeld;
			bool altClicked = clicked && AppInput.AltHeld;

			if (regularClicked) {
				foreach (Polygon polygon in Selection.SelectedPolygons) {
					foreach (Vertex vertex in polygon.Vertices) {
						float rotationDegrees = LeftButtonHoveredResults.HasHit && _leftButtonIsClockwise ||
						                        RightButtonHoveredResults.HasHit && !_leftButtonIsClockwise
							? 90
							: -90;

						Matrix rotation = _axis switch {
							Axis.X => Matrix.CreateRotationX(MathHelper.ToRadians(rotationDegrees)),
							Axis.Y => Matrix.CreateRotationY(MathHelper.ToRadians(-rotationDegrees)),
							_ => Matrix.CreateRotationZ(MathHelper.ToRadians(rotationDegrees))
						};

						vertex.Position -= _center;
						vertex.Position = Vector3.Transform(
							vertex.Position,
							rotation
						);
						vertex.Position += _center;

						vertex.Position = new Vector3(
							(float) Math.Round(vertex.Position.X, MidpointRounding.AwayFromZero),
							(float) Math.Round(vertex.Position.Y, MidpointRounding.AwayFromZero),
							(float) Math.Round(vertex.Position.Z, MidpointRounding.AwayFromZero)
						);
					}
				}
			} else if (shiftClicked) {
				foreach (Polygon polygon in Selection.SelectedPolygons) {
					foreach (Vertex vertex in polygon.Vertices) {
						if (_axis == Axis.X) {
							float distanceFromPlane = vertex.Position.Z - _center.Z;
							vertex.Position = new Vector3(
								vertex.Position.X,
								vertex.Position.Y,
								vertex.Position.Z - distanceFromPlane * 2
							);
						}

						if (_axis == Axis.Z) {
							float distanceFromPlane = vertex.Position.X - _center.X;
							vertex.Position = new Vector3(
								vertex.Position.X - distanceFromPlane * 2,
								vertex.Position.Y,
								vertex.Position.Z
							);
						}

						if (_axis == Axis.Y) {
							float distanceFromPlane = vertex.Position.Y - _center.Y;

							vertex.Position = new Vector3(
								vertex.Position.X,
								vertex.Position.Y - distanceFromPlane * 2,
								vertex.Position.Z
							);
						}
					}

					polygon.FlipNormals();
					polygon.FlipUvsHorizontally();
				}
			} else if (altClicked) {
				foreach (Polygon polygon in Selection.SelectedPolygons) {
					if (LeftButtonHoveredResults.HasHit && _leftButtonIsClockwise ||
					    RightButtonHoveredResults.HasHit && !_leftButtonIsClockwise
					   ) {
						polygon.RotateVerticesClockwise();
					} else {
						polygon.RotateVerticesCounterClockwise();
					}
				}
			}
		}

		private CameraRayResults GetLeftButtonHoveredResults() {
			for (int vertexIndex = 0; vertexIndex < _leftButtonVertices.Count; vertexIndex += 3) {
				CameraRayResults cameraRayResults = CameraRay.GetResults(
					new List<Vector3> {
						_leftButtonVertices[vertexIndex] + _center,
						_leftButtonVertices[vertexIndex + 1] + _center,
						_leftButtonVertices[vertexIndex + 2] + _center
					}
				);

				if (cameraRayResults.HasHit) {
					return cameraRayResults;
				}
			}

			return new CameraRayResults { HasHit = false };
		}

		private CameraRayResults GetRightButtonHoveredResults() {
			for (int vertexIndex = 0; vertexIndex < _rightButtonVertices.Count; vertexIndex += 3) {
				CameraRayResults cameraRayResults = CameraRay.GetResults(
					new List<Vector3> {
						_rightButtonVertices[vertexIndex] + _center,
						_rightButtonVertices[vertexIndex + 1] + _center,
						_rightButtonVertices[vertexIndex + 2] + _center
					}
				);

				if (cameraRayResults.HasHit) {
					return cameraRayResults;
				}
			}

			return new CameraRayResults { HasHit = false };
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

				new Vector3(-DistanceFromCenter - ArrowLength, -ArrowWidth, -ArrowWidth),
				new Vector3(-DistanceFromCenter - ArrowLength, ArrowWidth, -ArrowWidth),
				new Vector3(-DistanceFromCenter - ArrowLength, ArrowWidth, ArrowWidth),

				new Vector3(-DistanceFromCenter - ArrowLength, ArrowWidth, ArrowWidth),
				new Vector3(-DistanceFromCenter - ArrowLength, -ArrowWidth, ArrowWidth),
				new Vector3(-DistanceFromCenter - ArrowLength, -ArrowWidth, -ArrowWidth),
			};
		}

		private List<Vector3> GetRotationButtonDefinition() {
			const float frontPlaneX = -DistanceFromCenter - ArrowLength;
			const float backPlaneX = -DistanceFromCenter - ArrowLength + RotationButtonDepth;
			const float bottomPlaneY = -RotationButtonHeight;
			const float topPlaneY = RotationButtonHeight;
			const float leftPlaneZ = -RotationButtonWidth;
			const float rightPlaneZ = RotationButtonWidth;
			const float middlePlaneZ = 0;

			return new List<Vector3> {
				//front
				new Vector3(frontPlaneX, topPlaneY, middlePlaneZ),
				new Vector3(frontPlaneX, bottomPlaneY, rightPlaneZ),
				new Vector3(frontPlaneX, bottomPlaneY, leftPlaneZ),

				//back
				new Vector3(backPlaneX, bottomPlaneY, leftPlaneZ),
				new Vector3(backPlaneX, bottomPlaneY, rightPlaneZ),
				new Vector3(backPlaneX, topPlaneY, middlePlaneZ),

				//left
				new Vector3(backPlaneX, topPlaneY, middlePlaneZ),
				new Vector3(frontPlaneX, topPlaneY, middlePlaneZ),
				new Vector3(frontPlaneX, bottomPlaneY, leftPlaneZ),

				new Vector3(frontPlaneX, bottomPlaneY, leftPlaneZ),
				new Vector3(backPlaneX, bottomPlaneY, leftPlaneZ),
				new Vector3(backPlaneX, topPlaneY, middlePlaneZ),

				//right
				new Vector3(frontPlaneX, topPlaneY, middlePlaneZ),
				new Vector3(backPlaneX, topPlaneY, middlePlaneZ),
				new Vector3(backPlaneX, bottomPlaneY, rightPlaneZ),

				new Vector3(backPlaneX, bottomPlaneY, rightPlaneZ),
				new Vector3(frontPlaneX, bottomPlaneY, rightPlaneZ),
				new Vector3(frontPlaneX, topPlaneY, middlePlaneZ),

				//bottom
				new Vector3(backPlaneX, bottomPlaneY, rightPlaneZ),
				new Vector3(backPlaneX, bottomPlaneY, leftPlaneZ),
				new Vector3(frontPlaneX, bottomPlaneY, leftPlaneZ),

				new Vector3(frontPlaneX, bottomPlaneY, leftPlaneZ),
				new Vector3(frontPlaneX, bottomPlaneY, rightPlaneZ),
				new Vector3(backPlaneX, bottomPlaneY, rightPlaneZ),
			};
		}
	}
}