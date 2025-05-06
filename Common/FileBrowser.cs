using System.IO;
using GaneshaDx.Environment;
using GaneshaDx.Resources;
using GaneshaDx.UserInterface;
using NativeFileDialogExtendedSharp;

namespace GaneshaDx.Common;

public static class FileBrowser {
	public static string LastImportedTextureFile = "";

	public static void OpenMapDialog() {
		NfdFilter[] filters = { new() { Specification = "gns", Description = "GNS File" } };
		NfdDialogResult result = Nfd.FileOpen(filters);

		if (result.Status == NfdStatus.Ok) {
			Selection.SelectedPolygons.Clear();
			Selection.HoveredPolygons.Clear();
			Stage.Ganesha.PostponeRender(1);
			MapData.LoadMapDataFromFullPath(result.Path);
		}
	}

	public static void SaveMapAsDialog() {
		NfdFilter[] filters = { new() { Specification = "gns", Description = "GNS File" } };
		NfdDialogResult result = Nfd.FileSave(filters);

		if (result.Status == NfdStatus.Ok) {
			string mapName = Path.GetFileNameWithoutExtension(result.Path);
			string directory = Path.GetDirectoryName(result.Path);
			MapData.SaveMapAs(directory, mapName);
		}
	}

	public static void ExportGlbDialog() {
		NfdFilter[] filters = { new() { Specification = "glb", Description = "GLB File" } };
		NfdDialogResult result = Nfd.FileSave(filters, MapData.MapName);

		if (result.Status == NfdStatus.Ok) {
			MapData.ExportGlb(result.Path);
		}
	}

	public static void ExportTextureDialog() {
		NfdFilter[] filters = { new() { Specification = "png", Description = "PNG File" } };
		NfdDialogResult result = Nfd.FileSave(filters, MapData.MapName);

		if (result.Status == NfdStatus.Ok) {
			MapData.ExportTexture(result.Path);
		}
	}

	public static void ImportTextureDialog() {
		NfdFilter[] filters = { new() { Specification = "png", Description = "PNG File" } };
		NfdDialogResult result = Nfd.FileOpen(filters);

		if (result.Status == NfdStatus.Ok) {
			MapData.ImportTexture(result.Path);
			LastImportedTextureFile = result.Path;
		}
	}

	public static void ExportUvsDialog() {
		NfdFilter[] filters = { new() { Specification = "png", Description = "PNG File" } };
		NfdDialogResult result = Nfd.FileSave(filters, MapData.MapName + "_UVMap");

		if (result.Status == NfdStatus.Ok) {
			MapData.ExportUvMap(result.Path);
		}
	}

	public static void ExportPalette(int paletteId, string paletteType) {
		NfdFilter[] filters = { new() { Specification = "act", Description = "Palette File" } };
		string fileName = paletteId >= 0
			? MapData.MapName + "_Palette" + paletteId + "_" + paletteType
			: "FFT_DefaultPalette";

		NfdDialogResult result = Nfd.FileSave(filters, fileName);

		if (result.Status == NfdStatus.Ok) {
			MapData.ExportPalette(
				result.Path,
				paletteId,
				paletteType
			);
		}
	}

	public static void ImportPalette(int paletteId, string paletteType) {
		NfdFilter[] filters = { new() { Specification = "act", Description = "Palette File" } };
		NfdDialogResult result = Nfd.FileOpen(filters);

		if (result.Status == NfdStatus.Ok) {
			MapData.ImportPalette(result.Path, paletteId, paletteType);
		}
	}
}