﻿using System.Numerics;
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

			int windowWidth = (directionWidth + 10) * 2 +
			                  elevationWidth +
			                  gapWidth +
			                  (int) ImGui.GetStyle().WindowPadding.X;

			int windowHeight = buttonHeight * 2 + (int) ImGui.GetStyle().WindowPadding.Y + 36 + 20;
			bool windowIsOpen = true;

			GuiStyle.SetNewUiToDefaultStyle();
			ImGui.GetStyle().WindowRounding = 3;
			ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[2]);
			ImGui.SetNextWindowSize(new Vector2(windowWidth, windowHeight));

			ImGui.Begin("Camera Controls", ref windowIsOpen, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse);
			{
				ImGui.PopFont();
				ImGui.Columns(5, "CameraControlsColumns", false);
				ImGui.SetColumnWidth(0, directionWidth + 10);
				ImGui.SetColumnWidth(1, directionWidth + 10);
				ImGui.SetColumnWidth(2, gapWidth);
				ImGui.SetColumnWidth(3, directionWidth + 10);
				ImGui.SetColumnWidth(4, directionWidth + 10);

				ImGui.Text("Bottom");
				ImGui.NextColumn();
				ImGui.NextColumn();
				ImGui.NextColumn();
				ImGui.Text("Top");
				ImGui.NextColumn();
				ImGui.NextColumn();


				if (ImGui.Button("NW##NWBottom", directionButtonSize)) {
					StageCamera.SpinCameraTo(StageCamera.CameraView.Northwest, StageCamera.CameraElevation.Bottom);
					// StageCamera.CameraHorizontalAngle = 225;
					// StageCamera.CameraHeightAngle = StageCamera.BottomElevation;
				}

				ImGui.NextColumn();

				if (ImGui.Button("NE##NEBottom", directionButtonSize)) {
					StageCamera.SpinCameraTo(StageCamera.CameraView.Northeast, StageCamera.CameraElevation.Bottom);
					// StageCamera.CameraHorizontalAngle = 315;
					// StageCamera.CameraHeightAngle = StageCamera.BottomElevation;
				}

				ImGui.NextColumn();
				ImGui.NextColumn();

				if (ImGui.Button("NW##NWTop", directionButtonSize)) {
					StageCamera.SpinCameraTo(StageCamera.CameraView.Northwest, StageCamera.CameraElevation.Top);
					// StageCamera.CameraHorizontalAngle = 225;
					// StageCamera.CameraHeightAngle = StageCamera.TopElevation;
				}

				ImGui.NextColumn();

				if (ImGui.Button("NE##NETop", directionButtonSize)) {
					StageCamera.SpinCameraTo(StageCamera.CameraView.Northeast, StageCamera.CameraElevation.Top);
					// StageCamera.CameraHorizontalAngle = 315;
					// StageCamera.CameraHeightAngle = StageCamera.TopElevation;
				}

				ImGui.NextColumn();

				if (ImGui.Button("SW##SWBottom", directionButtonSize)) {
					StageCamera.SpinCameraTo(StageCamera.CameraView.Southwest, StageCamera.CameraElevation.Bottom);
					// StageCamera.CameraHorizontalAngle = 135;
					// StageCamera.CameraHeightAngle = StageCamera.BottomElevation;
				}

				ImGui.NextColumn();

				if (ImGui.Button("SE##SEBottom", directionButtonSize)) {
					StageCamera.SpinCameraTo(StageCamera.CameraView.Southeast, StageCamera.CameraElevation.Bottom);
					// StageCamera.CameraHorizontalAngle = 45;
					// StageCamera.CameraHeightAngle = StageCamera.BottomElevation;
				}

				ImGui.NextColumn();
				ImGui.NextColumn();

				if (ImGui.Button("SW##SWTop", directionButtonSize)) {
					StageCamera.SpinCameraTo(StageCamera.CameraView.Southwest, StageCamera.CameraElevation.Top);
					// StageCamera.CameraHorizontalAngle = 135;
					// StageCamera.CameraHeightAngle = StageCamera.TopElevation;
				}

				ImGui.NextColumn();

				if (ImGui.Button("SE##SETop", directionButtonSize)) {
					StageCamera.SpinCameraTo(StageCamera.CameraView.Southeast, StageCamera.CameraElevation.Top);
					// StageCamera.CameraHorizontalAngle = 45;
					// StageCamera.CameraHeightAngle = StageCamera.TopElevation;
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