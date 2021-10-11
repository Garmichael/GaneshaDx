using System.Numerics;
using GaneshaDx.Common;
using GaneshaDx.UserInterface.GuiDefinitions;
using ImGuiNET;

namespace GaneshaDx.UserInterface.GuiForms {
	public static class GuiWindowPreferences {
		public static void Render() {
			bool windowIsOpen = true;

			GuiStyle.SetNewUiToDefaultStyle();
			ImGui.GetStyle().WindowRounding = 4;
			ImGui.GetStyle().FrameRounding = 0;
			ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[2]);
			const ImGuiWindowFlags flags = ImGuiWindowFlags.NoResize |
			                               ImGuiWindowFlags.AlwaysAutoResize |
			                               ImGuiWindowFlags.NoCollapse;

			ImGui.Begin("Preferences", ref windowIsOpen, flags);
			{
				ImGui.PopFont();
				ImGui.Columns(2, "PreferencesControls", false);
				ImGui.SetColumnWidth(0, GuiStyle.LabelWidth + 10);
				ImGui.SetColumnWidth(1, GuiStyle.WidgetWidth + 10);

				ImGui.Text("Control+Drag Modifier");
				ImGui.NextColumn();

				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.InputFloat("PreferencesCntSpeed", ref Configuration.Properties.TranslateAxisControlSpeed, 0.05f);
				Configuration.Properties.TranslateAxisControlSpeed =
					Utilities.Min(Configuration.Properties.TranslateAxisControlSpeed, 0.1f);
				ImGui.NextColumn();

				ImGui.Text("Panning Strength");
				ImGui.NextColumn();

				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.InputFloat("PreferencesPanningStrength", ref Configuration.Properties.PanningSensitivity, 0.05f);
				Configuration.Properties.PanningSensitivity =
					Utilities.Min(Configuration.Properties.PanningSensitivity, 0.1f);
				ImGui.NextColumn();

				ImGui.Text("Panning Inverted");
				ImGui.NextColumn();

				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.Checkbox("###PreferencesPanningInverted", ref Configuration.Properties.InvertedPanning);
				ImGui.NextColumn();

				GuiStyle.AddSpace();
				ImGui.Text("Rotation Strength");
				ImGui.NextColumn();

				GuiStyle.AddSpace();
				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.InputFloat("PreferencesRotationStrength", ref Configuration.Properties.RotationSensitivity,
					0.05f);
				Configuration.Properties.RotationSensitivity =
					Utilities.Min(Configuration.Properties.RotationSensitivity, 0.1f);
				ImGui.NextColumn();

				ImGui.Text("Rotation Inverted");
				ImGui.NextColumn();

				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.Checkbox("###PreferencesRotationInverted", ref Configuration.Properties.InvertedRotation);
				ImGui.NextColumn();

				GuiStyle.AddSpace();
				ImGui.Text("Zoom Strength");
				ImGui.NextColumn();

				GuiStyle.AddSpace();
				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.InputFloat("PreferencesZoomStrength", ref Configuration.Properties.ZoomStrength, 0.1f);
				Configuration.Properties.ZoomStrength = Utilities.Min(Configuration.Properties.ZoomStrength, 1.1f);
				ImGui.NextColumn();

				GuiStyle.AddSpace();
				ImGui.Text("Alpha as Semi-Transparent");
				ImGui.NextColumn();

				GuiStyle.AddSpace();
				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.Checkbox("###PreferencesAlpha", ref Configuration.Properties.RenderAlphaAsSemiTransparent);
				ImGui.NextColumn();

				ImGui.Text("Select Backfaces");
				ImGui.NextColumn();

				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.Checkbox("###PreferencesBackfaces", ref Configuration.Properties.AllowBackfaceSelection);
				ImGui.NextColumn();

				ImGui.Text("Show FPS");
				ImGui.NextColumn();

				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.Checkbox("###PreferencesShowFPS", ref Configuration.Properties.ShowFps);
				ImGui.NextColumn();

				ImGui.Text("Swap Camera Buttons");
				ImGui.NextColumn();

				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.Checkbox("###PreferencesSwapCameraControls", ref Configuration.Properties.SwapCameraControls);
				ImGui.NextColumn();

				
				GuiStyle.AddSpace();
				ImGui.Text("Render Ortho Like FFT");
				ImGui.NextColumn();

				GuiStyle.AddSpace();
				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.Checkbox("###PreferencesRenderFFTOrtho", ref Configuration.Properties.RenderFFTOrtho);
				ImGui.NextColumn();

				GuiStyle.AddSpace();
				ImGui.Text("AutoSave");
				ImGui.NextColumn();

				GuiStyle.AddSpace();
				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.Checkbox("###PreferencesAutoSave", ref Configuration.Properties.AutoSaveEnabled);
				ImGui.NextColumn();

				ImGui.Text("AutoSave Every X Minutes");
				ImGui.NextColumn();
				
				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.InputInt("###PreferencesAutoSaveDuration", ref Configuration.Properties.AutoSaveDuration);
				Configuration.Properties.AutoSaveDuration = Utilities.Min(Configuration.Properties.AutoSaveDuration, 1);
				ImGui.NextColumn();

				ImGui.Columns(1);

				GuiStyle.AddSpace();

				const int saveButtonWidth = GuiStyle.LabelWidth + 10 + GuiStyle.WidgetWidth;
				const int saveButtonHeight = 20;
				if (ImGui.Button("Save Preferences", new Vector2(saveButtonWidth, saveButtonHeight))) {
					Configuration.SaveConfiguration();
				}
			}
			ImGui.End();

			if (!windowIsOpen) {
				Gui.ShowPreferencesWindow = false;
			}
		}
	}
}