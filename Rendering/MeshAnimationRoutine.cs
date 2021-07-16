using System.Collections.Generic;
using GaneshaDx.Common;
using GaneshaDx.Environment;
using GaneshaDx.Resources;
using GaneshaDx.Resources.ContentDataTypes.MeshAnimations;
using Microsoft.Xna.Framework;

namespace GaneshaDx.Rendering {
	public class MeshAnimationRoutine {
		public readonly MeshAnimationFrame CurrentFrame;
		private readonly MeshAnimationFrameState _currentFrameState;
		private readonly double _startTime;
		private double _timeIntoAnimation;
		private double _percentIntoAnimation;
		private readonly bool _isInitialFrame;

		private readonly Vector3 _startPosition;
		private readonly Vector3 _startRotation;
		private readonly Vector3 _startScale;

		private Vector3 _endPosition;
		private Vector3 _endRotation;
		private Vector3 _endScale;

		public Vector3 CurrentPosition;
		public Vector3 CurrentRotation;
		public Vector3 CurrentScale;

		public MeshAnimationRoutine(
			MeshAnimationFrame currentFrame,
			Vector3 startPosition,
			Vector3 startRotation,
			Vector3 startScale,
			bool isInitialFrame
		) {
			MeshAnimationInstructions frameStates = CurrentMapState.StateData.MeshAnimationInstructions;
			_currentFrameState = frameStates.FrameStates[currentFrame.FrameStateId - 1];
			CurrentFrame = currentFrame;
			_startTime = Stage.GameTime.TotalGameTime.TotalSeconds;
			_isInitialFrame = isInitialFrame;
			_startPosition = startPosition;
			_startRotation = startRotation;
			_startScale = startScale;
			CurrentPosition = startPosition;
			CurrentRotation = startRotation;
			CurrentScale = startScale;
		}

		public bool IsFinished() {
			return _percentIntoAnimation >= 1;
		}

		public void Animate() {
			_timeIntoAnimation = Stage.GameTime.TotalGameTime.TotalSeconds - _startTime;
			double totalDurationInSeconds = CurrentFrame.Duration / 60f;
			_percentIntoAnimation = _timeIntoAnimation / totalDurationInSeconds;

			_endPosition = new Vector3(
				(float) _currentFrameState.Position[0],
				(float) _currentFrameState.Position[1],
				(float) _currentFrameState.Position[2]
			);
			_endRotation = new Vector3(
				(float) _currentFrameState.Rotation[0],
				(float) _currentFrameState.Rotation[1],
				(float) _currentFrameState.Rotation[2]
			);
			_endScale = new Vector3(
				(float) _currentFrameState.Scale[0],
				(float) _currentFrameState.Scale[1],
				(float) _currentFrameState.Scale[2]
			);

			if (_isInitialFrame) {
				CurrentPosition.X = _endPosition.X;
				CurrentPosition.Y = _endPosition.Y;
				CurrentPosition.Z = _endPosition.Z;
				CurrentScale.X = _endScale.X;
				CurrentScale.Y = _endScale.Y;
				CurrentScale.Z = _endScale.Z;
			} else {
				CurrentPosition.X = _currentFrameState.PositionKeyFrameTypes[0] switch {
					MeshAnimationKeyFrameType.TweenTo => TweenTo(_startPosition.X, _endPosition.X),
					MeshAnimationKeyFrameType.TweenBy => TweenBy(_startPosition.X, _endPosition.X),
					_ => _startPosition.X
				};

				CurrentPosition.Y = _currentFrameState.PositionKeyFrameTypes[1] switch {
					MeshAnimationKeyFrameType.TweenTo => TweenTo(_startPosition.Y, _endPosition.Y),
					MeshAnimationKeyFrameType.TweenBy => TweenBy(_startPosition.Y, _endPosition.Y),
					_ => _startPosition.Y
				};

				CurrentPosition.Z = _currentFrameState.PositionKeyFrameTypes[2] switch {
					MeshAnimationKeyFrameType.TweenTo => TweenTo(_startPosition.Z, _endPosition.Z),
					MeshAnimationKeyFrameType.TweenBy => TweenBy(_startPosition.Z, _endPosition.Z),
					_ => _startPosition.Z
				};

				CurrentScale.X = _currentFrameState.ScaleKeyFrameTypes[0] switch {
					MeshAnimationKeyFrameType.TweenTo => TweenTo(_startScale.X, _endScale.X),
					MeshAnimationKeyFrameType.TweenBy => TweenBy(_startScale.X, _endScale.X),
					_ => _startScale.X
				};

				CurrentScale.Y = _currentFrameState.ScaleKeyFrameTypes[1] switch {
					MeshAnimationKeyFrameType.TweenTo => TweenTo(_startScale.Y, _endScale.Y),
					MeshAnimationKeyFrameType.TweenBy => TweenBy(_startScale.Y, _endScale.Y),
					_ => _startScale.Y
				};

				CurrentScale.Z = _currentFrameState.ScaleKeyFrameTypes[2] switch {
					MeshAnimationKeyFrameType.TweenTo => TweenTo(_startScale.Z, _endScale.Z),
					MeshAnimationKeyFrameType.TweenBy => TweenBy(_startScale.Z, _endScale.Z),
					_ => _startScale.Z
				};
			}

			CurrentRotation.X = _currentFrameState.RotationKeyFrameTypes[0] switch {
				MeshAnimationKeyFrameType.TweenTo => TweenTo(_startRotation.X, _endRotation.X),
				MeshAnimationKeyFrameType.TweenBy => TweenBy(_startRotation.X, _endRotation.X),
				_ => _startRotation.X
			};

			CurrentRotation.Y = _currentFrameState.RotationKeyFrameTypes[1] switch {
				MeshAnimationKeyFrameType.TweenTo => TweenTo(_startRotation.Y, _endRotation.Y),
				MeshAnimationKeyFrameType.TweenBy => TweenBy(_startRotation.Y, _endRotation.Y),
				_ => _startRotation.Y
			};

			CurrentRotation.Z = _currentFrameState.RotationKeyFrameTypes[2] switch {
				MeshAnimationKeyFrameType.TweenTo => TweenTo(_startRotation.Z, _endRotation.Z),
				MeshAnimationKeyFrameType.TweenBy => TweenBy(_startRotation.Z, _endRotation.Z),
				_ => _startRotation.Z
			};
		}

		private float TweenTo(float start, float end) {
			return (float) ((end - start) * _percentIntoAnimation + start);
		}

		private float TweenBy(float start, float end) {
			end += start;
			return (float) ((end - start) * _percentIntoAnimation + start);
		}
	}
}