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
			Dictionary<PolygonType, List<Polygon>> primaryMesh = StateData.PolygonCollection[MeshType.PrimaryMesh];

			foreach (Polygon polygon in Selection.SelectedPolygons) {
				if (polygon.IsQuad) {
					if (polygon.IsTextured) {
						primaryMesh[PolygonType.TexturedQuad].Remove(polygon);
					} else {
						primaryMesh[PolygonType.UntexturedQuad].Remove(polygon);
					}
				} else {
					if (polygon.IsTextured) {
						primaryMesh[PolygonType.TexturedTriangle].Remove(polygon);
					} else {
						primaryMesh[PolygonType.UntexturedTriangle].Remove(polygon);
					}
				}
			}

			Selection.SelectedPolygons.Clear();
		}
	}
}