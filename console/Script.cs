using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ManyConsole;
using SchemaZen.Library;
using SchemaZen.Library.Command;
using SchemaZen.Library.Models;

namespace SchemaZen.console {
	public class Script : BaseCommand {
		public Script()
			: base(
				"Script", "Generate scripts for the specified database.") {
			HasOption(
				"filterTypes=",
				"A comma separated list of the types that will not be scripted. Valid types: " +
				Database.ValidTypes,
				o => FilterTypes = o);
			HasOption(
				"excludedRoutines",
				"A comma seperated list of routine names to exclude.",
				o => ExcludedRoutines = o);
			HasOption(
				"onlyTypes=",
				"A comma separated list of the types that will only be scripted. Valid types: " +
				Database.ValidTypes,
				o => OnlyTypes = o);
			HasOption(
				"no-comments",
				"If true, produces a file with no section comments",
				o => NoComments = o != null);
		}

		protected string FilterTypes { get; set; }
		protected string OnlyTypes { get; set; }
		protected string ExcludedRoutines { get; set; }
		protected bool NoComments { get; set; }

		public override int Run(string[] args) {
			var logger = new Logger(Verbose);

			if (!Overwrite && Directory.Exists(ScriptPath)) {
				if (!ConsoleQuestion.AskYN(
					$"{ScriptPath} already exists - do you want to replace it"))
					return 1;
				Overwrite = true;
			}

			var scriptCommand = new ScriptCommand {
				ConnectionString = ConnectionString,
				ScriptPath = ScriptPath,
				NoDependencies = NoDependencies,
				Logger = logger,
				Comments = !NoComments,
				Overwrite = Overwrite
			};

			var excludedRoutines = ExcludedRoutines?.Split(',').Select(x => x.ToLower()).ToList() ?? new List<string>();
			var filteredTypes = TypesHelper.HandleFilteredTypes(FilterTypes, OnlyTypes, logger);

			try {
				scriptCommand.Execute(filteredTypes, excludedRoutines);
			} catch (Exception ex) {
				throw new ConsoleHelpAsException(ex.Message);
			}

			return 0;
		}
	}
}
