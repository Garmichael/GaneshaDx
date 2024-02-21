using System.Collections.Generic;
using GaneshaDx.Common;
using GaneshaDx.Rendering;
using GaneshaDx.Resources;
using GaneshaDx.Resources.ContentDataTypes;
using GaneshaDx.Resources.ContentDataTypes.Polygons;
using GaneshaDx.Resources.ResourceContent;
using GaneshaDx.UserInterface.GuiDefinitions;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Vector3 = System.Numerics.Vector3;

namespace GaneshaDx.UserInterface.GuiForms {
	public static class GuiPanelMap {
		private static MapStateData StateData => CurrentMapState.StateData;

		public static void Render() {
			RenderCurrentStateSection();
			RenderBackgroundColorsSection();
			RenderLightingSection();
			if (Configuration.Properties.ShowUnknownValues) {
				RenderUnknownEndOfBackgroundData();
			}
			RenderStatsSection();
		}

		private static void RenderCurrentStateSection() {
			GuiStyle.SetNewUiToDefaultStyle();
			GuiStyle.SetElementStyle(ElementStyle.Header);

			if (ImGui.CollapsingHeader("Current State", ImGuiTreeNodeFlags.DefaultOpen)) {
				ImGui.Indent();
				GuiStyle.SetNewUiToDefaultStyle();

				ImGui.Columns(2, "CurrentStateColumns", false);
				ImGui.SetColumnWidth(0, GuiStyle.LabelWidth);
				ImGui.SetColumnWidth(1, GuiStyle.WidgetWidth + 10);

				bool reloadMapState = false;

				{
					ImGui.PushItemWidth(GuiStyle.LabelWidth);
					ImGui.Text("Arrangement");
					ImGui.PopItemWidth();

					ImGui.NextColumn();
				}

				{
					int comboSelection = (int) CurrentMapState.StateData.MapArrangementState;
					int before = comboSelection;

					ImGui.PushItemWidth(GuiStyle.WidgetWidth);
					string[] selection = {"Primary", "Secondary"};
					ImGui.Combo("MapArrangement", ref comboSelection, selection, 2);
					ImGui.PopItemWidth();
					CurrentMapState.StateData.MapArrangementState = (MapArrangementState) comboSelection;

					if (before != comboSelection) {
						reloadMapState = true;
					}

					ImGui.NextColumn();
				}


				{
					ImGui.PushItemWidth(GuiStyle.LabelWidth);
					ImGui.Text("Time");
					ImGui.PopItemWidth();

					ImGui.NextColumn();
				}

				{
					int comboSelection = (int) CurrentMapState.StateData.MapTime;
					int before = comboSelection;

					ImGui.PushItemWidth(GuiStyle.WidgetWidth);
					string[] selection = {"Day", "Night"};
					ImGui.Combo("MapTime", ref comboSelection, selection, 2);
					ImGui.PopItemWidth();
					CurrentMapState.StateData.MapTime = (MapTime) comboSelection;

					if (before != comboSelection) {
						reloadMapState = true;
					}

					ImGui.NextColumn();
				}


				{
					ImGui.PushItemWidth(GuiStyle.LabelWidth);
					ImGui.Text("Weather");
					ImGui.PopItemWidth();

					GuiStyle.AddSpace();
					ImGui.NextColumn();
				}

				{
					int comboSelection = (int) CurrentMapState.StateData.MapWeather;
					int before = comboSelection;

					ImGui.PushItemWidth(GuiStyle.WidgetWidth);
					string[] selection = {"None", "None Alt", "Normal", "Strong", "Heavy"};
					ImGui.Combo("MapWeather", ref comboSelection, selection, 5);
					ImGui.PopItemWidth();
					CurrentMapState.StateData.MapWeather = (MapWeather) comboSelection;

					if (before != comboSelection) {
						reloadMapState = true;
					}

					GuiStyle.AddSpace();
					ImGui.NextColumn();
				}

				if (reloadMapState) {
					CurrentMapState.SetState(
						CurrentMapState.StateData.MapArrangementState,
						CurrentMapState.StateData.MapTime,
						CurrentMapState.StateData.MapWeather
					);
				}

				{
					ImGui.PushItemWidth(GuiStyle.LabelWidth);
					ImGui.Text("Texture File");
					ImGui.PopItemWidth();

					ImGui.NextColumn();
				}
				{
					ImGui.PushItemWidth(GuiStyle.LabelWidth);
					ImGui.Text(MapData.MapName + "." + CurrentMapState.StateData.StateTextureResource.XFile);
					ImGui.PopItemWidth();

					ImGui.NextColumn();
				}

				List<string> meshFilenames = new List<string>();
				foreach (MapResource meshResource in CurrentMapState.StateData.StateMeshResources) {
					meshFilenames.Add(MapData.MapName + "." + meshResource.XFile);
				}

				{
					ImGui.PushItemWidth(GuiStyle.LabelWidth);
					ImGui.Text("Mesh File" + (meshFilenames.Count > 1 ? "s" : ""));
					ImGui.PopItemWidth();

					ImGui.NextColumn();
				}
				
				{
					ImGui.PushItemWidth(GuiStyle.LabelWidth);
					ImGui.Text(string.Join("\n", meshFilenames));
					ImGui.PopItemWidth();

					ImGui.NextColumn();
				}

				ImGui.Columns(1);
				
				GuiStyle.AddSpace();

				if (ImGui.Button("Manage Map Resources")) {
					Gui.ToggleManageResourcesWindow();
				}

				ImGui.Unindent();
				GuiStyle.AddSpace();
			}
		}

		private static void RenderBackgroundColorsSection() {
			GuiStyle.SetNewUiToDefaultStyle();
			GuiStyle.SetElementStyle(ElementStyle.Header);

			if (ImGui.CollapsingHeader("Background Colors", ImGuiTreeNodeFlags.DefaultOpen)) {
				GuiStyle.SetNewUiToDefaultStyle();
				ImGui.Indent();

				ImGui.Columns(2, "BackgroundColors", false);
				ImGui.SetColumnWidth(0, GuiStyle.LabelWidth);
				ImGui.SetColumnWidth(1, GuiStyle.WidgetWidth + 10);

				ImGui.PushItemWidth(GuiStyle.LabelWidth);
				ImGui.Text("Top Color");
				ImGui.PopItemWidth();

				ImGui.NextColumn();

				Vector3 topColor = Utilities.ConvertVector3(StateData.BackgroundTopColor.ToVector4());
				ImGui.ColorEdit3("Background Top Color", ref topColor, GuiStyle.ColorBoxFlags);

				ImGui.NextColumn();

				ImGui.PushItemWidth(GuiStyle.LabelWidth);
				ImGui.Text("Bottom Color");
				ImGui.PopItemWidth();

				ImGui.NextColumn();

				Vector3 botColor = Utilities.ConvertVector3(StateData.BackgroundBottomColor.ToVector4());
				ImGui.ColorEdit3("Background Bottom Color", ref botColor, GuiStyle.ColorBoxFlags);

				ImGui.Columns(1);
				ImGui.Unindent();

				GuiStyle.AddSpace();
				bool backgroundChanged =
					StateData.BackgroundTopColor.R != (int) (topColor.X * 255) ||
					StateData.BackgroundTopColor.G != (int) (topColor.Y * 255) ||
					StateData.BackgroundTopColor.B != (int) (topColor.Z * 255) ||
					StateData.BackgroundBottomColor.R != (int) (botColor.X * 255) ||
					StateData.BackgroundBottomColor.G != (int) (botColor.Y * 255) ||
					StateData.BackgroundBottomColor.B != (int) (botColor.Z * 255);

				if (backgroundChanged) {
					StateData.BackgroundTopColor = new Color(topColor.X, topColor.Y, topColor.Z, 255);
					StateData.BackgroundBottomColor = new Color(botColor.X, botColor.Y, botColor.Z, 255);

					Background.SetAsGradient(
						StateData.BackgroundTopColor,
						StateData.BackgroundBottomColor
					);
				}
			}

			GuiStyle.SetNewUiToDefaultStyle();
		}

		private static void RenderLightingSection() {
			GuiStyle.SetNewUiToDefaultStyle();
			GuiStyle.SetElementStyle(ElementStyle.Header);

			if (ImGui.CollapsingHeader("Lighting", ImGuiTreeNodeFlags.DefaultOpen)) {
				GuiStyle.SetNewUiToDefaultStyle();
				ImGui.Indent();

				List<DirectionalLight> directionalLights = StateData.DirectionalLights;

				ImGui.Columns(2, "Lighting", false);
				{
					ImGui.SetColumnWidth(0, GuiStyle.LabelWidth);
					ImGui.SetColumnWidth(1, GuiStyle.WidgetWidth + 10);
					
					ImGui.PushItemWidth(GuiStyle.LabelWidth);
					ImGui.Text("Ambient Color");
					ImGui.PopItemWidth();

					ImGui.NextColumn();

					Vector3 lightColor = Utilities.ConvertVector3(StateData.AmbientLightColor.ToVector4());
					ImGui.ColorEdit3("Ambient Color", ref lightColor, GuiStyle.ColorBoxFlags);
					StateData.AmbientLightColor = new Color(lightColor.X, lightColor.Y, lightColor.Z);

					ImGui.NextColumn();

					GuiStyle.AddSpace();
					ImGui.PushItemWidth(GuiStyle.LabelWidth);
					ImGui.Text("Light 1 Color");
					ImGui.PopItemWidth();

					ImGui.NextColumn();

					GuiStyle.AddSpace();
					lightColor = Utilities.ConvertVector3(directionalLights[0].LightColor.ToVector4());
					ImGui.ColorEdit3("Light1Color", ref lightColor, GuiStyle.ColorBoxFlags);
					directionalLights[0].LightColor = new Color(lightColor.X, lightColor.Y, lightColor.Z);

					ImGui.NextColumn();

					ImGui.PushItemWidth(GuiStyle.LabelWidth);
					ImGui.Text("Light 1 Elevation");
					ImGui.PopItemWidth();

					ImGui.NextColumn();

					float elevation = (float) directionalLights[0].DirectionElevation;
					ImGui.PushItemWidth(GuiStyle.WidgetWidth);
					ImGui.SliderFloat("Light1Elevation", ref elevation, -90, 90);
					ImGui.PopItemWidth();
					directionalLights[0].DirectionElevation = elevation;

					ImGui.NextColumn();

					ImGui.PushItemWidth(GuiStyle.LabelWidth);
					ImGui.Text("Light 1 Azimuth");
					ImGui.PopItemWidth();

					ImGui.NextColumn();

					float azimuth = (float) directionalLights[0].DirectionAzimuth;
					ImGui.PushItemWidth(GuiStyle.WidgetWidth);
					ImGui.SliderFloat("Light1Azimuth", ref azimuth, 0, 360);
					ImGui.PopItemWidth();
					directionalLights[0].DirectionAzimuth = azimuth;

					ImGui.NextColumn();

					GuiStyle.AddSpace();
					ImGui.PushItemWidth(GuiStyle.LabelWidth);
					ImGui.Text("Light 2 Color");
					ImGui.PopItemWidth();

					ImGui.NextColumn();

					GuiStyle.AddSpace();
					lightColor = Utilities.ConvertVector3(directionalLights[1].LightColor.ToVector4());
					ImGui.ColorEdit3("Light2Color", ref lightColor, GuiStyle.ColorBoxFlags);
					directionalLights[1].LightColor = new Color(lightColor.X, lightColor.Y, lightColor.Z);

					ImGui.NextColumn();

					ImGui.PushItemWidth(GuiStyle.LabelWidth);
					ImGui.Text("Light 2 Elevation");
					ImGui.PopItemWidth();

					ImGui.NextColumn();

					elevation = (float) directionalLights[1].DirectionElevation;
					ImGui.PushItemWidth(GuiStyle.WidgetWidth);
					ImGui.SliderFloat("Light2Elevation", ref elevation, -90, 90);
					ImGui.PopItemWidth();
					directionalLights[1].DirectionElevation = elevation;

					ImGui.NextColumn();

					ImGui.PushItemWidth(GuiStyle.LabelWidth);
					ImGui.Text("Light 2 Azimuth");
					ImGui.PopItemWidth();

					ImGui.NextColumn();

					azimuth = (float) directionalLights[1].DirectionAzimuth;
					ImGui.PushItemWidth(GuiStyle.WidgetWidth);
					ImGui.SliderFloat("Light2Azimuth", ref azimuth, 0, 360);
					ImGui.PopItemWidth();
					directionalLights[1].DirectionAzimuth = azimuth;

					ImGui.NextColumn();

					GuiStyle.AddSpace();
					ImGui.PushItemWidth(GuiStyle.LabelWidth);
					ImGui.Text("Light 3 Color");
					ImGui.PopItemWidth();

					ImGui.NextColumn();

					GuiStyle.AddSpace();
					lightColor = Utilities.ConvertVector3(directionalLights[2].LightColor.ToVector4());
					ImGui.ColorEdit3("Light3Color", ref lightColor, GuiStyle.ColorBoxFlags);
					directionalLights[2].LightColor = new Color(lightColor.X, lightColor.Y, lightColor.Z);

					ImGui.NextColumn();

					ImGui.PushItemWidth(GuiStyle.LabelWidth);
					ImGui.Text("Light 3 Elevation");
					ImGui.PopItemWidth();

					ImGui.NextColumn();

					elevation = (float) directionalLights[2].DirectionElevation;
					ImGui.PushItemWidth(GuiStyle.WidgetWidth);
					ImGui.SliderFloat("Light3Elevation", ref elevation, -90, 90);
					ImGui.PopItemWidth();
					directionalLights[2].DirectionElevation = elevation;

					ImGui.NextColumn();

					ImGui.PushItemWidth(GuiStyle.LabelWidth);
					ImGui.Text("Light 3 Azimuth");
					ImGui.PopItemWidth();

					ImGui.NextColumn();

					azimuth = (float) directionalLights[2].DirectionAzimuth;
					ImGui.PushItemWidth(GuiStyle.WidgetWidth);
					ImGui.SliderFloat("Light3Azimuth", ref azimuth, 0, 360);
					ImGui.PopItemWidth();
					directionalLights[2].DirectionAzimuth = azimuth;
				}

				ImGui.Columns(1);
				ImGui.Unindent();
				GuiStyle.AddSpace();
			}
		}

		private static void RenderUnknownEndOfBackgroundData() {
			GuiStyle.SetNewUiToDefaultStyle();
			GuiStyle.SetElementStyle(ElementStyle.Header);
			
			if (ImGui.CollapsingHeader("Unknown Data", ImGuiTreeNodeFlags.DefaultOpen)) {
				GuiStyle.SetNewUiToDefaultStyle();
				ImGui.Indent();
				ImGui.Columns(2, "Unknown Data", false);
				ImGui.SetColumnWidth(0, GuiStyle.LabelWidth);
				ImGui.SetColumnWidth(1, GuiStyle.WidgetWidth + 10);
				
				ImGui.Text("Has Post-Poly Padding");
				ImGui.NextColumn();
				bool usesEndOfPolygonPadding = CurrentMapState.StateData.UsesEndOfPolygonPadding[GuiPanelMeshSelector.SelectedMesh];
				ImGui.Checkbox("##UsesPostPolygonPadding", ref usesEndOfPolygonPadding);
				CurrentMapState.StateData.UsesEndOfPolygonPadding[GuiPanelMeshSelector.SelectedMesh] = usesEndOfPolygonPadding;
				ImGui.NextColumn();
				
				ImGui.Text("Post-Poly Bytes");
				ImGui.NextColumn();
				
				int postPolyByteA = CurrentMapState.StateData.EndOfPolygonPadding[GuiPanelMeshSelector.SelectedMesh][0];
				int postPolyByteB = CurrentMapState.StateData.EndOfPolygonPadding[GuiPanelMeshSelector.SelectedMesh][1];
				
				ImGui.InputInt("##PolyPolyByteA", ref postPolyByteA);
				ImGui.InputInt("##PolyPolyByteB", ref postPolyByteB);
				
				CurrentMapState.StateData.EndOfPolygonPadding[GuiPanelMeshSelector.SelectedMesh][0] = (byte) postPolyByteA;
				CurrentMapState.StateData.EndOfPolygonPadding[GuiPanelMeshSelector.SelectedMesh][1] = (byte) postPolyByteB;
				
				ImGui.NextColumn();
				
				GuiStyle.AddSpace();
				ImGui.Text("Has Post-Background");
				ImGui.Text("Padding");
				ImGui.NextColumn();
				GuiStyle.AddSpace();
				bool usesEndOfBackgroundColorPadding = CurrentMapState.StateData.UsesEndOfBackgroundColorPadding;
				ImGui.Checkbox("##UsesPostBackgroundPadding", ref usesEndOfBackgroundColorPadding);
				CurrentMapState.StateData.UsesEndOfBackgroundColorPadding = usesEndOfBackgroundColorPadding;
				ImGui.NextColumn();
				
				ImGui.Text("Post-Background Bytes");
				ImGui.NextColumn();
				int postBackgroundByteA = CurrentMapState.StateData.EndOfBackgroundColorPadding[0];
				int postBackgroundByteB = CurrentMapState.StateData.EndOfBackgroundColorPadding[1];
				int postBackgroundByteC = CurrentMapState.StateData.EndOfBackgroundColorPadding[2];
				
				ImGui.InputInt("##PostBackgroundByteA", ref postBackgroundByteA);
				ImGui.InputInt("##PostBackgroundByteB", ref postBackgroundByteB);
				ImGui.InputInt("##PostBackgroundByteC", ref postBackgroundByteC);

				CurrentMapState.StateData.EndOfBackgroundColorPadding[0] = (byte) postBackgroundByteA;
				CurrentMapState.StateData.EndOfBackgroundColorPadding[1] = (byte) postBackgroundByteB;
				CurrentMapState.StateData.EndOfBackgroundColorPadding[2] = (byte) postBackgroundByteC;
				
				ImGui.NextColumn();
				
				GuiStyle.AddSpace();
				ImGui.Text("Has Post-Terrain");
				ImGui.Text("Padding");
				ImGui.NextColumn();
				GuiStyle.AddSpace();
				bool usesEndOfTerrainPadding = CurrentMapState.StateData.UsesEndOfTerrainPadding;
				ImGui.Checkbox("##UsesPostTerrainPadding", ref usesEndOfTerrainPadding);
				CurrentMapState.StateData.UsesEndOfTerrainPadding = usesEndOfTerrainPadding;
				ImGui.NextColumn();
				
				ImGui.Text("Post-Terrain Bytes");
				ImGui.NextColumn();
				int postTerrainByteA = CurrentMapState.StateData.EndOfTerrainPadding[0];
				int postTerrainByteB = CurrentMapState.StateData.EndOfTerrainPadding[1];
				
				ImGui.InputInt("##PostTerrainByteA", ref postTerrainByteA);
				ImGui.InputInt("##PostTerrainByteB", ref postTerrainByteB);

				CurrentMapState.StateData.EndOfTerrainPadding[0] = (byte) postTerrainByteA;
				CurrentMapState.StateData.EndOfTerrainPadding[1] = (byte) postTerrainByteB;
				
				ImGui.NextColumn();
				
				ImGui.Columns(1);
				ImGui.Unindent();
				GuiStyle.AddSpace();
			}
		}
		
		private static void RenderStatsSection() {
			const int valueColumnWidth = 30;
			GuiStyle.SetNewUiToDefaultStyle();
			GuiStyle.SetElementStyle(ElementStyle.Header);

			if (ImGui.CollapsingHeader("Polygon Counts", ImGuiTreeNodeFlags.DefaultOpen)) {
				GuiStyle.SetNewUiToDefaultStyle();
				ImGui.Indent();
				ImGui.Columns(3, "PolygonCounts", false);
				ImGui.SetColumnWidth(0, GuiStyle.LabelWidth);
				ImGui.SetColumnWidth(1, valueColumnWidth);
				ImGui.SetColumnWidth(2, valueColumnWidth * 2);

				{
					ImGui.PushItemWidth(GuiStyle.LabelWidth);
					ImGui.Text("Untextured Triangles");
					ImGui.PopItemWidth();

					ImGui.NextColumn();

					ImGui.PushItemWidth(valueColumnWidth);
					ImGui.Text(Utilities.GetTotalPolygonCount(PolygonType.UntexturedTriangle).ToString());
					ImGui.PopItemWidth();

					ImGui.NextColumn();

					ImGui.PushItemWidth(valueColumnWidth * 2);
					ImGui.Text("of    64");
					ImGui.PopItemWidth();

					ImGui.NextColumn();
				}

				{
					ImGui.PushItemWidth(GuiStyle.LabelWidth);
					ImGui.Text("Untextured Quads");
					ImGui.PopItemWidth();

					ImGui.NextColumn();

					ImGui.PushItemWidth(valueColumnWidth);
					ImGui.Text(Utilities.GetTotalPolygonCount(PolygonType.UntexturedQuad).ToString());

					ImGui.PopItemWidth();
					ImGui.NextColumn();

					ImGui.PushItemWidth(valueColumnWidth * 2);
					ImGui.Text("of    256");
					ImGui.PopItemWidth();

					ImGui.NextColumn();
				}

				{
					ImGui.PushItemWidth(GuiStyle.LabelWidth);
					ImGui.Text("Textured Triangles");
					ImGui.PopItemWidth();

					ImGui.NextColumn();

					ImGui.PushItemWidth(valueColumnWidth);
					ImGui.Text(Utilities.GetTotalPolygonCount(PolygonType.TexturedTriangle).ToString());
					ImGui.PopItemWidth();

					ImGui.NextColumn();

					ImGui.PushItemWidth(valueColumnWidth * 2);
					ImGui.Text("of    360");
					ImGui.PopItemWidth();

					ImGui.NextColumn();
				}

				{
					ImGui.PushItemWidth(GuiStyle.LabelWidth);
					ImGui.Text("Textured Quads");
					ImGui.PopItemWidth();

					ImGui.NextColumn();

					ImGui.PushItemWidth(valueColumnWidth);
					ImGui.Text(Utilities.GetTotalPolygonCount(PolygonType.TexturedQuad).ToString());
					ImGui.PopItemWidth();

					ImGui.NextColumn();

					ImGui.PushItemWidth(valueColumnWidth * 2);
					ImGui.Text("of    710");
					ImGui.PopItemWidth();
				}

				ImGui.Columns(1);
				ImGui.Unindent();
			}
		}
	}
}