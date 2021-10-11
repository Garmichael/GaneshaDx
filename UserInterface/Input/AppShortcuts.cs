using GaneshaDx.Common;
using GaneshaDx.Environment;
using GaneshaDx.Resources;
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
				if (!AppInput.ControlHeld) {
					if (AppInput.KeyJustPressed(Keys.F1)) {
						Gui.ShowTipsWindow = !Gui.ShowTipsWindow;
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

					if (AppInput.KeyJustPressed(Keys.OemPlus) || AppInput.KeyJustPressed(Keys.Add)) {
						if (Gui.SelectedTab == RightPanelTab.Terrain) {
							Selection.GrowTerrainSelection();
						} else {
							Selection.GrowPolygonSelection();
						}
					}

					if (AppInput.KeyJustPressed(Keys.P)) {
						Gui.ShowPreferencesWindow = !Gui.ShowPreferencesWindow;
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

					if (AppInput.KeyJustPressed(Keys.B)) {
						Configuration.Properties.RenderFFTOrtho = !Configuration.Properties.RenderFFTOrtho;
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
				}

				if (AppInput.ControlHeld) {
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

					if (AppInput.KeyJustPressed(Keys.R)) {
						if (MyraGui.LastImportedTextureFile != "") {
							MapData.ImportTexture(MyraGui.LastImportedTextureFile);
						}
					}

					if (AppInput.KeyJustPressed(Keys.G)) {
						bool canEditMeshAnimations = MapData.MapIsLoaded &&
						                             CurrentMapState.StateData.MeshAnimationInstructions != null;
						if (canEditMeshAnimations) {
							Gui.ShowMeshAnimationsWindow = !Gui.ShowMeshAnimationsWindow;
						}
					}

					if (AppInput.KeyJustPressed(Keys.H)) {
						bool canEditMeshAnimations = MapData.MapIsLoaded &&
						                             CurrentMapState.StateData.MeshAnimationInstructions != null;
						if (canEditMeshAnimations) {
							Gui.ShowDebugAnimatedMeshWindow = !Gui.ShowDebugAnimatedMeshWindow;
						}
					}
				}
			}
		}
	}
}