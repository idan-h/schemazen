using System;
using System.IO;
using SchemaZen.Library.Models;

namespace SchemaZen.Library.Command {
	public class CompareCommand : BaseCommand {
		public string Source { get; set; }
		public string Target { get; set; }
		public bool Verbose { get; set; }

		public bool Execute() {
			var sourceDb = new Database();
			var targetDb = new Database();
			sourceDb.Connection = Source;
			targetDb.Connection = Target;
			//sourceDb.NoDependencies = NoDependencies;
			//targetDb.NoDependencies = NoDependencies;
			Console.WriteLine("Loading source...");
			sourceDb.Load();
			Console.WriteLine("Loading target...");
			targetDb.Load();
			Console.WriteLine("Starting comparison...");
			var diff = sourceDb.Compare(targetDb, ObjectTypes);
			if (diff.IsDiff) {
				Console.WriteLine("Databases are different.");
				Console.WriteLine(diff.SummarizeChanges(Verbose));
				if (!string.IsNullOrEmpty(ScriptPath)) {
					Console.WriteLine();
					if (!Overwrite && File.Exists(ScriptPath)) {
						var message =
							$"{ScriptPath} already exists - set overwrite to true if you want to delete it";
						throw new InvalidOperationException(message);
					}

					File.WriteAllText(ScriptPath, diff.Script());
					Console.WriteLine(
						$"Script to make the databases identical has been created at {Path.GetFullPath(ScriptPath)}");
				}

				return true;
			}

			Console.WriteLine("Databases are identical.");
			return false;
		}
	}
}
