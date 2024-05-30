using System.Collections.Generic;
using GaneshaDx.Common;
using GaneshaDx.Resources;
using GaneshaDx.Resources.ContentDataTypes.Polygons;
using GaneshaDx.UserInterface.GuiDefinitions;
using ImGuiNET;
using Vector2 = System.Numerics.Vector2;

namespace GaneshaDx.UserInterface.GuiForms;

public static class GuiWindowPolygonList {
	public static void Render() {
		bool windowIsOpen = true;
		GuiStyle.SetNewUiToDefaultStyle();
		ImGui.GetStyle().WindowRounding = 4;
		ImGui.GetStyle().FrameRounding = 0;
		GuiStyle.SetFont(Fonts.Large);
		const ImGuiWindowFlags flags = ImGuiWindowFlags.NoResize |
		                               ImGuiWindowFlags.AlwaysAutoResize |
		                               ImGuiWindowFlags.NoCollapse;

		ImGui.SetNextWindowSize(new Vector2(250, 400));
		ImGui.Begin("Polygon List", ref windowIsOpen, flags);
		{
			GuiStyle.SetFont(Fonts.Default);

			List<Polygon> untexturedTriangles = CurrentMapState.StateData.PolygonCollection
				[GuiPanelMeshSelector.SelectedMesh][PolygonType.UntexturedTriangle];

			List<Polygon> untexturedQuads = CurrentMapState.StateData.PolygonCollection
				[GuiPanelMeshSelector.SelectedMesh][PolygonType.UntexturedQuad];

			List<Polygon> texturedTriangles = CurrentMapState.StateData.PolygonCollection
				[GuiPanelMeshSelector.SelectedMesh][PolygonType.TexturedTriangle];

			List<Polygon> texturedQuads = CurrentMapState.StateData.PolygonCollection
				[GuiPanelMeshSelector.SelectedMesh][PolygonType.TexturedQuad];

			GuiStyle.SetNewUiToDefaultStyle();
			GuiStyle.SetElementStyle(ElementStyle.Header);

			if (ImGui.CollapsingHeader("Untextured Triangles (" + untexturedTriangles.Count + " / 64)")) {
				ImGui.Indent();
				for (int index = 0; index < untexturedTriangles.Count; index++) {
					GuiStyle.SetNewUiToDefaultStyle();
					Polygon polygon = untexturedTriangles[index];

					if (Selection.SelectedPolygons.Contains(polygon)) {
						GuiStyle.SetElementStyle(ElementStyle.ButtonSelected);
					}

					if (ImGui.Button("Untextured Triangle " + index + "##" + "untexturedTriangle" + index)) {
						Selection.SelectPolygon(polygon);
					}
				}

				ImGui.Unindent();
			}

			GuiStyle.SetNewUiToDefaultStyle();
			GuiStyle.SetElementStyle(ElementStyle.Header);
			if (ImGui.CollapsingHeader("Untextured Quads (" + untexturedQuads.Count + " / 256)")) {
				GuiStyle.SetNewUiToDefaultStyle();
				ImGui.Indent();
				for (int index = 0; index < untexturedQuads.Count; index++) {
					GuiStyle.SetNewUiToDefaultStyle();
					Polygon polygon = untexturedQuads[index];

					if (Selection.SelectedPolygons.Contains(polygon)) {
						GuiStyle.SetElementStyle(ElementStyle.ButtonSelected);
					}

					if (ImGui.Button("Untextured Quad" + index + "##" + "untexturedQuad" + index)) {
						Selection.SelectPolygon(polygon);
					}
				}

				ImGui.Unindent();
			}

			GuiStyle.SetNewUiToDefaultStyle();
			GuiStyle.SetElementStyle(ElementStyle.Header);
			if (ImGui.CollapsingHeader("Textured Triangles (" + texturedTriangles.Count + " / 360)")) {
				GuiStyle.SetNewUiToDefaultStyle();
				ImGui.Indent();
				for (int index = 0; index < texturedTriangles.Count; index++) {
					GuiStyle.SetNewUiToDefaultStyle();
					Polygon polygon = texturedTriangles[index];

					if (Selection.SelectedPolygons.Contains(polygon)) {
						GuiStyle.SetElementStyle(ElementStyle.ButtonSelected);
					}

					if (ImGui.Button("Textured Triangle " + index + "##" + "texturedTriangle" + index)) {
						Selection.SelectPolygon(polygon);
					}
				}

				ImGui.Unindent();
			}

			GuiStyle.SetNewUiToDefaultStyle();
			GuiStyle.SetElementStyle(ElementStyle.Header);
			if (ImGui.CollapsingHeader("Textured Quads (" + texturedQuads.Count + " / 710)")) {
				ImGui.Indent();
				for (int index = 0; index < texturedQuads.Count; index++) {
					GuiStyle.SetNewUiToDefaultStyle();
					Polygon polygon = texturedQuads[index];

					if (Selection.SelectedPolygons.Contains(polygon)) {
						GuiStyle.SetElementStyle(ElementStyle.ButtonSelected);
					}

					if (ImGui.Button("Textured Quad " + index + "##" + "texturedQuad" + index)) {
						Selection.SelectPolygon(polygon);
					}
				}

				ImGui.Unindent();
			}


			ImGui.Columns(1);
		}
		ImGui.End();

		if (!windowIsOpen) {
			Gui.ShowPolygonListWindow = false;
		}
	}
}