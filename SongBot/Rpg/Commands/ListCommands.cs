﻿namespace SongBot.Rpg.Commands
{
	using System.Threading.Tasks;

	using DSharpPlus.CommandsNext;
	using DSharpPlus.CommandsNext.Attributes;
	using DSharpPlus.Entities;

	[Group("list"), RequireRoles(RoleCheckMode.All, "Realm Admin")]
	public class ListCommands
	{
		[Command("races")]
		public async Task ListRaces(CommandContext c)
		{
			var sb = new System.Text.StringBuilder();
			sb.Append("**Available races**: ");
			sb.Append(string.Join(", ", ContentManager.Races.Keys));

			await c.Message.CreateReactionAsync(DiscordEmoji.FromName(c.Client, ContentManager.EMOJI_GREEN_CHECK));
			await c.RespondWithDmAsync(sb.ToString());
		}
	}
}
