using SchemaZen.Library;
using SchemaZen.Library.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchemaZen.console
{
	public static class TypesHelper
	{
		public static List<string> HandleFilteredTypes(string filterTypes, string onlyTypes, ILogger logger)
		{
			var removeTypes = filterTypes?.Split(',').ToList() ?? new List<string>();
			var keepTypes = onlyTypes?.Split(',').ToList() ?? new List<string>(Database.Dirs);

			var invalidTypes = removeTypes.Union(keepTypes).Except(Database.Dirs).ToList();
			if (invalidTypes.Any())
			{
				var msg = invalidTypes.Count() > 1 ? " are not valid types." :
					" is not a valid type.";
				logger.Log(TraceLevel.Warning, string.Join(", ", invalidTypes.ToArray()) + msg);
				logger.Log(TraceLevel.Warning, $"Valid types: {Database.ValidTypes}");
			}

			return Database.Dirs.Except(keepTypes.Except(removeTypes)).ToList();
		}
	}
}
