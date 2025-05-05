﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GaneshaDx.Common;
using GaneshaDx.Environment;
using GaneshaDx.Rendering;
using GaneshaDx.Resources.ContentDataTypes.Palettes;
using GaneshaDx.Resources.GnsData;
using GaneshaDx.Resources.ResourceContent;
using GaneshaDx.UserInterface;
using GaneshaDx.UserInterface.GuiDefinitions;
using GaneshaDx.UserInterface.GuiForms;
using Microsoft.Xna.Framework.Graphics;

namespace GaneshaDx.Resources;

public static class MapData {
	public static bool MapIsLoaded;
	public static double TimeSinceLastSave;

	public static string MapFolder;
	public static string MapName { get; private set; }

	public static Gns Gns;
	public static List<MapResource> AllResources;
	public static List<MapResource> MeshResources;
	public static List<MapResource> TextureResources;

	public static void ReloadCurrentMap() {
		LoadMapDataFromFullPath(MapFolder + Path.DirectorySeparatorChar + MapName + ".gns");
		OverlayConsole.AddMessage("Map Reloaded");
	}
	
	public static void LoadMapDataFromFullPath(string gnsPath) {
		string extension = Path.GetExtension(gnsPath);
		
		if (extension.ToLower() != ".gns") {
			OverlayConsole.AddMessage("Map File is not a .GNS file");
			return;
		}

		MapIsLoaded = false;
		AllResources = new List<MapResource>();
		MeshResources = new List<MapResource>();
		TextureResources = new List<MapResource>();
		
		MapName = Path.GetFileNameWithoutExtension(gnsPath);
		MapFolder = Path.GetDirectoryName(gnsPath);

		List<byte> gnsData = File.ReadAllBytes(gnsPath).ToList();
		Gns = new Gns(gnsData);

		ProcessAllResources();
		SetResourceFileData(MapFolder + Path.DirectorySeparatorChar + MapName);

		if (AllResourcesLoaded()) {
			Stage.Window.Title = "GaneshaDx - " + MapName;
			MapIsLoaded = true;
			TimeSinceLastSave = Stage.GameTime.TotalGameTime.TotalSeconds;
			CurrentMapState.SetState(MapArrangementState.Primary, MapTime.Day, MapWeather.None);
			ResetEditorState();
			MeshAnimationController.PlayAnimations();
		}
	}

	private static void ResetEditorState() {
		Selection.SelectedPolygons.Clear();
		Selection.SelectedTerrainTiles.Clear();
		Gui.SelectedTab = RightPanelTab.Map;
		Gui.ShowMeshAnimationsWindow = false;
		Gui.ShowDebugAnimatedMeshWindow = false;
		GuiPanelTerrain.ResizeTerrainMode = false;
		GuiPanelMeshSelector.SelectedMesh = MeshType.PrimaryMesh;
		StageCamera.Reset();
	}

	private static void ProcessAllResources() {
		for (int index = 0; index < Gns.RawData.Count; index++) {
			List<byte> resourceRawData = new();

			int lengthOfResource = 20;

			while (index + lengthOfResource > Gns.RawData.Count) {
				lengthOfResource--;
			}

			for (int resourceIndex = 0; resourceIndex < lengthOfResource; resourceIndex++) {
				resourceRawData.Add(Gns.RawData[index + resourceIndex]);
			}

			MapResource mapResource = new(resourceRawData);

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
		List<int> fileSectors = new();
		List<List<byte>> resourceFileData = new();
		List<int> resourceXFiles = new();

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

	public static void ExportGlb(string filePath) {
		GlbExporter.Export(filePath);
		OverlayConsole.AddMessage("Map Exported as " + filePath);
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
		Palette sourcePalette = new();

		for (int colorIndex = 0; colorIndex < totalColors * 3; colorIndex += 3) {
			int red = (int) Math.Floor(paletteData[colorIndex] / 8f);
			int green = (int) Math.Floor(paletteData[colorIndex + 1] / 8f);
			int blue = (int) Math.Floor(paletteData[colorIndex + 2] / 8f);
			PaletteColor color = new(red, green, blue, false);
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
		List<byte> actData = new();

		if (paletteId == -1) {
			for (int i = 0; i < 16; i++) {
				actData.Add((byte) (i * 17));
				actData.Add((byte) (i * 17));
				actData.Add((byte) (i * 17));
			}
		} else {
			List<PaletteColor> sourcePalette = paletteType == "main"
				? CurrentMapState.StateData.Palettes[paletteId].Colors
				: CurrentMapState.StateData.PaletteAnimationFrames[paletteId].Colors;

			foreach (PaletteColor color in sourcePalette) {
				actData.Add((byte) (color.Red * 8));
				actData.Add((byte) (color.Green * 8));
				actData.Add((byte) (color.Blue * 8));
			}
		}

		while (actData.Count < 256 * 3) {
			actData.Add(0);
		}

		Stream stream = File.Create(filePath);
		stream.Write(actData.ToArray());
		stream.Dispose();
	}

	public static void ExportUvMap(string filePath) {
		Stream stream = File.Create(filePath);
		Texture2D texture = GuiWindowTextureElement.GetUvMapTexture();
		texture.SaveAsPng(stream, texture.Width, texture.Height);
		OverlayConsole.AddMessage("Uv Map Exported as " + filePath);
		stream.Dispose();
	}

	public static void SaveMap(bool isAutoSave = false) {
		string mapFolder = MapFolder;
		string backupNotation = "";

		if (isAutoSave) {
			mapFolder += Path.DirectorySeparatorChar + "gdx_autosave";
			DateTime time = DateTime.Now;
			backupNotation = " (" +
			                 time.Date.Year + "-" +
			                 time.Date.DayOfYear + "--" +
			                 time.Hour + "-" +
			                 time.Minute +
			                 ")";

			if (!Directory.Exists(mapFolder)) {
				Directory.CreateDirectory(mapFolder);
			}
		}

		string mapRoot = mapFolder + Path.DirectorySeparatorChar + MapName;

		Stream gnsStream = File.Create(mapRoot + backupNotation + ".GNS");
		gnsStream.Write(Gns.RawData.ToArray());
		gnsStream.Dispose();

		foreach (MapResource textureResource in TextureResources) {
			TextureResourceData data = (TextureResourceData) textureResource.ResourceData;
			data.RebuildRawData();

			Stream stream = File.Create(mapRoot + backupNotation + "." + textureResource.XFile);
			stream.Write(data.RawData.ToArray());
			stream.Dispose();
		}

		foreach (MapResource mehResource in MeshResources) {
			MeshResourceData data = (MeshResourceData) mehResource.ResourceData;
			data.RebuildRawData();

			Stream stream = File.Create(mapRoot + backupNotation + "." + mehResource.XFile);
			stream.Write(data.RawData.ToArray());
			stream.Dispose();
		}

		TimeSinceLastSave = Stage.GameTime.TotalGameTime.TotalSeconds;
		Stage.Window.Title = "GaneshaDx - " + MapName;

		OverlayConsole.AddMessage(isAutoSave ? "Map Backed up to \\gdx_autosave\\" +  MapName + backupNotation : "Map Saved");
	}

	public static void SaveMapAs(string newFolder, string mapName) {
		MapFolder = newFolder;
		MapName = mapName;
		SaveMap();
	}
}