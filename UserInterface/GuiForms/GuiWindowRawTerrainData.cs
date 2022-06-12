using System;
using System.Collections.Generic;
using System.Diagnostics;
using GaneshaDx.Common;
using GaneshaDx.Resources;
using GaneshaDx.UserInterface.GuiDefinitions;
using ImGuiNET;
using TextCopy;
using Vector2 = System.Numerics.Vector2;

namespace GaneshaDx.UserInterface.GuiForms {
	public static class GuiWindowRawTerrainData {
		private static int _windowWidth = 400;
		private static string _rawData = "";
		
		public static void Render() {
			bool windowIsOpen = true;
			GuiStyle.SetNewUiToDefaultStyle();
			ImGui.GetStyle().WindowRounding = 4;
			ImGui.GetStyle().FrameRounding = 0;
			ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[2]);
			const ImGuiWindowFlags flags = ImGuiWindowFlags.NoResize |
			                               ImGuiWindowFlags.AlwaysAutoResize |
			                               ImGuiWindowFlags.NoCollapse;

			ImGui.SetNextWindowSize(new Vector2(_windowWidth, 125));
			ImGui.Begin("Terrain Data", ref windowIsOpen, flags);
			{
				ImGui.PopFont();
				ImGui.PushTextWrapPos();
				ImGui.Text("Use this code when setting up an FFHacktics Wiki page for your Map.");
				ImGui.GetStyle().Colors[(int) ImGuiCol.Button] = GuiStyle.ColorPalette[ColorName.Transparent];
				ImGui.GetStyle().Colors[(int) ImGuiCol.Text] = GuiStyle.ColorPalette[ColorName.Highlighted];
				ImGui.GetStyle().Colors[(int) ImGuiCol.ButtonHovered] = GuiStyle.ColorPalette[ColorName.Dark];
				ImGui.GetStyle().Colors[(int) ImGuiCol.ButtonActive] = GuiStyle.ColorPalette[ColorName.Transparent];

				if (ImGui.Button("View the Tutorial for Adding Maps to the Wiki")) {
					Process.Start(new ProcessStartInfo {
						FileName = "https://ffhacktics.com/wiki/Tutorial:_Adding_custom_maps_to_the_wiki",
						UseShellExecute = true
					});
				}

				GuiStyle.SetNewUiToDefaultStyle();
				ImGui.SetNextItemWidth(_windowWidth - ImGui.GetStyle().WindowPadding.X * 2);
				ImGui.InputText("##hey", ref _rawData, UInt32.MaxValue);

				ImGui.Columns(3, "RawTerrainButtons", false);

				const int columnThree = 100;
				const int columnTwo = 140;
				ImGui.SetColumnWidth(0, _windowWidth - columnTwo - columnThree - 5);
				ImGui.SetColumnWidth(1, columnTwo);
				ImGui.SetColumnWidth(2, columnThree);

				ImGui.NextColumn();

				if (ImGui.Button("Copy to Clipboard", new Vector2(columnTwo - 10, 20))) {
					Clipboard.SetText(_rawData);
				}

				ImGui.NextColumn();

				if (ImGui.Button("Generate", new Vector2(columnThree - 10, 20))) {
					string text = "";
					List<byte> data = CurrentMapState.StateData.Terrain.GetRawData();
					data.Insert(0, (byte) CurrentMapState.StateData.Terrain.SizeX);
					data.Insert(1, (byte) CurrentMapState.StateData.Terrain.SizeZ);

					foreach (byte value in data) {
						text += Utilities.GetHexFromInt(value);
					}

					_rawData = text;
				}

				ImGui.NextColumn();

				ImGui.Columns(1);
			}
			ImGui.End();

			if (!windowIsOpen) {
				_rawData = "";
				Gui.ShowRawTerrainDataWindow = false;
			}
		}
	}
}