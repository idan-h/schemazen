using System;
using System.Diagnostics;
using ManyConsole;
using SchemaZen.Library;
using SchemaZen.Library.Command;
using SchemaZen.Library.Models;

namespace SchemaZen.console {
	public class Import : BaseCommand {
		public Import()
			: base(
				"Import", "Imports the data from the specified directory.") {
			HasRequiredOption(
				"d|importDir=",
				"The directory from which the data will be imported",
				o => ImportDir = o);
		}

		private Logger _logger;
		protected string ImportDir { get; set; }

		public override int Run(string[] remainingArguments) {
			_logger = new Logger(Verbose);

			var importCommand = new ImportCommand {
				ConnectionString = ConnectionString,
				DataDir = ImportDir,
				Logger = _logger,
				Overwrite = Overwrite
			};

			try {
				importCommand.Execute();
			} catch (BatchSqlFileException ex) {
				_logger.Log(TraceLevel.Info,
					$"{Environment.NewLine}Import completed with the following errors:");
				foreach (var e in ex.Exceptions) {
					_logger.Log(TraceLevel.Info,
						$"- {e.FileName.Replace("/", "\\")} (Line {e.LineNumber}):");
					_logger.Log(TraceLevel.Error, $" {e.Message}");
				}

				return -1;
			} catch (SqlFileException ex) {
				_logger.Log(TraceLevel.Info,
					$@"{Environment.NewLine}An unexpected SQL error occurred while executing scripts, and the process wasn't completed.
{ex.FileName.Replace("/", "\\")} (Line {ex.LineNumber}):");
				_logger.Log(TraceLevel.Error, ex.Message);
				return -1;
			} catch (Exception ex) {
				throw new ConsoleHelpAsException(ex.Message);
			}

			return 0;
		}
	}
}
