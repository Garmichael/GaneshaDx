using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
using GaneshaDx.Common;
using GaneshaDx.Rendering;
using GaneshaDx.Resources;
using GaneshaDx.Resources.ContentDataTypes.MeshAnimations;
using GaneshaDx.UserInterface.GuiDefinitions;
using ImGuiNET;

namespace GaneshaDx.UserInterface.GuiForms {
	public static class GuiWindowEditMeshAnimations {
		private const int LeftPanelWidth = 280;
		private const int RightPanelWidth = 490;
		private const int WindowWidth = LeftPanelWidth + RightPanelWidth;
		private const int WindowHeight = 550;
		private const int FooterHeight = 70;

		private static int _selectedFrameState;
		private static int _selectedMeshId;
		private static int _selectedStateId;

		public static void Render() {
			bool windowIsOpen = true;

			GuiStyle.SetNewUiToDefaultStyle();
			ImGui.GetStyle().WindowRounding = 3;
			ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[2]);
			ImGui.SetNextWindowSize(new Vector2(WindowWidth, WindowHeight));

			ImGui.Begin("Mesh Animations", ref windowIsOpen, ImGuiWindowFlags.NoResize);
			{
				GuiStyle.SetNewUiToDefaultStyle();
				ImGui.GetStyle().FrameRounding = 0;

				ImGui.Columns(2, "MeshAnimationsMainLayout", false);
				ImGui.SetColumnWidth(0, LeftPanelWidth);
				ImGui.SetColumnWidth(1, RightPanelWidth);

				ImGui.BeginChild("KeyFrames", new Vector2(LeftPanelWidth, WindowHeight - FooterHeight));
				RenderKeyFrames();
				ImGui.EndChild();

				ImGui.NextColumn();

				ImGui.BeginChild("KeyFrameStates", new Vector2(RightPanelWidth, WindowHeight - FooterHeight));
				RenderFrameStatePanel();
				ImGui.EndChild();

				ImGui.Columns(1);

				RenderControlsPanel();
			}
			ImGui.End();

			if (!windowIsOpen) {
				Gui.ShowMeshAnimationsWindow = false;
			}
		}

		private static void RenderKeyFrames() {
			const int optionsLabelWidth = 40;
			const int optionsInputWidth = 75;
			const int spacing = 10;
			ImGui.Columns(5, "KeyframeTableHeader", false);
			ImGui.SetColumnWidth(0, optionsLabelWidth);
			ImGui.SetColumnWidth(1, optionsInputWidth + 10);
			ImGui.SetColumnWidth(2, spacing);
			ImGui.SetColumnWidth(3, optionsLabelWidth);
			ImGui.SetColumnWidth(4, optionsInputWidth + 10);

			ImGui.Text("Mesh");
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(optionsInputWidth);
			ImGui.InputInt("##meshId", ref _selectedMeshId);
			_selectedMeshId = Utilities.Clamp(_selectedMeshId, 1, 8);
			ImGui.NextColumn();

			ImGui.Text("");
			ImGui.NextColumn();

			ImGui.Text("State");
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(optionsInputWidth);
			ImGui.InputInt("##stateId", ref _selectedStateId);
			_selectedStateId = Utilities.Clamp(_selectedStateId, 0, 7);
			ImGui.NextColumn();

			const int idWidth = 30;
			const int inputWidth = 55;
			const int buttonWidth = 25;

			GuiStyle.AddSpace();
			
			ImGui.Columns(1);
			ImGui.Columns(5, "KeyframeTable", false);
			ImGui.SetColumnWidth(0, idWidth);
			ImGui.SetColumnWidth(1, inputWidth + 10);
			ImGui.SetColumnWidth(2, buttonWidth + 10);
			ImGui.SetColumnWidth(3, inputWidth + 10);
			ImGui.SetColumnWidth(4, inputWidth + 10);


			ImGui.Text("Id");
			GuiStyle.AddSpace(5);

			ImGui.NextColumn();

			ImGui.Text("KeyFrame");
			GuiStyle.AddSpace(5);

			ImGui.NextColumn();

			ImGui.Text("");
			GuiStyle.AddSpace(5);
			ImGui.NextColumn();

			ImGui.Text("Duration");
			GuiStyle.AddSpace(5);
			ImGui.NextColumn();

			ImGui.Text("Next Id");
			GuiStyle.AddSpace(5);
			ImGui.NextColumn();

			int linkIndex = (_selectedMeshId - 1) + (_selectedStateId * 8);
			MeshAnimation meshAnimation = CurrentMapState.StateData.MeshAnimationInstructions.MeshAnimations[linkIndex];

			for (int i = 0; i < 16; i++) {
				ImGui.Text((i + 1).ToString());
				ImGui.NextColumn();

				ImGui.SetNextItemWidth(inputWidth);
				ImGui.DragInt("##MeshAnimationFrameFrameStateId" + i, ref meshAnimation.Frames[i].FrameStateId);
				ImGui.NextColumn();

				if (ImGui.Button(">##MeshAnimationGoToFrameState" + i, new Vector2(buttonWidth, 20))) {
					_selectedFrameState = meshAnimation.Frames[i].FrameStateId;
				}

				ImGui.NextColumn();

				ImGui.SetNextItemWidth(inputWidth);
				ImGui.DragInt("##MeshAnimationFrameDuration" + i, ref meshAnimation.Frames[i].Duration);
				meshAnimation.Frames[i].Duration = Utilities.Min(meshAnimation.Frames[i].Duration, 1);
				ImGui.NextColumn();

				ImGui.SetNextItemWidth(inputWidth);
				ImGui.DragInt("##MeshAnimationFrameNextFrameId" + i, ref meshAnimation.Frames[i].NextFrameId);
				ImGui.NextColumn();
			}
		}

		private static void RenderFrameStatePanel() {
			RenderFrameStatePanelHeader();

			MeshAnimationKeyFrame selectedKeyFrame =
				CurrentMapState.StateData.MeshAnimationInstructions.KeyFrames[_selectedFrameState - 1];

			RenderFrameStatePanelValueSet(
				"Position",
				selectedKeyFrame.PositionTweenTypes,
				selectedKeyFrame.Position,
				true,
				selectedKeyFrame.PositionStartPercents,
				selectedKeyFrame.PositionEndPercents
			);
			GuiStyle.AddSpace();

			RenderFrameStatePanelValueSet(
				"Rotation",
				selectedKeyFrame.RotationTweenTypes,
				selectedKeyFrame.Rotation,
				false,
				selectedKeyFrame.RotationStartPercents,
				selectedKeyFrame.RotationEndPercents
			);
			GuiStyle.AddSpace();

			RenderFrameStatePanelValueSet(
				"Scale",
				selectedKeyFrame.ScaleTweenTypes,
				selectedKeyFrame.Scale,
				false,
				selectedKeyFrame.ScaleStartPercents,
				selectedKeyFrame.ScaleEndPercents
			);
			GuiStyle.AddSpace();
		}

		private static void RenderFrameStatePanelHeader() {
			const int labelWidth = 80;
			const int inputWidth = 85;
			ImGui.Columns(3, "FrameStateTable", false);
			ImGui.SetColumnWidth(0, RightPanelWidth - inputWidth - 40 - labelWidth - 10);
			ImGui.SetColumnWidth(1, labelWidth + 10);
			ImGui.SetColumnWidth(2, inputWidth + 10);

			ImGui.NextColumn();

			ImGui.Text("KeyFrame Id");
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			ImGui.InputInt("##FrameStateId", ref _selectedFrameState);
			_selectedFrameState = Utilities.Clamp(_selectedFrameState, 1, 128);
			ImGui.NextColumn();
			GuiStyle.AddSpace();
		}

		private static void RenderFrameStatePanelValueSet(
			string type,
			List<MeshAnimationTweenType> sourceKeyFrameTypes,
			List<double> values,
			bool valueIsInt,
			List<double> startIndexes,
			List<double> endIndexes
		) {
			string[] keyFrameTypes = Enum.GetNames(typeof(MeshAnimationTweenType));
			for (int keyframeTypeIndex = 0; keyframeTypeIndex < keyFrameTypes.Length; keyframeTypeIndex++) {
				keyFrameTypes[keyframeTypeIndex] = Regex.Replace(keyFrameTypes[keyframeTypeIndex], "(\\B[A-Z])", " $1");
			}

			const int inputWidth = 100;
			const int labelWidth = 60;

			ImGui.Columns(4, "AnimationStatePropertiesColumns" + type, false);
			ImGui.SetColumnWidth(0, labelWidth + 30);
			ImGui.SetColumnWidth(1, inputWidth + 30);
			ImGui.SetColumnWidth(2, inputWidth + 30);
			ImGui.SetColumnWidth(3, inputWidth + 30);

			ImGui.PushStyleColor(ImGuiCol.Text, GuiStyle.ColorPalette[ColorName.HighlightedText]);
			ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[2]);
			ImGui.Text(type);

			ImGui.NextColumn();

			ImGui.Text("                X");
			ImGui.NextColumn();

			ImGui.Text("                Y");
			ImGui.NextColumn();

			ImGui.Text("                Z");
			ImGui.NextColumn();

			ImGui.PopFont();
			ImGui.PopStyleColor();

			ImGui.Text("Tween Type");
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);

			int dropDownValue = (int) sourceKeyFrameTypes[0];
			ImGui.Combo("##FrameState" + type + "XTween", ref dropDownValue, keyFrameTypes, keyFrameTypes.Length);
			sourceKeyFrameTypes[0] = (MeshAnimationTweenType) dropDownValue;
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			dropDownValue = (int) sourceKeyFrameTypes[1];
			ImGui.Combo("##FrameState" + type + "YTween", ref dropDownValue, keyFrameTypes, keyFrameTypes.Length);
			sourceKeyFrameTypes[1] = (MeshAnimationTweenType) dropDownValue;
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			dropDownValue = (int) sourceKeyFrameTypes[2];
			ImGui.Combo("##FrameState" + type + "ZTween", ref dropDownValue, keyFrameTypes, keyFrameTypes.Length);
			sourceKeyFrameTypes[2] = (MeshAnimationTweenType) dropDownValue;
			ImGui.NextColumn();

			ImGui.Text("Values");
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			if (valueIsInt) {
				int valueX = (int) values[0];
				ImGui.InputInt("##FrameState" + type + "XValueInt", ref valueX);
				values[0] = valueX;
			} else {
				float valueX = (float) values[0];
				ImGui.InputFloat("##FrameState" + type + "XValueFloat", ref valueX, 0.1f);
				values[0] = valueX;
			}

			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			if (valueIsInt) {
				int valueY = (int) values[1];
				ImGui.InputInt("##FrameState" + type + "YValue", ref valueY);
				values[1] = valueY;
			} else {
				float valueY = (float) values[1];
				ImGui.InputFloat("##FrameState" + type + "YValue", ref valueY, 0.1f);
				values[1] = valueY;
			}

			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			if (valueIsInt) {
				int valueZ = (int) values[2];
				ImGui.InputInt("##FrameState" + type + "ZValue", ref valueZ);
				values[2] = valueZ;
			} else {
				float valueZ = (float) values[2];
				ImGui.InputFloat("##FrameState" + type + "ZValue", ref valueZ, 0.1f);
				values[2] = valueZ;
			}

			ImGui.NextColumn();


			ImGui.Text("Start Percent");
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			float startX = (float) startIndexes[0];
			ImGui.InputFloat("##FrameState" + type + "XStart", ref startX, 0.1f);
			startIndexes[0] = startX;
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			float startY = (float) startIndexes[1];
			ImGui.InputFloat("##FrameState" + type + "YStart", ref startY, 0.1f);
			startIndexes[1] = startY;
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			float startZ = (float) startIndexes[2];
			ImGui.InputFloat("##FrameState" + type + "ZStart", ref startZ, 0.1f);
			startIndexes[2] = startZ;
			ImGui.NextColumn();

			ImGui.Text("End Percent");
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			float endX = (float) endIndexes[0];
			ImGui.InputFloat("##FrameState" + type + "XEnd", ref endX, 0.1f);
			endIndexes[0] = endX;
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			float endY = (float) endIndexes[1];
			ImGui.InputFloat("##FrameState" + type + "YEnd", ref endY, 0.1f);
			endIndexes[1] = endY;
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			float endZ = (float) endIndexes[2];
			ImGui.InputFloat("##FrameState" + type + "ZEnd", ref endZ, 0.1f);
			endIndexes[2] = endZ;
			ImGui.NextColumn();

			ImGui.Columns(1);
			GuiStyle.AddSpace();
		}

		private static void RenderControlsPanel() {
			ImGui.SetCursorPosX(WindowWidth / 2 - 40);
			GuiStyle.SetNewUiToDefaultStyle();

			if (!MeshAnimationController.AnimationsPlaying) {
				GuiStyle.SetElementStyle(ElementStyle.ButtonSelected);
			} else {
				GuiStyle.SetNewUiToDefaultStyle();
			}

			ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[3]);
			if (ImGui.Button("O##MeshAnimationStop")) {
				MeshAnimationController.StopAnimations();
			}

			ImGui.PopFont();

			ImGui.SameLine();

			if (MeshAnimationController.AnimationsPlaying) {
				GuiStyle.SetElementStyle(ElementStyle.ButtonSelected);
			} else {
				GuiStyle.SetNewUiToDefaultStyle();
			}

			ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[3]);
			if (ImGui.Button("P##MeshAnimationPlay")) {
				MeshAnimationController.PlayAnimations();
			}

			ImGui.PopFont();
		}
	}
}