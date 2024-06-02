using System.Diagnostics;
using System.Numerics;
using GaneshaDx.Common;
using GaneshaDx.UserInterface.GuiDefinitions;
using ImGuiNET;

namespace GaneshaDx.UserInterface.GuiForms;

public static class GuiWindowUpdateAvailable {
	public static bool ShouldRender() {
		return UpdateChecker.AppIsOutdated;
	}

	public static void Render() {
		GuiStyle.SetNewUiToDefaultStyle();
		ImGui.GetStyle().WindowRounding = 0;
		ImGui.GetStyle().FrameRounding = 0;
		GuiStyle.SetFont(Fonts.Large);
		const ImGuiWindowFlags flags = ImGuiWindowFlags.NoResize |
		                               ImGuiWindowFlags.AlwaysAutoResize |
		                               ImGuiWindowFlags.NoCollapse |
		                               ImGuiWindowFlags.NoTitleBar |
		                               ImGuiWindowFlags.NoCollapse;

		ImGui.SetNextWindowSize(new Vector2(235, 70));
		ImGui.SetNextWindowPos(new Vector2(0, 50));

		ImGui.Begin("Update Modal", flags);
		{
			ImGuiStylePtr style = ImGui.GetStyle();
			style.Colors[(int) ImGuiCol.Text] = GuiStyle.ColorPalette[ColorName.Selected];
			ImGui.Text("    A new version has downloaded");

			GuiStyle.AddSpace();
			ImGui.Text("                ");

			GuiStyle.SetNewUiToDefaultStyle();
			ImGui.SameLine();

			if (ImGui.Button("Install Update")) {
				
				Process process = new();
				process.StartInfo.FileName = UpdateChecker.InstallerFilePath;
				process.Start();
			}
		}
		ImGui.End();
	}
}