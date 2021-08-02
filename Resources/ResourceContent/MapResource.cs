using System;
using System.Collections.Generic;
using GaneshaDx.Common;

namespace GaneshaDx.Resources.ResourceContent {
	public class MapResource {
		private List<byte> RawData { get; }
		public ResourceType ResourceType { get; private set; }
		public MapArrangementState MapArrangementState { get; private set; }
		public MapTime MapTime { get; private set; }
		public MapWeather MapWeather { get; private set; }
		public int FileSector { get; private set; }
		private List<byte> RawResourceData { get; set; }
		public ResourceData ResourceData { get; private set; }
		public int XFile { get; private set; } = -1;

		public bool IsMesh => ResourceType == ResourceType.InitialMeshData ||
		                      ResourceType == ResourceType.OverrideMeshData ||
		                      ResourceType == ResourceType.AlternateStateMehData;

		public bool IsTexture => ResourceType == ResourceType.Texture;

		private const int ResourceHeaderIndex = 0;
		private const int ResourceTypeIndex = 4;
		private const int ResourceTypeIndex2 = 5;
		private const int ArrangementIdIndex = 2;
		private const int MapTimeIndex = 3;
		private const int FileSectorIndex = 8;

		public MapResource(List<byte> rawData) {
			RawData = rawData;

			if (rawData.Count < 20) {
				ResourceType = ResourceType.UnknownTrailingData;
				return;
			}

			SetResourceType();
			SetArrangementId();
			SetMapTime();
			SetFileSectorData();
		}

		public void SetResourceData(int xFile, List<byte> resourceData) {
			XFile = xFile;
			RawResourceData = resourceData;

			if (IsTexture) {
				ResourceData = new TextureResourceData(RawResourceData);
			} else if (IsMesh) {
				ResourceData = new MeshResourceData(RawResourceData);
			} else {
				throw new ArgumentOutOfRangeException();
			}
		}

		private void SetResourceType() {
			byte high = RawData[ResourceTypeIndex];
			byte low = RawData[ResourceTypeIndex2];

			List<byte> acceptableHeaders = new List<byte> {34, 48, 112};

			if (!acceptableHeaders.Contains(RawData[ResourceHeaderIndex])) {
				ResourceType = ResourceType.UnknownExtraDataB;
				return;
			}

			Dictionary<byte, ResourceType> resourceTypes = new Dictionary<byte, ResourceType> {
				{23, ResourceType.Texture},
				{46, ResourceType.InitialMeshData},
				{47, ResourceType.OverrideMeshData},
				{48, ResourceType.AlternateStateMehData},
				{49, ResourceType.Padded},
				{128, ResourceType.UnknownExtraDataA},
				{133, ResourceType.UnknownTwin},
				{134, ResourceType.UnknownTwin},
				{135, ResourceType.UnknownTwin},
				{136, ResourceType.UnknownTwin}
			};

			if (high == 1 && resourceTypes.ContainsKey(low)) {
				ResourceType = resourceTypes[low];
				return;
			}

			ResourceType = ResourceType.BadFormat;
		}

		private void SetArrangementId() {
			byte data = RawData[ArrangementIdIndex];

			MapArrangementState = data == 1
				? MapArrangementState.Secondary
				: MapArrangementState.Primary;
		}

		private void SetMapTime() {
			byte data = RawData[MapTimeIndex];
			string binary = Utilities.GetBinaryFromInt(data);

			bool isDay = Utilities.GetIntFromBinary(binary[0].ToString()) == 0;

			string weatherBits = binary.Substring(1, 3);
			int weatherData = Utilities.GetIntFromBinary(weatherBits);

			MapTime = isDay
				? MapTime.Day
				: MapTime.Night;

			if (weatherData == 0) {
				MapWeather = MapWeather.None;
			} else if (weatherData == 1) {
				MapWeather = MapWeather.NoneAlt;
			} else if (weatherData == 2) {
				MapWeather = MapWeather.Normal;
			} else if (weatherData == 3) {
				MapWeather = MapWeather.Strong;
			} else if (weatherData == 4) {
				MapWeather = MapWeather.VeryStrong;
			} else {
				Console.WriteLine("Invalid Weather Data");
			}
		}

		private void SetFileSectorData() {
			FileSector = Utilities.GetUInt16FromLittleEndian(RawData[FileSectorIndex], RawData[FileSectorIndex + 1]);
		}
	}
}