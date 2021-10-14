using System.Collections.Generic;
using GaneshaDx.Common;
using GaneshaDx.Environment;
using GaneshaDx.Resources;
using GaneshaDx.Resources.ContentDataTypes.Polygons;
using GaneshaDx.UserInterface.GuiDefinitions;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Vector2 = System.Numerics.Vector2;

namespace GaneshaDx.UserInterface.GuiForms {
	public static class GuiWindowAddPolygon {
		private static bool _newPolygonIsQuad = true;
		private static bool _newPolygonIsTextured = true;

		public static void Render() {
			bool windowIsOpen = true;

			GuiStyle.SetNewUiToDefaultStyle();
			ImGui.GetStyle().WindowRounding = 3;
			ImGui.GetStyle().FrameRounding = 0;
			ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[2]);

			const ImGuiWindowFlags flags = ImGuiWindowFlags.NoResize |
			                               ImGuiWindowFlags.AlwaysAutoResize |
			                               ImGuiWindowFlags.NoCollapse;

			ImGui.Begin("Add New Polygon", ref windowIsOpen, flags);
			{
				ImGui.PopFont();
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
					MeshType selectedMeshType = GuiPanelMeshSelector.SelectedMesh;
					Vector3 direction = StageCamera.CamPosition - StageCamera.CamTarget;
					direction.Normalize();
					Vector3 spawnPoint = StageCamera.CamTarget + direction * 50;

					Polygon newPolygon = new Polygon {
						Vertices = new List<Vertex> {
							new Vertex(spawnPoint, Color.Red, true, 0, 90),
							new Vertex(spawnPoint + new Vector3(28, 0, 0), Color.Green, true, 0, 90),
							new Vertex(spawnPoint + new Vector3(0, 0, 28), Color.Blue, true, 0, 90)
						},
						MeshType = selectedMeshType,
						RenderingProperties = new PolygonRenderingProperties(),
						PolygonType = _newPolygonIsQuad
							? _newPolygonIsTextured
								? PolygonType.TexturedQuad
								: PolygonType.UntexturedQuad
							: _newPolygonIsTextured
								? PolygonType.TexturedTriangle
								: PolygonType.UntexturedTriangle,
						TerrainLevel = 0,
						TerrainX = 255,
						TerrainZ = 127
					};

					if (newPolygon.IsTextured) {
						newPolygon.UvCoordinates = new List<Microsoft.Xna.Framework.Vector2> {
							new Microsoft.Xna.Framework.Vector2(8, 8),
							new Microsoft.Xna.Framework.Vector2(27, 8),
							new Microsoft.Xna.Framework.Vector2(8, 27)
						};
					}

					if (newPolygon.IsQuad) {
						newPolygon.Vertices.Add(
							new Vertex(spawnPoint + new Vector3(28, 0, 28), Color.Yellow, true, 0, 90)
						);

						if (newPolygon.IsTextured) {
							newPolygon.UvCoordinates.Add(new Microsoft.Xna.Framework.Vector2(27, 27));
						}
					}
					
					Dictionary<PolygonType, List<Polygon>> selectedMesh =
						CurrentMapState.StateData.PolygonCollection[selectedMeshType];

					if (newPolygon.IsQuad) {
						if (newPolygon.IsTextured) {
							selectedMesh[PolygonType.TexturedQuad].Add(newPolygon);
						} else {
							selectedMesh[PolygonType.UntexturedQuad].Add(newPolygon);
						}
					} else {
						if (newPolygon.IsTextured) {
							selectedMesh[PolygonType.TexturedTriangle].Add(newPolygon);
						} else {
							selectedMesh[PolygonType.UntexturedTriangle].Add(newPolygon);
						}
					}

					Selection.SelectedPolygons.Clear();
					Selection.SelectedPolygons.Add(newPolygon);
				}
			}
			ImGui.End();

			if (!windowIsOpen) {
				Gui.ShowAddPolygonWindow = false;
			}
		}
	}
}