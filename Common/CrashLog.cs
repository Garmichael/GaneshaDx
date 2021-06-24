using System;
using System.IO;
using System.Text;

namespace GaneshaDx.Common {
	public static class CrashLog {
		public static void Write(Exception exception) {
			const string logfile = "CrashLog.txt";

			FileStream stream = new FileStream(logfile, FileMode.Create);
			string stackTrace = exception.StackTrace;
			stackTrace = stackTrace != null
				? stackTrace.Replace("C:\\Users\\Garmy\\Documents\\GaneshaX\\", "...")
				: "No Stack Trace Data";

			using (StreamWriter writer = new StreamWriter(stream, Encoding.Default)) {
				writer.WriteLine(DateTime.Now + ": " + exception.Message);
				writer.WriteLine(stackTrace);
			}

			stream.Dispose();
		}
	}
}