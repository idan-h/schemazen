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
	public class Export : BaseCommand {
		public Export()
			: base(
				"Export", "Generate data files for the specified tables.") {
			HasRequiredOption(
				"d|exportDir=",
				"The directory in which the exported data will be stored",
				o => ExportDir = o);
			HasOption(
				"dataTables=",
				"A comma separated list of tables to export data from.",
				o => DataTables = o);
			HasOption(
				"dataTablesPattern=",
				"A regular expression pattern that matches tables to export data from.",
				o => DataTablesPattern = o);
			HasOption(
				"dataTablesExcludePattern=",
				"A regular expression pattern that exclude tables to export data from.",
				o => DataTablesExcludePattern = o);
			HasOption(
				"tableHint=",
				"Table hint to use when exporting data.",
				o => TableHint = o);
		}

		private Logger _logger;
		protected string ExportDir { get; set; }
		protected string DataTables { get; set; }
		protected string DataTablesPattern { get; set; }
		protected string DataTablesExcludePattern { get; set; }
		protected string TableHint { get; set; }

		public override int Run(string[] args) {
			_logger = new Logger(Verbose);

			if (!Overwrite && Directory.Exists(ExportDir)) {
				if (!ConsoleQuestion.AskYN(
					$"{ExportDir} already exists - do you want to replace it"))
					return 1;
				Overwrite = true;
			}

			var exportCommand = new ExportCommand {
				ConnectionString = ConnectionString,
				DataDir = ExportDir,
				Logger = _logger,
				Overwrite = Overwrite
			};

			if (string.IsNullOrWhiteSpace($"{DataTablesPattern}{DataTables}"))
				DataTablesPattern = ".";
			
			var namesAndSchemas = HandleDataTables(DataTables);

			try {
				exportCommand.Execute(namesAndSchemas, DataTablesPattern, DataTablesExcludePattern,
					TableHint);
			} catch (Exception ex) {
				throw new ConsoleHelpAsException(ex.Message);
			}

			return 0;
		}

		private Dictionary<string, string> HandleDataTables(string tableNames) {
			var dataTables = new Dictionary<string, string>();

			if (string.IsNullOrEmpty(tableNames))
				return dataTables;

			foreach (var value in tableNames.Split(',')) {
				var schema = "dbo";
				var name = value;
				if (value.Contains(".")) {
					schema = value.Split('.')[0];
					name = value.Split('.')[1];
				}

				dataTables[name] = schema;
			}

			return dataTables;
		}
	}
}
