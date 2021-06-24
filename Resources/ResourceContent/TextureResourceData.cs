using System;
using System.Collections.Generic;
using GaneshaDx.Common;
using GaneshaDx.Environment;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GaneshaDx.Resources.ResourceContent {
	public class TextureResourceData : ResourceData {
		public readonly List<byte> RawData;
		public readonly List<Color> Pixels = new List<Color>();
		public Texture2D Texture;

		private const int TextureWidth = 256;
		private const int TextureHeight = 1024;

		public TextureResourceData(List<byte> rawData) {
			RawData = rawData;
			ProcessPixels();
			SaveTexture2D();
		}

		public void RebuildRawData() {
			RawData.Clear();
			Color[] pixels = new Color[Texture.Width * Texture.Height];
			Texture.GetData(pixels);

			for (int color = 0; color < pixels.Length; color += 2) {
				Color pixelA = pixels[color];
				Color pixelB = pixels[color + 1];

				string binary = Utilities.GetBinaryFromInt((int) Math.Floor(pixelB.R / 17f), 4) +
				                Utilities.GetBinaryFromInt((int) Math.Floor(pixelA.R / 17f), 4);

				RawData.Add((byte) Utilities.GetIntFromBinary(binary));
			}
		}

		private void ProcessPixels() {
			Pixels.Clear();

			foreach (byte data in RawData) {
				string binary = Utilities.GetBinaryFromInt(data);

				int pixelA = Utilities.GetIntFromBinary(binary.Substring(4, 4));
				int pixelB = Utilities.GetIntFromBinary(binary.Substring(0, 4));

				Pixels.Add(new Color(pixelA * 17, pixelA * 17, pixelA * 17, 255));
				Pixels.Add(new Color(pixelB * 17, pixelB * 17, pixelB * 17, 255));
			}
		}

		private void SaveTexture2D() {
			Texture = new Texture2D(Stage.GraphicsDevice, TextureWidth, TextureHeight);
			Texture.SetData(Pixels.ToArray());
		}
	}
}