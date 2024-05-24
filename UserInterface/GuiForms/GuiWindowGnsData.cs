using System.Numerics;
using GaneshaDx.Common;
using GaneshaDx.Resources;
using GaneshaDx.UserInterface.GuiDefinitions;
using ImGuiNET;

namespace GaneshaDx.UserInterface.GuiForms;

public static class GuiWindowGnsData {
	private const int WindowWidth = 930;
	private static int _windowHeight = 250;

	public static void Render() {
		bool windowIsOpen = true;

		GuiStyle.SetNewUiToDefaultStyle();
		ImGui.GetStyle().WindowRounding = 3;
		ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[2]);

		ImGui.SetNextWindowSize(new Vector2(WindowWidth, _windowHeight));

		ImGui.Begin("GNS Data", ref windowIsOpen);
		{
			ImGui.PopFont();
			GuiStyle.SetNewUiToDefaultStyle();
			ImGui.GetStyle().FrameRounding = 0;
			RenderGnsData();

			_windowHeight = (int) ImGui.GetWindowSize().Y;
			_windowHeight = Utilities.Min(_windowHeight, 200);
		}
		ImGui.End();


		if (!windowIsOpen) {
			Gui.ShowGnsDataWindow = false;
		}
	}

	private static void RenderGnsData() {
		const int totalColumns = 20;
		const int width = 40;
		int columnWidth = width + 5;

		ImGui.Columns(totalColumns, "GnsDataTable", false);

		for (int columnIndex = 0; columnIndex < totalColumns; columnIndex++) {
			ImGui.SetColumnWidth(columnIndex, columnWidth);
		}

		for (int dataIndex = 0; dataIndex < MapData.Gns.RawData.Count; dataIndex++) {
			ImGui.Text("    " + Utilities.GetHexFromInt(MapData.Gns.RawData[dataIndex]));

			int data = MapData.Gns.RawData[dataIndex];
			ImGui.SetNextItemWidth(width);
			ImGui.DragInt("###data" + dataIndex, ref data, 1);
			MapData.Gns.RawData[dataIndex] = (byte) data;

			GuiStyle.AddSpace();
				
			ImGui.NextColumn();
		}

		ImGui.Columns(1);
	}
}