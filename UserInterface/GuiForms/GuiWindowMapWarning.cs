using System.Collections.Generic;
using GaneshaDx.Common;
using GaneshaDx.Environment;
using GaneshaDx.Resources;
using GaneshaDx.UserInterface.GuiDefinitions;
using ImGuiNET;
using Vector2 = System.Numerics.Vector2;

namespace GaneshaDx.UserInterface.GuiForms {
	public static class GuiWindowMapWarning {
		private const int Width = 500;
		private const int HeightPerRow = 19;
		private static readonly List<string> Warnings = new List<string>();

		private const int MaxTexturedQuads = 710;
		private const int MaxTexturedTriangles = 360;
		private const int MaxUntexturedQuads = 256;
		private const int MaxUntexturedTriangles = 64;

		public static bool ShouldRender() {
			Warnings.Clear();
			GetWarnings();
			return Warnings.Count > 0;
		}

		public static void Render() {
			GuiStyle.SetNewUiToDefaultStyle();
			ImGui.GetStyle().WindowRounding = 0;
			ImGui.GetStyle().FrameRounding = 0;
			ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[2]);
			const ImGuiWindowFlags flags = ImGuiWindowFlags.NoResize |
			                               ImGuiWindowFlags.AlwaysAutoResize |
			                               ImGuiWindowFlags.NoCollapse |
			                               ImGuiWindowFlags.NoTitleBar |
			                               ImGuiWindowFlags.NoCollapse;

			int totalHeight = HeightPerRow * Warnings.Count + 16;
			ImGui.SetNextWindowSize(new Vector2(Width, totalHeight));
			int modelingSpaceWidth = Stage.Width - GuiStyle.RightPanelWidth;
			ImGui.SetNextWindowPos(new Vector2((modelingSpaceWidth - Width) / 2f, Stage.Height - totalHeight));

			ImGui.Begin("Map Warnings", flags);
			{
				ImGuiStylePtr style = ImGui.GetStyle();
				style.Colors[(int) ImGuiCol.Text] = GuiStyle.ColorPalette[ColorName.Selected];

				foreach (string warning in Warnings) {
					ImGui.Text(warning);
				}
			}
			ImGui.End();
		}

		private static void GetWarnings() {
			Warnings.Clear();

			
			if (CurrentMapState.StateMeshMapResources.Count == 0) {
				Warnings.Add("The Selected State Does Not Exist");
			}

			int totalTexturedQuads = Utilities.GetTotalPolygonCount(PolygonType.TexturedQuad);
			if (totalTexturedQuads > MaxTexturedQuads) {
				Warnings.Add("Too Many Textured Quads. Using " + totalTexturedQuads + " of " + MaxTexturedQuads);
			}

			int totalTexturedTriangles = Utilities.GetTotalPolygonCount(PolygonType.TexturedTriangle);
			if (totalTexturedTriangles > MaxTexturedTriangles) {
				Warnings.Add("Too Many Textured Triangles. Using " + totalTexturedTriangles + " of " + MaxTexturedTriangles);
			}

			int totalUntexturedQuads = Utilities.GetTotalPolygonCount(PolygonType.UntexturedQuad);
			if (totalUntexturedQuads > MaxUntexturedQuads) {
				Warnings.Add("Too Many Untextured Quads. Using " + totalUntexturedQuads + " of " + MaxUntexturedQuads);
			}

			int totalUntexturedTriangles = Utilities.GetTotalPolygonCount(PolygonType.UntexturedTriangle);
			if (totalUntexturedTriangles > MaxUntexturedTriangles) {
				Warnings.Add("Too Many Untextured Triangles. Using " + totalUntexturedTriangles + " of " + MaxUntexturedTriangles);
			}
		}
	}
}