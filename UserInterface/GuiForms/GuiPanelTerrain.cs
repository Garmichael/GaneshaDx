using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GaneshaDx.Common;
using GaneshaDx.Environment;
using GaneshaDx.Resources;
using GaneshaDx.Resources.ContentDataTypes.Terrains;
using GaneshaDx.UserInterface.GuiDefinitions;
using ImGuiNET;
using Vector2 = System.Numerics.Vector2;

namespace GaneshaDx.UserInterface.GuiForms;

public static class GuiPanelTerrain {
	public static bool ResizeTerrainMode;
	private static int _resizeXSize;
	private static int _resizeZSize;
	private static bool _resizeFromFront;
	private static bool _writeGreyboxTexture = true;
	private static bool _writeGreyboxPalettes = true;
	private static bool _greyboxSinglePolyWalls;

	public static void Render() {
		if (Selection.SelectedTerrainTiles.Count > 0 && !ResizeTerrainMode) {
			RenderTerrainTileProperties();
		}

		RenderResizeTerrainPanel();

		if (!ResizeTerrainMode) {
			RenderGreyboxingForm();
		}

		RenderTerrainRenderOptions();
	}

	private static void RenderTerrainTileProperties() {
		TerrainTile terrainTile = Selection.SelectedTerrainTiles[0];
		TerrainTile companionTile = CurrentMapState.StateData.Terrain.GetCompanionTerrainTile(terrainTile);

		List<TerrainTile> tiles = new();
		if (terrainTile.Level == 0) {
			tiles.Add(terrainTile);
			tiles.Add(companionTile);
		} else {
			tiles.Add(companionTile);
			tiles.Add(terrainTile);
		}

		List<string> slopeTypes = Enum.GetNames(typeof(TerrainSlopeType)).ToList();
		for (int slopeTypeIndex = 0; slopeTypeIndex < slopeTypes.Count; slopeTypeIndex++) {
			slopeTypes[slopeTypeIndex] = Regex.Replace(slopeTypes[slopeTypeIndex], "(\\B[A-Z])", " $1");
		}

		List<string> surfaceTypes = Enum.GetNames(typeof(TerrainSurfaceType)).ToList();
		for (int surfaceTypeIndex = 0; surfaceTypeIndex < surfaceTypes.Count; surfaceTypeIndex++) {
			surfaceTypes[surfaceTypeIndex] =
				Regex.Replace(surfaceTypes[surfaceTypeIndex], "(\\B[A-Z])", " $1");
		}

		GuiStyle.SetElementStyle(ElementStyle.Header);
		ImGui.PushStyleColor(ImGuiCol.Text, GuiStyle.ColorPalette[ColorName.Lightest]);

		GuiStyle.AddSpace(5);
		string positionText = terrainTile.IndexX + "x     " + terrainTile.IndexZ + "z";
		float positionTextWidth = ImGui.CalcTextSize(positionText).X;
		ImGui.SetCursorPosX((GuiStyle.RightPanelWidth - positionTextWidth) * 0.5f);
		ImGui.Text(positionText);
		GuiStyle.AddSpace(5);

		GuiStyle.SetNewUiToDefaultStyle();

		if (GuiStyle.CenteredButton("Reset Selected Tiles")) {
			foreach (TerrainTile resettingTile in Selection.SelectedTerrainTiles) {
				resettingTile.Height = 0;
				resettingTile.Depth = 0;
				resettingTile.Shading = 0;
				resettingTile.SlopeHeight = 0;
				resettingTile.SlopeType = TerrainSlopeType.Flat;
				resettingTile.PassThroughOnly = false;
				resettingTile.Unselectable = false;
				resettingTile.Impassable = false;
				resettingTile.RotatesNortheastBottom = false;
				resettingTile.RotatesNortheastTop = false;
				resettingTile.RotatesNorthwestBottom = false;
				resettingTile.RotatesNorthwestTop = false;
				resettingTile.RotatesSoutheastBottom = false;
				resettingTile.RotatesSoutheastTop = false;
				resettingTile.RotatesSouthwestBottom = false;
				resettingTile.RotatesSouthwestTop = false;
			}
		}
		
		GuiStyle.AddSpace();
		
		for (int tileIndex = 0; tileIndex < 2; tileIndex++) {
			GuiStyle.SetNewUiToDefaultStyle();
			GuiStyle.SetElementStyle(ElementStyle.Header);

			if (ImGui.CollapsingHeader("Level " + tileIndex + " Properties", ImGuiTreeNodeFlags.DefaultOpen)) {
				GuiStyle.SetNewUiToDefaultStyle();
				ImGui.Indent();

				ImGui.Columns(2, "TerrainPropertiesColumns", false);
				ImGui.SetColumnWidth(0, GuiStyle.LabelWidth);
				ImGui.SetColumnWidth(1, GuiStyle.WidgetWidth + 10);

				GuiStyle.AddSpace();
				ImGui.Text("Height");
				ImGui.NextColumn();

				GuiStyle.AddSpace();
				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				int beforeHeight = tiles[tileIndex].Height;
				ImGui.InputInt("Height" + tileIndex, ref tiles[tileIndex].Height, 1);
				tiles[tileIndex].Height = Utilities.Clamp(tiles[tileIndex].Height, 0, 63);

				if (beforeHeight != tiles[tileIndex].Height) {
					int difference = tiles[tileIndex].Height - beforeHeight;
					foreach (TerrainTile otherTerrainTile in Selection.SelectedTerrainTiles) {
						if (otherTerrainTile != tiles[tileIndex] &&
						    otherTerrainTile.Level == tiles[tileIndex].Level
						   ) {
							otherTerrainTile.Height += difference;
							otherTerrainTile.Height = Utilities.Clamp(otherTerrainTile.Height, 0, 63);
						}
					}
				}

				ImGui.NextColumn();

				ImGui.Text("Depth");
				ImGui.NextColumn();

				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				int beforeDepth = tiles[tileIndex].Depth;
				ImGui.InputInt("Depth" + tileIndex, ref tiles[tileIndex].Depth, 1);
				tiles[tileIndex].Depth = Utilities.Clamp(tiles[tileIndex].Depth, 0, 63);

				if (beforeDepth != tiles[tileIndex].Depth) {
					foreach (TerrainTile otherTerrainTile in Selection.SelectedTerrainTiles) {
						if (otherTerrainTile != tiles[tileIndex] &&
						    otherTerrainTile.Level == tiles[tileIndex].Level
						   ) {
							otherTerrainTile.Depth = tiles[tileIndex].Depth;
						}
					}
				}

				ImGui.NextColumn();

				GuiStyle.AddSpace();
				ImGui.Text("Slope Type");
				ImGui.NextColumn();

				GuiStyle.AddSpace();
				int comboSelection = (int) tiles[tileIndex].SlopeType;
				TerrainSlopeType beforeSlopeType = tiles[tileIndex].SlopeType;
				ImGui.PushItemWidth(GuiStyle.WidgetWidth);
				ImGui.Combo("SlopeType" + tileIndex, ref comboSelection, slopeTypes.ToArray(), slopeTypes.Count);
				ImGui.PopItemWidth();
				tiles[tileIndex].SlopeType = (TerrainSlopeType) comboSelection;

				if (beforeSlopeType != tiles[tileIndex].SlopeType) {
					foreach (TerrainTile otherTerrainTile in Selection.SelectedTerrainTiles) {
						if (otherTerrainTile != tiles[tileIndex] &&
						    otherTerrainTile.Level == tiles[tileIndex].Level
						   ) {
							otherTerrainTile.SlopeType = tiles[tileIndex].SlopeType;
						}
					}
				}

				ImGui.NextColumn();

				ImGui.Text("Slope Height");
				ImGui.NextColumn();

				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				int beforeSlopeHeight = tiles[tileIndex].SlopeHeight;
				ImGui.InputInt("SlopeHeight" + tileIndex, ref tiles[tileIndex].SlopeHeight, 1);
				tiles[tileIndex].SlopeHeight = Utilities.Min(tiles[tileIndex].SlopeHeight, 0);
				if (beforeSlopeHeight != tiles[tileIndex].SlopeHeight) {
					foreach (TerrainTile otherTerrainTile in Selection.SelectedTerrainTiles) {
						if (otherTerrainTile != tiles[tileIndex] &&
						    otherTerrainTile.Level == tiles[tileIndex].Level
						   ) {
							otherTerrainTile.SlopeHeight = tiles[tileIndex].SlopeHeight;
						}
					}
				}

				ImGui.NextColumn();

				GuiStyle.AddSpace();
				ImGui.Text("Surface Type");
				ImGui.NextColumn();

				GuiStyle.AddSpace();
				comboSelection = (int) tiles[tileIndex].SurfaceType;
				TerrainSurfaceType beforeSurfaceType = tiles[tileIndex].SurfaceType;
				ImGui.PushItemWidth(GuiStyle.WidgetWidth);

				ImGui.Combo(
					"SurfaceType" + tileIndex,
					ref comboSelection,
					surfaceTypes.ToArray(),
					surfaceTypes.Count
				);
				ImGui.PopItemWidth();
				tiles[tileIndex].SurfaceType = (TerrainSurfaceType) comboSelection;
				if (beforeSurfaceType != tiles[tileIndex].SurfaceType) {
					foreach (TerrainTile otherTerrainTile in Selection.SelectedTerrainTiles) {
						if (otherTerrainTile != tiles[tileIndex] &&
						    otherTerrainTile.Level == tiles[tileIndex].Level
						   ) {
							otherTerrainTile.SurfaceType = tiles[tileIndex].SurfaceType;
						}
					}
				}

				ImGui.NextColumn();
				ImGui.Text("Shading");

				ImGui.NextColumn();

				List<string> shadingTypes = Enum.GetNames(typeof(TerrainDarkness)).ToList();
				for (int shadingTypeIndex = 0; shadingTypeIndex < shadingTypes.Count; shadingTypeIndex++) {
					shadingTypes[shadingTypeIndex] =
						Regex.Replace(shadingTypes[shadingTypeIndex], "(\\B[A-Z])", " $1");
				}

				int beforeShading = tiles[tileIndex].Shading;

				ImGui.PushItemWidth(GuiStyle.WidgetWidth);
				ImGui.Combo(
					"###ShadingType" + tileIndex,
					ref tiles[tileIndex].Shading,
					shadingTypes.ToArray(),
					shadingTypes.Count
				);
				ImGui.PopItemWidth();

				if (beforeShading != tiles[tileIndex].Shading) {
					foreach (TerrainTile otherTerrainTile in Selection.SelectedTerrainTiles) {
						if (otherTerrainTile != tiles[tileIndex] &&
						    otherTerrainTile.Level == tiles[tileIndex].Level
						   ) {
							otherTerrainTile.Shading = tiles[tileIndex].Shading;
						}
					}
				}


				ImGui.NextColumn();
				GuiStyle.AddSpace();

				ImGui.Text("Pass Through Only");
				ImGui.NextColumn();
				GuiStyle.AddSpace();

				ImGui.PushItemWidth(GuiStyle.CheckBoxWidth);
				bool beforeUnknown6A = tiles[tileIndex].PassThroughOnly;
				ImGui.Checkbox("###unknown6a" + tileIndex, ref tiles[tileIndex].PassThroughOnly);
				if (beforeUnknown6A != tiles[tileIndex].PassThroughOnly) {
					foreach (TerrainTile otherTerrainTile in Selection.SelectedTerrainTiles) {
						if (otherTerrainTile != tiles[tileIndex] &&
						    otherTerrainTile.Level == tiles[tileIndex].Level
						   ) {
							otherTerrainTile.PassThroughOnly = tiles[tileIndex].PassThroughOnly;
						}
					}
				}

				ImGui.NextColumn();
				ImGui.Text("Impassable");

				ImGui.NextColumn();
				ImGui.PushItemWidth(GuiStyle.CheckBoxWidth);
				bool beforeImpassable = tiles[tileIndex].Impassable;
				ImGui.Checkbox("##impassable" + tileIndex, ref tiles[tileIndex].Impassable);
				if (beforeImpassable != tiles[tileIndex].Impassable) {
					foreach (TerrainTile otherTerrainTile in Selection.SelectedTerrainTiles) {
						if (otherTerrainTile != tiles[tileIndex] &&
						    otherTerrainTile.Level == tiles[tileIndex].Level
						   ) {
							otherTerrainTile.Impassable = tiles[tileIndex].Impassable;
						}
					}
				}

				ImGui.NextColumn();

				ImGui.Text("Unselectable");
				ImGui.NextColumn();

				ImGui.PushItemWidth(GuiStyle.CheckBoxWidth);
				bool beforeUnselectable = tiles[tileIndex].Unselectable;
				ImGui.Checkbox("###unselectable" + tileIndex, ref tiles[tileIndex].Unselectable);
				if (beforeUnselectable != tiles[tileIndex].Unselectable) {
					foreach (TerrainTile otherTerrainTile in Selection.SelectedTerrainTiles) {
						if (otherTerrainTile != tiles[tileIndex] &&
						    otherTerrainTile.Level == tiles[tileIndex].Level
						   ) {
							otherTerrainTile.Unselectable = tiles[tileIndex].Unselectable;
						}
					}
				}

				ImGui.NextColumn();

				if (Configuration.Properties.ShowUnknownValues) {
					GuiStyle.AddSpace();
					ImGui.Text("Unknown0A");

					ImGui.NextColumn();
					GuiStyle.AddSpace();

					int beforeUnknown0A = tiles[tileIndex].Unknown0A;
					ImGui.InputInt("###Unknown0A" + tileIndex, ref tiles[tileIndex].Unknown0A, 1);
					tiles[tileIndex].Unknown0A = Utilities.Min(tiles[tileIndex].Unknown0A, 0);

					if (beforeUnknown0A != tiles[tileIndex].Unknown0A) {
						foreach (TerrainTile otherTerrainTile in Selection.SelectedTerrainTiles) {
							if (otherTerrainTile != tiles[tileIndex] &&
							    otherTerrainTile.Level == tiles[tileIndex].Level
							   ) {
								otherTerrainTile.Unknown0A = tiles[tileIndex].Unknown0A;
							}
						}
					}

					ImGui.NextColumn();

					ImGui.Text("Unknown0B");
					ImGui.NextColumn();

					int beforeUnknown0B = tiles[tileIndex].Unknown0B;
					ImGui.InputInt("###Unknown0B" + tileIndex, ref tiles[tileIndex].Unknown0B, 1);
					tiles[tileIndex].Unknown0B = Utilities.Min(tiles[tileIndex].Unknown0B, 0);

					if (beforeUnknown0B != tiles[tileIndex].Unknown0B) {
						foreach (TerrainTile otherTerrainTile in Selection.SelectedTerrainTiles) {
							if (otherTerrainTile != tiles[tileIndex] &&
							    otherTerrainTile.Level == tiles[tileIndex].Level
							   ) {
								otherTerrainTile.Unknown0B = tiles[tileIndex].Unknown0B;
							}
						}
					}

					ImGui.NextColumn();

					ImGui.Text("Unknown1");
					ImGui.NextColumn();

					int beforeUnknown1 = tiles[tileIndex].Unknown1;
					ImGui.InputInt("###Unknown1" + tileIndex, ref tiles[tileIndex].Unknown1, 1);
					tiles[tileIndex].Unknown1 = Utilities.Min(tiles[tileIndex].Unknown1, 0);

					if (beforeUnknown1 != tiles[tileIndex].Unknown1) {
						foreach (TerrainTile otherTerrainTile in Selection.SelectedTerrainTiles) {
							if (otherTerrainTile != tiles[tileIndex] &&
							    otherTerrainTile.Level == tiles[tileIndex].Level
							   ) {
								otherTerrainTile.Unknown1 = tiles[tileIndex].Unknown1;
							}
						}
					}

					ImGui.NextColumn();

					ImGui.Text("Unknown5");
					ImGui.NextColumn();

					int beforeUnknown5 = tiles[tileIndex].Unknown5;
					ImGui.InputInt("###Unknown5" + tileIndex, ref tiles[tileIndex].Unknown5, 1);
					tiles[tileIndex].Unknown5 = Utilities.Min(tiles[tileIndex].Unknown5, 0);

					if (beforeUnknown5 != tiles[tileIndex].Unknown5) {
						foreach (TerrainTile otherTerrainTile in Selection.SelectedTerrainTiles) {
							if (otherTerrainTile != tiles[tileIndex] &&
							    otherTerrainTile.Level == tiles[tileIndex].Level
							   ) {
								otherTerrainTile.Unknown5 = tiles[tileIndex].Unknown5;
							}
						}
					}

					ImGui.NextColumn();

					ImGui.Text("Unknown 6B");
					ImGui.NextColumn();

					ImGui.PushItemWidth(GuiStyle.CheckBoxWidth);
					bool beforeUnknown6B = tiles[tileIndex].Unknown6B;
					ImGui.Checkbox("###unknown6b" + tileIndex, ref tiles[tileIndex].Unknown6B);
					if (beforeUnknown6B != tiles[tileIndex].Unknown6B) {
						foreach (TerrainTile otherTerrainTile in Selection.SelectedTerrainTiles) {
							if (otherTerrainTile != tiles[tileIndex] &&
							    otherTerrainTile.Level == tiles[tileIndex].Level
							   ) {
								otherTerrainTile.Unknown6B = tiles[tileIndex].Unknown6B;
							}
						}
					}

					ImGui.NextColumn();

					ImGui.Text("Unknown 6C");
					ImGui.NextColumn();

					ImGui.PushItemWidth(GuiStyle.CheckBoxWidth);
					bool beforeUnknown6C = tiles[tileIndex].Unknown6C;
					ImGui.Checkbox("###unknown6C" + tileIndex, ref tiles[tileIndex].Unknown6C);
					if (beforeUnknown6C != tiles[tileIndex].Unknown6C) {
						foreach (TerrainTile otherTerrainTile in Selection.SelectedTerrainTiles) {
							if (otherTerrainTile != tiles[tileIndex] &&
							    otherTerrainTile.Level == tiles[tileIndex].Level
							   ) {
								otherTerrainTile.Unknown6C = tiles[tileIndex].Unknown6C;
							}
						}
					}

					ImGui.NextColumn();
					ImGui.Text("Unknown 6D");
					ImGui.NextColumn();

					ImGui.PushItemWidth(GuiStyle.CheckBoxWidth);
					bool beforeUnknown6D = tiles[tileIndex].Unknown6D;
					ImGui.Checkbox("###unknown6D" + tileIndex, ref tiles[tileIndex].Unknown6D);
					if (beforeUnknown6D != tiles[tileIndex].Unknown6D) {
						foreach (TerrainTile otherTerrainTile in Selection.SelectedTerrainTiles) {
							if (otherTerrainTile != tiles[tileIndex] &&
							    otherTerrainTile.Level == tiles[tileIndex].Level
							   ) {
								otherTerrainTile.Unknown6D = tiles[tileIndex].Unknown6D;
							}
						}
					}

					ImGui.NextColumn();
				}

				ImGui.Columns(1);

				GuiStyle.AddSpace();
				ImGui.Text("Auto-Rotates When Camera Facing: ");
				GuiStyle.AddSpace();

				ImGui.Columns(5, "TerrainPropertiesRotationAnglesColumns", false);

				const int mainLabelWidth = 80;
				const int checkboxWidth = 20;

				ImGui.Indent();

				ImGui.SetColumnWidth(0, mainLabelWidth);
				ImGui.SetColumnWidth(1, checkboxWidth + 10);
				ImGui.SetColumnWidth(2, checkboxWidth + 10);
				ImGui.SetColumnWidth(3, checkboxWidth + 10);
				ImGui.SetColumnWidth(4, checkboxWidth + 10);

				ImGui.NextColumn();

				ImGuiStylePtr style = ImGui.GetStyle();

				if (StageCamera.FacingDirection == StageCamera.CameraView.Northwest) {
					style.Colors[(int) ImGuiCol.Text] = GuiStyle.ColorPalette[ColorName.HighlightedText];
				} else {
					style.Colors[(int) ImGuiCol.Text] = GuiStyle.ColorPalette[ColorName.Lightest];
				}

				ImGui.Text("NW");
				ImGui.NextColumn();

				if (StageCamera.FacingDirection == StageCamera.CameraView.Northeast) {
					style.Colors[(int) ImGuiCol.Text] = GuiStyle.ColorPalette[ColorName.HighlightedText];
				} else {
					style.Colors[(int) ImGuiCol.Text] = GuiStyle.ColorPalette[ColorName.Lightest];
				}

				ImGui.Text("NE");
				ImGui.NextColumn();

				if (StageCamera.FacingDirection == StageCamera.CameraView.Southwest) {
					style.Colors[(int) ImGuiCol.Text] = GuiStyle.ColorPalette[ColorName.HighlightedText];
				} else {
					style.Colors[(int) ImGuiCol.Text] = GuiStyle.ColorPalette[ColorName.Lightest];
				}

				ImGui.Text("SW");
				ImGui.NextColumn();

				if (StageCamera.FacingDirection == StageCamera.CameraView.Southeast) {
					style.Colors[(int) ImGuiCol.Text] = GuiStyle.ColorPalette[ColorName.HighlightedText];
				} else {
					style.Colors[(int) ImGuiCol.Text] = GuiStyle.ColorPalette[ColorName.Lightest];
				}

				ImGui.Text("SE");
				ImGui.NextColumn();

				style.Colors[(int) ImGuiCol.Text] = GuiStyle.ColorPalette[ColorName.Lightest];

				ImGui.Text("Top");
				ImGui.NextColumn();

				bool beforeValue = tiles[tileIndex].RotatesNorthwestTop;
				ImGui.Checkbox("##rotationTopNW" + tileIndex, ref tiles[tileIndex].RotatesNorthwestTop);

				if (beforeValue != tiles[tileIndex].RotatesNorthwestTop) {
					foreach (TerrainTile selectedTerrainTile in Selection.SelectedTerrainTiles) {
						if (selectedTerrainTile.Level == tiles[tileIndex].Level) {
							selectedTerrainTile.RotatesNorthwestTop = tiles[tileIndex].RotatesNorthwestTop;
						}
					}
				}

				ImGui.NextColumn();

				beforeValue = tiles[tileIndex].RotatesNortheastTop;
				ImGui.Checkbox("##rotationTopNE" + tileIndex, ref tiles[tileIndex].RotatesNortheastTop);

				if (beforeValue != tiles[tileIndex].RotatesNortheastTop) {
					foreach (TerrainTile selectedTerrainTile in Selection.SelectedTerrainTiles) {
						if (selectedTerrainTile.Level == tiles[tileIndex].Level) {
							selectedTerrainTile.RotatesNortheastTop = tiles[tileIndex].RotatesNortheastTop;
						}
					}
				}

				ImGui.NextColumn();

				beforeValue = tiles[tileIndex].RotatesSouthwestTop;
				ImGui.Checkbox("##rotationTopSW" + tileIndex, ref tiles[tileIndex].RotatesSouthwestTop);

				if (beforeValue != tiles[tileIndex].RotatesSouthwestTop) {
					foreach (TerrainTile selectedTerrainTile in Selection.SelectedTerrainTiles) {
						if (selectedTerrainTile.Level == tiles[tileIndex].Level) {
							selectedTerrainTile.RotatesSouthwestTop = tiles[tileIndex].RotatesSouthwestTop;
						}
					}
				}

				ImGui.NextColumn();

				beforeValue = tiles[tileIndex].RotatesSoutheastTop;
				ImGui.Checkbox("##rotationTopSE" + tileIndex, ref tiles[tileIndex].RotatesSoutheastTop);

				if (beforeValue != tiles[tileIndex].RotatesSoutheastTop) {
					foreach (TerrainTile selectedTerrainTile in Selection.SelectedTerrainTiles) {
						if (selectedTerrainTile.Level == tiles[tileIndex].Level) {
							selectedTerrainTile.RotatesSoutheastTop = tiles[tileIndex].RotatesSoutheastTop;
						}
					}
				}

				ImGui.NextColumn();

				ImGui.Text("Bottom");
				ImGui.NextColumn();

				beforeValue = tiles[tileIndex].RotatesNorthwestBottom;
				ImGui.Checkbox("##rotationBottomNW" + tileIndex, ref tiles[tileIndex].RotatesNorthwestBottom);

				if (beforeValue != tiles[tileIndex].RotatesNorthwestBottom) {
					foreach (TerrainTile selectedTerrainTile in Selection.SelectedTerrainTiles) {
						if (selectedTerrainTile.Level == tiles[tileIndex].Level) {
							selectedTerrainTile.RotatesNorthwestBottom = tiles[tileIndex].RotatesNorthwestBottom;
						}
					}
				}

				ImGui.NextColumn();

				beforeValue = tiles[tileIndex].RotatesNortheastBottom;
				ImGui.Checkbox("##rotationBottomNE" + tileIndex, ref tiles[tileIndex].RotatesNortheastBottom);

				if (beforeValue != tiles[tileIndex].RotatesNortheastBottom) {
					foreach (TerrainTile selectedTerrainTile in Selection.SelectedTerrainTiles) {
						if (selectedTerrainTile.Level == tiles[tileIndex].Level) {
							selectedTerrainTile.RotatesNortheastBottom = tiles[tileIndex].RotatesNortheastBottom;
						}
					}
				}

				ImGui.NextColumn();

				beforeValue = tiles[tileIndex].RotatesSouthwestBottom;
				ImGui.Checkbox("##rotationBottomSW" + tileIndex, ref tiles[tileIndex].RotatesSouthwestBottom);

				if (beforeValue != tiles[tileIndex].RotatesSouthwestBottom) {
					foreach (TerrainTile selectedTerrainTile in Selection.SelectedTerrainTiles) {
						if (selectedTerrainTile.Level == tiles[tileIndex].Level) {
							selectedTerrainTile.RotatesSouthwestBottom = tiles[tileIndex].RotatesSouthwestBottom;
						}
					}
				}

				ImGui.NextColumn();

				beforeValue = tiles[tileIndex].RotatesSoutheastBottom;
				ImGui.Checkbox("##rotationBottomSE" + tileIndex, ref tiles[tileIndex].RotatesSoutheastBottom);

				if (beforeValue != tiles[tileIndex].RotatesSoutheastBottom) {
					foreach (TerrainTile selectedTerrainTile in Selection.SelectedTerrainTiles) {
						if (selectedTerrainTile.Level == tiles[tileIndex].Level) {
							selectedTerrainTile.RotatesSoutheastBottom = tiles[tileIndex].RotatesSoutheastBottom;
						}
					}
				}

				ImGui.NextColumn();

				ImGui.Unindent();

				ImGui.Columns(1);
				ImGui.Unindent();
				GuiStyle.AddSpace();
			}
		}
	}

	private static void RenderGreyboxingForm() {
		GuiStyle.SetNewUiToDefaultStyle();
		GuiStyle.SetElementStyle(ElementStyle.Header);

		if (ImGui.CollapsingHeader("Greyboxing", ImGuiTreeNodeFlags.DefaultOpen)) {
			GuiStyle.SetNewUiToDefaultStyle();
			ImGui.Indent();

			GuiStyle.SetElementStyle(ElementStyle.Header);
			GuiStyle.SetFont(Fonts.Default);
			if(ImGui.CollapsingHeader("About Greyboxing")){
				ImGui.Indent();
				GuiStyle.SetNewUiToDefaultStyle();
				ImGui.Text("Greyboxing will delete all the polygons in");
				ImGui.Text("the Primary Mesh, and then build new level");
				ImGui.Text("geometry based on the terrain. Use this to");
				ImGui.Text("prototype your map before building your");
				ImGui.Text("own details.");
				GuiStyle.AddSpace();
				ImGui.Text("Painting the Texture will update the map ");
				ImGui.Text("texture on Page 0 in the top left corner."); 
				GuiStyle.AddSpace();
				ImGui.Text("Modifying the Palette will overwrite the ");
				ImGui.Text("first and second colors of the first two ");
				ImGui.Text("palettes.");
				ImGui.Unindent();
			}

			GuiStyle.SetNewUiToDefaultStyle();

			GuiStyle.AddSpace();
			
			ImGui.Columns(2, "Greybox Settings", false);

			ImGui.SetColumnWidth(0, GuiStyle.LabelWidth - 20);
			ImGui.SetColumnWidth(1, GuiStyle.WidgetWidth + 30);

			ImGui.Text("Paint on Texture");
			ImGui.NextColumn();

			ImGui.Checkbox("##writeGreyboxTexture", ref _writeGreyboxTexture);
			ImGui.NextColumn();

			ImGui.Text("Modify Palettes");
			ImGui.NextColumn();

			ImGui.Checkbox("##writeGreyboxPalette", ref _writeGreyboxPalettes);
			ImGui.NextColumn();

			ImGui.Text("Single-Poly Walls");
			ImGui.NextColumn();

			ImGui.Checkbox("##SinglePolyWalls", ref _greyboxSinglePolyWalls);
			ImGui.NextColumn();

			ImGui.Columns(1);
			if (ImGui.Button("Greybox Mesh")) {
				Greyboxer.Greybox(_writeGreyboxTexture, _writeGreyboxPalettes, _greyboxSinglePolyWalls);
			}

			ImGui.Unindent();
			GuiStyle.AddSpace();
		}
	}

	private static void RenderTerrainRenderOptions() {
		GuiStyle.SetNewUiToDefaultStyle();
		GuiStyle.SetElementStyle(ElementStyle.Header);

		if (ImGui.CollapsingHeader("Render Options", ImGuiTreeNodeFlags.DefaultOpen)) {
			GuiStyle.SetNewUiToDefaultStyle();
			ImGui.Indent();

			ImGui.Columns(2, "TerrainOptionsColumns", false);
			ImGui.SetColumnWidth(0, GuiStyle.LabelWidth);
			ImGui.SetColumnWidth(1, GuiStyle.WidgetWidth + 10);

			ImGui.Text("Terrain Transparency");
			ImGui.NextColumn();

			ImGui.PushItemWidth(GuiStyle.WidgetWidth);
			int beforeTerrainTransparency = Configuration.Properties.TerrainTransparencyForTerrainEditing;
			ImGui.SliderInt(
				"###terrainTransparency",
				ref Configuration.Properties.TerrainTransparencyForTerrainEditing,
				0, 100
			);

			if (beforeTerrainTransparency != Configuration.Properties.TerrainTransparencyForTerrainEditing) {
				if (Configuration.Properties.TerrainTransparencyForTerrainEditing < 100) {
					Configuration.Properties.PolygonTransparencyForTerrainEditing = 100;
				}

				Configuration.SaveConfiguration();
			}

			ImGui.NextColumn();

			ImGui.Text("Polygon Transparency");
			ImGui.NextColumn();

			ImGui.PushItemWidth(GuiStyle.WidgetWidth);
			int beforeTransparency = Configuration.Properties.PolygonTransparencyForTerrainEditing;
			ImGui.SliderInt(
				"###polyTransparency",
				ref Configuration.Properties.PolygonTransparencyForTerrainEditing,
				0, 100
			);

			if (beforeTransparency != Configuration.Properties.PolygonTransparencyForTerrainEditing) {
				if (Configuration.Properties.PolygonTransparencyForTerrainEditing < 100) {
					Configuration.Properties.TerrainTransparencyForTerrainEditing = 100;
				}

				Configuration.SaveConfiguration();
			}

			ImGui.NextColumn();

			ImGui.Columns(1);
			ImGui.Unindent();
			GuiStyle.AddSpace();
		}
	}

	private static void RenderResizeTerrainPanel() {
		GuiStyle.SetNewUiToDefaultStyle();
		GuiStyle.SetElementStyle(ElementStyle.Header);

		if (ImGui.CollapsingHeader("Resize Terrain", ImGuiTreeNodeFlags.DefaultOpen)) {
			GuiStyle.SetNewUiToDefaultStyle();
			ImGui.Indent();

			if (!ResizeTerrainMode) {
				if (ImGui.Button("Resize Terrain###ToResizeTerrainMode")) {
					ResizeTerrainMode = true;
					_resizeXSize = CurrentMapState.StateData.Terrain.SizeX;
					_resizeZSize = CurrentMapState.StateData.Terrain.SizeZ;
				}
			} else {
				ImGui.Columns(2, "ResizeTerrainColumns", false);
				ImGui.SetColumnWidth(0, GuiStyle.LabelWidth);
				ImGui.SetColumnWidth(1, GuiStyle.WidgetWidth + 10);

				ImGui.Text("X");
				ImGui.NextColumn();

				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.InputInt("ResizeTerrainX", ref _resizeXSize);
				_resizeXSize = Utilities.Clamp(
					_resizeXSize,
					1,
					(int) Math.Floor(256f / _resizeZSize)
				);

				ImGui.NextColumn();

				ImGui.Text("Z");
				ImGui.NextColumn();

				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.InputInt("ResizeTerrainZ", ref _resizeZSize);
				_resizeZSize = Utilities.Clamp(
					_resizeZSize,
					1,
					(int) Math.Floor(256f / _resizeXSize)
				);

				ImGui.NextColumn();

				ImGui.Text("Insert/Delete from Front");
				ImGui.NextColumn();
				ImGui.Checkbox("###insertAtFront", ref _resizeFromFront);
				ImGui.NextColumn();

				GuiStyle.AddSpace();
				if (ImGui.Button("Cancel")) {
					ResizeTerrainMode = false;
				}

				ImGui.NextColumn();

				GuiStyle.AddSpace();
				if (ImGui.Button("Resize", new Vector2(GuiStyle.WidgetWidth, 20))) {
					ResizeTerrainMode = false;
					Selection.SelectedTerrainTiles.Clear();
					CurrentMapState.StateData.Terrain.ResizeTerrain(_resizeXSize, _resizeZSize, _resizeFromFront);
				}

				ImGui.Columns(1);

				if (_resizeXSize > 18 || _resizeZSize > 18) {
					GuiStyle.AddSpace();
					ImGui.TextWrapped(
						"Warning: The AI tends to break down on maps where either axis is greater than 18"
					);
				}
			}

			ImGui.Unindent();
			GuiStyle.AddSpace();
		}
	}
}