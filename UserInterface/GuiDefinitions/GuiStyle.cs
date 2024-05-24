using System.Collections.Generic;
using GaneshaDx.Common;
using ImGuiNET;
using Vector2 = System.Numerics.Vector2;
using Vector4 = System.Numerics.Vector4;

namespace GaneshaDx.UserInterface.GuiDefinitions {
	public static class GuiStyle {
		public static int RightPanelWidth = 300;
		public const int MenuBarHeight = 22;
		public const int TabPanelHeight = 22;
		public const int LabelWidth = 140;
		public const int WidgetWidth = 111;
		public const int CheckBoxWidth = 30;
		private static readonly Dictionary<int, Vector4> DefaultColors = new Dictionary<int, Vector4>();

		public static readonly Dictionary<ColorName, Vector4> ColorPalette = new Dictionary<ColorName, Vector4> {
			{ColorName.Clickable, Utilities.ConvertVector4(Utilities.GetColorFromHex("3077C1").ToVector4())},
			{ColorName.Highlighted, Utilities.ConvertVector4(Utilities.GetColorFromHex("45AEE9").ToVector4())},
			{ColorName.Selected, Utilities.ConvertVector4(Utilities.GetColorFromHex("f76363").ToVector4())},
			{ColorName.HighlightedText, Utilities.ConvertVector4(Utilities.GetColorFromHex("45AEE9").ToVector4())},
			{ColorName.Darkest, Utilities.ConvertVector4(Utilities.GetColorFromHex("1f2123").ToVector4())},
			{ColorName.Darker, Utilities.ConvertVector4(Utilities.GetColorFromHex("2f3136").ToVector4())},
			{ColorName.Dark, Utilities.ConvertVector4(Utilities.GetColorFromHex("36393e").ToVector4())},
			{ColorName.Light, Utilities.ConvertVector4(Utilities.GetColorFromHex("484b51").ToVector4())},
			{ColorName.Lighter, Utilities.ConvertVector4(Utilities.GetColorFromHex("c3c5cc").ToVector4())},
			{ColorName.Lightest, Utilities.ConvertVector4(Utilities.GetColorFromHex("eceef7").ToVector4())},
			{ColorName.Debug, Utilities.ConvertVector4(Utilities.GetColorFromHex("ca03fc").ToVector4())},
			{ColorName.Transparent, new Vector4(0, 0, 0, 0)}
		};

		public const ImGuiColorEditFlags ColorBoxFlags = ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoLabel;

		public const ImGuiWindowFlags FixedWindowFlags = ImGuiWindowFlags.NoTitleBar |
		                                                 ImGuiWindowFlags.NoMove |
		                                                 ImGuiWindowFlags.NoResize;

		static GuiStyle() {
			DefaultColors.Add((int) ImGuiCol.Text, ColorPalette[ColorName.Lightest]);
			DefaultColors.Add((int) ImGuiCol.TextDisabled, ColorPalette[ColorName.Light]);
			DefaultColors.Add((int) ImGuiCol.WindowBg, ColorPalette[ColorName.Darkest]);
			DefaultColors.Add((int) ImGuiCol.ChildBg, new Vector4(0.00f, 0.00f, 0.00f, 0.00f));
			DefaultColors.Add((int) ImGuiCol.PopupBg, new Vector4(0.08f, 0.08f, 0.08f, 0.94f));
			DefaultColors.Add((int) ImGuiCol.Border, new Vector4(0.00f, 0.00f, 0.00f, 1.00f));
			DefaultColors.Add((int) ImGuiCol.BorderShadow, ColorPalette[ColorName.Darkest]);
			DefaultColors.Add((int) ImGuiCol.FrameBg, ColorPalette[ColorName.Darker]);
			DefaultColors.Add((int) ImGuiCol.FrameBgHovered, ColorPalette[ColorName.Dark]);
			DefaultColors.Add((int) ImGuiCol.FrameBgActive, ColorPalette[ColorName.Dark]);
			DefaultColors.Add((int) ImGuiCol.TitleBg, ColorPalette[ColorName.Dark]);
			DefaultColors.Add((int) ImGuiCol.TitleBgActive, ColorPalette[ColorName.Clickable]);
			DefaultColors.Add((int) ImGuiCol.TitleBgCollapsed, ColorPalette[ColorName.Dark]);
			DefaultColors.Add((int) ImGuiCol.MenuBarBg, ColorPalette[ColorName.Darkest]);
			DefaultColors.Add((int) ImGuiCol.ScrollbarBg, ColorPalette[ColorName.Darkest]);
			DefaultColors.Add((int) ImGuiCol.ScrollbarGrab, ColorPalette[ColorName.Dark]);
			DefaultColors.Add((int) ImGuiCol.ScrollbarGrabHovered, ColorPalette[ColorName.Clickable]);
			DefaultColors.Add((int) ImGuiCol.ScrollbarGrabActive, ColorPalette[ColorName.Highlighted]);
			DefaultColors.Add((int) ImGuiCol.CheckMark, ColorPalette[ColorName.Clickable]);
			DefaultColors.Add((int) ImGuiCol.SliderGrab, ColorPalette[ColorName.Clickable]);
			DefaultColors.Add((int) ImGuiCol.SliderGrabActive, ColorPalette[ColorName.Clickable]);
			DefaultColors.Add((int) ImGuiCol.Button, ColorPalette[ColorName.Clickable]);
			DefaultColors.Add((int) ImGuiCol.ButtonHovered, ColorPalette[ColorName.Highlighted]);
			DefaultColors.Add((int) ImGuiCol.ButtonActive, ColorPalette[ColorName.Selected]);
			DefaultColors.Add((int) ImGuiCol.Header, ColorPalette[ColorName.Clickable]);
			DefaultColors.Add((int) ImGuiCol.HeaderHovered, ColorPalette[ColorName.Highlighted]);
			DefaultColors.Add((int) ImGuiCol.HeaderActive, ColorPalette[ColorName.Highlighted]);
			DefaultColors.Add((int) ImGuiCol.Separator, ColorPalette[ColorName.Dark]);
			DefaultColors.Add((int) ImGuiCol.SeparatorHovered, ColorPalette[ColorName.Dark]);
			DefaultColors.Add((int) ImGuiCol.SeparatorActive, ColorPalette[ColorName.Dark]);
			DefaultColors.Add((int) ImGuiCol.ResizeGrip, ColorPalette[ColorName.Clickable]);
			DefaultColors.Add((int) ImGuiCol.ResizeGripHovered, ColorPalette[ColorName.Highlighted]);
			DefaultColors.Add((int) ImGuiCol.ResizeGripActive, ColorPalette[ColorName.Highlighted]);
			DefaultColors.Add((int) ImGuiCol.Tab, new Vector4(0.18f, 0.35f, 0.58f, 0.86f));
			DefaultColors.Add((int) ImGuiCol.TabHovered, new Vector4(0.26f, 0.59f, 0.98f, 0.80f));
			DefaultColors.Add((int) ImGuiCol.TabActive, new Vector4(0.20f, 0.41f, 0.68f, 1.00f));
			DefaultColors.Add((int) ImGuiCol.TabUnfocused, new Vector4(0.07f, 0.10f, 0.15f, 0.97f));
			DefaultColors.Add((int) ImGuiCol.TabUnfocusedActive, new Vector4(0.14f, 0.26f, 0.42f, 1.00f));
			DefaultColors.Add((int) ImGuiCol.PlotLines, new Vector4(0.61f, 0.61f, 0.61f, 1.00f));
			DefaultColors.Add((int) ImGuiCol.PlotLinesHovered, new Vector4(1.00f, 0.43f, 0.35f, 1.00f));
			DefaultColors.Add((int) ImGuiCol.PlotHistogram, new Vector4(0.90f, 0.70f, 0.00f, 1.00f));
			DefaultColors.Add((int) ImGuiCol.PlotHistogramHovered, new Vector4(1.00f, 0.60f, 0.00f, 1.00f));
			DefaultColors.Add((int) ImGuiCol.TextSelectedBg, new Vector4(0.26f, 0.59f, 0.98f, 0.35f));
			DefaultColors.Add((int) ImGuiCol.DragDropTarget, new Vector4(1.00f, 1.00f, 0.00f, 0.90f));
			DefaultColors.Add((int) ImGuiCol.NavHighlight, new Vector4(0.26f, 0.59f, 0.98f, 1.00f));
			DefaultColors.Add((int) ImGuiCol.NavWindowingHighlight, new Vector4(1.00f, 1.00f, 1.00f, 0.70f));
			DefaultColors.Add((int) ImGuiCol.NavWindowingDimBg, new Vector4(0.80f, 0.80f, 0.80f, 0.20f));
			DefaultColors.Add((int) ImGuiCol.ModalWindowDimBg, new Vector4(0.80f, 0.80f, 0.80f, 0.35f));
		}

		public static void SetNewUiToDefaultStyle() {
			ImGuiIOPtr io = ImGui.GetIO();
			ImGuiStylePtr currentStyle = ImGui.GetStyle();
			RangeAccessor<Vector4> colors = currentStyle.Colors;

			colors[(int) ImGuiCol.Text] = DefaultColors[(int) ImGuiCol.Text];
			colors[(int) ImGuiCol.TextDisabled] = DefaultColors[(int) ImGuiCol.TextDisabled];
			colors[(int) ImGuiCol.WindowBg] = DefaultColors[(int) ImGuiCol.WindowBg];
			// colors[(int) ImGuiCol.ChildBg] = DefaultColors[(int) ImGuiCol.ChildBg];
			// colors[(int) ImGuiCol.PopupBg] = DefaultColors[(int) ImGuiCol.PopupBg];
			colors[(int) ImGuiCol.Border] = DefaultColors[(int) ImGuiCol.Border];
			colors[(int) ImGuiCol.BorderShadow] = DefaultColors[(int) ImGuiCol.BorderShadow];
			colors[(int) ImGuiCol.FrameBg] = DefaultColors[(int) ImGuiCol.FrameBg];
			colors[(int) ImGuiCol.FrameBgHovered] = DefaultColors[(int) ImGuiCol.FrameBgHovered];
			colors[(int) ImGuiCol.FrameBgActive] = DefaultColors[(int) ImGuiCol.FrameBgActive];
			colors[(int) ImGuiCol.TitleBg] = DefaultColors[(int) ImGuiCol.TitleBg];
			colors[(int) ImGuiCol.TitleBgActive] = DefaultColors[(int) ImGuiCol.TitleBgActive];
			colors[(int) ImGuiCol.TitleBgCollapsed] = DefaultColors[(int) ImGuiCol.TitleBgCollapsed];
			colors[(int) ImGuiCol.MenuBarBg] = DefaultColors[(int) ImGuiCol.MenuBarBg];
			colors[(int) ImGuiCol.ScrollbarBg] = DefaultColors[(int) ImGuiCol.ScrollbarBg];
			colors[(int) ImGuiCol.ScrollbarGrab] = DefaultColors[(int) ImGuiCol.ScrollbarGrab];
			colors[(int) ImGuiCol.ScrollbarGrabHovered] = DefaultColors[(int) ImGuiCol.ScrollbarGrabHovered];
			colors[(int) ImGuiCol.ScrollbarGrabActive] = DefaultColors[(int) ImGuiCol.ScrollbarGrabActive];
			colors[(int) ImGuiCol.CheckMark] = DefaultColors[(int) ImGuiCol.CheckMark];
			colors[(int) ImGuiCol.SliderGrab] = DefaultColors[(int) ImGuiCol.SliderGrab];
			colors[(int) ImGuiCol.SliderGrabActive] = DefaultColors[(int) ImGuiCol.SliderGrabActive];
			colors[(int) ImGuiCol.Button] = DefaultColors[(int) ImGuiCol.Button];
			colors[(int) ImGuiCol.ButtonHovered] = DefaultColors[(int) ImGuiCol.ButtonHovered];
			colors[(int) ImGuiCol.ButtonActive] = DefaultColors[(int) ImGuiCol.ButtonActive];
			colors[(int) ImGuiCol.Header] = DefaultColors[(int) ImGuiCol.Header];
			colors[(int) ImGuiCol.HeaderHovered] = DefaultColors[(int) ImGuiCol.HeaderHovered];
			colors[(int) ImGuiCol.HeaderActive] = DefaultColors[(int) ImGuiCol.HeaderActive];
			colors[(int) ImGuiCol.Separator] = DefaultColors[(int) ImGuiCol.Separator];
			colors[(int) ImGuiCol.SeparatorHovered] = DefaultColors[(int) ImGuiCol.SeparatorHovered];
			colors[(int) ImGuiCol.SeparatorActive] = DefaultColors[(int) ImGuiCol.SeparatorActive];
			colors[(int) ImGuiCol.ResizeGrip] = DefaultColors[(int) ImGuiCol.ResizeGrip];
			colors[(int) ImGuiCol.ResizeGripHovered] = DefaultColors[(int) ImGuiCol.ResizeGripHovered];
			colors[(int) ImGuiCol.ResizeGripActive] = DefaultColors[(int) ImGuiCol.ResizeGripActive];
			// colors[(int) ImGuiCol.Tab] = DefaultColors[(int) ImGuiCol.Tab];
			// colors[(int) ImGuiCol.TabHovered] = DefaultColors[(int) ImGuiCol.TabHovered];
			// colors[(int) ImGuiCol.TabActive] = DefaultColors[(int) ImGuiCol.TabActive];
			// colors[(int) ImGuiCol.TabUnfocused] = DefaultColors[(int) ImGuiCol.TabUnfocused];
			// colors[(int) ImGuiCol.TabUnfocusedActive] = DefaultColors[(int) ImGuiCol.TabUnfocusedActive];
			// colors[(int) ImGuiCol.PlotLines] = DefaultColors[(int) ImGuiCol.PlotLines];
			// colors[(int) ImGuiCol.PlotLinesHovered] = DefaultColors[(int) ImGuiCol.PlotLinesHovered];
			// colors[(int) ImGuiCol.PlotHistogram] = DefaultColors[(int) ImGuiCol.PlotHistogram];
			// colors[(int) ImGuiCol.PlotHistogramHovered] = DefaultColors[(int) ImGuiCol.PlotHistogramHovered];
			// colors[(int) ImGuiCol.TextSelectedBg] = DefaultColors[(int) ImGuiCol.TextSelectedBg];
			// colors[(int) ImGuiCol.DragDropTarget] = DefaultColors[(int) ImGuiCol.DragDropTarget];
			// colors[(int) ImGuiCol.NavHighlight] = DefaultColors[(int) ImGuiCol.NavHighlight];
			// colors[(int) ImGuiCol.NavWindowingHighlight] = DefaultColors[(int) ImGuiCol.NavWindowingHighlight];
			// colors[(int) ImGuiCol.NavWindowingDimBg] = DefaultColors[(int) ImGuiCol.NavWindowingDimBg];
			// colors[(int) ImGuiCol.ModalWindowDimBg] = DefaultColors[(int) ImGuiCol.ModalWindowDimBg];

			ImGui.PushFont(io.Fonts.Fonts[1]);
			currentStyle.WindowPadding = new Vector2(8, 8);
			currentStyle.FramePadding = new Vector2(8, 3);
			currentStyle.FrameRounding = 3;
			currentStyle.ItemSpacing = new Vector2(8, 4);
			currentStyle.WindowBorderSize = 0;
			currentStyle.ButtonTextAlign = new Vector2(0.5f, 0.5f);
		}

		public static void SetElementStyle(ElementStyle elementStyle) {
			ImGuiIOPtr io = ImGui.GetIO();
			ImGuiStylePtr currentStyle = ImGui.GetStyle();
			RangeAccessor<Vector4> colors = currentStyle.Colors;

			if (elementStyle == ElementStyle.ButtonTabUnselected) {
				colors[(int) ImGuiCol.Button] = ColorPalette[ColorName.Transparent];
				colors[(int) ImGuiCol.ButtonHovered] = ColorPalette[ColorName.Highlighted];
				colors[(int) ImGuiCol.ButtonActive] = ColorPalette[ColorName.Highlighted];
				colors[(int) ImGuiCol.Text] = DefaultColors[(int) ImGuiCol.Text];
				currentStyle.FrameRounding = 0;
				ImGui.PushFont(io.Fonts.Fonts[2]);
			}

			if (elementStyle == ElementStyle.ButtonTabSelected) {
				colors[(int) ImGuiCol.Button] = ColorPalette[ColorName.Transparent];
				colors[(int) ImGuiCol.ButtonHovered] = ColorPalette[ColorName.Transparent];
				colors[(int) ImGuiCol.ButtonActive] = ColorPalette[ColorName.Transparent];
				colors[(int) ImGuiCol.Text] = ColorPalette[ColorName.HighlightedText];
				ImGui.PushFont(io.Fonts.Fonts[2]);
			}

			if (elementStyle == ElementStyle.ButtonSelected) {
				colors[(int) ImGuiCol.Button] = ColorPalette[ColorName.Selected];
				colors[(int) ImGuiCol.ButtonHovered] = ColorPalette[ColorName.Selected];
				colors[(int) ImGuiCol.ButtonActive] = ColorPalette[ColorName.Selected];
				colors[(int) ImGuiCol.Text] = ColorPalette[ColorName.Lightest];
			}

			if (elementStyle == ElementStyle.ButtonDisabled) {
				colors[(int) ImGuiCol.Button] = ColorPalette[ColorName.Dark];
				colors[(int) ImGuiCol.ButtonHovered] = ColorPalette[ColorName.Dark];
				colors[(int) ImGuiCol.ButtonActive] = ColorPalette[ColorName.Dark];
				colors[(int) ImGuiCol.Text] = ColorPalette[ColorName.Darkest];
			}

			if (elementStyle == ElementStyle.ButtonFileBrowserDirectory) {
				colors[(int) ImGuiCol.Button] = ColorPalette[ColorName.Transparent];
				colors[(int) ImGuiCol.ButtonHovered] = ColorPalette[ColorName.Darker];
				colors[(int) ImGuiCol.ButtonActive] = ColorPalette[ColorName.Transparent];
				colors[(int) ImGuiCol.Text] = ColorPalette[ColorName.Highlighted];
				currentStyle.FrameRounding = 0;
				currentStyle.ButtonTextAlign = new Vector2(0, 0.5f);
			}

			if (elementStyle == ElementStyle.ButtonFileBrowserFile) {
				colors[(int) ImGuiCol.Button] = ColorPalette[ColorName.Transparent];
				colors[(int) ImGuiCol.ButtonHovered] = ColorPalette[ColorName.Darker];
				colors[(int) ImGuiCol.ButtonActive] = ColorPalette[ColorName.Darker];
				colors[(int) ImGuiCol.Text] = ColorPalette[ColorName.Lightest];
				currentStyle.FrameRounding = 0;
				currentStyle.ButtonTextAlign = new Vector2(0, 0.5f);
			}

			if (elementStyle == ElementStyle.ButtonFileBrowserFileSelected) {
				colors[(int) ImGuiCol.Button] = ColorPalette[ColorName.Dark];
				colors[(int) ImGuiCol.ButtonHovered] = ColorPalette[ColorName.Dark];
				colors[(int) ImGuiCol.ButtonActive] = ColorPalette[ColorName.Dark];
				colors[(int) ImGuiCol.Text] = ColorPalette[ColorName.Lightest];
				currentStyle.FrameRounding = 0;
				currentStyle.ButtonTextAlign = new Vector2(0, 0.5f);
			}
			
			if (elementStyle == ElementStyle.CheckboxDisabled) {
				colors[(int) ImGuiCol.FrameBg] = ColorPalette[ColorName.Darkest];
				colors[(int) ImGuiCol.FrameBgHovered] = ColorPalette[ColorName.Darkest];
				colors[(int) ImGuiCol.FrameBgActive] = ColorPalette[ColorName.Darkest];
			}


			if (elementStyle == ElementStyle.CheckboxEnabled) {
				colors[(int) ImGuiCol.FrameBg] = ColorPalette[ColorName.Darker];
				colors[(int) ImGuiCol.FrameBgHovered] = ColorPalette[ColorName.Dark];
				colors[(int) ImGuiCol.FrameBgActive] = ColorPalette[ColorName.Dark];
			}
			
			if (elementStyle == ElementStyle.InvisibleWindowStyle) {
				colors[(int) ImGuiCol.WindowBg] = Vector4.Zero;
			}

			if (elementStyle == ElementStyle.FixedPanelStyle) {
				colors[(int) ImGuiCol.WindowBg] = DefaultColors[(int) ImGuiCol.WindowBg];
				currentStyle.WindowRounding = 0;
			}

			if (elementStyle == ElementStyle.Header) {
				colors[(int) ImGuiCol.Text] = ColorPalette[ColorName.HighlightedText];
				colors[(int) ImGuiCol.Header] = ColorPalette[ColorName.Transparent];
				colors[(int) ImGuiCol.HeaderActive] = ColorPalette[ColorName.Transparent];
				colors[(int) ImGuiCol.HeaderHovered] = ColorPalette[ColorName.Transparent];
				ImGui.PushFont(io.Fonts.Fonts[2]);
			}

			if (elementStyle == ElementStyle.WindowNoPadding) {
				currentStyle.WindowPadding = Vector2.Zero;
			}

			if (elementStyle == ElementStyle.DarkText) {
				colors[(int) ImGuiCol.Text] = ColorPalette[ColorName.Darkest];
			}
		}

		public static void AddSpace(float height = 10f) {
			ImGui.Dummy(new Vector2(0, height));
		}
	}
}