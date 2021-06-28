using System.Collections.Generic;
using System.Linq;
using GaneshaDx.Common;
using GaneshaDx.Environment;
using GaneshaDx.Resources;
using GaneshaDx.Resources.ContentDataTypes.Polygons;
using GaneshaDx.Resources.ContentDataTypes.Terrains;
using GaneshaDx.UserInterface.GuiDefinitions;
using GaneshaDx.UserInterface.Input;
using GaneshaDx.UserInterface.Widgets;
using Microsoft.Xna.Framework;

namespace GaneshaDx.UserInterface {
	public static class Selection {
		public static readonly List<Polygon> SelectedPolygons = new List<Polygon>();
		public static List<Polygon> HoveredPolygons = new List<Polygon>();
		public static Vector3 MinOfSelection;
		public static Vector3 CenterOfSelection;
		public static Vector3 MaxOfSelection;
		private static Vector3 _hoverPosition = Vector3.Zero;

		public static readonly List<TerrainTile> SelectedTerrainTiles = new List<TerrainTile>();
		public static List<TerrainTile> HoveredTerrainTiles = new List<TerrainTile>();

		public static void Update() {
			if (!MapData.MapIsLoaded || MyraGui.IsActive) {
				return;
			}

			if (Gui.SelectedTab == RightPanelTab.Terrain) {
				HandleInputForTerrain();
			} else {
				HandleInputForPolygons();
			}
		}

		private static void HandleInputForPolygons() {
			SelectedTerrainTiles.Clear();
			HoveredTerrainTiles.Clear();
			HoveredPolygons.Clear();

			bool transformWidgetInUse = Gui.WidgetSelectionMode == WidgetSelectionMode.PolygonTranslate &&
			                            (TransformWidget.IsHovered || TransformWidget.IsActive) ||
			                            Gui.WidgetSelectionMode == WidgetSelectionMode.PolygonVertexTranslate &&
			                            (TransformWidget.IsHovered || TransformWidget.IsActive) ||
			                            Gui.WidgetSelectionMode == WidgetSelectionMode.PolygonEdgeTranslate &&
			                            (TransformWidget.IsHovered || TransformWidget.IsActive);

			bool rotationWidgetInUse = Gui.WidgetSelectionMode == WidgetSelectionMode.PolygonRotate &&
			                           RotationWidget.IsHovered;

			if (AppInput.MouseIsWithinModelViewport && !transformWidgetInUse && !rotationWidgetInUse) {
				CollectHoveredPolygons();
				ClickPolygonSelection();
			}

			SetAnchorPositions();
		}

		private static void ClickPolygonSelection() {
			if (AppInput.LeftMouseClicked) {
				if (HoveredPolygons.Count == 0) {
					SelectedPolygons.Clear();
				} else {
					int indexOfSelected = 0;

					if (Gui.WidgetSelectionMode == WidgetSelectionMode.PolygonVertexTranslate ||
					    Gui.WidgetSelectionMode == WidgetSelectionMode.PolygonEdgeTranslate
					) {
						if (AppInput.ControlHeld) {
							Gui.WidgetSelectionMode = WidgetSelectionMode.PolygonTranslate;
						}
					}

					if (Gui.WidgetSelectionMode != WidgetSelectionMode.PolygonVertexTranslate &&
					    Gui.WidgetSelectionMode != WidgetSelectionMode.PolygonEdgeTranslate
					) {
						if (!AppInput.ControlHeld && SelectedPolygons.Count == 1) {
							for (int polyIndex = 0; polyIndex < HoveredPolygons.Count; polyIndex++) {
								Polygon polygon = HoveredPolygons[polyIndex];
								if (SelectedPolygons.Contains(polygon)) {
									indexOfSelected = polyIndex + 1;
									if (indexOfSelected > HoveredPolygons.Count - 1) {
										indexOfSelected = 0;
									}
								}
							}
						}

						SelectPolygon(HoveredPolygons[indexOfSelected]);
						SelectCompleteAnimatedMeshPolygons();
					} else if (Gui.WidgetSelectionMode == WidgetSelectionMode.PolygonVertexTranslate) {
						if (SelectedPolygons.Count == 0) {
							SelectPolygon(HoveredPolygons[indexOfSelected]);
							SelectCompleteAnimatedMeshPolygons();
						} else {
							if (HoveredPolygons.First() != SelectedPolygons.First()) {
								SelectPolygon(HoveredPolygons[0]);
							}

							float closestPosition = 1000;
							Vertex closestVertex = null;

							foreach (Vertex vertex in SelectedPolygons[0].Vertices) {
								float distanceAway = Vector3.Distance(vertex.Position, _hoverPosition);
								if (distanceAway < closestPosition) {
									closestPosition = distanceAway;
									closestVertex = vertex;
								}
							}

							TransformWidget.SelectedVertexIndices.Clear();
							TransformWidget.SelectedVertexIndices.Add(
								SelectedPolygons[0].Vertices.IndexOf(closestVertex)
							);
						}
					} else if (Gui.WidgetSelectionMode == WidgetSelectionMode.PolygonEdgeTranslate) {
						if (SelectedPolygons.Count == 0) {
							SelectPolygon(HoveredPolygons[indexOfSelected]);
							SelectCompleteAnimatedMeshPolygons();
						} else {
							if (HoveredPolygons.First() != SelectedPolygons.First()) {
								SelectPolygon(HoveredPolygons[0]);
							}

							List<Vertex> vertices = SelectedPolygons[0].Vertices;
							List<Vertex> sortedVertices = vertices.OrderBy(
								vertex => Vector3.Distance(vertex.Position, _hoverPosition)
							).ToList();

							TransformWidget.SelectedVertexIndices.Clear();
							TransformWidget.SelectedVertexIndices.Add(vertices.IndexOf(sortedVertices[0]));
							TransformWidget.SelectedVertexIndices.Add(vertices.IndexOf(sortedVertices[1]));
						}
					}
				}
			}
		}

		private static void CollectHoveredPolygons() {
			foreach (Polygon polygon in CurrentMapState.StateData.PolygonCollectionBucket) {
				CameraRayResults triangleAIntersects = CameraRay.GetResults(
					new List<Vector3> {
						polygon.Vertices[0].AnimationAdjustedPosition,
						polygon.Vertices[1].AnimationAdjustedPosition,
						polygon.Vertices[2].AnimationAdjustedPosition
					}
				);

				CameraRayResults triangleBIntersects = polygon.IsQuad
					? CameraRay.GetResults(
						new List<Vector3> {
							polygon.Vertices[3].AnimationAdjustedPosition,
							polygon.Vertices[2].AnimationAdjustedPosition,
							polygon.Vertices[1].AnimationAdjustedPosition
						}
					)
					: new CameraRayResults {HasHit = false};

				if (triangleAIntersects.HasHit) {
					_hoverPosition = triangleAIntersects.HitPoint;
				}

				if (triangleBIntersects.HasHit) {
					_hoverPosition = triangleBIntersects.HitPoint;
				}

				if (triangleAIntersects.HasHit || triangleBIntersects.HasHit) {
					HoveredPolygons.Add(polygon);
				}
			}

			HoveredPolygons = HoveredPolygons.OrderBy(polygon => polygon.DistanceToCamera).ToList();
		}

		public static void SelectPolygon(Polygon polygon) {
			if (AppInput.ControlHeld) {
				if (SelectedPolygons.Contains(polygon)) {
					SelectedPolygons.Remove(polygon);
				} else {
					AddPolyToSelection(polygon);
				}
			} else if (AppInput.AltHeld) {
				if (SelectedPolygons.Contains(polygon)) {
					SelectedPolygons.Remove(polygon);
				}

				SelectedPolygons.Insert(0, polygon);
			} else {
				SelectedPolygons.Clear();
				SelectedPolygons.Add(polygon);
				if (AppInput.ShiftHeld) {
					StageCamera.CamTarget = polygon.AveragePoint;
				}
			}

			if (
				Gui.SelectedTab != RightPanelTab.Polygon &&
				Gui.SelectedTab != RightPanelTab.Texture
			) {
				Gui.SelectedTab = RightPanelTab.Polygon;
			}
		}

		public static void AddPolyToSelection(Polygon polygon) {
			if (!SelectedPolygons.Contains(polygon)) {
				SelectedPolygons.Add(polygon);
				if (AppInput.ShiftHeld) {
					StageCamera.CamTarget = polygon.AveragePoint;
				}
			}
		}

		private static void SelectCompleteAnimatedMeshPolygons() {
			List<MeshType> selectedMeshTypes = new List<MeshType>();

			foreach (Polygon polygon in SelectedPolygons) {
				if (polygon.MeshType != MeshType.PrimaryMesh && !selectedMeshTypes.Contains(polygon.MeshType)) {
					selectedMeshTypes.Add(polygon.MeshType);
				}
			}

			if (selectedMeshTypes.Count > 0) {
				if (Gui.WidgetSelectionMode == WidgetSelectionMode.PolygonVertexTranslate ||
				    Gui.WidgetSelectionMode == WidgetSelectionMode.PolygonEdgeTranslate
				) {
					Gui.WidgetSelectionMode = WidgetSelectionMode.PolygonTranslate;
				}
			}

			foreach (MeshType meshType in selectedMeshTypes) {
				foreach (PolygonType polygonType in CommonLists.PolygonTypes) {
					foreach (Polygon polygon in CurrentMapState.StateData.PolygonCollection[meshType][polygonType]) {
						if (!SelectedPolygons.Contains(polygon)) {
							AddPolyToSelection(polygon);
						}
					}
				}
			}
		}

		public static void SelectAllPolygons() {
			Polygon firstPolygon = null;
			if (SelectedPolygons.Count > 0) {
				firstPolygon = SelectedPolygons.First();
			}

			SelectedPolygons.Clear();
			SelectedPolygons.AddRange(CurrentMapState.StateData.PolygonCollectionBucket);

			if (firstPolygon != null) {
				SelectedPolygons.Remove(firstPolygon);
				SelectedPolygons.Insert(0, firstPolygon);
			}
		}

		public static void UnselectAllExceptLast() {
			while (SelectedPolygons.Count > 1) {
				SelectedPolygons.RemoveAt(0);
			}
		}

		public static void GrowPolygonSelection() {
			if (SelectedPolygons.Count == 0) {
				return;
			}

			List<Polygon> polygonsToAddToSelection = new List<Polygon>();

			foreach (Polygon otherPolygon in CurrentMapState.StateData.PolygonCollectionBucket) {
				if (!SelectedPolygons.Contains(otherPolygon)) {
					foreach (Polygon selectedPolygon in SelectedPolygons) {
						foreach (Vertex selectedVertex in selectedPolygon.Vertices) {
							foreach (Vertex otherVertex in otherPolygon.Vertices) {
								bool sharesVertices = (int) selectedVertex.Position.X == (int) otherVertex.Position.X &&
								                      (int) selectedVertex.Position.Y == (int) otherVertex.Position.Y &&
								                      (int) selectedVertex.Position.Z == (int) otherVertex.Position.Z;

								if (sharesVertices) {
									if (!polygonsToAddToSelection.Contains(otherPolygon)) {
										polygonsToAddToSelection.Add(otherPolygon);
									}
								}
							}
						}
					}
				}
			}

			SelectedPolygons.AddRange(polygonsToAddToSelection);
		}

		private static void HandleInputForTerrain() {
			HoveredPolygons.Clear();
			SelectedPolygons.Clear();
			HoveredTerrainTiles.Clear();

			if (AppInput.MouseIsWithinModelViewport) {
				CollectHoveredTerrainTiles();
				ClickTerrainSelection();
			}
		}

		private static void ClickTerrainSelection() {
			if (AppInput.LeftMouseClicked) {
				if (HoveredTerrainTiles.Count > 0) {
					SelectTerrainTile(HoveredTerrainTiles[0]);
				} else {
					SelectedTerrainTiles.Clear();
				}
			}
		}

		public static void SelectTerrainTile(TerrainTile terrainTile) {
			if (AppInput.ControlHeld) {
				if (SelectedTerrainTiles.Contains(terrainTile)) {
					SelectedTerrainTiles.Remove(terrainTile);
					TerrainTile companion = CurrentMapState.StateData.Terrain.GetCompanionTerrainTile(terrainTile);
					SelectedTerrainTiles.Remove(companion);
				} else {
					SelectedTerrainTiles.Add(terrainTile);
					TerrainTile companion = CurrentMapState.StateData.Terrain.GetCompanionTerrainTile(terrainTile);
					SelectedTerrainTiles.Add(companion);
				}
			} else {
				SelectedTerrainTiles.Clear();
				SelectedTerrainTiles.Add(terrainTile);
				TerrainTile companion = CurrentMapState.StateData.Terrain.GetCompanionTerrainTile(terrainTile);
				SelectedTerrainTiles.Add(companion);
			}

			if (AppInput.ShiftHeld) {
				StageCamera.CamTarget = terrainTile.AveragePoint;
			}
		}

		public static void SelectAllTerrainTiles() {
			SelectedTerrainTiles.Clear();
			foreach (List<TerrainTile> row in CurrentMapState.StateData.Terrain.Level0Tiles) {
				foreach (TerrainTile terrainTile in row) {
					SelectedTerrainTiles.Add(terrainTile);
					TerrainTile companion = CurrentMapState.StateData.Terrain.GetCompanionTerrainTile(terrainTile);
					SelectedTerrainTiles.Add(companion);
				}
			}
		}

		public static void GrowTerrainSelection() {
			if (SelectedTerrainTiles.Count == 0) {
				return;
			}

			Terrain terrain = CurrentMapState.StateData.Terrain;
			List<TerrainTile> tilesToAdd = new List<TerrainTile>();

			foreach (List<TerrainTile> row in terrain.Level0Tiles) {
				foreach (TerrainTile terrainTile in row) {
					if (SelectedTerrainTiles.Contains(terrainTile)) {
						continue;
					}

					if (terrainTile.IndexZ > 0) {
						TerrainTile targetTile = terrain.Level0Tiles[terrainTile.IndexZ - 1][terrainTile.IndexX];

						if (SelectedTerrainTiles.Contains(targetTile)) {
							tilesToAdd.Add(terrainTile);
							continue;
						}
					}

					if (terrainTile.IndexX > 0) {
						TerrainTile targetTile = terrain.Level0Tiles[terrainTile.IndexZ][terrainTile.IndexX - 1];

						if (SelectedTerrainTiles.Contains(targetTile)) {
							tilesToAdd.Add(terrainTile);
							continue;
						}
					}

					if (terrainTile.IndexZ < terrain.SizeZ - 1) {
						TerrainTile targetTile = terrain.Level0Tiles[terrainTile.IndexZ + 1][terrainTile.IndexX];

						if (SelectedTerrainTiles.Contains(targetTile)) {
							tilesToAdd.Add(terrainTile);
							continue;
						}
					}

					if (terrainTile.IndexX < terrain.SizeX - 1) {
						TerrainTile targetTile = terrain.Level0Tiles[terrainTile.IndexZ][terrainTile.IndexX + 1];

						if (SelectedTerrainTiles.Contains(targetTile)) {
							tilesToAdd.Add(terrainTile);
						}
					}
				}
			}

			foreach (TerrainTile terrainTile in tilesToAdd) {
				SelectedTerrainTiles.Add(terrainTile);
				TerrainTile companion = CurrentMapState.StateData.Terrain.GetCompanionTerrainTile(terrainTile);
				SelectedTerrainTiles.Add(companion);
			}
		}

		private static void CollectHoveredTerrainTiles() {
			List<List<TerrainTile>> allTiles = new List<List<TerrainTile>>();
			allTiles.AddRange(CurrentMapState.StateData.Terrain.Level0Tiles);
			allTiles.AddRange(CurrentMapState.StateData.Terrain.Level1Tiles);

			foreach (List<TerrainTile> terrainTiles in allTiles) {
				foreach (TerrainTile terrainTile in terrainTiles) {
					CameraRayResults triangleAIntersects = CameraRay.GetResults(
						new List<Vector3> {
							terrainTile.Vertices[0],
							terrainTile.Vertices[1],
							terrainTile.Vertices[2]
						}
					);

					CameraRayResults triangleBIntersects = CameraRay.GetResults(
						new List<Vector3> {
							terrainTile.Vertices[3],
							terrainTile.Vertices[0],
							terrainTile.Vertices[2]
						}
					);

					if (triangleAIntersects.HasHit) {
						_hoverPosition = triangleAIntersects.HitPoint;
					}

					if (triangleBIntersects.HasHit) {
						_hoverPosition = triangleBIntersects.HitPoint;
					}

					if (triangleAIntersects.HasHit || triangleBIntersects.HasHit) {
						HoveredTerrainTiles.Add(terrainTile);
					}
				}
			}

			HoveredTerrainTiles = HoveredTerrainTiles.OrderBy(terrainTile => terrainTile.DistanceToCamera).ToList();
		}

		private static void SetAnchorPositions() {
			if (SelectedPolygons.Count == 0) {
				return;
			}

			float minX = SelectedPolygons[0].Vertices[0].AnimationAdjustedPosition.X;
			float maxX = SelectedPolygons[0].Vertices[0].AnimationAdjustedPosition.X;
			float minY = SelectedPolygons[0].Vertices[0].AnimationAdjustedPosition.Y;
			float maxY = SelectedPolygons[0].Vertices[0].AnimationAdjustedPosition.Y;
			float minZ = SelectedPolygons[0].Vertices[0].AnimationAdjustedPosition.Z;
			float maxZ = SelectedPolygons[0].Vertices[0].AnimationAdjustedPosition.Z;

			foreach (Polygon polygon in SelectedPolygons) {
				foreach (Vertex vertex in polygon.Vertices) {
					if (vertex.AnimationAdjustedPosition.X < minX) {
						minX = vertex.AnimationAdjustedPosition.X;
					}

					if (vertex.AnimationAdjustedPosition.X > maxX) {
						maxX = vertex.AnimationAdjustedPosition.X;
					}

					if (vertex.AnimationAdjustedPosition.Y < minY) {
						minY = vertex.AnimationAdjustedPosition.Y;
					}

					if (vertex.AnimationAdjustedPosition.Y > maxY) {
						maxY = vertex.AnimationAdjustedPosition.Y;
					}

					if (vertex.AnimationAdjustedPosition.Z < minZ) {
						minZ = vertex.AnimationAdjustedPosition.Z;
					}

					if (vertex.AnimationAdjustedPosition.Z > maxZ) {
						maxZ = vertex.AnimationAdjustedPosition.Z;
					}
				}
			}

			MinOfSelection = new Vector3(minX, minY, minZ);
			MaxOfSelection = new Vector3(maxX, maxY, maxZ);
			CenterOfSelection = Utilities.GetAveragePoint(new List<Vector3> {MinOfSelection, MaxOfSelection});
		}
	}
}