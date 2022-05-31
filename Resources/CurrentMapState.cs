using System;
using System.Collections.Generic;
using GaneshaDx.Common;
using GaneshaDx.Rendering;
using GaneshaDx.Resources.ContentDataTypes;
using GaneshaDx.Resources.ContentDataTypes.Polygons;
using GaneshaDx.Resources.ResourceContent;
using GaneshaDx.UserInterface;
using GaneshaDx.UserInterface.GuiForms;

namespace GaneshaDx.Resources {
	public static class CurrentMapState {
		public static MapStateData StateData;
		private static readonly List<MapResource> InitialMeshMapResources = new List<MapResource>();
		private static readonly List<MapResource> StateMeshMapResources = new List<MapResource>();
		private static MapResource _initialTextureMapResource;
		private static MapResource _stateTextureMapResource;

		public static void SetState(MapArrangementState mapArrangementState, MapTime mapTime, MapWeather mapWeather) {
			StateData = new MapStateData {
				MapArrangementState = mapArrangementState,
				MapTime = mapTime,
				MapWeather = mapWeather
			};

			GuiPanelTexture.CurrentPanelMode = GuiPanelTexture.PanelMode.UVs;
			SetInitialMeshData();
			SetInitialTextureData();
			AssignStateResources();
			SetCurrentMapStateReferences();
			SceneRenderer.Reset();

			Background.SetAsGradient(StateData.BackgroundTopColor, StateData.BackgroundBottomColor);
		}

		public static void ResetState() {
			SetState(StateData.MapArrangementState, StateData.MapTime, StateData.MapWeather);
		}

		private static void SetInitialMeshData() {
			InitialMeshMapResources.Clear();

			foreach (MapResource resource in MapData.MeshResources) {
				bool resourceIsDefault = resource.MapArrangementState == MapArrangementState.Primary &&
				                         resource.MapTime == MapTime.Day &&
				                         resource.MapWeather == MapWeather.None;

				if (resourceIsDefault) {
					InitialMeshMapResources.Add(resource);
				}
			}
		}

		private static void SetInitialTextureData() {
			_initialTextureMapResource = null;

			foreach (MapResource resource in MapData.TextureResources) {
				bool resourceIsDefault = resource.MapArrangementState == MapArrangementState.Primary &&
				                         resource.MapTime == MapTime.Day &&
				                         resource.MapWeather == MapWeather.None;

				if (resourceIsDefault) {
					_initialTextureMapResource = resource;
					break;
				}
			}
		}

		private static void AssignStateResources() {
			StateMeshMapResources.Clear();
			_stateTextureMapResource = null;

			foreach (MapResource resource in MapData.MeshResources) {
				bool resourceMatchesState = resource.MapArrangementState == StateData.MapArrangementState &&
				                            resource.MapTime == StateData.MapTime &&
				                            resource.MapWeather == StateData.MapWeather;

				if (resourceMatchesState) {
					StateMeshMapResources.Add(resource);
				}
			}

			foreach (MapResource resource in MapData.TextureResources) {
				bool resourceMatchesState = resource.MapArrangementState == StateData.MapArrangementState &&
				                            resource.MapTime == StateData.MapTime &&
				                            resource.MapWeather == StateData.MapWeather;

				if (resourceMatchesState) {
					_stateTextureMapResource = resource;
					break;
				}
			}

			if (StateMeshMapResources.Count == 0) {
				OverlayConsole.AddMessage("THIS STATE DOESNT EXIST");
			}
		}

		private static void SetCurrentMapStateReferences() {
			MapResource stateTextureResource = _stateTextureMapResource ?? _initialTextureMapResource;

			StateData.SetStateResources(
				InitialMeshMapResources,
				StateMeshMapResources,
				stateTextureResource
			);
		}

		public static void CloneSelection() {
			List<Polygon> newPolygons = new List<Polygon>();

			foreach (Polygon polygon in Selection.SelectedPolygons) {
				Polygon newPolygon = polygon.CreateClone();

				Dictionary<PolygonType, List<Polygon>> meshBucket = StateData.PolygonCollection[polygon.MeshType];

				if (newPolygon.IsQuad) {
					if (newPolygon.IsTextured) {
						meshBucket[PolygonType.TexturedQuad].Add(newPolygon);
					} else {
						meshBucket[PolygonType.UntexturedQuad].Add(newPolygon);
					}
				} else {
					if (newPolygon.IsTextured) {
						meshBucket[PolygonType.TexturedTriangle].Add(newPolygon);
					} else {
						meshBucket[PolygonType.UntexturedTriangle].Add(newPolygon);
					}
				}

				newPolygons.Add(newPolygon);
			}

			Selection.SelectedPolygons.Clear();

			foreach (Polygon polygon in newPolygons) {
				Selection.SelectedPolygons.Add(polygon);
			}
		}

		public static void DeleteSelection() {
			foreach (Polygon polygon in Selection.SelectedPolygons) {
				Dictionary<PolygonType, List<Polygon>> meshContainer = StateData.PolygonCollection[polygon.MeshType];
				if (polygon.IsQuad) {
					if (polygon.IsTextured) {
						meshContainer[PolygonType.TexturedQuad].Remove(polygon);
					} else {
						meshContainer[PolygonType.UntexturedQuad].Remove(polygon);
					}
				} else {
					if (polygon.IsTextured) {
						meshContainer[PolygonType.TexturedTriangle].Remove(polygon);
					} else {
						meshContainer[PolygonType.UntexturedTriangle].Remove(polygon);
					}
				}
			}

			Selection.SelectedPolygons.Clear();
		}

		public static void SelectOverlappingPolygons() {
			Selection.SelectedPolygons.Clear();


			List<PolygonType> polygonTypes = new List<PolygonType> {
				PolygonType.TexturedQuad,
				PolygonType.UntexturedQuad,
				PolygonType.TexturedTriangle,
				PolygonType.UntexturedTriangle
			};

			foreach (PolygonType polygonType in polygonTypes) {
				List<Polygon> currentBucket = StateData.PolygonCollection[GuiPanelMeshSelector.SelectedMesh][polygonType];

				foreach (Polygon polygon in currentBucket) {
					if (!Selection.SelectedPolygons.Contains(polygon)) {
						foreach (Polygon otherPolygon in currentBucket) {
							if (
								polygon != otherPolygon &&
								!Selection.SelectedPolygons.Contains(otherPolygon) &&
								Math.Abs(polygon.Vertices[0].Position.X - otherPolygon.Vertices[0].Position.X) < .1 &&
								Math.Abs(polygon.Vertices[0].Position.Y - otherPolygon.Vertices[0].Position.Y) < .1 &&
								Math.Abs(polygon.Vertices[0].Position.Z - otherPolygon.Vertices[0].Position.Z) < .1 &&
								Math.Abs(polygon.Vertices[1].Position.X - otherPolygon.Vertices[1].Position.X) < .1 &&
								Math.Abs(polygon.Vertices[1].Position.Y - otherPolygon.Vertices[1].Position.Y) < .1 &&
								Math.Abs(polygon.Vertices[1].Position.Z - otherPolygon.Vertices[1].Position.Z) < .1 &&
								Math.Abs(polygon.Vertices[2].Position.X - otherPolygon.Vertices[2].Position.X) < .1 &&
								Math.Abs(polygon.Vertices[2].Position.Y - otherPolygon.Vertices[2].Position.Y) < .1 &&
								Math.Abs(polygon.Vertices[2].Position.Z - otherPolygon.Vertices[2].Position.Z) < .1 &&
								polygon.Vertices.Count > 3 &&
								Math.Abs(polygon.Vertices[3].Position.X - otherPolygon.Vertices[3].Position.X) < .1 &&
								Math.Abs(polygon.Vertices[3].Position.Y - otherPolygon.Vertices[3].Position.Y) < .1 &&
								Math.Abs(polygon.Vertices[3].Position.Z - otherPolygon.Vertices[3].Position.Z) < .1
							) {
								Selection.AddPolyToSelection(polygon);
							}
						}
					}
				}
			}
		}
	}
}