using System;
using GaneshaDx.Common;

namespace GaneshaDx;

public static class Program {
	public const string Version = "0.97.0";

	[STAThread]
	private static void Main(string[] args) {
#if !DEBUG
			try {
#endif
		using Ganesha ganesha = new Ganesha(args);
		ganesha.Run();
#if !DEBUG
			} catch (Exception exception) {
				CrashLog.Write(exception);
			}
#endif
	}
}