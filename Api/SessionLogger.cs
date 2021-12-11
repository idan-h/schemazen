using SchemaZen.Library;
using System.Collections.Generic;
using System.Diagnostics;

namespace Api
{
	public class SessionLogger : ILogger
	{
		private readonly List<string> _logs = new List<string>();
		public string[] GetLogs() => _logs.ToArray();
		public void Log(TraceLevel level, string message) => _logs.Add(message);
	}
}
