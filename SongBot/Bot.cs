namespace SongBot
{
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

			var tickTimer = new Timer(5000);
			tickTimer.Elapsed += TickTimer_ElapsedAsync;
			tickTimer.Start();
		}

		public async Task StartBotAsync()
		{
			await client.ConnectAsync();
			await Task.Delay(-1);
		}

		async void TickTimer_ElapsedAsync(object sender, ElapsedEventArgs e)
		{
			client.DebugLogger.LogMessage(LogLevel.Debug, nameof(Bot), "Processing timer tick", System.DateTime.Now);

			using (var db = ContentManager.GetDb())
			{
				var playerTable = db.GetPlayerTable();
				var players = playerTable.FindAll();
				var dscGuild = await client.GetGuildAsync(ContentManager.Config.SongDiscordGuildId);

				foreach (var p in players)
				{
					// Skip user if it's idle since it's not goind to do anything anyway
					if (p.CurrentAction == ContentManager.RpgAction.Idle) continue;

					// Check if player is online
					var member = await dscGuild.GetMemberAsync(p.DiscordId);
					if (member.Presence.Status == DSharpPlus.Entities.UserStatus.Offline) continue;

					// Reduce ticks by 1
					p.ActionTicksRemaining -= 1;

					if (p.ActionTicksRemaining <= 0)
					{
						client.DebugLogger.LogMessage(LogLevel.Debug, nameof(Bot),
							$"{p.DiscordId} is done with action \"{p.CurrentAction}\"", System.DateTime.Now);

						// Execute action

						// Set back to idle
						p.SetAction(ContentManager.RpgAction.Idle, null);
						playerTable.Update(p.DiscordId, p);
						continue;
					}

					playerTable.Update(p.DiscordId, p);
				}
			}
		}
	}
}
