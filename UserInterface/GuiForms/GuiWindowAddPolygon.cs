using System.Collections.Generic;
using GaneshaDx.Common;
using GaneshaDx.Environment;
using GaneshaDx.Resources;
using GaneshaDx.Resources.ContentDataTypes.Polygons;
using GaneshaDx.UserInterface.GuiDefinitions;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Vector2 = System.Numerics.Vector2;

namespace GaneshaDx.UserInterface.GuiForms;

public static class GuiWindowAddPolygon {
	private static bool _newPolygonIsQuad = true;
	private static bool _newPolygonIsTextured = true;

	public static void Render() {
		bool windowIsOpen = true;

		GuiStyle.SetNewUiToDefaultStyle();
		ImGui.GetStyle().WindowRounding = 3;
		ImGui.GetStyle().FrameRounding = 0;

		GuiStyle.SetFont(Fonts.Large);

		const ImGuiWindowFlags flags = ImGuiWindowFlags.NoResize |
		                               ImGuiWindowFlags.AlwaysAutoResize |
		                               ImGuiWindowFlags.NoCollapse;

		ImGui.Begin("Create New Polygon", ref windowIsOpen, flags);
		{
			GuiStyle.SetFont(Fonts.Default);
			ImGui.Columns(2, "AddNewPolygonColumns", false);
			ImGui.SetColumnWidth(0, GuiStyle.LabelWidth + 10);
			ImGui.SetColumnWidth(1, GuiStyle.WidgetWidth + 10);

			ImGui.Text("Is Quad?");
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
			ImGui.Checkbox("###NewPolygonIsQuad", ref _newPolygonIsQuad);
			ImGui.NextColumn();

			ImGui.Text("Is Textured?");
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
			ImGui.Checkbox("###NewPolygonIsTextured", ref _newPolygonIsTextured);
			ImGui.NextColumn();

			ImGui.Columns(1);

			GuiStyle.AddSpace();

			const int addPolygonButtonWidth = GuiStyle.LabelWidth + 10 + GuiStyle.WidgetWidth;
			const int addPolygonButtonHeight = 20;

			if (ImGui.Button("Add Polygon", new Vector2(addPolygonButtonWidth, addPolygonButtonHeight))) {
				Vector3 direction = StageCamera.CamPosition - StageCamera.CamTarget;
				direction.Normalize();
				Vector3 spawnPoint = StageCamera.CamTarget + direction * 50;

				List<Vertex> vertices = new() {
					new Vertex(spawnPoint, Color.Red, true, 0, 90),
					new Vertex(spawnPoint + new Vector3(28, 0, 0), Color.Green, true, 0, 90),
					new Vertex(spawnPoint + new Vector3(0, 0, 28), Color.Blue, true, 0, 90)
				};


				if (_newPolygonIsQuad) {
					vertices.Add(
						new Vertex(spawnPoint + new Vector3(28, 0, 28), Color.Yellow, true, 0, 90)
					);
				}

				List<Microsoft.Xna.Framework.Vector2> uvs = _newPolygonIsTextured
					? new List<Microsoft.Xna.Framework.Vector2> {
						new(9, 9),
						new(27, 9),
						new(9, 27)
					}
					: new List<Microsoft.Xna.Framework.Vector2>();

				if (_newPolygonIsQuad && _newPolygonIsTextured) {
					uvs.Add(new Vector2(27, 27));
				}

				CurrentMapState.CreatePolygon(vertices, uvs, GuiPanelMeshSelector.SelectedMesh);
			}
		}
		ImGui.End();

		if (!windowIsOpen) {
			Gui.ShowAddPolygonWindow = false;
		}
	}
}