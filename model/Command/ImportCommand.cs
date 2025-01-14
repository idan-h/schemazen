using System;
using System.Diagnostics;
using System.IO;

namespace SchemaZen.Library.Command {
	public class ImportCommand : BaseCommand {
		public void Execute(TimeSpan? timeout = null) {
			var db = CreateDatabase();
			db.DataDir = DataDir;

			if (!Directory.Exists(db.DataDir)) {
				throw new FileNotFoundException($"Directory {db.DataDir} does not exist.");
			}

			db.ImportData(Logger.Log, Overwrite, timeout);
			Logger.Log(TraceLevel.Info, $"{Environment.NewLine}Data imported successfully.");
		}
	}
}
