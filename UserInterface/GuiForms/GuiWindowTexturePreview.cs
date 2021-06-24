using GaneshaDx.Common;
using GaneshaDx.UserInterface.GuiDefinitions;
using GaneshaDx.UserInterface.Input;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Vector2 = System.Numerics.Vector2;

namespace GaneshaDx.UserInterface.GuiForms {
	public static class GuiWindowTexturePreview {
		public static bool ShouldRenderTexture;
		private static Vector2 _textureWindowSize = new Vector2(500, 500);
		private static Vector2 _textureWindowPosition = new Vector2(250, 250);

		public static bool IsMouseHovered => UvPreviewBounds.Contains(AppInput.ThisMousePosition);

		public static Rectangle UvPreviewBounds => new Rectangle(
			(int) (_textureWindowPosition.X + ImGui.GetStyle().WindowPadding.X),
			(int) (_textureWindowPosition.Y + ImGui.GetStyle().WindowPadding.Y + 20),
			(int) (_textureWindowSize.X - ImGui.GetStyle().WindowPadding.X * 2),
			(int) (_textureWindowSize.Y - ImGui.GetStyle().WindowPadding.Y * 2 - 20)
		);

		public static void Render() {
			GuiStyle.SetNewUiToDefaultStyle();
			bool isOnCorrectTab = Gui.SelectedTab == RightPanelTab.Texture;
			bool isOnCorrectPanel = GuiPanelTexture.CurrentPanelMode == GuiPanelTexture.PanelMode.UVs ||
			                        GuiPanelTexture.CurrentPanelMode == GuiPanelTexture.PanelMode.UvAnimation;

			if (!isOnCorrectTab || !isOnCorrectPanel) {
				ShouldRenderTexture = false;
				return;
			}

			_textureWindowSize.X = Utilities.Min(_textureWindowSize.X, 100);
			_textureWindowSize.Y = Utilities.Min(_textureWindowSize.Y, 100);

			if (_textureWindowSize.X < _textureWindowSize.Y) {
				_textureWindowSize.Y = _textureWindowSize.X;
			} else {
				_textureWindowSize.X = _textureWindowSize.Y;
			}

			ImGui.SetNextWindowSize(_textureWindowSize);
			ImGui.GetStyle().WindowRounding = 5;
			ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[2]);

			ImGui.Begin("Texture");
			{
				ImGui.PopFont();
				ShouldRenderTexture = true;

				if (!ImGui.IsWindowCollapsed()) {
					_textureWindowSize = ImGui.GetWindowSize();
					_textureWindowPosition = ImGui.GetWindowPos();
				} else {
					ShouldRenderTexture = false;
				}
			}
			ImGui.End();
		}
	}
}