using GaneshaDx.Common;
using GaneshaDx.Environment;
using GaneshaDx.Rendering;
using GaneshaDx.UserInterface;
using GaneshaDx.UserInterface.GuiForms;
using GaneshaDx.UserInterface.Input;
using GaneshaDx.UserInterface.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace GaneshaDx {
	public class Ganesha : Game {
		private readonly GraphicsDeviceManager _graphics;

		public Ganesha() {
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			Window.AllowUserResizing = true;
			IsMouseVisible = true;
		}

		protected override void Initialize() {
			Configuration.LoadConfiguration();
			_graphics.PreferredBackBufferWidth = 1366;
			_graphics.PreferredBackBufferHeight = 768;
			_graphics.ApplyChanges();

			Stage.SetStage(this, GraphicsDevice, _graphics, new SpriteBatch(GraphicsDevice), Content, Window);
			Background.SetAsGradient(Utilities.GetColorFromHex("0a0e16"), Color.Black);
			MyraGui.BuildUi();

			base.Initialize();
		}

		protected override void Update(GameTime gameTime) {
			Stage.Update(gameTime);
			AppInput.Update();
			AppShortcuts.Update();
			StageCamera.Update();
			MeshAnimationController.Update();
			SceneRenderer.Update();
			TransformWidget.Update();
			RotationWidget.Update();
			Selection.Update();
			OverlayConsole.Update();
			FpsCounter.Update();
			GuiWindowTextureElement.Update();
			AutoSaver.Update();

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.SetRenderTarget(Stage.ImGuiRenderTarget);
			Gui.Render();

			GraphicsDevice.SetRenderTarget(Stage.UvPreviewRenderTarget);
			GuiWindowTextureElement.Render();

			GraphicsDevice.SetRenderTarget(null);
			GraphicsDevice.Clear(Color.Black);
			Stage.UpdateEffects();
			Background.Render();
			SceneRenderer.Render();
			TransformWidget.Render();
			RotationWidget.Render();
			OverlayConsole.Render();
			MyraGui.Render();
			FpsCounter.DrawFps();

			Stage.SpriteBatch.Begin(
				SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
				DepthStencilState.Default, RasterizerState.CullNone
			);
			
			Stage.SpriteBatch.Draw(Stage.ImGuiRenderTarget, GraphicsDevice.PresentationParameters.Bounds, Color.White);

			if (GuiWindowTexturePreview.ShouldRenderTexture) {
				Stage.SpriteBatch.Draw(Stage.UvPreviewRenderTarget, GuiWindowTexturePreview.UvPreviewBounds, Color.White);
			}

			Stage.SpriteBatch.End();

			base.Draw(gameTime);
		}
	}
}