namespace SongBot.Rpg.Commands
{
	using System.Linq;
	using System.Threading.Tasks;

	using DSharpPlus.CommandsNext;
	using DSharpPlus.CommandsNext.Attributes;
	using DSharpPlus.Entities;

	[Group("skill")]
	public class SkillCommands
	{
		//TODO: This is a test implementation
		[Command("use"), Aliases("u"), Description("Use a specific skill")]
		public async Task UseSkill(CommandContext c, string skillName, string target = null)
		{
			c.LogDebug($"{c.User.GetFullUsername()} -> {nameof(UseSkill)} (skillName = {skillName}, target = {target})");

			using (var db = ContentManager.GetDb())
			{
				var player = db.GetPlayer(c.User.Id);
				if (player == null)
				{
					c.LogDebug($"Player {c.User.GetFullUsername()} not found");
					await c.RejectMessage();
					return;
				}

				var loc = ContentManager.Locations[player.CurrentLocation];

				var roll = ContentManager.Rng.Next(0, 100);
				var foundItems = loc.Items.Where(i => i.SpotDifficulty < roll).ToList();

				var foundItemEmbed = new DiscordEmbedBuilder()
					.WithTitle("You spot the following items")
					.WithColor(DiscordColor.Black)
					.WithFooter("Respond with item number to inspect");

				var sb = new System.Text.StringBuilder();
				for (int i = 0; i < foundItems.Count; i++)
				{
					// Get actual items
					var actItem = ContentManager.Items[foundItems[i].Id];
					sb.AppendLine($"[{i}] {actItem.Name}");
				}

				foundItemEmbed.WithDescription(sb.ToString());

				await c.RespondAsync(embed: foundItemEmbed);
			}
		}
	}
}
