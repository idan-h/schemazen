using SchemaZen.Library;
using SchemaZen.Library.Command;
using System;

namespace Api
{
    public class SchemazenApi
    {
		public static void Import(string connectionString, string importDir, ILogger logger, bool overwrite = false)
		{
			var importCommand = new ImportCommand
			{
				ConnectionString = connectionString,
				DataDir = importDir,
				Logger = logger,
				Overwrite = overwrite
			};

			importCommand.Execute();
		}
	}
}
