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
using GaneshaDx.Environment;
using Microsoft.Xna.Framework;
using Vector4 = System.Numerics.Vector4;
using Vector3 = System.Numerics.Vector3;
using Vector2 = System.Numerics.Vector2;
using MTex2D = Microsoft.Xna.Framework.Graphics.Texture2D;
using GaneshaDx.Resources.ContentDataTypes.Palettes;
using GaneshaDx.Resources.ContentDataTypes.TextureAnimations;
using SharpGLTF.Scenes;

namespace GaneshaDx.Common
{
  public static class GltfExport {

		private const float _reduceScaleFactor = 50;

		private static List<Color> _greyPalette = new List<Color>();
		public static List<Color> GreyPalette {
			get {
				if (_greyPalette.Count > 0) {
					return _greyPalette;
				}

				_greyPalette = new List<Color>() {
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

				return _greyPalette;
			}
		}

		public static void Export(string filePath) {
			MeshBuilder<VertexPosition, VertexTexture1> texturedMesh = new MeshBuilder<VertexPosition, VertexTexture1>("TexturedMesh");
			List<PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty>> texturedPrimitives = new List<PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty>>();
			CreateTextures(texturedPrimitives, texturedMesh);

			MeshBuilder<VertexPosition> blackMesh = new MeshBuilder<VertexPosition>("BlackMesh");
			MaterialBuilder blackMaterial = new MaterialBuilder();
			blackMaterial.WithBaseColor(new Vector4(0,0,0,1));
			PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexEmpty, VertexEmpty> blackPrimitive = blackMesh.UsePrimitive(blackMaterial);

			foreach (Polygon polygon in CurrentMapState.StateData.PolygonCollectionBucket) {
				if (polygon.IsQuad) {
					List<Vertex> vertices = polygon.Vertices;
					List<Microsoft.Xna.Framework.Vector2> uvs = polygon.UvCoordinates;

					if (polygon.IsTextured) {
						// 0,1,3,2 is FFT vertex order, reversing here to correct facing/back-culling.
						texturedPrimitives[polygon.PaletteId].AddQuadrangle(
							(ConvertAndScaleVector3(vertices[2].Position), GetAdjustedUvCoordinates(uvs[2], polygon.TexturePage)),
							(ConvertAndScaleVector3(vertices[3].Position), GetAdjustedUvCoordinates(uvs[3], polygon.TexturePage)),
							(ConvertAndScaleVector3(vertices[1].Position), GetAdjustedUvCoordinates(uvs[1], polygon.TexturePage)),
							(ConvertAndScaleVector3(vertices[0].Position), GetAdjustedUvCoordinates(uvs[0], polygon.TexturePage))
						);
					} else {
						blackPrimitive.AddQuadrangle(
							ConvertAndScaleVector3ToVertexPosition(vertices[2].Position), 
							ConvertAndScaleVector3ToVertexPosition(vertices[3].Position), 
							ConvertAndScaleVector3ToVertexPosition(vertices[1].Position), 
							ConvertAndScaleVector3ToVertexPosition(vertices[0].Position)
						);
					}
				} else {
					List<Vertex> verts = polygon.Vertices;
					List<Microsoft.Xna.Framework.Vector2> uvs = polygon.UvCoordinates;
					if (polygon.IsTextured) {
						texturedPrimitives[polygon.PaletteId].AddTriangle(
							(ConvertAndScaleVector3(verts[2].Position), GetAdjustedUvCoordinates(uvs[2], polygon.TexturePage)),
							(ConvertAndScaleVector3(verts[1].Position), GetAdjustedUvCoordinates(uvs[1], polygon.TexturePage)),
							(ConvertAndScaleVector3(verts[0].Position), GetAdjustedUvCoordinates(uvs[0], polygon.TexturePage))
						);
					} else {
						blackPrimitive.AddTriangle(
							ConvertAndScaleVector3ToVertexPosition(verts[2].Position), 
							ConvertAndScaleVector3ToVertexPosition(verts[1].Position), 
							ConvertAndScaleVector3ToVertexPosition(verts[0].Position)
						);
					}
				}
			}

			SceneBuilder scene = new SceneBuilder();
			scene.AddRigidMesh(texturedMesh, Matrix4x4.Identity);
			scene.AddRigidMesh(blackMesh, Matrix4x4.Identity);

			// @todo: export lights

			ModelRoot model = scene.ToGltf2();
			WriteSettings settings = new WriteSettings();
			settings.ImageWriting = ResourceWriteMode.EmbeddedAsBase64;
			settings.MergeBuffers = true;
			JsonWriterOptions jsonWriterOptions = new JsonWriterOptions();
			jsonWriterOptions.Indented = true;
			settings.JsonOptions = jsonWriterOptions;
			model.SaveGLTF(filePath, settings);
		}

		private static Vector3 ConvertAndScaleVector3(Microsoft.Xna.Framework.Vector3 vector3) {
			return new Vector3(vector3.X / _reduceScaleFactor, vector3.Y / _reduceScaleFactor, vector3.Z / _reduceScaleFactor);
		}

		private static VertexPosition ConvertAndScaleVector3ToVertexPosition (Microsoft.Xna.Framework.Vector3 vector3) {
			return new VertexPosition(vector3.X / _reduceScaleFactor, vector3.Y / _reduceScaleFactor, vector3.Z / _reduceScaleFactor);
		}

		private static Vector2 GetAdjustedUvCoordinates (Microsoft.Xna.Framework.Vector2 uv, int texturePage) {
			double adjustedX = uv.X / 256f;
			double adjustedY = (uv.Y + 256 * texturePage) / 1024f;
			return new Vector2((float) adjustedX, (float) adjustedY);
		}

		private static void CreateTextures (List<PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty>> texturedPrimitives, MeshBuilder<VertexPosition, VertexTexture1> texturedMesh) {
			MTex2D stateTexture = CurrentMapState.StateData.Texture;

			foreach(Palette palette in CurrentMapState.StateData.Palettes) {
				Color[] textureColors = new Color[stateTexture.Width * stateTexture.Height];
				stateTexture.GetData<Color>(textureColors);

				ApplyPalette(textureColors, palette);

				byte[] textureBytes = GeneratePngBytesFromColors(textureColors);

				MaterialBuilder material = new MaterialBuilder();
				// @todo: Tweak the material so that it exports as close to possible as game-rendered version.
				//material.WithMetallicRoughness(0, 0);
				//material.WithUnlitShader(); // this looks close but material doesn't get lighting at all.

				MemoryImage memoryImage = new MemoryImage(textureBytes);

				// @todo: Why do specular options always crash on export?
				//material.WithSpecularGlossinessShader()
				//	.WithDiffuse(memoryImage, new Vector4(0))
				//	.WithSpecularGlossiness(memoryImage, new Vector3(0), 0f);

				material.WithChannelImage(KnownChannel.BaseColor, memoryImage);
				material.GetChannel(KnownChannel.BaseColor)
                .Texture
                .WithSampler(
									TextureWrapMode.CLAMP_TO_EDGE, 
									TextureWrapMode.MIRRORED_REPEAT, 
									TextureMipMapFilter.NEAREST_MIPMAP_NEAREST, 
									TextureInterpolationFilter.NEAREST
								);

				texturedPrimitives.Add(texturedMesh.UsePrimitive(material));
			}
		}

		private static void ApplyPalette(Color[] textureColors, Palette palette) {
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

			for (int colorIndex = 0; colorIndex < textureColors.Length; colorIndex++) {
				Color pixelColor = textureColors[colorIndex];
				if (GreyPalette.Contains(pixelColor)) {
					int index = GreyPalette.IndexOf(pixelColor);
					PaletteColor paletteColor = palette.Colors[index];
					textureColors[colorIndex] = paletteColor.ToColor();
				}
			}
		}

		private static byte[] GeneratePngBytesFromColors(Color[] textureColors) {
			MTex2D stateTexture = CurrentMapState.StateData.Texture;
			MTex2D texture = new MTex2D(Stage.Ganesha.GraphicsDevice, stateTexture.Width, stateTexture.Height);
			texture.SetData<Color>(textureColors);
			MemoryStream stream = new MemoryStream();
			texture.SaveAsPng(stream, stateTexture.Width, stateTexture.Height);
			return stream.ToArray();
		}
	}
}