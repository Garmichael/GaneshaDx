using GaneshaDx.Common;
using GaneshaDx.Environment;
using GaneshaDx.Resources;
using GaneshaDx.UserInterface.GuiDefinitions;
using Microsoft.Xna.Framework.Input;

namespace GaneshaDx.UserInterface.Input {
	public static class AppShortcuts {
		public static void Update() {
			if (!MyraGui.IsActive && AppInput.ControlHeld && AppInput.KeyJustPressed(Keys.O)) {
				MyraGui.OpenOpenFileDialog();
			}

			if (MapData.MapIsLoaded && !MyraGui.IsActive) {
				if (AppInput.KeyJustPressed(Keys.F1)) {
					Gui.ShowTipsWindow = !Gui.ShowTipsWindow;
				}

				if (AppInput.ControlHeld && AppInput.KeyJustPressed(Keys.S)) {
					MapData.SaveMap();
				}

				if (AppInput.KeyJustPressed(Keys.Z)) {
					StageCamera.FocusOnSelection();
				}

				if (AppInput.KeyJustPressed(Keys.Q)) {
					Gui.WidgetSelectionMode = WidgetSelectionMode.Select;
				}

				if (AppInput.KeyJustPressed(Keys.W)) {
					Gui.WidgetSelectionMode = WidgetSelectionMode.PolygonTranslate;
				}

				if (AppInput.KeyJustPressed(Keys.E)) {
					Gui.WidgetSelectionMode = WidgetSelectionMode.PolygonRotate;
				}

				if (AppInput.KeyJustPressed(Keys.R)) {
					Gui.WidgetSelectionMode = WidgetSelectionMode.PolygonVertexTranslate;
				}

				if (AppInput.KeyJustPressed(Keys.T)) {
					Gui.WidgetSelectionMode = WidgetSelectionMode.PolygonEdgeTranslate;
				}

				if (AppInput.KeyJustPressed(Keys.L)) {
					Configuration.Properties.RenderPolygonsInLightingMode =
						!Configuration.Properties.RenderPolygonsInLightingMode;
					Configuration.SaveConfiguration();
				}

				if (AppInput.ControlHeld && AppInput.KeyJustPressed(Keys.A)) {
					if (Gui.SelectedTab == RightPanelTab.Terrain) {
						Selection.SelectAllTerrainTiles();
					} else {
						Gui.WidgetSelectionMode = WidgetSelectionMode.PolygonTranslate;
						Selection.SelectAllPolygons();
					}
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

				if (AppInput.ControlHeld && AppInput.KeyJustPressed(Keys.D)) {
					CurrentMapState.CloneSelection();
				}

				if (AppInput.KeyJustPressed(Keys.Delete)) {
					CurrentMapState.DeleteSelection();
				}

				if (AppInput.ControlHeld && AppInput.KeyJustPressed(Keys.N)) {
					Gui.ShowAddPolygonWindow = !Gui.ShowAddPolygonWindow;
				}

				if (AppInput.ControlHeld && AppInput.KeyJustPressed(Keys.Q)) {
					Stage.Ganesha.Exit();
				}

				if (AppInput.KeyJustPressed(Keys.C)) {
					Gui.ShowCameraControlWindow = !Gui.ShowCameraControlWindow;
				}

				if (AppInput.KeyJustPressed(Keys.M)) {
					Gui.ToggleManageResourcesWindow();
				}

				if (AppInput.ControlHeld && AppInput.KeyJustPressed(Keys.I)) {
					MyraGui.OpenImportTextureFileDialog();
				}

				if (AppInput.ControlHeld && AppInput.KeyJustPressed(Keys.E)) {
					string fileName = MapData.MapName + "." +
					                  CurrentMapState.StateData.StateTextureResource.XFile +
					                  ".png";

					MyraGui.OpenExportTextureFileDialog(fileName);
				}


				if (AppInput.ControlHeld && AppInput.KeyJustPressed(Keys.R)) {
					if (MyraGui.LastImportedTextureFile != "") {
						MapData.ImportTexture(MyraGui.LastImportedTextureFile);
					}
				}
				
				if (AppInput.ControlHeld && AppInput.KeyJustPressed(Keys.G)) {
					Gui.ShowDebugAnimatedMeshWindow = !Gui.ShowDebugAnimatedMeshWindow;
				}
			}
		}
	}
}