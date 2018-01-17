using System;

namespace SongBot
{
	using System.Linq;
	using System.Timers;
	using System.Threading.Tasks;

	using DSharpPlus;
	using DSharpPlus.CommandsNext;
	using DSharpPlus.Interactivity;

	using Rpg;

	public class Bot
	{
		readonly DiscordClient client;

		public Bot()
		{
			client = new DiscordClient(new DiscordConfiguration
			{
				Token = Environment.GetEnvironmentVariable("SONGBOT_KEY", EnvironmentVariableTarget.User),
				TokenType = TokenType.Bot,
				LogLevel = LogLevel.Debug
			});

			client.DebugLogger.LogMessageReceived += DebugLogger_LogMessageReceived;
			client.GuildAvailable += Client_GuildAvailable;

			var commands = client.UseCommandsNext(new CommandsNextConfiguration()
			{
				StringPrefixes = new[] { ".", "!" },
				CaseSensitive = false
			});

			client.UseInteractivity(new InteractivityConfiguration());
			commands.RegisterCommands(System.Reflection.Assembly.GetExecutingAssembly());
		}

		private Task Client_GuildAvailable(DSharpPlus.EventArgs.GuildCreateEventArgs e)
		{
			return Task.Run(() =>
			{
				if (e.Guild.Id != ContentManager.Config.SongDiscordGuildId) return;

				var rpgChannel = e.Guild.Channels.FirstOrDefault(c => c.Name == ContentManager.Config.RpgChannel);
				ContentManager.RpgChannel = rpgChannel ?? throw new ArgumentException("Could not find the RPG channel");
			});
		}

		void DebugLogger_LogMessageReceived(object sender, DSharpPlus.EventArgs.DebugLogMessageEventArgs e)
		{
			var l = Serilog.Log.ForContext("SourceContext", e.Application);

			switch (e.Level)
			{
				case LogLevel.Debug: l.Debug(e.Message); break;
				case LogLevel.Info: l.Information(e.Message); break;
				case LogLevel.Warning: l.Warning(e.Message); break;
				case LogLevel.Error: l.Error(e.Message); break;
				case LogLevel.Critical: l.Fatal(e.Message); break;
				default: throw new ArgumentOutOfRangeException();
			}
		}

		public async Task StartBotAsync()
		{
			await client.ConnectAsync();
			await Task.Delay(-1);
		}
	}
}
