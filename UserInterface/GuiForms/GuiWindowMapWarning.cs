using System.Collections.Generic;
using GaneshaDx.Common;
using GaneshaDx.Environment;
using GaneshaDx.Resources;
using GaneshaDx.UserInterface.GuiDefinitions;
using ImGuiNET;
using Vector2 = System.Numerics.Vector2;

namespace GaneshaDx.UserInterface.GuiForms {
	public static class GuiWindowMapWarning {
		private const int Width = 400;
		private const int HeightPerRow = 19;
		private static readonly List<string> Warnings = new List<string>();

		private const int MaxTexturedQuads = 700;
		private const int MaxTexturedTriangles = 512;
		private const int MaxUntexturedQuads = 256;
		private const int MaxUntexturedTriangles = 64;

		public static bool ShouldRender() {
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

			ImGui.Begin("Polygon List", flags);
			{
				ImGuiStylePtr style = ImGui.GetStyle();
				style.Colors[(int) ImGuiCol.Text] = GuiStyle.ColorPalette[ColorName.Selected];

				foreach (string warning in Warnings) {
					ImGui.Text(warning);
				}
			}
		}

		private static void GetWarnings() {
			Warnings.Clear();

			int totalTexturedQuads = CurrentMapState.StateData.PolygonCollection[MeshType.PrimaryMesh][PolygonType.TexturedQuad].Count;
			if (totalTexturedQuads > MaxTexturedQuads) {
				Warnings.Add("Too Many Textured Quads: Map Will Usually Crash In FFT");
			}

			int totalTexturedTriangles = CurrentMapState.StateData.PolygonCollection[MeshType.PrimaryMesh][PolygonType.TexturedTriangle].Count;
			if (totalTexturedTriangles > MaxTexturedTriangles) {
				Warnings.Add("Too Many Textured Triangles: Map Will Usually Crash In FFT");
			}

			int totalUntexturedQuads = CurrentMapState.StateData.PolygonCollection[MeshType.PrimaryMesh][PolygonType.UntexturedQuad].Count;
			if (totalUntexturedQuads > MaxUntexturedQuads) {
				Warnings.Add("Too Many Untextured Quads: Map Will Usually Crash In FFT");
			}

			int totalUntexturedTriangles = CurrentMapState.StateData.PolygonCollection[MeshType.PrimaryMesh][PolygonType.UntexturedTriangle].Count;
			if (totalUntexturedTriangles > MaxUntexturedTriangles) {
				Warnings.Add("Too Many Untextured Triangles: Map Will Usually Crash In FFT");
			}
		}
	}
}