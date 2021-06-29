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
		public static RightPanelTab SelectedSubTab = RightPanelTab.Polygon;
		public static WidgetSelectionMode WidgetSelectionMode = WidgetSelectionMode.PolygonTranslate;

		private static bool _showDebugPanel;
		public static bool ShowCameraControlWindow;
		public static bool ShowPreferencesWindow;
		public static bool ShowAddPolygonWindow;
		private static bool _showManageResourcesWindow;
		public static bool ShowTipsWindow;
		public static bool ShowDebugAnimatedMeshWindow;

		public static void Render() {
			Stage.GraphicsDevice.Clear(Color.Transparent);

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
				} else {
					GuiMenuBar.Render();
					GuiWindowTexturePreview.Render();

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

						if (ShowTipsWindow) {
							GuiWindowTips.Render();
						}

						if (ShowDebugAnimatedMeshWindow &&
						    MapData.MapIsLoaded && CurrentMapState.StateData.MeshAnimationInstructions != null
						) {
							GuiWindowDebugAnimatedMeshData.Render();
						}
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

			bool twoRows = SelectedTab == RightPanelTab.Mesh;
			ImGui.SetNextWindowSize(new Vector2(GuiStyle.RightPanelWidth, GuiStyle.TabPanelHeight * (twoRows ? 2 : 1)));
			ImGui.SetNextWindowPos(new Vector2(Stage.Width - GuiStyle.RightPanelWidth, GuiStyle.MenuBarHeight));

			ImGui.Begin("Tab Panel", GuiStyle.FixedWindowFlags | ImGuiWindowFlags.NoBringToFrontOnFocus);
			{
				ImGui.GetStyle().ItemSpacing = Vector2.Zero;
				ImGui.Indent();
				GuiStyle.SetElementStyle(SelectedTab == RightPanelTab.Mesh
					? ElementStyle.ButtonTabSelected
					: ElementStyle.ButtonTabUnselected);

				if (ImGui.Button("Mesh")) {
					SelectedTab = RightPanelTab.Mesh;
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

				if (SelectedTab == RightPanelTab.Mesh) {
					GuiStyle.SetElementStyle(SelectedSubTab == RightPanelTab.Polygon
						? ElementStyle.ButtonTabSelected
						: ElementStyle.ButtonTabUnselected);

					if (ImGui.Button("Polygon")) {
						SelectedSubTab = RightPanelTab.Polygon;
					}

					ImGui.SameLine();

					GuiStyle.SetElementStyle(SelectedSubTab == RightPanelTab.Texture
						? ElementStyle.ButtonTabSelected
						: ElementStyle.ButtonTabUnselected);

					if (ImGui.Button("Texture")) {
						SelectedSubTab = RightPanelTab.Texture;
					}
				}

				ImGui.Unindent();
			}
			ImGui.End();
		}

		private static void RenderMainPanel() {
			bool tabPanelTwoRows = SelectedTab == RightPanelTab.Mesh;
			int top = GuiStyle.TabPanelHeight * (tabPanelTwoRows ? 2 : 1) + GuiStyle.MenuBarHeight;

			ImGui.SetNextWindowSize(new Vector2(GuiStyle.RightPanelWidth, Stage.Height - top));
			ImGui.SetNextWindowPos(new Vector2(Stage.Width - GuiStyle.RightPanelWidth, top));

			GuiStyle.SetNewUiToDefaultStyle();
			ImGui.GetStyle().FrameRounding = 0;

			GuiStyle.SetElementStyle(ElementStyle.FixedPanelStyle);

			ImGui.Begin("Main Panel", GuiStyle.FixedWindowFlags);
			{
				switch (SelectedTab) {
					case RightPanelTab.Mesh:

						if (SelectedSubTab == RightPanelTab.Polygon) {
							GuiPanelPolygon.Render();
						} else if (SelectedSubTab == RightPanelTab.Texture) {
							GuiPanelTexture.Render();
						}

						break;
					case RightPanelTab.Terrain:
						GuiPanelTerrain.Render();
						break;
					case RightPanelTab.Map:
						GuiPanelMap.Render();
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