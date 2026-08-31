using System;
using System.IO;
using GaneshaDx.Environment;
using GaneshaDx.Rendering;
using GaneshaDx.Resources;
using DirectionalLight = GaneshaDx.Resources.ContentDataTypes.DirectionalLight;
using Palette = GaneshaDx.Resources.ContentDataTypes.Palettes.Palette;
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
	private const int TexturePadding = 8;

	private static readonly RasterizerState NoCullRasterizer = new() {
		CullMode = CullMode.None
	};
	private static Texture2D _cachedStateTexture;
	private static Color[] _cachedStateTextureColors = Array.Empty<Color>();

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

		bool[] uvIslandMask = BuildUvIslandMask(polygon, textureBounds);
		bool polygonUsesValidTransparency = PolygonUsesValidTransparency(polygon, textureBounds, uvIslandMask);

		if (!polygonUsesValidTransparency) {
			FillTransparentIslandPixels(bakedColors, uvIslandMask, textureBounds.Width, textureBounds.Height);
			ApplyUvPadding(bakedColors, uvIslandMask, textureBounds.Width, textureBounds.Height, TexturePadding);
		}

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
		Stage.FftPolygonEffect.Parameters["PaletteColors"].SetValue(
			BuildExportPaletteColors(SceneRenderer.AnimationAdjustedPalettes[polygon.PaletteId])
		);

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

	private static XnaVector4[] BuildExportPaletteColors(Palette palette) {
		XnaVector4[] shaderColors = new XnaVector4[palette.Colors.Count];

		for (int colorIndex = 0; colorIndex < palette.Colors.Count; colorIndex++) {
			shaderColors[colorIndex] = palette.Colors[colorIndex].ToColor(false).ToVector4();
		}

		return shaderColors;
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

	private static bool[] BuildUvIslandMask(Polygon polygon, Rectangle textureBounds) {
		bool[] uvIslandMask = new bool[textureBounds.Width * textureBounds.Height];
		XnaVector2[] localUvs = BuildLocalUvCoordinates(polygon, textureBounds);

		for (int y = 0; y < textureBounds.Height; y++) {
			for (int x = 0; x < textureBounds.Width; x++) {
				XnaVector2 point = new(x + 0.5f, y + 0.5f);
				int pixelIndex = x + y * textureBounds.Width;
				uvIslandMask[pixelIndex] = polygon.IsQuad
					? PointIsWithinTriangleInclusive(point, localUvs[0], localUvs[1], localUvs[2]) ||
					  PointIsWithinTriangleInclusive(point, localUvs[2], localUvs[1], localUvs[3])
					: PointIsWithinTriangleInclusive(point, localUvs[0], localUvs[1], localUvs[2]);
			}
		}

		return uvIslandMask;
	}

	private static XnaVector2[] BuildLocalUvCoordinates(Polygon polygon, Rectangle textureBounds) {
		XnaVector2[] localUvs = new XnaVector2[polygon.UvCoordinates.Count];

		for (int vertexIndex = 0; vertexIndex < polygon.UvCoordinates.Count; vertexIndex++) {
			float atlasY = polygon.UvCoordinates[vertexIndex].Y + polygon.TexturePage * 256;
			localUvs[vertexIndex] = new XnaVector2(
				polygon.UvCoordinates[vertexIndex].X - textureBounds.X,
				atlasY - textureBounds.Y
			);
		}

		return localUvs;
	}

	private static bool PolygonUsesValidTransparency(Polygon polygon, Rectangle textureBounds, bool[] uvIslandMask) {
		Palette palette = SceneRenderer.AnimationAdjustedPalettes[polygon.PaletteId];
		bool[] transparentPaletteIndices = new bool[palette.Colors.Count];
		bool paletteContainsTransparency = false;

		for (int colorIndex = 0; colorIndex < palette.Colors.Count; colorIndex++) {
			transparentPaletteIndices[colorIndex] = palette.Colors[colorIndex].ToColor(false).A == 0;
			paletteContainsTransparency |= transparentPaletteIndices[colorIndex];
		}

		Color[] textureColors = GetStateTextureColors();

		for (int y = 0; y < textureBounds.Height; y++) {
			for (int x = 0; x < textureBounds.Width; x++) {
				int pixelIndex = x + y * textureBounds.Width;
				if (!uvIslandMask[pixelIndex]) {
					continue;
				}

				int atlasX = x + textureBounds.X;
				int atlasY = y + textureBounds.Y;
				Color textureColor = textureColors[atlasX + atlasY * TextureAtlasWidth];
				if (textureColor.A < 255) {
					return true;
				}

				if (!paletteContainsTransparency) {
					continue;
				}

				int paletteIndex = GetPaletteIndex(textureColor);
				if (paletteIndex >= 0 && transparentPaletteIndices[paletteIndex]) {
					return true;
				}
			}
		}

		return false;
	}

	private static Color[] GetStateTextureColors() {
		Texture2D stateTexture = CurrentMapState.StateData.Texture;
		if (!ReferenceEquals(_cachedStateTexture, stateTexture) || _cachedStateTextureColors.Length != stateTexture.Width * stateTexture.Height) {
			_cachedStateTexture = stateTexture;
			_cachedStateTextureColors = new Color[stateTexture.Width * stateTexture.Height];
			stateTexture.GetData(_cachedStateTextureColors);
		}

		return _cachedStateTextureColors;
	}

	private static int GetPaletteIndex(Color textureColor) {
		float red = textureColor.R / 255f;

		for (int index = 0; index < 16; index++) {
			if (red >= index * 16f / 255f && red <= index * 18f / 255f) {
				return index;
			}
		}

		return -1;
	}

	private static bool PointIsWithinTriangleInclusive(
		XnaVector2 point,
		XnaVector2 trianglePointA,
		XnaVector2 trianglePointB,
		XnaVector2 trianglePointC
	) {
		float denominator =
			(trianglePointB.Y - trianglePointC.Y) * (trianglePointA.X - trianglePointC.X) +
			(trianglePointC.X - trianglePointB.X) * (trianglePointA.Y - trianglePointC.Y);

		if (Math.Abs(denominator) < 0.0001f) {
			return false;
		}

		float alpha =
			((trianglePointB.Y - trianglePointC.Y) * (point.X - trianglePointC.X) +
			 (trianglePointC.X - trianglePointB.X) * (point.Y - trianglePointC.Y)) /
			denominator;
		float beta =
			((trianglePointC.Y - trianglePointA.Y) * (point.X - trianglePointC.X) +
			 (trianglePointA.X - trianglePointC.X) * (point.Y - trianglePointC.Y)) /
			denominator;
		float gamma = 1f - alpha - beta;

		const float epsilon = -0.001f;
		return alpha >= epsilon && beta >= epsilon && gamma >= epsilon;
	}

	private static void FillTransparentIslandPixels(
		Color[] bakedColors,
		bool[] uvIslandMask,
		int textureWidth,
		int textureHeight
	) {
		Color[] repairedColors = new Color[bakedColors.Length];
		Array.Copy(bakedColors, repairedColors, bakedColors.Length);
		int maxSearchDistance = Math.Max(textureWidth, textureHeight);

		for (int y = 0; y < textureHeight; y++) {
			for (int x = 0; x < textureWidth; x++) {
				int pixelIndex = x + y * textureWidth;
				if (!uvIslandMask[pixelIndex] || bakedColors[pixelIndex].A != 0) {
					continue;
				}

				if (TryGetNearestIslandColor(
					bakedColors,
					uvIslandMask,
					textureWidth,
					textureHeight,
					x,
					y,
					maxSearchDistance,
					out Color sourceColor
				)) {
					repairedColors[pixelIndex] = sourceColor;
				}
			}
		}

		Array.Copy(repairedColors, bakedColors, bakedColors.Length);
	}

	private static bool TryGetNearestIslandColor(
		Color[] sourceColors,
		bool[] uvIslandMask,
		int textureWidth,
		int textureHeight,
		int x,
		int y,
		int maxSearchDistance,
		out Color sourceColor
	) {
		sourceColor = Color.Transparent;
		float nearestDistanceSquared = float.MaxValue;

		for (int searchDistance = 1; searchDistance <= maxSearchDistance; searchDistance++) {
			bool foundSourcePixel = false;
			int minX = Math.Max(0, x - searchDistance);
			int maxX = Math.Min(textureWidth - 1, x + searchDistance);
			int minY = Math.Max(0, y - searchDistance);
			int maxY = Math.Min(textureHeight - 1, y + searchDistance);

			for (int sampleY = minY; sampleY <= maxY; sampleY++) {
				for (int sampleX = minX; sampleX <= maxX; sampleX++) {
					bool isPerimeterPixel =
						sampleX == minX || sampleX == maxX || sampleY == minY || sampleY == maxY;
					if (!isPerimeterPixel) {
						continue;
					}

					int sampleIndex = sampleX + sampleY * textureWidth;
					if (!uvIslandMask[sampleIndex]) {
						continue;
					}

					Color candidateColor = sourceColors[sampleIndex];
					if (candidateColor.A == 0) {
						continue;
					}

					float deltaX = sampleX - x;
					float deltaY = sampleY - y;
					float distanceSquared = deltaX * deltaX + deltaY * deltaY;
					if (distanceSquared >= nearestDistanceSquared) {
						continue;
					}

					nearestDistanceSquared = distanceSquared;
					sourceColor = candidateColor;
					foundSourcePixel = true;
				}
			}

			if (foundSourcePixel) {
				return true;
			}
		}

		return false;
	}

	private static void ApplyUvPadding(
		Color[] bakedColors,
		bool[] uvIslandMask,
		int textureWidth,
		int textureHeight,
		int paddingSize
	) {
		Color[] currentColors = new Color[bakedColors.Length];
		Array.Copy(bakedColors, currentColors, bakedColors.Length);

		for (int paddingStep = 0; paddingStep < paddingSize; paddingStep++) {
			Color[] nextColors = new Color[currentColors.Length];
			Array.Copy(currentColors, nextColors, currentColors.Length);

			for (int y = 0; y < textureHeight; y++) {
				for (int x = 0; x < textureWidth; x++) {
					int pixelIndex = x + y * textureWidth;

					if (uvIslandMask[pixelIndex] || currentColors[pixelIndex].A != 0) {
						continue;
					}

					if (TryGetPaddingSourceColor(currentColors, textureWidth, textureHeight, x, y, out Color sourceColor)) {
						nextColors[pixelIndex] = sourceColor;
					}
				}
			}

			currentColors = nextColors;
		}

		Array.Copy(currentColors, bakedColors, bakedColors.Length);
	}

	private static bool TryGetPaddingSourceColor(
		Color[] sourceColors,
		int textureWidth,
		int textureHeight,
		int x,
		int y,
		out Color sourceColor
	) {
		sourceColor = Color.Transparent;
		XnaVector2[] directions = {
			new XnaVector2(0, -1),
			new XnaVector2(-1, 0),
			new XnaVector2(1, 0),
			new XnaVector2(0, 1),
			new XnaVector2(-1, -1),
			new XnaVector2(1, -1),
			new XnaVector2(-1, 1),
			new XnaVector2(1, 1)
		};

		foreach (XnaVector2 direction in directions) {
			int sampleX = x + (int) direction.X;
			int sampleY = y + (int) direction.Y;

			if (sampleX < 0 || sampleX >= textureWidth || sampleY < 0 || sampleY >= textureHeight) {
				continue;
			}

			Color candidateColor = sourceColors[sampleX + sampleY * textureWidth];
			if (candidateColor.A == 0) {
				continue;
			}

			sourceColor = candidateColor;
			return true;
		}

		return false;
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


