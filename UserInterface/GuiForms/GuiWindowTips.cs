using System.Numerics;
using GaneshaDx.UserInterface.GuiDefinitions;
using ImGuiNET;

namespace GaneshaDx.UserInterface.GuiForms {
	public static class GuiWindowTips {
		public static void Render() {
			bool windowIsOpen = true;
			GuiStyle.SetNewUiToDefaultStyle();
			ImGui.GetStyle().WindowRounding = 4;
			ImGui.GetStyle().FrameRounding = 0;
			ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[2]);
			const ImGuiWindowFlags flags = ImGuiWindowFlags.NoResize |
			                               ImGuiWindowFlags.AlwaysAutoResize |
			                               ImGuiWindowFlags.NoCollapse;

			ImGui.SetNextWindowSize(new Vector2(300, 500));
			
			ImGui.Begin("Tips", ref windowIsOpen, flags);
			{
				ImGui.PopFont();
				ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[2]);	
				ImGui.Text("Controls for Polygon Translate Widget:");
				ImGui.PopFont();
				ImGui.Text(":::Hold Shift, then click and drag: Create Clone of\nPolygon Selection");
				ImGui.Text(":::Hold Control while dragging: Translates the \nSelection slowly");
				ImGui.Text(":::Hold Alt while dragging: Snaps translation to grid\nincrements from where Alt was pressed");
				GuiStyle.AddSpace();
				
				ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[2]);	
				ImGui.Text("Crash Events:");
				ImGui.PopFont();
				ImGui.Text(":::Clicking an Animated Mesh");
				
				GuiStyle.AddSpace();
			}
			ImGui.End();

			if (!windowIsOpen) {
				Gui.ShowTipsWindow = false;
			}
		}
	}
}