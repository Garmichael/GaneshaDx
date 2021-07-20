using System;
using System.Collections.Generic;
using GaneshaDx.Common;
using GaneshaDx.Resources.ContentDataTypes;
using GaneshaDx.Resources.ContentDataTypes.MeshAnimations;
using GaneshaDx.Resources.ContentDataTypes.Palettes;
using GaneshaDx.Resources.ContentDataTypes.Polygons;
using GaneshaDx.Resources.ContentDataTypes.Terrains;
using GaneshaDx.Resources.ContentDataTypes.TextureAnimations;
using Microsoft.Xna.Framework;

namespace GaneshaDx.Resources.ResourceContent {
	public class MeshResourceData : ResourceData {
		private const int PrimaryMeshPointer = 64;
		private const int TexturePalettePointer = 68;
		private const int LightingAndBackgroundPointer = 100;
		private const int TerrainPointer = 104;
		private const int TextureAnimationsPointer = 108;
		private const int PaletteAnimationsPointer = 112;
		private const int TexturePalettesGrayscalePointer = 124;
		private const int AnimatedMeshInstructionsPointer = 140;
		private const int AnimatedMesh1Pointer = 144;
		private const int AnimatedMesh2Pointer = 148;
		private const int AnimatedMesh3Pointer = 152;
		private const int AnimatedMesh4Pointer = 156;
		private const int AnimatedMesh5Pointer = 160;
		private const int AnimatedMesh6Pointer = 164;
		private const int AnimatedMesh7Pointer = 168;
		private const int AnimatedMesh8Pointer = 172;
		private const int PolygonRenderPropertiesPointer = 176;

		private int _currentByteIndex;

		public bool HasPrimaryMesh;

		public bool HasAnimatedMeshInstructions;
		public bool HasAnimatedMesh1;
		public bool HasAnimatedMesh2;
		public bool HasAnimatedMesh3;
		public bool HasAnimatedMesh4;
		public bool HasAnimatedMesh5;
		public bool HasAnimatedMesh6;
		public bool HasAnimatedMesh7;
		public bool HasAnimatedMesh8;

		public bool HasPalettes;
		public bool HasLightsAndBackground;
		public bool HasTextureAnimations;
		public bool HasTerrain;
		public bool HasPaletteAnimationFrames;
		public bool HasPolygonRenderProperties;

		public readonly List<byte> RawData;

		private readonly Dictionary<MeshType, int> _texturedTriangleCount = new Dictionary<MeshType, int>();
		private readonly Dictionary<MeshType, int> _texturedQuadCount = new Dictionary<MeshType, int>();
		private readonly Dictionary<MeshType, int> _unTexturedTriangleCount = new Dictionary<MeshType, int>();
		private readonly Dictionary<MeshType, int> _unTexturedQuadCount = new Dictionary<MeshType, int>();

		public readonly Dictionary<MeshType, Dictionary<PolygonType, List<Polygon>>> PolygonCollection =
			new Dictionary<MeshType, Dictionary<PolygonType, List<Polygon>>>();

		public List<DirectionalLight> DirectionalLights;
		public Color BackgroundTopColor;
		public Color BackgroundBottomColor;
		public Color AmbientLightColor;

		private readonly Dictionary<MeshType, List<byte>> UnknownChunks = new Dictionary<MeshType, List<byte>> {
			{MeshType.PrimaryMesh, new List<byte>()},
			{MeshType.AnimatedMesh1, new List<byte>()},
			{MeshType.AnimatedMesh2, new List<byte>()},
			{MeshType.AnimatedMesh3, new List<byte>()},
			{MeshType.AnimatedMesh4, new List<byte>()},
			{MeshType.AnimatedMesh5, new List<byte>()},
			{MeshType.AnimatedMesh6, new List<byte>()},
			{MeshType.AnimatedMesh7, new List<byte>()},
			{MeshType.AnimatedMesh8, new List<byte>()}
		};

		public List<Palette> Palettes = new List<Palette>();
		public List<Palette> PaletteAnimationFrames = new List<Palette>();

		public List<AnimatedTextureInstructions> AnimatedTextureInstructions = new List<AnimatedTextureInstructions>();

		public Terrain Terrain;

		public MeshResourceData(List<byte> rawData) {
			RawData = rawData;

			SetUpPolyContainers();

			ProcessMeshes();

			ProcessTexturePalettes();
			ProcessLightingAndBackground();
			ProcessTerrain();
			ProcessTextureAnimations();
			ProcessPaletteAnimationFrames();
			ProcessMeshAnimationInstructions();
			ProcessPolygonRenderProperties();
		}

		public void SetUpPolyContainers() {
			PolygonCollection.Clear();

			foreach (MeshType meshType in CommonLists.MeshTypes) {
				PolygonCollection.Add(meshType, new Dictionary<PolygonType, List<Polygon>>());

				foreach (PolygonType polygonType in CommonLists.PolygonTypes) {
					PolygonCollection[meshType].Add(polygonType, new List<Polygon>());
				}
			}
		}

		private void ProcessMeshes() {
			List<int> pointers = new List<int> {
				PrimaryMeshPointer,
				AnimatedMesh1Pointer,
				AnimatedMesh2Pointer,
				AnimatedMesh3Pointer,
				AnimatedMesh4Pointer,
				AnimatedMesh5Pointer,
				AnimatedMesh6Pointer,
				AnimatedMesh7Pointer,
				AnimatedMesh8Pointer
			};

			for (int meshIndex = 0; meshIndex < pointers.Count; meshIndex++) {
				int pointer = pointers[meshIndex];
				MeshType meshType = CommonLists.MeshTypes[meshIndex];

				_currentByteIndex = Utilities.GetUIntFromLittleEndian(
					RawData[pointer],
					RawData[pointer + 1]
				);

				if (_currentByteIndex == 0) {
					return;
				}

				switch (meshType) {
					case MeshType.PrimaryMesh:
						HasPrimaryMesh = true;
						break;
					case MeshType.AnimatedMesh1:
						HasAnimatedMesh1 = true;
						break;
					case MeshType.AnimatedMesh2:
						HasAnimatedMesh2 = true;
						break;
					case MeshType.AnimatedMesh3:
						HasAnimatedMesh3 = true;
						break;
					case MeshType.AnimatedMesh4:
						HasAnimatedMesh4 = true;
						break;
					case MeshType.AnimatedMesh5:
						HasAnimatedMesh5 = true;
						break;
					case MeshType.AnimatedMesh6:
						HasAnimatedMesh6 = true;
						break;
					case MeshType.AnimatedMesh7:
						HasAnimatedMesh7 = true;
						break;
					case MeshType.AnimatedMesh8:
						HasAnimatedMesh8 = true;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				ProcessMeshPolyCounts(meshType, pointer);
				ProcessMeshBuildPolygons(meshType);
				ProcessMeshPositionData(meshType, pointer);
				ProcessMeshNormalData(meshType);
				ProcessMeshTextureData(meshType);
				ProcessMeshUnknownChunk(meshType);
				ProcessTerrainBinding(meshType);
			}
		}

		private void ProcessMeshPolyCounts(MeshType meshType, int pointer) {
			_texturedTriangleCount.Remove(meshType);
			_texturedQuadCount.Remove(meshType);
			_unTexturedTriangleCount.Remove(meshType);
			_unTexturedQuadCount.Remove(meshType);

			_currentByteIndex = Utilities.GetUIntFromLittleEndian(
				RawData[pointer],
				RawData[pointer + 1]
			);

			if (_currentByteIndex == 0) {
				return;
			}

			_texturedTriangleCount.Add(meshType, Utilities.GetUIntFromLittleEndian(
				RawData[_currentByteIndex],
				RawData[_currentByteIndex + 1]
			));


			_texturedQuadCount.Add(meshType, Utilities.GetUIntFromLittleEndian(
				RawData[_currentByteIndex + 2],
				RawData[_currentByteIndex + 3]
			));

			_unTexturedTriangleCount.Add(meshType, Utilities.GetUIntFromLittleEndian(
				RawData[_currentByteIndex + 4],
				RawData[_currentByteIndex + 5]
			));


			_unTexturedQuadCount.Add(meshType, Utilities.GetUIntFromLittleEndian(
				RawData[_currentByteIndex + 6],
				RawData[_currentByteIndex + 7]
			));
		}

		private void ProcessMeshBuildPolygons(MeshType meshType) {
			for (int i = 0; i < _texturedTriangleCount[meshType]; i++) {
				Polygon polygon = new Polygon {
					PolygonType = PolygonType.TexturedTriangle,
					MeshType = meshType
				};
				PolygonCollection[meshType][PolygonType.TexturedTriangle].Add(polygon);
			}

			for (int i = 0; i < _unTexturedTriangleCount[meshType]; i++) {
				Polygon polygon = new Polygon {
					PolygonType = PolygonType.UntexturedTriangle,
					MeshType = meshType
				};
				PolygonCollection[meshType][PolygonType.UntexturedTriangle].Add(polygon);
			}

			for (int i = 0; i < _texturedQuadCount[meshType]; i++) {
				Polygon polygon = new Polygon {
					PolygonType = PolygonType.TexturedQuad,
					MeshType = meshType
				};
				PolygonCollection[meshType][PolygonType.TexturedQuad].Add(polygon);
			}

			for (int i = 0; i < _unTexturedQuadCount[meshType]; i++) {
				Polygon polygon = new Polygon {
					PolygonType = PolygonType.UntexturedQuad,
					MeshType = meshType
				};
				PolygonCollection[meshType][PolygonType.UntexturedQuad].Add(polygon);
			}
		}

		private void ProcessMeshPositionData(MeshType meshType, int pointer) {
			_currentByteIndex = Utilities.GetUIntFromLittleEndian(
				RawData[pointer],
				RawData[pointer + 1]
			);

			_currentByteIndex += 8;

			ProcessMeshPositionDataPerPoly(
				_texturedTriangleCount[meshType],
				PolygonCollection[meshType][PolygonType.TexturedTriangle],
				3
			);

			ProcessMeshPositionDataPerPoly(
				_texturedQuadCount[meshType],
				PolygonCollection[meshType][PolygonType.TexturedQuad],
				4
			);

			ProcessMeshPositionDataPerPoly(
				_unTexturedTriangleCount[meshType],
				PolygonCollection[meshType][PolygonType.UntexturedTriangle],
				3
			);

			ProcessMeshPositionDataPerPoly(
				_unTexturedQuadCount[meshType],
				PolygonCollection[meshType][PolygonType.UntexturedQuad],
				4
			);
		}

		private void ProcessMeshPositionDataPerPoly(int totalCount, List<Polygon> polyContainer, int totalVerts) {
			List<Color> vertexColors = new List<Color> {Color.Red, Color.Green, Color.Blue, Color.Yellow};

			for (int index = 0; index < totalCount; index++) {
				polyContainer[index].Vertices = new List<Vertex>();

				for (int vertexCount = 0; vertexCount < totalVerts; vertexCount++) {
					Vector3 coordinates = new Vector3 {
						X = -Utilities.GetIntFromLittleEndian(
							RawData[_currentByteIndex],
							RawData[_currentByteIndex + 1]
						),
						Y = -Utilities.GetIntFromLittleEndian(
							RawData[_currentByteIndex + 2],
							RawData[_currentByteIndex + 3]
						),
						Z = Utilities.GetIntFromLittleEndian(
							RawData[_currentByteIndex + 4],
							RawData[_currentByteIndex + 5]
						)
					};

					polyContainer[index].Vertices.Add(new Vertex(coordinates, vertexColors[vertexCount]));

					_currentByteIndex += 6;
				}
			}
		}

		private void ProcessMeshNormalData(MeshType meshType) {
			ProcessMeshNormalDataPerPoly(
				_texturedTriangleCount[meshType],
				PolygonCollection[meshType][PolygonType.TexturedTriangle],
				3
			);

			ProcessMeshNormalDataPerPoly(
				_texturedQuadCount[meshType],
				PolygonCollection[meshType][PolygonType.TexturedQuad],
				4
			);
		}

		private void ProcessMeshNormalDataPerPoly(int totalCount, List<Polygon> polyContainer, int totalVerts) {
			for (int index = 0; index < totalCount; index++) {
				for (int vertexCount = 0; vertexCount < totalVerts; vertexCount++) {
					Vector3 normals = new Vector3 {
						X = -Utilities.GetIntFromLittleEndian(
							RawData[_currentByteIndex],
							RawData[_currentByteIndex + 1]
						) / 4096.0f,
						Y = -Utilities.GetIntFromLittleEndian(
							RawData[_currentByteIndex + 2],
							RawData[_currentByteIndex + 3]
						) / 4096.0f,
						Z = Utilities.GetIntFromLittleEndian(
							RawData[_currentByteIndex + 4],
							RawData[_currentByteIndex + 5]
						) / 4096.0f
					};

					(double elevation, double azimuth) = Utilities.VectorToSphere(normals);

					polyContainer[index].Vertices[vertexCount].UsesNormal = true;
					polyContainer[index].Vertices[vertexCount].NormalElevation = (float) elevation;
					polyContainer[index].Vertices[vertexCount].NormalAzimuth = (float) azimuth;

					_currentByteIndex += 6;
				}
			}
		}

		private void ProcessMeshTextureData(MeshType meshType) {
			ProcessMeshTextureDataPerPoly(
				_texturedTriangleCount[meshType],
				PolygonCollection[meshType][PolygonType.TexturedTriangle],
				3
			);

			ProcessMeshTextureDataPerPoly(
				_texturedQuadCount[meshType],
				PolygonCollection[meshType][PolygonType.TexturedQuad],
				4
			);
		}

		private void ProcessMeshTextureDataPerPoly(int totalCount, List<Polygon> polyContainer, int totalVerts) {
			for (int index = 0; index < totalCount; index++) {
				int vertexAu = _currentByteIndex + 0;
				int vertexAv = _currentByteIndex + 1;
				int paletteNumber = _currentByteIndex + 2;
				int vertexBu = _currentByteIndex + 4;
				int vertexBv = _currentByteIndex + 5;
				int texturePage = _currentByteIndex + 6;
				int vertexCu = _currentByteIndex + 8;
				int vertexCv = _currentByteIndex + 9;
				int vertexDu = _currentByteIndex + 10;
				int vertexDv = _currentByteIndex + 11;

				polyContainer[index].UvCoordinates = new List<Vector2> {
					new Vector2(RawData[vertexAu], RawData[vertexAv]),
					new Vector2(RawData[vertexBu], RawData[vertexBv]),
					new Vector2(RawData[vertexCu], RawData[vertexCv]),
				};

				int paletteId = RawData[paletteNumber];
				while (paletteId > 15) {
					paletteId -= 16;
				}

				polyContainer[index].PaletteId = paletteId;

				string texturePageBits = Utilities.GetBinaryFromInt(RawData[texturePage]);
				int texturePageId = Utilities.GetIntFromBinary(texturePageBits.Substring(texturePageBits.Length - 2));
				polyContainer[index].TexturePage = texturePageId;

				_currentByteIndex += 10;

				if (totalVerts == 4) {
					polyContainer[index].UvCoordinates.Add(new Vector2(RawData[vertexDu], RawData[vertexDv]));
					_currentByteIndex += 2;
				}
			}
		}

		private void ProcessMeshUnknownChunk(MeshType meshType) {
			int totalBytes = 4 * _unTexturedQuadCount[meshType] + 4 * _unTexturedTriangleCount[meshType];
			for (int index = 0; index < totalBytes; index++) {
				UnknownChunks[meshType].Add(RawData[_currentByteIndex]);
				_currentByteIndex++;
			}
		}

		private void ProcessTerrainBinding(MeshType meshType) {
			ProcessTerrainBindingPerPoly(
				_texturedTriangleCount[meshType],
				PolygonCollection[meshType][PolygonType.TexturedTriangle]
			);

			ProcessTerrainBindingPerPoly(
				_texturedQuadCount[meshType],
				PolygonCollection[meshType][PolygonType.TexturedQuad]
			);
		}

		private void ProcessTerrainBindingPerPoly(int totalCount, List<Polygon> polyContainer) {
			for (int index = 0; index < totalCount; index++) {
				string bits = Utilities.GetBinaryFromInt(RawData[_currentByteIndex]);
				polyContainer[index].TerrainZ = Utilities.GetIntFromBinary(bits.Substring(0, 7));
				polyContainer[index].TerrainX = RawData[_currentByteIndex + 1];
				polyContainer[index].TerrainLevel = Utilities.GetIntFromBinary(bits.Substring(bits.Length - 1));
				_currentByteIndex += 2;
			}
		}

		private void ProcessTexturePalettes() {
			_currentByteIndex = Utilities.GetUIntFromLittleEndian(
				RawData[TexturePalettePointer],
				RawData[TexturePalettePointer + 1]
			);

			if (_currentByteIndex == 0) {
				return;
			}

			HasPalettes = true;

			const int totalPalettes = 16;
			const int totalColors = 16;

			for (int paletteIndex = 0; paletteIndex < totalPalettes; paletteIndex++) {
				Palette palette = new Palette();

				for (int colorIndex = 0; colorIndex < totalColors; colorIndex++) {
					string allBits =
						Utilities.GetBinaryFromInt(RawData[_currentByteIndex + 1]) +
						Utilities.GetBinaryFromInt(RawData[_currentByteIndex]);

					bool isTransparent = Utilities.GetIntFromBinary(allBits.Substring(0, 1)) == 0;
					int blue = Utilities.GetIntFromBinary(allBits.Substring(1, 5));
					int green = Utilities.GetIntFromBinary(allBits.Substring(6, 5));
					int red = Utilities.GetIntFromBinary(allBits.Substring(11, 5));

					PaletteColor color = new PaletteColor(red, green, blue, isTransparent);

					palette.Colors.Add(color);
					_currentByteIndex += 2;
				}

				Palettes.Add(palette);
			}
		}

		private void ProcessLightingAndBackground() {
			_currentByteIndex = Utilities.GetUIntFromLittleEndian(
				RawData[LightingAndBackgroundPointer],
				RawData[LightingAndBackgroundPointer + 1]
			);

			if (_currentByteIndex == 0) {
				return;
			}

			HasLightsAndBackground = true;

			ProcessDirectionalLights();
			ProcessAmbientLight();
			ProcessBackgroundColors();
		}

		private void ProcessDirectionalLights() {
			DirectionalLights = new List<DirectionalLight> {
				new DirectionalLight(),
				new DirectionalLight(),
				new DirectionalLight()
			};

			DirectionalLights[0].LightColor = new Color(
				(int) (Utilities.Clamp(Utilities.GetIntFromLittleEndian(
						RawData[_currentByteIndex], RawData[_currentByteIndex + 1]), 0, 2040
				) / 8f),
				(int) (Utilities.Clamp(Utilities.GetIntFromLittleEndian(
						RawData[_currentByteIndex + 6], RawData[_currentByteIndex + 7]), 0, 2040
				) / 8f),
				(int) (Utilities.Clamp(Utilities.GetIntFromLittleEndian(
						RawData[_currentByteIndex + 12], RawData[_currentByteIndex + 13]), 0, 2040
				) / 8f),
				255
			);

			DirectionalLights[1].LightColor = new Color(
				(int) (Utilities.Clamp(Utilities.GetIntFromLittleEndian(
						RawData[_currentByteIndex + 2], RawData[_currentByteIndex + 3]), 0, 2040
				) / 8f),
				(int) (Utilities.Clamp(Utilities.GetIntFromLittleEndian(
						RawData[_currentByteIndex + 8], RawData[_currentByteIndex + 9]), 0, 2040
				) / 8f),
				(int) (Utilities.Clamp(Utilities.GetIntFromLittleEndian(
						RawData[_currentByteIndex + 14], RawData[_currentByteIndex + 15]), 0, 2040
				) / 8f),
				255
			);

			DirectionalLights[2].LightColor = new Color(
				(int) (Utilities.Clamp(Utilities.GetIntFromLittleEndian(
						RawData[_currentByteIndex + 4], RawData[_currentByteIndex + 5]), 0, 2040
				) / 8f),
				(int) (Utilities.Clamp(Utilities.GetIntFromLittleEndian(
					       RawData[_currentByteIndex + 10], RawData[_currentByteIndex + 11]), 0, 2040)
				       / 8f),
				(int) (Utilities.Clamp(Utilities.GetIntFromLittleEndian(
						RawData[_currentByteIndex + 16], RawData[_currentByteIndex + 17]), 0, 2040
				) / 8f),
				255
			);

			Vector3 direction = new Vector3(
				-Utilities.GetIntFromLittleEndian(RawData[_currentByteIndex + 18], RawData[_currentByteIndex + 19]),
				-Utilities.GetIntFromLittleEndian(RawData[_currentByteIndex + 20], RawData[_currentByteIndex + 21]),
				Utilities.GetIntFromLittleEndian(RawData[_currentByteIndex + 22], RawData[_currentByteIndex + 23])
			) / 4096.0f;

			direction.Normalize();

			(DirectionalLights[0].DirectionElevation, DirectionalLights[0].DirectionAzimuth) =
				Utilities.VectorToSphere(direction);

			direction = new Vector3(
				-Utilities.GetIntFromLittleEndian(RawData[_currentByteIndex + 24], RawData[_currentByteIndex + 25]),
				-Utilities.GetIntFromLittleEndian(RawData[_currentByteIndex + 26], RawData[_currentByteIndex + 27]),
				Utilities.GetIntFromLittleEndian(RawData[_currentByteIndex + 28], RawData[_currentByteIndex + 29])
			) / 4096.0f;

			direction.Normalize();

			(DirectionalLights[1].DirectionElevation, DirectionalLights[1].DirectionAzimuth) =
				Utilities.VectorToSphere(direction);

			direction = new Vector3(
				-Utilities.GetIntFromLittleEndian(RawData[_currentByteIndex + 30], RawData[_currentByteIndex + 31]),
				-Utilities.GetIntFromLittleEndian(RawData[_currentByteIndex + 32], RawData[_currentByteIndex + 33]),
				Utilities.GetIntFromLittleEndian(RawData[_currentByteIndex + 34], RawData[_currentByteIndex + 35])
			) / 4096.0f;

			direction.Normalize();

			(DirectionalLights[2].DirectionElevation, DirectionalLights[2].DirectionAzimuth) =
				Utilities.VectorToSphere(direction);

			_currentByteIndex += 36;
		}

		private void ProcessAmbientLight() {
			AmbientLightColor = new Color(
				RawData[_currentByteIndex],
				RawData[_currentByteIndex + 1],
				(int) RawData[_currentByteIndex + 2],
				255
			);

			_currentByteIndex += 3;
		}

		private void ProcessBackgroundColors() {
			int topR = RawData[_currentByteIndex];
			int topG = RawData[_currentByteIndex + 1];
			int topB = RawData[_currentByteIndex + 2];
			int bottomR = RawData[_currentByteIndex + 3];
			int bottomG = RawData[_currentByteIndex + 4];
			int bottomB = RawData[_currentByteIndex + 5];

			BackgroundTopColor = new Color(topR, topG, topB, 255);
			BackgroundBottomColor = new Color(bottomR, bottomG, bottomB, 255);
		}

		private void ProcessTerrain() {
			_currentByteIndex = Utilities.GetUIntFromLittleEndian(
				RawData[TerrainPointer],
				RawData[TerrainPointer + 1]
			);

			if (_currentByteIndex == 0) {
				return;
			}

			HasTerrain = true;

			int width = RawData[_currentByteIndex];
			int length = RawData[_currentByteIndex + 1];

			_currentByteIndex += 2;

			List<List<TerrainTile>> level0Tiles = new List<List<TerrainTile>>();
			List<List<TerrainTile>> level1Tiles = new List<List<TerrainTile>>();

			for (int terrainLevel = 0; terrainLevel <= 1; terrainLevel++) {
				for (int indexZ = 0; indexZ < length; indexZ++) {
					List<TerrainTile> terrainTiles = new List<TerrainTile>();

					for (int indexX = 0; indexX < width; indexX++) {
						string binary = Utilities.GetBinaryFromInt(RawData[_currentByteIndex]);
						int surfaceTypeId = Utilities.GetIntFromBinary(binary.Substring(2));
						TerrainSurfaceType surfaceType = CommonLists.TerrainSurfaceTypes[surfaceTypeId];

						int height = RawData[_currentByteIndex + 2];

						binary = Utilities.GetBinaryFromInt(RawData[_currentByteIndex + 3]);
						int depth = Utilities.GetIntFromBinary(binary.Substring(0, 3));
						int slopeHeight = Utilities.GetIntFromBinary(binary.Substring(3));


						TerrainSlopeType slopeType =
							CommonLists.TerrainSlopeTypes.ContainsKey(RawData[_currentByteIndex + 4])
								? CommonLists.TerrainSlopeTypes[RawData[_currentByteIndex + 4]]
								: TerrainSlopeType.Flat;

						binary = Utilities.GetBinaryFromInt(RawData[_currentByteIndex + 6]);
						bool impassable = Utilities.GetIntFromBinary(binary.Substring(6, 1)) == 1;
						bool unselectable = Utilities.GetIntFromBinary(binary.Substring(7, 1)) == 1;

						string rotationAngles = Utilities.GetBinaryFromInt(RawData[_currentByteIndex + 7]);
						bool rotatesNorthwestTop = rotationAngles.Substring(0, 1) == "1";
						bool rotatesSouthwestTop = rotationAngles.Substring(1, 1) == "1";
						bool rotatesSoutheastTop = rotationAngles.Substring(2, 1) == "1";
						bool rotatesNortheastTop = rotationAngles.Substring(3, 1) == "1";

						bool rotatesNorthwestBottom = rotationAngles.Substring(4, 1) == "1";
						bool rotatesSouthwestBottom = rotationAngles.Substring(5, 1) == "1";
						bool rotatesSoutheastBottom = rotationAngles.Substring(6, 1) == "1";
						bool rotatesNortheastBottom = rotationAngles.Substring(7, 1) == "1";

						TerrainTile terrainTile = new TerrainTile {
							SurfaceType = surfaceType,
							Height = height,
							Depth = depth,
							SlopeHeight = slopeHeight,
							SlopeType = slopeType,
							Impassable = impassable,
							Unselectable = unselectable,
							IndexZ = indexZ,
							IndexX = indexX,
							Level = terrainLevel,
							RotatesNorthwestTop = rotatesNorthwestTop,
							RotatesSouthwestTop = rotatesSouthwestTop,
							RotatesSoutheastTop = rotatesSoutheastTop,
							RotatesNortheastTop = rotatesNortheastTop,
							RotatesNorthwestBottom = rotatesNorthwestBottom,
							RotatesSouthwestBottom = rotatesSouthwestBottom,
							RotatesSoutheastBottom = rotatesSoutheastBottom,
							RotatesNortheastBottom = rotatesNortheastBottom,
						};

						terrainTiles.Add(terrainTile);

						_currentByteIndex += 8;
					}

					if (terrainLevel == 0) {
						level0Tiles.Add(terrainTiles);
					} else {
						level1Tiles.Add(terrainTiles);
					}
				}

				_currentByteIndex += 2048 - width * length * 8;
			}

			Terrain = new Terrain {
				SizeX = width,
				SizeZ = length,
				Level0Tiles = level0Tiles,
				Level1Tiles = level1Tiles
			};
		}

		private void ProcessTextureAnimations() {
			_currentByteIndex = Utilities.GetUIntFromLittleEndian(
				RawData[TextureAnimationsPointer],
				RawData[TextureAnimationsPointer + 1]
			);

			if (_currentByteIndex == 0) {
				return;
			}

			HasTextureAnimations = true;

			const int totalAnimations = 32;
			const int dataLength = 20;

			for (int animationIndex = 0; animationIndex < totalAnimations; animationIndex++) {
				List<byte> instructionRawData = new List<byte>(RawData.GetRange(_currentByteIndex, 20));
				AnimatedTextureInstructions.Add(new AnimatedTextureInstructions(instructionRawData));
				_currentByteIndex += dataLength;
			}
		}

		private void ProcessPaletteAnimationFrames() {
			_currentByteIndex = Utilities.GetUIntFromLittleEndian(
				RawData[PaletteAnimationsPointer],
				RawData[PaletteAnimationsPointer + 1]
			);

			if (_currentByteIndex == 0) {
				return;
			}

			HasPaletteAnimationFrames = true;

			const int totalPalettes = 16;
			const int totalColors = 16;

			for (int paletteIndex = 0; paletteIndex < totalPalettes; paletteIndex++) {
				Palette palette = new Palette();

				for (int colorIndex = 0; colorIndex < totalColors; colorIndex++) {
					string allBits =
						Utilities.GetBinaryFromInt(RawData[_currentByteIndex + 1]) +
						Utilities.GetBinaryFromInt(RawData[_currentByteIndex]);

					bool isTransparent = Utilities.GetIntFromBinary(allBits.Substring(0, 1)) == 0;
					int blue = Utilities.GetIntFromBinary(allBits.Substring(1, 5));
					int green = Utilities.GetIntFromBinary(allBits.Substring(6, 5));
					int red = Utilities.GetIntFromBinary(allBits.Substring(11, 5));

					PaletteColor color = new PaletteColor(red, green, blue, isTransparent);

					palette.Colors.Add(color);
					_currentByteIndex += 2;
				}

				PaletteAnimationFrames.Add(palette);
			}
		}

		private readonly List<byte> _rawMeshAnimationInstructionData = new List<byte>();

		public MeshAnimationInstructions MeshAnimationInstructions;

		private void ProcessMeshAnimationInstructions() {
			const int instructionChunkSize = 14620;

			_currentByteIndex = Utilities.GetUIntFromLittleEndian(
				RawData[AnimatedMeshInstructionsPointer],
				RawData[AnimatedMeshInstructionsPointer + 1]
			);

			if (_currentByteIndex == 0) {
				return;
			}

			HasAnimatedMeshInstructions = true;

			for (int i = 0; i < instructionChunkSize; i++) {
				if (_currentByteIndex + i > RawData.Count - 1) {
					break;
				}

				_rawMeshAnimationInstructionData.Add(RawData[_currentByteIndex + i]);
			}

			if (_rawMeshAnimationInstructionData.Count == instructionChunkSize) {
				MeshAnimationInstructions = new MeshAnimationInstructions(_rawMeshAnimationInstructionData);
			}
		}

		private void ProcessPolygonRenderProperties() {
			_currentByteIndex = Utilities.GetUIntFromLittleEndian(
				RawData[PolygonRenderPropertiesPointer],
				RawData[PolygonRenderPropertiesPointer + 1]
			);

			if (_currentByteIndex == 0) {
				return;
			}

			HasPolygonRenderProperties = true;

			const int unknownDataLength = 896;
			const int totalTexturedTriangles = 512;
			const int totalTexturedQuads = 768;
			const int totalUntexturedTriangles = 64;
			const int totalUntexturedQuads = 256;

			_currentByteIndex += unknownDataLength;

			List<Polygon> texturedTriangles = PolygonCollection[MeshType.PrimaryMesh][PolygonType.TexturedTriangle];
			List<Polygon> texturedQuads = PolygonCollection[MeshType.PrimaryMesh][PolygonType.TexturedQuad];
			List<Polygon> untexturedTriangles = PolygonCollection[MeshType.PrimaryMesh][PolygonType.UntexturedTriangle];
			List<Polygon> untexturedQuads = PolygonCollection[MeshType.PrimaryMesh][PolygonType.UntexturedQuad];

			for (int index = 0; index < totalTexturedTriangles; index++) {
				if (index < texturedTriangles.Count) {
					Polygon polygon = texturedTriangles[index];
					string binary = Utilities.GetBinaryFromInt(RawData[_currentByteIndex + 1]) +
					                Utilities.GetBinaryFromInt(RawData[_currentByteIndex]);

					polygon.RenderingProperties = new PolygonRenderingProperties(binary);
				}

				_currentByteIndex += 2;
			}

			for (int index = 0; index < totalTexturedQuads; index++) {
				if (index < texturedQuads.Count) {
					Polygon polygon = texturedQuads[index];
					string binary = Utilities.GetBinaryFromInt(RawData[_currentByteIndex + 1]) +
					                Utilities.GetBinaryFromInt(RawData[_currentByteIndex]);

					polygon.RenderingProperties = new PolygonRenderingProperties(binary);
				}

				_currentByteIndex += 2;
			}

			for (int index = 0; index < totalUntexturedTriangles; index++) {
				if (index < untexturedTriangles.Count) {
					Polygon polygon = untexturedTriangles[index];
					string binary = Utilities.GetBinaryFromInt(RawData[_currentByteIndex + 1]) +
					                Utilities.GetBinaryFromInt(RawData[_currentByteIndex]);

					polygon.RenderingProperties = new PolygonRenderingProperties(binary);
				}

				_currentByteIndex += 2;
			}

			for (int index = 0; index < totalUntexturedQuads; index++) {
				if (index < untexturedQuads.Count) {
					Polygon polygon = untexturedQuads[index];
					string binary = Utilities.GetBinaryFromInt(RawData[_currentByteIndex + 1]) +
					                Utilities.GetBinaryFromInt(RawData[_currentByteIndex]);

					polygon.RenderingProperties = new PolygonRenderingProperties(binary);
				}

				_currentByteIndex += 2;
			}
		}

		//Saving 
		public void RebuildRawData() {
			RawData.Clear();
			BuildRawDataHeader();
			BuildRawDataPrimaryMesh();
			BuildRawDataTexturePalettes();
			BuildRawDataLightsAndBackground();
			BuildRawDataTerrain();
			BuildRawDataTextureAnimations();
			BuildRawDataPaletteAnimationFrames();
			BuildRawDataGrayscalePalettes();
			BuildRawDataAnimatedMeshInstructions();
			BuildRawDataAnimatedMeshes();
			BuildRawDataRenderProperties();
		}

		private void BuildRawDataHeader() {
			for (int index = 0; index < 196; index++) {
				RawData.Add(0);
			}
		}

		private void BuildRawDataPrimaryMesh() {
			if (!HasPrimaryMesh) {
				return;
			}

			(RawData[PrimaryMeshPointer], RawData[PrimaryMeshPointer + 1]) =
				Utilities.GetLittleEndianFromInt(RawData.Count);

			BuildRawDataMeshHeader(MeshType.PrimaryMesh);
			BuildRawDataMeshPosition(MeshType.PrimaryMesh);
			BuildRawDataMeshNormals(MeshType.PrimaryMesh);
			BuildRawDataMeshTextureProperties(MeshType.PrimaryMesh);
			BuildRawDataMeshUnknownData(MeshType.PrimaryMesh);
			BuildRawDataMeshTerrainDefinitions(MeshType.PrimaryMesh);
		}

		private void BuildRawDataMeshHeader(MeshType meshType) {
			byte high;
			byte low;

			int count = PolygonCollection[meshType][PolygonType.TexturedTriangle].Count;
			(high, low) = Utilities.GetLittleEndianFromInt(count);

			RawData.Add(high);
			RawData.Add(low);

			count = PolygonCollection[meshType][PolygonType.TexturedQuad].Count;
			(high, low) = Utilities.GetLittleEndianFromInt(count);

			RawData.Add(high);
			RawData.Add(low);

			count = PolygonCollection[meshType][PolygonType.UntexturedTriangle].Count;
			(high, low) = Utilities.GetLittleEndianFromInt(count);

			RawData.Add(high);
			RawData.Add(low);

			count = PolygonCollection[meshType][PolygonType.UntexturedQuad].Count;
			(high, low) = Utilities.GetLittleEndianFromInt(count);

			RawData.Add(high);
			RawData.Add(low);
		}

		private void BuildRawDataMeshPosition(MeshType meshType) {
			foreach (Polygon polygon in PolygonCollection[meshType][PolygonType.TexturedTriangle]) {
				for (int vertexIndex = 0; vertexIndex < 3; vertexIndex++) {
					byte high;
					byte low;

					(high, low) = Utilities.GetLittleEndianFromInt((int) -polygon.Vertices[vertexIndex].Position.X);
					RawData.Add(high);
					RawData.Add(low);

					(high, low) = Utilities.GetLittleEndianFromInt((int) -polygon.Vertices[vertexIndex].Position.Y);
					RawData.Add(high);
					RawData.Add(low);

					(high, low) = Utilities.GetLittleEndianFromInt((int) polygon.Vertices[vertexIndex].Position.Z);
					RawData.Add(high);
					RawData.Add(low);
				}
			}

			foreach (Polygon polygon in PolygonCollection[meshType][PolygonType.TexturedQuad]) {
				for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++) {
					byte high;
					byte low;

					(high, low) = Utilities.GetLittleEndianFromInt((int) -polygon.Vertices[vertexIndex].Position.X);
					RawData.Add(high);
					RawData.Add(low);

					(high, low) = Utilities.GetLittleEndianFromInt((int) -polygon.Vertices[vertexIndex].Position.Y);
					RawData.Add(high);
					RawData.Add(low);

					(high, low) = Utilities.GetLittleEndianFromInt((int) polygon.Vertices[vertexIndex].Position.Z);
					RawData.Add(high);
					RawData.Add(low);
				}
			}

			foreach (Polygon polygon in PolygonCollection[meshType][PolygonType.UntexturedTriangle]) {
				for (int vertexIndex = 0; vertexIndex < 3; vertexIndex++) {
					byte high;
					byte low;

					(high, low) = Utilities.GetLittleEndianFromInt((int) -polygon.Vertices[vertexIndex].Position.X);
					RawData.Add(high);
					RawData.Add(low);

					(high, low) = Utilities.GetLittleEndianFromInt((int) -polygon.Vertices[vertexIndex].Position.Y);
					RawData.Add(high);
					RawData.Add(low);

					(high, low) = Utilities.GetLittleEndianFromInt((int) polygon.Vertices[vertexIndex].Position.Z);
					RawData.Add(high);
					RawData.Add(low);
				}
			}

			foreach (Polygon polygon in PolygonCollection[meshType][PolygonType.UntexturedQuad]) {
				for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++) {
					byte high;
					byte low;

					(high, low) = Utilities.GetLittleEndianFromInt((int) -polygon.Vertices[vertexIndex].Position.X);
					RawData.Add(high);
					RawData.Add(low);

					(high, low) = Utilities.GetLittleEndianFromInt((int) -polygon.Vertices[vertexIndex].Position.Y);
					RawData.Add(high);
					RawData.Add(low);

					(high, low) = Utilities.GetLittleEndianFromInt((int) polygon.Vertices[vertexIndex].Position.Z);
					RawData.Add(high);
					RawData.Add(low);
				}
			}
		}

		private void BuildRawDataMeshNormals(MeshType meshType) {
			foreach (Polygon polygon in PolygonCollection[meshType][PolygonType.TexturedTriangle]) {
				for (int vertexIndex = 0; vertexIndex < 3; vertexIndex++) {
					Vector3 normalData = Utilities.SphereToVector(
						polygon.Vertices[vertexIndex].NormalElevation,
						polygon.Vertices[vertexIndex].NormalAzimuth
					);

					byte high;
					byte low;

					(high, low) = Utilities.GetLittleEndianFromInt((int) -(normalData.X * 4096.0f));
					RawData.Add(high);
					RawData.Add(low);

					(high, low) = Utilities.GetLittleEndianFromInt((int) -(normalData.Y * 4096.0f));
					RawData.Add(high);
					RawData.Add(low);

					(high, low) = Utilities.GetLittleEndianFromInt((int) -(normalData.Z * 4096.0f));
					RawData.Add(high);
					RawData.Add(low);
				}
			}

			foreach (Polygon polygon in PolygonCollection[meshType][PolygonType.TexturedQuad]) {
				for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++) {
					Vector3 normalData = Utilities.SphereToVector(
						polygon.Vertices[vertexIndex].NormalElevation,
						polygon.Vertices[vertexIndex].NormalAzimuth
					);

					byte high;
					byte low;

					(high, low) = Utilities.GetLittleEndianFromInt((int) -(normalData.X * 4096.0f));
					RawData.Add(high);
					RawData.Add(low);

					(high, low) = Utilities.GetLittleEndianFromInt((int) -(normalData.Y * 4096.0f));
					RawData.Add(high);
					RawData.Add(low);

					(high, low) = Utilities.GetLittleEndianFromInt((int) -(normalData.Z * 4096.0f));
					RawData.Add(high);
					RawData.Add(low);
				}
			}
		}

		private void BuildRawDataMeshTextureProperties(MeshType meshType) {
			foreach (Polygon polygon in PolygonCollection[meshType][PolygonType.TexturedTriangle]) {
				RawData.Add((byte) polygon.UvCoordinates[0].X);
				RawData.Add((byte) polygon.UvCoordinates[0].Y);
				RawData.Add((byte) polygon.PaletteId);
				RawData.Add(120);
				RawData.Add((byte) polygon.UvCoordinates[1].X);
				RawData.Add((byte) polygon.UvCoordinates[1].Y);
				string binary = Utilities.GetBinaryFromInt(0, 4) +
				                Utilities.GetBinaryFromInt(3, 2) +
				                Utilities.GetBinaryFromInt(polygon.TexturePage, 2);
				RawData.Add((byte) Utilities.GetIntFromBinary(binary));
				RawData.Add(0);
				RawData.Add((byte) polygon.UvCoordinates[2].X);
				RawData.Add((byte) polygon.UvCoordinates[2].Y);
			}

			foreach (Polygon polygon in PolygonCollection[meshType][PolygonType.TexturedQuad]) {
				RawData.Add((byte) polygon.UvCoordinates[0].X);
				RawData.Add((byte) polygon.UvCoordinates[0].Y);
				RawData.Add((byte) polygon.PaletteId);
				RawData.Add(120);
				RawData.Add((byte) polygon.UvCoordinates[1].X);
				RawData.Add((byte) polygon.UvCoordinates[1].Y);
				string binary = Utilities.GetBinaryFromInt(0, 4) +
				                Utilities.GetBinaryFromInt(3, 2) +
				                Utilities.GetBinaryFromInt(polygon.TexturePage, 2);
				RawData.Add((byte) Utilities.GetIntFromBinary(binary));
				RawData.Add(0);
				RawData.Add((byte) polygon.UvCoordinates[2].X);
				RawData.Add((byte) polygon.UvCoordinates[2].Y);
				RawData.Add((byte) polygon.UvCoordinates[3].X);
				RawData.Add((byte) polygon.UvCoordinates[3].Y);
			}
		}

		private void BuildRawDataMeshUnknownData(MeshType meshType) {
			int totalBytes = PolygonCollection[meshType][PolygonType.UntexturedTriangle].Count * 4 +
			                 PolygonCollection[meshType][PolygonType.UntexturedQuad].Count * 4;

			for (int index = 0; index < totalBytes; index++) {
				RawData.Add(0);
			}
		}

		private void BuildRawDataMeshTerrainDefinitions(MeshType meshType) {
			foreach (Polygon polygon in PolygonCollection[meshType][PolygonType.TexturedTriangle]) {
				string binary = Utilities.GetBinaryFromInt(polygon.TerrainZ, 7) +
				                (polygon.TerrainLevel == 0 ? "0" : "1");

				RawData.Add((byte) Utilities.GetIntFromBinary(binary));
				RawData.Add((byte) polygon.TerrainX);
			}

			foreach (Polygon polygon in PolygonCollection[meshType][PolygonType.TexturedQuad]) {
				string binary = Utilities.GetBinaryFromInt(polygon.TerrainZ, 7) +
				                (polygon.TerrainLevel == 0 ? "0" : "1");


				RawData.Add((byte) Utilities.GetIntFromBinary(binary));
				RawData.Add((byte) polygon.TerrainX);
			}
		}

		private void BuildRawDataTexturePalettes() {
			if (!HasPalettes) {
				return;
			}

			(RawData[TexturePalettePointer], RawData[TexturePalettePointer + 1]) =
				Utilities.GetLittleEndianFromInt(RawData.Count);

			foreach (Palette palette in Palettes) {
				RawData.AddRange(palette.GetRawData());
			}
		}

		private void BuildRawDataLightsAndBackground() {
			if (!HasLightsAndBackground) {
				return;
			}

			(RawData[LightingAndBackgroundPointer], RawData[LightingAndBackgroundPointer + 1]) =
				Utilities.GetLittleEndianFromInt(RawData.Count);

			BuildRawDataDirectionalLights();
			BuildRawDataAmbientLight();
			BuildRawDataBackgroundColors();
		}

		private void BuildRawDataDirectionalLights() {
			byte high;
			byte low;

			(high, low) = Utilities.GetLittleEndianFromInt(DirectionalLights[0].LightColor.R * 8);
			RawData.Add(high);
			RawData.Add(low);

			(high, low) = Utilities.GetLittleEndianFromInt(DirectionalLights[1].LightColor.R * 8);
			RawData.Add(high);
			RawData.Add(low);

			(high, low) = Utilities.GetLittleEndianFromInt(DirectionalLights[2].LightColor.R * 8);
			RawData.Add(high);
			RawData.Add(low);

			(high, low) = Utilities.GetLittleEndianFromInt(DirectionalLights[0].LightColor.G * 8);
			RawData.Add(high);
			RawData.Add(low);

			(high, low) = Utilities.GetLittleEndianFromInt(DirectionalLights[1].LightColor.G * 8);
			RawData.Add(high);
			RawData.Add(low);

			(high, low) = Utilities.GetLittleEndianFromInt(DirectionalLights[2].LightColor.G * 8);
			RawData.Add(high);
			RawData.Add(low);

			(high, low) = Utilities.GetLittleEndianFromInt(DirectionalLights[0].LightColor.B * 8);
			RawData.Add(high);
			RawData.Add(low);

			(high, low) = Utilities.GetLittleEndianFromInt(DirectionalLights[1].LightColor.B * 8);
			RawData.Add(high);
			RawData.Add(low);

			(high, low) = Utilities.GetLittleEndianFromInt(DirectionalLights[2].LightColor.B * 8);
			RawData.Add(high);
			RawData.Add(low);

			for (int lightIndex = 0; lightIndex < 3; lightIndex++) {
				(float x, float y, float z) = Utilities.SphereToVector(
					DirectionalLights[lightIndex].DirectionElevation,
					DirectionalLights[lightIndex].DirectionAzimuth
				);

				(high, low) = Utilities.GetLittleEndianFromInt((int) Math.Floor(-x * 4096.0f));
				RawData.Add(high);
				RawData.Add(low);

				(high, low) = Utilities.GetLittleEndianFromInt((int) Math.Floor(-y * 4096.0f));
				RawData.Add(high);
				RawData.Add(low);

				(high, low) = Utilities.GetLittleEndianFromInt((int) Math.Floor(-z * 4096.0f));
				RawData.Add(high);
				RawData.Add(low);
			}
		}

		private void BuildRawDataAmbientLight() {
			RawData.Add(AmbientLightColor.R);
			RawData.Add(AmbientLightColor.G);
			RawData.Add(AmbientLightColor.B);
		}

		private void BuildRawDataBackgroundColors() {
			RawData.Add(BackgroundTopColor.R);
			RawData.Add(BackgroundTopColor.G);
			RawData.Add(BackgroundTopColor.B);
			RawData.Add(BackgroundBottomColor.R);
			RawData.Add(BackgroundBottomColor.G);
			RawData.Add(BackgroundBottomColor.B);
		}

		private void BuildRawDataTerrain() {
			if (!HasTerrain) {
				return;
			}

			(RawData[TerrainPointer], RawData[TerrainPointer + 1]) =
				Utilities.GetLittleEndianFromInt(RawData.Count);

			RawData.Add((byte) Terrain.SizeX);
			RawData.Add((byte) Terrain.SizeZ);
			RawData.AddRange(Terrain.GetRawData());
		}

		private void BuildRawDataTextureAnimations() {
			if (!HasTextureAnimations) {
				return;
			}

			(RawData[TextureAnimationsPointer], RawData[TextureAnimationsPointer + 1]) =
				Utilities.GetLittleEndianFromInt(RawData.Count);

			foreach (AnimatedTextureInstructions textureAnimation in AnimatedTextureInstructions) {
				RawData.AddRange(textureAnimation.GetRawData());
			}
		}

		private void BuildRawDataPaletteAnimationFrames() {
			if (!HasPaletteAnimationFrames) {
				return;
			}

			(RawData[PaletteAnimationsPointer], RawData[PaletteAnimationsPointer + 1]) =
				Utilities.GetLittleEndianFromInt(RawData.Count);

			foreach (Palette palette in PaletteAnimationFrames) {
				RawData.AddRange(palette.GetRawData());
			}
		}

		private void BuildRawDataGrayscalePalettes() {
			(RawData[TexturePalettesGrayscalePointer], RawData[TexturePalettesGrayscalePointer + 1]) =
				Utilities.GetLittleEndianFromInt(RawData.Count);

			foreach (Palette palette in Palettes) {
				foreach (PaletteColor color in palette.Colors) {
					string binary = "";

					int averageColor = (int) Math.Round((color.Red + color.Blue + color.Green) / 3f);

					binary += color.IsTransparent ? "0" : "1";
					binary += Utilities.GetBinaryFromInt(averageColor, 5);
					binary += Utilities.GetBinaryFromInt(averageColor, 5);
					binary += Utilities.GetBinaryFromInt(averageColor, 5);

					byte high = (byte) Utilities.GetIntFromBinary(binary.Substring(0, 8));
					byte low = (byte) Utilities.GetIntFromBinary(binary.Substring(8, 8));

					RawData.Add(low);
					RawData.Add(high);
				}
			}
		}

		private void BuildRawDataAnimatedMeshInstructions() {
			if (!HasAnimatedMeshInstructions) {
				return;
			}

			(RawData[AnimatedMeshInstructionsPointer], RawData[AnimatedMeshInstructionsPointer + 1]) =
				Utilities.GetLittleEndianFromInt(RawData.Count);

			RawData.AddRange(MeshAnimationInstructions.InstructionsHeader);

			foreach (MeshAnimationKeyFrame keyFrame in MeshAnimationInstructions.KeyFrames) {
				RawData.AddRange(keyFrame.GetRawData());
			}

			RawData.AddRange(MeshAnimationInstructions.MeshAnimationsHeader);

			foreach (MeshAnimation meshAnimation in MeshAnimationInstructions.MeshAnimations) {
				RawData.AddRange(meshAnimation.GetRawData());
			}

			RawData.AddRange(MeshAnimationInstructions.UnknownChunkHeader);
			RawData.AddRange(MeshAnimationInstructions.UnknownChunk.Data);
		}

		private void BuildRawDataAnimatedMeshes() {
			List<bool> hasMeshes = new List<bool> {
				HasAnimatedMesh1, HasAnimatedMesh2, HasAnimatedMesh3, HasAnimatedMesh4,
				HasAnimatedMesh5, HasAnimatedMesh6, HasAnimatedMesh7, HasAnimatedMesh8,
			};

			List<int> pointers = new List<int> {
				AnimatedMesh1Pointer, AnimatedMesh2Pointer, AnimatedMesh3Pointer, AnimatedMesh4Pointer,
				AnimatedMesh5Pointer, AnimatedMesh6Pointer, AnimatedMesh7Pointer, AnimatedMesh8Pointer
			};

			List<MeshType> meshTypes = new List<MeshType> {
				MeshType.AnimatedMesh1, MeshType.AnimatedMesh2, MeshType.AnimatedMesh3, MeshType.AnimatedMesh4,
				MeshType.AnimatedMesh5, MeshType.AnimatedMesh6, MeshType.AnimatedMesh7, MeshType.AnimatedMesh8
			};

			for (int index = 0; index < hasMeshes.Count; index++) {
				if (hasMeshes[index]) {
					(RawData[pointers[index]], RawData[pointers[index] + 1]) =
						Utilities.GetLittleEndianFromInt(RawData.Count);

					BuildRawDataMeshHeader(meshTypes[index]);
					BuildRawDataMeshPosition(meshTypes[index]);
					BuildRawDataMeshNormals(meshTypes[index]);
					BuildRawDataMeshTextureProperties(meshTypes[index]);
					BuildRawDataMeshUnknownData(meshTypes[index]);
					BuildRawDataMeshTerrainDefinitions(meshTypes[index]);
				}
			}
		}

		private void BuildRawDataRenderProperties() {
			if (!HasPrimaryMesh || !HasPolygonRenderProperties) {
				return;
			}

			(RawData[PolygonRenderPropertiesPointer], RawData[PolygonRenderPropertiesPointer + 1]) =
				Utilities.GetLittleEndianFromInt(RawData.Count);

			const int unknownDataLength = 896;
			const int totalTexturedTriangles = 512;
			const int totalTexturedQuads = 768;
			const int totalUntexturedTriangles = 64;
			const int totalUntexturedQuads = 256;

			for (int i = 0; i < unknownDataLength; i++) {
				RawData.Add(0);
			}

			foreach (Polygon polygon in PolygonCollection[MeshType.PrimaryMesh][PolygonType.TexturedTriangle]) {
				RawData.AddRange(polygon.RenderingProperties.GetRawData());
			}

			for (int i = 0;
				i < totalTexturedTriangles -
				PolygonCollection[MeshType.PrimaryMesh][PolygonType.TexturedTriangle].Count;
				i++
			) {
				RawData.Add(0);
				RawData.Add(128);
			}

			foreach (Polygon polygon in PolygonCollection[MeshType.PrimaryMesh][PolygonType.TexturedQuad]) {
				RawData.AddRange(polygon.RenderingProperties.GetRawData());
			}

			for (int i = 0;
				i < totalTexturedQuads -
				PolygonCollection[MeshType.PrimaryMesh][PolygonType.TexturedQuad].Count;
				i++
			) {
				RawData.Add(0);
				RawData.Add(128);
			}

			foreach (Polygon polygon in PolygonCollection[MeshType.PrimaryMesh][PolygonType.UntexturedTriangle]) {
				RawData.AddRange(polygon.RenderingProperties.GetRawData());
			}

			for (int i = 0;
				i < totalUntexturedTriangles -
				PolygonCollection[MeshType.PrimaryMesh][PolygonType.UntexturedTriangle].Count;
				i++
			) {
				RawData.Add(0);
				RawData.Add(128);
			}

			foreach (Polygon polygon in PolygonCollection[MeshType.PrimaryMesh][PolygonType.UntexturedQuad]) {
				RawData.AddRange(polygon.RenderingProperties.GetRawData());
			}

			for (int i = 0;
				i < totalUntexturedQuads -
				PolygonCollection[MeshType.PrimaryMesh][PolygonType.UntexturedQuad].Count;
				i++
			) {
				RawData.Add(0);
				RawData.Add(128);
			}
		}
	}
}