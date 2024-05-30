using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace GaneshaDx.Common;

public static class Configuration {
	private const string ConfigurationFile = "Preferences.json";
	private static string _configurationPath;
	public static ConfigurationProperties Properties;

	public static void LoadConfiguration() {
		_configurationPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\GaneshaDx" + "\\";
		if (!File.Exists(_configurationPath + ConfigurationFile)) {
			if (!Directory.Exists(_configurationPath)) {
				Directory.CreateDirectory(_configurationPath);
			}
			
			Properties = new ConfigurationProperties();
			SaveConfiguration();
		} else {
			string json = File.ReadAllText(ConfigurationFile);
			try {
				Properties = JsonConvert.DeserializeObject<ConfigurationProperties>(json);
			} catch {
				Properties = new ConfigurationProperties();
			}
		}
	}

	public static void SaveConfiguration() {
		FileStream stream = new(_configurationPath + ConfigurationFile, FileMode.Create);
		using (StreamWriter writer = new(stream, Encoding.Default)) {
			writer.WriteLine(JsonConvert.SerializeObject(Properties, Formatting.Indented));
		}

		stream.Dispose();
	}
}