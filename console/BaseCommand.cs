using ManyConsole;
using Mono.Options;
using System.Linq;

namespace SchemaZen.console {
	public abstract class BaseCommand : ConsoleCommand {
		protected BaseCommand(string command, string oneLineDescription) {
			IsCommand(command, oneLineDescription);
			Options = new OptionSet();
			SkipsCommandSummaryBeforeRunning();

			HasOption("c|connectionString=", "connection string", o => ConnectionString = o);
			HasOption(
				"f|scriptPath=",
				"Path to database script file.",
				o => ScriptPath = o);
			HasOption(
				"o|overwrite",
				"Overwrite existing target without prompt.",
				o => Overwrite = o != null);
			HasOption(
				"v|verbose",
				"Enable verbose log messages.",
				o => Verbose = o != null);
			HasOption(
				"df|databaseFilesPath=",
				"Path to database data and log files.",
				o => DatabaseFilesPath = o);
			HasOption(
				"no-depends",
				"Cancels the routines dependency collection - makes the db loading faster",
				o => NoDependencies = o != null);
		}

		protected string ConnectionString { get; set; }
		protected string ScriptPath { get; set; }
		protected bool Overwrite { get; set; }
		protected bool Verbose { get; set; }
		protected string DatabaseFilesPath { get; set; }
		protected bool NoDependencies { get; set; }
	}
}
