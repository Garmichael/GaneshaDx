using System.Collections.Generic;
using GaneshaDx.Common;

namespace GaneshaDx.Resources.ContentDataTypes.TextureAnimations {
	public class AnimatedTextureInstructions {
		public TextureAnimationType TextureAnimationType;
		public TextureAnimation Instructions;

		public AnimatedTextureInstructions() {
			TextureAnimationType = TextureAnimationType.None;
			Instructions = new TextureAnimation();
		}

		public AnimatedTextureInstructions(List<byte> rawData) {
			bool isUvAnimation = rawData[1] == 3 && rawData[9] == 3;
			bool isPaletteAnimation = rawData[2] == 224;

			bool isEmptyAnimation = IsEmptyAnimation(rawData);

			if (isUvAnimation) {
				TextureAnimationType = TextureAnimationType.UvAnimation;
				Instructions = new UvAnimation(rawData);
			} else if (isPaletteAnimation) {
				TextureAnimationType = TextureAnimationType.PaletteAnimation;
				Instructions = new PaletteAnimation(rawData);
			} else if (!isEmptyAnimation) {
				TextureAnimationType = TextureAnimationType.UnknownAnimation;
				Instructions = new UnknownAnimation(rawData);
			} else {
				TextureAnimationType = TextureAnimationType.None;
			}
		}

		private bool IsEmptyAnimation(List<byte> rawData) {
			bool isEmptyAnimation = true;
			foreach (byte data in rawData) {
				if (data != 0) {
					isEmptyAnimation = false;
				}
			}

			return isEmptyAnimation;
		}

		public List<byte> GetRawData() {
			List<byte> rawData = new List<byte>();

			if (TextureAnimationType == TextureAnimationType.UvAnimation) {
				UvAnimation instructions = (UvAnimation) Instructions;
				rawData.AddRange(instructions.GetRawData());
			} else if (TextureAnimationType == TextureAnimationType.PaletteAnimation) {
				PaletteAnimation instructions = (PaletteAnimation) Instructions;
				rawData.AddRange(instructions.GetRawData());
			} else {
				for (int i = 0; i < 20; i++) {
					rawData.Add(0);
				}
			}

			return rawData;
		}
	}
}