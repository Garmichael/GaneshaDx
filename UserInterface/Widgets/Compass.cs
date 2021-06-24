using System;
using System.Collections.Generic;
using GaneshaDx.Environment;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GaneshaDx.UserInterface.Widgets {
	public class Compass {
		private const float Radius = 1;
		private const float Length = 200;
		private const int Sides = 8;

		private VertexPositionColorTexture[] _lightIndicatorVertices;
		private readonly DepthStencilState _depthBufferState;

		public Compass() {
			_depthBufferState = new DepthStencilState {
				DepthBufferEnable = false
			};
		}

		public void Render() {
			BuildLightIndicatorVertices();
			Stage.UntexturedVertexBuffer.SetData(_lightIndicatorVertices);
			Stage.GraphicsDevice.SetVertexBuffer(Stage.UntexturedVertexBuffer);
			Stage.GraphicsDevice.DepthStencilState = _depthBufferState;

			Stage.BasicEffect.Alpha = 1f;
			Stage.BasicEffect.TextureEnabled = false;
			Stage.BasicEffect.VertexColorEnabled = true;

			foreach (EffectPass pass in Stage.BasicEffect.CurrentTechnique.Passes) {
				pass.Apply();
				Stage.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, Sides * 4);
			}
		}

		private void BuildLightIndicatorVertices() {
			List<VertexPositionColorTexture> verts = new List<VertexPositionColorTexture>();
			List<Vector3> vertPositions = new List<Vector3>();

			for (int vertexIndex = 0; vertexIndex < Sides; vertexIndex++) {
				vertPositions.Add(new Vector3(
					(float) (Radius * Math.Cos(2 * Math.PI * vertexIndex / Sides)),
					(float) (Radius * Math.Sin(2 * Math.PI * vertexIndex / Sides)),
					0
				));

				vertexIndex++;

				vertPositions.Add(new Vector3(
					(float) (Radius * Math.Cos(2 * Math.PI * vertexIndex / Sides)),
					(float) (Radius * Math.Sin(2 * Math.PI * vertexIndex / Sides)),
					0
				));

				vertPositions.Add(new Vector3(
					0,
					0,
					0
				));

				vertexIndex--;
			}

			for (int vertexIndex = Sides - 1; vertexIndex >= 0; vertexIndex--) {
				vertPositions.Add(new Vector3(
					(float) (Radius * Math.Cos(2 * Math.PI * vertexIndex / Sides)),
					(float) (Radius * Math.Sin(2 * Math.PI * vertexIndex / Sides)),
					Length
				));

				vertexIndex--;

				vertPositions.Add(new Vector3(
					(float) (Radius * Math.Cos(2 * Math.PI * vertexIndex / Sides)),
					(float) (Radius * Math.Sin(2 * Math.PI * vertexIndex / Sides)),
					Length
				));

				vertPositions.Add(new Vector3(
					0,
					0,
					Length
				));

				vertexIndex++;
			}

			for (int vertexIndex = 0; vertexIndex < Sides; vertexIndex++) {
				vertPositions.Add(new Vector3(
					(float) (Radius * Math.Cos(2 * Math.PI * vertexIndex / Sides)),
					(float) (Radius * Math.Sin(2 * Math.PI * vertexIndex / Sides)),
					0
				));

				vertexIndex++;

				vertPositions.Add(new Vector3(
					(float) (Radius * Math.Cos(2 * Math.PI * vertexIndex / Sides)),
					(float) (Radius * Math.Sin(2 * Math.PI * vertexIndex / Sides)),
					Length
				));

				vertPositions.Add(new Vector3(
					(float) (Radius * Math.Cos(2 * Math.PI * vertexIndex / Sides)),
					(float) (Radius * Math.Sin(2 * Math.PI * vertexIndex / Sides)),
					0
				));

				vertexIndex--;
			}

			for (int vertexIndex = 0; vertexIndex < Sides; vertexIndex++) {
				vertPositions.Add(new Vector3(
					(float) (Radius * Math.Cos(2 * Math.PI * vertexIndex / Sides)),
					(float) (Radius * Math.Sin(2 * Math.PI * vertexIndex / Sides)),
					0
				));

				vertPositions.Add(new Vector3(
					(float) (Radius * Math.Cos(2 * Math.PI * vertexIndex / Sides)),
					(float) (Radius * Math.Sin(2 * Math.PI * vertexIndex / Sides)),
					Length
				));

				vertexIndex++;

				vertPositions.Add(new Vector3(
					(float) (Radius * Math.Cos(2 * Math.PI * vertexIndex / Sides)),
					(float) (Radius * Math.Sin(2 * Math.PI * vertexIndex / Sides)),
					Length
				));

				vertexIndex--;
			}

			foreach (Vector3 vectorPosition in vertPositions) {
				Vector3 position = vectorPosition;

				Matrix elevationRotation = Matrix.CreateRotationX(
					MathHelper.ToRadians(-90)
				);

				position = Vector3.Transform(position, elevationRotation);


				verts.Add(new VertexPositionColorTexture(
					position,
					Color.Cyan,
					new Vector2(0, 0))
				);
			}

			_lightIndicatorVertices = verts.ToArray();
		}
	}
}