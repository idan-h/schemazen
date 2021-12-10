namespace SchemaZen.Library.Models {
	public class Permission : IScriptable, INameable {
		public string Name { get; set; }
		public string UserName { get; set; }
		public string ObjectName { get; set; }
		public string PermissionType { get; set; }
		public string StateDescription { get; set; }


		public Permission(string userName, string objectName, string permissionType, string stateDesc) {
			Name = $"{userName}___{objectName}___{permissionType}___{stateDesc}";
			UserName = userName;
			ObjectName = objectName;
			PermissionType = permissionType;
			StateDescription = stateDesc;
		}

		public string ScriptCreate() {
			var on = string.IsNullOrEmpty(ObjectName) ? "" : $"ON [{ObjectName}]";
			return $@"{StateDescription} {PermissionType} {on} TO [{UserName}]";
		}

		public string ScriptDrop() {
			var state = StateDescription == "GRANT" ? "REVOKE" : "GRANT";
			var on = string.IsNullOrEmpty(ObjectName) ? "" : $"ON [{ObjectName}]";
			return $@"{state} {PermissionType} {on} TO [{UserName}]";
		}
	}
}
