using System.Diagnostics;
using System.Numerics;
using GaneshaDx.Environment;
using GaneshaDx.UserInterface.GuiDefinitions;
using ImGuiNET;

namespace GaneshaDx.UserInterface.GuiForms;

public static class GuiWindowUpdateAvailable {
	public static bool ShouldRender() {
		return Stage.Ganesha.AppIsOutdated;
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

		ImGui.SetNextWindowSize(new Vector2(200, 70));
		ImGui.SetNextWindowPos(new Vector2(0, 50));

		ImGui.Begin("Update Modal", flags);
		{
			ImGuiStylePtr style = ImGui.GetStyle();
			style.Colors[(int) ImGuiCol.Text] = GuiStyle.ColorPalette[ColorName.Selected];
			ImGui.Text("    A new version is available");

			GuiStyle.AddSpace();
			ImGui.Text("        ");

			GuiStyle.SetNewUiToDefaultStyle();
			ImGui.SameLine();

			if (ImGui.Button("Download Update")) {
				Process.Start(new ProcessStartInfo {
					FileName = "https://github.com/Garmichael/GaneshaDx/releases",
					UseShellExecute = true
				});
			}
		}
		ImGui.End();
	}
}