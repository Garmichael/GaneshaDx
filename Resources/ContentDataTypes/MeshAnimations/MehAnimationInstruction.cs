using System.Collections.Generic;
using GaneshaDx.Common;

namespace GaneshaDx.Resources.ContentDataTypes.MeshAnimations {
	public class MeshAnimationInstruction {
		public Dictionary<Axis, int> Rotation = new Dictionary<Axis, int>();
		public Dictionary<Axis, int> Position = new Dictionary<Axis, int>();
		public Dictionary<Axis, int> Scale = new Dictionary<Axis, int>();
	}
}