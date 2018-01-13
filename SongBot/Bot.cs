namespace SongBot
{
	using System.Timers;
	using System.Threading.Tasks;

	using DSharpPlus;
	using DSharpPlus.CommandsNext;
	using DSharpPlus.Interactivity;

	public class Bot
	{
		readonly DiscordClient client;

		public Bot()
		{
			client = new DiscordClient(new DiscordConfiguration
			{
				Token = System.Environment.GetEnvironmentVariable("SONGBOT_KEY", System.EnvironmentVariableTarget.User),
				TokenType = TokenType.Bot,
				UseInternalLogHandler = true,
				LogLevel = LogLevel.Debug
			});

			var commands = client.UseCommandsNext(new CommandsNextConfiguration()
			{
				StringPrefixes = new[] { ".", "!" },
				CaseSensitive = false
			});

			client.UseInteractivity(new InteractivityConfiguration());
			commands.RegisterCommands(System.Reflection.Assembly.GetExecutingAssembly());

			var tickTimer = new Timer(60000);
			tickTimer.Elapsed += TickTimer_Elapsed;
			tickTimer.Start();
		}

		public async Task StartBotAsync()
		{
			await client.ConnectAsync();
			await Task.Delay(-1);
		}

		void TickTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			client.DebugLogger.LogMessage(LogLevel.Debug, nameof(Bot), "Processing timer tick", System.DateTime.Now);
		}
	}
}
