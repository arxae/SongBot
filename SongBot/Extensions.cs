namespace SongBot
{
	using System.Threading.Tasks;

	using DSharpPlus;
	using DSharpPlus.CommandsNext;
	using DSharpPlus.Entities;

	using Rpg;

	public static class Extensions
	{
		static void Log(this CommandContext c, LogLevel level, string application, string message)
		{
			if (application == null)
			{
				application = "SongBot";
			}

			c.Client.DebugLogger.LogMessage(level, application, message, System.DateTime.Now);
		}

		public static void LogCritical(this CommandContext c, string message, string application = null) => c.Log(LogLevel.Critical, application, message);
		public static void LogDebug(this CommandContext c, string message, string application = null) => c.Log(LogLevel.Debug, application, message);
		public static void LogInfo(this CommandContext c, string message, string application = null) => c.Log(LogLevel.Info, application, message);
		public static void LogWarning(this CommandContext c, string message, string application = null) => c.Log(LogLevel.Warning, application, message);

		public static async Task<DiscordMessage> RespondWithDmAsync(this CommandContext c, string content = null, bool isTTS = false, DiscordEmbed embed = null)
		{
			if (c.Channel.Type == ChannelType.Private)
			{
				return await c.RespondAsync(content, isTTS, embed);
			}

			return await c.Member.SendMessageAsync(content, isTTS, embed);
		}

		public static string GetFullUsername(this DiscordUser user) => $"{user.Username}#{user.Discriminator}";
		public static LiteDB.LiteCollection<Player> GetPlayerTable(this LiteDB.LiteDatabase db) => db.GetCollection<Player>(ContentManager.DB_PLAYER_TABLE);
	}
}
