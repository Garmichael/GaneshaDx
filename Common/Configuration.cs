using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace GaneshaDx.Common;

public static class Configuration {
	private static string ApplicationDataFolderPath => System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
	private static readonly string ApplicationFolderName = @"\GaneshaDx\";
	private static readonly string ConfigurationFileName = "Preferences.json";
	private static string ConfigurationFilePath => ApplicationDataFolderPath + ApplicationFolderName + ConfigurationFileName;

	public static ConfigurationProperties Properties;

	public static void LoadConfiguration() {
		if (PreferencesFileExists()) {
			Properties = ReadFromPreferencesFile();
		} else {
			Properties = new ConfigurationProperties();
			SaveConfiguration();
		}
	}

	public static void SaveConfiguration() {
		if (!PreferencesFileExists()) {
			CreatePreferencesFile();
		}

		WriteToPreferencesFile();
	}

	private static ConfigurationProperties ReadFromPreferencesFile() {
		ConfigurationProperties properties;
		string json;

		try {
			json = File.ReadAllText(ConfigurationFilePath);
		} catch {
			return new ConfigurationProperties();
		}

		try {
			properties = JsonConvert.DeserializeObject<ConfigurationProperties>(json);
		} catch {
			properties = new ConfigurationProperties();
		}

		return properties;
	}

	private static void WriteToPreferencesFile() {
		FileStream stream = new(ConfigurationFilePath, FileMode.Create);
		using (StreamWriter writer = new(stream, Encoding.Default)) {
			writer.WriteLine(JsonConvert.SerializeObject(Properties, Formatting.Indented));
		}

		stream.Dispose();
	}

	private static bool PreferencesFileExists() {
		bool fileExists = File.Exists(ConfigurationFilePath);
		return fileExists;
	}

	private static void CreatePreferencesFile() {
		Directory.CreateDirectory(ApplicationDataFolderPath + ApplicationFolderName);
	}
}