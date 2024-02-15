using System;
using System.Data.SqlClient;
using System.IO;
using SchemaZen.Library.Models;

namespace SchemaZen.Library {
	public class DBHelper {
		public static bool EchoSql { get; set; } = false;

		public static void ExecSql(string conn, string sql , int? timeout = null) {
			if (EchoSql) Console.WriteLine(sql);
			using (var cn = new SqlConnection(conn)) {
				cn.Open();
				using (var cm = cn.CreateCommand()) {
					cm.CommandText = sql;
					if (timeout != null)
					{
						cm.CommandTimeout = timeout.Value;
					}
					cm.ExecuteNonQuery();
				}
			}
		}

		public static void ExecBatchSql(string conn, string sql, int? timeout = null) {
			var prevLines = 0;
			using (var cn = new SqlConnection(conn)) {
				cn.Open();
				using (var cm = cn.CreateCommand()) {
					foreach (var script in BatchSqlParser.SplitBatch(sql)) {
						if (EchoSql) Console.WriteLine(script);
						cm.CommandText = script;
						if (timeout != null)
						{
							cm.CommandTimeout = timeout.Value;
						}
						try {
							cm.ExecuteNonQuery();
						} catch (SqlException ex) {
							throw new SqlBatchException(ex, prevLines);
						}

						prevLines += script.Split('\n').Length;
						prevLines += 1; // add one line for GO statement
					}
				}
			}
		}

		public static void DropDb(string conn) {
			var cnBuilder = new SqlConnectionStringBuilder(conn);
			var initialCatalog = cnBuilder.InitialCatalog;

			var dbName = "[" + initialCatalog + "]";

			if (DbExists(cnBuilder.ToString())) {
				cnBuilder.InitialCatalog = "master";
				ExecSql(cnBuilder.ToString(),
					"ALTER DATABASE " + dbName + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
				ExecSql(cnBuilder.ToString(), "drop database " + dbName);

				cnBuilder.InitialCatalog = initialCatalog;
				ClearPool(cnBuilder.ToString());
			}
		}

		public static void CreateDb(string connection, string databaseFilesPath = null, string extraQuery = null) {
			var cnBuilder = new SqlConnectionStringBuilder(connection);
			var dbName = cnBuilder.InitialCatalog;
			cnBuilder.InitialCatalog = "master";

			var files = string.Empty;
			if (databaseFilesPath != null) {
				Directory.CreateDirectory(databaseFilesPath);
				files = $@"ON 
(NAME = '{dbName}',
    FILENAME = '{databaseFilesPath}\{dbName + Guid.NewGuid()}.mdf')
LOG ON
(NAME = '{dbName}_log',
    FILENAME =  '{databaseFilesPath}\{dbName + Guid.NewGuid()}.ldf')";
			}

			ExecSql(cnBuilder.ToString(), $"CREATE DATABASE [{dbName}] {files} {extraQuery}", 900);
		}

		public static bool DbExists(string conn) {
			var cnBuilder = new SqlConnectionStringBuilder(conn);
			var dbName = cnBuilder.InitialCatalog;
			cnBuilder.InitialCatalog = "master";

			using var cn = new SqlConnection(cnBuilder.ToString());
			using var cm = cn.CreateCommand();

			cm.CommandText = "SELECT database_id FROM sys.databases WHERE name = '" + dbName + "'";

			cn.Open();
			var result = cm.ExecuteScalar();

			if (result == null)
			{
				return false;
			}

			return !ReferenceEquals(result, DBNull.Value);
		}

		public static void ClearPool(string conn) {
			using (var cn = new SqlConnection(conn)) {
				SqlConnection.ClearPool(cn);
			}
		}
	}
}
