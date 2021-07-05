using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using GaneshaDx.Common;
using GaneshaDx.Resources;
using GaneshaDx.Resources.ContentDataTypes.MeshAnimations;
using GaneshaDx.UserInterface.GuiDefinitions;
using ImGuiNET;

namespace GaneshaDx.UserInterface.GuiForms {
	public static class GuiWindowEditMeshAnimations {
		private static int _selectedFrameState;
		private static int _selectedMeshId;
		private static int _selectedStateId;

		public static void Render() {
			const int windowHeight = 700;
			const int windowWidth = 550;
			bool windowIsOpen = true;

			GuiStyle.SetNewUiToDefaultStyle();
			ImGui.GetStyle().WindowRounding = 3;
			ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[2]);
			ImGui.SetNextWindowSize(new Vector2(windowWidth, windowHeight));

			ImGui.Begin("Mesh Animations", ref windowIsOpen, ImGuiWindowFlags.NoResize);
			{
				GuiStyle.SetNewUiToDefaultStyle();
				RenderFrameStatePanel();
				RenderKeyFrames();
			}
			ImGui.End();

			if (!windowIsOpen) {
				Gui.ShowMeshAnimationsWindow = false;
			}
		}

		private static void RenderFrameStatePanel() {
			ImGui.Columns(2, "PreferencesControls", false);
			ImGui.SetColumnWidth(0, 194);
			ImGui.SetColumnWidth(1, GuiStyle.WidgetWidth + 10);

			ImGui.Text("Frame State");
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
			ImGui.InputInt("##FrameStateId", ref _selectedFrameState);
			_selectedFrameState = Utilities.Clamp(_selectedFrameState, 1, 128);
			ImGui.NextColumn();

			ImGui.Separator();

			MeshAnimationFrameState selectedFrameState =
				CurrentMapState.StateData.MeshAnimationInstructions.FrameStates[_selectedFrameState - 1];

			List<string> keyFrameTypes = Enum.GetNames(typeof(MeshAnimationKeyFrameType)).ToList();
			for (int keyframeTypeIndex = 0; keyframeTypeIndex < keyFrameTypes.Count; keyframeTypeIndex++) {
				keyFrameTypes[keyframeTypeIndex] = Regex.Replace(keyFrameTypes[keyframeTypeIndex], "(\\B[A-Z])", " $1");
			}

			const int comboWidth = 90;
			const int inputWidth = 60;

			ImGui.Columns(6, "AnimationStatePropertiesColumns", false);
			ImGui.SetColumnWidth(0, comboWidth + 10);
			ImGui.SetColumnWidth(1, inputWidth + 20);
			ImGui.SetColumnWidth(2, comboWidth + 10);
			ImGui.SetColumnWidth(3, inputWidth + 20);
			ImGui.SetColumnWidth(4, comboWidth + 10);
			ImGui.SetColumnWidth(5, inputWidth + 20);

			ImGui.Text("Position (X, Y, Z)");
			ImGui.NextColumn();
			ImGui.NextColumn();
			ImGui.NextColumn();
			ImGui.NextColumn();
			ImGui.NextColumn();
			ImGui.NextColumn();


			ImGui.SetNextItemWidth(comboWidth);
			int dropDownValue = (int) selectedFrameState.PositionKeyFrameTypes[0];
			ImGui.Combo("##FrameStatePositionXTween", ref dropDownValue, keyFrameTypes.ToArray(), keyFrameTypes.Count);
			selectedFrameState.PositionKeyFrameTypes[0] = (MeshAnimationKeyFrameType) dropDownValue;
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			int positionX = (int) selectedFrameState.Position.X;
			ImGui.DragInt("##FrameStatePositionX", ref positionX, 0.1f);
			selectedFrameState.Position.X = positionX;
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(comboWidth);
			dropDownValue = (int) selectedFrameState.PositionKeyFrameTypes[1];
			ImGui.Combo("##FrameStatePositionYTween", ref dropDownValue, keyFrameTypes.ToArray(), keyFrameTypes.Count);
			selectedFrameState.PositionKeyFrameTypes[1] = (MeshAnimationKeyFrameType) dropDownValue;
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			int positionY = (int) selectedFrameState.Position.Y;
			ImGui.DragInt("##FrameStatePositionY", ref positionY, 0.1f);
			selectedFrameState.Position.Y = positionY;
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(comboWidth);
			dropDownValue = (int) selectedFrameState.PositionKeyFrameTypes[2];
			ImGui.Combo("##FrameStatePositionZTween", ref dropDownValue, keyFrameTypes.ToArray(), keyFrameTypes.Count);
			selectedFrameState.PositionKeyFrameTypes[2] = (MeshAnimationKeyFrameType) dropDownValue;
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			int positionZ = (int) selectedFrameState.Position.Z;
			ImGui.DragInt("##FrameStatePositionZ", ref positionZ, 0.1f);
			selectedFrameState.Position.Z = positionZ;
			ImGui.NextColumn();

			GuiStyle.AddSpace();
			ImGui.Text("Rotation (X, Y, Z)");
			ImGui.NextColumn();
			ImGui.NextColumn();
			ImGui.NextColumn();
			ImGui.NextColumn();
			ImGui.NextColumn();
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(comboWidth);
			dropDownValue = (int) selectedFrameState.RotationKeyFrameTypes[0];
			ImGui.Combo("##FrameStateRotationXTween", ref dropDownValue, keyFrameTypes.ToArray(), keyFrameTypes.Count);
			selectedFrameState.RotationKeyFrameTypes[0] = (MeshAnimationKeyFrameType) dropDownValue;
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			double rotationX = selectedFrameState.Rotation.X;
			float degreesX = (float) (rotationX * 360);
			ImGui.DragFloat("##FrameStateRotationX", ref degreesX, 0.1f);
			rotationX = degreesX / 360;
			selectedFrameState.Rotation.X = (float) rotationX;
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(comboWidth);
			dropDownValue = (int) selectedFrameState.RotationKeyFrameTypes[1];
			ImGui.Combo("##FrameStateRotationYTween", ref dropDownValue, keyFrameTypes.ToArray(), keyFrameTypes.Count);
			selectedFrameState.RotationKeyFrameTypes[1] = (MeshAnimationKeyFrameType) dropDownValue;
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			double rotationY = selectedFrameState.Rotation.Y;
			float degreesY = (float) (rotationY * 360);
			ImGui.DragFloat("##FrameStateRotationY", ref degreesY, 0.1f);
			rotationY = degreesY / 360;
			selectedFrameState.Rotation.Y = (float) rotationY;
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(comboWidth);
			dropDownValue = (int) selectedFrameState.RotationKeyFrameTypes[2];
			ImGui.Combo("##FrameStateRotationZTween", ref dropDownValue, keyFrameTypes.ToArray(), keyFrameTypes.Count);
			selectedFrameState.RotationKeyFrameTypes[2] = (MeshAnimationKeyFrameType) dropDownValue;
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			double rotationZ = selectedFrameState.Rotation.Z;
			float degreesZ = (float) (rotationZ * 360);
			ImGui.DragFloat("##FrameStateRotationZ", ref degreesZ, 0.1f);
			rotationZ = degreesZ / 360;
			selectedFrameState.Rotation.Z = (float) rotationZ;
			ImGui.NextColumn();

			GuiStyle.AddSpace();
			ImGui.Text("Scale (X, Y, Z)");
			ImGui.NextColumn();
			ImGui.NextColumn();
			ImGui.NextColumn();
			ImGui.NextColumn();
			ImGui.NextColumn();
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(comboWidth);
			dropDownValue = (int) selectedFrameState.ScaleKeyFrameTypes[0];
			ImGui.Combo("##FrameStateScaleXTween", ref dropDownValue, keyFrameTypes.ToArray(), keyFrameTypes.Count);
			selectedFrameState.ScaleKeyFrameTypes[0] = (MeshAnimationKeyFrameType) dropDownValue;
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			ImGui.DragFloat("##FrameStateScaleX", ref selectedFrameState.Scale.X, 0.1f);
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(comboWidth);
			dropDownValue = (int) selectedFrameState.ScaleKeyFrameTypes[1];
			ImGui.Combo("##FrameStateScaleYTween", ref dropDownValue, keyFrameTypes.ToArray(), keyFrameTypes.Count);
			selectedFrameState.ScaleKeyFrameTypes[1] = (MeshAnimationKeyFrameType) dropDownValue;
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			ImGui.DragFloat("##FrameStateScaleY", ref selectedFrameState.Scale.Y, 0.1f);
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(comboWidth);
			dropDownValue = (int) selectedFrameState.ScaleKeyFrameTypes[2];
			ImGui.Combo("##FrameStateScaleZTween", ref dropDownValue, keyFrameTypes.ToArray(), keyFrameTypes.Count);
			selectedFrameState.ScaleKeyFrameTypes[2] = (MeshAnimationKeyFrameType) dropDownValue;
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			ImGui.DragFloat("##FrameStateScaleZ", ref selectedFrameState.Scale.Z, 0.1f);
			ImGui.NextColumn();

			ImGui.Columns(1);
			ImGui.Separator();
			GuiStyle.AddSpace();
		}


		private static void RenderKeyFrames() {
			const int optionsLabelWidth = 40;
			const int optionsInputWidth = 90;
			const int spacing = 35;
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
			const int inputWidth = 50;
			const int buttonWidth = 25;

			ImGui.Columns(1);
			ImGui.Columns(5, "KeyframeTable", false);
			ImGui.SetColumnWidth(0, idWidth);
			ImGui.SetColumnWidth(1, inputWidth + 10);
			ImGui.SetColumnWidth(2, buttonWidth + 10);
			ImGui.SetColumnWidth(3, inputWidth + 10);
			ImGui.SetColumnWidth(4, inputWidth + 10);


			ImGui.Text("\n\nId");
			GuiStyle.AddSpace(5);

			ImGui.NextColumn();

			ImGui.Text("\nFrame\nState Id");
			GuiStyle.AddSpace(5);

			ImGui.NextColumn();

			ImGui.Text("");
			GuiStyle.AddSpace(5);
			ImGui.NextColumn();

			ImGui.Text("\n\nDuration");
			GuiStyle.AddSpace(5);
			ImGui.NextColumn();

			ImGui.Text("\n\nNext Id");
			GuiStyle.AddSpace(5);
			ImGui.NextColumn();

			int linkIndex = (_selectedMeshId - 1) + (_selectedStateId * 8);
			MeshAnimation meshAnimation = CurrentMapState.StateData.MeshAnimationInstructions.Links[linkIndex];

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
				ImGui.NextColumn();

				ImGui.SetNextItemWidth(inputWidth);
				ImGui.DragInt("##MeshAnimationFrameNextFrameId" + i, ref meshAnimation.Frames[i].NextFrameId);
				ImGui.NextColumn();
			}
		}
	}
}