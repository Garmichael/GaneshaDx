using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace GaneshaDx.Common;

public static class Configuration {
	private const string ConfigurationFile = "Preferences.json";
	public static ConfigurationProperties Properties;

	public static void LoadConfiguration() {
		if (!File.Exists(ConfigurationFile)) {
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
		FileStream stream = new(ConfigurationFile, FileMode.Create);
		using (StreamWriter writer = new(stream, Encoding.Default)) {
			writer.WriteLine(JsonConvert.SerializeObject(Properties, Formatting.Indented));
		}

		stream.Dispose();
	}
}