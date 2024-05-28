using System;
using System.Collections.Generic;
using System.Linq;
using GaneshaDx.Common;
using GaneshaDx.Rendering;
using GaneshaDx.Resources.ContentDataTypes;
using GaneshaDx.Resources.ContentDataTypes.Polygons;
using GaneshaDx.Resources.ContentDataTypes.Terrains;
using GaneshaDx.Resources.ResourceContent;
using GaneshaDx.UserInterface;
using GaneshaDx.UserInterface.GuiForms;
using Microsoft.Xna.Framework;

namespace GaneshaDx.Resources;

public static class CurrentMapState {
	public static MapStateData StateData;
	private static readonly List<MapResource> InitialMeshMapResources = new();
	public static readonly List<MapResource> StateMeshMapResources = new();
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
		List<Polygon> newPolygons = new();

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

	public static void DeleteAllPolygons(MeshType meshType) {
		Dictionary<PolygonType, List<Polygon>> meshContainer = StateData.PolygonCollection[meshType];

		meshContainer[PolygonType.TexturedQuad].Clear();
		meshContainer[PolygonType.UntexturedQuad].Clear();
		meshContainer[PolygonType.TexturedTriangle].Clear();
		meshContainer[PolygonType.UntexturedTriangle].Clear();
	}

	public static Polygon CreatePolygon(List<Vertex> vertices, List<Vector2> uvs, MeshType selectedMeshType, bool addToBucket = true) {
		if (vertices.Count != 3 && vertices.Count != 4) {
			return null;
		}

		if (uvs.Count != 0 && uvs.Count != vertices.Count) {
			return null;
		}

		bool newPolygonIsQuad = vertices.Count == 4;
		bool newPolygonIsTextured = uvs.Count > 0;

		Polygon newPolygon = new() {
			Vertices = vertices,
			MeshType = selectedMeshType,
			RenderingProperties = new PolygonRenderingProperties(),
			UvCoordinates = uvs,
			PolygonType = newPolygonIsQuad
				? newPolygonIsTextured
					? PolygonType.TexturedQuad
					: PolygonType.UntexturedQuad
				: newPolygonIsTextured
					? PolygonType.TexturedTriangle
					: PolygonType.UntexturedTriangle,
			TerrainLevel = 0,
			TerrainX = 255,
			TerrainZ = 127
		};

		if (addToBucket) {
			Dictionary<PolygonType, List<Polygon>> selectedMesh = StateData.PolygonCollection[selectedMeshType];

			if (newPolygon.IsQuad) {
				if (newPolygon.IsTextured) {
					selectedMesh[PolygonType.TexturedQuad].Add(newPolygon);
				} else {
					selectedMesh[PolygonType.UntexturedQuad].Add(newPolygon);
				}
			} else {
				if (newPolygon.IsTextured) {
					selectedMesh[PolygonType.TexturedTriangle].Add(newPolygon);
				} else {
					selectedMesh[PolygonType.UntexturedTriangle].Add(newPolygon);
				}
			}

			Selection.SelectedPolygons.Clear();
			Selection.SelectedPolygons.Add(newPolygon);
			newPolygon.GuessNormals();
		}

		return newPolygon;
	}
}