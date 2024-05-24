using System;
using System.Collections.Generic;
using GaneshaDx.Environment;
using GaneshaDx.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DirectionalLight = GaneshaDx.Resources.ContentDataTypes.DirectionalLight;

namespace GaneshaDx.UserInterface.Widgets;

public class DirectionalLightIndicator {
	private const float Radius = 18;
	private const float LightLength = 36;
	private const int LightSides = 8;

	private readonly DirectionalLight _light;
	private readonly Color _color;
	private VertexPositionColorTexture[] _lightIndicatorVertices = {};
	private readonly DepthStencilState _depthBufferState;

	public DirectionalLightIndicator(DirectionalLight light, Color color) {
		_light = light;
		_color = color;

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
			Stage.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, LightSides * 4);
		}
	}

	private void BuildLightIndicatorVertices() {
		float zoomAdjustedRadius = Radius * (float) StageCamera.ZoomLevel;
		float zoomAdjustedLength = LightLength * (float) StageCamera.ZoomLevel;
		List<Vector3> vertPositions = new();

		for (int vertexIndex = 0; vertexIndex < LightSides; vertexIndex++) {
			vertPositions.Add(new Vector3(
				(float) (zoomAdjustedRadius * Math.Cos(2 * Math.PI * vertexIndex / LightSides)),
				(float) (zoomAdjustedRadius * Math.Sin(2 * Math.PI * vertexIndex / LightSides)),
				0
			));

			vertexIndex++;

			vertPositions.Add(new Vector3(
				(float) (zoomAdjustedRadius * Math.Cos(2 * Math.PI * vertexIndex / LightSides)),
				(float) (zoomAdjustedRadius * Math.Sin(2 * Math.PI * vertexIndex / LightSides)),
				0
			));

			vertPositions.Add(new Vector3(
				0,
				0,
				0
			));

			vertexIndex--;
		}

		for (int vertexIndex = LightSides - 1; vertexIndex >= 0; vertexIndex--) {
			vertPositions.Add(new Vector3(
				(float) (zoomAdjustedRadius * Math.Cos(2 * Math.PI * vertexIndex / LightSides)),
				(float) (zoomAdjustedRadius * Math.Sin(2 * Math.PI * vertexIndex / LightSides)),
				zoomAdjustedLength
			));

			vertexIndex--;

			vertPositions.Add(new Vector3(
				(float) (zoomAdjustedRadius * Math.Cos(2 * Math.PI * vertexIndex / LightSides)),
				(float) (zoomAdjustedRadius * Math.Sin(2 * Math.PI * vertexIndex / LightSides)),
				zoomAdjustedLength
			));

			vertPositions.Add(new Vector3(
				0,
				0,
				zoomAdjustedLength
			));

			vertexIndex++;
		}

		for (int vertexIndex = 0; vertexIndex < LightSides; vertexIndex++) {
			vertPositions.Add(new Vector3(
				(float) (zoomAdjustedRadius * Math.Cos(2 * Math.PI * vertexIndex / LightSides)),
				(float) (zoomAdjustedRadius * Math.Sin(2 * Math.PI * vertexIndex / LightSides)),
				0
			));

			vertexIndex++;

			vertPositions.Add(new Vector3(
				(float) (zoomAdjustedRadius * Math.Cos(2 * Math.PI * vertexIndex / LightSides)),
				(float) (zoomAdjustedRadius * Math.Sin(2 * Math.PI * vertexIndex / LightSides)),
				zoomAdjustedLength
			));

			vertPositions.Add(new Vector3(
				(float) (zoomAdjustedRadius * Math.Cos(2 * Math.PI * vertexIndex / LightSides)),
				(float) (zoomAdjustedRadius * Math.Sin(2 * Math.PI * vertexIndex / LightSides)),
				0
			));

			vertexIndex--;
		}

		for (int vertexIndex = 0; vertexIndex < LightSides; vertexIndex++) {
			vertPositions.Add(new Vector3(
				(float) (zoomAdjustedRadius * Math.Cos(2 * Math.PI * vertexIndex / LightSides)),
				(float) (zoomAdjustedRadius * Math.Sin(2 * Math.PI * vertexIndex / LightSides)),
				0
			));

			vertPositions.Add(new Vector3(
				(float) (zoomAdjustedRadius * Math.Cos(2 * Math.PI * vertexIndex / LightSides)),
				(float) (zoomAdjustedRadius * Math.Sin(2 * Math.PI * vertexIndex / LightSides)),
				zoomAdjustedLength
			));

			vertexIndex++;

			vertPositions.Add(new Vector3(
				(float) (zoomAdjustedRadius * Math.Cos(2 * Math.PI * vertexIndex / LightSides)),
				(float) (zoomAdjustedRadius * Math.Sin(2 * Math.PI * vertexIndex / LightSides)),
				zoomAdjustedLength
			));

			vertexIndex--;
		}

		if (_lightIndicatorVertices.Length != vertPositions.Count) {
			_lightIndicatorVertices = new VertexPositionColorTexture[vertPositions.Count];
		}

		for (int vertexIndex = 0; vertexIndex < vertPositions.Count; vertexIndex++) {
			Vector3 vectorPosition = vertPositions[vertexIndex];
			Vector3 position = vectorPosition;
			Color color = position.Z == 0 ? _color : _light.LightColor;

			Vector3 center = new Vector3(
				-CurrentMapState.StateData.Terrain.SizeX * 28f / 2f,
				0,
				CurrentMapState.StateData.Terrain.SizeZ * 28f / 2f
			);

			Matrix elevationRotation = Matrix.CreateRotationX(
				MathHelper.ToRadians((float) _light.DirectionElevation)
			);

			Matrix azimuthRotation = Matrix.CreateRotationY(
				MathHelper.ToRadians((float) -_light.DirectionAzimuth - 90)
			);

			position.Z -= CurrentMapState.StateData.Terrain.SizeZ * 28f * 1.5f;
			position = Vector3.Transform(position, elevationRotation);
			position = Vector3.Transform(position, azimuthRotation);
			position += center;

			_lightIndicatorVertices[vertexIndex] = new VertexPositionColorTexture(position, color, Vector2.Zero);
		}
	}
}