using Microsoft.Xna.Framework;
using Vector3 = System.Numerics.Vector3;

namespace GaneshaDx.Common;

public class ConfigurationProperties {
	public bool ShowFps = true;
	public bool ShowUnknownValues = false;
	public float PanningSensitivity = 1f;
	public float RotationSensitivity = 1f;
	public float ZoomStrength = 1.2f;
	public bool InvertedPanning;
	public bool InvertedRotation;
	public bool RenderAlphaAsSemiTransparent;
	public bool AllowBackfaceSelection;
	public bool HighlightSelectedPoly = true;
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
	public bool ExpandCompassToTerrainLength = false;
	public float AutoMapRatio = 0.679f;
	public bool HideNormalIndicators = false;
	public int NormalIndicatorLength = 50;
	public bool ShowGameViewOverlay = false;
	public bool HideRedTiles = false;
	public bool PutPanelOnLeft = false;
}