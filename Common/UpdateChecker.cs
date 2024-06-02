using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Octokit;

namespace GaneshaDx.Common;

public static class UpdateChecker {
	public static bool AppIsOutdated;
	private static string ApplicationDataFolderPath => System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
	private static readonly string ApplicationFolderName = @"\GaneshaDx\";
	private static readonly string InstallerFileName = "GaneshaDxSetup.exe";
	public static string InstallerFilePath => ApplicationDataFolderPath + ApplicationFolderName + InstallerFileName;

	private static bool _hasCheckedForUpdate;
	
	public static bool ShouldCheckForUpdate() {
		return !_hasCheckedForUpdate;
	}
	
	public static async Task CheckForUpdate() {
		_hasCheckedForUpdate = true;
		
		GitHubClient githubClient = new(new ProductHeaderValue("GaneshaDx"));
		Release remoteRelease = await githubClient.Repository.Release.GetLatest("Garmichael", "GaneshaDx");
		ParsedVersion remoteVersion = new(remoteRelease.TagName);
		ParsedVersion localVersion = new(Program.Version);
		ParsedVersion mostRecent = GetMostRecentVersion(remoteVersion, localVersion);

		if (mostRecent == remoteVersion) {
			IReadOnlyList<ReleaseAsset> assets = remoteRelease.Assets;

			using HttpClient client = new();
			Uri uri = null;

			foreach (ReleaseAsset asset in assets) {
				if (asset.BrowserDownloadUrl.Contains(InstallerFileName)) {
					uri = new Uri(asset.BrowserDownloadUrl);
				}
			}

			if (uri != null) {
				await client.DownloadFileTaskAsync(uri);
			}
		}
	}

	private static async Task DownloadFileTaskAsync(this HttpClient client, Uri uri) {
		await using Stream stream = await client.GetStreamAsync(uri);
		await using FileStream fileStream = new(InstallerFilePath, System.IO.FileMode.Create);
		await stream.CopyToAsync(fileStream);

		AppIsOutdated = true;
	}

	private static ParsedVersion GetMostRecentVersion(ParsedVersion first, ParsedVersion second) {
		if (first.Major > second.Major) {
			return first;
		}

		if (first.Major < second.Major) {
			return second;
		}

		if (first.Minor > second.Minor) {
			return first;
		}

		if (first.Minor < second.Minor) {
			return second;
		}

		if (first.Patch > second.Patch) {
			return first;
		}

		if (first.Patch < second.Patch) {
			return second;
		}

		return null;
	}
}

public class ParsedVersion {
	public readonly int Major;
	public readonly int Minor;
	public readonly int Patch;

	public ParsedVersion(string versionString) {
		string[] versionSegments = versionString.Split(".");

		if (versionSegments.Length > 0) {
			int.TryParse(versionSegments[0], out Major);

			if (versionSegments.Length > 1) {
				int.TryParse(versionSegments[1], out Minor);

				if (versionSegments.Length > 1) {
					int.TryParse(versionSegments[2], out Patch);
				} else {
					Minor = 0;
				}
			} else {
				Minor = 0;
				Patch = 0;
			}
		} else {
			Major = 0;
			Minor = 0;
			Patch = 0;
		}
	}
}