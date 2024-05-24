using System;
using System.IO;
using System.Text;

namespace GaneshaDx.Common;

public static class CrashLog {
	public static void Write(Exception exception) {
		const string logfile = "CrashLog.txt";

		FileStream stream = new(logfile, FileMode.Create);
		string stackTrace = exception.StackTrace;
		if (stackTrace == null) {
			stackTrace = "No Stacktrace Data";
		}
			
		using (StreamWriter writer = new(stream, Encoding.Default)) {
			writer.WriteLine(DateTime.Now + ": " + exception.Message);
			writer.WriteLine(stackTrace);
		}

		stream.Dispose();
	}
}