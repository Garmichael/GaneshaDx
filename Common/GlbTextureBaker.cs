using System;
using System.IO;
using GaneshaDx.Environment;
using GaneshaDx.Rendering;
using GaneshaDx.Resources;
using DirectionalLight = GaneshaDx.Resources.ContentDataTypes.DirectionalLight;
using GaneshaDx.Resources.ContentDataTypes.Polygons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaVector2 = Microsoft.Xna.Framework.Vector2;
using XnaVector3 = Microsoft.Xna.Framework.Vector3;
using XnaVector4 = Microsoft.Xna.Framework.Vector4;

namespace GaneshaDx.Common;

public readonly struct GlbBakedTextureData {
	public GlbBakedTextureData(byte[] textureBytes, XnaVector2[] uvCoordinates) {
		TextureBytes = textureBytes;
		UvCoordinates = uvCoordinates;
	}

	public byte[] TextureBytes { get; }
	public XnaVector2[] UvCoordinates { get; }
}

public static class GlbTextureBaker {
	private const int TextureAtlasWidth = 256;
	private const int TextureAtlasHeight = 1024;
	private const int TexturePadding = 1;

	private static readonly RasterizerState NoCullRasterizer = new() {
		CullMode = CullMode.None
	};

	public static GlbBakedTextureData BakePolygonTexture(Polygon polygon) {
		Rectangle textureBounds = GetTextureBounds(polygon);

		using RenderTarget2D renderTarget = new(
			Stage.GraphicsDevice,
			textureBounds.Width,
			textureBounds.Height,
			false,
			SurfaceFormat.Color,
			DepthFormat.Depth24
		);

		RenderTargetBinding[] previousRenderTargets = Stage.GraphicsDevice.GetRenderTargets();
		Viewport previousViewport = Stage.GraphicsDevice.Viewport;
		BlendState previousBlendState = Stage.GraphicsDevice.BlendState;
		DepthStencilState previousDepthStencilState = Stage.GraphicsDevice.DepthStencilState;
		RasterizerState previousRasterizerState = Stage.GraphicsDevice.RasterizerState;

		try {
			Stage.GraphicsDevice.SetRenderTarget(renderTarget);
			Stage.GraphicsDevice.Viewport = new Viewport(0, 0, textureBounds.Width, textureBounds.Height);
			Stage.GraphicsDevice.Clear(Color.Transparent);
			Stage.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
			Stage.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			Stage.GraphicsDevice.RasterizerState = NoCullRasterizer;

			SetPolygonEffect(polygon, textureBounds.Width, textureBounds.Height);
			Stage.PolygonVertexBuffer.SetData(BuildTextureRenderVertices(polygon, textureBounds));
			Stage.GraphicsDevice.SetVertexBuffer(Stage.PolygonVertexBuffer);

			foreach (EffectPass pass in Stage.FftPolygonEffect.CurrentTechnique.Passes) {
				pass.Apply();
				Stage.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, polygon.IsQuad ? 2 : 1);
			}
		} finally {
			Stage.GraphicsDevice.SetRenderTargets(previousRenderTargets);
			Stage.GraphicsDevice.Viewport = previousViewport;
			Stage.GraphicsDevice.BlendState = previousBlendState;
			Stage.GraphicsDevice.DepthStencilState = previousDepthStencilState;
			Stage.GraphicsDevice.RasterizerState = previousRasterizerState;
		}

		Color[] bakedColors = new Color[textureBounds.Width * textureBounds.Height];
		renderTarget.GetData(bakedColors);

		using Texture2D bakedTexture = new(Stage.GraphicsDevice, textureBounds.Width, textureBounds.Height);
		bakedTexture.SetData(bakedColors);

		using MemoryStream stream = new();
		bakedTexture.SaveAsPng(stream, textureBounds.Width, textureBounds.Height);
		return new GlbBakedTextureData(stream.ToArray(), BuildExportUvs(polygon, textureBounds));
	}

	private static Rectangle GetTextureBounds(Polygon polygon) {
		float minX = float.MaxValue;
		float minY = float.MaxValue;
		float maxX = float.MinValue;
		float maxY = float.MinValue;

		foreach (XnaVector2 uv in polygon.UvCoordinates) {
			float atlasY = uv.Y + polygon.TexturePage * 256;
			minX = Math.Min(minX, uv.X);
			minY = Math.Min(minY, atlasY);
			maxX = Math.Max(maxX, uv.X);
			maxY = Math.Max(maxY, atlasY);
		}

		int left = Utilities.Clamp((int) Math.Floor(minX) - TexturePadding, 0, TextureAtlasWidth - 1);
		int top = Utilities.Clamp((int) Math.Floor(minY) - TexturePadding, 0, TextureAtlasHeight - 1);
		int right = Utilities.Clamp((int) Math.Ceiling(maxX) + TexturePadding, left + 1, TextureAtlasWidth);
		int bottom = Utilities.Clamp((int) Math.Ceiling(maxY) + TexturePadding, top + 1, TextureAtlasHeight);

		return new Rectangle(left, top, Math.Max(1, right - left), Math.Max(1, bottom - top));
	}

	private static void SetPolygonEffect(Polygon polygon, int textureWidth, int textureHeight) {
		Matrix projection = Matrix.CreateOrthographic(textureWidth, textureHeight, -10f, 10f);
		Matrix view = Matrix.CreateLookAt(
			new XnaVector3(textureWidth / 2f, textureHeight / 2f, 0),
			new XnaVector3(textureWidth / 2f, textureHeight / 2f, 1),
			new XnaVector3(0, -1, 0)
		);

		Stage.FftPolygonEffect.Parameters["Projection"].SetValue(projection);
		Stage.FftPolygonEffect.Parameters["View"].SetValue(view);
		Stage.FftPolygonEffect.Parameters["World"].SetValue(Matrix.Identity);
		Stage.FftPolygonEffect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Identity);
		Stage.FftPolygonEffect.Parameters["ModelTexture"].SetValue(CurrentMapState.StateData.Texture);
		Stage.FftPolygonEffect.Parameters["PaletteColors"].SetValue(SceneRenderer.AnimationAdjustedPalettes[polygon.PaletteId].ShaderColors);

		bool isUnlit = polygon.RenderingProperties != null && !polygon.RenderingProperties.LitTexture;
		Stage.FftPolygonEffect.Parameters["AmbientColor"].SetValue(
			isUnlit
				? new XnaVector4(0.5f, 0.5f, 0.5f, 1f)
				: CurrentMapState.StateData.AmbientLightColor.ToVector4()
		);

		for (int lightIndex = 0; lightIndex < 3; lightIndex++) {
			DirectionalLight light = CurrentMapState.StateData.DirectionalLights[lightIndex];
			XnaVector3 lightDirection = Utilities.SphereToVector(light.DirectionElevation, light.DirectionAzimuth);
			lightDirection.Normalize();
			Stage.FftPolygonEffect.Parameters["DirectionalLightDirection" + lightIndex].SetValue(lightDirection);
			Stage.FftPolygonEffect.Parameters["DirectionalLightColor" + lightIndex].SetValue(
				isUnlit
					? new XnaVector4(0f, 0f, 0f, 1f)
					: light.LightColor.ToVector4()
			);
		}

		Stage.FftPolygonEffect.Parameters["HighlightBright"].SetValue(false);
		Stage.FftPolygonEffect.Parameters["HighlightDim"].SetValue(false);
		Stage.FftPolygonEffect.Parameters["MaxAlpha"].SetValue(1f);
	}

	private static VertexPositionNormalTexture[] BuildTextureRenderVertices(Polygon polygon, Rectangle textureBounds) {
		VertexPositionNormalTexture[] textureVertices = polygon.IsQuad
			? new VertexPositionNormalTexture[4]
			: new VertexPositionNormalTexture[3];

		for (int vertexIndex = 0; vertexIndex < textureVertices.Length; vertexIndex++) {
			XnaVector3 normal = Utilities.SphereToVector(
				polygon.Vertices[vertexIndex].NormalElevation,
				polygon.Vertices[vertexIndex].NormalAzimuth
			);
			float atlasY = polygon.UvCoordinates[vertexIndex].Y + polygon.TexturePage * 256;
			textureVertices[vertexIndex] = new VertexPositionNormalTexture(
				new XnaVector3(
					polygon.UvCoordinates[vertexIndex].X - textureBounds.X,
					atlasY - textureBounds.Y,
					1f
				),
				normal,
				GetAdjustedUvCoordinate(polygon.UvCoordinates[vertexIndex], polygon.TexturePage)
			);
		}

		return textureVertices;
	}

	private static XnaVector2[] BuildExportUvs(Polygon polygon, Rectangle textureBounds) {
		XnaVector2[] exportUvs = new XnaVector2[polygon.UvCoordinates.Count];

		for (int vertexIndex = 0; vertexIndex < polygon.UvCoordinates.Count; vertexIndex++) {
			float atlasY = polygon.UvCoordinates[vertexIndex].Y + polygon.TexturePage * 256;
			exportUvs[vertexIndex] = new XnaVector2(
				(polygon.UvCoordinates[vertexIndex].X - textureBounds.X) / textureBounds.Width,
				(atlasY - textureBounds.Y) / textureBounds.Height
			);
		}

		return exportUvs;
	}

	private static XnaVector2 GetAdjustedUvCoordinate(XnaVector2 uv, int texturePage) {
		return new XnaVector2(uv.X / 256f, (uv.Y + texturePage * 256) / 1024f);
	}
}


