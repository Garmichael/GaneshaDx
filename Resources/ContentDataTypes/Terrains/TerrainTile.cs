using System.Collections.Generic;
using GaneshaDx.Common;
using GaneshaDx.Environment;
using GaneshaDx.UserInterface;
using GaneshaDx.UserInterface.GuiDefinitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GaneshaDx.Resources.ContentDataTypes.Terrains {
	public class TerrainTile {
		public TerrainSurfaceType SurfaceType;
		public int Shading;
		public int Height;
		public int Depth;
		public int SlopeHeight;
		public TerrainSlopeType SlopeType;
		public bool PassThroughOnly;
		public bool Impassable;
		public bool Unselectable;
		public int IndexZ;
		public int IndexX;
		public int Level;
		public List<Vector3> Vertices = new List<Vector3> {Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero};
		public bool RotatesNorthwestTop;
		public bool RotatesSouthwestTop;
		public bool RotatesSoutheastTop;
		public bool RotatesNortheastTop;
		public bool RotatesNorthwestBottom;
		public bool RotatesSouthwestBottom;
		public bool RotatesSoutheastBottom;
		public bool RotatesNortheastBottom;
		public int Unknown0A;
		public int Unknown0B;
		public int Unknown1;
		public int Unknown5;
		public bool Unknown6B;
		public bool Unknown6C;
		public bool Unknown6D;
		

		public Vector3 AveragePoint {
			get {
				List<Vector3> adjustedVerts = new List<Vector3>();
				foreach (Vector3 vertex in Vertices) {
					adjustedVerts.Add(vertex);
				}

				return Utilities.GetAveragePoint(adjustedVerts);
			}
		}

		public double DistanceToCamera => Vector3.Distance(StageCamera.CamPosition, AveragePoint);

		private VertexPositionColorTexture[] _renderVertices;

		public void Render() {
			SetRenderVertices();

			Stage.UntexturedVertexBuffer.SetData(_renderVertices);
			Stage.GraphicsDevice.SetVertexBuffer(Stage.UntexturedVertexBuffer);
			Stage.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

			float alpha;

			if (Gui.SelectedTab == RightPanelTab.Polygon) {
				alpha = Configuration.Properties.TerrainTransparencyForPolygonEditing / 100f;
			} else if (Gui.SelectedTab != RightPanelTab.Terrain) {
				alpha = 0;
			} else {
				alpha = Configuration.Properties.TerrainTransparencyForTerrainEditing / 100f;
			}

			Stage.BasicEffect.Alpha = alpha;

			Stage.BasicEffect.TextureEnabled = false;
			Stage.BasicEffect.VertexColorEnabled = true;

			foreach (EffectPass pass in Stage.BasicEffect.CurrentTechnique.Passes) {
				pass.Apply();
				Stage.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
			}
		}

		private void SetRenderVertices() {
			Vertices = new List<Vector3> {
				new Vector3(-IndexX * 28, (Height + Depth) * 12 + 1, IndexZ * 28),
				new Vector3(-IndexX * 28, (Height + Depth) * 12 + 1, IndexZ * 28 + 28),
				new Vector3(-IndexX * 28 - 28, (Height + Depth) * 12 + 1, IndexZ * 28 + 28),
				new Vector3(-IndexX * 28 - 28, (Height + Depth) * 12 + 1, IndexZ * 28),
			};

			List<TerrainSlopeType> vert0LiftTypes = new List<TerrainSlopeType> {
				TerrainSlopeType.InclineSouth,
				TerrainSlopeType.InclineWest,
				TerrainSlopeType.ConvexSouthwest,
				TerrainSlopeType.ConcaveSoutheast,
				TerrainSlopeType.ConcaveSouthwest,
				TerrainSlopeType.ConcaveNorthwest
			};

			List<TerrainSlopeType> vert1LiftTypes = new List<TerrainSlopeType> {
				TerrainSlopeType.InclineNorth,
				TerrainSlopeType.InclineWest,
				TerrainSlopeType.ConvexNorthwest,
				TerrainSlopeType.ConcaveNortheast,
				TerrainSlopeType.ConcaveSouthwest,
				TerrainSlopeType.ConcaveNorthwest
			};

			List<TerrainSlopeType> vert2LiftTypes = new List<TerrainSlopeType> {
				TerrainSlopeType.InclineNorth,
				TerrainSlopeType.InclineEast,
				TerrainSlopeType.ConvexNortheast,
				TerrainSlopeType.ConcaveNortheast,
				TerrainSlopeType.ConcaveSoutheast,
				TerrainSlopeType.ConcaveNorthwest
			};

			List<TerrainSlopeType> vert3LiftTypes = new List<TerrainSlopeType> {
				TerrainSlopeType.InclineEast,
				TerrainSlopeType.InclineSouth,
				TerrainSlopeType.ConvexSoutheast,
				TerrainSlopeType.ConcaveNortheast,
				TerrainSlopeType.ConcaveSoutheast,
				TerrainSlopeType.ConcaveSouthwest
			};

			Vector3 liftAmount = new Vector3(0, 12 + (SlopeHeight - 1) * 12, 0);

			if (vert0LiftTypes.Contains(SlopeType)) {
				Vertices[0] += liftAmount;
			}

			if (vert1LiftTypes.Contains(SlopeType)) {
				Vertices[1] += liftAmount;
			}

			if (vert2LiftTypes.Contains(SlopeType)) {
				Vertices[2] += liftAmount;
			}

			if (vert3LiftTypes.Contains(SlopeType)) {
				Vertices[3] += liftAmount;
			}

			_renderVertices = new[] {
				new VertexPositionColorTexture(Vertices[0], GetVertexColor(Vertices[0].Y), Vector2.Zero),
				new VertexPositionColorTexture(Vertices[1], GetVertexColor(Vertices[1].Y), Vector2.Zero),
				new VertexPositionColorTexture(Vertices[2], GetVertexColor(Vertices[2].Y), Vector2.Zero),
				new VertexPositionColorTexture(Vertices[3], GetVertexColor(Vertices[3].Y), Vector2.Zero),
				new VertexPositionColorTexture(Vertices[0], GetVertexColor(Vertices[0].Y), Vector2.Zero),
				new VertexPositionColorTexture(Vertices[2], GetVertexColor(Vertices[2].Y), Vector2.Zero),
			};
		}

		private Color GetVertexColor(float height) {
			if (Gui.SelectedTab == RightPanelTab.Polygon) {
				return new Color(
					Configuration.Properties.TerrainColorForPolygonEditing.X,
					Configuration.Properties.TerrainColorForPolygonEditing.Y,
					Configuration.Properties.TerrainColorForPolygonEditing.Z,
					255
				);
			}

			height /= 12;
			bool isOffColor = (IndexX + IndexZ) % 2 == 0;
			int baseColor = (int) height * 7 + 16 + (isOffColor ? 0 : 8);
			Color returnColor = new Color(baseColor + 16, baseColor + 16, baseColor, 255);

			if (Impassable || Unselectable) {
				returnColor.R += 32;
			}

			if (Selection.SelectedTerrainTiles.Contains(this)) {
				returnColor = new Color(128, 128, 255, 255);
			} else if (Selection.HoveredTerrainTiles.Count > 0 && Selection.HoveredTerrainTiles[0] == this) {
				returnColor = new Color(200, 200, 128, 255);
			}

			return returnColor;
		}

		public List<byte> GetRawData() {
			List<byte> rawData = new List<byte>();
			int surfaceTypeId = 0;

			foreach (KeyValuePair<int, TerrainSurfaceType> surfaceType in CommonLists.TerrainSurfaceTypes) {
				if (surfaceType.Value == SurfaceType) {
					surfaceTypeId = surfaceType.Key;
					break;
				}
			}

			string binary = Unknown0A + Unknown0B + Utilities.GetBinaryFromInt(surfaceTypeId, 6);
			rawData.Add((byte) Utilities.GetIntFromBinary(binary));
			rawData.Add((byte) Unknown1);
			rawData.Add((byte) Height);

			binary = Utilities.GetBinaryFromInt(Depth, 3) +
			         Utilities.GetBinaryFromInt(SlopeHeight, 5);
			rawData.Add((byte) Utilities.GetIntFromBinary(binary));

			int slopeTypeId = 0;
			foreach (KeyValuePair<int, TerrainSlopeType> slopeType in CommonLists.TerrainSlopeTypes) {
				if (slopeType.Value == SlopeType) {
					slopeTypeId = slopeType.Key;
					break;
				}
			}

			rawData.Add((byte) slopeTypeId);

			rawData.Add((byte) Unknown5);

			binary = (PassThroughOnly ? "1" : "0") +
			         (Unknown6B ? "1" : "0") +
			         (Unknown6C ? "1" : "0") +
			         (Unknown6D ? "1" : "0") +
			         Utilities.GetBinaryFromInt(Shading, 2) +
			         (Impassable ? "1" : "0") +
			         (Unselectable ? "1" : "0");
			rawData.Add((byte) Utilities.GetIntFromBinary(binary));

			binary = "";
			binary += RotatesNorthwestTop ? "1" : "0";
			binary += RotatesSouthwestTop ? "1" : "0";
			binary += RotatesSoutheastTop ? "1" : "0";
			binary += RotatesNortheastTop ? "1" : "0";
			binary += RotatesNorthwestBottom ? "1" : "0";
			binary += RotatesSouthwestBottom ? "1" : "0";
			binary += RotatesSoutheastBottom ? "1" : "0";
			binary += RotatesNortheastBottom ? "1" : "0";

			rawData.Add((byte) Utilities.GetIntFromBinary(binary));

			return rawData;
		}
	}
}