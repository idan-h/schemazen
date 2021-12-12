using Microsoft.Extensions.DependencyInjection;
using SchemaZen.Tests;

namespace Test; 

public class Startup {
	public void ConfigureServices(IServiceCollection services) {
		var dbHelper = new TestDbHelper(
			"server=localhost;database=master;User Id=sa;Password=P@ssw0rd;");
		services.AddSingleton(dbHelper);
	}
}
