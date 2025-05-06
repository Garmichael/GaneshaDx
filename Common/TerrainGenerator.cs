//	Written as-is by https://github.com/Duodecimus

using System;
using System.Collections.Generic;
using System.Linq;
using GaneshaDx.Resources;
using GaneshaDx.Resources.ContentDataTypes.Polygons;
using GaneshaDx.UserInterface;
using Microsoft.Xna.Framework;

namespace GaneshaDx.Common;	

public static class TerrainGenerator {
	public static void Generate() {
		// Figure out how large the terrain grid should be
		int farthestZ = 0;
		int farthestX = 0;
		Dictionary<Tuple<int, int>, List<Polygon>> PolysInEachTerrain = new();
		foreach (var (polygontype, polylist) in CurrentMapState.StateData.PolygonCollection[MeshType.PrimaryMesh]) {
			foreach (Polygon poly in polylist) {
				float minX = float.MaxValue, maxX = float.MinValue;
				float minZ = float.MaxValue, maxZ = float.MinValue;
				// Work out the full size of the model, and the max XZ dimensions of the current poly 
				foreach (Vertex vert in poly.Vertices) {
					if (Math.Abs(vert.Position.X) > farthestX) {
						farthestX = Math.Abs((int) vert.Position.X);
					}

					if (Math.Abs(vert.Position.Z) > farthestZ) {
						farthestZ = Math.Abs((int) vert.Position.Z);
					}

					minX = Math.Min(minX, Math.Abs(vert.Position.X));
					maxX = Math.Max(maxX, Math.Abs(vert.Position.X));
					minZ = Math.Min(minZ, Math.Abs(vert.Position.Z));
					maxZ = Math.Max(maxZ, Math.Abs(vert.Position.Z));
				}

				// Calculate the tiles the polygon might touch
				int startX = (int) (MathF.Floor(MathF.Abs(minX) / 28));
				int endX = (int) (MathF.Floor(MathF.Abs(maxX) / 28));
				int startZ = (int) (MathF.Floor(MathF.Abs(minZ) / 28));
				int endZ = (int) (MathF.Floor(MathF.Abs(maxZ) / 28));

				// Add the polygon to all tiles it touches
				for (int x = startX; x <= endX; x++) {
					for (int z = startZ; z <= endZ; z++) {
						// Check if the polygon's center is inside the tile, if it's edges intersects the tile, or if it fully covers the current tile
						if ((MathF.Floor(MathF.Abs(poly.AveragePoint.X) / 28) == x && MathF.Floor(MathF.Abs(poly.AveragePoint.Z) / 28) == z) ||
						    DoEdgesIntersectGrid(poly, x, z) ||
						    IsTileFullyCoveredByPolygon(poly, x, z)) {
							Tuple<int, int> terraintile = new(x, z);
							// Ensure the terrain tile exists in the dictionary
							PolysInEachTerrain.TryAdd(terraintile, new List<Polygon>());
							// Add the polygon to this tile's list
							PolysInEachTerrain[terraintile].Add(poly);
						}
					}
				}
			}
		}

		// Set terrain to those dimensions
		Selection.SelectedTerrainTiles.Clear();
		CurrentMapState.StateData.Terrain.ResizeTerrain(0, 0, true);
		CurrentMapState.StateData.Terrain.ResizeTerrain(farthestX / 28, farthestZ / 28, true);

		// For each tile in terrain find the model tri/quads above it
		for (int indexZ = 0; indexZ < CurrentMapState.StateData.Terrain.SizeZ; indexZ++) {
			for (int indexX = 0; indexX < CurrentMapState.StateData.Terrain.SizeX; indexX++) {
				TerrainSlopeType slopeType0;
				TerrainSlopeType slopeType1;
				float upperMinY, upperMaxY, lowerMinY, lowerMaxY;
				Boolean unselectable0, unselectable1;
				Boolean impassable0, impassable1;

				// Generate a center vertex based on the tile dimensions
				var centerX = (indexX * 28 + 14);
				var centerZ = (indexZ * 28 + 14);

				Tuple<int, int> tileindex = new(indexX, indexZ);
				List<Polygon> polyList;
				PolysInEachTerrain.TryGetValue(tileindex, out polyList);
				// Determine TerrainSlopeType and slope height
				if (polyList != null) {
					var (upperTile, lowerTile) = AnalyzePolygons(polyList, centerX, centerZ);


					// Determine the slope type based on the analysis
					if (lowerTile.Count > 0) {
						(slopeType0, upperMinY, upperMaxY) = DetermineSlopeType(upperTile, centerX, centerZ);
						(slopeType1, lowerMinY, lowerMaxY) = DetermineSlopeType(lowerTile, centerX, centerZ);

						if (upperMaxY == 0 && upperMinY == 0 && lowerMaxY == 0 && lowerMinY == 0) {
							// Set lower to unselectable, and upper to impassable
							impassable0 = true;
							unselectable0 = false;
							impassable1 = false;
							unselectable1 = true;
						} else if (lowerMaxY == 0 && lowerMinY == 0) {
							// Set lower to unselectable
							impassable1 = false;
							unselectable1 = true;
							impassable0 = false;
							unselectable0 = false;
						} else {
							// Set both to normal terrain
							impassable1 = false;
							unselectable1 = false;
							impassable0 = false;
							unselectable0 = false;
						}
					} else if (upperTile.Count > 0) {
						(slopeType0, upperMinY, upperMaxY) = DetermineSlopeType(upperTile, centerX, centerZ);
						slopeType1 = TerrainSlopeType.Flat;
						lowerMinY = 0;
						lowerMaxY = 0;
						if (upperMaxY == 0 && upperMinY == 0) {
							impassable0 = true;
							unselectable0 = false;
							impassable1 = false;
							unselectable1 = true;
						} else {
							impassable1 = false;
							unselectable1 = true;
							impassable0 = false;
							unselectable0 = false;
						}
					} else {
						slopeType0 = TerrainSlopeType.Flat;
						upperMinY = 0;
						upperMaxY = 0;
						impassable0 = true;
						unselectable0 = false;

						slopeType1 = TerrainSlopeType.Flat;
						lowerMinY = 0;
						lowerMaxY = 0;
						impassable1 = false;
						unselectable1 = true;
					}
				} else {
					slopeType0 = TerrainSlopeType.Flat;
					upperMinY = 0;
					upperMaxY = 0;
					impassable0 = true;
					unselectable0 = false;

					slopeType1 = TerrainSlopeType.Flat;
					lowerMinY = 0;
					lowerMaxY = 0;
					impassable1 = false;
					unselectable1 = true;
				}

				CurrentMapState.StateData.Terrain.Level0Tiles[indexZ][indexX].SlopeType = slopeType0;
				CurrentMapState.StateData.Terrain.Level0Tiles[indexZ][indexX].Height = (int) MathF.Floor(upperMinY / 12); // Each point of slope height is worth 12 points of Y values
				CurrentMapState.StateData.Terrain.Level0Tiles[indexZ][indexX].SlopeHeight = (int) MathF.Floor((upperMaxY - upperMinY) / 12);
				CurrentMapState.StateData.Terrain.Level0Tiles[indexZ][indexX].Impassable = impassable0;
				CurrentMapState.StateData.Terrain.Level0Tiles[indexZ][indexX].Unselectable = unselectable0;

				CurrentMapState.StateData.Terrain.Level1Tiles[indexZ][indexX].SlopeType = slopeType1;
				CurrentMapState.StateData.Terrain.Level1Tiles[indexZ][indexX].Height = (int) MathF.Floor(lowerMinY / 12);
				CurrentMapState.StateData.Terrain.Level1Tiles[indexZ][indexX].SlopeHeight = (int) MathF.Floor((lowerMaxY - lowerMinY) / 12);
				CurrentMapState.StateData.Terrain.Level1Tiles[indexZ][indexX].Impassable = impassable1;
				CurrentMapState.StateData.Terrain.Level1Tiles[indexZ][indexX].Unselectable = unselectable1;
			}
		}
	}

	public static bool DoEdgesIntersectGrid(Polygon poly, int gridX, int gridZ) {
		// Get the boundaries of the current grid cell
		float minTileX = gridX * 28;
		float maxTileX = (gridX + 1) * 28;
		float minTileZ = gridZ * 28;
		float maxTileZ = (gridZ + 1) * 28;

		// Iterate through each edge of the polygon
		for (int i = 0, j = poly.Vertices.Count - 1; i < poly.Vertices.Count; j = i++) {
			// Flip the sign on X, as Ganesha has them negative
			Vector3 v1 = new(-poly.Vertices[i].Position.X, poly.Vertices[i].Position.Y, poly.Vertices[i].Position.Z);
			Vector3 v2 = new(-poly.Vertices[j].Position.X, poly.Vertices[j].Position.Y, poly.Vertices[j].Position.Z);

			// Check if the edge between v1 and v2 intersects with the tile
			if (DoLineSegmentIntersectTile(v1, v2, minTileX, maxTileX, minTileZ, maxTileZ)) {
				return true; // If an edge intersects the tile, return true
			}
		}

		return false; // No intersection
	}

	/// <summary>
	/// Return true if the passed line interescts one of the edges of the described tile
	/// Lines that are perfectly aligned with an edge, or which end on an edge and otherwise do not enter the tile will return false
	/// </summary>
	/// <param name="v1"></param>
	/// <param name="v2"></param>
	/// <param name="minTileX"></param>
	/// <param name="maxTileX"></param>
	/// <param name="minTileZ"></param>
	/// <param name="maxTileZ"></param>
	/// <returns></returns>
	public static bool DoLineSegmentIntersectTile(Vector3 v1, Vector3 v2, float minTileX, float maxTileX, float minTileZ, float maxTileZ) {
		// Check if the line segment is entirely outside the tile in the X or Z direction
		if ((v1.X < minTileX && v2.X < minTileX) || (v1.X > maxTileX && v2.X > maxTileX) ||
		    (v1.Z < minTileZ && v2.Z < minTileZ) || (v1.Z > maxTileZ && v2.Z > maxTileZ)) {
			return false;
		}

		// Check if the line segment is overlapping one of the edges of the tile
		if ((v1.X == minTileX && v2.X == minTileX) || (v1.X == maxTileX && v2.X == maxTileX) ||
		    (v1.Z == minTileZ && v2.Z == minTileZ) || (v1.Z == maxTileZ && v2.Z == maxTileZ)) {
			return false;
		}

		// Box reference:
		// 1-2
		// | |
		// 3-4
		// Check if line segment ends on a side
		bool v1OnEdge12 = (v1.Z == maxTileZ && v1.X <= maxTileX && v1.X >= minTileX);
		bool v1OnEdge34 = (v1.Z == minTileZ && v1.X <= maxTileX && v1.X >= minTileX);
		bool v1OnEdge24 = (v1.X == maxTileX && v1.Z <= maxTileZ && v1.Z >= minTileZ);
		bool v1OnEdge13 = (v1.X == minTileX && v1.Z <= maxTileZ && v1.Z >= minTileZ);
		bool v2OnEdge12 = (v2.Z == maxTileZ && v2.X <= maxTileX && v2.X >= minTileX);
		bool v2OnEdge34 = (v2.Z == minTileZ && v2.X <= maxTileX && v2.X >= minTileX);
		bool v2OnEdge24 = (v2.X == maxTileX && v2.Z <= maxTileZ && v2.Z >= minTileZ);
		bool v2OnEdge13 = (v2.X == minTileX && v2.Z <= maxTileZ && v2.Z >= minTileZ);

		// Define the four edges of the tile		 
		Vector3[] squareCorners = new Vector3[] {
			new Vector3(minTileX, 0, maxTileZ), //1	 
			new Vector3(maxTileX, 0, maxTileZ), //2	 
			new Vector3(minTileX, 0, minTileZ), //3	 
			new Vector3(maxTileX, 0, minTileZ) //4
		};

		// The points are not on lines
		if (!v1OnEdge12 && !v1OnEdge34 && !v1OnEdge24 && !v1OnEdge13 &&
		    !v2OnEdge12 && !v2OnEdge34 && !v2OnEdge24 && !v2OnEdge13) {
			// Check intersection with each edge of the tile
			for (int i = 0; i < 4; i++) {
				Vector3 edgeStart = squareCorners[i];
				Vector3 edgeEnd = squareCorners[(i + 1) % 4];

				if (DoLineSegmentsIntersect(v1, v2, edgeStart, edgeEnd)) {
					return true;
				}
			}
		}

		// Both points are on the same line
		if ((v1OnEdge12 && v2OnEdge12) ||
		    (v1OnEdge34 && v2OnEdge34) ||
		    (v1OnEdge24 && v2OnEdge24) ||
		    (v1OnEdge13 && v2OnEdge13)) {
			//This line only borders the tile, return false
			return false;
		}

		// The points are on two different lines
		if ((v1OnEdge12 && (v2OnEdge34 || v2OnEdge24 || v2OnEdge13)) ||
		    (v1OnEdge34 && (v2OnEdge12 || v2OnEdge24 || v2OnEdge13)) ||
		    (v1OnEdge24 && (v2OnEdge12 || v2OnEdge34 || v2OnEdge13)) ||
		    (v1OnEdge13 && (v2OnEdge12 || v2OnEdge34 || v2OnEdge24)) ||
		    (v2OnEdge12 && (v1OnEdge34 || v1OnEdge24 || v1OnEdge13)) ||
		    (v2OnEdge34 && (v1OnEdge12 || v1OnEdge24 || v1OnEdge13)) ||
		    (v2OnEdge24 && (v1OnEdge12 || v1OnEdge34 || v1OnEdge13)) ||
		    (v2OnEdge13 && (v1OnEdge12 || v1OnEdge34 || v1OnEdge24))) {
			return true;
		}

		// Only one point is on a line
		// Need to check if the line crosses any line other than the one the point is on
		// This should also handle a point being on a corner

		// Box reference:
		// 1-2
		// | |
		// 3-4

		// Line 12
		if (!(v1OnEdge12 || v2OnEdge12)) {
			Vector3 edgeStart = squareCorners[0];
			Vector3 edgeEnd = squareCorners[1];

			if (DoLineSegmentsIntersect(v1, v2, edgeStart, edgeEnd)) {
				return true;
			}
		}

		// Line 13
		if (!(v1OnEdge13 || v2OnEdge13)) {
			Vector3 edgeStart = squareCorners[0];
			Vector3 edgeEnd = squareCorners[2];

			if (DoLineSegmentsIntersect(v1, v2, edgeStart, edgeEnd)) {
				return true;
			}
		}

		// Line 24
		if (!(v1OnEdge24 || v2OnEdge24)) {
			Vector3 edgeStart = squareCorners[1];
			Vector3 edgeEnd = squareCorners[3];

			if (DoLineSegmentsIntersect(v1, v2, edgeStart, edgeEnd)) {
				return true;
			}
		}

		// Line 34
		if (!(v1OnEdge34 || v2OnEdge34)) {
			Vector3 edgeStart = squareCorners[2];
			Vector3 edgeEnd = squareCorners[3];

			if (DoLineSegmentsIntersect(v1, v2, edgeStart, edgeEnd)) {
				return true;
			}
		}

		return false;
	}

	private static bool DoLineSegmentsIntersect(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4) {
		// Project points onto XZ plane by setting the Y component to 0
		p1.Y = 0;
		p2.Y = 0;
		p3.Y = 0;
		p4.Y = 0;

		// Use the orientation method to check if the lines intersect
		return LinesIntersect(p1, p2, p3, p4);
	}

	private static bool LinesIntersect(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4) {
		// Orientation of ordered triplet (p, q, r)
		// The function returns a value:
		// 0 -> p, q and r are collinear
		// 1 -> Clockwise
		// 2 -> Counterclockwise
		int o1 = Orientation(p1, p2, p3);
		int o2 = Orientation(p1, p2, p4);
		int o3 = Orientation(p3, p4, p1);
		int o4 = Orientation(p3, p4, p2);

		// General case
		if (o1 != o2 && o3 != o4)
			return true;

		// Special case: Check if the points are collinear and lie on the segment
		if (o1 == 0 && OnSegment(p1, p2, p3)) return true;
		if (o2 == 0 && OnSegment(p1, p2, p4)) return true;
		if (o3 == 0 && OnSegment(p3, p4, p1)) return true;
		if (o4 == 0 && OnSegment(p3, p4, p2)) return true;

		return false;
	}

	private static int Orientation(Vector3 p, Vector3 q, Vector3 r) {
		float val = (q.Z - p.Z) * (r.X - q.X) - (q.X - p.X) * (r.Z - q.Z);
		if (val == 0)
			return 0; // Collinear
		return (val > 0) ? 1 : 2; // Clockwise or counterclockwise
	}

	private static bool OnSegment(Vector3 p, Vector3 q, Vector3 r) {
		// Check if point r lies on line segment pq
		return r.X <= Math.Max(p.X, q.X) && r.X >= Math.Min(p.X, q.X) &&
		       r.Z <= Math.Max(p.Z, q.Z) && r.Z >= Math.Min(p.Z, q.Z);
	}

	public static bool IsTileFullyCoveredByPolygon(Polygon poly, int gridX, int gridZ) {
		// Get the boundaries of the current grid cell
		float minTileX = gridX * 28;
		float maxTileX = (gridX + 1) * 28;
		float minTileZ = gridZ * 28;
		float maxTileZ = (gridZ + 1) * 28;

		// Check if the polygon fully covers the grid cell (i.e., the grid cell's bounds are fully inside the polygon's bounds)
		return (
			IsPointInPolygon(new Vector3(minTileX, 0, minTileZ), poly) &&
			IsPointInPolygon(new Vector3(maxTileX, 0, minTileZ), poly) &&
			IsPointInPolygon(new Vector3(minTileX, 0, maxTileZ), poly) &&
			IsPointInPolygon(new Vector3(maxTileX, 0, maxTileZ), poly)
		);
	}

	public static bool IsPointInPolygon(Vector3 point, Polygon poly) {
		// Ray-casting algorithm to check if the point is inside the polygon (same as before)
		int n = poly.Vertices.Count;
		bool inside = false;

		for (int i = 0, j = n - 1; i < n; j = i++) {
			Vertex vi = poly.Vertices[i];
			Vertex vj = poly.Vertices[j];

			if (((vi.Position.Z > point.Z) != (vj.Position.Z > point.Z)) &&
			    (point.X < (vj.Position.X - vi.Position.X) * (point.Z - vi.Position.Z) / (vj.Position.Z - vi.Position.Z) + vi.Position.X)) {
				inside = !inside;
			}
		}

		return inside;
	}

	public static (List<Vector3> lowerGroup, List<Vector3> upperGroup) AnalyzePolygons(List<Polygon> polygons, int centerX, int centerZ) {
		List<Vector3> validVertices = new List<Vector3>();
		Vector3 normal;

		// Process each polygon
		foreach (var polygon in polygons) {
			// Filp the sign on all X values, as Ganesha sets them all to negative for some reason
			var vertexList = FlipX(polygon);

			// Make a guess at a tile center point
			Vector3 tileCenter = new(centerX, polygon.AveragePoint.Y, centerZ);

			// Calculate the normal for the polygon
			normal = CalculateNormal(vertexList);

			// Skip polygons with normals facing down (i.e., downward-facing)
			if (normal.Y < 0) {
				continue;
			}

			// Skip polygons that are very nearly or completely vertical

			// The normal of the XZ plane is the Y-axis (0, 1, 0)
			Vector3 xzPlaneNormal = new Vector3(0, 1, 0);
			// Calculate the angle between the triangle's normal and the Y-axis
			float angle = (float) Math.Acos(Vector3.Dot(normal, xzPlaneNormal));

			// Convert the angle to degrees
			float angleDegrees = MathHelper.ToDegrees(angle);

			// Check if the angle is within 1 degree of 90 degrees (parallel to the XZ plane)
			if (Math.Abs(angleDegrees) <= 95 && Math.Abs(angleDegrees) >= 85) {
				continue;
			}

			// Add the polygon's vertices to the valid list and track the Y range
			foreach (var vertex in vertexList) {
				// If the vertex is very far from the tile center, get a vertex that is close to the tile
				// This is for very large polygons, such as a single flat quad acting as an entire floor of a room.
				var nearbyVertex = GetNearbyPoint(tileCenter, vertex, 14, 21); //a tile is 28 wide, 20 is ~the distance from the center to a corner. Use 21 to avoid messing with actual corners
				Vector3 nearbyVertexRounded = new(MathF.Round(nearbyVertex.X), MathF.Round(nearbyVertex.Y / 12) * 12, MathF.Round(nearbyVertex.Z));
				if (!validVertices.Contains(nearbyVertexRounded)) {
					validVertices.Add(nearbyVertexRounded);
				}
			}
		}

		var (upperTile, lowerTile) = SortAndGroupByY(validVertices);

		return (upperTile, lowerTile);
	}

	public static Vector3 GetNearbyPoint(Vector3 center, Vector3 input, float goalDistance, float allowedDistance) {
		// Calculate the vector between center and input
		Vector3 direction = input - center;

		// Get the length (magnitude) of the direction vector
		float length = direction.Length();

		// If the distance is greater than or equal to the length, return the input
		if (length <= allowedDistance) {
			return input;
		}

		// Normalize the direction vector and scale it by the desired distance
		direction.Normalize();
		return center + direction * goalDistance;
	}

	private static List<Vector3> FlipX(Polygon Polygon) {
		List<Vector3> resultList = new();
		for (int i = 0; i < Polygon.Vertices.Count; i++) {
			resultList.Add(new(Polygon.Vertices[i].Position.X * -1, Polygon.Vertices[i].Position.Y, Polygon.Vertices[i].Position.Z));
		}

		return resultList;
	}

	public static (List<Vector3> upperGroup, List<Vector3> lowerGroup) SortAndGroupByY(List<Vector3> vertices) {
		// Sort the list by Y values
		List<Vector3> sortedVertices = vertices.OrderBy(v => v.Y).ToList();

		// Find the minimum Y value and try to separate into two groups
		List<Vector3> lowerGroup = new();
		List<Vector3> upperGroup = new();

		// Separate the list into two groups based on the Y value separation of at least 50
		foreach (var vertex in sortedVertices) {
			if (lowerGroup.Count == 0 || vertex.Y <= lowerGroup.Last().Y + 50) {
				lowerGroup.Add(vertex);
			} else {
				upperGroup.Add(vertex);
			}
		}

		// If the lower group has fewer than 4 values, clear it
		if (lowerGroup.Count < 4) {
			lowerGroup.Clear();
		}

		if (upperGroup.Count < 4) {
			//Same for the upper group
			upperGroup.Clear();
		}

		// If only one group has an entry, make sure it is lowergroup, so the first return always has data
		if (upperGroup.Count == 0) {
			upperGroup = lowerGroup;
			lowerGroup = new();
		}

		// Return the two groups
		return (upperGroup, lowerGroup);
	}

	private static Vector3 CalculateNormal(List<Vector3> vertices) {
		// For a triangle
		if (vertices.Count == 3) {
			Vector3 v1 = vertices[1] - vertices[0];
			Vector3 v2 = vertices[2] - vertices[0];
			return Vector3.Cross(v1, v2);
		}
		// For a quad
		else if (vertices.Count == 4) {
			Vector3 v1 = vertices[1] - vertices[0];
			Vector3 v2 = vertices[2] - vertices[0];
			Vector3 normal1 = Vector3.Cross(v1, v2);

			v1 = vertices[2] - vertices[3];
			v2 = vertices[0] - vertices[3];
			Vector3 normal2 = Vector3.Cross(v1, v2);

			return Vector3.Normalize(normal1 + normal2);
		}

		return Vector3.Zero;
	}

	private static (TerrainSlopeType slopeType, float minY, float maxY) DetermineSlopeType(List<Vector3> vertices, int centerX, int centerZ) {
		// If we don't have enough vertices to make a full square, assume this isn't a tile
		if (vertices.Count <= 3) {
			return (TerrainSlopeType.Flat, 0, 0);
		}

		// Find the min, max, and average Y values
		float minY = vertices[0].Y, maxY = vertices[0].Y, aveY = 0;
		foreach (var vertex in vertices) {
			minY = Math.Min(minY, vertex.Y);
			maxY = Math.Max(maxY, vertex.Y);
			aveY += vertex.Y;
		}

		aveY /= vertices.Count;

		// Calculate the center of the tile
		Vector3 center = new(centerX, aveY, centerZ);

		// Classify each vertex into one of the four corners based on its position relative to the center
		float yNorthEast = float.MinValue, yNorthWest = float.MinValue, ySouthEast = float.MinValue, ySouthWest = float.MinValue;
		float yNorth = float.MinValue, yWest = float.MinValue, yEast = float.MinValue, ySouth = float.MinValue;
		bool hasNorthEast = false, hasNorthWest = false, hasSouthEast = false, hasSouthWest = false;
		bool hasNorth = false, hasWest = false, hasEast = false, hasSouth = false;

		foreach (var vertex in vertices) {
			// Determine the local position of each vertex relative to the center in the ZX plane
			bool isNorth = vertex.Z < center.Z; // North is negative Z (since we have a Z-axis pointing up)
			bool isEast = vertex.X < center.X; // East is negative X, because Ganesha reverses things
			bool isSouth = vertex.Z > center.Z; // Also calculate South and West, as points that = the center shouldn't be treated as possible corners
			bool isWest = vertex.X > center.X;

			// Find the largest Y value of all the points in each quadrent, and mark if at least one point lands in each quadrent
			if (isNorth && isEast) {
				yNorthEast = Math.Max(yNorthEast, vertex.Y);
				hasNorthEast = true;
			} else if (isNorth && isWest) {
				yNorthWest = Math.Max(yNorthWest, vertex.Y);
				hasNorthWest = true;
			} else if (isSouth && isEast) {
				ySouthEast = Math.Max(ySouthEast, vertex.Y);
				hasSouthEast = true;
			} else if (isSouth && isWest) {
				ySouthWest = Math.Max(ySouthWest, vertex.Y);
				hasSouthWest = true;
			}
			// Also find points centered on one side
			else if (isSouth && !isWest && !isEast) {
				ySouth = Math.Max(ySouth, vertex.Y);
				hasSouth = true;
			} else if (isNorth && !isWest && !isEast) {
				yNorth = Math.Max(yNorth, vertex.Y);
				hasNorth = true;
			} else if (isWest && !isSouth && !isNorth) {
				yWest = Math.Max(yWest, vertex.Y);
				hasWest = true;
			} else if (isEast && !isSouth && !isNorth) {
				yEast = Math.Max(yEast, vertex.Y);
				hasEast = true;
			}
		}

		// If you have both sides and are missing a corner, generate a corner from them
		// Will likely almost never happen in imported meshes, but FFT has a lot of squares with a corner cut off
		if (hasSouth && hasWest && !hasSouthWest) {
			ySouthWest = (ySouth + yWest) / 2;
			hasSouthWest = true;
		}

		if (hasSouth && hasEast && !hasSouthEast) {
			ySouthEast = (ySouth + yEast) / 2;
			hasSouthEast = true;
		}

		if (hasNorth && hasWest && !hasNorthWest) {
			yNorthWest = (yNorth + yWest) / 2;
			hasNorthWest = true;
		}

		if (hasNorth && hasEast && !hasNorthEast) {
			yNorthEast = (yNorth + yEast) / 2;
			hasNorthEast = true;
		}

		// If any corners are still missing, default to not a walkable tile, as the mesh has 0 points in an entire corner 
		// This is probably an overhang, or something set into a wall
		if (!hasNorthEast || !hasNorthWest || !hasSouthEast || !hasSouthWest) {
			return (TerrainSlopeType.Flat, 0, 0);
		}

		// If the Y-range is less than one tick of terrain height, return flat
		if (Math.Abs(maxY - minY) < 10) {
			return (TerrainSlopeType.Flat, aveY, aveY);
		}

		var minYCorner = Math.Min(yNorthEast, yNorthWest);
		minYCorner = Math.Min(minYCorner, ySouthEast);
		minYCorner = Math.Min(minYCorner, ySouthWest);
		var maxYCorner = Math.Max(yNorthEast, yNorthWest);
		maxYCorner = Math.Max(maxYCorner, ySouthEast);
		maxYCorner = Math.Max(maxYCorner, ySouthWest);

		// Divide y values by 12 and round to standardize to terrain heights
		yNorthEast = MathF.Round(yNorthEast / 12);
		yNorthWest = MathF.Round(yNorthWest / 12);
		ySouthEast = MathF.Round(ySouthEast / 12);
		ySouthWest = MathF.Round(ySouthWest / 12);

		// Analyze the Y-values of the corners to determine the slope type
		// All equal, then flat
		if (yNorthEast == yNorthWest && yNorthEast == ySouthEast && yNorthEast == ySouthWest) {
			return (TerrainSlopeType.Flat, maxYCorner, maxYCorner);
		}

		// All but one are equal, then convex/concave
		if (yNorthEast == yNorthWest && yNorthEast == ySouthEast) {
			if (ySouthWest > yNorthEast) {
				return (TerrainSlopeType.ConvexNortheast, minY, maxY);
			} else {
				return (TerrainSlopeType.ConcaveSouthwest, minYCorner, maxYCorner);
			}
		}

		if (yNorthWest == yNorthEast && yNorthWest == ySouthWest) {
			if (ySouthEast > yNorthWest) {
				return (TerrainSlopeType.ConvexNorthwest, minY, maxY);
			} else {
				return (TerrainSlopeType.ConcaveSoutheast, minYCorner, maxYCorner);
			}
		}

		if (ySouthEast == ySouthWest && ySouthEast == yNorthEast) {
			if (yNorthWest > ySouthEast) {
				return (TerrainSlopeType.ConvexSoutheast, minY, maxY);
			} else {
				return (TerrainSlopeType.ConcaveNorthwest, minYCorner, maxYCorner);
			}
		}

		if (ySouthWest == ySouthEast && ySouthWest == yNorthWest) {
			if (yNorthEast > ySouthWest) {
				return (TerrainSlopeType.ConvexSouthwest, minY, maxY);
			} else {
				return (TerrainSlopeType.ConcaveNortheast, minYCorner, maxYCorner);
			}
		}

		// Looser rules for slopes, so long as one side is higher than the other, slope.
		if (yNorthEast > ySouthEast && yNorthWest > ySouthWest) {
			return (TerrainSlopeType.InclineSouth, minYCorner, maxYCorner);
		}

		if (ySouthEast > yNorthEast && ySouthWest > yNorthWest) {
			return (TerrainSlopeType.InclineNorth, minYCorner, maxYCorner);
		}

		if (yNorthEast > yNorthWest && ySouthEast > ySouthWest) {
			return (TerrainSlopeType.InclineWest, minYCorner, maxYCorner);
		}

		if (yNorthWest > yNorthEast && ySouthWest > ySouthEast) {
			return (TerrainSlopeType.InclineEast, minYCorner, maxYCorner);
		}

		// If all else fails, flat on the highest point of the mesh
		return (TerrainSlopeType.Flat, maxY, maxY);
	}
}