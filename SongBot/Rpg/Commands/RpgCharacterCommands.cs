namespace SongBot.Rpg.Commands
{
	using System.Linq;
	using System.Threading.Tasks;

	using DSharpPlus.CommandsNext;
	using DSharpPlus.CommandsNext.Attributes;

	[Group("rpg")]
	[Description("RPG Commands")]
	public class RpgCharacterCommands
	{
		[Command("register"), Description("Register yourself as a new character")]
		public async Task RegisterNewCharacter(CommandContext c, string raceName)
		{
			c.LogDebug($"Character registration received from {c.User.GetFullUsername()} (race: {raceName})");
			
			if (await c.RpgChannelGuard() == false) return;

			if (ContentManager.Races.ContainsKey(raceName) == false)
			{
				var validRaces = string.Join(", ", ContentManager.Races.Select(r => r.Value.RaceName));
				await c.RespondAsync(
					$":warning: {c.User.Mention}: \"{raceName}\" is not a valid race. Valid races are: {validRaces}");
				await c.Message.DeleteAsync();
			}

			using (var db = ContentManager.GetDb())
			{
				var players = db.GetPlayerTable();
				var player = new Player(c.User.Id, raceName, ContentManager.Config.StartingClass);
				players.Insert(player.DiscordId, player);
				c.LogDebug($"Added player character for {c.User.GetFullUsername()} ({c.User.Id})");
			}

			await c.RespondAsync($"{ContentManager.Config.StartingClass} {c.User.Mention} has joined the adventure!");
			await c.ConfirmMessage();

			c.LogDebug($"Character registration completed for {c.User.GetFullUsername()}");
		}

		[Command("delete"), Description("Unregister your character")]
		public async Task DeleteCharacter(CommandContext c)
		{
			c.LogDebug($"{c.User.GetFullUsername()} -> {nameof(DeleteCharacter)}");

			if (await c.RpgChannelGuard() == false) return;

			using (var db = ContentManager.GetDb())
			{
				var players = db.GetPlayerTable();

				if (players.Exists(p => p.DiscordId == c.User.Id) == false)
				{
					await c.RejectMessage();
					return;
				}

				players.Delete(p => p.DiscordId == c.User.Id);
			}

			await c.ConfirmMessage();
			await c.RespondAsync($"{c.User.Mention} has left the adventure! {ContentManager.EMOJI_FACE_SAD}");
		}
	}
}
