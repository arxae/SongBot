namespace SongBot.Rpg.Commands
{
	using System.Threading.Tasks;

	using DSharpPlus.CommandsNext;
	using DSharpPlus.CommandsNext.Attributes;
	using DSharpPlus.Entities;

	[Group("char")]
	public class CharacterCommands
	{
		[Command("status")]
		public async Task CharacterStatus(CommandContext c)
		{
			if (await c.RpgChannelGuard() == false) return;
			c.LogDebug($"{c.User.GetFullUsername()} -> {nameof(CharacterStatus)}");

			using (var db = ContentManager.GetDb())
			{
				var players = db.GetPlayerTable();
				var player = players.FindOne(p => p.DiscordId == c.User.Id);

				if (player == null)
				{
					c.LogDebug($"Player {c.User.GetFullUsername()} not found");
					await c.RejectMessage();
					return;
				}

				var embedBuilder = new DiscordEmbedBuilder()
					.WithTitle($"**{c.User.Username} the {player.Class}**")
					.WithColor(DiscordColor.DarkGray);

				var status = new System.Text.StringBuilder();
				status.AppendLine("**Status**");
				status.AppendLine("**Level:**")
				status.AppendLine($"**- Hp:** {player.HpCurrent}/{player.HpMax}");

				embedBuilder.WithDescription(status.ToString());

				await c.RespondAsync(embed: embedBuilder);
				await c.ConfirmMessage();
			}
		}
	}
}
