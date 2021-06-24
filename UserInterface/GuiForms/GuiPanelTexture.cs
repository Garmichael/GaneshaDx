using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GaneshaDx.Common;
using GaneshaDx.Resources;
using GaneshaDx.Resources.ContentDataTypes.Palettes;
using GaneshaDx.Resources.ContentDataTypes.Polygons;
using GaneshaDx.Resources.ContentDataTypes.TextureAnimations;
using GaneshaDx.UserInterface.GuiDefinitions;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Vector2 = System.Numerics.Vector2;
using Vector4 = System.Numerics.Vector4;

namespace GaneshaDx.UserInterface.GuiForms {
	public static class GuiPanelTexture {
		public enum PanelMode {
			UVs,
			Palettes,
			AnimationsList,
			UvAnimation,
			PaletteAnimation
		}

		public static PanelMode CurrentPanelMode;
		private static int _paletteToChange;
		private static int _colorToChange;
		public static TextureAnimation SelectedTextureAnimation;
		public static int TextureAnimationPreviewPaletteId;
		public static int TextureAnimationPreviewPageId;
		private static Polygon _copiedPolygon;

		public static void Render() {
			if (CurrentPanelMode == PanelMode.UVs) {
				if (Selection.SelectedPolygons.Count > 0 && Selection.SelectedPolygons[0].IsTextured) {
					RenderUvValues();
				}

				RenderTexturePreviewOptionsPanel();
				RenderTextureOptionsPanel();
			} else if (CurrentPanelMode == PanelMode.Palettes) {
				RenderPalettes();
			} else if (CurrentPanelMode == PanelMode.AnimationsList) {
				RenderAnimationList();
			} else if (CurrentPanelMode == PanelMode.PaletteAnimation) {
				RenderPaletteAnimation();
			} else if (CurrentPanelMode == PanelMode.UvAnimation) {
				RenderUvAnimation();
			}
		}

		private static void RenderUvValues() {
			GuiStyle.SetNewUiToDefaultStyle();
			GuiStyle.SetElementStyle(ElementStyle.Header);

			if (ImGui.CollapsingHeader("UV Properties", ImGuiTreeNodeFlags.DefaultOpen)) {
				Polygon polygon = Selection.SelectedPolygons[0];
				const int axisLabelWidth = 36;
				const int valueWidth = 65;

				GuiStyle.SetNewUiToDefaultStyle();
				ImGui.Indent();
				ImGui.Columns(3, "UvPropertiesColumns", false);
				ImGui.SetColumnWidth(0, axisLabelWidth);
				ImGui.SetColumnWidth(1, valueWidth + 10);
				ImGui.SetColumnWidth(2, valueWidth + 10);
				ImGui.SetColumnWidth(3, valueWidth + 10);

				GuiStyle.SetElementStyle(ElementStyle.Header);
				ImGui.NextColumn();

				ImGui.PushStyleColor(ImGuiCol.Text, Utilities.ConvertVector4(Color.Red.ToVector4()));
				ImGui.Text("U");
				ImGui.NextColumn();

				ImGui.PushStyleColor(ImGuiCol.Text, Utilities.ConvertVector4(Color.LightGreen.ToVector4()));
				ImGui.Text("V");
				ImGui.NextColumn();

				string[] vertexLabels = {"A", "B", "C", "D"};
				Vector4[] vertexColors = {
					Utilities.ConvertVector4(Color.Red.ToVector4()),
					Utilities.ConvertVector4(Color.LightGreen.ToVector4()),
					Utilities.ConvertVector4(Color.Cyan.ToVector4()),
					Utilities.ConvertVector4(Color.Yellow.ToVector4())
				};

				for (int vertexIndex = 0; vertexIndex < (polygon.IsQuad ? 4 : 3); vertexIndex++) {
					ImGui.PushStyleColor(ImGuiCol.Text, vertexColors[vertexIndex]);
					ImGui.Text(vertexLabels[vertexIndex]);
					ImGui.NextColumn();

					GuiStyle.SetNewUiToDefaultStyle();

					ImGui.PushItemWidth(valueWidth);
					int uPosition = (int) polygon.UvCoordinates[vertexIndex].X;
					ImGui.DragInt(
						vertexLabels[vertexIndex] + "U_Position",
						ref uPosition,
						0.1f
					);
					ImGui.NextColumn();

					ImGui.PushItemWidth(valueWidth);
					int vPosition = (int) polygon.UvCoordinates[vertexIndex].Y;
					ImGui.DragInt(
						vertexLabels[vertexIndex] + "V_Position",
						ref vPosition,
						0.1f
					);

					ImGui.NextColumn();

					polygon.UvCoordinates[vertexIndex] = new Microsoft.Xna.Framework.Vector2(uPosition, vPosition);
				}

				GuiStyle.SetNewUiToDefaultStyle();

				GuiStyle.AddSpace();

				ImGui.Columns(2, "MoreUvPropertiesColumns", false);
				ImGui.SetColumnWidth(0, GuiStyle.LabelWidth);
				ImGui.SetColumnWidth(1, GuiStyle.WidgetWidth + 10);

				ImGui.Text("Texture Page");
				ImGui.NextColumn();

				int texturePageId = Selection.SelectedPolygons[0].TexturePage;
				int beforeTexturePage = texturePageId;
				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.InputInt("TexturePageId", ref texturePageId, 1);
				texturePageId = Utilities.Clamp(texturePageId, 0, 3);

				if (beforeTexturePage != texturePageId) {
					foreach (Polygon selectedPolygon in Selection.SelectedPolygons) {
						selectedPolygon.TexturePage = texturePageId;
					}
				}

				ImGui.NextColumn();

				ImGui.Text("Texture Palette");
				ImGui.NextColumn();

				int texturePalette = Selection.SelectedPolygons[0].PaletteId;
				int beforeTexturePalette = texturePalette;
				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.InputInt("TexturePaletteId", ref texturePalette, 1);
				texturePalette = Utilities.Clamp(texturePalette, 0, 15);

				if (beforeTexturePalette != texturePalette) {
					foreach (Polygon selectedPolygon in Selection.SelectedPolygons) {
						selectedPolygon.PaletteId = texturePalette;
					}
				}

				ImGui.NextColumn();

				ImGui.Text("Texture is Lit");
				ImGui.NextColumn();

				bool isLit = Selection.SelectedPolygons[0].RenderingProperties.LitTexture;
				bool beforeIsLit = isLit;
				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.Checkbox("##TextureIsLit", ref isLit);

				if (beforeIsLit != isLit) {
					foreach (Polygon selectedPolygon in Selection.SelectedPolygons) {
						selectedPolygon.RenderingProperties.LitTexture = isLit;
					}
				}

				ImGui.Columns(1);

				const int buttonWidth = 120;
				const int buttonHeight = 20;


				GuiStyle.AddSpace();
				if (ImGui.Button("Flip Horizontally", new Vector2(buttonWidth, buttonHeight))) {
					(float centerX, _) = GetCenterOfUvSelection();

					foreach (Polygon selectedPolygon in Selection.SelectedPolygons) {
						for (int uvIndex = 0; uvIndex < selectedPolygon.UvCoordinates.Count; uvIndex++) {
							float diff = centerX - selectedPolygon.UvCoordinates[uvIndex].X;
							selectedPolygon.UvCoordinates[uvIndex] = new Microsoft.Xna.Framework.Vector2(
								centerX + diff,
								selectedPolygon.UvCoordinates[uvIndex].Y
							);
						}
					}
				}

				ImGui.SameLine();

				if (ImGui.Button("Flip Vertically", new Vector2(buttonWidth, buttonHeight))) {
					(_, float centerY) = GetCenterOfUvSelection();

					foreach (Polygon selectedPolygon in Selection.SelectedPolygons) {
						for (int uvIndex = 0; uvIndex < selectedPolygon.UvCoordinates.Count; uvIndex++) {
							float diff = centerY - selectedPolygon.UvCoordinates[uvIndex].Y;
							selectedPolygon.UvCoordinates[uvIndex] = new Microsoft.Xna.Framework.Vector2(
								selectedPolygon.UvCoordinates[uvIndex].X,
								centerY + diff
							);
						}
					}
				}

				if (ImGui.Button("Rotate Left", new Vector2(buttonWidth, buttonHeight))) {
					RotateUvSelection(-90);
				}

				ImGui.SameLine();

				if (ImGui.Button("Rotate Right", new Vector2(buttonWidth, buttonHeight))) {
					RotateUvSelection(90);
				}


				GuiStyle.AddSpace();

				if (ImGui.Button("Copy##CopyPolygonUv", new Vector2(buttonWidth, buttonHeight))) {
					_copiedPolygon = Selection.SelectedPolygons[0];
				}

				ImGui.SameLine();

				bool isDisabled = _copiedPolygon == null;

				if (isDisabled) {
					GuiStyle.SetElementStyle(ElementStyle.ButtonDisabled);
				}

				if (ImGui.Button("Paste##PastePolygonUv", new Vector2(buttonWidth, buttonHeight)) && !isDisabled) {
					foreach (Polygon selectedPolygon in Selection.SelectedPolygons) {
						if (selectedPolygon.IsTextured) {
							selectedPolygon.TexturePage = _copiedPolygon.TexturePage;
							selectedPolygon.PaletteId = _copiedPolygon.PaletteId;
							selectedPolygon.RenderingProperties.LitTexture =
								_copiedPolygon.RenderingProperties.LitTexture;

							for (int uvIndex = 0; uvIndex < selectedPolygon.UvCoordinates.Count; uvIndex++) {
								selectedPolygon.UvCoordinates[uvIndex] = new Microsoft.Xna.Framework.Vector2(
									_copiedPolygon.UvCoordinates[uvIndex].X,
									_copiedPolygon.UvCoordinates[uvIndex].Y
								);
							}
						}
					}
				}

				ImGui.Columns(1);
				ImGui.Unindent();
				GuiStyle.AddSpace();
			}
		}

		private static Microsoft.Xna.Framework.Vector2 GetCenterOfUvSelection() {
			List<Microsoft.Xna.Framework.Vector2> points = new List<Microsoft.Xna.Framework.Vector2>();
			foreach (Polygon selectedPolygon in Selection.SelectedPolygons) {
				foreach (Microsoft.Xna.Framework.Vector2 uvVertex in selectedPolygon.UvCoordinates) {
					points.Add(uvVertex);
				}
			}

			return Utilities.GetAveragePoint(points);
		}

		private static void RotateUvSelection(int angle) {
			Microsoft.Xna.Framework.Vector2 center = GetCenterOfUvSelection();
			Matrix rotation = Matrix.CreateRotationZ(MathHelper.ToRadians(angle));

			foreach (Polygon selectedPolygon in Selection.SelectedPolygons) {
				for (int uvIndex = 0; uvIndex < selectedPolygon.UvCoordinates.Count; uvIndex++) {
					Vector3 position = new Vector3(selectedPolygon.UvCoordinates[uvIndex], 0);
					position -= new Vector3(center, 0);
					position = Vector3.Transform(position, rotation);
					position += new Vector3(center, 0);

					int newX = (int) Math.Round(position.X, MidpointRounding.AwayFromZero);
					int newY = (int) Math.Round(position.Y, MidpointRounding.AwayFromZero);

					selectedPolygon.UvCoordinates[uvIndex] = new Microsoft.Xna.Framework.Vector2(newX, newY);
				}
			}
		}

		private static void RenderTexturePreviewOptionsPanel() {
			GuiStyle.SetNewUiToDefaultStyle();
			GuiStyle.SetElementStyle(ElementStyle.Header);

			if (ImGui.CollapsingHeader("Preview Options", ImGuiTreeNodeFlags.DefaultOpen)) {
				GuiStyle.SetNewUiToDefaultStyle();
				ImGui.Indent();

				ImGui.Columns(2, "TexturePreviewOptions", false);
				ImGui.SetColumnWidth(0, GuiStyle.LabelWidth);
				ImGui.SetColumnWidth(1, GuiStyle.WidgetWidth + 10);

				ImGui.Text("shift+Arrow distance");
				ImGui.NextColumn();

				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				int beforeShiftArrowDistance = Configuration.Properties.TexturePreviewShiftArrowDistance;
				ImGui.InputInt(
					"TexturePreviewOptionBigMoveDifference",
					ref Configuration.Properties.TexturePreviewShiftArrowDistance
				);
				if (beforeShiftArrowDistance != Configuration.Properties.TexturePreviewShiftArrowDistance) {
					Configuration.SaveConfiguration();
				}

				ImGui.NextColumn();

				ImGui.Text("Show Unselected Uvs");
				ImGui.NextColumn();

				ImGui.PushItemWidth(GuiStyle.CheckBoxWidth);
				bool beforeShowUnselectedUvs = Configuration.Properties.TexturePreviewShowUnselectedUvs;
				ImGui.Checkbox(
					"###TexturePreviewOptionShowUnselected",
					ref Configuration.Properties.TexturePreviewShowUnselectedUvs
				);
				if (beforeShowUnselectedUvs != Configuration.Properties.TexturePreviewShowUnselectedUvs) {
					Configuration.SaveConfiguration();
				}

				ImGui.NextColumn();

				ImGui.Text("Highlight Selected Polys");
				ImGui.NextColumn();

				ImGui.PushItemWidth(GuiStyle.CheckBoxWidth);
				bool beforeHighSelection = Configuration.Properties.HighlightSelectionOnTexturePage;
				ImGui.Checkbox(
					"###TexturePreviewOptionHighlightPoly",
					ref Configuration.Properties.HighlightSelectionOnTexturePage
				);
				if (beforeHighSelection != Configuration.Properties.HighlightSelectionOnTexturePage) {
					Configuration.SaveConfiguration();
				}

				ImGui.NextColumn();

				ImGui.Columns(1);
				ImGui.Unindent();
				GuiStyle.AddSpace();
			}
		}

		private static void RenderTextureOptionsPanel() {
			const int buttonWidth = 120;
			const int buttonHeight = 20;

			GuiStyle.SetNewUiToDefaultStyle();
			GuiStyle.SetElementStyle(ElementStyle.Header);

			if (ImGui.CollapsingHeader("Texture Data", ImGuiTreeNodeFlags.DefaultOpen)) {
				GuiStyle.SetNewUiToDefaultStyle();
				ImGui.Indent();

				if (ImGui.Button("Edit Palettes", new Vector2(buttonWidth, buttonHeight))) {
					CurrentPanelMode = PanelMode.Palettes;
				}

				ImGui.SameLine();

				bool disabled = CurrentMapState.StateData.TextureAnimations == null || 
				                CurrentMapState.StateData.TextureAnimations.Count == 0;
				
				if (disabled) {
					GuiStyle.SetElementStyle(ElementStyle.ButtonDisabled);
				}

				if (ImGui.Button("Edit Animations", new Vector2(buttonWidth, buttonHeight)) && !disabled) {
					CurrentPanelMode = PanelMode.AnimationsList;
				}

				GuiStyle.SetNewUiToDefaultStyle();

				if (ImGui.Button("Import Texture", new Vector2(buttonWidth, buttonHeight))) {
					MyraGui.OpenImportTextureFileDialog();
				}

				ImGui.SameLine();

				if (ImGui.Button("Export Texture", new Vector2(buttonWidth, buttonHeight))) {
					string fileName = MapData.MapName + "." +
					                  CurrentMapState.StateData.StateTextureResource.XFile +
					                  ".png";

					MyraGui.OpenExportTextureFileDialog(fileName);
				}

				ImGui.Columns(1);
				GuiStyle.AddSpace();
				ImGui.Unindent();
				GuiStyle.AddSpace();
			}
		}

		private static void RenderPalettes() {
			GuiStyle.SetNewUiToDefaultStyle();

			if (ImGui.Button("< Return")) {
				CurrentPanelMode = PanelMode.UVs;
			}

			GuiStyle.AddSpace();

			const int totalPalettes = 16;
			const int totalColorsPerPalette = 16;
			const int colorBoxWidth = 12;

			ImGui.Columns(3, "paletteGrid", false);
			ImGui.SetColumnWidth(0, 30);
			ImGui.SetColumnWidth(1, colorBoxWidth * totalColorsPerPalette + 2);

			for (int paletteIndex = 0; paletteIndex < totalPalettes; paletteIndex++) {
				ImGuiStylePtr style = ImGui.GetStyle();
				style.ItemSpacing = Vector2.Zero;
				style.FrameRounding = 0;
				style.FrameBorderSize = 0;
				style.FramePadding = Vector2.Zero;

				if (Selection.SelectedPolygons.Count > 0 && Selection.SelectedPolygons[0].PaletteId == paletteIndex) {
					style.Colors[(int) ImGuiCol.Text] = GuiStyle.ColorPalette[ColorName.HighlightedText];
				}

				ImGui.Text(paletteIndex.ToString());
				ImGui.NextColumn();

				Palette palette = CurrentMapState.StateData.Palettes[paletteIndex];

				for (int colorIndex = 0; colorIndex < totalColorsPerPalette; colorIndex++) {
					Vector4 color = Utilities.ConvertVector4(palette.Colors[colorIndex].ToColor().ToVector4());

					if (
						palette.Colors[colorIndex].IsTransparent &&
						color.X == 0 &&
						color.Y == 0 &&
						color.Z == 0
					) {
						color.W = 0;
					}

					style.Colors[(int) ImGuiCol.Button] = color;
					style.Colors[(int) ImGuiCol.ButtonHovered] = color + new Vector4(0.1f, 0.1f, 0.1f, 0);
					style.Colors[(int) ImGuiCol.ButtonActive] = color;

					string id = "##COLOR" + paletteIndex + "_" + colorIndex;

					if (ImGui.Button(id, new Vector2(colorBoxWidth, 20))) {
						_paletteToChange = paletteIndex;
						_colorToChange = colorIndex;
						ImGui.OpenPopup("paletteColorPicker");
					}

					if (colorIndex < totalColorsPerPalette - 1) {
						ImGui.SameLine();
					}
				}

				ImGui.NextColumn();

				GuiStyle.SetNewUiToDefaultStyle();
				style.ItemSpacing = new Vector2(1, 1);
				style.FrameRounding = 0;
				style.FrameBorderSize = 0;

				ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[3]);
				ImGui.GetStyle().Colors[(int) ImGuiCol.Button] =
					GuiStyle.ColorPalette[ColorName.Transparent];

				if (ImGui.Button("I##import" + paletteIndex)) {
					MyraGui.OpenImportPaletteFileDialog(paletteIndex, "main");
				}

				ImGui.SameLine();

				if (ImGui.Button("E##export" + paletteIndex)) {
					string fileName = MapData.MapName + "." +
					                  CurrentMapState.StateData.StateMeshResources[0].XFile +
					                  ".act";

					MyraGui.OpenExportPaletteFileDialog(fileName, paletteIndex, "main");
				}

				ImGui.PopFont();
				ImGui.NextColumn();
			}

			if (ImGui.BeginPopup("paletteColorPicker")) {
				Palette palette = CurrentMapState.StateData.Palettes[_paletteToChange];
				Vector4 color = Utilities.ConvertVector4(palette.Colors[_colorToChange].ToColor().ToVector4());

				if (palette.Colors[_colorToChange].IsTransparent) {
					color.W = 0;
				}

				float beforeAlpha = color.W;

				ImGui.ColorPicker4("##picker", ref color);

				if (color.W > beforeAlpha) {
					color.W = 255;
				} else if (color.W < beforeAlpha) {
					color.W = 0;
				}

				Color toColor = new Color(color.X, color.Y, color.Z, color.W);
				palette.Colors[_colorToChange].FromColor(toColor);
				ImGui.EndPopup();
			}

			ImGui.Columns(1);
		}

		private static void RenderAnimationList() {
			const int labelWidth = 25;
			const int comboWidth = GuiStyle.WidgetWidth + 68;

			GuiStyle.SetNewUiToDefaultStyle();

			if (ImGui.Button("< Return")) {
				CurrentPanelMode = PanelMode.UVs;
			}

			GuiStyle.AddSpace();

			ImGui.Indent();

			ImGui.Columns(3, "AnimationListColumns", false);
			ImGui.SetColumnWidth(0, labelWidth);
			ImGui.SetColumnWidth(1, comboWidth + 10);

			List<AnimatedTextureInstructions> textureAnimations = CurrentMapState.StateData.TextureAnimations;

			List<string> animationTypes = Enum.GetNames(typeof(TextureAnimationType)).ToList();
			for (int animationTypeIndex = 0; animationTypeIndex < animationTypes.Count; animationTypeIndex++) {
				animationTypes[animationTypeIndex] = Regex.Replace(
					animationTypes[animationTypeIndex], "(\\B[A-Z])", " $1"
				);
			}

			for (int index = 0; index < textureAnimations.Count; index++) {
				ImGui.Text(index.ToString());

				ImGui.NextColumn();

				int animationType = (int) textureAnimations[index].TextureAnimationType;
				int beforeAnimationType = animationType;
				ImGui.SetNextItemWidth(comboWidth);
				ImGui.Combo("##animation" + index, ref animationType, animationTypes.ToArray(), animationTypes.Count);

				if (beforeAnimationType != animationType) {
					TextureAnimationType newType = (TextureAnimationType) animationType;
					bool newTypeIsValid = !(newType == TextureAnimationType.PaletteAnimation &&
					                        (CurrentMapState.StateData.PaletteAnimationFrames == null ||
					                         CurrentMapState.StateData.PaletteAnimationFrames.Count == 0
					                        ));

					if (newTypeIsValid) {
						textureAnimations[index].TextureAnimationType = (TextureAnimationType) animationType;

						textureAnimations[index].Instructions = textureAnimations[index].TextureAnimationType switch {
							TextureAnimationType.PaletteAnimation => new PaletteAnimation(),
							TextureAnimationType.UvAnimation => new UvAnimation(),
							_ => null
						};
					} else {
						OverlayConsole.AddMessage("Animation Type not supported. Add Support in Mesh Management");
					}
				}

				ImGui.NextColumn();

				bool disabled = textureAnimations[index].TextureAnimationType == TextureAnimationType.None;

				if (disabled) {
					GuiStyle.SetElementStyle(ElementStyle.ButtonDisabled);
				}

				if (ImGui.Button("Edit###" + index) && !disabled) {
					if (textureAnimations[index].TextureAnimationType == TextureAnimationType.PaletteAnimation) {
						SelectedTextureAnimation = CurrentMapState.StateData.TextureAnimations[index].Instructions;
						CurrentPanelMode = PanelMode.PaletteAnimation;
					}

					if (textureAnimations[index].TextureAnimationType == TextureAnimationType.UvAnimation) {
						SelectedTextureAnimation = CurrentMapState.StateData.TextureAnimations[index].Instructions;
						CurrentPanelMode = PanelMode.UvAnimation;
					}
				}

				GuiStyle.SetNewUiToDefaultStyle();

				ImGui.NextColumn();
			}


			ImGui.Unindent();
		}

		private static void RenderPaletteAnimation() {
			GuiStyle.SetNewUiToDefaultStyle();

			if (ImGui.Button("< Return")) {
				CurrentPanelMode = PanelMode.AnimationsList;
			}

			GuiStyle.AddSpace();

			const int totalPalettes = 16;
			const int totalColorsPerPalette = 16;
			const int colorBoxWidth = 12;

			ImGui.Columns(3, "paletteGrid", false);
			ImGui.SetColumnWidth(0, 30);
			ImGui.SetColumnWidth(1, colorBoxWidth * totalColorsPerPalette + 2);

			for (int paletteIndex = 0; paletteIndex < totalPalettes; paletteIndex++) {
				ImGuiStylePtr style = ImGui.GetStyle();
				style.ItemSpacing = Vector2.Zero;
				style.FrameRounding = 0;
				style.FrameBorderSize = 0;
				style.FramePadding = Vector2.Zero;

				ImGui.Text(paletteIndex.ToString());
				ImGui.NextColumn();

				Palette palette = CurrentMapState.StateData.PaletteAnimationFrames[paletteIndex];

				for (int colorIndex = 0; colorIndex < totalColorsPerPalette; colorIndex++) {
					Vector4 color = Utilities.ConvertVector4(palette.Colors[colorIndex].ToColor().ToVector4());

					if (
						palette.Colors[colorIndex].IsTransparent &&
						color.X == 0 &&
						color.Y == 0 &&
						color.Z == 0
					) {
						color.W = 0;
					}

					style.Colors[(int) ImGuiCol.Button] = color;
					style.Colors[(int) ImGuiCol.ButtonHovered] = color + new Vector4(0.1f, 0.1f, 0.1f, 0);
					style.Colors[(int) ImGuiCol.ButtonActive] = color;

					string id = "##COLOR" + paletteIndex + "_" + colorIndex;

					if (ImGui.Button(id, new Vector2(colorBoxWidth, 20))) {
						_paletteToChange = paletteIndex;
						_colorToChange = colorIndex;
						ImGui.OpenPopup("paletteAnimationColorPicker");
					}

					if (colorIndex < totalColorsPerPalette - 1) {
						ImGui.SameLine();
					}
				}

				ImGui.NextColumn();

				GuiStyle.SetNewUiToDefaultStyle();
				style.ItemSpacing = new Vector2(1, 1);
				style.FrameRounding = 0;
				style.FrameBorderSize = 0;

				ImGui.PushFont(ImGui.GetIO().Fonts.Fonts[3]);
				ImGui.GetStyle().Colors[(int) ImGuiCol.Button] =
					GuiStyle.ColorPalette[ColorName.Transparent];

				if (ImGui.Button("I##importA" + paletteIndex)) {
					MyraGui.OpenImportPaletteFileDialog(paletteIndex, "animation");
				}

				ImGui.SameLine();

				if (ImGui.Button("E##exportA" + paletteIndex)) {
					string fileName = MapData.MapName + "." +
					                  CurrentMapState.StateData.StateMeshResources[0].XFile +
					                  ".act";

					MyraGui.OpenExportPaletteFileDialog(fileName, paletteIndex, "animation");
				}

				ImGui.PopFont();

				ImGui.NextColumn();
			}

			if (ImGui.BeginPopup("paletteAnimationColorPicker")) {
				Palette palette = CurrentMapState.StateData.PaletteAnimationFrames[_paletteToChange];
				Vector4 color = Utilities.ConvertVector4(palette.Colors[_colorToChange].ToColor().ToVector4());

				if (palette.Colors[_colorToChange].IsTransparent) {
					color.W = 0;
				}

				float beforeAlpha = color.W;

				ImGui.ColorPicker4("##picker", ref color);

				if (color.W > beforeAlpha) {
					color.W = 255;
				} else if (color.W < beforeAlpha) {
					color.W = 0;
				}

				Color toColor = new Color(color.X, color.Y, color.Z, color.W);
				palette.Colors[_colorToChange].FromColor(toColor);
				ImGui.EndPopup();
			}

			ImGui.Columns(1);

			GuiStyle.AddSpace();

			PaletteAnimation selectedAnimation = (PaletteAnimation) SelectedTextureAnimation;

			GuiStyle.SetNewUiToDefaultStyle();
			ImGui.Indent();
			ImGui.Columns(2, "PaletteAnimationColumns", false);
			ImGui.SetColumnWidth(0, GuiStyle.LabelWidth);
			ImGui.SetColumnWidth(1, GuiStyle.WidgetWidth + 10);

			ImGui.Text("Target Palette Id");
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
			ImGui.InputInt("PA_TargetPaletteId", ref selectedAnimation.OverriddenPaletteId, 1);
			selectedAnimation.OverriddenPaletteId = Utilities.Clamp(selectedAnimation.OverriddenPaletteId, 0, 15);
			ImGui.NextColumn();

			ImGui.Text("Start Index");
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
			ImGui.InputInt("PA_StartIndex", ref selectedAnimation.AnimationStartIndex, 1);
			selectedAnimation.AnimationStartIndex = Utilities.Clamp(selectedAnimation.AnimationStartIndex, 0, 14);
			ImGui.NextColumn();

			ImGui.Text("Total Frames");
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
			ImGui.InputInt("PA_FrameCount", ref selectedAnimation.FrameCount, 1);
			selectedAnimation.FrameCount = Utilities.Clamp(
				selectedAnimation.FrameCount, 0, 15 - selectedAnimation.AnimationStartIndex
			);
			ImGui.NextColumn();

			ImGui.Text("Frame Duration");
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
			ImGui.InputInt("PA_FrameDuration", ref selectedAnimation.FrameDuration, 1);
			selectedAnimation.FrameDuration = Utilities.Min(selectedAnimation.FrameDuration, 1);
			ImGui.NextColumn();

			ImGui.Columns(1);
			ImGui.Unindent();
		}

		private static void RenderUvAnimation() {
			UvAnimation selectedAnimation = (UvAnimation) SelectedTextureAnimation;

			GuiStyle.SetNewUiToDefaultStyle();

			if (ImGui.Button("< Return")) {
				CurrentPanelMode = PanelMode.AnimationsList;
			}

			GuiStyle.AddSpace();

			ImGui.Indent();
			ImGui.Columns(2, "UVA_AnimationColumns", false);
			ImGui.SetColumnWidth(0, GuiStyle.LabelWidth);
			ImGui.SetColumnWidth(1, GuiStyle.WidgetWidth + 10);

			ImGui.Text("Animation Mode");
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
			int animationMode = (int) selectedAnimation.UvAnimationMode;

			List<string> animationModes = Enum.GetNames(typeof(UvAnimationMode)).ToList();
			for (int animationModeIndex = 0; animationModeIndex < animationModes.Count; animationModeIndex++) {
				animationModes[animationModeIndex] = Regex.Replace(
					animationModes[animationModeIndex], "(\\B[A-Z])", " $1"
				);
			}

			ImGui.Combo("UVA_AnimationMode", ref animationMode, animationModes.ToArray(), animationModes.Count);
			selectedAnimation.UvAnimationMode = (UvAnimationMode) animationMode;
			ImGui.NextColumn();

			ImGui.Text("Preview Page");
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
			string[] options = {"Canvas Page", "Frames Page"};
			ImGui.Combo("UVA_PreviewPage", ref TextureAnimationPreviewPageId, options, 2);
			TextureAnimationPreviewPageId = Utilities.Clamp(TextureAnimationPreviewPageId, 0, 15);
			ImGui.NextColumn();

			ImGui.Text("Preview Palette");
			ImGui.NextColumn();

			ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
			ImGui.InputInt("UVA_PreviewPaletteId", ref TextureAnimationPreviewPaletteId, 1);
			TextureAnimationPreviewPaletteId = Utilities.Clamp(TextureAnimationPreviewPaletteId, 0, 15);

			ImGui.Columns(1);
			ImGui.Unindent();

			GuiStyle.SetElementStyle(ElementStyle.Header);
			if (ImGui.CollapsingHeader("Canvas Properties", ImGuiTreeNodeFlags.DefaultOpen)) {
				GuiStyle.SetNewUiToDefaultStyle();
				ImGui.Indent();
				ImGui.Columns(2, "UVA_CanvasPropertiesColumns", false);
				ImGui.SetColumnWidth(0, GuiStyle.LabelWidth);
				ImGui.SetColumnWidth(1, GuiStyle.WidgetWidth + 10);

				ImGui.Text("Texture Page");
				ImGui.NextColumn();

				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.InputInt("PA_CanvasTexturePage", ref selectedAnimation.CanvasTexturePage, 1);
				selectedAnimation.CanvasTexturePage = Utilities.Clamp(selectedAnimation.CanvasTexturePage, 0, 3);
				ImGui.NextColumn();

				GuiStyle.AddSpace();
				ImGui.Text("Position X");
				ImGui.NextColumn();

				GuiStyle.AddSpace();
				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.InputInt("PA_CanvasPositionX", ref selectedAnimation.CanvasX, 4);
				selectedAnimation.CanvasX = Utilities.Clamp(selectedAnimation.CanvasX, 0, 252);
				selectedAnimation.CanvasX = Utilities.SnapIntToInterval(selectedAnimation.CanvasX, 4);
				ImGui.NextColumn();

				ImGui.Text("Position Y");
				ImGui.NextColumn();

				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.InputInt("PA_CanvasPositionY", ref selectedAnimation.CanvasY, 1);
				selectedAnimation.CanvasY = Utilities.Clamp(selectedAnimation.CanvasY, 0, 252);
				ImGui.NextColumn();

				GuiStyle.AddSpace();
				ImGui.Text("Size X");
				ImGui.NextColumn();

				GuiStyle.AddSpace();
				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.InputInt("PA_CanvasSizeX", ref selectedAnimation.SizeWidth, 4);
				selectedAnimation.SizeWidth = Utilities.Clamp(selectedAnimation.SizeWidth, 0, 252);
				selectedAnimation.SizeWidth = Utilities.SnapIntToInterval(selectedAnimation.SizeWidth, 4);
				ImGui.NextColumn();

				ImGui.Text("Size Y");
				ImGui.NextColumn();

				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.InputInt("PA_CanvasSizeY", ref selectedAnimation.SizeHeight, 1);
				selectedAnimation.SizeHeight = Utilities.Clamp(selectedAnimation.SizeHeight, 0, 252);
				ImGui.NextColumn();

				ImGui.Unindent();
				ImGui.Columns(1);
			}


			GuiStyle.SetElementStyle(ElementStyle.Header);
			if (ImGui.CollapsingHeader("Frame Properties", ImGuiTreeNodeFlags.DefaultOpen)) {
				GuiStyle.SetNewUiToDefaultStyle();
				ImGui.Indent();
				ImGui.Columns(2, "UVA_FramePropertiesColumns", false);
				ImGui.SetColumnWidth(0, GuiStyle.LabelWidth);
				ImGui.SetColumnWidth(1, GuiStyle.WidgetWidth + 10);

				ImGui.Text("Texture Page");
				ImGui.NextColumn();

				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.InputInt("PA_FrameTexturePage", ref selectedAnimation.FirstFrameTexturePage, 1);
				selectedAnimation.FirstFrameTexturePage =
					Utilities.Clamp(selectedAnimation.FirstFrameTexturePage, 0, 3);
				ImGui.NextColumn();

				GuiStyle.AddSpace();
				ImGui.Text("Position X");
				ImGui.NextColumn();

				GuiStyle.AddSpace();
				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.InputInt("PA_FramePositionX", ref selectedAnimation.FirstFrameX, 4);
				selectedAnimation.FirstFrameX = Utilities.Clamp(selectedAnimation.FirstFrameX, 0, 252);
				selectedAnimation.FirstFrameX = Utilities.SnapIntToInterval(selectedAnimation.FirstFrameX, 4);
				ImGui.NextColumn();

				ImGui.Text("Position Y");
				ImGui.NextColumn();

				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.InputInt("PA_FramePositionY", ref selectedAnimation.FirstFrameY, 1);
				selectedAnimation.FirstFrameY = Utilities.Clamp(selectedAnimation.FirstFrameY, 0, 252);
				ImGui.NextColumn();

				GuiStyle.AddSpace();
				ImGui.Text("Total Frames");
				ImGui.NextColumn();

				GuiStyle.AddSpace();
				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.InputInt("PA_FrameCount", ref selectedAnimation.FrameCount, 1);
				selectedAnimation.FrameCount = Utilities.Clamp(selectedAnimation.FrameCount, 1, 252);
				ImGui.NextColumn();

				ImGui.Text("Frame Duration");
				ImGui.NextColumn();

				ImGui.SetNextItemWidth(GuiStyle.WidgetWidth);
				ImGui.InputInt("PA_FrameDuration", ref selectedAnimation.FrameDuration, 1);
				selectedAnimation.FrameDuration = Utilities.Min(selectedAnimation.FrameDuration, 1);
				ImGui.NextColumn();

				ImGui.Unindent();
				ImGui.Columns(1);
			}

			ImGui.Columns(1);
			ImGui.Unindent();
		}
	}
}