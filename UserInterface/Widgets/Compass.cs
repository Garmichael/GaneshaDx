﻿using System;
using System.Collections.Generic;
using GaneshaDx.Common;
using GaneshaDx.Environment;
using GaneshaDx.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GaneshaDx.UserInterface.Widgets;

public class Compass {
	private const float Radius = 1;
	private const int Sides = 8;
	private int _totalTriangles;

	private VertexPositionColorTexture[] _postVertices = {};
	private readonly DepthStencilState _depthBufferState;

	public Compass() {
		_depthBufferState = new DepthStencilState {
			DepthBufferEnable = false
		};
	}

	public void Render() {
		BuildCompassVertices();
		Stage.UntexturedVertexBuffer.SetData(_postVertices);
		Stage.GraphicsDevice.SetVertexBuffer(Stage.UntexturedVertexBuffer);
		Stage.GraphicsDevice.DepthStencilState = _depthBufferState;

		Stage.BasicEffect.Alpha = 1f;
		Stage.BasicEffect.TextureEnabled = false;
		Stage.BasicEffect.VertexColorEnabled = true;

		foreach (EffectPass pass in Stage.BasicEffect.CurrentTechnique.Passes) {
			pass.Apply();
			Stage.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, _totalTriangles);
		}
	}

	private void BuildCompassVertices() {
		List<Vector3> sourceVertexPositions = BuildSingleAxisVertices();
		List<Vector3> bothAxisVertices = new();
		List<Vector3> xAxisVertices = new();
		List<Vector3> zAxisVertices = new();
			
		foreach (Vector3 vertexPosition in sourceVertexPositions) {
			Matrix zRotation = Matrix.CreateRotationX(MathHelper.ToRadians(90));
			Vector3 newVertexPosition = Vector3.Transform(vertexPosition, zRotation);

			if (Configuration.Properties.ExpandCompassToTerrainLength) {
				Matrix zScale = Matrix.CreateScale(1, CurrentMapState.StateData.Terrain.SizeZ, 1);
				newVertexPosition = Vector3.Transform(newVertexPosition, zScale);
			}

			zAxisVertices.Add(newVertexPosition);

			Matrix xRotation = Matrix.CreateRotationY(MathHelper.ToRadians(-90));
			newVertexPosition = Vector3.Transform(vertexPosition, xRotation);

			if (Configuration.Properties.ExpandCompassToTerrainLength) {
				Matrix xScale = Matrix.CreateScale(CurrentMapState.StateData.Terrain.SizeX, 1, 1);
				newVertexPosition = Vector3.Transform(newVertexPosition, xScale);
			}

			xAxisVertices.Add(newVertexPosition);
		}

		bothAxisVertices.AddRange(xAxisVertices);
		bothAxisVertices.AddRange(zAxisVertices);

		if (_postVertices.Length != bothAxisVertices.Count) {
			_postVertices = new VertexPositionColorTexture[bothAxisVertices.Count];
		}

		for (int vertexIndex = 0; vertexIndex < bothAxisVertices.Count; vertexIndex++) {
			Vector3 vectorPosition = bothAxisVertices[vertexIndex];
			Matrix elevationRotation = Matrix.CreateRotationX(MathHelper.ToRadians(-90));
			Vector3 position = Vector3.Transform(vectorPosition, elevationRotation);
			_postVertices[vertexIndex] = new VertexPositionColorTexture(position, Color.Cyan, Vector2.Zero);
		}

		_totalTriangles = Sides * 4 * 2;
	}

	private List<Vector3> BuildSingleAxisVertices() {
		List<Vector3> vertexPositions = new();

		const int length = 28;

		for (int vertexIndex = 0; vertexIndex < Sides; vertexIndex++) {
			vertexPositions.Add(new Vector3(
				(float) (Radius * Math.Cos(2 * Math.PI * vertexIndex / Sides)),
				(float) (Radius * Math.Sin(2 * Math.PI * vertexIndex / Sides)),
				0
			));

			vertexIndex++;

			vertexPositions.Add(new Vector3(
				(float) (Radius * Math.Cos(2 * Math.PI * vertexIndex / Sides)),
				(float) (Radius * Math.Sin(2 * Math.PI * vertexIndex / Sides)),
				0
			));

			vertexPositions.Add(new Vector3(
				0,
				0,
				0
			));

			vertexIndex--;
		}

		for (int vertexIndex = Sides - 1; vertexIndex >= 0; vertexIndex--) {
			vertexPositions.Add(new Vector3(
				(float) (Radius * Math.Cos(2 * Math.PI * vertexIndex / Sides)),
				(float) (Radius * Math.Sin(2 * Math.PI * vertexIndex / Sides)),
				length
			));

			vertexIndex--;

			vertexPositions.Add(new Vector3(
				(float) (Radius * Math.Cos(2 * Math.PI * vertexIndex / Sides)),
				(float) (Radius * Math.Sin(2 * Math.PI * vertexIndex / Sides)),
				length
			));

			vertexPositions.Add(new Vector3(
				0,
				0,
				length
			));

			vertexIndex++;
		}

		for (int vertexIndex = 0; vertexIndex < Sides; vertexIndex++) {
			vertexPositions.Add(new Vector3(
				(float) (Radius * Math.Cos(2 * Math.PI * vertexIndex / Sides)),
				(float) (Radius * Math.Sin(2 * Math.PI * vertexIndex / Sides)),
				0
			));

			vertexIndex++;

			vertexPositions.Add(new Vector3(
				(float) (Radius * Math.Cos(2 * Math.PI * vertexIndex / Sides)),
				(float) (Radius * Math.Sin(2 * Math.PI * vertexIndex / Sides)),
				length
			));

			vertexPositions.Add(new Vector3(
				(float) (Radius * Math.Cos(2 * Math.PI * vertexIndex / Sides)),
				(float) (Radius * Math.Sin(2 * Math.PI * vertexIndex / Sides)),
				0
			));

			vertexIndex--;
		}

		for (int vertexIndex = 0; vertexIndex < Sides; vertexIndex++) {
			vertexPositions.Add(new Vector3(
				(float) (Radius * Math.Cos(2 * Math.PI * vertexIndex / Sides)),
				(float) (Radius * Math.Sin(2 * Math.PI * vertexIndex / Sides)),
				0
			));

			vertexPositions.Add(new Vector3(
				(float) (Radius * Math.Cos(2 * Math.PI * vertexIndex / Sides)),
				(float) (Radius * Math.Sin(2 * Math.PI * vertexIndex / Sides)),
				length
			));

			vertexIndex++;

			vertexPositions.Add(new Vector3(
				(float) (Radius * Math.Cos(2 * Math.PI * vertexIndex / Sides)),
				(float) (Radius * Math.Sin(2 * Math.PI * vertexIndex / Sides)),
				length
			));

			vertexIndex--;
		}

		return vertexPositions;
	}
}