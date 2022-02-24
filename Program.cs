using System;
using GaneshaDx.Common;

namespace GaneshaDx {
	public static class Program {
		public static readonly string Version = "0.86 (alpha)";
		
		[STAThread]
		private static void Main() {
#if !DEBUG
			try {
#endif
				using Ganesha ganesha = new Ganesha();
				ganesha.Run();
#if !DEBUG
			} catch (Exception exception) {
				CrashLog.Write(exception);
			}
#endif
		}
	}
}