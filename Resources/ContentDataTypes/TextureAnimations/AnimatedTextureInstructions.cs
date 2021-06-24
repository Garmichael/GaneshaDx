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
			bool isPaletteAnimation = rawData[1] == 0 && rawData[2] == 224 && rawData[3] == 1 && rawData[14] == 3;

			if (isUvAnimation) {
				TextureAnimationType = TextureAnimationType.UvAnimation;
				Instructions = new UvAnimation(rawData);
			} else if (isPaletteAnimation) {
				TextureAnimationType = TextureAnimationType.PaletteAnimation;
				Instructions = new PaletteAnimation(rawData);
			} else {
				TextureAnimationType = TextureAnimationType.None;
			}
		}
	}
}