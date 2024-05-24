using System.Collections.Generic;
using GaneshaDx.Environment;
using GaneshaDx.UserInterface.Input;
using Microsoft.Xna.Framework;

namespace GaneshaDx.Common;

public static class CameraRay {
	private static Vector3 _rayOrigin;
	private static Vector3 _rayDirection;
	const int RayMagnitude = 10000;
		
	public static CameraRayResults GetResults(List<Vector3> vertices, bool forceBackfacing = false) {
		BuildProjectionProperties();
			
		bool checksBackfacing = Configuration.Properties.AllowBackfaceSelection || forceBackfacing;
		Vector3 edgeA = vertices[1] - vertices[0];
		Vector3 edgeB = vertices[2] - vertices[0];
		Vector3 distanceVector = _rayOrigin - vertices[0];
		Vector3 edgeACross = Vector3.Cross(distanceVector, edgeA);
		Vector3 edgeBCross = Vector3.Cross(_rayDirection, edgeB);
		double determinant = Vector3.Dot(edgeA, edgeBCross);
		double inverseDeterminant = determinant == 0 ? 0 : 1.0f / determinant;
		double triangleU = Vector3.Dot(distanceVector, edgeBCross) * inverseDeterminant;
		double triangleV = Vector3.Dot(_rayDirection, edgeACross) * inverseDeterminant;
		double distanceToHit = Vector3.Dot(edgeB, edgeACross) * inverseDeterminant;

		if (
			!checksBackfacing && determinant > 0 ||
			checksBackfacing && determinant == 0 ||
			triangleU < -0 ||
			triangleU > 1 ||
			triangleV < -0 ||
			triangleU + triangleV > 1
		) {
			return new CameraRayResults {
				HitDistance = 0,
				HitPoint = Vector3.Zero,
				HasHit = false
			};
		}

		Vector3 hitPoint = _rayOrigin + _rayDirection * (float) distanceToHit;

		return new CameraRayResults {
			HitDistance = distanceToHit,
			HitPoint = hitPoint,
			HasHit = !(distanceToHit <= 0f) && !(distanceToHit >= RayMagnitude)
		};
	}

	private static void BuildProjectionProperties() {
		_rayOrigin = Stage.ModelingViewport.Unproject(
			new Vector3(AppInput.ThisMousePosition.X, AppInput.ThisMousePosition.Y, 0.0f),
			Stage.ProjectionMatrix,
			Stage.ViewMatrix,
			Matrix.Identity
		);

		Vector3 farPoint = Stage.ModelingViewport.Unproject(
			new Vector3(AppInput.ThisMousePosition.X, AppInput.ThisMousePosition.Y, 1.0f),
			Stage.ProjectionMatrix,
			Stage.ViewMatrix,
			Matrix.Identity
		);

		_rayDirection = farPoint - _rayOrigin;
		_rayDirection.Normalize();
	}
}