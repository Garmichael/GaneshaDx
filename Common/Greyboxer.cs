using System;
using System.Collections.Generic;
using System.Linq;
using GaneshaDx.Resources;
using GaneshaDx.Resources.ContentDataTypes.Palettes;
using GaneshaDx.Resources.ContentDataTypes.Polygons;
using GaneshaDx.Resources.ContentDataTypes.Terrains;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GaneshaDx.Common;

public static class Greyboxer {
	private static bool _buildSinglePolyWalls;

	public static void Greybox(bool modifyTexture, bool modifyPalettes, bool buildSinglePolyWalls) {
		_buildSinglePolyWalls = buildSinglePolyWalls;

		CurrentMapState.DeleteAllPolygons(MeshType.PrimaryMesh);

		if (modifyTexture) {
			PaintTexture();
		}

		if (modifyPalettes) {
			SetPalette();
		}

		List<List<Polygon>> surfaceMap = BuildGreyboxSurface();
		BuildGreyboxWalls(surfaceMap);
	}

	private static List<List<Polygon>> BuildGreyboxSurface() {
		List<List<Polygon>> surfaceMap = new();
		List<List<TerrainTile>> level0TerrainTiles = CurrentMapState.StateData.Terrain.Level0Tiles;
		List<List<TerrainTile>> level1TerrainTiles = CurrentMapState.StateData.Terrain.Level1Tiles;

		for (int indexZ = 0; indexZ < CurrentMapState.StateData.Terrain.SizeZ; indexZ++) {
			surfaceMap.Add(new List<Polygon>());

			for (int indexX = 0; indexX < CurrentMapState.StateData.Terrain.SizeX; indexX++) {
				TerrainTile level0TerrainTile = level0TerrainTiles[indexZ][indexX];
				TerrainTile level1TerrainTile = level1TerrainTiles[indexZ][indexX];
				TerrainTile terrainTile = level0TerrainTile.Height + level0TerrainTile.Depth > level1TerrainTile.Height + level1TerrainTile.Depth
					? level0TerrainTiles[indexZ][indexX]
					: level1TerrainTiles[indexZ][indexX];

				Vector3 red = new(-indexX * 28, (terrainTile.Height + terrainTile.Depth) * 12, indexZ * 28 + 28);
				Vector3 green = red + new Vector3(-28, 0, 0);
				Vector3 blue = red + new Vector3(0, 0, -28);
				Vector3 yellow = red + new Vector3(-28, 0, -28);

				List<TerrainSlopeType> redLiftTypes = new() {
					TerrainSlopeType.InclineNorth,
					TerrainSlopeType.InclineWest,
					TerrainSlopeType.ConvexNorthwest,
					TerrainSlopeType.ConcaveNortheast,
					TerrainSlopeType.ConcaveSouthwest,
					TerrainSlopeType.ConcaveNorthwest
				};

				List<TerrainSlopeType> greenLiftTypes = new() {
					TerrainSlopeType.InclineNorth,
					TerrainSlopeType.InclineEast,
					TerrainSlopeType.ConvexNortheast,
					TerrainSlopeType.ConcaveNortheast,
					TerrainSlopeType.ConcaveSoutheast,
					TerrainSlopeType.ConcaveNorthwest
				};

				List<TerrainSlopeType> yellowLiftTypes = new() {
					TerrainSlopeType.InclineEast,
					TerrainSlopeType.InclineSouth,
					TerrainSlopeType.ConvexSoutheast,
					TerrainSlopeType.ConcaveNortheast,
					TerrainSlopeType.ConcaveSoutheast,
					TerrainSlopeType.ConcaveSouthwest
				};

				List<TerrainSlopeType> blueLiftTypes = new() {
					TerrainSlopeType.InclineSouth,
					TerrainSlopeType.InclineWest,
					TerrainSlopeType.ConvexSouthwest,
					TerrainSlopeType.ConcaveSoutheast,
					TerrainSlopeType.ConcaveSouthwest,
					TerrainSlopeType.ConcaveNorthwest
				};

				Vector3 liftAmount = new(0, 12 + (terrainTile.SlopeHeight + terrainTile.Depth - 1) * 12, 0);

				if (redLiftTypes.Contains(terrainTile.SlopeType)) {
					red += liftAmount;
				}

				if (greenLiftTypes.Contains(terrainTile.SlopeType)) {
					green += liftAmount;
				}

				if (blueLiftTypes.Contains(terrainTile.SlopeType)) {
					blue += liftAmount;
				}

				if (yellowLiftTypes.Contains(terrainTile.SlopeType)) {
					yellow += liftAmount;
				}

				List<Vertex> vertices = new() {
					new Vertex(red, Color.Red, true),
					new Vertex(green, Color.Green, true),
					new Vertex(blue, Color.Blue, true),
					new Vertex(yellow, Color.Yellow, true)
				};

				List<Vector2> horizontalUvs = new() {
					new Vector2(9, 9),
					new Vector2(37, 9),
					new Vector2(9, 37),
					new Vector2(37, 37)
				};

				Polygon addedPolygon = CurrentMapState.CreatePolygon(vertices, horizontalUvs, MeshType.PrimaryMesh);
				addedPolygon.PaletteId = 0;
				surfaceMap.Last().Add(addedPolygon);
			}
		}

		return surfaceMap;
	}

	private static void BuildGreyboxWalls(List<List<Polygon>> surfaceMap) {
		for (int rowIndex = 0; rowIndex < surfaceMap.Count; rowIndex++) {
			for (int colIndex = 0; colIndex < surfaceMap[rowIndex].Count; colIndex++) {
				if (rowIndex < surfaceMap.Count - 1) {
					BuildGreyboxWall("North", surfaceMap[rowIndex][colIndex].Vertices, surfaceMap[rowIndex + 1][colIndex].Vertices, false);
				}

				if (colIndex < surfaceMap[rowIndex].Count - 1) {
					BuildGreyboxWall("East", surfaceMap[rowIndex][colIndex].Vertices, surfaceMap[rowIndex][colIndex + 1].Vertices, false);
				}

				List<Vertex> baseFloor = new() {
					new Vertex(surfaceMap[rowIndex][colIndex].Vertices[2].Position, Color.Purple, true),
					new Vertex(surfaceMap[rowIndex][colIndex].Vertices[3].Position, Color.Purple, true),
					new Vertex(surfaceMap[rowIndex][colIndex].Vertices[0].Position, Color.Purple, true),
					new Vertex(surfaceMap[rowIndex][colIndex].Vertices[1].Position, Color.Purple, true)
				};

				foreach (Vertex vertex in baseFloor) {
					vertex.Position = new Vector3(vertex.Position.X, 0, vertex.Position.Z);
				}

				if (rowIndex == 0) {
					BuildGreyboxWall("North", baseFloor, surfaceMap[rowIndex][colIndex].Vertices, true);
				}

				if (rowIndex == surfaceMap.Count - 1) {
					BuildGreyboxWall("North", surfaceMap[rowIndex][colIndex].Vertices, baseFloor, true);
				}

				baseFloor = new List<Vertex> {
					new(surfaceMap[rowIndex][colIndex].Vertices[1].Position, Color.Purple, true),
					new(surfaceMap[rowIndex][colIndex].Vertices[0].Position, Color.Purple, true),
					new(surfaceMap[rowIndex][colIndex].Vertices[3].Position, Color.Purple, true),
					new(surfaceMap[rowIndex][colIndex].Vertices[2].Position, Color.Purple, true)
				};

				foreach (Vertex vertex in baseFloor) {
					vertex.Position = new Vector3(vertex.Position.X, 0, vertex.Position.Z);
				}

				if (colIndex == 0) {
					BuildGreyboxWall("East", baseFloor, surfaceMap[rowIndex][colIndex].Vertices, true);
				}

				if (colIndex == surfaceMap[rowIndex].Count - 1) {
					BuildGreyboxWall("East", surfaceMap[rowIndex][colIndex].Vertices, baseFloor, true);
				}
			}
		}
	}

	private static void BuildGreyboxWall(string direction, List<Vertex> surfaceVertices, List<Vertex> destinationVertices, bool untextured) {
		List<Vector3> vertices = GetWallShape(direction, surfaceVertices, destinationVertices);
		BuildWallPolygons(vertices, untextured);
	}

	private static List<Vector3> GetWallShape(string direction, List<Vertex> surfaceVertices, List<Vertex> destinationVertices) {
		Vector3 topLeft = new();
		Vector3 topRight = new();
		Vector3 bottomLeft = new();
		Vector3 bottomRight = new();

		if (direction == "North") {
			bool destinationIsHigher = destinationVertices[2].Position.Y > surfaceVertices[0].Position.Y ||
			                           destinationVertices[3].Position.Y > surfaceVertices[1].Position.Y;
			if (destinationIsHigher) {
				topLeft = destinationVertices[2].Position + Vector3.Zero;
				topRight = destinationVertices[3].Position + Vector3.Zero;
				bottomLeft = surfaceVertices[0].Position + Vector3.Zero;
				bottomRight = surfaceVertices[1].Position + Vector3.Zero;
			} else {
				topRight = surfaceVertices[0].Position + Vector3.Zero;
				topLeft = surfaceVertices[1].Position + Vector3.Zero;
				bottomRight = destinationVertices[2].Position + Vector3.Zero;
				bottomLeft = destinationVertices[3].Position + Vector3.Zero;
			}
		}

		if (direction == "East") {
			bool destinationIsHigher = destinationVertices[0].Position.Y > surfaceVertices[1].Position.Y ||
			                           destinationVertices[2].Position.Y > surfaceVertices[3].Position.Y;

			if (destinationIsHigher) {
				topLeft = destinationVertices[0].Position + Vector3.Zero;
				topRight = destinationVertices[2].Position + Vector3.Zero;
				bottomLeft = surfaceVertices[1].Position + Vector3.Zero;
				bottomRight = surfaceVertices[3].Position + Vector3.Zero;
			} else {
				bottomRight = destinationVertices[0].Position + Vector3.Zero;
				bottomLeft = destinationVertices[2].Position + Vector3.Zero;
				topRight = surfaceVertices[1].Position + Vector3.Zero;
				topLeft = surfaceVertices[3].Position + Vector3.Zero;
			}
		}

		return new List<Vector3> {
			topLeft,
			topRight,
			bottomLeft,
			bottomRight
		};
	}

	private static void BuildWallPolygons(List<Vector3> vertices, bool untextured) {
		bool isXAligned = (int) vertices[0].Z == (int) vertices[1].Z;
		bool leftOverlaps = (vertices[0] - vertices[2]).Length() <= 0.001;
		bool rightOverlaps = (vertices[1] - vertices[3]).Length() <= 0.001;
		bool bothMatch = leftOverlaps && rightOverlaps;

		if (bothMatch) {
			return;
		}

		if (leftOverlaps) {
			vertices[2] = new Vector3(
				vertices[3].X,
				vertices[3].Y,
				vertices[3].Z
			);
			vertices.RemoveAt(vertices.Count - 1);
		}

		if (rightOverlaps) {
			vertices.RemoveAt(vertices.Count - 1);
		}

		bool isTriangle = vertices.Count == 3;
		bool isRect = vertices.Count == 4 &&
		              (int) vertices[0].Y == (int) vertices[1].Y &&
		              (int) vertices[2].Y == (int) vertices[3].Y;
		bool isIsoscelesTrapezoid = vertices.Count == 4 &&
		                            ((int) vertices[0].Y == (int) vertices[1].Y &&
		                             (int) vertices[2].Y != (int) vertices[3].Y ||
		                             (int) vertices[0].Y != (int) vertices[1].Y &&
		                             (int) vertices[2].Y == (int) vertices[3].Y);
		bool isRhombus = vertices.Count == 4 &&
		                 (int) vertices[0].Y != (int) vertices[1].Y &&
		                 (int) vertices[2].Y != (int) vertices[3].Y;

		if (_buildSinglePolyWalls) {
			MakeSinglePolyWall(vertices, untextured);
		} else if (isTriangle) {
			MakeTriangle(vertices, isXAligned, untextured);
		} else if (isRect) {
			MakeRectangles(vertices, untextured);
		} else if (isIsoscelesTrapezoid) {
			MakeIsoscelesTrapezoid(vertices, isXAligned, untextured);
		} else if (isRhombus) {
			MakeRhombus(vertices, isXAligned, untextured);
		}
	}

	private static void MakeSinglePolyWall(List<Vector3> vertices, bool untextured) {
		List<Vertex> verticesToBuild = new() {
			new Vertex(vertices[0] + Vector3.Zero, Color.Red, true),
			new Vertex(vertices[1] + Vector3.Zero, Color.Green, true),
			new Vertex(vertices[2] + Vector3.Zero, Color.Blue, true),
		};

		List<Vector2> uvs = new() {
			new Vector2(9, 37),
			new Vector2(37, 37),
			new Vector2(9, 49)
		};

		if (vertices.Count == 4) {
			verticesToBuild.Add(new Vertex(vertices[3] + Vector3.Zero, Color.Yellow, true));
			uvs.Add(new Vector2(37, 49));
		}
		
		Polygon addedPolygon = CurrentMapState.CreatePolygon(verticesToBuild, untextured ? new List<Vector2>() : uvs, MeshType.PrimaryMesh);
		addedPolygon.PaletteId = 1;
	}

	private static void MakeTriangle(List<Vector3> vertices, bool isXAligned, bool untextured) {
		bool leftLong = isXAligned
			? (int) vertices[0].X == (int) vertices[2].X
			: (int) vertices[0].Z == (int) vertices[2].Z;

		int height = leftLong
			? (int) Math.Abs(vertices[0].Y - vertices[2].Y)
			: (int) Math.Abs(vertices[1].Y - vertices[2].Y);

		int segments = height / 12;

		if (leftLong) {
			for (int segmentIndex = 0; segmentIndex < segments; segmentIndex++) {
				List<Vector2> uvs = new() {
					new Vector2(9, 37),
					new Vector2(37, 37),
					new Vector2(9, 49)
				};

				List<Vertex> verticesToBuild = new() {
					new Vertex(vertices[0] - new Vector3(0, segmentIndex * 12, 0), Color.Red, true),
					new Vertex(vertices[1] + Vector3.Zero, Color.Green, true),
					new Vertex(vertices[0] - new Vector3(0, (segmentIndex + 1) * 12, 0), Color.Blue, true),
				};

				Polygon addedPolygon = CurrentMapState.CreatePolygon(verticesToBuild, untextured ? new List<Vector2>() : uvs, MeshType.PrimaryMesh);
				addedPolygon.PaletteId = 1;
			}
		} else {
			for (int segmentIndex = 0; segmentIndex < segments; segmentIndex++) {
				List<Vector2> uvs = new() {
					new Vector2(9, 37),
					new Vector2(37, 37),
					new Vector2(9, 49)
				};

				List<Vertex> verticesToBuild = new() {
					new Vertex(vertices[0] + Vector3.Zero, Color.Red, true),
					new Vertex(vertices[1] - new Vector3(0, segmentIndex * 12, 0), Color.Green, true),
					new Vertex(vertices[1] - new Vector3(0, (segmentIndex + 1) * 12, 0), Color.Blue, true),
				};

				Polygon addedPolygon = CurrentMapState.CreatePolygon(verticesToBuild, untextured ? new List<Vector2>() : uvs, MeshType.PrimaryMesh);
				addedPolygon.PaletteId = 1;
			}
		}
	}

	private static void MakeRectangles(List<Vector3> vertices, bool untextured) {
		int height = (int) Math.Abs(vertices[0].Y - vertices[2].Y);
		int segments = height / 12;

		for (int segmentIndex = 0; segmentIndex < segments; segmentIndex++) {
			List<Vector2> uvs = new() {
				new Vector2(9, 37),
				new Vector2(37, 37),
				new Vector2(9, 49),
				new Vector2(37, 49)
			};

			List<Vertex> verticesToBuild = new() {
				new Vertex(vertices[0] - new Vector3(0, segmentIndex * 12, 0), Color.Red, true),
				new Vertex(vertices[1] - new Vector3(0, segmentIndex * 12, 0), Color.Green, true),
				new Vertex(vertices[0] - new Vector3(0, (segmentIndex + 1) * 12, 0), Color.Blue, true),
				new Vertex(vertices[1] - new Vector3(0, (segmentIndex + 1) * 12, 0), Color.Yellow, true)
			};

			Polygon addedPolygon = CurrentMapState.CreatePolygon(verticesToBuild, untextured ? new List<Vector2>() : uvs, MeshType.PrimaryMesh);
			addedPolygon.PaletteId = 1;
		}
	}

	private static void MakeIsoscelesTrapezoid(List<Vector3> vertices, bool isXAligned, bool untextured) {
		bool pointEndUp = (int) vertices[0].Y != (int) vertices[1].Y;

		List<Vector3> triangleVertices = new();
		List<Vector3> rectangleVertices = new();

		if (pointEndUp) {
			bool pointEndLeft = (int) vertices[0].Y > (int) vertices[1].Y;

			if (pointEndLeft) {
				triangleVertices.Add(vertices[0] + Vector3.Zero);
				triangleVertices.Add(vertices[1] + Vector3.Zero);
				triangleVertices.Add(new Vector3(vertices[0].X, vertices[1].Y, vertices[0].Z));

				rectangleVertices.Add(new Vector3(vertices[0].X, vertices[1].Y, vertices[0].Z));
				rectangleVertices.Add(vertices[1] + Vector3.Zero);
				rectangleVertices.Add(vertices[2] + Vector3.Zero);
				rectangleVertices.Add(vertices[3] + Vector3.Zero);
			} else {
				triangleVertices.Add(vertices[0] + Vector3.Zero);
				triangleVertices.Add(vertices[1] + Vector3.Zero);
				triangleVertices.Add(new Vector3(vertices[1].X, vertices[0].Y, vertices[1].Z));

				rectangleVertices.Add(vertices[0] + Vector3.Zero);
				rectangleVertices.Add(new Vector3(vertices[1].X, vertices[0].Y, vertices[1].Z));
				rectangleVertices.Add(vertices[2] + Vector3.Zero);
				rectangleVertices.Add(vertices[3] + Vector3.Zero);
			}
		} else {
			bool pointEndLeft = (int) vertices[2].Y < (int) vertices[3].Y;

			if (pointEndLeft) {
				triangleVertices.Add(new Vector3(vertices[0].X, vertices[3].Y, vertices[0].Z));
				triangleVertices.Add(vertices[3] + Vector3.Zero);
				triangleVertices.Add(vertices[2] + Vector3.Zero);

				rectangleVertices.Add(vertices[0] + Vector3.Zero);
				rectangleVertices.Add(vertices[1] + Vector3.Zero);
				triangleVertices.Add(new Vector3(vertices[0].X, vertices[3].Y, vertices[0].Z));
				rectangleVertices.Add(vertices[3] + Vector3.Zero);
			} else {
				triangleVertices.Add(vertices[2] + Vector3.Zero);
				triangleVertices.Add(new Vector3(vertices[1].X, vertices[2].Y, vertices[1].Z));
				triangleVertices.Add(vertices[3] + Vector3.Zero);

				rectangleVertices.Add(vertices[0] + Vector3.Zero);
				rectangleVertices.Add(vertices[1] + Vector3.Zero);
				rectangleVertices.Add(vertices[2] + Vector3.Zero);
				triangleVertices.Add(new Vector3(vertices[1].X, vertices[2].Y, vertices[1].Z));
			}
		}

		MakeTriangle(triangleVertices, isXAligned, untextured);
		MakeRectangles(rectangleVertices, untextured);
	}

	private static void MakeRhombus(List<Vector3> vertices, bool isXAligned, bool untextured) {
		bool topPointEndLeft = vertices[0].Y > vertices[1].Y;
		bool bottomPointEndLeft = vertices[2].Y < vertices[3].Y;

		bool isIrregular = topPointEndLeft != bottomPointEndLeft &&
		                   (topPointEndLeft && vertices[2].Y > vertices[1].Y ||
		                    !topPointEndLeft && vertices[3].Y > vertices[0].Y);

		if (isIrregular) {
			if (topPointEndLeft) {
				MakeTriangle(new List<Vector3> {
					vertices[0] + Vector3.Zero,
					vertices[1] + Vector3.Zero,
					vertices[2] + Vector3.Zero
				}, isXAligned, untextured);
				MakeTriangle(new List<Vector3> {
					vertices[2] + Vector3.Zero,
					vertices[1] + Vector3.Zero,
					vertices[3] + Vector3.Zero
				}, isXAligned, untextured);
			} else {
				MakeTriangle(new List<Vector3> {
					vertices[0] + Vector3.Zero,
					vertices[1] + Vector3.Zero,
					vertices[3] + Vector3.Zero
				}, isXAligned, untextured);
				MakeTriangle(new List<Vector3> {
					vertices[0] + Vector3.Zero,
					vertices[3] + Vector3.Zero,
					vertices[2] + Vector3.Zero
				}, isXAligned, untextured);
			}

			return;
		}

		List<Vector3> topTriangleVertices = new();
		List<Vector3> rectangleVertices = new();
		List<Vector3> bottomTriangleVertices = new();

		if (topPointEndLeft == bottomPointEndLeft) {
			if (topPointEndLeft) {
				topTriangleVertices.Add(vertices[0] + Vector3.Zero);
				topTriangleVertices.Add(vertices[1] + Vector3.Zero);
				topTriangleVertices.Add(new Vector3(vertices[0].X, vertices[1].Y, vertices[0].Z));

				rectangleVertices.Add(new Vector3(vertices[0].X, vertices[1].Y, vertices[0].Z));
				rectangleVertices.Add(vertices[1] + Vector3.Zero);
				rectangleVertices.Add(new Vector3(vertices[0].X, vertices[2].Y, vertices[0].Z));
				rectangleVertices.Add(vertices[3] + Vector3.Zero);

				bottomTriangleVertices.Add(new Vector3(vertices[0].X, vertices[3].Y, vertices[0].Z));
				bottomTriangleVertices.Add(vertices[3] + Vector3.Zero);
				bottomTriangleVertices.Add(vertices[2] + Vector3.Zero);
			} else {
				topTriangleVertices.Add(vertices[0] + Vector3.Zero);
				topTriangleVertices.Add(vertices[1] + Vector3.Zero);
				topTriangleVertices.Add(new Vector3(vertices[1].X, vertices[0].Y, vertices[1].Z));

				rectangleVertices.Add(new Vector3(vertices[0].X, vertices[0].Y, vertices[0].Z));
				rectangleVertices.Add(new Vector3(vertices[1].X, vertices[0].Y, vertices[1].Z));
				rectangleVertices.Add(vertices[2] + Vector3.Zero);
				rectangleVertices.Add(new Vector3(vertices[1].X, vertices[3].Y, vertices[1].Z));

				bottomTriangleVertices.Add(vertices[2] + Vector3.Zero);
				bottomTriangleVertices.Add(new Vector3(vertices[1].X, vertices[2].Y, vertices[1].Z));
				bottomTriangleVertices.Add(vertices[3] + Vector3.Zero);
			}

			MakeTriangle(topTriangleVertices, isXAligned, untextured);
			MakeRectangles(rectangleVertices, untextured);
			MakeTriangle(bottomTriangleVertices, isXAligned, untextured);
		} else {
			if (topPointEndLeft) {
				topTriangleVertices.Add(vertices[0] + Vector3.Zero);
				topTriangleVertices.Add(vertices[1] + Vector3.Zero);
				topTriangleVertices.Add(new Vector3(vertices[0].X, vertices[1].Y, vertices[0].Z));

				rectangleVertices.Add(new Vector3(vertices[0].X, vertices[1].Y, vertices[0].Z));
				rectangleVertices.Add(vertices[1] + Vector3.Zero);
				rectangleVertices.Add(vertices[2] + Vector3.Zero);
				rectangleVertices.Add(new Vector3(vertices[1].X, vertices[2].Y, vertices[1].Z));

				bottomTriangleVertices.Add(vertices[2] + Vector3.Zero);
				bottomTriangleVertices.Add(new Vector3(vertices[1].X, vertices[2].Y, vertices[1].Z));
				bottomTriangleVertices.Add(vertices[3] + Vector3.Zero);
			} else {
				topTriangleVertices.Add(vertices[0] + Vector3.Zero);
				topTriangleVertices.Add(vertices[1] + Vector3.Zero);
				topTriangleVertices.Add(new Vector3(vertices[1].X, vertices[0].Y, vertices[1].Z));

				rectangleVertices.Add(vertices[0] + Vector3.Zero);
				rectangleVertices.Add(new Vector3(vertices[1].X, vertices[0].Y, vertices[1].Z));
				rectangleVertices.Add(vertices[2] + Vector3.Zero);
				rectangleVertices.Add(new Vector3(vertices[3].X, vertices[2].Y, vertices[3].Z));

				bottomTriangleVertices.Add(vertices[2] + Vector3.Zero);
				bottomTriangleVertices.Add(new Vector3(vertices[1].X, vertices[2].Y, vertices[1].Z));
				bottomTriangleVertices.Add(vertices[3] + Vector3.Zero);
			}

			MakeTriangle(topTriangleVertices, isXAligned, untextured);
			MakeRectangles(rectangleVertices, untextured);
			MakeTriangle(bottomTriangleVertices, isXAligned, untextured);
		}
	}

	private static void PaintTexture() {
		Texture2D texture2D = CurrentMapState.StateData.Texture;
		Color[] textureColors = new Color[texture2D.Width * texture2D.Height];
		texture2D.GetData(textureColors);

		Color color0 = new(0, 0, 0, 1);
		Color color1 = new(17, 17, 17, 1);

		for (int x = 8; x <= 38; x++) {
			for (int y = 8; y <= 38; y++) {
				if (x <= 9 || x >= 36 || y <= 9 || y >= 36) {
					SetPixel(x, y, color0, textureColors);
				} else {
					SetPixel(x, y, color1, textureColors);
				}
			}
		}

		for (int x = 8; x <= 38; x++) {
			for (int y = 38; y <= 49; y++) {
				if (x <= 9 || x >= 36 || y <= 37 || y >= 48) {
					SetPixel(x, y, color0, textureColors);
				} else {
					SetPixel(x, y, color1, textureColors);
				}
			}
		}

		texture2D.SetData(textureColors);
	}

	private static void SetPalette() {
		CurrentMapState.StateData.Palettes[0].Colors[0] = new PaletteColor(new Color(0, 74, 0, 1));
		CurrentMapState.StateData.Palettes[0].Colors[1] = new PaletteColor(new Color(0, 58, 0, 1));
		CurrentMapState.StateData.Palettes[1].Colors[0] = new PaletteColor(new Color(74, 58, 0, 1));
		CurrentMapState.StateData.Palettes[1].Colors[1] = new PaletteColor(new Color(66, 49, 0, 1));
	}

	private static void SetPixel(int x, int y, Color color, Color[] textureColors) {
		int index = y * 256 + x;
		textureColors[index] = color;
	}
}