using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Vector3 = System.Numerics.Vector3;

namespace GaneshaDx.Common {
	public static class Configuration {
		private const string ConfigurationFile = "Preferences.json";
		public static ConfigurationProperties Properties;

		public static void LoadConfiguration() {
			if (!File.Exists(ConfigurationFile)) {
				Properties = new ConfigurationProperties();
				SaveConfiguration();
			} else {
				string json = File.ReadAllText(ConfigurationFile);
				Properties = JsonConvert.DeserializeObject<ConfigurationProperties>(json);
			}
		}

		public static void SaveConfiguration() {
			FileStream stream = new FileStream(ConfigurationFile, FileMode.Create);
			using (StreamWriter writer = new StreamWriter(stream, Encoding.Default)) {
				writer.WriteLine(JsonConvert.SerializeObject(Properties, Formatting.Indented));
			}

			stream.Dispose();
		}

		public class ConfigurationProperties {
			public bool ShowFps = true;
			public bool ShowUnknownValues = false;
			public string LoadFolder = @"C:\";
			public float PanningSensitivity = 1f;
			public float RotationSensitivity = 1f;
			public float ZoomStrength = 1.2f;
			public bool InvertedPanning;
			public bool InvertedRotation;
			public bool RenderAlphaAsSemiTransparent;
			public bool AllowBackfaceSelection;
			public bool HighlightSelectionOnTexturePage = true;
			public bool TexturePreviewShowUnselectedUvs = true;
			public int TexturePreviewShiftArrowDistance = 20;
			public float TranslateAxisControlSpeed = 0.33f;
			public int PolygonTransparencyForTerrainEditing = 0;
			public int TerrainTransparencyForTerrainEditing = 100;
			public int TerrainTransparencyForPolygonEditing = 0;
			public Vector3 TerrainColorForPolygonEditing = Utilities.ConvertVector3(Color.Cyan.ToVector3());
			public bool RenderPolygonsInLightingMode = false;
			public bool IsolateMeshes = false;
			public bool SwapCameraControls = false;
			public bool AutoSaveEnabled = true;
			public int AutoSaveDuration = 10;
			public bool RenderFftOrtho = false;
			public bool HideHiddenPolysByFacing = false;
			public bool PlaysScriptedTextureAnimations = false;
			public bool ExpandCompassToTerrainLength = true;
			public float AutoMapRatio = 0.679f;
			public bool HideNormalIndicators = false;
			public int NormalIndicatorLength = 50;
		}
	}
}