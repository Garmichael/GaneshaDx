﻿using System;
using GaneshaDx.Common;
using GaneshaDx.Environment;
using GaneshaDx.Resources;
using GaneshaDx.Resources.ContentDataTypes.MeshAnimations;
using Microsoft.Xna.Framework;

namespace GaneshaDx.Rendering;

public class MeshAnimationRoutine {
	public readonly AnimatedMeshInstruction AnimatedMeshInstruction;
	public AnimatedMeshInstructionSet MyAnimationInstructionSet;
	private readonly MeshAnimationKeyframe _currentKeyframe;
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
		AnimatedMeshInstruction animatedMeshInstruction,
		Vector3 startPosition,
		Vector3 startRotation,
		Vector3 startScale,
		bool isInitialFrame,
		AnimatedMeshInstructionSet myAnimationInstructionSet
	) {
		MeshAnimationSet instructions = CurrentMapState.StateData.MeshAnimationSet;
		_currentKeyframe = animatedMeshInstruction == null || animatedMeshInstruction.FrameStateId == 0
			? null
			: instructions.Keyframes[animatedMeshInstruction.FrameStateId - 1];
		AnimatedMeshInstruction = animatedMeshInstruction;
		_startTime = Stage.GameTime.TotalGameTime.TotalSeconds;
		_isInitialFrame = isInitialFrame;
		_startPosition = startPosition;
		_startRotation = startRotation;
		_startScale = startScale;
		CurrentPosition = startPosition;
		CurrentRotation = startRotation;
		CurrentScale = startScale;
		MyAnimationInstructionSet = myAnimationInstructionSet;
	}

	public bool IsFinished() {
		return _percentIntoAnimation >= 1;
	}

	public void Animate() {
		if (AnimatedMeshInstruction == null || _currentKeyframe == null) {
			return;
		}

		_timeIntoAnimation = Stage.GameTime.TotalGameTime.TotalSeconds - _startTime;
		double totalDurationInSeconds = AnimatedMeshInstruction.Duration / 60f;

		if (AnimatedMeshInstruction.Duration <= 0) {
			totalDurationInSeconds = 1 / 60f;
		}

		_percentIntoAnimation = _timeIntoAnimation / totalDurationInSeconds;

		_endPosition = new Vector3(
			(float) _currentKeyframe.Position[0],
			(float) _currentKeyframe.Position[1],
			(float) _currentKeyframe.Position[2]
		);
		_endRotation = new Vector3(
			(float) _currentKeyframe.Rotation[0],
			(float) _currentKeyframe.Rotation[1],
			(float) _currentKeyframe.Rotation[2]
		);
		_endScale = new Vector3(
			(float) _currentKeyframe.Scale[0],
			(float) _currentKeyframe.Scale[1],
			(float) _currentKeyframe.Scale[2]
		);


		CurrentPosition.X = _currentKeyframe.PositionTweenTypes[0] switch {
			MeshAnimationTweenType.TweenTo => TweenTo(_startPosition.X, _endPosition.X),
			MeshAnimationTweenType.TweenBy => TweenBy(_startPosition.X, _endPosition.X),
			MeshAnimationTweenType.Oscillate => Oscillate(_startPosition.X, _endPosition.X),
			_ => _startPosition.X
		};

		CurrentPosition.Y = _currentKeyframe.PositionTweenTypes[1] switch {
			MeshAnimationTweenType.TweenTo => TweenTo(_startPosition.Y, _endPosition.Y),
			MeshAnimationTweenType.TweenBy => TweenBy(_startPosition.Y, _endPosition.Y),
			MeshAnimationTweenType.Oscillate => Oscillate(_startPosition.Y, _endPosition.Y),
			_ => _startPosition.Y
		};

		CurrentPosition.Z = _currentKeyframe.PositionTweenTypes[2] switch {
			MeshAnimationTweenType.TweenTo => TweenTo(_startPosition.Z, _endPosition.Z),
			MeshAnimationTweenType.TweenBy => TweenBy(_startPosition.Z, _endPosition.Z),
			MeshAnimationTweenType.Oscillate => Oscillate(_startPosition.Z, _endPosition.Z),
			_ => _startPosition.Z
		};

		CurrentRotation.X = _currentKeyframe.RotationTweenTypes[0] switch {
			MeshAnimationTweenType.TweenTo => TweenTo(_startRotation.X, _endRotation.X),
			MeshAnimationTweenType.TweenBy => TweenBy(_startRotation.X, _endRotation.X),
			MeshAnimationTweenType.Oscillate => Oscillate(_startRotation.X, _endRotation.X),
			_ => _startRotation.X
		};

		CurrentRotation.Y = _currentKeyframe.RotationTweenTypes[1] switch {
			MeshAnimationTweenType.TweenTo => TweenTo(_startRotation.Y, _endRotation.Y),
			MeshAnimationTweenType.TweenBy => TweenBy(_startRotation.Y, _endRotation.Y),
			MeshAnimationTweenType.Oscillate => Oscillate(_startRotation.Y, _endRotation.Y),
			_ => _startRotation.Y
		};

		CurrentRotation.Z = _currentKeyframe.RotationTweenTypes[2] switch {
			MeshAnimationTweenType.TweenTo => TweenTo(_startRotation.Z, _endRotation.Z),
			MeshAnimationTweenType.TweenBy => TweenBy(_startRotation.Z, _endRotation.Z),
			MeshAnimationTweenType.Oscillate => Oscillate(_startRotation.Z, _endRotation.Z),
			_ => _startRotation.Z
		};

		CurrentScale.X = _currentKeyframe.ScaleTweenTypes[0] switch {
			MeshAnimationTweenType.TweenTo => TweenTo(_startScale.X, _endScale.X),
			MeshAnimationTweenType.TweenBy => TweenBy(_startScale.X, _endScale.X),
			MeshAnimationTweenType.Oscillate => Oscillate(_startScale.X, _endScale.X),
			_ => _startScale.X
		};

		CurrentScale.Y = _currentKeyframe.ScaleTweenTypes[1] switch {
			MeshAnimationTweenType.TweenTo => TweenTo(_startScale.Y, _endScale.Y),
			MeshAnimationTweenType.TweenBy => TweenBy(_startScale.Y, _endScale.Y),
			MeshAnimationTweenType.Oscillate => Oscillate(_startScale.Y, _endScale.Y),
			_ => _startScale.Y
		};

		CurrentScale.Z = _currentKeyframe.ScaleTweenTypes[2] switch {
			MeshAnimationTweenType.TweenTo => TweenTo(_startScale.Z, _endScale.Z),
			MeshAnimationTweenType.TweenBy => TweenBy(_startScale.Z, _endScale.Z),
			MeshAnimationTweenType.Oscillate => Oscillate(_startScale.Z, _endScale.Z),
			_ => _startScale.Z
		};

		while (CurrentRotation.X < 0) {
			CurrentRotation.X += 360;
		}

		while (CurrentRotation.Y < 0) {
			CurrentRotation.Y += 360;
		}

		while (CurrentRotation.Z < 0) {
			CurrentRotation.Z += 360;
		}

		while (CurrentRotation.X > 360) {
			CurrentRotation.X -= 360;
		}

		while (CurrentRotation.Y > 360) {
			CurrentRotation.Y -= 360;
		}

		while (CurrentRotation.Z > 360) {
			CurrentRotation.Z -= 360;
		}
	}

	private float TweenTo(float start, float end) {
		return _isInitialFrame
			? end
			: (float) ((end - start) * _percentIntoAnimation + start);
	}

	private float TweenBy(float start, float end) {
		end += start;
		return (float) ((end - start) * _percentIntoAnimation + start);
	}

	private float Oscillate(float start, float end) {
		return (float) (end * Math.Sin(2 * Math.PI * _percentIntoAnimation) + start);
	}
}