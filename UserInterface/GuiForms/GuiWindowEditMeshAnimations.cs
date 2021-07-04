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
		private static int _selectedFrameState = 0;
		private static int _selectedMeshId;
		private static int _selectedStateId;

		public static void Render() {
			const int windowHeight = 615;
			const int windowWidth = 330;
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

			const int labelWidth = 65;
			const int inputWidth = 40;
			const int dropDownWidth = 90;
			
			ImGui.Columns(5, "AnimationStateProperties", false);
			ImGui.SetColumnWidth(0, labelWidth);
			ImGui.SetColumnWidth(1, dropDownWidth + 10);
			ImGui.SetColumnWidth(2, inputWidth + 10);
			ImGui.SetColumnWidth(3, inputWidth + 10);
			ImGui.SetColumnWidth(4, inputWidth + 10);

			ImGui.Text("Position");
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(dropDownWidth);
// ImGui.Combo("##FrameStatePositionTweenType", ref dropDownValue, keyFrameTypes.ToArray(), keyFrameTypes.Count);
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			ImGui.DragFloat("##FrameStatePositionX", ref selectedFrameState.Position.X, 0.1f);
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			ImGui.DragFloat("##FrameStatePositionY", ref selectedFrameState.Position.Y, 0.1f);
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			ImGui.DragFloat("##FrameStatePositionZ", ref selectedFrameState.Position.Z, 0.1f);
			ImGui.NextColumn();

			ImGui.Text("Rotation");
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(dropDownWidth);
	// ImGui.Combo("##FrameStateRotationTweenType", ref dropDownValue, keyFrameTypes.ToArray(), keyFrameTypes.Count);
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			ImGui.DragFloat("##FrameStateRotationX", ref selectedFrameState.Rotation.X, 0.1f);
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			ImGui.DragFloat("##FrameStateRotationY", ref selectedFrameState.Rotation.Y, 0.1f);
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			ImGui.DragFloat("##FrameStateRotationZ", ref selectedFrameState.Rotation.Z, 0.1f);
			ImGui.NextColumn();

			ImGui.Text("Scale");
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(dropDownWidth);
	//ImGui.Combo("##FrameStateScaleTweenType", ref dropDownValue, keyFrameTypes.ToArray(), keyFrameTypes.Count);
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			ImGui.DragFloat("##FrameStateScaleX", ref selectedFrameState.Scale.X, 0.1f);
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(inputWidth);
			ImGui.DragFloat("##FrameStateScaleY", ref selectedFrameState.Scale.Y, 0.1f);
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
			ImGui.NextColumn();

			ImGui.Text("");
			ImGui.NextColumn();

			ImGui.Text("State");
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(optionsInputWidth);
			ImGui.InputInt("##stateId", ref _selectedStateId);
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

			for (int i = 1; i <= 16; i++) {
				ImGui.Text(i.ToString());
				ImGui.NextColumn();

				int val = 0;
				ImGui.SetNextItemWidth(inputWidth);
				ImGui.DragInt("##blaha" + i, ref val);
				ImGui.NextColumn();

				ImGui.Button(">", new Vector2(buttonWidth, 20));
				ImGui.NextColumn();

				ImGui.SetNextItemWidth(inputWidth);
				ImGui.DragInt("##blahb" + i, ref val);
				ImGui.NextColumn();

				ImGui.SetNextItemWidth(inputWidth);
				ImGui.DragInt("##blahc" + i, ref val);
				ImGui.NextColumn();
			}
		}
	}
}