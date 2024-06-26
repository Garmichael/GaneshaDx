﻿using GaneshaDx.UserInterface.GuiDefinitions;
using ImGuiNET;
using Vector2 = System.Numerics.Vector2;

namespace GaneshaDx.UserInterface.GuiForms;

public static class GuiWindowAbout {
	public static void Render() {
		bool windowIsOpen = true;
		GuiStyle.SetNewUiToDefaultStyle();
		ImGui.GetStyle().WindowRounding = 4;
		ImGui.GetStyle().FrameRounding = 0;

		GuiStyle.SetFont(Fonts.Large);
		const ImGuiWindowFlags flags = ImGuiWindowFlags.NoResize |
		                               ImGuiWindowFlags.AlwaysAutoResize |
		                               ImGuiWindowFlags.NoCollapse;

		ImGui.SetNextWindowSize(new Vector2(260, 210));
		ImGui.Begin("About GaneshaDx", ref windowIsOpen, flags);
		{
			GuiStyle.SetFont(Fonts.Default);
			ImGui.Text("GaneshaDx");
			ImGui.Text("Final Fantasy Tactics Map Editor");
			GuiStyle.AddSpace();

			ImGui.Text("Version " + Program.Version);
			GuiStyle.AddSpace();

			ImGui.Text("Developed by Garret Bright");
			ImGui.Text("GLB Exporter by CalamityCoder");

			GuiStyle.AddSpace();
			ImGui.Text("Copyright 2021 - Present");
			ImGui.Text("Storm Garden Studio");
		}
		ImGui.End();

		if (!windowIsOpen) {
			Gui.ShowAboutWindow = false;
		}
	}
}