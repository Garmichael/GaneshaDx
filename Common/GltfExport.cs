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

namespace GaneshaDx.Common
{
  public static class GltfExport {

		/**
		* Exports the map model to the .gltf format.
		*/
		public static void Export(string filePath) {
			var stateTexture = CurrentMapState.StateData.Texture;
			
			var greyPalette = new List<Color>() {
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
			
			var texturedMesh = new MeshBuilder<VertexPosition, VertexTexture1>("texturedMesh");
			var texturedPrims = new List<PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexTexture1, VertexEmpty>>();

			foreach(var palette in CurrentMapState.StateData.Palettes) {
				var textureColors = new Color[stateTexture.Width * stateTexture.Height];
				stateTexture.GetData<Color>(textureColors);

				// For the current palette, replace greyscale pixels with palette color.
				for (int x = 0; x < textureColors.Length; x++) {
					var pixelColor = textureColors[x];
					if (greyPalette.Contains(pixelColor)) {
						var index = greyPalette.IndexOf(pixelColor);
						var paletteColor = palette.Colors[index];
						textureColors[x] = paletteColor.ToColor();
					}
				}

				// Convert raw Color data back to a Texture2D to be converted to PNG for embedding.
				var texture = new MTex2D(Stage.Ganesha.GraphicsDevice, stateTexture.Width, stateTexture.Height);
				texture.SetData<Color>(textureColors);
				var stream = new MemoryStream();
				texture.SaveAsPng(stream, stateTexture.Width, stateTexture.Height);
				var textureBytes = stream.ToArray();

				var material = new MaterialBuilder();
				// @todo: Tweak the material so that it exports as close to possible as game-rendered version.
				//material.WithMetallicRoughness(0, 0);
				//material.WithUnlitShader(); // this looks close but material doesn't get lighting at all.

				var memoryImage = new MemoryImage(textureBytes);

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

				texturedPrims.Add(texturedMesh.UsePrimitive(material));
			}

			var blackMesh = new MeshBuilder<VertexPosition>("blackMesh");
			var blackMat = new MaterialBuilder();
			blackMat.WithBaseColor(new Vector4(0,0,0,1));
			var blackPrim = blackMesh.UsePrimitive(blackMat);

			foreach (Polygon polygon in CurrentMapState.StateData.PolygonCollectionBucket) {
				if (polygon.IsQuad) {
					var verts = polygon.Vertices;
					var uvs = polygon.UvCoordinates;

					if (polygon.IsTextured) {
						// 0,1,3,2 is FFT vertex order, reversing here to correct facing/back-culling.
						texturedPrims[polygon.PaletteId].AddQuadrangle(
							(PositionToV3(verts[2].Position), UvToV2(uvs[2], polygon.TexturePage)),
							(PositionToV3(verts[3].Position), UvToV2(uvs[3], polygon.TexturePage)),
							(PositionToV3(verts[1].Position), UvToV2(uvs[1], polygon.TexturePage)),
							(PositionToV3(verts[0].Position), UvToV2(uvs[0], polygon.TexturePage))
						);
					} else {
						blackPrim.AddQuadrangle(
							PositionToVPos(verts[2].Position), 
							PositionToVPos(verts[3].Position), 
							PositionToVPos(verts[1].Position), 
							PositionToVPos(verts[0].Position)
						);
					}
				} else {
					var verts = polygon.Vertices;
					var uvs = polygon.UvCoordinates;
					if (polygon.IsTextured) {
						// 0, 1, 2
						texturedPrims[polygon.PaletteId].AddTriangle(
							(PositionToV3(verts[2].Position), UvToV2(uvs[2], polygon.TexturePage)),
							(PositionToV3(verts[1].Position), UvToV2(uvs[1], polygon.TexturePage)),
							(PositionToV3(verts[0].Position), UvToV2(uvs[0], polygon.TexturePage))
						);
					} else {
						blackPrim.AddTriangle(
							PositionToVPos(verts[2].Position), 
							PositionToVPos(verts[1].Position), 
							PositionToVPos(verts[0].Position)
						);
					}
				}
			}

			var scene = new SharpGLTF.Scenes.SceneBuilder();
			scene.AddRigidMesh(texturedMesh, Matrix4x4.Identity);
			scene.AddRigidMesh(blackMesh, Matrix4x4.Identity);

			// @todo: export lights

			var model = scene.ToGltf2();
			var settings = new WriteSettings();
			settings.ImageWriting = ResourceWriteMode.EmbeddedAsBase64;
			settings.MergeBuffers = true;
			var jsonWriterOptions = new JsonWriterOptions();
			jsonWriterOptions.Indented = true;
			settings.JsonOptions = jsonWriterOptions;
			model.SaveGLTF(filePath, settings);
		}

		/**	
		 * Convert XNA Vector3 to System.Numerics.Vector3.
		 */ 
		private static Vector3 PositionToV3 (Microsoft.Xna.Framework.Vector3 position) {
			return new Vector3(position.X, position.Y, position.Z);
		}

		/**	
		 * Convert XNA Vector3 to SharpGLTF VertexPosition.
		 */ 
		private static VertexPosition PositionToVPos (Microsoft.Xna.Framework.Vector3 position) {
			return new VertexPosition(position.X, position.Y, position.Z);
		}

		/**
		 * Convert XNA Vector2 to System.Numerics.Vector2 and adjust UV position based on the texture page.
		 */
		private static Vector2 UvToV2 (Microsoft.Xna.Framework.Vector2 uv, int texturePage) {
			double adjustedX = uv.X / 256f;
			double adjustedY = (uv.Y + 256 * texturePage) / 1024f;
			return new Vector2((float) adjustedX, (float) adjustedY);
		}
	}
}