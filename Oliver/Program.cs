using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Oliver {
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable", Justification = "Program class is not supposed to be static")]
	public class Program {
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Top level exception handler")]
		public static void Main(string[] args) {
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Information()
				.WriteTo.Debug()
				.WriteTo.File("logs.txt", rollingInterval: RollingInterval.Day)
				.CreateLogger();

			try {
				Log.Information("Starting Web Host");
				CreateHostBuilder(args).Build().Run();
			} catch (Exception ex) {
				Log.Fatal(ex, "Host terminated unexpectedly");
			} finally {
				Log.CloseAndFlush();
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.UseSerilog()
				.ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
	}
}
