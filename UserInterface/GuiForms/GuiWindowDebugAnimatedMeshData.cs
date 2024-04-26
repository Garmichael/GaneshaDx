using System;
using System.Collections.Generic;
using GaneshaDx.Resources;
using GaneshaDx.Resources.ContentDataTypes.MeshAnimations;
using GaneshaDx.UserInterface.GuiDefinitions;
using ImGuiNET;
using TextCopy;

namespace GaneshaDx.UserInterface.GuiForms {
	public static class GuiWindowDebugAnimatedMeshData {
		private static bool _showUnused = false;

		public static void Render() {
			GuiStyle.SetNewUiToDefaultStyle();
			ImGui.GetStyle().WindowRounding = 3;
			ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[2]);

			bool windowIsOpen = true;

			ImGui.Begin("Animation Bytes", ref windowIsOpen);
			{
				ImGui.Checkbox("Show Unused? ", ref _showUnused);

				MeshAnimationInstructions set = CurrentMapState.StateData.MeshAnimationInstructions;
				ImGui.PopFont();

				if (ImGui.CollapsingHeader("Headers")) {
					const int inputWidth = 40;
						ImGui.GetStyle().Colors[(int) ImGuiCol.Header] = GuiStyle.ColorPalette[ColorName.Lightest];
						ImGui.GetStyle().Colors[(int) ImGuiCol.Text] = GuiStyle.ColorPalette[ColorName.Dark];

						ImGui.Indent();
						
						if(ImGui.CollapsingHeader("Keyframes Header")) {
							GuiStyle.SetNewUiToDefaultStyle();
							ImGui.Columns(4, "HeaderData", false);
							ImGui.SetColumnWidth(0, inputWidth + 10);
							ImGui.SetColumnWidth(1, inputWidth + 10);
							ImGui.SetColumnWidth(2, inputWidth + 10);
							ImGui.SetColumnWidth(3, inputWidth + 10);

							for (int dataIndex = 0; dataIndex < set.KeyframesHeader.Count; dataIndex++) {
								int data = set.KeyframesHeader[dataIndex];
								ImGui.SetNextItemWidth(inputWidth);
								ImGui.DragInt("###keyframeHeader_" + dataIndex, ref data);
								set.KeyframesHeader[dataIndex] = (byte) data;
								ImGui.NextColumn();
								if (dataIndex == 3) {
									
								}
							}
							ImGui.Columns(1);
						}
						
						if(ImGui.CollapsingHeader("Mesh Animations Header")) {
							GuiStyle.SetNewUiToDefaultStyle();
							ImGui.Columns(4, "HeaderData", false);
							ImGui.SetColumnWidth(0, inputWidth + 10);
							ImGui.SetColumnWidth(1, inputWidth + 10);
							ImGui.SetColumnWidth(2, inputWidth + 10);
							ImGui.SetColumnWidth(3, inputWidth + 10);

							for (int dataIndex = 0; dataIndex < set.MeshAnimationsHeader.Count; dataIndex++) {
								int data = set.MeshAnimationsHeader[dataIndex];
								ImGui.SetNextItemWidth(inputWidth);
								ImGui.DragInt("###meshAnimationsHeader_" + dataIndex, ref data);
								set.MeshAnimationsHeader[dataIndex] = (byte) data;
								ImGui.NextColumn();
								if (dataIndex == 3) {
									
								}
							}
							ImGui.Columns(1);
						}
						
						if(ImGui.CollapsingHeader("Unknown Chunk Header")) {
							GuiStyle.SetNewUiToDefaultStyle();
							ImGui.Columns(4, "HeaderData", false);
							ImGui.SetColumnWidth(0, inputWidth + 10);
							ImGui.SetColumnWidth(1, inputWidth + 10);
							ImGui.SetColumnWidth(2, inputWidth + 10);
							ImGui.SetColumnWidth(3, inputWidth + 10);

							for (int dataIndex = 0; dataIndex < set.UnknownChunkHeader.Count; dataIndex++) {
								int data = set.UnknownChunkHeader[dataIndex];
								ImGui.SetNextItemWidth(inputWidth);
								ImGui.DragInt("###unknownChunkHeader_" + dataIndex, ref data);
								set.UnknownChunkHeader[dataIndex] = (byte) data;
								ImGui.NextColumn();
								if (dataIndex == 3) {
									
								}
							}
							ImGui.Columns(1);
						}
						
						ImGui.Unindent();
						GuiStyle.SetNewUiToDefaultStyle();
				}
				
				if (ImGui.CollapsingHeader("Keyframes")) {
					ImGui.Indent();

					const int inputWidth = 40;
					for (int setIndex = 0; setIndex < set.KeyFrames.Count; setIndex++) {
						MeshAnimationKeyFrame keyFrame = set.KeyFrames[setIndex];

						GuiStyle.SetNewUiToDefaultStyle();

						bool highlightHeader = false;

						foreach (int data in keyFrame.Properties) {
							if (data != 0) {
								highlightHeader = true;
								break;
							}
						}

						if (highlightHeader) {
							ImGui.GetStyle().Colors[(int) ImGuiCol.Header] = GuiStyle.ColorPalette[ColorName.Lightest];
							ImGui.GetStyle().Colors[(int) ImGuiCol.Text] = GuiStyle.ColorPalette[ColorName.Dark];
						}

						if ((highlightHeader || _showUnused) && ImGui.CollapsingHeader("Keyframe " + (setIndex + 1))) {
							GuiStyle.SetNewUiToDefaultStyle();
							ImGui.Columns(5, "InstructionSetData", false);
							ImGui.SetColumnWidth(0, 30);
							ImGui.SetColumnWidth(1, inputWidth + 10);
							ImGui.SetColumnWidth(2, inputWidth + 10);
							ImGui.SetColumnWidth(3, inputWidth + 10);
							ImGui.SetColumnWidth(4, inputWidth + 10);

							for (int dataIndex = 0; dataIndex < keyFrame.Properties.Count;) {
								ImGui.Text(dataIndex + ": ");
								ImGui.NextColumn();

								for (int field = 0; field < 4; field++) {
									int value = keyFrame.Properties[dataIndex];
									ImGui.SetNextItemWidth(inputWidth);
									ImGui.DragInt("###instruction_" + setIndex + "_" + dataIndex, ref value);
									keyFrame.Properties[dataIndex] = value;
									ImGui.NextColumn();
									dataIndex++;
								}
							}

							ImGui.Columns(1);

							if (ImGui.Button("Copy##instructionSet_" + setIndex)) {
								List<string> bytes = new List<string>();
								foreach (int x in keyFrame.Properties) {
									bytes.Add(x.ToString());
								}

								Clipboard.SetText(string.Join(",", bytes));
							}

							ImGui.SameLine();

							if (ImGui.Button("Paste##instructionSet_" + setIndex)) {
								string bytes = Clipboard.GetText();
								string[] byteArray = bytes.Split(",");
								for (int index = 0; index < byteArray.Length; index++) {
									string entry = byteArray[index];
									byte byteValue = (byte) int.Parse(entry);
									keyFrame.Properties[index] = byteValue;
								}
							}
						}
					}

					ImGui.Unindent();
				}

				if (ImGui.CollapsingHeader("Mesh Animations")) {
					ImGui.Indent();

					const int inputWidth = 40;
					for (int setIndex = 0; setIndex < set.MeshAnimations.Count; setIndex++) {
						MeshAnimation link = set.MeshAnimations[setIndex];

						GuiStyle.SetNewUiToDefaultStyle();

						bool highlightHeader = false;

						foreach (int data in link.RawData) {
							if (data != 0) {
								highlightHeader = true;
								break;
							}
						}

						if (highlightHeader) {
							ImGui.GetStyle().Colors[(int) ImGuiCol.Header] = GuiStyle.ColorPalette[ColorName.Lightest];
							ImGui.GetStyle().Colors[(int) ImGuiCol.Text] = GuiStyle.ColorPalette[ColorName.Dark];
						}

						int meshId = setIndex % 8 + 1;
						int stateId = (int) Math.Floor(setIndex / 8f);
						if ((highlightHeader || _showUnused) &&
						    ImGui.CollapsingHeader("Mesh " + meshId + " | State " + stateId)
						) {
							GuiStyle.SetNewUiToDefaultStyle();
							ImGui.Columns(6, "LinkSetData", false);
							ImGui.SetColumnWidth(0, 30);
							ImGui.SetColumnWidth(1, 30);
							ImGui.SetColumnWidth(2, inputWidth + 10);
							ImGui.SetColumnWidth(3, inputWidth + 10);
							ImGui.SetColumnWidth(4, inputWidth + 10);
							ImGui.SetColumnWidth(5, inputWidth + 10);

							ImGui.Text("");
							ImGui.NextColumn();
							ImGui.Text("KF");
							ImGui.NextColumn();
							ImGui.Text("A");
							ImGui.NextColumn();
							ImGui.Text("B");
							ImGui.NextColumn();
							ImGui.Text("C");
							ImGui.NextColumn();
							ImGui.Text("D");
							ImGui.NextColumn();

							int keyFrameId = 1;
							for (int dataIndex = 0; dataIndex < link.RawData.Count;) {
								ImGui.Text(dataIndex + ": ");
								ImGui.NextColumn();

								ImGui.Text(keyFrameId.ToString());
								keyFrameId++;
								ImGui.NextColumn();

								for (int field = 0; field < 4; field++) {
									int value = link.RawData[dataIndex];
									ImGui.SetNextItemWidth(inputWidth);
									ImGui.DragInt("###link_" + setIndex + "_" + dataIndex, ref value);
									link.RawData[dataIndex] = (byte) value;
									ImGui.NextColumn();
									dataIndex++;
								}
							}

							ImGui.Columns(1);

							if (ImGui.Button("Copy##linkSet_" + setIndex)) {
								List<string> bytes = new List<string>();
								foreach (byte x in link.RawData) {
									bytes.Add(x.ToString());
								}

								Clipboard.SetText(string.Join(",", bytes));
							}

							ImGui.SameLine();

							if (ImGui.Button("Paste##linkSet_" + setIndex)) {
								string bytes = Clipboard.GetText();
								string[] byteArray = bytes.Split(",");
								for (int index = 0; index < byteArray.Length; index++) {
									string entry = byteArray[index];
									byte byteValue = (byte) int.Parse(entry);
									link.RawData[index] = byteValue;
								}
							}
						}
					}

					ImGui.Unindent();
				}

				if (ImGui.CollapsingHeader("Unknown Chunk")) {
					ImGui.Indent();

					const int inputWidth = 40;

					ImGui.Columns(5, "Data", false);
					ImGui.SetColumnWidth(0, 30);
					ImGui.SetColumnWidth(1, inputWidth + 10);
					ImGui.SetColumnWidth(2, inputWidth + 10);
					ImGui.SetColumnWidth(3, inputWidth + 10);
					ImGui.SetColumnWidth(4, inputWidth + 10);

					for (int dataIndex = 0; dataIndex < set.UnknownChunk.Data.Count;) {
						ImGui.Text(dataIndex + ": ");
						ImGui.NextColumn();

						for (int field = 0; field < 4; field++) {
							int value = set.UnknownChunk.Data[dataIndex];
							ImGui.SetNextItemWidth(inputWidth);
							ImGui.DragInt("###linkUnknownChunk" + dataIndex, ref value);
							set.UnknownChunk.Data[dataIndex] = (byte) value;
							ImGui.NextColumn();
							dataIndex++;
							if (dataIndex > set.UnknownChunk.Data.Count - 1) {
								break;
							}
						}
					}

					ImGui.Columns(1);

					if (ImGui.Button("Copy##linkUnknownChunk")) {
						List<string> bytes = new List<string>();
						foreach (byte x in set.UnknownChunk.Data) {
							bytes.Add(x.ToString());
						}

						Clipboard.SetText(string.Join(",", bytes));
					}

					ImGui.SameLine();

					if (ImGui.Button("Paste##UnknownChunk")) {
						string bytes = Clipboard.GetText();
						string[] byteArray = bytes.Split(",");
						for (int index = 0; index < byteArray.Length; index++) {
							string entry = byteArray[index];
							byte byteValue = (byte) int.Parse(entry);
							set.UnknownChunk.Data[index] = byteValue;
						}
					}
				}

				ImGui.Unindent();
			}
			if (!windowIsOpen) {
				Gui.ShowDebugAnimatedMeshWindow = false;
			}
		}
	}
}