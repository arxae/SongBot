namespace SongBot.Rpg.Commands
{
	using System.Linq;
	using System.Threading.Tasks;

	using DSharpPlus.CommandsNext;
	using DSharpPlus.CommandsNext.Attributes;
	using LiteDB;

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

				var skillResult = player.SetSkillAction("perception", new System.Collections.Generic.Dictionary<string, object>
				{
					{"locationid",player.CurrentLocation }
				});

				if (skillResult == false)
				{
					await c.RejectMessage();
					return;
				}

				db.GetPlayerTable().Update(player);
			}
		}
	}
}
