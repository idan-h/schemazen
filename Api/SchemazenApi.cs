using SchemaZen.Library;
using SchemaZen.Library.Command;
using System;

namespace Api
{
    public class SchemazenApi
    {
		public static void Import(string connectionString,
								  string importDir,
								  ILogger logger = null,
								  bool overwrite = false)
		{
			logger ??= new Logger(true);

			var importCommand = new ImportCommand
			{
				ConnectionString = connectionString,
				DataDir = importDir,
				Logger = logger,
				Overwrite = overwrite,
			};

			importCommand.Execute();
		}

		public static void Create(string connectionString,
								  string scriptPath,
								  ILogger logger = null,
								  bool overwrite = false,
								  string databaseFilesPath = null)
		{
			logger ??= new Logger(true);

			var createCommand = new CreateCommand
			{
				ConnectionString = connectionString,
				ScriptPath = scriptPath,
				Logger = logger,
				Overwrite = overwrite
			};

			createCommand.Execute(databaseFilesPath);
		}
	}
}
