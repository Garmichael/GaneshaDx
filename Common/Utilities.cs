using System;
using System.Collections.Generic;
using GaneshaDx.Resources;
using GaneshaDx.Resources.ContentDataTypes.Polygons;
using GaneshaDx.UserInterface;
using Microsoft.Xna.Framework;

namespace GaneshaDx.Common {
	public static class Utilities {
		public static int GetIntFromHex(string value) {
			return int.Parse(value, System.Globalization.NumberStyles.HexNumber);
		}

		public static int GetIntFromBinary(string value) {
			return Convert.ToInt32(value, 2);
		}

		public static string GetBinaryFromInt(int value, int expandSize = 8) {
			string binaryValue = Convert.ToString(value, 2);

			while (binaryValue.Length < expandSize) {
				binaryValue = "0" + binaryValue;
			}

			return binaryValue;
		}

		public static string GetBinaryFromHex(string value, int expandSize = 8) {
			return GetBinaryFromInt(GetIntFromHex(value), expandSize);
		}

		public static string GetHexFromBinary(string value) {
			string hex = Convert.ToInt32(value, 2).ToString("X");
			if (hex.Length == 1) {
				hex = "0" + hex;
			}

			return hex;
		}

		public static string GetHexFromInt(int value) {
			return GetHexFromBinary(GetBinaryFromInt(value));
		}

		public static int GetUInt16FromLittleEndian(byte high, byte low) {
			return BitConverter.ToUInt16(new[] {high, low}, 0);
		}

		public static int GetInt16FromLittleEndian(byte high, byte low) {
			return BitConverter.ToInt16(new[] {high, low}, 0);
		}

		public static (byte high, byte low) GetLittleEndianFromInt16(int value) {
			byte[] bytes = BitConverter.GetBytes((short)value);
			return (bytes[0], bytes[1]);
		}

		public static int GetInt32FromLittleEndian(byte highest, byte high, byte low, byte lowest) {
			return BitConverter.ToInt32(new[] {highest, high, low, lowest}, 0);
		}
		
		public static (byte highest, byte high, byte low, byte lowest) GetLittleEndianFromInt32(int value) {
			byte[] bytes = BitConverter.GetBytes(value);
			return (bytes[0], bytes[1], bytes[2], bytes[3]);
		}

		public static Color GetColorFromHex(string hex) {
			hex = hex.Replace("#", string.Empty);
			int r = GetIntFromHex(hex.Substring(0, 2));
			int g = GetIntFromHex(hex.Substring(2, 2));
			int b = GetIntFromHex(hex.Substring(4, 2));

			return new Color(r, g, b, 255);
		}

		public static Vector3 RoundVector3ToInt(Vector3 vector3) {
			vector3.X = (float) Math.Round(vector3.X);
			vector3.Y = (float) Math.Round(vector3.Y);
			vector3.Z = (float) Math.Round(vector3.Z);

			return vector3;
		}

		public static int SnapIntToInterval(int value, int interval) {
			double newValue = Math.Round(value / (double) interval, MidpointRounding.AwayFromZero) * interval;

			return (int) newValue;
		}

		public static Vector3 SnapVector3ToGrid(Vector3 vector3, float gridSize) {
			vector3.X = SnapFloatToGrid(vector3.X, gridSize);
			vector3.Y = SnapFloatToGrid(vector3.Y, gridSize);
			vector3.Z = SnapFloatToGrid(vector3.Z, gridSize);

			return vector3;
		}

		public static float SnapFloatToGrid(float value, float gridSize) {
			return (float) Math.Round(value / gridSize, MidpointRounding.AwayFromZero) * gridSize;
		}

		public static Vector3 GetAveragePoint(List<Vector3> points) {
			Vector3 total = Vector3.Zero;
			foreach (Vector3 vertex in points) {
				total += vertex;
			}

			return new Vector3(total.X / points.Count, total.Y / points.Count, total.Z / points.Count);
		}

		public static Vector2 GetAveragePoint(List<Vector2> points) {
			Vector2 total = Vector2.Zero;
			foreach (Vector2 vertex in points) {
				total += vertex;
			}

			return new Vector2(total.X / points.Count, total.Y / points.Count);
		}

		public static Color[] CropColorArray(Color[] colors, Rectangle rectangle) {
			List<Color> croppedColors = new List<Color>();

			int startIndex = 256 * (rectangle.Top - 1);
			int endIndex = 256 * (rectangle.Bottom + 1);

			for (int colorIndex = startIndex; colorIndex < endIndex; colorIndex++) {
				int col = GetColFromStrip(colorIndex, 256);
				int row = GetRowFromStrip(colorIndex, 256);
				if (
					col >= rectangle.Left &&
					col < rectangle.Right &&
					row >= rectangle.Top &&
					row < rectangle.Bottom
				) {
					croppedColors.Add(colors[colorIndex]);
				}
			}

			return croppedColors.ToArray();
		}

		private static int GetColFromStrip(int index, int width) {
			return index % width;
		}

		private static int GetRowFromStrip(int index, int width) {
			return (int) Math.Floor(index / (float) width);
		}

		private static readonly Random RandomNumber = new Random(432897);

		public static int GetRandomInt(int min, int max) {
			return RandomNumber.Next(min, max + 1);
		}

		public static int Clamp(int value, int min, int max) {
			if (value < min) {
				value = min;
			}

			if (value > max) {
				value = max;
			}

			return value;
		}

		public static float Clamp(float value, float min, float max) {
			if (value < min) {
				value = min;
			}

			if (value > max) {
				value = max;
			}

			return value;
		}

		public static int Min(int value, int min) {
			if (value < min) {
				value = min;
			}

			return value;
		}

		public static float Min(float value, float min) {
			if (value < min) {
				value = min;
			}

			return value;
		}

		public static (double, double) VectorToSphere(Vector3 vector) {
			double radius = Math.Sqrt(
				vector.X * vector.X +
				vector.Y * vector.Y +
				vector.Z * vector.Z
			);

			if (radius == 0) {
				return (0, 0);
			}

			double elevation = Math.Acos(-vector.Y / radius);
			double azimuth = Math.Atan2(vector.X, vector.Z);

			float elevationAngle = MathHelper.ToDegrees((float) elevation) - 90;
			float azimuthAngle = -MathHelper.ToDegrees((float) azimuth) + 90;
			if (azimuthAngle <= 0) {
				azimuthAngle += 360;
			}

			return (elevationAngle, azimuthAngle);
		}

		public static Vector3 SphereToVector(double elevation, double azimuth) {
			double elevationRadians = MathHelper.ToRadians((float) (elevation + 90));
			double azimuthRadians = MathHelper.ToRadians((float) (azimuth + 90));

			return new Vector3(
				(float) (Math.Sin(elevationRadians) * Math.Sin(azimuthRadians)),
				(float) -Math.Cos(elevationRadians),
				(float) (Math.Sin(elevationRadians) * Math.Cos(azimuthRadians))
			);
		}

		public static bool Vector3Equal(Vector3 a, Vector3 b) {
			return Math.Abs(a.X - b.X) < 0.001f &&
			       Math.Abs(a.Y - b.Y) < 0.001f &&
			       Math.Abs(a.Z - b.Z) < 0.001f;
		}

		public static void AverageNormals() {
			Dictionary<(int, int, int), List<Vertex>> overlappingVertexSets =
				new Dictionary<(int, int, int), List<Vertex>>();

			foreach (Polygon polygon in Selection.SelectedPolygons) {
				foreach (Vertex vertex in polygon.Vertices) {
					int x = (int) vertex.Position.X;
					int y = (int) vertex.Position.Y;
					int z = (int) vertex.Position.Z;

					if (!overlappingVertexSets.ContainsKey((x, y, z))) {
						overlappingVertexSets.Add((x, y, z), new List<Vertex> {vertex});
					} else {
						overlappingVertexSets[(x, y, z)].Add(vertex);
					}
				}
			}

			foreach (
				KeyValuePair<(int, int, int), List<Vertex>> overlappingVertexSet
				in overlappingVertexSets
			) {
				double totalElevation = 0;
				double azimuthCosSum = 0;
				double azimuthSinSum = 0;
				double azimuthsCounted = 0;

				foreach (Vertex vertex in overlappingVertexSet.Value) {
					totalElevation += vertex.NormalElevation;

					if (Math.Abs(vertex.NormalElevation - 90) > 0.01f) {
						azimuthSinSum += Math.Sin(MathHelper.ToRadians(vertex.NormalAzimuth));
						azimuthCosSum += Math.Cos(MathHelper.ToRadians(vertex.NormalAzimuth));
						azimuthsCounted++;
					}
				}

				double averageElevation = totalElevation / overlappingVertexSet.Value.Count;

				double averageAzimuth = azimuthsCounted == 0
					? 90
					: MathHelper.ToDegrees((float) Math.Atan2(azimuthSinSum, azimuthCosSum));

				if (averageAzimuth < 0) {
					averageAzimuth += 360;
				}

				foreach (Vertex vertex in overlappingVertexSet.Value) {
					vertex.NormalElevation = (float) averageElevation;
					vertex.NormalAzimuth = (float) averageAzimuth;
				}
			}
		}

		public static System.Numerics.Vector3 ConvertVector3(Vector4 vector4) {
			return new System.Numerics.Vector3(vector4.X, vector4.Y, vector4.Z);
		}

		public static System.Numerics.Vector3 ConvertVector3(Vector3 vector3) {
			return new System.Numerics.Vector3(vector3.X, vector3.Y, vector3.Z);
		}

		public static System.Numerics.Vector4 ConvertVector4(Vector4 vector4) {
			return new System.Numerics.Vector4(vector4.X, vector4.Y, vector4.Z, vector4.W);
		}

		public static bool PointIsWithinTriangle(
			Vector2 point,
			Vector2 trianglePointA,
			Vector2 trianglePointB,
			Vector2 TrianglePointC
		) {
			float a = .5f * (-trianglePointB.Y * TrianglePointC.X +
			                 trianglePointA.Y * (-trianglePointB.X + TrianglePointC.X) +
			                 trianglePointA.X * (trianglePointB.Y - TrianglePointC.Y) +
			                 trianglePointB.X * TrianglePointC.Y);

			int sign = a < 0 ? -1 : 1;

			float s = sign * (trianglePointA.Y * TrianglePointC.X -
			                  trianglePointA.X * TrianglePointC.Y +
			                  (TrianglePointC.Y - trianglePointA.Y) * point.X +
			                  (trianglePointA.X - TrianglePointC.X) * point.Y);

			float t = sign * (trianglePointA.X * trianglePointB.Y -
			                  trianglePointA.Y * trianglePointB.X +
			                  (trianglePointA.Y - trianglePointB.Y) * point.X +
			                  (trianglePointB.X - trianglePointA.X) * point.Y);

			return s > 0 && t > 0 && (s + t) < 2 * a * sign;
		}

		public static bool PointIsWithinRange(Vector2 point, Vector2 center, float radius) {
			return Vector2.Distance(point, center) < radius;
		}
		
		public static int GetTotalPolygonCount(PolygonType polygonType) {
			List<MeshType> allMeshTypes = new List<MeshType> {
				MeshType.PrimaryMesh, MeshType.AnimatedMesh1, MeshType.AnimatedMesh2, MeshType.AnimatedMesh3,
				MeshType.AnimatedMesh4, MeshType.AnimatedMesh5, MeshType.AnimatedMesh6, MeshType.AnimatedMesh7, MeshType.AnimatedMesh8
			};

			int total = 0;

			foreach (MeshType meshType in allMeshTypes) {
				total += CurrentMapState.StateData.PolygonCollection[meshType][polygonType].Count;
			}

			return total;
		}
	}
}