using System.Collections.Generic;
using GaneshaDx.Common;
using GaneshaDx.Resources;
using GaneshaDx.Resources.ContentDataTypes.Polygons;
using GaneshaDx.UserInterface.GuiDefinitions;
using ImGuiNET;
using Vector2 = System.Numerics.Vector2;

namespace GaneshaDx.UserInterface.GuiForms {
	public static class GuiWindowPolygonList {
		private static int _selectedIndex = 0;

		public static void Render() {
			bool windowIsOpen = true;
			GuiStyle.SetNewUiToDefaultStyle();
			ImGui.GetStyle().WindowRounding = 4;
			ImGui.GetStyle().FrameRounding = 0;
			ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[2]);
			const ImGuiWindowFlags flags = ImGuiWindowFlags.NoResize |
			                               ImGuiWindowFlags.AlwaysAutoResize |
			                               ImGuiWindowFlags.NoCollapse;

			ImGui.SetNextWindowSize(new Vector2(250, 400));
			ImGui.Begin("Polygon List", ref windowIsOpen, flags);
			{
				ImGui.PopFont();

				List<Polygon> untexturedTriangles =
					CurrentMapState.StateData.PolygonCollection[GuiPanelMesh.SelectedMesh][
						PolygonType.UntexturedTriangle];

				List<Polygon> untexturedQuads =
					CurrentMapState.StateData.PolygonCollection[GuiPanelMesh.SelectedMesh][PolygonType.UntexturedQuad];

				List<Polygon> texturedTriangles =
					CurrentMapState.StateData.PolygonCollection[GuiPanelMesh.SelectedMesh][
						PolygonType.TexturedTriangle];

				List<Polygon> texturedQuads =
					CurrentMapState.StateData.PolygonCollection[GuiPanelMesh.SelectedMesh][PolygonType.TexturedQuad];


				GuiStyle.SetNewUiToDefaultStyle();
				GuiStyle.SetElementStyle(ElementStyle.Header);

				if (ImGui.CollapsingHeader("Untextured Triangles (" + untexturedTriangles.Count + ")")) {
					GuiStyle.SetNewUiToDefaultStyle();
					ImGui.Indent();
					for (int index = 0; index < untexturedTriangles.Count; index++) {
						Polygon polygon = untexturedTriangles[index];
						if (ImGui.Button("Untextured Triangle " + index + "##" + "untexturedTriangle" + index)) {
							Selection.SelectPolygon(polygon);
						}
					}

					ImGui.Unindent();
				}

				GuiStyle.SetNewUiToDefaultStyle();
				GuiStyle.SetElementStyle(ElementStyle.Header);
				if (ImGui.CollapsingHeader("Textured Triangles (" + texturedTriangles.Count + ")")) {
					GuiStyle.SetNewUiToDefaultStyle();
					ImGui.Indent();
					for (int index = 0; index < texturedTriangles.Count; index++) {
						Polygon polygon = texturedTriangles[index];
						if (ImGui.Button("Textured Triangle " + index + "##" + "texturedTriangle" + index)) {
							Selection.SelectPolygon(polygon);
						}
					}

					ImGui.Unindent();
				}

				GuiStyle.SetNewUiToDefaultStyle();
				GuiStyle.SetElementStyle(ElementStyle.Header);
				if (ImGui.CollapsingHeader("Untextured Quads (" + untexturedQuads.Count + ")")) {
					GuiStyle.SetNewUiToDefaultStyle();
					ImGui.Indent();
					for (int index = 0; index < untexturedQuads.Count; index++) {
						Polygon polygon = untexturedQuads[index];
						if (ImGui.Button("Untextured Quad" + index + "##" + "untexturedQuad" + index)) {
							Selection.SelectPolygon(polygon);
						}
					}

					ImGui.Unindent();
				}

				GuiStyle.SetNewUiToDefaultStyle();
				GuiStyle.SetElementStyle(ElementStyle.Header);
				if (ImGui.CollapsingHeader("Textured Quads (" + texturedQuads.Count + ")")) {
					GuiStyle.SetNewUiToDefaultStyle();
					ImGui.Indent();
					for (int index = 0; index < texturedQuads.Count; index++) {
						Polygon polygon = texturedQuads[index];
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
}