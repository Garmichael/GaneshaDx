using System.Collections.Generic;
using GaneshaDx.Environment;
using GaneshaDx.Rendering;
using GaneshaDx.UserInterface.GuiDefinitions;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Vector2 = System.Numerics.Vector2;

namespace GaneshaDx.UserInterface.GuiForms;

public static class GuiWindowScreenshotBackgroundSelector {
	private const int WindowWidth = 150;
	private const int WindowHeight = 35;
	private static int _selectedIndex;
	private static readonly List<string> BackgroundColors;
	
	static GuiWindowScreenshotBackgroundSelector() {
		BackgroundColors = new List<string> { "Map Background" };
		foreach (KeyValuePair<string, Color> backgroundColor in Background.ScreenshotBackgroundColors) {
			BackgroundColors.Add(backgroundColor.Key);
		}
	}
	
	public static void Render() {
		bool windowIsOpen = true;

		GuiStyle.SetNewUiToDefaultStyle();
		ImGui.GetStyle().FrameRounding = 0;
		ImGui.GetStyle().WindowRounding = 0;
		
		const ImGuiWindowFlags flags = ImGuiWindowFlags.NoResize |
		                               ImGuiWindowFlags.NoCollapse |
		                               ImGuiWindowFlags.NoTitleBar |
		                               ImGuiWindowFlags.NoScrollbar;

		ImGui.SetNextWindowSize(new Vector2(WindowWidth, WindowHeight));
		ImGui.SetNextWindowPos(new Vector2(Stage.Width - WindowWidth, Stage.Height - WindowHeight));
		
		ImGui.Begin("Screenshot Background Selector", ref windowIsOpen, flags);
		{
			GuiStyle.SetNewUiToDefaultStyle();
			ImGui.SetNextItemWidth(WindowWidth - 15);
			ImGui.Combo("##SelectedScreenshotBackground", ref _selectedIndex, BackgroundColors.ToArray(), BackgroundColors.Count);
			
			if (_selectedIndex > 0) {
				Background.ScreenshotBackgroundIndex = _selectedIndex - 1;
				Background.UseScreenshotBackground = true;
			} else {
				Background.UseScreenshotBackground = false;
			}
		}
		ImGui.End();
	}
}