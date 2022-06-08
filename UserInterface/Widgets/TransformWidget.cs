using System.Collections.Generic;
using System.Linq;
using GaneshaDx.Common;
using GaneshaDx.Environment;
using GaneshaDx.Resources.ContentDataTypes.Polygons;
using Microsoft.Xna.Framework;

namespace GaneshaDx.UserInterface.Widgets {
	public static class TransformWidget {
		private static readonly List<TransformWidgetAxis> AxesSet;
		public static readonly List<int> SelectedVertexIndices = new List<int>();
		public static Vertex SnappingVertex = null;

		public static bool IsHovered {
			get {
				foreach (TransformWidgetAxis axis in AxesSet) {
					if (axis.IsHovered) {
						return true;
					}
				}

				return false;
			}
		}

		public static bool IsActive {
			get {
				foreach (TransformWidgetAxis axis in AxesSet) {
					if (axis.IsActive) {
						return true;
					}
				}

				return false;
			}
		}

		static TransformWidget() {
			AxesSet = new List<TransformWidgetAxis> {
				new TransformWidgetAxis(Axis.X),
				new TransformWidgetAxis(Axis.Y),
				new TransformWidgetAxis(Axis.Z)
			};
		}

		public static void Update() {
			bool isVertexSelectMode = Gui.Widget == WidgetSelectionMode.PolygonVertexTranslate ||
			                          Gui.Widget == WidgetSelectionMode.PolygonEdgeTranslate;

			if (!isVertexSelectMode) {
				foreach (TransformWidgetAxis axis in AxesSet) {
					axis.Update();
				}
			} else {
				if (Gui.Widget == WidgetSelectionMode.PolygonVertexTranslate) {
					if (SelectedVertexIndices.Count != 1) {
						SelectedVertexIndices.Clear();
						SelectedVertexIndices.Add(0);
					}

					if (Selection.SelectedPolygons.Count > 0) {
						if (!Selection.SelectedPolygons[0].IsQuad && SelectedVertexIndices[0] == 3) {
							SelectedVertexIndices.Clear();
							SelectedVertexIndices.Add(0);
						}
					}
				}

				if (Gui.Widget == WidgetSelectionMode.PolygonEdgeTranslate) {
					if (SelectedVertexIndices.Count != 2) {
						SelectedVertexIndices.Clear();
						SelectedVertexIndices.Add(0);
						SelectedVertexIndices.Add(1);
					}

					if (Selection.SelectedPolygons.Count > 0) {
						if (!Selection.SelectedPolygons[0].IsQuad &&
						    (SelectedVertexIndices[0] == 3 || SelectedVertexIndices[1] == 3)
						) {
							SelectedVertexIndices.Clear();
							SelectedVertexIndices.Add(0);
							SelectedVertexIndices.Add(1);
						}
					}
				}

				if (Selection.SelectedPolygons.Count > 0) {
					Selection.UnselectAllExceptLast();
				}

				if (Selection.SelectedPolygons.Count == 1) {
					foreach (TransformWidgetAxis axis in AxesSet) {
						axis.Update();
					}
				}
			}

			SetOnlyOneAxisToHovered();
		}

		public static void Render() {
			if (Stage.ScreenshotMode ||
			    Selection.SelectedPolygons.Count == 0 ||
			    Gui.Widget != WidgetSelectionMode.PolygonTranslate &&
			    Gui.Widget != WidgetSelectionMode.PolygonVertexTranslate &&
			    Gui.Widget != WidgetSelectionMode.PolygonEdgeTranslate
			) {
				return;
			}

			List<TransformWidgetAxis> orderedAxes = AxesSet.OrderByDescending(axis => axis.DistanceToCamera).ToList();
			foreach (TransformWidgetAxis axis in orderedAxes) {
				axis.Render();
			}
		}

		private static void SetOnlyOneAxisToHovered() {
			List<TransformWidgetAxis> hoveredAxes = new List<TransformWidgetAxis>();
			foreach (TransformWidgetAxis axis in AxesSet) {
				if (axis.IsHovered) {
					hoveredAxes.Add(axis);
				}
			}

			if (hoveredAxes.Count > 1) {
				hoveredAxes = hoveredAxes.OrderBy(axis => axis.DistanceToCamera).ToList();
			}

			for (int axisIndex = 1; axisIndex < hoveredAxes.Count; axisIndex++) {
				hoveredAxes[axisIndex].HoveredAxisResults = new CameraRayResults { HasHit = false };
			}
		}

		public static void SelectNextVertex(bool reverse) {
			if (Selection.SelectedPolygons[0].IsQuad) {
				List<int> vertexOrder = new List<int> { 0, 1, 3, 2 };
				int currentIndex = vertexOrder.IndexOf(SelectedVertexIndices[0]);

				int newIndex = reverse
					? currentIndex - 1
					: currentIndex + 1;

				if (newIndex < 0) {
					newIndex = vertexOrder.Count - 1;
				}

				if (newIndex > vertexOrder.Count - 1) {
					newIndex = 0;
				}

				SelectedVertexIndices[0] = vertexOrder[newIndex];
			} else {
				int lastIndex = Selection.SelectedPolygons[0].Vertices.Count - 1;
				if (reverse) {
					SelectedVertexIndices[0]--;
				} else {
					SelectedVertexIndices[0]++;
				}

				if (SelectedVertexIndices[0] > lastIndex) {
					SelectedVertexIndices[0] = 0;
				}

				if (SelectedVertexIndices[0] < 0) {
					SelectedVertexIndices[0] = lastIndex;
				}
			}
		}

		public static void SelectNextEdge(bool reverse) {
			if (Selection.SelectedPolygons[0].IsQuad) {
				SelectedVertexIndices.Sort();

				List<int[]> coordinates = new List<int[]> {
					new[] { 0, 1 }, new[] { 1, 3 }, new[] { 2, 3 }, new[] { 0, 2 }, new[] { 0, 3 }, new[] { 1, 2 }
				};

				int currentIndex = 0;
				for (int coordinateIndex = 0; coordinateIndex < coordinates.Count; coordinateIndex++) {
					int[] coordinate = coordinates[coordinateIndex];
					if (SelectedVertexIndices[0] == coordinate[0] &&
					    SelectedVertexIndices[1] == coordinate[1]
					) {
						currentIndex = coordinateIndex;
						break;
					}
				}

				int newIndex = reverse
					? currentIndex - 1
					: currentIndex + 1;

				if (newIndex > coordinates.Count - 1) {
					newIndex = 0;
				}

				if (newIndex < 0) {
					newIndex = coordinates.Count - 1;
				}

				SelectedVertexIndices[0] = coordinates[newIndex][0];
				SelectedVertexIndices[1] = coordinates[newIndex][1];
			} else {
				int lastIndex = Selection.SelectedPolygons[0].Vertices.Count - 1;

				if (reverse) {
					SelectedVertexIndices[0]--;
					SelectedVertexIndices[1]--;
				} else {
					SelectedVertexIndices[0]++;
					SelectedVertexIndices[1]++;
				}

				if (SelectedVertexIndices[0] > lastIndex) {
					SelectedVertexIndices[0] = 0;
				}

				if (SelectedVertexIndices[1] > lastIndex) {
					SelectedVertexIndices[1] = 0;
				}

				if (SelectedVertexIndices[0] < 0) {
					SelectedVertexIndices[0] = lastIndex;
				}

				if (SelectedVertexIndices[1] < 0) {
					SelectedVertexIndices[1] = lastIndex;
				}
			}
		}

		public static void SelectSnappingVertex() {
			if (Gui.Widget != WidgetSelectionMode.PolygonVertexTranslate ||
			    Selection.SelectedPolygons.Count == 0
			) {
				SnappingVertex = null;
			} else {
				Vertex selectedVertex = Selection.SelectedPolygons[0].Vertices[SelectedVertexIndices[0]];
				SnappingVertex = selectedVertex;
			}
		}

		public static void SnapSnappingVertex() {
			if (SnappingVertex != null &&
			    Gui.Widget == WidgetSelectionMode.PolygonVertexTranslate &&
			    Selection.SelectedPolygons.Count > 0
			) {
				Vertex selectedVertex = Selection.SelectedPolygons[0].Vertices[SelectedVertexIndices[0]];

				SnappingVertex.Position = new Vector3(
					selectedVertex.Position.X,
					selectedVertex.Position.Y,
					selectedVertex.Position.Z
				);
			}
		}
	}
}