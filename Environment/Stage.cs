using GaneshaDx.Resources;
using GaneshaDx.UserInterface;
using GaneshaDx.UserInterface.GuiDefinitions;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GaneshaDx.Environment;

/// <summary>
/// The Stage contains references to all the elements needed for the base functionality of the App.
/// This includes references to the Graphics Device, the Game Window, 3D Projection Matrices, Default Viewports,
/// and Default Renderers. 
/// </summary>
public static class Stage {
	public static Ganesha Ganesha;
	public static Matrix ProjectionMatrix;
	public static Matrix ViewMatrix;
	public static Matrix WorldMatrix;
	public static GraphicsDeviceManager Graphics;
	public static GraphicsDevice GraphicsDevice;
	public static SpriteBatch SpriteBatch;
	public static BasicEffect BasicEffect;
	public static Effect FftPolygonEffect;
	public static int Width;
	public static int Height;
	public static ContentManager Content;
	public static GameWindow Window;
	public static Viewport WholeViewport;
	public static Viewport ModelingViewport;
	public static GameTime GameTime;
	public static VertexBuffer UntexturedVertexBuffer;
	public static VertexBuffer PolygonVertexBuffer;
	public static ImGuiRenderer ImGuiRenderer;
	public static bool FullModelingViewportMode;
	public static RenderTarget2D ImGuiRenderTarget;
	public static RenderTarget2D UvPreviewRenderTarget;
	public static bool ScreenshotMode;

	public static void SetStage(
		Ganesha ganesha,
		GraphicsDevice graphicsDevice,
		GraphicsDeviceManager graphics,
		SpriteBatch spriteBatch,
		ContentManager content,
		GameWindow window
	) {
		Ganesha = ganesha;
		GraphicsDevice = graphicsDevice;
		Graphics = graphics;
		SpriteBatch = spriteBatch;
		Content = content;
		Window = window;

		WholeViewport = new Viewport {MinDepth = 0, MaxDepth = 1};
		ModelingViewport = new Viewport {MinDepth = 0, MaxDepth = 1};

		GraphicsDevice.Viewport = WholeViewport;
		GraphicsDevice.RasterizerState = new RasterizerState {
			CullMode = CullMode.CullCounterClockwiseFace
		};

		ViewMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.Zero, new Vector3(0f, 1f, 0f));
		WorldMatrix = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
		ProjectionMatrix = Matrix.CreateOrthographic(ModelingViewport.Width, ModelingViewport.Height, 1f, 5000f);

		UntexturedVertexBuffer = new VertexBuffer(
			GraphicsDevice,
			typeof(VertexPositionColorTexture),
			300,
			BufferUsage.WriteOnly
		);

		PolygonVertexBuffer = new VertexBuffer(
			GraphicsDevice,
			typeof(VertexPositionNormalTexture),
			300,
			BufferUsage.WriteOnly
		);

		BasicEffect = new BasicEffect(GraphicsDevice) {
			Projection = ProjectionMatrix,
			View = ViewMatrix,
			World = WorldMatrix
		};

		FftPolygonEffect = Content.Load<Effect>("FFTPolygonShader");

		ImGuiRenderer = new ImGuiRenderer(Ganesha);
		ImGuiRenderer.RebuildFontAtlas();
		
		ImGuiIOPtr io = ImGui.GetIO();
		io.Fonts.AddFontFromFileTTF("./Content/Roboto-Medium.ttf", 14.0f, null, io.Fonts.GetGlyphRangesCyrillic());
		io.Fonts.AddFontFromFileTTF("./Content/Roboto-Medium.ttf", 16.0f, null, io.Fonts.GetGlyphRangesCyrillic());
		io.Fonts.AddFontFromFileTTF("./Content/icomoon.ttf", 16.0f, null, io.Fonts.GetGlyphRangesCyrillic());
		io.Fonts.Build();
		io.ConfigWindowsMoveFromTitleBarOnly = true;

		ImGuiRenderer.RebuildFontAtlas();
		
		UpdateRenderTargets();
		Window.ClientSizeChanged += WindowSizeChanged;
		Window.FileDrop += DroppedFileIn;
	}

	public static void Update(GameTime gameTime) {
		GameTime = gameTime;
		Width = GraphicsDevice.PresentationParameters.Bounds.Width;
		Height = GraphicsDevice.PresentationParameters.Bounds.Height;

		WholeViewport.Width = Width;
		WholeViewport.Height = Height;

		if (ScreenshotMode || FullModelingViewportMode) {
			ModelingViewport.Width = Width;
			ModelingViewport.Height = Height;
			ModelingViewport.X = 0;
			ModelingViewport.Y = 0;
		} else {
			ModelingViewport.Width = Width - GuiStyle.RightPanelWidth;
			ModelingViewport.Height = Height - 20;
			ModelingViewport.X = 0;
			ModelingViewport.Y = 20;
		}

		GraphicsDevice.Viewport = WholeViewport;
	}

	public static void UpdateEffects() {
		BasicEffect.Projection = ProjectionMatrix;
		BasicEffect.View = ViewMatrix;
		BasicEffect.World = WorldMatrix;

		Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(WorldMatrix));
		FftPolygonEffect.Parameters["Projection"].SetValue(ProjectionMatrix);
		FftPolygonEffect.Parameters["View"].SetValue(ViewMatrix);
		FftPolygonEffect.Parameters["World"].SetValue(WorldMatrix);
		FftPolygonEffect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
	}

	public static void ToggleScreenshotMode() {
		ScreenshotMode = !ScreenshotMode;
		if (ScreenshotMode) {
			Gui.SelectedTab = RightPanelTab.Polygon;
		}
	}

	private static void WindowSizeChanged(object sender, System.EventArgs e) {
		Window.ClientSizeChanged -= WindowSizeChanged;
		UpdateRenderTargets();
		Window.ClientSizeChanged += WindowSizeChanged;
	}

	private static void UpdateRenderTargets() {
		ImGuiRenderTarget = new RenderTarget2D(
			GraphicsDevice,
			GraphicsDevice.PresentationParameters.BackBufferWidth,
			GraphicsDevice.PresentationParameters.BackBufferHeight,
			false,
			GraphicsDevice.PresentationParameters.BackBufferFormat,
			DepthFormat.Depth24);

		UvPreviewRenderTarget = new RenderTarget2D(
			GraphicsDevice,
			256,
			256,
			false,
			GraphicsDevice.PresentationParameters.BackBufferFormat,
			DepthFormat.Depth24);
	}

	private static void DroppedFileIn(object sender, FileDropEventArgs e) {
		MapData.LoadMapDataFromFullPath(e.Files[0]);
	}
}