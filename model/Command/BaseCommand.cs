using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SchemaZen.Library.Models;

namespace SchemaZen.Library.Command {
	public abstract class BaseCommand {
		public string ConnectionString { get; set; }
		public string ScriptPath { get; set; }
		public string DataDir { get; set; }
		public ILogger Logger { get; set; }
		public bool Overwrite { get; set; }
		public string[] ObjectTypes { get; set; }
		public bool NoDependencies { get; set; }

		public Database CreateDatabase(IList<string> filteredTypes = null) {
			filteredTypes ??= new List<string>();

			if (!string.IsNullOrEmpty(ConnectionString)) {
				return new Database(filteredTypes) {
					Connection = ConnectionString,
					ScriptPath = ScriptPath
				};
			}

			throw new ArgumentException(
				"You must provide a connection string, or a server and database name");
		}

		public void AddDataTable(Database db, string name, string schema) {
			var t = db.FindTable(name, schema);
			if (t == null) {
				Console.WriteLine($"warning: could not find data table {schema}.{name}");
			}

			if (db.DataTables.Contains(t)) return;
			db.DataTables.Add(t);
		}
	}
}
