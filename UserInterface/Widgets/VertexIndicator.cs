using System.Collections.Generic;
using GaneshaDx.Common;
using GaneshaDx.Environment;
using GaneshaDx.Resources.ContentDataTypes.Polygons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.UI;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace GaneshaDx.UserInterface.Widgets {
	public class VertexIndicator {
		private const float Radius = 6;
		private const float NormalWidth = 2;

		private readonly Vertex _vertex;
		private VertexPositionColorTexture[] _vertexIndicatorVertices = { };
		private VertexPositionColorTexture[] _normalIndicatorVertices = { };
		private readonly DepthStencilState _depthBufferState;

		readonly List<Vector3> _vertexIndicatorDefinition = new List<Vector3> {
			new Vector3(0, 1, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 1),
			new Vector3(0, 1, 0), new Vector3(0, 0, 1), new Vector3(-1, 0, 0),
			new Vector3(0, 1, 0), new Vector3(-1, 0, 0), new Vector3(0, 0, -1),
			new Vector3(0, 1, 0), new Vector3(0, 0, -1), new Vector3(1, 0, 0),
			new Vector3(0, -1, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0),
			new Vector3(0, -1, 0), new Vector3(-1, 0, 0), new Vector3(0, 0, 1),
			new Vector3(0, -1, 0), new Vector3(0, 0, -1), new Vector3(-1, 0, 0),
			new Vector3(0, -1, 0), new Vector3(1, 0, 0), new Vector3(0, 0, -1)
		};

		private readonly List<Vector3> _normalIndicatorDefinition = new List<Vector3> {
			new Vector3(0, -NormalWidth, -NormalWidth), new Vector3(0, -NormalWidth, NormalWidth), new Vector3(1, 0, 0),
			new Vector3(0, -NormalWidth, NormalWidth), new Vector3(0, NormalWidth, NormalWidth), new Vector3(1, 0, 0),
			new Vector3(0, NormalWidth, NormalWidth), new Vector3(0, NormalWidth, -NormalWidth), new Vector3(1, 0, 0),
			new Vector3(0, NormalWidth, -NormalWidth), new Vector3(0, -NormalWidth, -NormalWidth), new Vector3(1, 0, 0)
		};

		public VertexIndicator(Vertex vertex) {
			_vertex = vertex;

			_depthBufferState = new DepthStencilState {
				DepthBufferEnable = false
			};
		}

		public void Render() {
			BuildVertexIndicatorVertices();
			Stage.UntexturedVertexBuffer.SetData(_vertexIndicatorVertices);
			Stage.GraphicsDevice.SetVertexBuffer(Stage.UntexturedVertexBuffer);
			Stage.GraphicsDevice.DepthStencilState = _depthBufferState;

			Stage.BasicEffect.Alpha = 1f;
			Stage.BasicEffect.TextureEnabled = false;
			Stage.BasicEffect.VertexColorEnabled = true;

			foreach (EffectPass pass in Stage.BasicEffect.CurrentTechnique.Passes) {
				pass.Apply();
				Stage.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 8);
			}

			if(!Configuration.Properties.HideNormalIndicators){
				BuildNormalIndicatorVertices();
				Stage.UntexturedVertexBuffer.SetData(_normalIndicatorVertices);

				foreach (EffectPass pass in Stage.BasicEffect.CurrentTechnique.Passes) {
					pass.Apply();
					Stage.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 4);
				}
			}
		}

		private void BuildVertexIndicatorVertices() {
			float zoomAdjustedRadius = Radius * (float) StageCamera.ZoomLevel;

			if (_vertexIndicatorVertices.Length != _vertexIndicatorDefinition.Count) {
				_vertexIndicatorVertices = new VertexPositionColorTexture[_vertexIndicatorDefinition.Count];
			}

			for (int vertexIndex = 0; vertexIndex < _vertexIndicatorDefinition.Count; vertexIndex++) {
				Vector3 position = _vertexIndicatorDefinition[vertexIndex] * zoomAdjustedRadius + _vertex.Position;
				_vertexIndicatorVertices[vertexIndex] = new VertexPositionColorTexture(position, _vertex.Color, Vector2.Zero);
			}
		}

		private void BuildNormalIndicatorVertices() {
			float zoomAdjustedRadius = (float) StageCamera.ZoomLevel;

			UpdateNormalIndicatorLength();
			if (_normalIndicatorVertices.Length != _normalIndicatorDefinition.Count) {
				_normalIndicatorVertices = new VertexPositionColorTexture[_normalIndicatorDefinition.Count];
			}

			for (int index = 0; index < _normalIndicatorDefinition.Count; index++) {
				Matrix faceElevation = Matrix.CreateRotationZ(MathHelper.ToRadians(_vertex.NormalElevation));
				Vector3 adjustedPosition = Vector3.Transform(_normalIndicatorDefinition[index], faceElevation);

				Matrix faceAzimuth = Matrix.CreateRotationY(MathHelper.ToRadians(-_vertex.NormalAzimuth));
				adjustedPosition = Vector3.Transform(adjustedPosition, faceAzimuth);

				bool inVertexSelectionMode = Gui.Widget == WidgetSelectionMode.PolygonVertexTranslate ||
				                             Gui.Widget == WidgetSelectionMode.PolygonEdgeTranslate;


				int vertexIndex = Selection.SelectedPolygons[0].Vertices.IndexOf(_vertex);
				bool vertexIsSelected = inVertexSelectionMode &&
				                        TransformWidget.SelectedVertexIndices.Contains(vertexIndex);


				Vector3 position = _vertex.Position + adjustedPosition * zoomAdjustedRadius;
				Color vertexColor = vertexIsSelected ? Color.Cyan : _vertex.Color;
				_normalIndicatorVertices[index] = new VertexPositionColorTexture(position, vertexColor, Vector2.Zero);
			}
		}

		private void UpdateNormalIndicatorLength() {
			_normalIndicatorDefinition[2] = new Vector3(Configuration.Properties.NormalIndicatorLength, 0, 0);
			_normalIndicatorDefinition[5] = new Vector3(Configuration.Properties.NormalIndicatorLength, 0, 0);
			_normalIndicatorDefinition[8] = new Vector3(Configuration.Properties.NormalIndicatorLength, 0, 0);
			_normalIndicatorDefinition[11] = new Vector3(Configuration.Properties.NormalIndicatorLength, 0, 0);
		}
	}
}