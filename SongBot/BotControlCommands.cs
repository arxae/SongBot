namespace SongBot
{
	using System.Threading.Tasks;

	using DSharpPlus.CommandsNext;
	using DSharpPlus.CommandsNext.Attributes;

	/// <summary>
	/// Extra bot functionality
	/// </summary>
	public class BotControlCommands
	{
		[Command("leave"), RequireOwner]
		public async Task Leave(CommandContext c)
		{
			c.LogCritical($"Leave requested by {c.User.GetFullUsername()}");
			await c.Client.DisconnectAsync();
		}

		[Command("purge")]
		public async Task BurnMessages(CommandContext c, int rows)
		{
			var msgs = await c.Channel.GetMessagesBeforeAsync(c.Message, rows);
			await c.Channel.DeleteMessagesAsync(msgs);
			await c.Channel.DeleteMessageAsync(c.Message);
			c.LogWarning($"{c.User.Username}#{c.User.Discriminator} purged {msgs.Count + 1} messages.");
		}
	}
}
