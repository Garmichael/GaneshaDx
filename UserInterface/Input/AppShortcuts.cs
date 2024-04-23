using System.Collections.Generic;
using GaneshaDx.Common;
using GaneshaDx.Environment;
using GaneshaDx.Resources;
using GaneshaDx.Resources.ContentDataTypes.Polygons;
using GaneshaDx.UserInterface.GuiDefinitions;
using GaneshaDx.UserInterface.Widgets;
using Microsoft.Xna.Framework.Input;

namespace GaneshaDx.UserInterface.Input {
	public static class AppShortcuts {
		public static void Update() {
			if (!MyraGui.IsActive && AppInput.ControlHeld && AppInput.KeyJustPressed(Keys.O)) {
				MyraGui.OpenOpenFileDialog();
			}

			if (AppInput.KeyJustPressed(Keys.F12)) {
				Stage.ToggleScreenshotMode();
			}

			if (MapData.MapIsLoaded && !MyraGui.IsActive) {
				if (!AppInput.ControlHeld && !AppInput.ShiftHeld && !AppInput.AltHeld) {
					if (AppInput.KeyJustPressed(Keys.D1)) {
						Gui.SelectedTab = RightPanelTab.Polygon;
					}

					if (AppInput.KeyJustPressed(Keys.D2)) {
						Gui.SelectedTab = RightPanelTab.Texture;
					}

					if (AppInput.KeyJustPressed(Keys.D3)) {
						Gui.SelectedTab = RightPanelTab.Terrain;
					}

					if (AppInput.KeyJustPressed(Keys.D4)) {
						Gui.SelectedTab = RightPanelTab.Map;
					}

					if (AppInput.KeyJustPressed(Keys.Z)) {
						StageCamera.FocusOnSelection();
					}

					if (AppInput.KeyJustPressed(Keys.Q)) {
						Gui.Widget = WidgetSelectionMode.Select;
					}

					if (AppInput.KeyJustPressed(Keys.W)) {
						Gui.Widget = WidgetSelectionMode.PolygonTranslate;
					}

					if (AppInput.KeyJustPressed(Keys.R)) {
						Gui.Widget = WidgetSelectionMode.PolygonVertexTranslate;
					}

					if (AppInput.KeyJustPressed(Keys.E)) {
						Gui.Widget = WidgetSelectionMode.PolygonRotate;
					}

					if (AppInput.KeyJustPressed(Keys.T)) {
						Gui.Widget = WidgetSelectionMode.PolygonEdgeTranslate;
					}

					if (AppInput.KeyJustPressed(Keys.L)) {
						Configuration.Properties.RenderPolygonsInLightingMode =
							!Configuration.Properties.RenderPolygonsInLightingMode;
						Configuration.SaveConfiguration();
					}

					if (AppInput.KeyJustPressed(Keys.H)) {
						Configuration.Properties.HideHiddenPolysByFacing =
							!Configuration.Properties.HideHiddenPolysByFacing;
						Configuration.SaveConfiguration();
					}

					if (AppInput.KeyJustPressed(Keys.OemPlus) || AppInput.KeyJustPressed(Keys.Add)) {
						if (Gui.SelectedTab == RightPanelTab.Terrain) {
							Selection.GrowTerrainSelection();
						} else {
							Selection.GrowPolygonSelection();
						}
					}

					if (AppInput.KeyJustPressed(Keys.P)) {
						Gui.ShowPolygonListWindow = !Gui.ShowPolygonListWindow;
					}

					if (AppInput.KeyJustPressed(Keys.Delete)) {
						CurrentMapState.DeleteSelection();
					}

					if (AppInput.KeyJustPressed(Keys.C)) {
						Gui.ShowCameraControlWindow = !Gui.ShowCameraControlWindow;
					}

					if (AppInput.KeyJustPressed(Keys.M)) {
						Gui.ToggleManageResourcesWindow();
					}

					if (AppInput.KeyJustPressed(Keys.O)) {
						Configuration.Properties.RenderFftOrtho = !Configuration.Properties.RenderFftOrtho;
						Configuration.SaveConfiguration();
					}

					if (AppInput.KeyJustPressed(Keys.F)) {
						if (Gui.Widget == WidgetSelectionMode.PolygonEdgeTranslate) {
							TransformWidget.SelectNextEdge(AppInput.ShiftHeld);
						} else if (Gui.Widget == WidgetSelectionMode.PolygonVertexTranslate) {
							TransformWidget.SelectNextVertex(AppInput.ShiftHeld);
						} else if (Gui.Widget == WidgetSelectionMode.PolygonRotate &&
						           Selection.SelectedPolygons.Count > 0
						) {
							if (!RotationWidget.AnchoredToVertex) {
								RotationWidget.AnchoredToVertex = true;
								RotationWidget.AnchoredVertex = 0;
							} else {
								RotationWidget.AnchoredVertex++;

								int totalVerts = Selection.SelectedPolygons[0].IsQuad
									? 4
									: 3;

								if (RotationWidget.AnchoredVertex > totalVerts - 1) {
									RotationWidget.AnchoredToVertex = false;
									RotationWidget.AnchoredVertex = 0;
								}
							}
						}
					}

					if (AppInput.KeyJustPressed(Keys.S)) {
						TransformWidget.SelectSnappingVertex();
					}

					if (AppInput.KeyJustPressed(Keys.D)) {
						TransformWidget.SnapSnappingVertex();
					}
					
					if (AppInput.KeyJustPressed(Keys.A)) {
						bool canEditMeshAnimations = MapData.MapIsLoaded &&
						                             CurrentMapState.StateData.MeshAnimationInstructions != null;
						if (canEditMeshAnimations) {
							Gui.ShowMeshAnimationsWindow = !Gui.ShowMeshAnimationsWindow;
						}
					}

					if (AppInput.KeyJustPressed(Keys.K)) {
						Configuration.Properties.PlaysScriptedTextureAnimations = !Configuration.Properties.PlaysScriptedTextureAnimations;
						Configuration.SaveConfiguration();
					}
				}

				if (AppInput.ControlHeld && !AppInput.ShiftHeld && !AppInput.AltHeld) {
					if (AppInput.KeyJustPressed(Keys.S)) {
						MapData.SaveMap();
					}

					if (AppInput.KeyJustPressed(Keys.A)) {
						if (Gui.SelectedTab == RightPanelTab.Terrain) {
							Selection.SelectAllTerrainTiles();
						} else {
							Gui.Widget = WidgetSelectionMode.PolygonTranslate;
							Selection.SelectAllPolygons();
						}
					}

					if (AppInput.KeyJustPressed(Keys.D)) {
						CurrentMapState.CloneSelection();
					}

					if (AppInput.KeyJustPressed(Keys.N)) {
						Gui.ShowAddPolygonWindow = !Gui.ShowAddPolygonWindow;
					}

					if (AppInput.KeyJustPressed(Keys.Q)) {
						Stage.Ganesha.Exit();
					}

					if (AppInput.KeyJustPressed(Keys.I)) {
						MyraGui.OpenImportTextureFileDialog();
					}

					if (AppInput.KeyJustPressed(Keys.E)) {
						string fileName = MapData.MapName + "." +
						                  CurrentMapState.StateData.StateTextureResource.XFile +
						                  ".png";

						MyraGui.OpenExportTextureFileDialog(fileName);
					}

					if (AppInput.KeyJustPressed(Keys.U)) {
						string fileName = MapData.MapName + "." +
						                  CurrentMapState.StateData.StateTextureResource.XFile +
						                  ".uvMap" +
						                  ".png";

						MyraGui.OpenExportUvMapFileDialog(fileName);
					}

					if (AppInput.KeyJustPressed(Keys.R)) {
						if (MyraGui.LastImportedTextureFile != "") {
							MapData.ImportTexture(MyraGui.LastImportedTextureFile);
						}
					}

					if (AppInput.KeyJustPressed(Keys.G)) {
						if (Selection.SelectedPolygons.Count > 0) {
							foreach (Polygon selectedPolygon in Selection.SelectedPolygons) {
								selectedPolygon.GuessNormals();
							}

							Utilities.AverageNormals();
						}
					}
					
					if (AppInput.KeyJustPressed(Keys.L)) {
						if (Selection.SelectedPolygons.Count > 0) {
							foreach (Polygon selectedPolygon in Selection.SelectedPolygons) {
								selectedPolygon.GuessNormals();
							}
						}
					}

					if (AppInput.KeyJustPressed(Keys.B)) {
						List<Polygon> newPolys = new List<Polygon>();
						foreach (Polygon selectedPolygon in Selection.SelectedPolygons) {
							newPolys.AddRange(selectedPolygon.Break());
						}

						Selection.SelectedPolygons.Clear();

						foreach (Polygon newPolygon in newPolys) {
							Selection.AddPolyToSelection(newPolygon);
						}
					}

					if (AppInput.KeyJustPressed(Keys.F)) {
						foreach (Polygon selectedPolygon in Selection.SelectedPolygons) {
							selectedPolygon.FlipNormals();
						}

						Utilities.AverageNormals();
					}

					if (AppInput.KeyJustPressed(Keys.H)) {
						bool canEditMeshAnimations = MapData.MapIsLoaded &&
						                             CurrentMapState.StateData.MeshAnimationInstructions != null;
						if (canEditMeshAnimations) {
							Gui.ShowDebugAnimatedMeshWindow = !Gui.ShowDebugAnimatedMeshWindow;
						}
					}

					if (AppInput.KeyJustPressed(Keys.T)) {
						Gui.ShowRawTerrainDataWindow = !Gui.ShowRawTerrainDataWindow;
					}

					if (AppInput.KeyJustPressed(Keys.P)) {
						Gui.ShowPreferencesWindow = !Gui.ShowPreferencesWindow;
					}
				}

				if (AppInput.ControlHeld && AppInput.ShiftHeld && !AppInput.AltHeld) {
					if (AppInput.KeyJustPressed(Keys.E)) {
						Gui.ShowExportGlbWindow = !Gui.ShowExportGlbWindow;
					}

					if (AppInput.KeyJustPressed(Keys.A)) {
						if (Gui.SelectedTab != RightPanelTab.Terrain) {
							Selection.SelectOverlappingPolygons();
						}
					}
				}

				if (!AppInput.ControlHeld && AppInput.ShiftHeld && !AppInput.AltHeld) {
					if (AppInput.KeyJustPressed(Keys.F)) {
						if (Gui.Widget == WidgetSelectionMode.PolygonEdgeTranslate) {
							TransformWidget.SelectNextEdge(AppInput.ShiftHeld);
						} else if (Gui.Widget == WidgetSelectionMode.PolygonVertexTranslate) {
							TransformWidget.SelectNextVertex(AppInput.ShiftHeld);
						} else if (Gui.Widget == WidgetSelectionMode.PolygonRotate &&
						           Selection.SelectedPolygons.Count > 0
						) {
							if (!RotationWidget.AnchoredToVertex) {
								RotationWidget.AnchoredToVertex = true;
								RotationWidget.AnchoredVertex = 0;
							} else {
								RotationWidget.AnchoredVertex++;

								int totalVerts = Selection.SelectedPolygons[0].IsQuad
									? 4
									: 3;

								if (RotationWidget.AnchoredVertex > totalVerts - 1) {
									RotationWidget.AnchoredToVertex = false;
									RotationWidget.AnchoredVertex = 0;
								}
							}
						}
					}
				}
			}
		}
	}
}