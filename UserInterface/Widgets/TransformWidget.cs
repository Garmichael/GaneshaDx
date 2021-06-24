using System.Collections.Generic;
using System.Linq;
using GaneshaDx.Common;
using GaneshaDx.Environment;

namespace GaneshaDx.UserInterface.Widgets {
	public static class TransformWidget {
		private static readonly List<TransformWidgetAxis> AxesSet;
		public static readonly List<int> SelectedVertexIndices = new List<int>();

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
			if (Stage.WidgetSelectionMode == WidgetSelectionMode.PolygonTranslate) {
				foreach (TransformWidgetAxis axis in AxesSet) {
					axis.Update();
				}
			} else if (
				Stage.WidgetSelectionMode == WidgetSelectionMode.PolygonVertexTranslate ||
				Stage.WidgetSelectionMode == WidgetSelectionMode.PolygonEdgeTranslate
			) {
				if (
					Stage.WidgetSelectionMode == WidgetSelectionMode.PolygonVertexTranslate &&
					SelectedVertexIndices.Count == 0
				) {
					SelectedVertexIndices.Clear();
					SelectedVertexIndices.Add(0);
				}

				if (
					Stage.WidgetSelectionMode == WidgetSelectionMode.PolygonEdgeTranslate &&
					SelectedVertexIndices.Count < 2
				) {
					SelectedVertexIndices.Clear();
					SelectedVertexIndices.Add(0);
					SelectedVertexIndices.Add(1);
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
			if (Selection.SelectedPolygons.Count == 0 ||
			    Stage.WidgetSelectionMode != WidgetSelectionMode.PolygonTranslate &&
			    Stage.WidgetSelectionMode != WidgetSelectionMode.PolygonVertexTranslate &&
			    Stage.WidgetSelectionMode != WidgetSelectionMode.PolygonEdgeTranslate
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
				hoveredAxes[axisIndex].HoveredAxisResults = new CameraRayResults {HasHit = false};
			}
		}
	}
}