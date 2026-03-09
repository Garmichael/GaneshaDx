using System;
using GaneshaDx.Resources.ContentDataTypes.Polygons;
using System.Numerics;
using System.Text.Json;
using SharpGLTF.Geometry;
using SharpGLTF.Materials;
using SharpGLTF.Memory;
using SharpGLTF.Schema2;
using SharpGLTF.Geometry.VertexTypes;
using System.IO;
using GaneshaDx.Resources;
using System.Collections.Generic;
using System.Drawing.Imaging;
using FreeImageAPI;
using GaneshaDx.Rendering;
using GaneshaDx.Resources.ContentDataTypes.MeshAnimations;
using Microsoft.Xna.Framework;
using Vector4 = System.Numerics.Vector4;
using Vector3 = System.Numerics.Vector3;
using Vector2 = System.Numerics.Vector2;
using MTex2D = Microsoft.Xna.Framework.Graphics.Texture2D;
using GaneshaDx.Resources.ContentDataTypes.TextureAnimations;
using GaneshaDx.UserInterface.GuiForms;
using SharpGLTF.Scenes;
using AlphaMode = SharpGLTF.Materials.AlphaMode;
using Palette = GaneshaDx.Resources.ContentDataTypes.Palettes.Palette;
using VertexPosition = SharpGLTF.Geometry.VertexTypes.VertexPosition;

namespace GaneshaDx.Common;

public static class GlbExporter {
	// Scale to 1 tile is roughly 1cm x 1cm.
	private const float ReduceScaleFactorX = 0.000321F;
	private const float ReduceScaleFactorY = 0.000321F;
	private const float ReduceScaleFactorZ = 0.000335F;

	private static readonly List<Color> GreyPalette = new() {
		Utilities.GetColorFromHex("000000"),
		Utilities.GetColorFromHex("111111"),
		Utilities.GetColorFromHex("222222"),
		Utilities.GetColorFromHex("333333"),
		Utilities.GetColorFromHex("444444"),
		Utilities.GetColorFromHex("555555"),
		Utilities.GetColorFromHex("666666"),
		Utilities.GetColorFromHex("777777"),
		Utilities.GetColorFromHex("888888"),
		Utilities.GetColorFromHex("999999"),
		Utilities.GetColorFromHex("AAAAAA"),
		Utilities.GetColorFromHex("BBBBBB"),
		Utilities.GetColorFromHex("CCCCCC"),
		Utilities.GetColorFromHex("DDDDDD"),
		Utilities.GetColorFromHex("EEEEEE"),
		Utilities.GetColorFromHex("FFFFFF")
	};

	public static void Export(string filePath) {
		SceneRenderer.Update();

		Dictionary<MeshType, MeshBuilder<VertexPosition, VertexTexture1>> meshes = CreateMeshes();
		Dictionary<MeshType, PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty>> untexturedPrimitives =
			CreateUntexturedPrimitives(meshes);
		Dictionary<MeshType, List<PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty>>> texturedPrimitives =
			GuiWindowExportGlb.BakeSceneLightingIntoTextures ? null : CreateSharedTexturePrimitives(meshes);

		foreach (Polygon polygon in CurrentMapState.StateData.PolygonCollectionBucket) {
			if (polygon.IsQuad) {
				List<Vertex> vertices = polygon.Vertices;
				List<Microsoft.Xna.Framework.Vector2> uvs = polygon.UvCoordinates;

				List<Microsoft.Xna.Framework.Vector3> animatedVertices = new() {
					TranslateVertexToAnimatedPosition(vertices[0], polygon.MeshType),
					TranslateVertexToAnimatedPosition(vertices[1], polygon.MeshType),
					TranslateVertexToAnimatedPosition(vertices[2], polygon.MeshType),
					TranslateVertexToAnimatedPosition(vertices[3], polygon.MeshType),
				};

				if (polygon.IsTextured) {
					PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty> primitiveBuilder =
						GetTexturedPrimitiveBuilder(polygon, meshes, texturedPrimitives, out List<Vector2> exportUvs);

					// 0,1,3,2 is FFT vertex order, reversing here to correct facing/back-culling.
					primitiveBuilder.AddQuadrangle(
						(ConvertAndScaleVector3(animatedVertices[2]), exportUvs[2]),
						(ConvertAndScaleVector3(animatedVertices[3]), exportUvs[3]),
						(ConvertAndScaleVector3(animatedVertices[1]), exportUvs[1]),
						(ConvertAndScaleVector3(animatedVertices[0]), exportUvs[0])
					);
				} else {
					untexturedPrimitives[polygon.MeshType].AddQuadrangle(
						ConvertAndScaleVector3ToVertexPosition(animatedVertices[2]),
						ConvertAndScaleVector3ToVertexPosition(animatedVertices[3]),
						ConvertAndScaleVector3ToVertexPosition(animatedVertices[1]),
						ConvertAndScaleVector3ToVertexPosition(animatedVertices[0])
					);
				}
			} else {
				List<Vertex> vertices = polygon.Vertices;
				List<Microsoft.Xna.Framework.Vector2> uvs = polygon.UvCoordinates;

				List<Microsoft.Xna.Framework.Vector3> animatedVertices = new() {
					TranslateVertexToAnimatedPosition(vertices[0], polygon.MeshType),
					TranslateVertexToAnimatedPosition(vertices[1], polygon.MeshType),
					TranslateVertexToAnimatedPosition(vertices[2], polygon.MeshType)
				};

				if (polygon.IsTextured) {
					PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty> primitiveBuilder =
						GetTexturedPrimitiveBuilder(polygon, meshes, texturedPrimitives, out List<Vector2> exportUvs);

					primitiveBuilder.AddTriangle(
						(ConvertAndScaleVector3(animatedVertices[2]), exportUvs[2]),
						(ConvertAndScaleVector3(animatedVertices[1]), exportUvs[1]),
						(ConvertAndScaleVector3(animatedVertices[0]), exportUvs[0])
					);
				} else {
					untexturedPrimitives[polygon.MeshType].AddTriangle(
						ConvertAndScaleVector3ToVertexPosition(animatedVertices[2]),
						ConvertAndScaleVector3ToVertexPosition(animatedVertices[1]),
						ConvertAndScaleVector3ToVertexPosition(animatedVertices[0])
					);
				}
			}
		}

		SceneBuilder scene = new();
		scene.AddRigidMesh(meshes[MeshType.PrimaryMesh], Matrix4x4.Identity);
		scene.AddRigidMesh(meshes[MeshType.AnimatedMesh1], Matrix4x4.Identity);
		scene.AddRigidMesh(meshes[MeshType.AnimatedMesh2], Matrix4x4.Identity);
		scene.AddRigidMesh(meshes[MeshType.AnimatedMesh3], Matrix4x4.Identity);
		scene.AddRigidMesh(meshes[MeshType.AnimatedMesh4], Matrix4x4.Identity);
		scene.AddRigidMesh(meshes[MeshType.AnimatedMesh5], Matrix4x4.Identity);
		scene.AddRigidMesh(meshes[MeshType.AnimatedMesh6], Matrix4x4.Identity);
		scene.AddRigidMesh(meshes[MeshType.AnimatedMesh7], Matrix4x4.Identity);
		scene.AddRigidMesh(meshes[MeshType.AnimatedMesh8], Matrix4x4.Identity);

		ModelRoot model = scene.ToGltf2();
		WriteSettings settings = new() {
			ImageWriting = ResourceWriteMode.EmbeddedAsBase64,
			MergeBuffers = true
		};
		JsonWriterOptions jsonWriterOptions = new() {
			Indented = true
		};
		settings.JsonOptions = jsonWriterOptions;
		model.SaveGLB(filePath, settings);
	}

	private static Dictionary<MeshType, MeshBuilder<VertexPosition, VertexTexture1>> CreateMeshes() {
		return new Dictionary<MeshType, MeshBuilder<VertexPosition, VertexTexture1>> {
			{ MeshType.PrimaryMesh, new MeshBuilder<VertexPosition, VertexTexture1>("PrimaryMesh") },
			{ MeshType.AnimatedMesh1, new MeshBuilder<VertexPosition, VertexTexture1>("AnimatedMesh1") },
			{ MeshType.AnimatedMesh2, new MeshBuilder<VertexPosition, VertexTexture1>("AnimatedMesh2") },
			{ MeshType.AnimatedMesh3, new MeshBuilder<VertexPosition, VertexTexture1>("AnimatedMesh3") },
			{ MeshType.AnimatedMesh4, new MeshBuilder<VertexPosition, VertexTexture1>("AnimatedMesh4") },
			{ MeshType.AnimatedMesh5, new MeshBuilder<VertexPosition, VertexTexture1>("AnimatedMesh5") },
			{ MeshType.AnimatedMesh6, new MeshBuilder<VertexPosition, VertexTexture1>("AnimatedMesh6") },
			{ MeshType.AnimatedMesh7, new MeshBuilder<VertexPosition, VertexTexture1>("AnimatedMesh7") },
			{ MeshType.AnimatedMesh8, new MeshBuilder<VertexPosition, VertexTexture1>("AnimatedMesh8") },
		};
	}

	private static Dictionary<MeshType, PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty>> CreateUntexturedPrimitives(
		Dictionary<MeshType, MeshBuilder<VertexPosition, VertexTexture1>> meshes
	) {
		MaterialBuilder blackMaterial = new();
		blackMaterial.WithBaseColor(new Vector4(0, 0, 0, 1));
		blackMaterial.WithUnlitShader();

		Dictionary<MeshType, PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty>> untexturedPrimitives = new();
		foreach (KeyValuePair<MeshType, MeshBuilder<VertexPosition, VertexTexture1>> mesh in meshes) {
			untexturedPrimitives.Add(mesh.Key, mesh.Value.UsePrimitive(blackMaterial));
		}

		return untexturedPrimitives;
	}

	private static Dictionary<MeshType, List<PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty>>> CreateSharedTexturePrimitives(
		Dictionary<MeshType, MeshBuilder<VertexPosition, VertexTexture1>> meshes
	) {
		Dictionary<MeshType, List<PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty>>> texturedPrimitives = new() {
			{ MeshType.PrimaryMesh, new List<PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty>>() },
			{ MeshType.AnimatedMesh1, new List<PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty>>() },
			{ MeshType.AnimatedMesh2, new List<PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty>>() },
			{ MeshType.AnimatedMesh3, new List<PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty>>() },
			{ MeshType.AnimatedMesh4, new List<PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty>>() },
			{ MeshType.AnimatedMesh5, new List<PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty>>() },
			{ MeshType.AnimatedMesh6, new List<PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty>>() },
			{ MeshType.AnimatedMesh7, new List<PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty>>() },
			{ MeshType.AnimatedMesh8, new List<PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty>>() },
		};

		MTex2D stateTexture = CurrentMapState.StateData.Texture;

		foreach (Palette palette in CurrentMapState.StateData.Palettes) {
			Color[] textureColors = new Color[stateTexture.Width * stateTexture.Height];
			stateTexture.GetData(textureColors);

			byte[] textureBytes = GeneratePngBytesFromColors(textureColors, ApplyPaletteAnimation(palette));
			MaterialBuilder material = CreateTexturedMaterial(new MemoryImage(textureBytes), false);

			foreach (
				KeyValuePair<MeshType, List<PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty>>> texturedPrimitive
				in texturedPrimitives
			) {
				texturedPrimitive.Value.Add(meshes[texturedPrimitive.Key].UsePrimitive(material));
			}
		}

		return texturedPrimitives;
	}

	private static PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty> GetTexturedPrimitiveBuilder(
		Polygon polygon,
		Dictionary<MeshType, MeshBuilder<VertexPosition, VertexTexture1>> meshes,
		Dictionary<MeshType, List<PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty>>> texturedPrimitives,
		out List<Vector2> exportUvs
	) {
		if (GuiWindowExportGlb.BakeSceneLightingIntoTextures) {
			GlbBakedTextureData bakedTexture = GlbTextureBaker.BakePolygonTexture(polygon);
			exportUvs = ConvertUvCoordinates(bakedTexture.UvCoordinates);
			MaterialBuilder material = CreateTexturedMaterial(new MemoryImage(bakedTexture.TextureBytes), true);
			return meshes[polygon.MeshType].UsePrimitive(material);
		}

		exportUvs = GetAdjustedUvCoordinates(polygon.UvCoordinates, polygon.TexturePage);
		return texturedPrimitives[polygon.MeshType][polygon.PaletteId];
	}

	private static MaterialBuilder CreateTexturedMaterial(MemoryImage memoryImage, bool useUnlitShader) {
		MaterialBuilder material = new MaterialBuilder().WithAlpha(AlphaMode.MASK);

		if (useUnlitShader) {
			material.WithUnlitShader();
		} else {
			material.WithMetallicRoughnessShader();
			material.WithMetallicRoughness(0, 1);
			material.WithSpecularFactor(memoryImage, 0);
			material.WithSpecularColor(memoryImage, Vector3.Zero);
		}

		material.WithChannelImage(KnownChannel.BaseColor, memoryImage);
		material.UseChannel(KnownChannel.BaseColor)
			.Texture
			.WithSampler(
				TextureWrapMode.CLAMP_TO_EDGE,
				TextureWrapMode.MIRRORED_REPEAT,
				TextureMipMapFilter.NEAREST_MIPMAP_NEAREST,
				TextureInterpolationFilter.NEAREST
			);

		return material;
	}

	private static Palette ApplyPaletteAnimation(Palette palette) {
		bool usesTextureAnimations = CurrentMapState.StateData.TextureAnimations != null &&
		                             CurrentMapState.StateData.TextureAnimations.Count > 0;

		if (usesTextureAnimations) {
			int paletteIndex = CurrentMapState.StateData.Palettes.IndexOf(palette);

			foreach (AnimatedTextureInstructions instruction in CurrentMapState.StateData.TextureAnimations) {
				if (instruction.TextureAnimationType == TextureAnimationType.PaletteAnimation) {
					PaletteAnimation paletteAnimation = (PaletteAnimation) instruction.Instructions;
					if (paletteIndex == paletteAnimation.OverriddenPaletteId) {
						palette = CurrentMapState.StateData.PaletteAnimationFrames[paletteAnimation.AnimationStartIndex];
						break;
					}
				}
			}
		}

		return palette;
	}

	private static byte[] GeneratePngBytesFromColors(Color[] textureColors, Palette palette) {
		MTex2D stateTexture = CurrentMapState.StateData.Texture;
		FreeImageBitmap indexedPng = new(stateTexture.Width, stateTexture.Height, PixelFormat.Format32bppArgb);

		for (int x = 0; x < stateTexture.Width; x++) {
			for (int y = 0; y < stateTexture.Height; y++) {
				Color textureColor = textureColors[x + (stateTexture.Height - y - 1) * stateTexture.Width];
				System.Drawing.Color convertedColor = System.Drawing.Color.FromArgb(textureColor.A, textureColor.R, textureColor.G, textureColor.B);
				indexedPng.SetPixel(x, y, convertedColor);
			}
		}

		indexedPng.ConvertColorDepth(FREE_IMAGE_COLOR_DEPTH.FICD_08_BPP);

		ApplyPalette(indexedPng, palette);

		MemoryStream stream = new();
		indexedPng.Save(stream, FREE_IMAGE_FORMAT.FIF_PNG, FREE_IMAGE_SAVE_FLAGS.PNG_Z_BEST_COMPRESSION);
		return stream.ToArray();
	}

	private static void ApplyPalette(FreeImageBitmap indexedPng, Palette palette) {
		List<int> transparentIndices = new();

		for (int indexedPngPaletteIndex = 0; indexedPngPaletteIndex < palette.Colors.Count; indexedPngPaletteIndex++) {
			RGBQUAD indexedPngPaletteColor = indexedPng.Palette.GetValue(indexedPngPaletteIndex);

			for (int grayPaletteIndex = 0; grayPaletteIndex < GreyPalette.Count; grayPaletteIndex++) {
				Color grayColor = GreyPalette[grayPaletteIndex];
				if (
					indexedPngPaletteColor.Color.R == grayColor.R &&
					indexedPngPaletteColor.Color.G == grayColor.G &&
					indexedPngPaletteColor.Color.B == grayColor.B
				) {
					indexedPngPaletteColor = new RGBQUAD(System.Drawing.Color.FromArgb(
						255,
						palette.Colors[grayPaletteIndex].ToColor(false).R,
						palette.Colors[grayPaletteIndex].ToColor(false).G,
						palette.Colors[grayPaletteIndex].ToColor(false).B
					));
					indexedPng.Palette.SetValue(indexedPngPaletteColor, indexedPngPaletteIndex);
					if (palette.Colors[grayPaletteIndex].ToColor(false).A == 0) {
						transparentIndices.Add(indexedPngPaletteIndex);
					}
				}
			}
		}

		byte[] transparencyTable = new byte[256];

		for (int i = 0; i < transparencyTable.Length; i++) {
			transparencyTable[i] = 255;
		}

		foreach (int transparentIndex in transparentIndices) {
			transparencyTable[transparentIndex] = 0;
		}

		indexedPng.TransparencyTable = transparencyTable;
	}

	private static Vector3 ConvertAndScaleVector3(Microsoft.Xna.Framework.Vector3 vector3) {
		return new Vector3(vector3.X * ReduceScaleFactorX, vector3.Y * ReduceScaleFactorY, vector3.Z * ReduceScaleFactorZ);
	}

	private static VertexPosition ConvertAndScaleVector3ToVertexPosition(Microsoft.Xna.Framework.Vector3 vector3) {
		return new VertexPosition(vector3.X * ReduceScaleFactorX, vector3.Y * ReduceScaleFactorY, vector3.Z * ReduceScaleFactorZ);
	}

	private static Microsoft.Xna.Framework.Vector3 TranslateVertexToAnimatedPosition(Vertex vertex, MeshType meshType) {
		Microsoft.Xna.Framework.Vector3 animatedPosition = vertex.Position;

		if (meshType == MeshType.PrimaryMesh) {
			return animatedPosition;
		}

		List<MeshType> animatedMeshTypes = new() {
			MeshType.AnimatedMesh1, MeshType.AnimatedMesh2, MeshType.AnimatedMesh3, MeshType.AnimatedMesh4,
			MeshType.AnimatedMesh5, MeshType.AnimatedMesh6, MeshType.AnimatedMesh7, MeshType.AnimatedMesh8
		};
		AnimatedMeshInstructionSet animatedMeshInstructionSet =
			CurrentMapState.StateData.MeshAnimationSet.MeshInstructionSets[animatedMeshTypes.IndexOf(meshType)];

		MeshAnimationKeyframe keyframe = CurrentMapState.StateData.MeshAnimationSet.Keyframes[animatedMeshInstructionSet.Instructions[0].FrameStateId - 1];

		Matrix rotationX = Matrix.CreateRotationX(MathHelper.ToRadians((float) keyframe.Rotation[0]));
		Matrix rotationY = Matrix.CreateRotationY(MathHelper.ToRadians((float) keyframe.Rotation[1]));
		Matrix rotationZ = Matrix.CreateRotationZ(MathHelper.ToRadians((float) keyframe.Rotation[2]));

		animatedPosition = Microsoft.Xna.Framework.Vector3.Transform(animatedPosition, rotationX * rotationY * rotationZ);

		Matrix scale = Matrix.CreateScale(
			(float) keyframe.Scale[0],
			(float) keyframe.Scale[1],
			(float) keyframe.Scale[2]
		);

		animatedPosition = Microsoft.Xna.Framework.Vector3.Transform(animatedPosition, scale);

		animatedPosition += new Microsoft.Xna.Framework.Vector3(
			-(float) keyframe.Position[0],
			(float) keyframe.Position[1],
			(float) keyframe.Position[2]
		);

		return animatedPosition;
	}

	private static List<Vector2> ConvertUvCoordinates(IReadOnlyList<Microsoft.Xna.Framework.Vector2> uvs) {
		List<Vector2> convertedUvs = new();
		foreach (Microsoft.Xna.Framework.Vector2 uv in uvs) {
			convertedUvs.Add(new Vector2(uv.X, uv.Y));
		}

		return convertedUvs;
	}

	private static List<Vector2> GetAdjustedUvCoordinates(IReadOnlyList<Microsoft.Xna.Framework.Vector2> uvs, int texturePage) {
		List<Vector2> adjustedUvs = new();
		foreach (Microsoft.Xna.Framework.Vector2 uv in uvs) {
			adjustedUvs.Add(GetAdjustedUvCoordinate(uv, texturePage));
		}

		return adjustedUvs;
	}

	private static Vector2 GetAdjustedUvCoordinate(Microsoft.Xna.Framework.Vector2 uv, int texturePage) {
		double adjustedX = uv.X / 256f;
		double adjustedY = (uv.Y + 256 * texturePage) / 1024f;
		return new Vector2((float) adjustedX, (float) adjustedY);
	}
}
