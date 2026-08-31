using System.Numerics;
using GaneshaDx.Common;
using GaneshaDx.Resources;
using GaneshaDx.UserInterface.GuiDefinitions;
using ImGuiNET;

namespace GaneshaDx.UserInterface.GuiForms;

public static class GuiWindowExportGlb {
	public static bool BakeSceneLightingIntoTextures = true;

	public static void Render() {
		;
		bool windowIsOpen = true;
		GuiStyle.SetNewUiToDefaultStyle();
		ImGui.GetStyle().WindowRounding = 4;
		GuiStyle.SetFont(Fonts.Large);
		const ImGuiWindowFlags flags = ImGuiWindowFlags.NoResize |
		                               ImGuiWindowFlags.AlwaysAutoResize |
		                               ImGuiWindowFlags.NoCollapse;

		ImGui.SetNextWindowSize(new Vector2(340, 130));
		ImGui.Begin("Export Glb", ref windowIsOpen, flags);
		{
			GuiStyle.SetFont(Fonts.Default);

			ImGui.Columns(2, "GlbOptionsSettings", false);
			ImGui.SetColumnWidth(0, 200);
			ImGui.SetColumnWidth(1, GuiStyle.WidgetWidth + 10);

			ImGui.Text("Bake Scene Lighting Into Textures");
			ImGui.NextColumn();
			ImGui.Checkbox("##bakeSceneLighting", ref BakeSceneLightingIntoTextures);
			ImGui.NextColumn();

			ImGui.TextWrapped("Baked exports use unlit materials so the GLB matches the current editor lighting.");
			ImGui.NextColumn();
			GuiStyle.AddSpace();

			if (ImGui.Button("Export")) {
				FileBrowser.ExportGlbDialog();
				windowIsOpen = false;
			}

			ImGui.NextColumn();

			ImGui.Columns(1);
		}
		ImGui.End();

		if (!windowIsOpen) {
			Gui.ShowExportGlbWindow = false;
		}
	}
}
