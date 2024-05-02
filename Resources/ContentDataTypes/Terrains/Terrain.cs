using System.Collections.Generic;
using GaneshaDx.Common;

namespace GaneshaDx.Resources.ContentDataTypes.Terrains {
	public class Terrain {
		public int SizeX;
		public int SizeZ;

		public List<List<TerrainTile>> Level0Tiles = new List<List<TerrainTile>>();
		public List<List<TerrainTile>> Level1Tiles = new List<List<TerrainTile>>();

		public void Render() {
			foreach (List<TerrainTile> terrainTiles in Level0Tiles) {
				foreach (TerrainTile terrainTile in terrainTiles) {
					terrainTile.Render();
				}
			}

			foreach (List<TerrainTile> terrainTiles in Level1Tiles) {
				foreach (TerrainTile terrainTile in terrainTiles) {
					terrainTile.Render();
				}
			}
		}

		public void ResizeTerrain(int newSizeX, int newSizeZ, bool resizeFromFront) {
			if (newSizeX < SizeX) {
				foreach (List<TerrainTile> row in Level0Tiles) {
					while (row.Count > newSizeX) {
						if (resizeFromFront) {
							row.RemoveAt(0);
						} else {
							row.RemoveAt(row.Count - 1);
						}
					}
				}

				foreach (List<TerrainTile> row in Level1Tiles) {
					while (row.Count > newSizeX) {
						if (resizeFromFront) {
							row.RemoveAt(0);
						} else {
							row.RemoveAt(row.Count - 1);
						}
					}
				}
			} else if (newSizeX > SizeX) {
				for (int rowIndex = 0; rowIndex < Level0Tiles.Count; rowIndex++) {
					List<TerrainTile> row = Level0Tiles[rowIndex];

					while (row.Count < newSizeX) {
						TerrainTile newTerrainTile = new TerrainTile {
							Level = 0,
							IndexX = row.Count,
							IndexZ = rowIndex
						};

						if (resizeFromFront) {
							row.Insert(0, newTerrainTile);
						} else {
							row.Add(newTerrainTile);
						}
					}
				}

				for (int rowIndex = 0; rowIndex < Level1Tiles.Count; rowIndex++) {
					List<TerrainTile> row = Level1Tiles[rowIndex];

					while (row.Count < newSizeX) {
						TerrainTile newTerrainTile = new TerrainTile {
							Level = 1,
							IndexX = row.Count,
							IndexZ = rowIndex
						};

						if (resizeFromFront) {
							row.Insert(0, newTerrainTile);
						} else {
							row.Add(newTerrainTile);
						}
					}
				}
			}


			if (newSizeZ < SizeZ) {
				while (Level0Tiles.Count > newSizeZ) {
					if (resizeFromFront) {
						Level0Tiles.RemoveAt(0);
					} else {
						Level0Tiles.RemoveAt(Level0Tiles.Count - 1);
					}
				}

				while (Level1Tiles.Count > newSizeZ) {
					if (resizeFromFront) {
						Level1Tiles.RemoveAt(0);
					} else {
						Level1Tiles.RemoveAt(Level1Tiles.Count - 1);
					}
				}
			} else if (newSizeZ > SizeZ) {
				while (Level0Tiles.Count < newSizeZ) {
					List<TerrainTile> row = new List<TerrainTile>();

					for (int i = 0; i < newSizeX; i++) {
						TerrainTile newTerrainTile = new TerrainTile {
							Level = 0,
							IndexX = i,
							IndexZ = Level0Tiles.Count
						};

						row.Add(newTerrainTile);
					}

					if (resizeFromFront) {
						Level0Tiles.Insert(0, row);
					} else {
						Level0Tiles.Add(row);
					}
				}

				while (Level1Tiles.Count < newSizeZ) {
					List<TerrainTile> row = new List<TerrainTile>();

					for (int i = 0; i < newSizeX; i++) {
						row.Add(new TerrainTile {
							Level = 1,
							IndexX = i,
							IndexZ = Level1Tiles.Count
						});
					}

					if (resizeFromFront) {
						Level1Tiles.Insert(0, row);
					} else {
						Level1Tiles.Add(row);
					}
				}
			}

			for (int rowIndex = 0; rowIndex < Level0Tiles.Count; rowIndex++) {
				List<TerrainTile> row = Level0Tiles[rowIndex];

				for (int colIndex = 0; colIndex < row.Count; colIndex++) {
					TerrainTile terrainTile = row[colIndex];
					terrainTile.IndexX = colIndex;
					terrainTile.IndexZ = rowIndex;
				}
			}

			for (int rowIndex = 0; rowIndex < Level1Tiles.Count; rowIndex++) {
				List<TerrainTile> row = Level1Tiles[rowIndex];

				for (int colIndex = 0; colIndex < row.Count; colIndex++) {
					TerrainTile terrainTile = row[colIndex];
					terrainTile.IndexX = colIndex;
					terrainTile.IndexZ = rowIndex;
				}
			}

			SizeX = newSizeX;
			SizeZ = newSizeZ;
		}

		public TerrainTile GetCompanionTerrainTile(TerrainTile terrainTile) {
			return terrainTile.Level == 0
				? Level1Tiles[terrainTile.IndexZ][terrainTile.IndexX]
				: Level0Tiles[terrainTile.IndexZ][terrainTile.IndexX];
		}

		public List<byte> GetRawData() {
			List<byte> rawData = new List<byte>();
			List<List<List<TerrainTile>>> allLevels = new List<List<List<TerrainTile>>> { Level0Tiles, Level1Tiles };

			foreach (List<List<TerrainTile>> terrainLevel in allLevels) {
				foreach (List<TerrainTile> row in terrainLevel) {
					foreach (TerrainTile terrainTile in row) {
						rawData.AddRange(terrainTile.GetRawData());
					}
				}

				int totalTiles = SizeX * SizeZ;
				const int totalSpaceForTiles = 256;
				const int totalBytesPerTile = 8;

				while (totalTiles < totalSpaceForTiles) {
					for (int i = 0; i < totalBytesPerTile; i++) {
						rawData.Add(0);
					}

					totalTiles++;
				}
			}

			return rawData;
		}
	}
}