using GaneshaDx.Environment;
using GaneshaDx.Resources;

namespace GaneshaDx.Common;

public static class AutoSaver {
	public static void Update() {
		const int secondsPerMinute = 60;
		double currentTime = Stage.GameTime.TotalGameTime.TotalSeconds;
		double timeSinceLastSave = currentTime - MapData.TimeSinceLastSave;

		if (Configuration.Properties.AutoSaveEnabled &&
		    MapData.MapIsLoaded &&
		    timeSinceLastSave >= Configuration.Properties.AutoSaveDuration * secondsPerMinute
		   ) {
			MapData.SaveMap(true);
		}
	}
}