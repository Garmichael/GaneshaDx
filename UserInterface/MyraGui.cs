using System.Collections.Generic;
using System.Linq;
using GaneshaDx.Common;
using GaneshaDx.Environment;
using GaneshaDx.Resources;
using Myra;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.File;

namespace GaneshaDx.UserInterface {
	public static class MyraGui {
		public static Desktop Desktop;
		private static bool _modalIsOpen;
		private static FileDialog _openFileDialog;
		private static FileDialog _saveAsDialog;
		private static FileDialog _exportGlbDialog;
		private static string _lastGlbFileLocation;

		private static FileDialog _importTextureDialog;
		private static FileDialog _exportTextureDialog;
		private static FileDialog _exportUvMapDialog;
		private static string _lastTextureFileLocation;
		public static string LastImportedTextureFile = "";

		private static FileDialog _importPaletteDialog;
		private static FileDialog _exportPaletteDialog;
		private static string _lastPaletteFileLocation;

		public static bool LockModeling = false;
		public static bool IsActive => Desktop.FocusedKeyboardWidget != null || _modalIsOpen || LockModeling;

		public static void BuildUi() {
			MyraEnvironment.Game = Stage.Ganesha;

			BuildOpenFileDialog();
			BuildSaveAsFileDialog();
			BuildExportGlbFileDialog();

			BuildImportTextureFileDialog();
			BuildExportTextureFileDialog();
			BuildExportUvMapFileDialog();

			BuildImportPaletteFileDialog();
			BuildExportPaletteFileDialog();

			Desktop = new Desktop();
		}

		public static void Render() {
			Stage.GraphicsDevice.Viewport = Stage.WholeViewport;

			Desktop.Render();
		}

		public static void OpenOpenFileDialog() {
			Selection.SelectedPolygons.Clear();
			Selection.HoveredPolygons.Clear();

			_openFileDialog.ShowModal(Desktop);
			_modalIsOpen = true;
		}

		public static void OpenImportTextureFileDialog() {
			_importTextureDialog.ShowModal(Desktop);
			_modalIsOpen = true;
		}

		public static void OpenExportTextureFileDialog(string fileName) {
			_exportTextureDialog.FilePath = fileName;
			_exportTextureDialog.ShowModal(Desktop);
			_modalIsOpen = true;
		}
		
		public static void OpenSaveAsFileDialog() {
			_saveAsDialog.ShowModal(Desktop);
			_modalIsOpen = true;
		}
		
		public static void OpenExportGlbFileDialog(string fileName) {
			_exportGlbDialog.FilePath = fileName;
			_exportGlbDialog.ShowModal(Desktop);
			_modalIsOpen = true;
		}

		public static void OpenExportUvMapFileDialog(string fileName) {
			_exportUvMapDialog.FilePath = fileName;
			_exportUvMapDialog.ShowModal(Desktop);
			_modalIsOpen = true;
		}

		public static void OpenImportPaletteFileDialog(int paletteId, string paletteType) {
			_importPaletteDialog.UserData["PaletteId"] = paletteId.ToString();
			_importPaletteDialog.UserData["PaletteType"] = paletteType;
			_importPaletteDialog.ShowModal(Desktop);
			_modalIsOpen = true;
		}

		public static void OpenExportPaletteFileDialog(string fileName, int paletteId, string paletteType) {
			_exportPaletteDialog.UserData["PaletteId"] = paletteId.ToString();
			_exportPaletteDialog.UserData["PaletteType"] = paletteType;
			_exportPaletteDialog.FilePath = fileName;
			_exportPaletteDialog.ShowModal(Desktop);
			_modalIsOpen = true;
		}

		private static void BuildOpenFileDialog() {
			_openFileDialog = new FileDialog(FileDialogMode.OpenFile) {
				Filter = "*.gns",
				Folder = Configuration.Properties.LoadFolder
			};

			_openFileDialog.Closed += (s, a) => {
				if (_openFileDialog.Result) {
					_openFileDialog.Folder = _openFileDialog.FilePath;

					Configuration.Properties.LoadFolder = _openFileDialog.Folder;
					Configuration.SaveConfiguration();

					List<string> pathSegments = _openFileDialog.FilePath.Split('\\').ToList();
					string fileName = pathSegments.Last();

					List<string> mapNameSegments = fileName.Split('.').ToList();
					mapNameSegments.RemoveAt(mapNameSegments.Count - 1);
					string mapName = string.Join('.', mapNameSegments);

					pathSegments.RemoveAt(pathSegments.Count - 1);
					string fileDirectory = string.Join('\\', pathSegments);

					MapData.LoadMapDataFromFiles(fileDirectory, mapName);
				}

				_modalIsOpen = false;
			};
		}

		private static void BuildSaveAsFileDialog() {
			_saveAsDialog = new FileDialog(FileDialogMode.SaveFile) {
				Folder = Configuration.Properties.LoadFolder
			};

			_saveAsDialog.Closed += (s, a) => {
				if (_saveAsDialog.Result) {
					string filePath = _saveAsDialog.FilePath;
					_saveAsDialog.Folder = filePath;
					Configuration.Properties.LoadFolder = filePath;

					List<string> pathSegments = filePath.Split('\\').ToList();
					string fileName = pathSegments.Last();

					MapData.SaveMapAs(_saveAsDialog.Folder, fileName);
				}

				_modalIsOpen = false;
			};
		}
		
		private static void BuildExportGlbFileDialog() {
			_lastGlbFileLocation = Configuration.Properties.LoadFolder;
				
			_exportGlbDialog = new FileDialog(FileDialogMode.SaveFile) {
				Folder = _lastGlbFileLocation,
				Filter = "*.glb"
			};

			_exportGlbDialog.Closed += (s, a) => {
				if (_exportGlbDialog.Result) {
					string filePath = _exportGlbDialog.FilePath;
					_exportGlbDialog.Folder = filePath;
					_lastGlbFileLocation = filePath;

					List<string> pathSegments = filePath.Split('\\').ToList();
					string fileName = pathSegments.Last();
					List<string> filenameSegments = fileName.Split('.').ToList();

					if (filenameSegments.Last().ToLower() != "glb") {
						fileName += ".glb";
					}

					MapData.ExportGlb(_exportGlbDialog.Folder + "\\" + fileName);
				}

				_modalIsOpen = false;
			};
		}

		private static void BuildImportTextureFileDialog() {
			_lastTextureFileLocation = Configuration.Properties.LoadFolder;

			_importTextureDialog = new FileDialog(FileDialogMode.OpenFile) {
				Filter = "*.png",
				Folder = _lastTextureFileLocation,
			};

			_importTextureDialog.Closed += (s, a) => {
				if (_importTextureDialog.Result) {
					_importTextureDialog.Folder = _importTextureDialog.FilePath;
					_exportTextureDialog.Folder = _importTextureDialog.FilePath;
					_lastTextureFileLocation = _importTextureDialog.Folder;
					LastImportedTextureFile = _importTextureDialog.FilePath;
					MapData.ImportTexture(_importTextureDialog.FilePath);
				}

				_modalIsOpen = false;
			};
		}

		private static void BuildExportTextureFileDialog() {
			_exportTextureDialog = new FileDialog(FileDialogMode.SaveFile) {
				Folder = _lastTextureFileLocation,
				Filter = "*.png"
			};

			_exportTextureDialog.Closed += (s, a) => {
				if (_exportTextureDialog.Result) {
					string filePath = _exportTextureDialog.FilePath;
					_exportTextureDialog.Folder = filePath;
					_importTextureDialog.Folder = filePath;
					_lastTextureFileLocation = filePath;

					List<string> pathSegments = filePath.Split('\\').ToList();
					string fileName = pathSegments.Last();
					List<string> filenameSegments = fileName.Split('.').ToList();

					if (filenameSegments.Last().ToLower() != "png") {
						fileName += ".png";
					}

					MapData.ExportTexture(_exportTextureDialog.Folder + "\\" + fileName);
				}

				_modalIsOpen = false;
			};
		}

		private static void BuildExportUvMapFileDialog() {
			_exportUvMapDialog = new FileDialog(FileDialogMode.SaveFile) {
				Folder = _lastTextureFileLocation,
				Filter = "*.png"
			};

			_exportUvMapDialog.Closed += (s, a) => {
				if (_exportUvMapDialog.Result) {
					string filePath = _exportUvMapDialog.FilePath;
					_exportUvMapDialog.Folder = filePath;
					_importTextureDialog.Folder = filePath;
					_lastTextureFileLocation = filePath;

					List<string> pathSegments = filePath.Split('\\').ToList();
					string fileName = pathSegments.Last();
					List<string> filenameSegments = fileName.Split('.').ToList();

					if (filenameSegments.Last().ToLower() != "png") {
						fileName += ".png";
					}

					MapData.ExportUvMap(_exportUvMapDialog.Folder + "\\" + fileName);
				}

				_modalIsOpen = false;
			};
		}
		
		private static void BuildImportPaletteFileDialog() {
			_lastPaletteFileLocation = Configuration.Properties.LoadFolder;

			_importPaletteDialog = new FileDialog(FileDialogMode.OpenFile) {
				Filter = "*.act",
				Folder = _lastPaletteFileLocation,
			};

			_importPaletteDialog.UserData.Add("PaletteId", "0");
			_importPaletteDialog.UserData.Add("PaletteType", "Main");

			_importPaletteDialog.Closed += (s, a) => {
				if (_importPaletteDialog.Result) {
					_importPaletteDialog.Folder = _importPaletteDialog.FilePath;
					_exportPaletteDialog.Folder = _importPaletteDialog.FilePath;
					_lastPaletteFileLocation = _importPaletteDialog.Folder;

					MapData.ImportPalette(
						_importPaletteDialog.FilePath,
						int.Parse(_importPaletteDialog.UserData["PaletteId"]),
						_importPaletteDialog.UserData["PaletteType"]
					);
				}

				_modalIsOpen = false;
			};
		}

		private static void BuildExportPaletteFileDialog() {
			_exportPaletteDialog = new FileDialog(FileDialogMode.SaveFile) {
				Folder = _lastPaletteFileLocation,
				Filter = "*.act"
			};

			_exportPaletteDialog.Closed += (s, a) => {
				if (_exportPaletteDialog.Result) {
					string filePath = _exportPaletteDialog.FilePath;
					_exportPaletteDialog.Folder = filePath;
					_importPaletteDialog.Folder = filePath;
					_lastPaletteFileLocation = filePath;

					List<string> pathSegments = filePath.Split('\\').ToList();
					string fileName = pathSegments.Last();
					List<string> filenameSegments = fileName.Split('.').ToList();

					if (filenameSegments.Last().ToLower() != "act") {
						fileName += ".act";
					}

					MapData.ExportPalette(
						_exportPaletteDialog.Folder + "\\" + fileName,
						int.Parse(_exportPaletteDialog.UserData["PaletteId"]),
						_exportPaletteDialog.UserData["PaletteType"]
					);
				}

				_modalIsOpen = false;
			};
		}
	}
}