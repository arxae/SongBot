using System;
using System.Reflection;

namespace SongBot
{
	using System.Linq;
	using Serilog;

	class Program
	{
		static void Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{SourceContext}] [{Level:u3}] - {Message}{NewLine}{Exception}",
					theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code)
				.CreateLogger();

			//Rpg.ContentManager.Initialize();

			//new Bot().StartBotAsync()
			//	.ConfigureAwait(false)
			//	.GetAwaiter()
			//	.GetResult();


			// Load into dictionary or list
			var interfaceType = typeof(Rpg.LocationServices.ILocationService);
			var res = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(s => s.GetTypes())
				.Where(t => t.GetInterfaces().Contains(interfaceType));

			// Find type that starts with the service name, and ends with service
			var innType = res.FirstOrDefault(c =>
				c.Name.StartsWith("Inn", StringComparison.OrdinalIgnoreCase)
				& c.Name.EndsWith("Service"));

			// Instantiate and run method
			var obj = (Rpg.LocationServices.ILocationService)Activator.CreateInstance(innType);
			obj.Test();

			Console.ReadKey();
		}
	}
}