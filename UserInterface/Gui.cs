using System;
using GaneshaDx.Common;
using GaneshaDx.Environment;
using GaneshaDx.Resources;
using GaneshaDx.UserInterface.GuiDefinitions;
using GaneshaDx.UserInterface.GuiForms;
using GaneshaDx.UserInterface.Input;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Vector2 = System.Numerics.Vector2;

namespace GaneshaDx.UserInterface {
	public static class Gui {
		public static RightPanelTab SelectedTab = RightPanelTab.Map;
		public static WidgetSelectionMode Widget = WidgetSelectionMode.PolygonTranslate;

		private static bool _showDebugPanel;
		public static bool ShowCameraControlWindow;
		public static bool ShowPreferencesWindow;
		public static bool ShowAddPolygonWindow;
		public static bool ShowPolygonListWindow;
		public static bool ShowRawTerrainDataWindow;
		private static bool _showManageResourcesWindow;
		public static bool ShowAboutWindow;
		public static bool ShowDebugAnimatedMeshWindow;
		public static bool ShowMeshAnimationsWindow;
		public static bool ShowExportGlbWindow;
		public static bool ShowGnsDataWindow;
		public static bool ShowOpenFileWindow;

		public static void Render() {
			Stage.GraphicsDevice.Clear(Color.Transparent);

			if (!Stage.Ganesha.IsActive) {
				return;	
			}
			
			if (AppInput.ControlHeld && AppInput.ShiftHeld && AppInput.AltHeld && AppInput.KeyJustPressed(Keys.D)) {
				_showDebugPanel = !_showDebugPanel;
			}

			Stage.ImGuiRenderer.BeforeLayout(Stage.GameTime.ElapsedGameTime);
			{
				GuiStyle.SetNewUiToDefaultStyle();
				MyraGui.LockModeling = false;

				if (_showDebugPanel) {
					ImGui.ShowDemoWindow();
				}

				if (_showManageResourcesWindow) {
					GuiWindowManageResources.Render();
					MyraGui.LockModeling = true;
				} else if (Stage.ScreenshotMode) {
					MyraGui.LockModeling = true;
				} else {
					GuiMenuBar.Render();
					GuiWindowTexturePreview.Render();
	
					if(ShowOpenFileWindow){
						GuiWindowFileBrowser.Render();
					}
					
					if (MapData.MapIsLoaded) {
						RenderTabPanel();
						RenderMainPanel();

						if (ShowCameraControlWindow) {
							GuiWindowCameraControls.Render();
						}

						if (ShowPreferencesWindow) {
							GuiWindowPreferences.Render();
						}

						if (ShowAddPolygonWindow) {
							GuiWindowAddPolygon.Render();
						}

						if (ShowPolygonListWindow) {
							GuiWindowPolygonList.Render();
						}

						if (ShowRawTerrainDataWindow) {
							GuiWindowRawTerrainData.Render();
						}

						if (ShowDebugAnimatedMeshWindow &&
						    MapData.MapIsLoaded && CurrentMapState.StateData.MeshAnimationSet != null
						) {
							GuiWindowDebugAnimatedMeshData.Render();
						}

						if (ShowMeshAnimationsWindow) {
							GuiWindowEditMeshAnimations.Render();
						}

						if (ShowExportGlbWindow) {
							GuiWindowExportGlb.Render();
						}

						if (GuiWindowMapWarning.ShouldRender()) {
							GuiWindowMapWarning.Render();
						}

						if (ShowGnsDataWindow) {
							GuiWindowGnsData.Render();
						}
					}

					if (ShowAboutWindow) {
						GuiWindowAbout.Render();
					}

					if (ImGui.GetIO().WantCaptureKeyboard || ImGui.GetIO().WantCaptureMouse) {
						MyraGui.LockModeling = true;
					}
				}
			}
			Stage.ImGuiRenderer.AfterLayout();
		}

		private static void RenderTabPanel() {
			GuiStyle.SetNewUiToDefaultStyle();

			ImGui.GetStyle().FrameRounding = 0;
			ImGui.GetStyle().WindowRounding = 0;
			GuiStyle.SetElementStyle(ElementStyle.WindowNoPadding);
			ImGui.GetStyle().Colors[(int) ImGuiCol.WindowBg] = GuiStyle.ColorPalette[ColorName.Darker];

			ImGui.SetNextWindowSize(new Vector2(GuiStyle.RightPanelWidth, GuiStyle.TabPanelHeight));
			ImGui.SetNextWindowPos(new Vector2(Stage.Width - GuiStyle.RightPanelWidth, GuiStyle.MenuBarHeight));

			ImGui.Begin("Tab Panel", GuiStyle.FixedWindowFlags | ImGuiWindowFlags.NoBringToFrontOnFocus);
			{
				ImGui.GetStyle().ItemSpacing = Vector2.Zero;
				ImGui.Indent();
				GuiStyle.SetElementStyle(SelectedTab == RightPanelTab.Polygon
					? ElementStyle.ButtonTabSelected
					: ElementStyle.ButtonTabUnselected);

				if (ImGui.Button("Polygon")) {
					SelectedTab = RightPanelTab.Polygon;
				}

				ImGui.SameLine();

				GuiStyle.SetElementStyle(SelectedTab == RightPanelTab.Texture
					? ElementStyle.ButtonTabSelected
					: ElementStyle.ButtonTabUnselected);

				if (ImGui.Button("Texture")) {
					SelectedTab = RightPanelTab.Texture;
				}

				ImGui.SameLine();

				GuiStyle.SetElementStyle(SelectedTab == RightPanelTab.Terrain
					? ElementStyle.ButtonTabSelected
					: ElementStyle.ButtonTabUnselected);

				if (ImGui.Button("Terrain")) {
					SelectedTab = RightPanelTab.Terrain;
				}

				ImGui.SameLine();

				GuiStyle.SetElementStyle(SelectedTab == RightPanelTab.Map
					? ElementStyle.ButtonTabSelected
					: ElementStyle.ButtonTabUnselected);

				if (ImGui.Button("Map")) {
					SelectedTab = RightPanelTab.Map;
				}

				ImGui.Unindent();
			}
			ImGui.End();
		}

		private static void RenderMainPanel() {
			const int top = GuiStyle.TabPanelHeight + GuiStyle.MenuBarHeight;

			ImGui.SetNextWindowSize(new Vector2(GuiStyle.RightPanelWidth, Stage.Height - top));
			ImGui.SetNextWindowPos(new Vector2(Stage.Width - GuiStyle.RightPanelWidth, top));

			GuiStyle.SetNewUiToDefaultStyle();
			ImGui.GetStyle().FrameRounding = 0;

			GuiStyle.SetElementStyle(ElementStyle.FixedPanelStyle);

			ImGui.Begin("Main Panel", GuiStyle.FixedWindowFlags);
			{
				switch (SelectedTab) {
					case RightPanelTab.Polygon:
						GuiPanelMeshSelector.Render();
						GuiPanelPolygon.Render();
						GuiPanelTerrain.ResizeTerrainMode = false;
						break;
					case RightPanelTab.Texture:
						GuiPanelMeshSelector.Render();
						GuiPanelTexture.Render();
						GuiPanelTerrain.ResizeTerrainMode = false;
						break;
					case RightPanelTab.Terrain:
						GuiPanelTerrain.Render();
						break;
					case RightPanelTab.Map:
						GuiPanelMap.Render();
						GuiPanelTerrain.ResizeTerrainMode = false;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			ImGui.End();
		}

		public static void ToggleManageResourcesWindow() {
			_showManageResourcesWindow = !_showManageResourcesWindow;
			Stage.FullModelingViewportMode = _showManageResourcesWindow;

			if (_showManageResourcesWindow) {
				Selection.SelectedPolygons.Clear();
				Selection.SelectedTerrainTiles.Clear();
			}
		}
	}
}