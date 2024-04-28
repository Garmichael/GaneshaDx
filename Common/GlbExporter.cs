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
using System.Linq;
using FreeImageAPI;
using GaneshaDx.Environment;
using GaneshaDx.Resources.ContentDataTypes.MeshAnimations;
using Microsoft.Xna.Framework;
using Vector4 = System.Numerics.Vector4;
using Vector3 = System.Numerics.Vector3;
using Vector2 = System.Numerics.Vector2;
using MTex2D = Microsoft.Xna.Framework.Graphics.Texture2D;
using GaneshaDx.Resources.ContentDataTypes.Palettes;
using GaneshaDx.Resources.ContentDataTypes.TextureAnimations;
using GaneshaDx.UserInterface.GuiForms;
using SharpGLTF.Scenes;
using AlphaMode = SharpGLTF.Materials.AlphaMode;
using Palette = GaneshaDx.Resources.ContentDataTypes.Palettes.Palette;
using VertexPosition = SharpGLTF.Geometry.VertexTypes.VertexPosition;

namespace GaneshaDx.Common {
	public static class GlbExporter {
		private const float ReduceScaleFactor = 50;

		private static readonly List<Color> GreyPalette = new List<Color> {
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
			Dictionary<MeshType, MeshBuilder<VertexPosition, VertexTexture1>> meshes = new Dictionary<MeshType, MeshBuilder<VertexPosition, VertexTexture1>> {
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

			Dictionary<MeshType, List<PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty>>> texturedPrimitives =
				new Dictionary<MeshType, List<PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty>>> {
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

			CreateTextures(texturedPrimitives, meshes);


			foreach (Polygon polygon in CurrentMapState.StateData.PolygonCollectionBucket) {
				if (polygon.IsQuad) {
					List<Vertex> vertices = polygon.Vertices;
					List<Microsoft.Xna.Framework.Vector2> uvs = polygon.UvCoordinates;

					List<Microsoft.Xna.Framework.Vector3> animatedVertices = new List<Microsoft.Xna.Framework.Vector3> {
						TranslateVertexToAnimatedPosition(vertices[0], polygon.MeshType),
						TranslateVertexToAnimatedPosition(vertices[1], polygon.MeshType),
						TranslateVertexToAnimatedPosition(vertices[2], polygon.MeshType),
						TranslateVertexToAnimatedPosition(vertices[3], polygon.MeshType),
					};

					if (polygon.IsTextured) {
						// 0,1,3,2 is FFT vertex order, reversing here to correct facing/back-culling.
						texturedPrimitives[polygon.MeshType][polygon.PaletteId].AddQuadrangle(
							(ConvertAndScaleVector3(animatedVertices[2]), GetAdjustedUvCoordinates(uvs[2], polygon.TexturePage)),
							(ConvertAndScaleVector3(animatedVertices[3]), GetAdjustedUvCoordinates(uvs[3], polygon.TexturePage)),
							(ConvertAndScaleVector3(animatedVertices[1]), GetAdjustedUvCoordinates(uvs[1], polygon.TexturePage)),
							(ConvertAndScaleVector3(animatedVertices[0]), GetAdjustedUvCoordinates(uvs[0], polygon.TexturePage))
						);
					} else {
						texturedPrimitives[polygon.MeshType].Last().AddQuadrangle(
							ConvertAndScaleVector3ToVertexPosition(animatedVertices[2]),
							ConvertAndScaleVector3ToVertexPosition(animatedVertices[3]),
							ConvertAndScaleVector3ToVertexPosition(animatedVertices[1]),
							ConvertAndScaleVector3ToVertexPosition(animatedVertices[0])
						);
					}
				} else {
					List<Vertex> vertices = polygon.Vertices;
					List<Microsoft.Xna.Framework.Vector2> uvs = polygon.UvCoordinates;

					List<Microsoft.Xna.Framework.Vector3> animatedVertices = new List<Microsoft.Xna.Framework.Vector3> {
						TranslateVertexToAnimatedPosition(vertices[0], polygon.MeshType),
						TranslateVertexToAnimatedPosition(vertices[1], polygon.MeshType),
						TranslateVertexToAnimatedPosition(vertices[2], polygon.MeshType)
					};

					if (polygon.IsTextured) {
						texturedPrimitives[polygon.MeshType][polygon.PaletteId].AddTriangle(
							(ConvertAndScaleVector3(animatedVertices[2]), GetAdjustedUvCoordinates(uvs[2], polygon.TexturePage)),
							(ConvertAndScaleVector3(animatedVertices[1]), GetAdjustedUvCoordinates(uvs[1], polygon.TexturePage)),
							(ConvertAndScaleVector3(animatedVertices[0]), GetAdjustedUvCoordinates(uvs[0], polygon.TexturePage))
						);
					} else {
						texturedPrimitives[polygon.MeshType].Last().AddTriangle(
							ConvertAndScaleVector3ToVertexPosition(animatedVertices[2]),
							ConvertAndScaleVector3ToVertexPosition(animatedVertices[1]),
							ConvertAndScaleVector3ToVertexPosition(animatedVertices[0])
						);
					}
				}
			}

			SceneBuilder scene = new SceneBuilder();
			scene.AddRigidMesh(meshes[MeshType.PrimaryMesh], Matrix4x4.Identity);
			scene.AddRigidMesh(meshes[MeshType.AnimatedMesh1], Matrix4x4.Identity);
			scene.AddRigidMesh(meshes[MeshType.AnimatedMesh2], Matrix4x4.Identity);
			scene.AddRigidMesh(meshes[MeshType.AnimatedMesh3], Matrix4x4.Identity);
			scene.AddRigidMesh(meshes[MeshType.AnimatedMesh4], Matrix4x4.Identity);
			scene.AddRigidMesh(meshes[MeshType.AnimatedMesh5], Matrix4x4.Identity);
			scene.AddRigidMesh(meshes[MeshType.AnimatedMesh6], Matrix4x4.Identity);
			scene.AddRigidMesh(meshes[MeshType.AnimatedMesh7], Matrix4x4.Identity);
			scene.AddRigidMesh(meshes[MeshType.AnimatedMesh8], Matrix4x4.Identity);

			// @todo: export lights

			ModelRoot model = scene.ToGltf2();
			WriteSettings settings = new WriteSettings();
			settings.ImageWriting = ResourceWriteMode.EmbeddedAsBase64;
			settings.MergeBuffers = true;
			JsonWriterOptions jsonWriterOptions = new JsonWriterOptions();
			jsonWriterOptions.Indented = true;
			settings.JsonOptions = jsonWriterOptions;
			model.SaveGLB(filePath, settings);
		}

		private static void CreateTextures(
			Dictionary<MeshType, List<PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty>>> texturedPrimitives,
			Dictionary<MeshType, MeshBuilder<VertexPosition, VertexTexture1>> texturedMesh
		) {
			MTex2D stateTexture = CurrentMapState.StateData.Texture;

			foreach (Palette palette in CurrentMapState.StateData.Palettes) {
				Color[] textureColors = new Color[stateTexture.Width * stateTexture.Height];
				stateTexture.GetData(textureColors);

				byte[] textureBytes = GeneratePngBytesFromColors(textureColors, ApplyPaletteAnimation(palette));

				MemoryImage memoryImage = new MemoryImage(textureBytes);
				MaterialBuilder material = new MaterialBuilder().WithAlpha(AlphaMode.MASK);

				if (GuiWindowExportGlb.ExportUnlit) {
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

				foreach (
					KeyValuePair<MeshType, List<PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty>>> texturedPrimitive
					in texturedPrimitives
				) {
					texturedPrimitive.Value.Add(texturedMesh[texturedPrimitive.Key].UsePrimitive(material));
				}
			}

			MaterialBuilder blackMaterial = new MaterialBuilder();
			blackMaterial.WithBaseColor(new Vector4(0, 0, 0, 1));
			blackMaterial.WithUnlitShader();
			foreach (
				KeyValuePair<MeshType, List<PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty>>> texturedPrimitive
				in texturedPrimitives
			) {
				texturedPrimitive.Value.Add(texturedMesh[texturedPrimitive.Key].UsePrimitive(blackMaterial));
			}
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
			FreeImageBitmap indexedPng = new FreeImageBitmap(stateTexture.Width, stateTexture.Height, PixelFormat.Format32bppArgb);

			for (int x = 0; x < stateTexture.Width; x++) {
				for (int y = 0; y < stateTexture.Height; y++) {
					Color textureColor = textureColors[x + (stateTexture.Height - y - 1) * stateTexture.Width];
					System.Drawing.Color convertedColor = System.Drawing.Color.FromArgb(textureColor.A, textureColor.R, textureColor.G, textureColor.B);
					indexedPng.SetPixel(x, y, convertedColor);
				}
			}

			indexedPng.ConvertColorDepth(FREE_IMAGE_COLOR_DEPTH.FICD_08_BPP);

			ApplyPalette(indexedPng, palette);

			MemoryStream stream = new MemoryStream();
			indexedPng.Save(stream, FREE_IMAGE_FORMAT.FIF_PNG, FREE_IMAGE_SAVE_FLAGS.PNG_Z_BEST_COMPRESSION);
			return stream.ToArray();
		}

		private static void ApplyPalette(FreeImageBitmap indexedPng, Palette palette) {
			List<int> transparentIndices = new List<int>();

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
			return new Vector3(vector3.X / ReduceScaleFactor, vector3.Y / ReduceScaleFactor, vector3.Z / ReduceScaleFactor);
		}

		private static VertexPosition ConvertAndScaleVector3ToVertexPosition(Microsoft.Xna.Framework.Vector3 vector3) {
			return new VertexPosition(vector3.X / ReduceScaleFactor, vector3.Y / ReduceScaleFactor, vector3.Z / ReduceScaleFactor);
		}

		private static Microsoft.Xna.Framework.Vector3 TranslateVertexToAnimatedPosition(Vertex vertex, MeshType meshType) {
			Microsoft.Xna.Framework.Vector3 animatedPosition = vertex.Position;

			if (meshType == MeshType.PrimaryMesh) {
				return animatedPosition;
			}

			List<MeshType> animatedMeshTypes = new List<MeshType> {
				MeshType.AnimatedMesh1, MeshType.AnimatedMesh2, MeshType.AnimatedMesh3, MeshType.AnimatedMesh4,
				MeshType.AnimatedMesh5, MeshType.AnimatedMesh6, MeshType.AnimatedMesh7, MeshType.AnimatedMesh8
			};
			AnimatedMeshInstructionSet animatedMeshInstructionSet = CurrentMapState.StateData.MeshAnimationSet.MeshInstructionSets[animatedMeshTypes.IndexOf(meshType)];

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

		private static Vector2 GetAdjustedUvCoordinates(Microsoft.Xna.Framework.Vector2 uv, int texturePage) {
			double adjustedX = uv.X / 256f;
			double adjustedY = (uv.Y + 256 * texturePage) / 1024f;
			return new Vector2((float) adjustedX, (float) adjustedY);
		}
	}
}