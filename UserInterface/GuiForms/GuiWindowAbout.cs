using GaneshaDx.Common;
using GaneshaDx.UserInterface.GuiDefinitions;
using ImGuiNET;
using Vector2 = System.Numerics.Vector2;

namespace GaneshaDx.UserInterface.GuiForms {
	public static class GuiWindowAbout {
		public static void Render() {
			bool windowIsOpen = true;
			GuiStyle.SetNewUiToDefaultStyle();
			ImGui.GetStyle().WindowRounding = 4;
			ImGui.GetStyle().FrameRounding = 0;
			ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[2]);
			const ImGuiWindowFlags flags = ImGuiWindowFlags.NoResize |
			                               ImGuiWindowFlags.AlwaysAutoResize |
			                               ImGuiWindowFlags.NoCollapse;

			ImGui.SetNextWindowSize(new Vector2(250, 160));
			ImGui.Begin("About GaneshaDx", ref windowIsOpen, flags);
			{
				ImGui.PopFont();
				ImGui.Text("GaneshaDx");
				ImGui.Text("Final Fantasy Tactics Map Editor");
				GuiStyle.AddSpace();
				
				ImGui.Text("Version " + Program.Version);
				GuiStyle.AddSpace();
				
				ImGui.Text("Developed by Garret Bright");
				ImGui.Text("Copyright 2022, Storm Garden Studio");
			}
			ImGui.End();

			if (!windowIsOpen) {
				Gui.ShowAboutWindow = false;
			}
		}
	}
}