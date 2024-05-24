using System;
using System.Collections.Generic;
using System.Linq;
using GaneshaDx.Common;
using GaneshaDx.Environment;
using GaneshaDx.Resources;
using GaneshaDx.Resources.ContentDataTypes;
using GaneshaDx.Resources.ContentDataTypes.MeshAnimations;
using GaneshaDx.Resources.ContentDataTypes.Palettes;
using GaneshaDx.Resources.ContentDataTypes.Polygons;
using GaneshaDx.Resources.ContentDataTypes.Terrains;
using GaneshaDx.Resources.ContentDataTypes.TextureAnimations;
using GaneshaDx.Resources.ResourceContent;
using GaneshaDx.UserInterface.GuiDefinitions;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Vector2 = System.Numerics.Vector2;

namespace GaneshaDx.UserInterface.GuiForms;

public static class GuiWindowManageResources {
	private static List<(string, int)> _columnHeaders;

	public static void Render() {
		_columnHeaders = new List<(string, int)> {
			("\n ", 50),
			("\nFile", 90),
			("\nMesh Type", 90),
			("\nArrange", 75),
			("\nTime", 50),
			("\nWeather", 75),
			("Primary\nMesh", 50),
			("\nPalettes", 50),
			("Grayscale\nPalette", 60),
			("Lights and\nBackground", 70),
			("Render\nProperties", 70),
			("\nTerrain", 50),
			("Texture\nAnimations", 70),
			("Palette\nAnimations", 70),
			("Animated\nMesh Frames", 80),
			("\nAnimated Meshes", 170)
		};

		bool windowIsOpen = true;

		GuiStyle.SetNewUiToDefaultStyle();
		ImGui.GetStyle().ItemSpacing = new Vector2(2, 2);
		ImGui.GetStyle().WindowRounding = 0;
		ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[2]);
		const ImGuiWindowFlags flags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.HorizontalScrollbar |
		                               ImGuiWindowFlags.NoResize |
		                               ImGuiWindowFlags.NoMove;

		ImGui.SetNextWindowSize(new Vector2(Stage.Width, 300));
		ImGui.SetNextWindowPos(new Vector2(0, Stage.Height - 300));

		ImGui.Begin("Manage Mesh Resources", ref windowIsOpen, flags);
		{
			ImGui.BeginChild("ScrollWindowMMR", new Vector2(1475, (MapData.MeshResources.Count + 1) * 30), false);
			ImGui.PopFont();
			ImGui.Columns(_columnHeaders.Count, "ManageResourcesGrid", false);

			for (int columnIndex = 0; columnIndex < _columnHeaders.Count; columnIndex++) {
				(string header, int columnWidth) = _columnHeaders[columnIndex];
				ImGui.SetColumnWidth(columnIndex, columnWidth + 10);

				ImGui.Text(header);
				ImGui.NextColumn();
			}

			ImGui.Separator();

			for (int index = 0; index < MapData.MeshResources.Count; index++) {
				BuildRow(index);
				ImGui.Separator();
			}

			ImGui.Columns(1);
			ImGui.EndChild();
		}
		ImGui.End();

		if (!windowIsOpen) {
			Gui.ToggleManageResourcesWindow();
		}
	}

	private static void BuildRow(int index) {
		MapResource mapResource = MapData.MeshResources[index];
		MeshResourceData meshResourceData = (MeshResourceData) mapResource.ResourceData;

		bool isInitialState = mapResource.MapArrangementState == MapArrangementState.Primary &&
		                      mapResource.MapTime == MapTime.Day &&
		                      mapResource.MapWeather == MapWeather.None;

		List<string> mapArrangementStates = Enum.GetNames(typeof(MapArrangementState)).ToList();
		List<string> mapTimeStates = Enum.GetNames(typeof(MapTime)).ToList();
		List<string> mapWeathers = Enum.GetNames(typeof(MapWeather)).ToList();

		bool stateSelected = CurrentMapState.StateData.MapArrangementState == mapResource.MapArrangementState &&
		                     CurrentMapState.StateData.MapTime == mapResource.MapTime &&
		                     CurrentMapState.StateData.MapWeather == mapResource.MapWeather;

		GuiStyle.SetNewUiToDefaultStyle();
		ImGui.GetStyle().ItemSpacing = new Vector2(2, 2);
		ImGui.GetStyle().FrameRounding = 0;

		if (stateSelected) {
			GuiStyle.SetElementStyle(ElementStyle.ButtonDisabled);
		}

		if (ImGui.Button("Select##SelectState_" + index) && !stateSelected) {
			CurrentMapState.SetState(
				mapResource.MapArrangementState,
				mapResource.MapTime,
				mapResource.MapWeather
			);
		}

		ImGui.NextColumn();

		ImGui.GetStyle().Colors[(int) ImGuiCol.Text] = stateSelected
			? GuiStyle.ColorPalette[ColorName.Highlighted]
			: GuiStyle.ColorPalette[ColorName.Lightest];

		ImGui.Text(MapData.MapName + "." + mapResource.XFile);
		ImGui.NextColumn();

		string meshType = mapResource.ResourceType switch {
			ResourceType.InitialMeshData => "Initial",
			ResourceType.OverrideMeshData => "Overridden",
			ResourceType.AlternateStateMehData => "Alternate",
			_ => "something else"
		};

		ImGui.Text(meshType);
		ImGui.NextColumn();

		ImGui.Text(mapArrangementStates[(int) mapResource.MapArrangementState]);
		ImGui.NextColumn();

		ImGui.Text(mapTimeStates[(int) mapResource.MapTime]);
		ImGui.NextColumn();

		ImGui.Text(mapWeathers[(int) mapResource.MapWeather]);
		ImGui.NextColumn();

		ImGui.GetStyle().Colors[(int) ImGuiCol.Text] = GuiStyle.ColorPalette[ColorName.Lightest];
		ImGui.GetStyle().Colors[(int) ImGuiCol.Button] = GuiStyle.ColorPalette[ColorName.Transparent];
		ImGui.GetStyle().FrameRounding = 0;

		BuildColumnPrimaryMesh(index, meshResourceData, isInitialState);
		ImGui.NextColumn();
		BuildColumnPalettes(index, meshResourceData, isInitialState);
		ImGui.NextColumn();
		BuildColumnGrayscalePalette(index, meshResourceData, isInitialState);
		ImGui.NextColumn();
		BuildColumnLightsAndBackground(index, meshResourceData, isInitialState);
		ImGui.NextColumn();
		BuildColumnRenderProperties(index, meshResourceData, isInitialState);
		ImGui.NextColumn();
		BuildColumnTerrain(index, meshResourceData, isInitialState);
		ImGui.NextColumn();
		BuildColumnTextureAnimations(index, meshResourceData);
		ImGui.NextColumn();
		BuildColumnPaletteAnimationFrames(index, meshResourceData);
		ImGui.NextColumn();
		BuildColumnHasAnimatedMeshInstructions(index, meshResourceData);
		ImGui.NextColumn();
		BuildColumnHasAnimatedMeshes(index, meshResourceData);
		ImGui.NextColumn();
	}

	private static void BuildColumnPrimaryMesh(int index, MeshResourceData data, bool isInitialState) {
		bool beforeHasPrimaryMesh = data.HasPrimaryMesh;

		GuiStyle.SetElementStyle(isInitialState ? ElementStyle.CheckboxDisabled : ElementStyle.CheckboxEnabled);

		ImGui.Checkbox("###hasPrimaryMesh" + index, ref data.HasPrimaryMesh);

		if (beforeHasPrimaryMesh != data.HasPrimaryMesh) {
			if (isInitialState) {
				data.HasPrimaryMesh = true;
			} else {
				data.SetUpPolyContainers();
				CurrentMapState.ResetState();
			}
		}

		GuiStyle.SetElementStyle(ElementStyle.CheckboxEnabled);
	}

	private static void BuildColumnPalettes(int index, MeshResourceData data, bool isInitialState) {
		bool beforeHasPalettes = data.HasPalettes;
		GuiStyle.SetElementStyle(isInitialState ? ElementStyle.CheckboxDisabled : ElementStyle.CheckboxEnabled);

		ImGui.Checkbox("###hasPalettes" + index, ref data.HasPalettes);

		if (beforeHasPalettes != data.HasPalettes) {
			if (isInitialState) {
				data.HasPalettes = true;
			} else {
				data.Palettes.Clear();

				if (data.HasPalettes) {
					const int totalPalettes = 16;
					const int totalColors = 16;

					for (int paletteIndex = 0; paletteIndex < totalPalettes; paletteIndex++) {
						Palette newPalette = new Palette();

						for (int colorIndex = 0; colorIndex < totalColors; colorIndex++) {
							int color = colorIndex * 2;
							PaletteColor newColor = new(color, color, color, false);
							newPalette.Colors.Add(newColor);
						}

						data.Palettes.Add(newPalette);
					}
				}

				CurrentMapState.ResetState();
			}
		}

		GuiStyle.SetElementStyle(ElementStyle.CheckboxEnabled);
	}

	private static void BuildColumnGrayscalePalette(int index, MeshResourceData data, bool isInitialState) {
		bool beforeHasPalettes = data.HasGrayscalePalettes;
		GuiStyle.SetElementStyle(isInitialState ? ElementStyle.CheckboxDisabled : ElementStyle.CheckboxEnabled);

		ImGui.Checkbox("###hasGrayscalePalettes" + index, ref data.HasGrayscalePalettes);

		if (beforeHasPalettes != data.HasGrayscalePalettes) {
			if (isInitialState) {
				data.HasGrayscalePalettes = true;
			} else {
				CurrentMapState.ResetState();
			}
		}

		GuiStyle.SetElementStyle(ElementStyle.CheckboxEnabled);
	}

	private static void BuildColumnLightsAndBackground(int index, MeshResourceData data, bool isInitialState) {
		bool beforeHasLightsBackground = data.HasLightsAndBackground;

		GuiStyle.SetElementStyle(isInitialState ? ElementStyle.CheckboxDisabled : ElementStyle.CheckboxEnabled);

		ImGui.Checkbox("###hasLighting" + index, ref data.HasLightsAndBackground);

		if (beforeHasLightsBackground != data.HasLightsAndBackground) {
			if (isInitialState) {
				data.HasLightsAndBackground = true;
			}

			if (data.HasLightsAndBackground) {
				data.BackgroundTopColor = Color.White;
				data.BackgroundBottomColor = Color.Black;
				data.AmbientLightColor = Color.Gray;
				data.DirectionalLights = new List<DirectionalLight> {
					new() { LightColor = Color.Gray, DirectionElevation = 45, DirectionAzimuth = 0 },
					new() { LightColor = Color.Gray, DirectionElevation = 45, DirectionAzimuth = 120 },
					new() { LightColor = Color.Gray, DirectionElevation = 45, DirectionAzimuth = 240 }
				};
			}

			CurrentMapState.ResetState();
		}

		GuiStyle.SetElementStyle(ElementStyle.CheckboxEnabled);
	}

	private static void BuildColumnRenderProperties(int index, MeshResourceData data, bool isInitialState) {
		bool beforeHasPalettes = data.HasPolygonRenderProperties;

		ImGui.Checkbox("###hasRenderProperties" + index, ref data.HasPolygonRenderProperties);

		if (beforeHasPalettes != data.HasPolygonRenderProperties) {
			foreach (KeyValuePair<MeshType, Dictionary<PolygonType, List<Polygon>>> polygonBuckets in data.PolygonCollection) {
				foreach (KeyValuePair<PolygonType, List<Polygon>> polygons in polygonBuckets.Value) {
					foreach (Polygon polygon in polygons.Value) {
						polygon.RenderingProperties = data.HasPolygonRenderProperties
							? new PolygonRenderingProperties()
							: null;
					}
				}
			}

			data.UnknownRenderPropertiesData = data.HasPolygonRenderProperties
				? new List<byte>()
				: null;

			if (data.HasPolygonRenderProperties && data.UnknownRenderPropertiesData != null) {
				for (int unknownPropertiesIndex = 0; unknownPropertiesIndex < 4096; unknownPropertiesIndex++) {
					data.UnknownRenderPropertiesData.Add(0);
				}
			}

			if (!isInitialState) {
				CurrentMapState.ResetState();
			}
		}
	}

	private static void BuildColumnTerrain(int index, MeshResourceData data, bool isInitialState) {
		bool beforeHasTerrain = data.HasTerrain;

		GuiStyle.SetElementStyle(isInitialState ? ElementStyle.CheckboxDisabled : ElementStyle.CheckboxEnabled);

		ImGui.Checkbox("###hasTerrain" + index, ref data.HasTerrain);

		if (beforeHasTerrain != data.HasTerrain) {
			if (isInitialState) {
				data.HasTerrain = true;
			} else {
				if (data.HasTerrain) {
					List<List<TerrainTile>> level0Tiles = new() { new List<TerrainTile>() };
					level0Tiles[0].Add(new TerrainTile { Level = 0 });
					List<List<TerrainTile>> level1Tiles = new() { new List<TerrainTile>() };
					level1Tiles[0].Add(new TerrainTile { Level = 1 });

					data.Terrain = new Terrain {
						Level0Tiles = level0Tiles,
						Level1Tiles = level1Tiles,
						SizeX = 1,
						SizeZ = 1
					};
				}

				CurrentMapState.ResetState();
			}
		}

		GuiStyle.SetElementStyle(ElementStyle.CheckboxEnabled);
	}

	private static void BuildColumnTextureAnimations(int index, MeshResourceData data) {
		bool beforeHasTerrain = data.HasTextureAnimations;
		ImGui.Checkbox("###hasTextureAnimation" + index, ref data.HasTextureAnimations);

		if (beforeHasTerrain != data.HasTextureAnimations) {
			if (data.HasTextureAnimations) {
				const int totalAnimations = 32;
				data.AnimatedTextureInstructions = new List<AnimatedTextureInstructions>();
				for (int animationIndex = 0; animationIndex < totalAnimations; animationIndex++) {
					data.AnimatedTextureInstructions.Add(new AnimatedTextureInstructions());
				}
			} else {
				data.AnimatedTextureInstructions = null;
				data.HasPaletteAnimationFrames = false;
				data.PaletteAnimationFrames.Clear();
			}

			CurrentMapState.ResetState();
		}
	}

	private static void BuildColumnPaletteAnimationFrames(int index, MeshResourceData data) {
		bool beforeHasAnimatedFrame = data.HasPaletteAnimationFrames;
		ImGui.Checkbox("###hasAnimatedPalettes" + index, ref data.HasPaletteAnimationFrames);

		if (beforeHasAnimatedFrame != data.HasPaletteAnimationFrames) {
			if (data.HasPaletteAnimationFrames) {
				const int totalPalettes = 16;
				const int totalColors = 16;

				Palette sourcePalette = new();

				for (int colorIndex = 0; colorIndex < totalColors; colorIndex++) {
					int color = colorIndex * 17 / 8;
					PaletteColor newColor = new(color, color, color, false);
					sourcePalette.Colors.Add(newColor);
				}

				for (int paletteIndex = 0; paletteIndex < totalPalettes; paletteIndex++) {
					Palette newPalette = new();

					for (int sourceIndex = 0; sourceIndex < sourcePalette.Colors.Count; sourceIndex++) {
						int offsetIndex = sourceIndex + paletteIndex;
						if (offsetIndex > totalColors - 1) {
							offsetIndex -= totalColors;
						}

						PaletteColor paletteColor = sourcePalette.Colors[offsetIndex];
						newPalette.Colors.Add(new PaletteColor(
							paletteColor.Red,
							paletteColor.Green,
							paletteColor.Red,
							false
						));
					}

					data.PaletteAnimationFrames.Add(newPalette);
				}

				if (!data.HasTextureAnimations) {
					data.HasTextureAnimations = true;
					const int totalAnimations = 32;
					data.AnimatedTextureInstructions = new List<AnimatedTextureInstructions>();
					for (int animationIndex = 0; animationIndex < totalAnimations; animationIndex++) {
						data.AnimatedTextureInstructions.Add(new AnimatedTextureInstructions());
					}
				}
			} else {
				data.PaletteAnimationFrames.Clear();

				if (data.AnimatedTextureInstructions != null) {
					foreach (AnimatedTextureInstructions instructions in data.AnimatedTextureInstructions) {
						if (instructions.TextureAnimationType == TextureAnimationType.PaletteAnimation) {
							instructions.TextureAnimationType = TextureAnimationType.None;
						}
					}
				}
			}

			CurrentMapState.ResetState();
		}
	}

	private static void BuildColumnHasAnimatedMeshInstructions(int index, MeshResourceData data) {
		bool beforeAnimatedMeshes = data.HasAnimatedMeshInstructions;
		ImGui.Checkbox("###hasAnimatedMeshes" + index, ref data.HasAnimatedMeshInstructions);

		if (beforeAnimatedMeshes != data.HasAnimatedMeshInstructions) {
			data.MeshAnimationSet = data.HasAnimatedMeshInstructions
				? new MeshAnimationSet()
				: null;

			CurrentMapState.ResetState();
		}
	}

	private static void BuildColumnHasAnimatedMeshes(int index, MeshResourceData data) {
		bool beforeAnimatedMesh1 = data.HasAnimatedMesh1;
		bool beforeAnimatedMesh2 = data.HasAnimatedMesh2;
		bool beforeAnimatedMesh3 = data.HasAnimatedMesh3;
		bool beforeAnimatedMesh4 = data.HasAnimatedMesh4;
		bool beforeAnimatedMesh5 = data.HasAnimatedMesh5;
		bool beforeAnimatedMesh6 = data.HasAnimatedMesh6;
		bool beforeAnimatedMesh7 = data.HasAnimatedMesh7;
		bool beforeAnimatedMesh8 = data.HasAnimatedMesh8;

		ImGui.Checkbox("###hasAnimatedMesh1_" + index, ref data.HasAnimatedMesh1);
		ImGui.SameLine();
		ImGui.Checkbox("###hasAnimatedMesh2_" + index, ref data.HasAnimatedMesh2);
		ImGui.SameLine();
		ImGui.Checkbox("###hasAnimatedMesh3_" + index, ref data.HasAnimatedMesh3);
		ImGui.SameLine();
		ImGui.Checkbox("###hasAnimatedMesh4_" + index, ref data.HasAnimatedMesh4);
		ImGui.SameLine();
		ImGui.Checkbox("###hasAnimatedMesh5_" + index, ref data.HasAnimatedMesh5);
		ImGui.SameLine();
		ImGui.Checkbox("###hasAnimatedMesh6_" + index, ref data.HasAnimatedMesh6);
		ImGui.SameLine();
		ImGui.Checkbox("###hasAnimatedMesh7_" + index, ref data.HasAnimatedMesh7);
		ImGui.SameLine();
		ImGui.Checkbox("###hasAnimatedMesh8_" + index, ref data.HasAnimatedMesh8);

		if (beforeAnimatedMesh1 != data.HasAnimatedMesh1 ||
		    beforeAnimatedMesh2 != data.HasAnimatedMesh2 ||
		    beforeAnimatedMesh3 != data.HasAnimatedMesh3 ||
		    beforeAnimatedMesh4 != data.HasAnimatedMesh4 ||
		    beforeAnimatedMesh5 != data.HasAnimatedMesh5 ||
		    beforeAnimatedMesh6 != data.HasAnimatedMesh6 ||
		    beforeAnimatedMesh7 != data.HasAnimatedMesh7 ||
		    beforeAnimatedMesh8 != data.HasAnimatedMesh8
		   ) {
			CurrentMapState.ResetState();
		}
	}
}