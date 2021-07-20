using Microsoft.Xna.Framework;

namespace GaneshaDx.Resources.ContentDataTypes.Polygons {
	public class Vertex {
		private Vector3 _originalPosition;

		public Vector3 Position {
			get => _originalPosition;
			set {
				_originalPosition = value;
				LastAnimatedStartPosition = value;
				CurrentAnimatedPosition = value;
			}
		}

		public Vector3 LastAnimatedStartPosition;
		public Vector3 CurrentAnimatedPosition;
		public bool UsesNormal;
		public float NormalAzimuth;
		public float NormalElevation;
		public Color Color;

		public Vertex(
			Vector3 position,
			Color color,
			bool usesNormal = false,
			float normalAzimuth = 0,
			float normalElevation = 0
		) {
			_originalPosition = position;
			Position = position;
			LastAnimatedStartPosition = position;
			CurrentAnimatedPosition = position;
			Color = color;
			UsesNormal = usesNormal;
			NormalAzimuth = normalAzimuth;
			NormalElevation = normalElevation;
		}
	}
}