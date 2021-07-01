using System.Collections.Generic;
using System.Linq;
using GaneshaDx.Common;
using GaneshaDx.Environment;

namespace GaneshaDx.UserInterface.Widgets {
	public static class RotationWidget {
		private static readonly List<RotationWidgetAxis> AxesSet;
		public static bool AnchoredToVertex;
		public static int AnchoredVertex;

		public static bool IsHovered {
			get {
				foreach (RotationWidgetAxis axis in AxesSet) {
					if (axis.IsHovered) {
						return true;
					}
				}

				return false;
			}
		}

		static RotationWidget() {
			AxesSet = new List<RotationWidgetAxis> {
				new RotationWidgetAxis(Axis.X),
				new RotationWidgetAxis(Axis.Y),
				new RotationWidgetAxis(Axis.Z)
			};
		}


		public static void Update() {
			if (Gui.Widget == WidgetSelectionMode.PolygonRotate &&
			    Selection.SelectedPolygons.Count > 0
			) {
				foreach (RotationWidgetAxis axis in AxesSet) {
					axis.Update();
				}
			}

			SetOnlyClosestAxisToHovered();
		}

		public static void Render() {
			if (Gui.Widget != WidgetSelectionMode.PolygonRotate ||
			    Selection.SelectedPolygons.Count == 0
			) {
				return;
			}

			List<RotationWidgetAxis> orderedAxes = AxesSet.OrderByDescending(axis => axis.DistanceToCamera).ToList();

			foreach (RotationWidgetAxis axis in orderedAxes) {
				axis.Render();
			}
		}

		private static void SetOnlyClosestAxisToHovered() {
			List<RotationWidgetAxis> hoveredAxes = new List<RotationWidgetAxis>();
			foreach (RotationWidgetAxis axis in AxesSet) {
				if (axis.IsHovered) {
					hoveredAxes.Add(axis);
				}
			}

			if (hoveredAxes.Count > 1) {
				hoveredAxes = hoveredAxes.OrderBy(axis => axis.DistanceToCamera).ToList();
			}

			for (int axisIndex = 1; axisIndex < hoveredAxes.Count; axisIndex++) {
				hoveredAxes[axisIndex].RightButtonHoveredResults = new CameraRayResults {HasHit = false};
				hoveredAxes[axisIndex].LeftButtonHoveredResults = new CameraRayResults {HasHit = false};
			}
		}
	}
}