using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ManyConsole;
using Mono.Options;
using SchemaZen.Library;
using SchemaZen.Library.Command;
using SchemaZen.Library.Models;

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
			HasOption(
				"filterTypes=",
				"A comma separated list of the types that will not be scripted. Valid types: " +
				Database.ValidTypes,
				o => FilterTypes = o);
			HasOption(
				"onlyTypes=",
				"A comma separated list of the types that will only be scripted. Valid types: " +
				Database.ValidTypes,
				o => OnlyTypes = o);
		}

		protected string Source { get; set; }
		protected string Target { get; set; }
		protected string FilterTypes { get; set; }
		protected string OnlyTypes { get; set; }

		public override int Run(string[] remainingArguments) {
			var logger = new Logger(Verbose);

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
				NoDependencies = NoDependencies,
				Overwrite = Verbose
			};

			var filteredTypes = TypesHelper.HandleFilteredTypes(FilterTypes, OnlyTypes, logger);

			try
			{
				return compareCommand.Execute(filteredTypes) ? 1 : 0;
			} catch (Exception ex) {
				throw new ConsoleHelpAsException(ex.Message);
			}
		}
	}
}
