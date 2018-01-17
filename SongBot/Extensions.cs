namespace SongBot
{
	using System.Threading.Tasks;

	using DSharpPlus;
	using DSharpPlus.CommandsNext;
	using DSharpPlus.Entities;

	using Rpg;

	public static class Extensions
	{
		// Logging
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

		// Discord Stuff
		public static bool IsRpgChannel(this CommandContext c) => c.Channel.Id == ContentManager.RpgChannel.Id;

		/// <summary>
		/// Returns true if it's the correct channel, false otherwise
		/// </summary>
		/// <param name="c"></param>
		/// <returns>True if it the channel is correct, false if not</returns>
		public static async Task<bool> RpgChannelGuard(this CommandContext c)
		{
			if (c.IsRpgChannel()) return true;

			await c.RespondWithDmAsync($"RPG commands can only be used in the {ContentManager.RpgChannel.Mention} channel.");
			await c.Message.DeleteAsync();

			return false;
		}

		public static async Task ConfirmMessage(this CommandContext c) => await c.Message.CreateReactionAsync(DiscordEmoji.FromName(c.Client, ContentManager.EMOJI_GREEN_CHECK));
		public static async Task RejectMessage(this CommandContext c) => await c.Message.CreateReactionAsync(DiscordEmoji.FromName(c.Client, ContentManager.EMOJI_RED_CROSS));

		public static async Task<DiscordMessage> RespondWithDmAsync(this CommandContext c, string content = null, bool isTTS = false, DiscordEmbed embed = null)
		{
			if (ContentManager.Config.NeverDmResults)
			{
				return await c.Channel.SendMessageAsync(content, isTTS, embed);
			}

			if (c.Channel.Type == ChannelType.Private)
			{
				return await c.RespondAsync(content, isTTS, embed);
			}

			return await c.Member.SendMessageAsync(content, isTTS, embed);
		}

		// DB Stuff
		public static string GetFullUsername(this DiscordUser user) => $"{user.Username}#{user.Discriminator}";
		public static LiteDB.LiteCollection<Player> GetPlayerTable(this LiteDB.LiteDatabase db) => db.GetCollection<Player>(ContentManager.DB_PLAYER_TABLE);

		public static Player GetPlayer(this LiteDB.LiteDatabase db, ulong playerId)
		{
			var playerTable = db.GetPlayerTable();
			return playerTable.FindOne(p => p.DiscordId == playerId);
		}
	}
}
