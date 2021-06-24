using System.Numerics;
using GaneshaDx.Environment;
using GaneshaDx.UserInterface.GuiDefinitions;
using ImGuiNET;

namespace GaneshaDx.UserInterface.GuiForms {
	public static class GuiWindowCameraControls {
		public static void Render() {
			const int directionWidth = 50;
			const int elevationWidth = 120;
			const int gapWidth = 20;
			const int buttonHeight = 20;

			Vector2 directionButtonSize = new Vector2(directionWidth, buttonHeight);
			Vector2 elevationButtonSize = new Vector2(elevationWidth, buttonHeight);

			int windowWidth = (directionWidth + 10) * 2 +
			                  elevationWidth +
			                  gapWidth +
			                  (int) ImGui.GetStyle().WindowPadding.X;

			int windowHeight = buttonHeight * 2 + (int) ImGui.GetStyle().WindowPadding.Y + 36;
			bool windowIsOpen = true;

			GuiStyle.SetNewUiToDefaultStyle();
			ImGui.GetStyle().WindowRounding = 3; 
			ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[2]);
			ImGui.SetNextWindowSize(new Vector2(windowWidth, windowHeight));
			
			ImGui.Begin("Camera Controls", ref windowIsOpen, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse);
			{
				ImGui.PopFont();
				ImGui.Columns(4, "CameraControlsColumns", false);
				ImGui.SetColumnWidth(0, directionWidth + 10);
				ImGui.SetColumnWidth(1, directionWidth + 10);
				ImGui.SetColumnWidth(2, gapWidth);
				ImGui.SetColumnOffset(3, elevationWidth + 10);

				if (ImGui.Button("NW", directionButtonSize)) {
					StageCamera.CameraHorizontalAngle = 225;
				}

				ImGui.NextColumn();

				if (ImGui.Button("NE", directionButtonSize)) {
					StageCamera.CameraHorizontalAngle = 315;
				}

				ImGui.NextColumn();
				ImGui.NextColumn();

				if (ImGui.Button("Top Elevation", elevationButtonSize)) {
					StageCamera.CameraHeightAngle = StageCamera.TopElevation;
				}

				ImGui.NextColumn();

				if (ImGui.Button("SW", directionButtonSize)) {
					StageCamera.CameraHorizontalAngle = 135;
				}

				ImGui.NextColumn();

				if (ImGui.Button("SE", directionButtonSize)) {
					StageCamera.CameraHorizontalAngle = 45;
				}

				ImGui.NextColumn();
				ImGui.NextColumn();

				if (ImGui.Button("Bottom Elevation", elevationButtonSize)) {
					StageCamera.CameraHeightAngle = StageCamera.BottomElevation;
				}

				ImGui.Columns(1);
			}
			ImGui.End();

			if (!windowIsOpen) {
				Gui.ShowCameraControlWindow = false;
			}
		}
	}
}