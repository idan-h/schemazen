using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SchemaZen.Library.Command {
	public class ScriptCommand : BaseCommand {
		public bool Comments { get; set; } = true;

		public void Execute(List<string> filteredTypes, List<string> excludedRoutines) {
			if (!Overwrite && File.Exists(ScriptPath)) {
				var message = $"{ScriptPath} already exists - you must set overwrite to true";
				throw new InvalidOperationException(message);
			}

			var db = CreateDatabase(filteredTypes);
			db.NoDependencies = NoDependencies;

			Logger.Log(TraceLevel.Info, "Loading database schema...");
			db.Load();
			Logger.Log(TraceLevel.Info, "Database schema loaded.");

			db.ScriptToDir(Logger.Log, Comments, excludedRoutines);

			Logger.Log(TraceLevel.Info,
				$"{Environment.NewLine}Snapshot successfully created at {db.ScriptPath}");
			var routinesWithWarnings = db.Routines.Select(r => new {
				Routine = r,
				Warnings = r.Warnings().ToList()
			}).Where(r => r.Warnings.Any()).ToList();
			if (routinesWithWarnings.Any()) {
				Logger.Log(TraceLevel.Info, "With the following warnings:");
				var warnings = routinesWithWarnings.SelectMany(r => r.Warnings.Select(
					w => $"- {r.Routine.RoutineType} [{r.Routine.Owner}].[{r.Routine.Name}]: {w}"));
				foreach (var warning in warnings) {
					Logger.Log(TraceLevel.Warning, warning);
				}
			}
		}
	}
}
