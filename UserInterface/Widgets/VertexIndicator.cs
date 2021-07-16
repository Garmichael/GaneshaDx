using System.Collections.Generic;
using GaneshaDx.Common;
using GaneshaDx.Environment;
using GaneshaDx.Resources.ContentDataTypes.Polygons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace GaneshaDx.UserInterface.Widgets {
	public class VertexIndicator {
		private const float Radius = 6;
		private const float NormalLength = 100;
		private const float NormalWidth = 2;

		private readonly Vertex _vertex;
		private VertexPositionColorTexture[] _vertexIndicatorVertices;
		private VertexPositionColorTexture[] _normalIndicatorVertices;
		private readonly DepthStencilState _depthBufferState;

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

			BuildNormalIndicatorVertices();
			Stage.UntexturedVertexBuffer.SetData(_normalIndicatorVertices);

			foreach (EffectPass pass in Stage.BasicEffect.CurrentTechnique.Passes) {
				pass.Apply();
				Stage.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 4);
			}
		}

		private void BuildVertexIndicatorVertices() {
			float zoomAdjustedRadius = Radius * (float) StageCamera.ZoomLevel;
			List<VertexPositionColorTexture> verts = new List<VertexPositionColorTexture>();
			List<Vector3> vertPositions = new List<Vector3> {
				new Vector3(0, 1, 0) * zoomAdjustedRadius,
				new Vector3(1, 0, 0) * zoomAdjustedRadius,
				new Vector3(0, 0, 1) * zoomAdjustedRadius,

				new Vector3(0, 1, 0) * zoomAdjustedRadius,
				new Vector3(0, 0, 1) * zoomAdjustedRadius,
				new Vector3(-1, 0, 0) * zoomAdjustedRadius,

				new Vector3(0, 1, 0) * zoomAdjustedRadius,
				new Vector3(-1, 0, 0) * zoomAdjustedRadius,
				new Vector3(0, 0, -1) * zoomAdjustedRadius,

				new Vector3(0, 1, 0) * zoomAdjustedRadius,
				new Vector3(0, 0, -1) * zoomAdjustedRadius,
				new Vector3(1, 0, 0) * zoomAdjustedRadius,


				new Vector3(0, -1, 0) * zoomAdjustedRadius,
				new Vector3(0, 0, 1) * zoomAdjustedRadius,
				new Vector3(1, 0, 0) * zoomAdjustedRadius,

				new Vector3(0, -1, 0) * zoomAdjustedRadius,
				new Vector3(-1, 0, 0) * zoomAdjustedRadius,
				new Vector3(0, 0, 1) * zoomAdjustedRadius,

				new Vector3(0, -1, 0) * zoomAdjustedRadius,
				new Vector3(0, 0, -1) * zoomAdjustedRadius,
				new Vector3(-1, 0, 0) * zoomAdjustedRadius,

				new Vector3(0, -1, 0) * zoomAdjustedRadius,
				new Vector3(1, 0, 0) * zoomAdjustedRadius,
				new Vector3(0, 0, -1) * zoomAdjustedRadius
			};

			foreach (Vector3 position in vertPositions) {
				verts.Add(new VertexPositionColorTexture(
					_vertex.Position + position,
					_vertex.Color,
					new Vector2(0, 0))
				);
			}

			_vertexIndicatorVertices = verts.ToArray();
		}

		private void BuildNormalIndicatorVertices() {
			float zoomAdjustedRadius = (float) StageCamera.ZoomLevel;
			List<VertexPositionColorTexture> verts = new List<VertexPositionColorTexture>();
			List<Vector3> vertPositions = new List<Vector3> {
				new Vector3(0, -NormalWidth, -NormalWidth) * zoomAdjustedRadius,
				new Vector3(0, -NormalWidth, NormalWidth) * zoomAdjustedRadius,
				new Vector3(NormalLength, 0, 0) * zoomAdjustedRadius,

				new Vector3(0, -NormalWidth, NormalWidth) * zoomAdjustedRadius,
				new Vector3(0, NormalWidth, NormalWidth) * zoomAdjustedRadius,
				new Vector3(NormalLength, 0, 0) * zoomAdjustedRadius,

				new Vector3(0, NormalWidth, NormalWidth) * zoomAdjustedRadius,
				new Vector3(0, NormalWidth, -NormalWidth) * zoomAdjustedRadius,
				new Vector3(NormalLength, 0, 0) * zoomAdjustedRadius,

				new Vector3(0, NormalWidth, -NormalWidth) * zoomAdjustedRadius,
				new Vector3(0, -NormalWidth, -NormalWidth) * zoomAdjustedRadius,
				new Vector3(NormalLength, 0, 0) * zoomAdjustedRadius
			};


			for (int index = 0; index < vertPositions.Count; index++) {
				Matrix faceElevation = Matrix.CreateRotationZ(MathHelper.ToRadians(_vertex.NormalElevation));
				vertPositions[index] = Vector3.Transform(vertPositions[index], faceElevation);

				Matrix faceAzimuth = Matrix.CreateRotationY(MathHelper.ToRadians(-_vertex.NormalAzimuth));
				vertPositions[index] = Vector3.Transform(vertPositions[index], faceAzimuth);
				
				bool inVertexSelectionMode = Gui.Widget == WidgetSelectionMode.PolygonVertexTranslate ||
				                             Gui.Widget == WidgetSelectionMode.PolygonEdgeTranslate;

				int vertexIndex = Selection.SelectedPolygons[0].Vertices.IndexOf(_vertex);
				bool vertexIsSelected = inVertexSelectionMode &&
				                        TransformWidget.SelectedVertexIndices.Contains(vertexIndex);

				verts.Add(new VertexPositionColorTexture(
					_vertex.Position + vertPositions[index],
					vertexIsSelected ? Color.Cyan : _vertex.Color,
					new Vector2(0, 0))
				);
			}

			_normalIndicatorVertices = verts.ToArray();
		}
	}
}