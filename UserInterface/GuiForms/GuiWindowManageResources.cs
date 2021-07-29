using System;
using System.Collections.Generic;
using System.Linq;
using GaneshaDx.Common;
using GaneshaDx.Resources;
using GaneshaDx.Resources.ContentDataTypes;
using GaneshaDx.Resources.ContentDataTypes.Palettes;
using GaneshaDx.Resources.ContentDataTypes.Terrains;
using GaneshaDx.Resources.ContentDataTypes.TextureAnimations;
using GaneshaDx.Resources.ResourceContent;
using GaneshaDx.UserInterface.GuiDefinitions;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Vector2 = System.Numerics.Vector2;

namespace GaneshaDx.UserInterface.GuiForms {
	public static class GuiWindowManageResources {
		private static readonly List<(string, int)> Columns = new List<(string, int)> {
			("\nFile", 90),
			("\nMesh Type", 90),
			("\nArrange", 75),
			("\nTime", 50),
			("\nWeather", 75),
			("Primary\nMesh", 100),
			("\nPalettes", 100),
			("Lights and\nBackground", 100),
			("\nTerrain", 100),
			("Texture\nAnimations", 100),
			("Palette\nAnimations", 100),
			("Animated\nMesh Frames", 100),
			("\nAnimated Meshes", 170),
			("", 100)
		};

		public static void Render() {
			bool windowIsOpen = true;

			GuiStyle.SetNewUiToDefaultStyle();
			ImGui.GetStyle().WindowRounding = 3;
			ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[2]);
			const ImGuiWindowFlags flags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.HorizontalScrollbar;

			ImGui.Begin("Manage Mesh Resources", ref windowIsOpen, flags);
			{
				ImGui.BeginChild("ScrollWindowMMR", new Vector2(1475, (MapData.MeshResources.Count + 1) * 30), false);
				ImGui.PopFont();
				ImGui.Columns(Columns.Count, "ManageResourcesGrid", false);

				for (int columnIndex = 0; columnIndex < Columns.Count; columnIndex++) {
					(string header, int columnWidth) = Columns[columnIndex];
					ImGui.SetColumnWidth(columnIndex, columnWidth + 10);

					ImGui.Text(header);
					ImGui.NextColumn();
				}

				ImGui.Separator();

				for (int index = 0; index < MapData.MeshResources.Count; index++) {
					BuildRow(index);
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
			BuildColumnLightsAndBackground(index, meshResourceData, isInitialState);
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

			ImGui.GetStyle().ItemSpacing = new Vector2(8, 4);
			GuiStyle.SetNewUiToDefaultStyle();
			ImGui.GetStyle().FrameRounding = 0;

			if (stateSelected) {
				GuiStyle.SetElementStyle(ElementStyle.ButtonDisabled);
			}

			if (ImGui.Button("Select State##SelectState_" + index) && !stateSelected) {
				CurrentMapState.SetState(
					mapResource.MapArrangementState,
					mapResource.MapTime,
					mapResource.MapWeather
				);
			}

			ImGui.NextColumn();
		}

		private static void BuildColumnPrimaryMesh(int index, MeshResourceData data, bool isInitialState) {
			ImGui.GetStyle().ItemSpacing = new Vector2(1, 0);
			bool beforeHasPrimaryMesh = data.HasPrimaryMesh;
			ImGui.Checkbox("###hasPrimaryMesh" + index, ref data.HasPrimaryMesh);
			if (data.HasPrimaryMesh) {
				ImGui.SameLine();
				ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[3]);
				ImGui.Button("C###copyPrimaryMesh" + index);
				ImGui.SameLine();
				ImGui.Button("V###copyPrimaryMesh" + index);
				ImGui.PopFont();
			}

			if (beforeHasPrimaryMesh != data.HasPrimaryMesh) {
				if (isInitialState) {
					data.HasPrimaryMesh = true;
				} else {
					data.SetUpPolyContainers();
				}

				CurrentMapState.ResetState();
			}
		}

		private static void BuildColumnPalettes(int index, MeshResourceData data, bool isInitialState) {
			ImGui.GetStyle().ItemSpacing = new Vector2(1, 0);
			bool beforeHasPalettes = data.HasPalettes;
			ImGui.Checkbox("###hasPalettes" + index, ref data.HasPalettes);
			if (data.HasPalettes) {
				ImGui.SameLine();
				ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[3]);
				ImGui.Button("C###copyPalettes" + index);
				ImGui.SameLine();
				ImGui.Button("V###copyPalettes" + index);
				ImGui.PopFont();
			}

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
								PaletteColor newColor = new PaletteColor(color, color, color, false);
								newPalette.Colors.Add(newColor);
							}

							data.Palettes.Add(newPalette);
						}
					}
				}

				CurrentMapState.ResetState();
			}
		}

		private static void BuildColumnLightsAndBackground(int index, MeshResourceData data, bool isInitialState) {
			ImGui.GetStyle().ItemSpacing = new Vector2(1, 0);
			bool beforeHasLightsBackground = data.HasLightsAndBackground;
			ImGui.Checkbox("###hasLighting" + index, ref data.HasLightsAndBackground);
			if (data.HasLightsAndBackground) {
				ImGui.SameLine();
				ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[3]);
				ImGui.Button("C###copyLighting" + index);
				ImGui.SameLine();
				ImGui.Button("V###copyLighting" + index);
				ImGui.PopFont();
			}

			if (beforeHasLightsBackground != data.HasLightsAndBackground) {
				if (isInitialState) {
					data.HasLightsAndBackground = true;
				}

				if (data.HasLightsAndBackground) {
					data.BackgroundTopColor = Color.White;
					data.BackgroundBottomColor = Color.Black;
					data.AmbientLightColor = Color.Gray;
					data.DirectionalLights = new List<DirectionalLight> {
						new DirectionalLight {LightColor = Color.Gray, DirectionElevation = 45, DirectionAzimuth = 0},
						new DirectionalLight {LightColor = Color.Gray, DirectionElevation = 45, DirectionAzimuth = 120},
						new DirectionalLight {LightColor = Color.Gray, DirectionElevation = 45, DirectionAzimuth = 240}
					};
				}

				CurrentMapState.ResetState();
			}
		}

		private static void BuildColumnTerrain(int index, MeshResourceData data, bool isInitialState) {
			ImGui.GetStyle().ItemSpacing = new Vector2(1, 0);
			bool beforeHasTerrain = data.HasTerrain;
			ImGui.Checkbox("###hasTerrain" + index, ref data.HasTerrain);
			if (data.HasLightsAndBackground) {
				ImGui.SameLine();
				ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[3]);
				ImGui.Button("C###copyTerrain" + index);
				ImGui.SameLine();
				ImGui.Button("V###copyTerrain" + index);
				ImGui.PopFont();
			}

			if (beforeHasTerrain != data.HasTerrain) {
				if (isInitialState) {
					data.HasTerrain = true;
				} else {
					if (data.HasTerrain) {
						List<List<TerrainTile>> level0Tiles = new List<List<TerrainTile>> {new List<TerrainTile>()};
						level0Tiles[0].Add(new TerrainTile {Level = 0});
						List<List<TerrainTile>> level1Tiles = new List<List<TerrainTile>> {new List<TerrainTile>()};
						level1Tiles[0].Add(new TerrainTile {Level = 1});

						data.Terrain = new Terrain {
							Level0Tiles = level0Tiles,
							Level1Tiles = level1Tiles,
							SizeX = 1,
							SizeZ = 1
						};
					}
				}

				CurrentMapState.ResetState();
			}
		}

		private static void BuildColumnTextureAnimations(int index, MeshResourceData data) {
			ImGui.GetStyle().ItemSpacing = new Vector2(1, 0);
			bool beforeHasTerrain = data.HasTextureAnimations;
			ImGui.Checkbox("###hasTextureAnimation" + index, ref data.HasTextureAnimations);
			if (data.HasTextureAnimations) {
				ImGui.SameLine();
				ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[3]);
				ImGui.Button("C###copyTextureAnimation" + index);
				ImGui.SameLine();
				ImGui.Button("V###copyTextureAnimation" + index);
				ImGui.PopFont();
			}

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
			ImGui.GetStyle().ItemSpacing = new Vector2(1, 0);
			bool beforeHasAnimatedFrame = data.HasPaletteAnimationFrames;
			ImGui.Checkbox("###hasAnimatedPalettes" + index, ref data.HasPaletteAnimationFrames);
			if (data.HasPaletteAnimationFrames) {
				ImGui.SameLine();
				ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[3]);
				ImGui.Button("C###copyAnimatedPalettes" + index);
				ImGui.SameLine();
				ImGui.Button("V###copyAnimatedPalettes" + index);
				ImGui.PopFont();
			}

			if (beforeHasAnimatedFrame != data.HasPaletteAnimationFrames) {
				if (data.HasPaletteAnimationFrames) {
					const int totalPalettes = 16;
					const int totalColors = 16;

					Palette sourcePalette = new Palette();

					for (int colorIndex = 0; colorIndex < totalColors; colorIndex++) {
						int color = colorIndex * 17 / 8;
						PaletteColor newColor = new PaletteColor(color, color, color, false);
						sourcePalette.Colors.Add(newColor);
					}

					for (int paletteIndex = 0; paletteIndex < totalPalettes; paletteIndex++) {
						Palette newPalette = new Palette();

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
			ImGui.GetStyle().ItemSpacing = new Vector2(1, 0);
			bool beforeAnimatedMeshes = data.HasAnimatedMeshInstructions;
			ImGui.Checkbox("###hasAnimatedMeshes" + index, ref data.HasAnimatedMeshInstructions);
			if (data.HasAnimatedMeshInstructions) {
				ImGui.SameLine();
				ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[3]);
				ImGui.Button("C###copyAnimatedMeshes" + index);
				ImGui.SameLine();
				ImGui.Button("V###copyAnimatedMeshes" + index);
				ImGui.PopFont();
			}

			if (beforeAnimatedMeshes != data.HasAnimatedMeshInstructions) {
				data.HasAnimatedMeshInstructions = beforeAnimatedMeshes;
				CurrentMapState.ResetState();
			}
		}

		private static void BuildColumnHasAnimatedMeshes(int index, MeshResourceData data) {
			ImGui.GetStyle().ItemSpacing = new Vector2(1, 0);
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
}