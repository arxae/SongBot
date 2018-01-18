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

			Rpg.ContentManager.Initialize();

			new Bot().StartBotAsync()
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult();
		}
	}
}