using System;
using System.Collections.Generic;
using System.Linq;
using GaneshaDx.Common;
using GaneshaDx.Environment;
using GaneshaDx.Rendering;
using GaneshaDx.Resources;
using GaneshaDx.Resources.ContentDataTypes.Polygons;
using GaneshaDx.Resources.ContentDataTypes.TextureAnimations;
using GaneshaDx.UserInterface.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GaneshaDx.UserInterface.GuiForms {
	public static class GuiWindowTextureElement {
		private const int TextureSize = 256;
		private static Matrix _viewMatrix;
		private static Matrix _projectionMatrix;
		private static Matrix _projectionMatrixForExportedUvMap;
		private static int _texturePage = 1;
		private static int _paletteId = 2;
		private static float _zoomLevel = 1;
		private const float ZoomStrength = 1.2f;
		private static Vector2 _cameraCenter;
		private const float VertexIndicatorSize = 5;
		private static readonly List<Polygon> HoveredPolygons = new List<Polygon>();
		private static readonly List<Vector2> HoveredVertices = new List<Vector2>();

		private static Vector3 CameraPosition => new Vector3(
			TextureSize / 2f + _cameraCenter.X,
			TextureSize / 2f + _cameraCenter.Y, 0
		);

		private static Vector3 CameraTarget => new Vector3(
			TextureSize / 2f + _cameraCenter.X,
			TextureSize / 2f + _cameraCenter.Y, 1
		);

		private static float CameraSize => TextureSize * _zoomLevel / 2;

		private static Vector2 PanningOffset => new Vector2(
			TextureSize / 2f - CameraSize + _cameraCenter.X,
			TextureSize / 2f - CameraSize + _cameraCenter.Y
		);

		private static Vector2 AdjustedMousePosition => new Vector2(
			(AppInput.ThisMouseState.Position.X - GuiWindowTexturePreview.UvPreviewBounds.X) *
			(TextureSize / (float) GuiWindowTexturePreview.UvPreviewBounds.Width) *
			_zoomLevel + PanningOffset.X,
			(AppInput.ThisMouseState.Position.Y - GuiWindowTexturePreview.UvPreviewBounds.Y) *
			(TextureSize / (float) GuiWindowTexturePreview.UvPreviewBounds.Height) *
			_zoomLevel + PanningOffset.Y
		);

		private static Vector2 PixelMousePosition => new Vector2(
			(float) Math.Floor(AdjustedMousePosition.X),
			(float) Math.Floor(AdjustedMousePosition.Y)
		);

		private static Vector2 _mouseDownPoint;
		private static Vector2 _lastMousePosition;
		private static bool _mouseIsDown;
		private static bool _mouseIsDragging;
		private static bool _draggingVertex;
		private static Vector2 _draggedVertex;

		public static void Update() {
			if (MapData.MapIsLoaded && GuiWindowTexturePreview.ShouldRenderTexture) {
				SetTextureProperties();
				HandleInput();
			}
		}

		private static void SetTextureProperties() {
			bool isUvAnimationMode = GuiPanelTexture.CurrentPanelMode == GuiPanelTexture.PanelMode.UvAnimation;
			if (!isUvAnimationMode && Selection.SelectedPolygons.Count > 0) {
				_texturePage = Selection.SelectedPolygons[0].TexturePage;
				_paletteId = Selection.SelectedPolygons[0].PaletteId;
			} else if (isUvAnimationMode) {
				UvAnimation animation = (UvAnimation) GuiPanelTexture.SelectedTextureAnimation;
				_texturePage = GuiPanelTexture.TextureAnimationPreviewPageId == 0
					? animation.CanvasTexturePage
					: animation.FirstFrameTexturePage;

				_paletteId = GuiPanelTexture.TextureAnimationPreviewPaletteId;
			}
		}

		private static void HandleInput() {
			if (!GuiWindowTexturePreview.IsMouseHovered) {
				return;
			}

			CollectHoveredPolygons();
			CollectHoveredVertices();
			HandleLeftMouseAndPosition();
			HandlePanning();
			HandleZooming();
			HandleVertexSelectionAndDragging();
			HandlePolygonSelectionAndDragging();

			_lastMousePosition = PixelMousePosition;
		}

		private static void CollectHoveredPolygons() {
			HoveredPolygons.Clear();

			foreach (Polygon polygon in CurrentMapState.StateData.PolygonCollectionBucket) {
				if (polygon.IsSelected || !polygon.IsTextured || polygon.TexturePage != _texturePage) {
					continue;
				}

				bool isHovered = Utilities.PointIsWithinTriangle(
					AdjustedMousePosition,
					polygon.UvCoordinates[0],
					polygon.UvCoordinates[1],
					polygon.UvCoordinates[2]
				);

				if (!isHovered && polygon.IsQuad) {
					isHovered = Utilities.PointIsWithinTriangle(
						AdjustedMousePosition,
						polygon.UvCoordinates[3],
						polygon.UvCoordinates[2],
						polygon.UvCoordinates[1]
					);
				}

				if (isHovered) {
					HoveredPolygons.Add(polygon);
				}
			}
		}

		private static void CollectHoveredVertices() {
			HoveredVertices.Clear();

			foreach (Polygon polygon in Selection.SelectedPolygons) {
				if (polygon.IsTextured) {
					foreach (Vector2 uvCoordinate in polygon.UvCoordinates) {
						bool isHovered = Utilities.PointIsWithinRange(
							AdjustedMousePosition,
							uvCoordinate,
							VertexIndicatorSize * _zoomLevel
						);

						if (isHovered) {
							HoveredVertices.Add(uvCoordinate);
						}
					}
				}
			}
		}

		private static void HandleLeftMouseAndPosition() {
			if (AppInput.LeftMouseClicked) {
				_mouseDownPoint = PixelMousePosition;
				_lastMousePosition = PixelMousePosition;
				_mouseIsDown = true;
				_mouseIsDragging = false;

				if (HoveredVertices.Count > 0) {
					_mouseIsDragging = true;
					_draggingVertex = true;
					_draggedVertex = HoveredVertices[0];
				} else {
					_draggingVertex = false;
				}
			}

			if (_mouseIsDown && _mouseDownPoint != PixelMousePosition) {
				_mouseIsDragging = true;
			}

			if (AppInput.LeftMouseReleased && _mouseIsDown && !_mouseIsDragging) {
				if (!_draggingVertex) {
					if (HoveredPolygons.Count > 0) {
						Selection.SelectPolygon(HoveredPolygons[0]);
					} else {
						Selection.SelectedPolygons.Clear();
					}
				}

				_mouseIsDown = false;
				_draggingVertex = false;
			}

			if (AppInput.LeftMouseReleased && _mouseIsDown && _mouseIsDragging) {
				_mouseIsDown = false;
				_mouseIsDragging = false;
				_draggingVertex = false;
			}
		}

		private static void HandlePanning() {
			if (AppInput.ThisMouseState.RightButton == ButtonState.Pressed ||
			    AppInput.ThisMouseState.MiddleButton == ButtonState.Pressed
			) {
				float windowSizeOffset = 256f / GuiWindowTexturePreview.UvPreviewBounds.Width;
				int speedModifier = AppInput.ShiftHeld ? 2 : 1;
				_cameraCenter.X -= AppInput.MousePositionChange.X * _zoomLevel * windowSizeOffset * speedModifier;
				_cameraCenter.Y -= AppInput.MousePositionChange.Y * _zoomLevel * windowSizeOffset * speedModifier;
				ClampCameraWithinTexture();
			}
		}

		private static void HandleZooming() {
			if (AppInput.MouseScrolledDown || AppInput.MouseScrolledUp) {
				if (AppInput.MouseScrolledUp) {
					_zoomLevel *= ZoomStrength;
				}

				if (AppInput.MouseScrolledDown) {
					_zoomLevel /= ZoomStrength;
				}

				if (_zoomLevel < 0) {
					_zoomLevel = 0;
				}

				if (_zoomLevel > 1) {
					_zoomLevel = 1;
				}

				ClampCameraWithinTexture();
			}
		}

		private static void ClampCameraWithinTexture() {
			_cameraCenter.X = Utilities.Clamp(_cameraCenter.X, CameraSize - 128, 128 - CameraSize);
			_cameraCenter.Y = Utilities.Clamp(_cameraCenter.Y, CameraSize - 128, 128 - CameraSize);
		}

		private static void HandleVertexSelectionAndDragging() {
			if (_draggingVertex) {
				foreach (Polygon polygon in Selection.SelectedPolygons) {
					if (polygon.IsTextured) {
						for (int uvIndex = 0; uvIndex < polygon.UvCoordinates.Count; uvIndex++) {
							if (
								Math.Abs(_draggedVertex.X - polygon.UvCoordinates[uvIndex].X) < 0.001f &&
								Math.Abs(_draggedVertex.Y - polygon.UvCoordinates[uvIndex].Y) < 0.001f
							) {
								polygon.UvCoordinates[uvIndex] += PixelMousePosition - _lastMousePosition;
							}
						}
					}
				}

				_draggedVertex += PixelMousePosition - _lastMousePosition;
			}
		}

		private static void HandlePolygonSelectionAndDragging() {
			if (_mouseIsDragging && !_draggingVertex) {
				foreach (Polygon polygon in Selection.SelectedPolygons) {
					if (polygon.IsTextured) {
						for (int uvIndex = 0; uvIndex < polygon.UvCoordinates.Count; uvIndex++) {
							polygon.UvCoordinates[uvIndex] += PixelMousePosition - _lastMousePosition;
						}
					}
				}
			}

			if (Selection.SelectedPolygons.Count > 0) {
				int multiplier = AppInput.ShiftHeld ? Configuration.Properties.TexturePreviewShiftArrowDistance : 1;

				if (AppInput.KeyJustPressed(Keys.Left)) {
					foreach (Polygon polygon in Selection.SelectedPolygons) {
						if (polygon.IsTextured) {
							for (int uvIndex = 0; uvIndex < polygon.UvCoordinates.Count; uvIndex++) {
								polygon.UvCoordinates[uvIndex] -= new Vector2(1, 0) * multiplier;
							}
						}
					}
				}

				if (AppInput.KeyJustPressed(Keys.Right)) {
					foreach (Polygon polygon in Selection.SelectedPolygons) {
						if (polygon.IsTextured) {
							for (int uvIndex = 0; uvIndex < polygon.UvCoordinates.Count; uvIndex++) {
								polygon.UvCoordinates[uvIndex] += new Vector2(1, 0) * multiplier;
							}
						}
					}
				}

				if (AppInput.KeyJustPressed(Keys.Up)) {
					foreach (Polygon polygon in Selection.SelectedPolygons) {
						if (polygon.IsTextured) {
							for (int uvIndex = 0; uvIndex < polygon.UvCoordinates.Count; uvIndex++) {
								polygon.UvCoordinates[uvIndex] -= new Vector2(0, 1) * multiplier;
							}
						}
					}
				}

				if (AppInput.KeyJustPressed(Keys.Down)) {
					foreach (Polygon polygon in Selection.SelectedPolygons) {
						if (polygon.IsTextured) {
							for (int uvIndex = 0; uvIndex < polygon.UvCoordinates.Count; uvIndex++) {
								polygon.UvCoordinates[uvIndex] += new Vector2(0, 1) * multiplier;
							}
						}
					}
				}

				if (AppInput.KeyJustPressed(Keys.OemPlus) || AppInput.KeyJustPressed(Keys.Add)) {
					Selection.GrowUvSelection();
				}
			}
		}

		public static void Render() {
			if (!GuiWindowTexturePreview.ShouldRenderTexture) {
				return;
			}

			_projectionMatrix = Matrix.CreateOrthographic(TextureSize * _zoomLevel, TextureSize * _zoomLevel, -10f, 10);
			_projectionMatrixForExportedUvMap = Matrix.CreateOrthographic(TextureSize, TextureSize, -10f, 10);
			_viewMatrix = Matrix.CreateLookAt(CameraPosition, CameraTarget, new Vector3(0, -1, 0));

			Stage.GraphicsDevice.Clear(Color.DarkCyan);

			SetPolygonEffectForTexture();
			RenderTextureImage();

			if (GuiPanelTexture.CurrentPanelMode != GuiPanelTexture.PanelMode.UvAnimation) {
				RenderPolygons();
			} else {
				RenderAnimation();
			}
		}

		private static void SetPolygonEffectForTexture() {
			Vector4 neutralLight = new Vector4(0.5f, 0.5f, 0.5f, 1f);
			Vector4[] shaderColors = SceneRenderer.AnimationAdjustedPalettes[_paletteId].ShaderColors;
			Stage.FftPolygonEffect.Parameters["Projection"].SetValue(_projectionMatrix);
			Stage.FftPolygonEffect.Parameters["View"].SetValue(_viewMatrix);
			Stage.FftPolygonEffect.Parameters["ModelTexture"].SetValue(CurrentMapState.StateData.Texture);
			Stage.FftPolygonEffect.Parameters["PaletteColors"].SetValue(shaderColors);
			Stage.FftPolygonEffect.Parameters["AmbientColor"].SetValue(neutralLight);
			Stage.FftPolygonEffect.Parameters["DirectionalLightDirection0"].SetValue(Vector3.Down);
			Stage.FftPolygonEffect.Parameters["DirectionalLightDirection1"].SetValue(Vector3.Down);
			Stage.FftPolygonEffect.Parameters["DirectionalLightDirection2"].SetValue(Vector3.Down);
			Stage.FftPolygonEffect.Parameters["DirectionalLightColor0"].SetValue(neutralLight);
			Stage.FftPolygonEffect.Parameters["DirectionalLightColor1"].SetValue(neutralLight);
			Stage.FftPolygonEffect.Parameters["DirectionalLightColor2"].SetValue(neutralLight);
			Stage.FftPolygonEffect.Parameters["HighlightBright"].SetValue(false);
			Stage.FftPolygonEffect.Parameters["HighlightDim"].SetValue(false);
			Stage.FftPolygonEffect.Parameters["MaxAlpha"].SetValue(1f);
		}

		private static void RenderTextureImage() {
			Stage.PolygonVertexBuffer.SetData(BuildTextureRenderVertices());
			Stage.GraphicsDevice.SetVertexBuffer(Stage.PolygonVertexBuffer);
			Stage.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

			foreach (EffectPass pass in Stage.FftPolygonEffect.CurrentTechnique.Passes) {
				pass.Apply();
				Stage.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
			}
		}

		private static VertexPositionNormalTexture[] BuildTextureRenderVertices() {
			VertexPositionNormalTexture[] textureVerts = {
				new VertexPositionNormalTexture(),
				new VertexPositionNormalTexture(),
				new VertexPositionNormalTexture(),
				new VertexPositionNormalTexture()
			};

			textureVerts[0].Position = new Vector3(0, 0, 1);
			textureVerts[1].Position = new Vector3(TextureSize, 0, 1);
			textureVerts[3].Position = new Vector3(TextureSize, TextureSize, 1);
			textureVerts[2].Position = new Vector3(0, TextureSize, 1);

			double top = _texturePage / 4f;
			double bottom = _texturePage / 4f + 1 / 4f;
			textureVerts[0].TextureCoordinate = new Vector2(0, (float) top);
			textureVerts[1].TextureCoordinate = new Vector2(1, (float) top);
			textureVerts[3].TextureCoordinate = new Vector2(1, (float) bottom);
			textureVerts[2].TextureCoordinate = new Vector2(0, (float) bottom);

			return textureVerts;
		}

		private static void RenderPolygons() {
			if (Configuration.Properties.TexturePreviewShowUnselectedUvs) {
				foreach (Polygon polygon in CurrentMapState.StateData.PolygonCollectionBucket) {
					if (RenderPolyAsUnselected(polygon)) {
						RenderPolygonUvs(polygon);
					}
				}

				foreach (Polygon polygon in CurrentMapState.StateData.PolygonCollectionBucket) {
					if (RenderPolyAsHovered(polygon)) {
						RenderPolygonUvs(polygon);
					}
				}
			}

			foreach (Polygon polygon in CurrentMapState.StateData.PolygonCollectionBucket) {
				if (RenderPolyAsSelected(polygon)) {
					RenderPolygonUvs(polygon);
				}
			}
		}

		private static bool RenderPolyAsUnselected(Polygon polygon) {
			return PolygonIsValidToRender(polygon) &&
			       !PolygonIsSelected(polygon) &&
			       (!PolygonIsHovered(polygon) || HoveredVertices.Count > 0);
		}

		private static bool RenderPolyAsHovered(Polygon polygon) {
			return PolygonIsValidToRender(polygon) &&
			       !PolygonIsSelected(polygon) &&
			       PolygonIsHovered(polygon) &&
			       HoveredVertices.Count == 0;
		}

		private static bool RenderPolyAsSelected(Polygon polygon) {
			return PolygonIsValidToRender(polygon) &&
			       polygon.IsSelected;
		}

		private static bool PolygonIsSelected(Polygon polygon) {
			return polygon.IsSelected;
		}

		private static bool PolygonIsHovered(Polygon polygon) {
			return HoveredPolygons.Contains(polygon);
		}

		private static bool PolygonIsValidToRender(Polygon polygon) {
			return polygon.IsTextured && polygon.TexturePage == _texturePage;
		}

		private static void RenderPolygonUvs(Polygon polygon) {
			SetPolygonBasicEffectProperties(polygon);

			foreach (EffectPass pass in Stage.BasicEffect.CurrentTechnique.Passes) {
				pass.Apply();
				Stage.GraphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, 0, polygon.IsQuad ? 4 : 3);
			}

			if (polygon.IsSelected) {
				RenderPolygonVertices(polygon);
			}
		}

		private static void SetPolygonBasicEffectProperties(Polygon polygon) {
			Stage.BasicEffect.View = _viewMatrix;
			Stage.BasicEffect.Projection = _projectionMatrix;
			Stage.BasicEffect.Alpha = 1f;
			Stage.BasicEffect.TextureEnabled = true;
			Stage.BasicEffect.VertexColorEnabled = false;
			Stage.BasicEffect.Texture = RenderPolyAsSelected(polygon)
				? UniversalTextures.GreenTexture
				: RenderPolyAsHovered(polygon)
					? UniversalTextures.RedTexture
					: UniversalTextures.WhiteTexture;

			Stage.PolygonVertexBuffer.SetData(BuildPolygonRenderVertices(polygon));
			Stage.GraphicsDevice.SetVertexBuffer(Stage.PolygonVertexBuffer);
			Stage.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
		}

		private static VertexPositionNormalTexture[] BuildPolygonRenderVertices(Polygon polygon) {
			VertexPositionNormalTexture[] polyVerts = new VertexPositionNormalTexture[polygon.IsQuad ? 5 : 4];

			if (polygon.IsQuad) {
				polyVerts[0] = new VertexPositionNormalTexture
					{ Position = new Vector3(polygon.UvCoordinates[0], 0.9f) };
				polyVerts[1] = new VertexPositionNormalTexture
					{ Position = new Vector3(polygon.UvCoordinates[2], 0.9f) };
				polyVerts[2] = new VertexPositionNormalTexture
					{ Position = new Vector3(polygon.UvCoordinates[3], 0.9f) };
				polyVerts[3] = new VertexPositionNormalTexture
					{ Position = new Vector3(polygon.UvCoordinates[1], 0.9f) };
				polyVerts[4] = new VertexPositionNormalTexture
					{ Position = new Vector3(polygon.UvCoordinates[0], 0.9f) };
			} else {
				polyVerts[0] = new VertexPositionNormalTexture
					{ Position = new Vector3(polygon.UvCoordinates[0], 0.9f) };
				polyVerts[1] = new VertexPositionNormalTexture
					{ Position = new Vector3(polygon.UvCoordinates[1], 0.9f) };
				polyVerts[2] = new VertexPositionNormalTexture
					{ Position = new Vector3(polygon.UvCoordinates[2], 0.9f) };
				polyVerts[3] = new VertexPositionNormalTexture
					{ Position = new Vector3(polygon.UvCoordinates[0], 0.9f) };
			}

			return polyVerts;
		}

		private static void RenderPolygonVertices(Polygon polygon) {
			RenderPolygonVertex(
				new Vector3(polygon.UvCoordinates[0].X, polygon.UvCoordinates[0].Y, 0), Color.Red, Color.Pink
			);

			RenderPolygonVertex(
				new Vector3(polygon.UvCoordinates[1].X, polygon.UvCoordinates[1].Y, 0), Color.Green, Color.LightGreen
			);

			RenderPolygonVertex(
				new Vector3(polygon.UvCoordinates[2].X, polygon.UvCoordinates[2].Y, 0), Color.Blue, Color.Cyan
			);

			if (polygon.IsQuad) {
				RenderPolygonVertex(
					new Vector3(polygon.UvCoordinates[3].X, polygon.UvCoordinates[3].Y, 0),
					Color.Yellow,
					Color.LightYellow
				);
			}
		}

		private static void RenderPolygonVertex(Vector3 position, Color color, Color hoverColor) {
			SetVertexBasicEffectProperties(position, color, hoverColor);

			foreach (EffectPass pass in Stage.BasicEffect.CurrentTechnique.Passes) {
				pass.Apply();
				Stage.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
			}
		}

		private static void SetVertexBasicEffectProperties(Vector3 position, Color color, Color hoverColor) {
			Stage.BasicEffect.View = _viewMatrix;
			Stage.BasicEffect.Projection = _projectionMatrix;
			Stage.BasicEffect.Alpha = 1f;
			Stage.BasicEffect.TextureEnabled = false;
			Stage.BasicEffect.VertexColorEnabled = true;

			Stage.UntexturedVertexBuffer.SetData(SetVertexRenderVertices(position, color, hoverColor));
			Stage.GraphicsDevice.SetVertexBuffer(Stage.UntexturedVertexBuffer);
			Stage.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
		}

		private static VertexPositionColorTexture[] SetVertexRenderVertices(
			Vector3 center,
			Color color,
			Color hoverColor
		) {
			bool isHovered = false;

			foreach (Vector2 vertexPosition in HoveredVertices) {
				if (
					Math.Abs(vertexPosition.X - center.X) < 0.001f &&
					Math.Abs(vertexPosition.Y - center.Y) < 0.001f
				) {
					isHovered = true;
					break;
				}
			}

			return new[] {
				new VertexPositionColorTexture {
					Position = center + new Vector3(0, -VertexIndicatorSize * _zoomLevel, -2),
					Color = isHovered ? hoverColor : color
				},
				new VertexPositionColorTexture {
					Position = center + new Vector3(VertexIndicatorSize * _zoomLevel, 0, -2),
					Color = isHovered ? hoverColor : color
				},
				new VertexPositionColorTexture {
					Position = center + new Vector3(0, VertexIndicatorSize * _zoomLevel, -2),
					Color = isHovered ? hoverColor : color
				},
				new VertexPositionColorTexture {
					Position = center + new Vector3(0, VertexIndicatorSize * _zoomLevel, -2),
					Color = isHovered ? hoverColor : color
				},

				new VertexPositionColorTexture {
					Position = center + new Vector3(-VertexIndicatorSize * _zoomLevel, 0, -2),
					Color = isHovered ? hoverColor : color
				},

				new VertexPositionColorTexture {
					Position = center + new Vector3(0, -VertexIndicatorSize * _zoomLevel, -2),
					Color = isHovered ? hoverColor : color
				},
			};
		}

		private static void RenderAnimation() {
			UvAnimation selectedAnimation = (UvAnimation) GuiPanelTexture.SelectedTextureAnimation;

			if (selectedAnimation.CanvasTexturePage == _texturePage) {
				SetAnimationFrameBasicEffectProperties(selectedAnimation, "canvas", 0);

				foreach (EffectPass pass in Stage.BasicEffect.CurrentTechnique.Passes) {
					pass.Apply();
					Stage.GraphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, 0, 4);
				}
			}

			if (selectedAnimation.FirstFrameTexturePage == _texturePage) {
				for (int frameIndex = 0; frameIndex < selectedAnimation.FrameCount; frameIndex++) {
					SetAnimationFrameBasicEffectProperties(selectedAnimation, "frame", frameIndex);

					foreach (EffectPass pass in Stage.BasicEffect.CurrentTechnique.Passes) {
						pass.Apply();
						Stage.GraphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, 0, 4);
					}
				}
			}
		}

		private static void SetAnimationFrameBasicEffectProperties(UvAnimation animation, string type, int frameId) {
			Stage.BasicEffect.View = _viewMatrix;
			Stage.BasicEffect.Projection = _projectionMatrix;
			Stage.BasicEffect.Alpha = 1f;
			Stage.BasicEffect.TextureEnabled = true;
			Stage.BasicEffect.VertexColorEnabled = false;
			Stage.BasicEffect.Texture = type == "canvas"
				? UniversalTextures.RedTexture
				: UniversalTextures.GreenTexture;

			Stage.PolygonVertexBuffer.SetData(BuildAnimationFrameRenderVertices(type, animation, frameId));
			Stage.GraphicsDevice.SetVertexBuffer(Stage.PolygonVertexBuffer);
			Stage.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
		}

		private static VertexPositionNormalTexture[] BuildAnimationFrameRenderVertices(
			string type,
			UvAnimation animation,
			int frameId
		) {
			VertexPositionNormalTexture[] polyVerts = new VertexPositionNormalTexture[5];

			Vector3 topLeft;
			Vector3 topRight;
			Vector3 bottomLeft;
			Vector3 bottomRight;

			int totalFramesForTopRow = (int) Math.Floor((float) (256 - animation.FirstFrameX) / animation.SizeWidth);
			int totalFramesForNextRows = (int) Math.Floor((float) 256 / animation.SizeWidth);

			int row = frameId < totalFramesForTopRow
				? 0
				: (int) Math.Floor((float) (frameId - totalFramesForTopRow) / totalFramesForNextRows) + 1;

			if (row == 0) {
				topLeft = new Vector3(
					(type == "canvas" ? animation.CanvasX : animation.FirstFrameX) + frameId * animation.SizeWidth,
					type == "canvas" ? animation.CanvasY : animation.FirstFrameY,
					0.9f
				);
				topRight = topLeft + new Vector3(animation.SizeWidth, 0, 0);
				bottomRight = topLeft + new Vector3(animation.SizeWidth, animation.SizeHeight, 0);
				bottomLeft = topLeft + new Vector3(0, animation.SizeHeight, 0);
			} else {
				int col = (frameId - totalFramesForTopRow) % totalFramesForNextRows;
				topLeft = new Vector3(
					8 + col * animation.SizeWidth,
					type == "canvas" ? animation.CanvasY : animation.FirstFrameY + row * animation.SizeHeight,
					0.9f
				);
				topRight = topLeft + new Vector3(animation.SizeWidth, 0, 0);
				bottomRight = topLeft + new Vector3(animation.SizeWidth, animation.SizeHeight, 0);
				bottomLeft = topLeft + new Vector3(0, animation.SizeHeight, 0);
			}

			polyVerts[0] = new VertexPositionNormalTexture { Position = topLeft };
			polyVerts[1] = new VertexPositionNormalTexture { Position = topRight };
			polyVerts[2] = new VertexPositionNormalTexture { Position = bottomRight };
			polyVerts[3] = new VertexPositionNormalTexture { Position = bottomLeft };
			polyVerts[4] = new VertexPositionNormalTexture { Position = topLeft };

			return polyVerts;
		}

		public static Texture2D GetUvMapTexture() {
			Texture2D finalTexture = new Texture2D(
				Stage.GraphicsDevice,
				256, 256 * 4,
				false,
				Stage.GraphicsDevice.PresentationParameters.BackBufferFormat
			);

			List<RenderTarget2D> renderTargets = new List<RenderTarget2D>();

			const int totalTexturePages = 4;
			for (int i = 0; i < totalTexturePages; i++) {
				renderTargets.Add(new RenderTarget2D(
						Stage.GraphicsDevice,
						256,
						256,
						false,
						Stage.GraphicsDevice.PresentationParameters.BackBufferFormat,
						DepthFormat.Depth24
					)
				);
			}

			for (int page = 0; page < renderTargets.Count; page++) {
				Stage.GraphicsDevice.SetRenderTarget(renderTargets[page]);
				Stage.GraphicsDevice.Clear(Color.Transparent);

				foreach (Polygon polygon in CurrentMapState.StateData.PolygonCollectionBucket) {
					if (polygon.IsTextured && polygon.TexturePage == page) {
						Stage.BasicEffect.View = _viewMatrix;
						Stage.BasicEffect.Projection = _projectionMatrixForExportedUvMap;
						Stage.BasicEffect.Alpha = 1f;
						Stage.BasicEffect.TextureEnabled = true;
						Stage.BasicEffect.VertexColorEnabled = false;
						Stage.BasicEffect.Texture = UniversalTextures.WhiteTexture;

						Stage.PolygonVertexBuffer.SetData(BuildPolygonRenderVertices(polygon));
						Stage.GraphicsDevice.SetVertexBuffer(Stage.PolygonVertexBuffer);
						Stage.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

						foreach (EffectPass pass in Stage.BasicEffect.CurrentTechnique.Passes) {
							pass.Apply();
							Stage.GraphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, 0, polygon.IsQuad ? 4 : 3);
						}
					}
				}
			}

			List<Color> pixelData = new List<Color>();
			for (int page = 0; page < totalTexturePages; page++) {
				Color[] pixels = new Color[256 * 256];
				renderTargets[page].GetData(0, new Rectangle(0, 0, 256, 256), pixels, 0, 256 * 256);
				pixelData.AddRange(pixels.ToList());
			}

			finalTexture.SetData(pixelData.ToArray());
			Stage.GraphicsDevice.SetRenderTarget(null);
			return finalTexture;
		}
	}
}