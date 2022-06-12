using System.Collections.Generic;
using System.Text.RegularExpressions;
using GaneshaDx.Common;
using GaneshaDx.Environment;
using GaneshaDx.Resources;
using GaneshaDx.Resources.ContentDataTypes.Polygons;
using GaneshaDx.UserInterface.GuiDefinitions;
using GaneshaDx.UserInterface.Widgets;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;

namespace GaneshaDx.UserInterface.GuiForms {
	public static class GuiPanelPolygon {
		private static int _selectedMoveToId = 0;

		public static void Render() {
			RenderWidgetSelection();

			if (Selection.SelectedPolygons.Count > 0) {
				RenderPositionValues();
				RenderNormalValues();
				RenderConversionOptions();
				RenderTerrainValues();
				RenderInvisibilityAngles();
			}

			RenderRenderOptions();
		}

		private static void RenderWidgetSelection() {
			GuiStyle.SetNewUiToDefaultStyle();
			GuiStyle.AddSpace();
			ImGui.Indent();

			ImGui.SetCursorPosX(54);

			if (Gui.Widget == WidgetSelectionMode.Select) {
				GuiStyle.SetElementStyle(ElementStyle.ButtonSelected);
			} else {
				GuiStyle.SetNewUiToDefaultStyle();
			}

			ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[3]);
			if (ImGui.Button("Q##selectionMode")) {
				Gui.Widget = WidgetSelectionMode.Select;
			}

			ImGui.PopFont();

			if (ImGui.IsItemHovered()) {
				ImGui.BeginTooltip();
				ImGui.Text("Select Polygons");
				ImGui.EndTooltip();
			}

			ImGui.SameLine();

			if (Gui.Widget == WidgetSelectionMode.PolygonTranslate) {
				GuiStyle.SetElementStyle(ElementStyle.ButtonSelected);
			} else {
				GuiStyle.SetNewUiToDefaultStyle();
			}

			ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[3]);
			if (ImGui.Button("W##translateMode")) {
				Gui.Widget = WidgetSelectionMode.PolygonTranslate;
			}

			ImGui.PopFont();

			if (ImGui.IsItemHovered()) {
				ImGui.BeginTooltip();
				ImGui.Text("Translate Polygons");
				ImGui.EndTooltip();
			}

			ImGui.SameLine();

			if (Gui.Widget == WidgetSelectionMode.PolygonRotate) {
				GuiStyle.SetElementStyle(ElementStyle.ButtonSelected);
			} else {
				GuiStyle.SetNewUiToDefaultStyle();
			}

			ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[3]);
			if (ImGui.Button("E##rotateMode")) {
				Gui.Widget = WidgetSelectionMode.PolygonRotate;
			}

			ImGui.PopFont();

			if (ImGui.IsItemHovered()) {
				ImGui.BeginTooltip();
				ImGui.Text("Rotate Polygons");
				ImGui.EndTooltip();
			}

			ImGui.SameLine();

			if (Gui.Widget == WidgetSelectionMode.PolygonVertexTranslate) {
				GuiStyle.SetElementStyle(ElementStyle.ButtonSelected);
			} else {
				GuiStyle.SetNewUiToDefaultStyle();
			}

			ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[3]);
			if (ImGui.Button("R##vertexMode")) {
				Gui.Widget = WidgetSelectionMode.PolygonVertexTranslate;
			}

			ImGui.PopFont();

			if (ImGui.IsItemHovered()) {
				ImGui.BeginTooltip();
				ImGui.Text("Translate Vertex");
				ImGui.EndTooltip();
			}

			ImGui.SameLine();

			if (Gui.Widget == WidgetSelectionMode.PolygonEdgeTranslate) {
				GuiStyle.SetElementStyle(ElementStyle.ButtonSelected);
			} else {
				GuiStyle.SetNewUiToDefaultStyle();
			}

			ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[3]);

			if (ImGui.Button("T##edgeMode")) {
				Gui.Widget = WidgetSelectionMode.PolygonEdgeTranslate;
			}

			ImGui.PopFont();
			if (ImGui.IsItemHovered()) {
				ImGui.BeginTooltip();
				ImGui.Text("Translate Edge");
				ImGui.EndTooltip();
			}


			if (Gui.Widget == WidgetSelectionMode.PolygonRotate &&
			    Selection.SelectedPolygons.Count > 0
			) {
				RenderRotationVertexSelection();
			}

			ImGui.Unindent();
			GuiStyle.AddSpace();
		}

		private static void RenderRotationVertexSelection() {
			GuiStyle.AddSpace();

			ImGui.SetCursorPosX(98);
			GuiStyle.SetNewUiToDefaultStyle();

			const int buttonSize = 20;
			RangeAccessor<Vector4> colors = ImGui.GetStyle().Colors;
			ImGuiStylePtr imGuiStyle = ImGui.GetStyle();

			colors[(int) ImGuiCol.Button] = Utilities.ConvertVector4(Color.Red.ToVector4());
			colors[(int) ImGuiCol.ButtonHovered] = Utilities.ConvertVector4(Color.Red.ToVector4());
			colors[(int) ImGuiCol.ButtonActive] = Utilities.ConvertVector4(Color.Red.ToVector4());
			imGuiStyle.FrameRounding = RotationWidget.AnchoredToVertex && RotationWidget.AnchoredVertex == 0
				? buttonSize
				: 0;

			if (ImGui.Button("##RotationRed", new Vector2(buttonSize))) {
				if (RotationWidget.AnchoredToVertex && RotationWidget.AnchoredVertex == 0) {
					RotationWidget.AnchoredToVertex = false;
				} else {
					RotationWidget.AnchoredToVertex = true;
					RotationWidget.AnchoredVertex = 0;
				}
			}

			ImGui.SameLine();
			colors[(int) ImGuiCol.Button] = Utilities.ConvertVector4(Color.Green.ToVector4());
			colors[(int) ImGuiCol.ButtonHovered] = Utilities.ConvertVector4(Color.Green.ToVector4());
			colors[(int) ImGuiCol.ButtonActive] = Utilities.ConvertVector4(Color.Green.ToVector4());
			imGuiStyle.FrameRounding = RotationWidget.AnchoredToVertex && RotationWidget.AnchoredVertex == 1
				? buttonSize
				: 0;

			if (ImGui.Button("##RotationGreen", new Vector2(buttonSize))) {
				if (RotationWidget.AnchoredToVertex && RotationWidget.AnchoredVertex == 1) {
					RotationWidget.AnchoredToVertex = false;
				} else {
					RotationWidget.AnchoredToVertex = true;
					RotationWidget.AnchoredVertex = 1;
				}
			}

			ImGui.SameLine();
			colors[(int) ImGuiCol.Button] = Utilities.ConvertVector4(Color.Blue.ToVector4());
			colors[(int) ImGuiCol.ButtonHovered] = Utilities.ConvertVector4(Color.Blue.ToVector4());
			colors[(int) ImGuiCol.ButtonActive] = Utilities.ConvertVector4(Color.Blue.ToVector4());
			imGuiStyle.FrameRounding = RotationWidget.AnchoredToVertex && RotationWidget.AnchoredVertex == 2
				? buttonSize
				: 0;

			if (ImGui.Button("##RotationBlue", new Vector2(buttonSize))) {
				if (RotationWidget.AnchoredToVertex && RotationWidget.AnchoredVertex == 2) {
					RotationWidget.AnchoredToVertex = false;
				} else {
					RotationWidget.AnchoredToVertex = true;
					RotationWidget.AnchoredVertex = 2;
				}
			}

			if (Selection.SelectedPolygons[0].IsQuad) {
				ImGui.SameLine();
				colors[(int) ImGuiCol.Button] = Utilities.ConvertVector4(Color.Yellow.ToVector4());
				colors[(int) ImGuiCol.ButtonHovered] = Utilities.ConvertVector4(Color.Yellow.ToVector4());
				colors[(int) ImGuiCol.ButtonActive] = Utilities.ConvertVector4(Color.Yellow.ToVector4());
				imGuiStyle.FrameRounding = RotationWidget.AnchoredToVertex && RotationWidget.AnchoredVertex == 3
					? buttonSize
					: 0;

				if (ImGui.Button("##RotationYellow", new Vector2(buttonSize))) {
					if (RotationWidget.AnchoredToVertex && RotationWidget.AnchoredVertex == 3) {
						RotationWidget.AnchoredToVertex = false;
					} else {
						RotationWidget.AnchoredToVertex = true;
						RotationWidget.AnchoredVertex = 3;
					}
				}
			}

			if (!Selection.SelectedPolygons[0].IsQuad && RotationWidget.AnchoredVertex == 3) {
				RotationWidget.AnchoredVertex = 2;
			}
		}

		private static void RenderPositionValues() {
			GuiStyle.SetNewUiToDefaultStyle();
			GuiStyle.SetElementStyle(ElementStyle.Header);

			if (ImGui.CollapsingHeader("Position", ImGuiTreeNodeFlags.DefaultOpen)) {
				Polygon polygon = Selection.SelectedPolygons[0];
				const int axisLabelWidth = 36;
				const int valueWidth = 65;

				GuiStyle.SetNewUiToDefaultStyle();
				ImGui.Indent();
				ImGui.Columns(4, "PositionColumns", false);
				ImGui.SetColumnWidth(0, axisLabelWidth);
				ImGui.SetColumnWidth(1, valueWidth + 10);
				ImGui.SetColumnWidth(2, valueWidth + 10);
				ImGui.SetColumnWidth(3, valueWidth + 10);

				GuiStyle.SetElementStyle(ElementStyle.Header);
				ImGui.NextColumn();

				ImGui.PushStyleColor(ImGuiCol.Text, Utilities.ConvertVector4(Color.Red.ToVector4()));
				ImGui.Text("X");
				ImGui.NextColumn();

				ImGui.PushStyleColor(ImGuiCol.Text, Utilities.ConvertVector4(Color.LightGreen.ToVector4()));
				ImGui.Text("Y");
				ImGui.NextColumn();

				ImGui.PushStyleColor(ImGuiCol.Text, Utilities.ConvertVector4(Color.Cyan.ToVector4()));
				ImGui.Text("Z");
				ImGui.NextColumn();

				string[] vertexLabels = { "A", "B", "C", "D" };
				Vector4[] vertexColors = {
					Utilities.ConvertVector4(Color.Red.ToVector4()),
					Utilities.ConvertVector4(Color.LightGreen.ToVector4()),
					Utilities.ConvertVector4(Color.Cyan.ToVector4()),
					Utilities.ConvertVector4(Color.Yellow.ToVector4())
				};

				for (int vertexIndex = 0; vertexIndex < (polygon.IsQuad ? 4 : 3); vertexIndex++) {
					ImGui.PushStyleColor(ImGuiCol.Text, vertexColors[vertexIndex]);
					ImGui.Text(vertexLabels[vertexIndex]);
					ImGui.NextColumn();

					GuiStyle.SetNewUiToDefaultStyle();


					ImGui.PushItemWidth(valueWidth);
					int position = (int) -polygon.Vertices[vertexIndex].Position.X;
					ImGui.DragInt(
						vertexLabels[vertexIndex] + "X_Position",
						ref position,
						0.1f
					);

					polygon.Vertices[vertexIndex].Position = new Microsoft.Xna.Framework.Vector3(
						-position,
						polygon.Vertices[vertexIndex].Position.Y,
						polygon.Vertices[vertexIndex].Position.Z
					);
					ImGui.NextColumn();

					ImGui.PushItemWidth(valueWidth);
					position = (int) polygon.Vertices[vertexIndex].Position.Y;
					ImGui.DragInt(
						vertexLabels[vertexIndex] + "Y_Position",
						ref position,
						0.1f
					);

					polygon.Vertices[vertexIndex].Position = new Microsoft.Xna.Framework.Vector3(
						polygon.Vertices[vertexIndex].Position.X,
						position,
						polygon.Vertices[vertexIndex].Position.Z
					);

					ImGui.NextColumn();

					ImGui.PushItemWidth(valueWidth);
					position = (int) polygon.Vertices[vertexIndex].Position.Z;
					ImGui.DragInt(
						vertexLabels[vertexIndex] + "Z_Position",
						ref position,
						0.1f
					);

					polygon.Vertices[vertexIndex].Position = new Microsoft.Xna.Framework.Vector3(
						polygon.Vertices[vertexIndex].Position.X,
						polygon.Vertices[vertexIndex].Position.Y,
						position
					);

					ImGui.NextColumn();
				}

				ImGui.Columns(1);
				ImGui.Unindent();
				GuiStyle.AddSpace();
			}
		}

		private static void RenderNormalValues() {
			GuiStyle.SetNewUiToDefaultStyle();
			GuiStyle.SetElementStyle(ElementStyle.Header);

			if (ImGui.CollapsingHeader("Normals", ImGuiTreeNodeFlags.DefaultOpen)) {
				Polygon polygon = Selection.SelectedPolygons[0];

				GuiStyle.SetNewUiToDefaultStyle();
				ImGui.Indent();
				ImGui.Columns(2, "NormalsColumns", false);
				ImGui.SetColumnWidth(0, GuiStyle.LabelWidth);
				ImGui.SetColumnWidth(1, GuiStyle.WidgetWidth + 10);

				GuiStyle.SetNewUiToDefaultStyle();

				string[] vertexLabels = { "A", "B", "C", "D" };
				Vector4[] vertexColors = {
					Utilities.ConvertVector4(Color.Red.ToVector4()),
					Utilities.ConvertVector4(Color.LightGreen.ToVector4()),
					Utilities.ConvertVector4(Color.Cyan.ToVector4()),
					Utilities.ConvertVector4(Color.Yellow.ToVector4())
				};

				for (int vertexIndex = 0; vertexIndex < (polygon.IsQuad ? 4 : 3); vertexIndex++) {
					string label = vertexLabels[vertexIndex];
					ImGui.PushStyleColor(ImGuiCol.Text, vertexColors[vertexIndex]);
					ImGui.PushItemWidth(GuiStyle.LabelWidth);
					ImGui.Text(label + " Elevation");
					ImGui.NextColumn();

					GuiStyle.SetNewUiToDefaultStyle();
					ImGui.PushItemWidth(GuiStyle.WidgetWidth);
					ImGui.SliderFloat(label + "Elevation", ref polygon.Vertices[vertexIndex].NormalElevation, -90, 90);
					ImGui.NextColumn();

					ImGui.PushStyleColor(ImGuiCol.Text, vertexColors[vertexIndex]);
					ImGui.PushItemWidth(GuiStyle.LabelWidth);
					ImGui.Text(label + " Azimuth");
					GuiStyle.AddSpace();
					ImGui.NextColumn();

					GuiStyle.SetNewUiToDefaultStyle();
					ImGui.PushItemWidth(GuiStyle.WidgetWidth);
					ImGui.SliderFloat(label + "Azimuth", ref polygon.Vertices[vertexIndex].NormalAzimuth, 0, 360);
					GuiStyle.AddSpace();
					ImGui.NextColumn();
				}

				ImGui.AlignTextToFramePadding();
				GuiStyle.SetElementStyle(ElementStyle.Header);
				ImGui.PushStyleColor(ImGuiCol.Text, GuiStyle.ColorPalette[ColorName.Lightest]);
				ImGui.Text("Set Normals as");

				ImGui.NextColumn();

				if (ImGui.Button("Local")) {
					foreach (Polygon selectedPolygon in Selection.SelectedPolygons) {
						selectedPolygon.GuessNormals();
					}
				}

				ImGui.SameLine();

				if (ImGui.Button("Group")) {
					foreach (Polygon selectedPolygon in Selection.SelectedPolygons) {
						selectedPolygon.GuessNormals();
					}

					Utilities.AverageNormals();
				}

				ImGui.NextColumn();
				ImGui.NextColumn();

				GuiStyle.AddSpace();

				if (ImGui.Button("Flip Normals", new Vector2(GuiStyle.WidgetWidth, 20))) {
					foreach (Polygon selectedPolygon in Selection.SelectedPolygons) {
						selectedPolygon.FlipNormals();
					}

					Utilities.AverageNormals();
				}

				if (polygon.IsQuad && ImGui.Button("Break", new Vector2(GuiStyle.WidgetWidth, 20))) {
					List<Polygon> newPolys = new List<Polygon>();
					foreach (Polygon selectedPolygon in Selection.SelectedPolygons) {
						newPolys.AddRange(selectedPolygon.Break());
					}

					Selection.SelectedPolygons.Clear();

					foreach (Polygon newPolygon in newPolys) {
						Selection.AddPolyToSelection(newPolygon);
					}
				}

				ImGui.Columns(1);
				ImGui.Unindent();
				GuiStyle.AddSpace();
			}
		}

		private static void RenderConversionOptions() {
			Polygon selectedPolygon = Selection.SelectedPolygons[0];

			GuiStyle.SetNewUiToDefaultStyle();
			GuiStyle.SetElementStyle(ElementStyle.Header);

			if (ImGui.CollapsingHeader("Conversion", ImGuiTreeNodeFlags.DefaultOpen)) {
				GuiStyle.SetNewUiToDefaultStyle();
				ImGui.Indent();

				if (CurrentMapState.StateData.HasAnimatedMesh1 ||
				    CurrentMapState.StateData.HasAnimatedMesh2 ||
				    CurrentMapState.StateData.HasAnimatedMesh3 ||
				    CurrentMapState.StateData.HasAnimatedMesh4 ||
				    CurrentMapState.StateData.HasAnimatedMesh5 ||
				    CurrentMapState.StateData.HasAnimatedMesh6 ||
				    CurrentMapState.StateData.HasAnimatedMesh7 ||
				    CurrentMapState.StateData.HasAnimatedMesh8
				) {
					List<MeshType> meshTypes = new List<MeshType>();
					List<string> labels = new List<string>();

					MeshType selectedMesh = GuiPanelMeshSelector.SelectedMesh;

					if (selectedMesh != MeshType.PrimaryMesh) {
						meshTypes.Add(MeshType.PrimaryMesh);
						labels.Add("Primary Mesh");
					}

					if (CurrentMapState.StateData.HasAnimatedMesh1 && selectedMesh != MeshType.AnimatedMesh1) {
						meshTypes.Add(MeshType.AnimatedMesh1);
						labels.Add("Animated Mesh 1");
					}

					if (CurrentMapState.StateData.HasAnimatedMesh2 && selectedMesh != MeshType.AnimatedMesh2) {
						meshTypes.Add(MeshType.AnimatedMesh2);
						labels.Add("Animated Mesh 2");
					}

					if (CurrentMapState.StateData.HasAnimatedMesh3 && selectedMesh != MeshType.AnimatedMesh3) {
						meshTypes.Add(MeshType.AnimatedMesh3);
						labels.Add("Animated Mesh 3");
					}

					if (CurrentMapState.StateData.HasAnimatedMesh4 && selectedMesh != MeshType.AnimatedMesh4) {
						meshTypes.Add(MeshType.AnimatedMesh4);
						labels.Add("Animated Mesh 4");
					}

					if (CurrentMapState.StateData.HasAnimatedMesh5 && selectedMesh != MeshType.AnimatedMesh5) {
						meshTypes.Add(MeshType.AnimatedMesh5);
						labels.Add("Animated Mesh 5");
					}

					if (CurrentMapState.StateData.HasAnimatedMesh6 && selectedMesh != MeshType.AnimatedMesh6) {
						meshTypes.Add(MeshType.AnimatedMesh6);
						labels.Add("Animated Mesh 6");
					}

					if (CurrentMapState.StateData.HasAnimatedMesh7 && selectedMesh != MeshType.AnimatedMesh7) {
						meshTypes.Add(MeshType.AnimatedMesh7);
						labels.Add("Animated Mesh 7");
					}

					if (CurrentMapState.StateData.HasAnimatedMesh8 && selectedMesh != MeshType.AnimatedMesh8) {
						meshTypes.Add(MeshType.AnimatedMesh8);
						labels.Add("Animated Mesh 8");
					}

					ImGui.Columns(2, "ConvertMeshTypeColumn", false);
					ImGui.SetColumnWidth(0, GuiStyle.LabelWidth);
					ImGui.SetColumnWidth(1, GuiStyle.WidgetWidth + 10);

					ImGui.SetNextItemWidth(GuiStyle.LabelWidth - 10);

					if (_selectedMoveToId > meshTypes.Count - 1) {
						_selectedMoveToId = 0;
					}
					
					ImGui.Combo("##MoveToMeshType", ref _selectedMoveToId, labels.ToArray(), meshTypes.Count);

					ImGui.NextColumn();
					
					if (ImGui.Button("Move", new Vector2(GuiStyle.WidgetWidth, 20))) {
						foreach (Polygon polygon in Selection.SelectedPolygons) {
							CurrentMapState.StateData.PolygonCollection[polygon.MeshType][polygon.PolygonType].Remove(polygon);
							polygon.MeshType = meshTypes[_selectedMoveToId];
							CurrentMapState.StateData.PolygonCollection[polygon.MeshType][polygon.PolygonType].Add(polygon);
						}
					}

					ImGui.NextColumn();
				}

				ImGui.NextColumn();

				string label = selectedPolygon.IsTextured
					? "To Untextured"
					: "To Textured";

				if (ImGui.Button(label, new Vector2(GuiStyle.WidgetWidth, 20))) {
					CurrentMapState.StateData.PolygonCollection[selectedPolygon.MeshType][selectedPolygon.PolygonType].Remove(selectedPolygon);
					if (selectedPolygon.IsTextured) {
						selectedPolygon.PolygonType = selectedPolygon.IsQuad
							? PolygonType.UntexturedQuad
							: PolygonType.UntexturedTriangle;

						selectedPolygon.UvCoordinates = null;
					} else {
						selectedPolygon.PolygonType = selectedPolygon.IsQuad
							? PolygonType.TexturedQuad
							: PolygonType.TexturedTriangle;

						selectedPolygon.UvCoordinates = new List<Microsoft.Xna.Framework.Vector2> {
							new Microsoft.Xna.Framework.Vector2(9, 9),
							new Microsoft.Xna.Framework.Vector2(28, 9),
							new Microsoft.Xna.Framework.Vector2(9, 28)
						};

						if (selectedPolygon.IsQuad) {
							selectedPolygon.UvCoordinates.Add(new Microsoft.Xna.Framework.Vector2(28, 28));
						}
					}

					CurrentMapState.StateData.PolygonCollection[selectedPolygon.MeshType][selectedPolygon.PolygonType].Add(selectedPolygon);
				}


				ImGui.Columns(1);
				ImGui.Unindent();
				GuiStyle.AddSpace();
			}
		}

		private static void RenderTerrainValues() {
			if (Selection.SelectedPolygons[0].MeshType != MeshType.PrimaryMesh) {
				return;
			}

			GuiStyle.SetNewUiToDefaultStyle();
			GuiStyle.SetElementStyle(ElementStyle.Header);

			if (ImGui.CollapsingHeader("Terrain", ImGuiTreeNodeFlags.DefaultOpen)) {
				Polygon polygon = Selection.SelectedPolygons[0];

				GuiStyle.SetNewUiToDefaultStyle();
				ImGui.Indent();
				ImGui.Columns(2, "TerrainColumns", false);
				ImGui.SetColumnWidth(0, GuiStyle.LabelWidth);
				ImGui.SetColumnWidth(1, GuiStyle.WidgetWidth + 10);

				GuiStyle.SetNewUiToDefaultStyle();

				int terrainLevel = polygon.TerrainLevel;
				int terrainX = polygon.TerrainX;
				int terrainZ = polygon.TerrainZ;

				int beforeTerrainLevel = terrainLevel;
				int beforeTerrainX = terrainX;
				int beforeTerrainZ = terrainZ;

				ImGui.Text("Level");
				ImGui.NextColumn();

				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.InputInt("TerrainLevel", ref terrainLevel, 1);
				ImGui.NextColumn();

				ImGui.Text("X");
				ImGui.NextColumn();

				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.InputInt("TerrainX", ref terrainX, 1);
				ImGui.NextColumn();

				ImGui.Text("Z");
				ImGui.NextColumn();

				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.InputInt("TerrainZ", ref terrainZ, 1);
				ImGui.NextColumn();


				terrainX = Utilities.Clamp(terrainX, 0, 255);
				terrainZ = Utilities.Clamp(terrainZ, 0, 127);
				terrainLevel = Utilities.Clamp(terrainLevel, 0, 1);

				if (terrainX != beforeTerrainX) {
					foreach (Polygon selectedPolygon in Selection.SelectedPolygons) {
						selectedPolygon.TerrainX = terrainX;
					}
				}

				if (terrainZ != beforeTerrainZ) {
					foreach (Polygon selectedPolygon in Selection.SelectedPolygons) {
						selectedPolygon.TerrainZ = terrainZ;
					}
				}

				if (terrainLevel != beforeTerrainLevel) {
					foreach (Polygon selectedPolygon in Selection.SelectedPolygons) {
						selectedPolygon.TerrainLevel = terrainLevel;
					}
				}

				ImGui.Columns(1);
				ImGui.Unindent();
				GuiStyle.AddSpace();
			}
		}

		private static void RenderInvisibilityAngles() {
			Polygon polygon = Selection.SelectedPolygons[0];

			if (polygon.RenderingProperties == null) {
				return;
			}

			GuiStyle.SetNewUiToDefaultStyle();
			GuiStyle.SetElementStyle(ElementStyle.Header);

			if (ImGui.CollapsingHeader("Invisibility Angles", ImGuiTreeNodeFlags.DefaultOpen)) {
				GuiStyle.SetNewUiToDefaultStyle();
				ImGui.Indent();

				ImGui.Text("Hide Polygon when Camera Facing:");
				GuiStyle.AddSpace();

				ImGui.Columns(5, "InvisibilityAnglesColumns", false);

				ImGui.SetColumnWidth(0, GuiStyle.CheckBoxWidth);
				ImGui.SetColumnWidth(1, GuiStyle.CheckBoxWidth);
				ImGui.SetColumnWidth(2, GuiStyle.CheckBoxWidth);
				ImGui.SetColumnWidth(3, GuiStyle.CheckBoxWidth);
				ImGui.SetColumnWidth(4, GuiStyle.CheckBoxWidth);

				PolygonRenderingProperties angles = polygon.RenderingProperties;
				PolygonRenderingProperties beforeAngles = new PolygonRenderingProperties {
					InvisibleNorthwest = angles.InvisibleNorthwest,
					InvisibleNorthNorthwest = angles.InvisibleNorthNorthwest,
					InvisibleNorthNortheast = angles.InvisibleNorthNortheast,
					InvisibleNortheast = angles.InvisibleNortheast,
					InvisibleWestNorthWest = angles.InvisibleWestNorthWest,
					InvisibleEastNortheast = angles.InvisibleEastNortheast,
					InvisibleWestSouthwest = angles.InvisibleWestSouthwest,
					InvisibleEastSoutheast = angles.InvisibleEastSoutheast,
					InvisibleSouthwest = angles.InvisibleSouthwest,
					InvisibleSouthSouthwest = angles.InvisibleSouthSouthwest,
					InvisibleSouthSoutheast = angles.InvisibleSouthSoutheast,
					InvisibleSoutheast = angles.InvisibleSoutheast
				};

				ImGuiStylePtr style = ImGui.GetStyle();

				ImGui.Checkbox("NW", ref angles.InvisibleNorthwest);
				ImGui.NextColumn();
				ImGui.Checkbox("NNW", ref angles.InvisibleNorthNorthwest);
				ImGui.NextColumn();
				ImGui.NextColumn();
				ImGui.Checkbox("NNE", ref angles.InvisibleNorthNortheast);
				ImGui.NextColumn();
				ImGui.Checkbox("NE", ref angles.InvisibleNortheast);
				ImGui.NextColumn();

				ImGui.Checkbox("WNW", ref angles.InvisibleWestNorthWest);
				ImGui.NextColumn();

				if (StageCamera.FacingDirection == StageCamera.CameraView.Northwest) {
					style.Colors[(int) ImGuiCol.Text] = GuiStyle.ColorPalette[ColorName.HighlightedText];
				} else {
					style.Colors[(int) ImGuiCol.Text] = GuiStyle.ColorPalette[ColorName.Lightest];
				}

				ImGui.Text("NW");
				ImGui.NextColumn();
				ImGui.NextColumn();

				if (StageCamera.FacingDirection == StageCamera.CameraView.Northeast) {
					style.Colors[(int) ImGuiCol.Text] = GuiStyle.ColorPalette[ColorName.HighlightedText];
				} else {
					style.Colors[(int) ImGuiCol.Text] = GuiStyle.ColorPalette[ColorName.Lightest];
				}

				ImGui.Text("NE");
				ImGui.NextColumn();
				ImGui.Checkbox("ENE", ref angles.InvisibleEastNortheast);
				ImGui.NextColumn();

				GuiStyle.AddSpace(30);
				ImGui.NextColumn();
				GuiStyle.AddSpace(30);
				ImGui.NextColumn();
				GuiStyle.AddSpace(30);
				ImGui.NextColumn();
				GuiStyle.AddSpace(30);
				ImGui.NextColumn();
				GuiStyle.AddSpace(30);
				ImGui.NextColumn();

				ImGui.Checkbox("WSW", ref angles.InvisibleWestSouthwest);
				ImGui.NextColumn();

				if (StageCamera.FacingDirection == StageCamera.CameraView.Southwest) {
					style.Colors[(int) ImGuiCol.Text] = GuiStyle.ColorPalette[ColorName.HighlightedText];
				} else {
					style.Colors[(int) ImGuiCol.Text] = GuiStyle.ColorPalette[ColorName.Lightest];
				}

				ImGui.Text("SW");
				ImGui.NextColumn();
				ImGui.NextColumn();

				if (StageCamera.FacingDirection == StageCamera.CameraView.Southeast) {
					style.Colors[(int) ImGuiCol.Text] = GuiStyle.ColorPalette[ColorName.HighlightedText];
				} else {
					style.Colors[(int) ImGuiCol.Text] = GuiStyle.ColorPalette[ColorName.Lightest];
				}

				ImGui.Text("SE");
				ImGui.NextColumn();
				ImGui.Checkbox("ESE", ref angles.InvisibleEastSoutheast);
				ImGui.NextColumn();

				ImGui.Checkbox("SW", ref angles.InvisibleSouthwest);
				ImGui.NextColumn();
				ImGui.Checkbox("SSW", ref angles.InvisibleSouthSouthwest);
				ImGui.NextColumn();
				ImGui.NextColumn();
				ImGui.Checkbox("SSE", ref angles.InvisibleSouthSoutheast);
				ImGui.NextColumn();
				ImGui.Checkbox("SE", ref angles.InvisibleSoutheast);
				ImGui.NextColumn();

				foreach (Polygon selectedPolygon in Selection.SelectedPolygons) {
					if (beforeAngles.InvisibleNorthwest != angles.InvisibleNorthwest) {
						selectedPolygon.RenderingProperties.InvisibleNorthwest = angles.InvisibleNorthwest;
					}

					if (beforeAngles.InvisibleNorthNorthwest != angles.InvisibleNorthNorthwest) {
						selectedPolygon.RenderingProperties.InvisibleNorthNorthwest = angles.InvisibleNorthNorthwest;
					}

					if (beforeAngles.InvisibleNorthNortheast != angles.InvisibleNorthNortheast) {
						selectedPolygon.RenderingProperties.InvisibleNorthNortheast = angles.InvisibleNorthNortheast;
					}

					if (beforeAngles.InvisibleNortheast != angles.InvisibleNortheast) {
						selectedPolygon.RenderingProperties.InvisibleNortheast = angles.InvisibleNortheast;
					}

					if (beforeAngles.InvisibleWestNorthWest != angles.InvisibleWestNorthWest) {
						selectedPolygon.RenderingProperties.InvisibleWestNorthWest = angles.InvisibleWestNorthWest;
					}

					if (beforeAngles.InvisibleEastNortheast != angles.InvisibleEastNortheast) {
						selectedPolygon.RenderingProperties.InvisibleEastNortheast = angles.InvisibleEastNortheast;
					}

					if (beforeAngles.InvisibleWestSouthwest != angles.InvisibleWestSouthwest) {
						selectedPolygon.RenderingProperties.InvisibleWestSouthwest = angles.InvisibleWestSouthwest;
					}

					if (beforeAngles.InvisibleEastSoutheast != angles.InvisibleEastSoutheast) {
						selectedPolygon.RenderingProperties.InvisibleEastSoutheast = angles.InvisibleEastSoutheast;
					}

					if (beforeAngles.InvisibleSouthwest != angles.InvisibleSouthwest) {
						selectedPolygon.RenderingProperties.InvisibleSouthwest = angles.InvisibleSouthwest;
					}

					if (beforeAngles.InvisibleSouthSouthwest != angles.InvisibleSouthSouthwest) {
						selectedPolygon.RenderingProperties.InvisibleSouthSouthwest = angles.InvisibleSouthSouthwest;
					}

					if (beforeAngles.InvisibleSouthSoutheast != angles.InvisibleSouthSoutheast) {
						selectedPolygon.RenderingProperties.InvisibleSouthSoutheast = angles.InvisibleSouthSoutheast;
					}

					if (beforeAngles.InvisibleSoutheast != angles.InvisibleSoutheast) {
						selectedPolygon.RenderingProperties.InvisibleSoutheast = angles.InvisibleSoutheast;
					}
				}

				ImGui.Columns(1);

				GuiStyle.AddSpace();

				if (ImGui.Button("Assign to Selection")) {
					foreach (Polygon selectedPolygon in Selection.SelectedPolygons) {
						selectedPolygon.RenderingProperties.InvisibleNorthwest = angles.InvisibleNorthwest;
						selectedPolygon.RenderingProperties.InvisibleNorthNorthwest = angles.InvisibleNorthNorthwest;
						selectedPolygon.RenderingProperties.InvisibleNorthNortheast = angles.InvisibleNorthNortheast;
						selectedPolygon.RenderingProperties.InvisibleNortheast = angles.InvisibleNortheast;
						selectedPolygon.RenderingProperties.InvisibleWestNorthWest = angles.InvisibleWestNorthWest;
						selectedPolygon.RenderingProperties.InvisibleEastNortheast = angles.InvisibleEastNortheast;
						selectedPolygon.RenderingProperties.InvisibleWestSouthwest = angles.InvisibleWestSouthwest;
						selectedPolygon.RenderingProperties.InvisibleEastSoutheast = angles.InvisibleEastSoutheast;
						selectedPolygon.RenderingProperties.InvisibleSouthwest = angles.InvisibleSouthwest;
						selectedPolygon.RenderingProperties.InvisibleSouthSouthwest = angles.InvisibleSouthSouthwest;
						selectedPolygon.RenderingProperties.InvisibleSouthSoutheast = angles.InvisibleSouthSoutheast;
						selectedPolygon.RenderingProperties.InvisibleSoutheast = angles.InvisibleSoutheast;
					}
				}

				ImGui.Unindent();
				GuiStyle.AddSpace();
			}
		}

		private static void RenderRenderOptions() {
			GuiStyle.SetNewUiToDefaultStyle();
			GuiStyle.SetElementStyle(ElementStyle.Header);

			if (ImGui.CollapsingHeader("Render Options###polyTab", ImGuiTreeNodeFlags.DefaultOpen)) {
				GuiStyle.SetNewUiToDefaultStyle();
				ImGui.Indent();
				ImGui.Columns(2, "RenderOptionsOnPolygonTab", false);
				ImGui.SetColumnWidth(0, GuiStyle.LabelWidth);
				ImGui.SetColumnWidth(1, GuiStyle.WidgetWidth + 10);

				GuiStyle.SetNewUiToDefaultStyle();

				ImGui.Text("Terrain Opacity");

				ImGui.NextColumn();
				int beforeTransparency = Configuration.Properties.TerrainTransparencyForPolygonEditing;
				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.SliderInt(
					"###TerrainTransparency",
					ref Configuration.Properties.TerrainTransparencyForPolygonEditing,
					0, 100
				);

				if (beforeTransparency != Configuration.Properties.TerrainTransparencyForPolygonEditing) {
					Configuration.SaveConfiguration();
				}

				ImGui.NextColumn();

				ImGui.Text("Terrain Color");

				ImGui.NextColumn();
				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);

				Vector3 beforeColor = Configuration.Properties.TerrainColorForPolygonEditing;
				ImGui.ColorEdit3(
					"TerrainColor",
					ref Configuration.Properties.TerrainColorForPolygonEditing,
					GuiStyle.ColorBoxFlags
				);

				if (beforeColor != Configuration.Properties.TerrainColorForPolygonEditing) {
					Configuration.SaveConfiguration();
				}


				ImGui.Columns(1);
				ImGui.Unindent();
			}
		}
	}
}