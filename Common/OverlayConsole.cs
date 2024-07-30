using System;
using System.Collections.Generic;
using GaneshaDx.Environment;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GaneshaDx.Common;

public static class OverlayConsole {
	private static readonly List<OverlayMessage> Messages = new();
	private static readonly SpriteFont Font;

	static OverlayConsole() {
		Font = Stage.Content.Load<SpriteFont>("OverlayFont");
	}

	public static void Update() {
		double currentTime = Stage.GameTime.TotalGameTime.TotalMilliseconds;

		for (int messageIndex = Messages.Count - 1; messageIndex >= 0; messageIndex--) {
			OverlayMessage message = Messages[messageIndex];

			if (currentTime >= message.StartTime + message.LengthOfDisplay) {
				Messages.RemoveAt(messageIndex);
			}
		}
	}

	public static void Render() {
		int top = 0;
		const int left = 10;
		const int messageHeight = 28;

		Stage.SpriteBatch.Begin();

		foreach (OverlayMessage message in Messages) {
			Vector2 displayPosition = new(left, top + 40);
			Stage.SpriteBatch.DrawString(Font, message.Text, displayPosition - new Vector2(1, -1), Color.Black);
			Stage.SpriteBatch.DrawString(Font, message.Text, displayPosition, Color.White);
			top += messageHeight;
		}

		Stage.SpriteBatch.End();
	}

	public static void AddMessage(string message) {
		Messages.Add(new OverlayMessage {
			StartTime = Stage.GameTime.TotalGameTime.TotalMilliseconds,
			LengthOfDisplay = 3000,
			Text = message
		});

		if (Messages.Count > 10) {
			Messages.RemoveAt(0);
		}
	}
}

public class OverlayMessage {
	public Double StartTime;
	public Double LengthOfDisplay;
	public string Text;
}