using GaneshaDx.Environment;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GaneshaDx.UserInterface.Input {
	public static class AppInput {
		public static KeyboardState ThisKeyboardState;
		private static KeyboardState _lastKeyboardState;

		public static bool ShiftHeld => ThisKeyboardState.IsKeyDown(Keys.LeftShift) ||
		                                ThisKeyboardState.IsKeyDown(Keys.RightShift);

		public static bool ControlHeld => ThisKeyboardState.IsKeyDown(Keys.LeftControl) ||
		                                  ThisKeyboardState.IsKeyDown(Keys.RightControl);


		public static bool AltHeld => ThisKeyboardState.IsKeyDown(Keys.LeftAlt) ||
		                              ThisKeyboardState.IsKeyDown(Keys.RightAlt);

		public static int ThisScrollValue;
		private static int _lastScrollValue;

		public static MouseState ThisMouseState;
		private static MouseState _lastMouseState;

		public static Vector2 ThisMousePosition;
		private static Vector2 _lastMousePosition;

		public static bool LeftMouseClicked;
		public static bool LeftMouseReleased;

		public static bool MouseScrolledUp;
		public static bool MouseScrolledDown;
		public static Vector2 MousePositionChange;

		public static bool MouseIsWithinModelViewport;

		public static void Update() {
			_lastKeyboardState = ThisKeyboardState;
			_lastMouseState = ThisMouseState;
			_lastMousePosition = ThisMousePosition;
			_lastMouseState = ThisMouseState;
			_lastScrollValue = ThisMouseState.ScrollWheelValue;

			ThisKeyboardState = Keyboard.GetState();
			ThisMouseState = Mouse.GetState();

			ThisMousePosition = ThisMouseState.Position.ToVector2();
			ThisScrollValue = ThisMouseState.ScrollWheelValue;
			LeftMouseClicked = ThisMouseState.LeftButton == ButtonState.Pressed &&
			                   _lastMouseState.LeftButton == ButtonState.Released;
			LeftMouseReleased = ThisMouseState.LeftButton == ButtonState.Released &&
			                    _lastMouseState.LeftButton == ButtonState.Pressed;

			MouseIsWithinModelViewport =
				ThisMousePosition.Y > Stage.ModelingViewport.Y &&
				ThisMousePosition.Y < Stage.ModelingViewport.Y + Stage.ModelingViewport.Height &&
				ThisMousePosition.X > Stage.ModelingViewport.X &&
				ThisMousePosition.X < Stage.ModelingViewport.X + Stage.ModelingViewport.Width;

			MouseScrolledUp = ThisScrollValue < _lastScrollValue;
			MouseScrolledDown = ThisScrollValue > _lastScrollValue;
			MousePositionChange = ThisMousePosition - _lastMousePosition;
		}

		public static bool KeyJustPressed(Keys key) {
			return ThisKeyboardState.IsKeyDown(key) &&
			       !_lastKeyboardState.IsKeyDown(key);
		}
	}
}