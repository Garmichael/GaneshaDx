using System.Numerics;
using GaneshaDx.UserInterface.GuiDefinitions;
using ImGuiNET;

namespace GaneshaDx.UserInterface.GuiForms;

public static class GuiWindowExportGlb {
	public static bool ExportUnlit;

	public static void Render() {
		;
		bool windowIsOpen = true;
		GuiStyle.SetNewUiToDefaultStyle();
		ImGui.GetStyle().WindowRounding = 4;
		GuiStyle.SetFont(Fonts.Large);
		const ImGuiWindowFlags flags = ImGuiWindowFlags.NoResize |
		                               ImGuiWindowFlags.AlwaysAutoResize |
		                               ImGuiWindowFlags.NoCollapse;

		ImGui.SetNextWindowSize(new Vector2(270, 100));
		ImGui.Begin("Export Glb", ref windowIsOpen, flags);
		{
			GuiStyle.SetFont(Fonts.Default);

			ImGui.Columns(2, "GlbOptionsSettings", false);
			ImGui.SetColumnWidth(0, 200);
			ImGui.SetColumnWidth(1, GuiStyle.WidgetWidth + 10);

			ImGui.Text("Export with Unlit Materials");
			ImGui.NextColumn();
			ImGui.Checkbox("##exportAsUnlit", ref ExportUnlit);
			ImGui.NextColumn();

			ImGui.NextColumn();
			GuiStyle.AddSpace();

			if (ImGui.Button("Export")) {
				GuiWindowFileBrowser.Open(GuiWindowFileBrowser.DialogBoxes.ExportGlb);
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