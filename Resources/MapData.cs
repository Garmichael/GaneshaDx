using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GaneshaDx.Common;
using GaneshaDx.Environment;
using GaneshaDx.Rendering;
using GaneshaDx.Resources.ContentDataTypes.Palettes;
using GaneshaDx.Resources.ResourceContent;
using GaneshaDx.UserInterface;
using GaneshaDx.UserInterface.GuiDefinitions;
using GaneshaDx.UserInterface.GuiForms;
using Microsoft.Xna.Framework.Graphics;

namespace GaneshaDx.Resources {
	public static class MapData {
		public static bool MapIsLoaded;

		private static string _mapRoot;
		public static string MapName { get; private set; }

		private static List<byte> _rawGnsData;
		public static List<MapResource> AllResources;
		public static List<MapResource> MeshResources;
		public static List<MapResource> TextureResources;

		public static void LoadMapDataFromFiles(string mapFolder, string mapName) {
			MapIsLoaded = false;
			AllResources = new List<MapResource>();
			MeshResources = new List<MapResource>();
			TextureResources = new List<MapResource>();
			MapName = mapName;
			_mapRoot = mapFolder + "\\" + MapName;

			_rawGnsData = File.ReadAllBytes(_mapRoot + ".GNS").ToList();

			ProcessAllResources();
			SetResourceFileData(_mapRoot);

			if (AllResourcesLoaded()) {
				Stage.Window.Title = "GaneshaDx - " + MapName;
				MapIsLoaded = true;
				CurrentMapState.SetState(MapArrangementState.Primary, MapTime.Day, MapWeather.None);
				ResetEditorState();
				MeshAnimationController.PlayAnimations();
			}
		}

		private static void ResetEditorState() {
			Gui.SelectedTab = RightPanelTab.Map;
			Gui.ShowMeshAnimationsWindow = false;
			Gui.ShowDebugAnimatedMeshWindow = false;
			GuiPanelTerrain.ResizeTerrainMode = false;
		}
		
		private static void ProcessAllResources() {
			for (int index = 0; index < _rawGnsData.Count; index++) {
				List<byte> resourceRawData = new List<byte>();

				int lengthOfResource = 20;

				while (index + lengthOfResource > _rawGnsData.Count) {
					lengthOfResource--;
				}

				for (int resourceIndex = 0; resourceIndex < lengthOfResource; resourceIndex++) {
					resourceRawData.Add(_rawGnsData[index + resourceIndex]);
				}

				MapResource mapResource = new MapResource(resourceRawData);

				if (mapResource.IsMesh || mapResource.IsTexture) {
					AllResources.Add(mapResource);
				}

				index += lengthOfResource - 1;
			}

			AllResources = AllResources.OrderBy(resource => resource.FileSector).ToList();

			foreach (MapResource resource in AllResources) {
				if (resource.IsMesh) {
					MeshResources.Add(resource);
				}

				if (resource.IsTexture) {
					TextureResources.Add(resource);
				}
			}
		}

		private static void SetResourceFileData(string mapRoot) {
			List<int> fileSectors = new List<int>();
			List<List<byte>> resourceFileData = new List<List<byte>>();
			List<int> resourceXFiles = new List<int>();

			foreach (MapResource resource in AllResources) {
				if (!fileSectors.Contains(resource.FileSector)) {
					fileSectors.Add(resource.FileSector);
				} else {
					OverlayConsole.AddMessage("Some Mismatch happened.. investigate this");
				}
			}

			int xFileIndex = 0;

			while (resourceFileData.Count < fileSectors.Count) {
				string xFileName = mapRoot + "." + xFileIndex;

				if (File.Exists(xFileName)) {
					resourceFileData.Add(File.ReadAllBytes(xFileName).ToList());
					resourceXFiles.Add(xFileIndex);
				}

				xFileIndex++;

				if (xFileIndex > 200) {
					Console.WriteLine("An XFile with more than 200??");
					break;
				}
			}

			if (resourceXFiles.Count == AllResources.Count) {
				for (int index = 0; index < AllResources.Count; index++) {
					AllResources[index].SetResourceData(resourceXFiles[index], resourceFileData[index]);
				}
			}
		}

		private static bool AllResourcesLoaded() {
			foreach (MapResource x in AllResources) {
				if (x.XFile == -1) {
					OverlayConsole.AddMessage("Could not locate all Resource files");
					return false;
				}
			}

			return true;
		}

		public static void ImportTexture(string filePath) {
			Texture2D importedTexture = Texture2D.FromFile(Stage.GraphicsDevice, filePath);

			if (importedTexture.Width != 256 && importedTexture.Height != 1024) {
				OverlayConsole.AddMessage("Cannot Import Texture. Textures must be 256x1024");
				return;
			}

			foreach (MapResource textureResource in TextureResources) {
				if (
					textureResource.MapArrangementState == CurrentMapState.StateData.MapArrangementState &&
					textureResource.MapTime == CurrentMapState.StateData.MapTime &&
					textureResource.MapWeather == CurrentMapState.StateData.MapWeather
				) {
					TextureResourceData textureResourceData = (TextureResourceData) textureResource.ResourceData;
					textureResourceData.Texture = importedTexture;
					CurrentMapState.StateData.Texture = importedTexture;
					break;
				}
			}
		}

		public static void ExportTexture(string filePath) {
			Stream stream = File.Create(filePath);
			Texture2D texture = CurrentMapState.StateData.Texture;
			texture.SaveAsPng(stream, texture.Width, texture.Height);
			OverlayConsole.AddMessage("Texture Exported as " + filePath);
			stream.Dispose();
		}

		public static void ImportPalette(string file, int paletteId, string paletteType) {
			List<byte> paletteData = File.ReadAllBytes(file).ToList();
			const int totalColors = 16;
			Palette sourcePalette = new Palette();

			for (int colorIndex = 0; colorIndex < totalColors * 3; colorIndex += 3) {
				int red = (int) Math.Floor(paletteData[colorIndex] / 8f);
				int green = (int) Math.Floor(paletteData[colorIndex + 1] / 8f);
				int blue = (int) Math.Floor(paletteData[colorIndex + 2] / 8f);
				PaletteColor color = new PaletteColor(red, green, blue, false);
				sourcePalette.Colors.Add(color);
			}

			List<PaletteColor> targetPalette = paletteType == "main"
				? CurrentMapState.StateData.Palettes[paletteId].Colors
				: CurrentMapState.StateData.PaletteAnimationFrames[paletteId].Colors;

			for (int colorIndex = 0; colorIndex < sourcePalette.Colors.Count; colorIndex++) {
				targetPalette[colorIndex].Red = sourcePalette.Colors[colorIndex].Red;
				targetPalette[colorIndex].Green = sourcePalette.Colors[colorIndex].Green;
				targetPalette[colorIndex].Blue = sourcePalette.Colors[colorIndex].Blue;
			}
		}

		public static void ExportPalette(string filePath, int paletteId, string paletteType) {
			List<byte> actData = new List<byte>();

			List<PaletteColor> sourcePalette = paletteType == "main"
				? CurrentMapState.StateData.Palettes[paletteId].Colors
				: CurrentMapState.StateData.PaletteAnimationFrames[paletteId].Colors;

			foreach (PaletteColor color in sourcePalette) {
				actData.Add((byte) (color.Red * 8));
				actData.Add((byte) (color.Green * 8));
				actData.Add((byte) (color.Blue * 8));
			}

			while (actData.Count < 256 * 3) {
				actData.Add(0);
			}

			Stream stream = File.Create(filePath);
			stream.Write(actData.ToArray());
			stream.Dispose();
		}

		public static void SaveMap() {
			foreach (MapResource textureResource in TextureResources) {
				TextureResourceData data = (TextureResourceData) textureResource.ResourceData;
				data.RebuildRawData();

				Stream stream = File.Create(_mapRoot + "." + textureResource.XFile);
				stream.Write(data.RawData.ToArray());
				stream.Dispose();
			}

			foreach (MapResource mehResource in MeshResources) {
				MeshResourceData data = (MeshResourceData) mehResource.ResourceData;
				data.RebuildRawData();

				Stream stream = File.Create(_mapRoot + "." + mehResource.XFile);
				stream.Write(data.RawData.ToArray());
				stream.Dispose();
			}

			OverlayConsole.AddMessage("Map Saved");
		}
	}
}