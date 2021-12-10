using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ManyConsole;
using Mono.Options;
using SchemaZen.Library.Command;

namespace SchemaZen.console {
	internal class Compare : BaseCommand {
		public Compare()
			: base(
				"Compare", "CreateDiff two databases.") {
			HasRequiredOption(
				"source=",
				"Connection string to a database to compare.",
				o => Source = o);
			HasRequiredOption(
				"target=",
				"Connection string to a database to compare.",
				o => Target = o);
		}

		protected string Source { get; set; }
		protected string Target { get; set; }

		public override int Run(string[] remainingArguments) {
			//if (_debug) Debugger.Launch();
			if (!string.IsNullOrEmpty(ScriptPath)) {
				Console.WriteLine();
				if (!Overwrite && File.Exists(ScriptPath)) {
					var question = $"{ScriptPath} already exists - do you want to replace it";
					if (!ConsoleQuestion.AskYN(question)) {
						return 1;
					}

					Overwrite = true;
				}
			}

			var compareCommand = new CompareCommand {
				Source = Source,
				Target = Target,
				Verbose = Verbose,
				ScriptPath = ScriptPath,
				ObjectTypes = ObjectTypes,
				NoDependencies = NoDependencies,
				Overwrite = Verbose
			};

			try {
				return compareCommand.Execute() ? 1 : 0;
			} catch (Exception ex) {
				throw new ConsoleHelpAsException(ex.Message);
			}
		}
	}
}
