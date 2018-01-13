namespace SongBot.Rpg.Commands
{
	using System.Linq;
	using System.Threading.Tasks;

	using DSharpPlus.CommandsNext;
	using DSharpPlus.CommandsNext.Attributes;

	[Group("rpgchar")]
	[Description("RPG Commands")]
	public class RpgCharacterCommands
	{
		[Command("register"), Description("Register yourself as a new character")]
		public async Task RegisterNewCharacter(CommandContext c, string raceName, string className)
		{
			c.LogDebug($"New character requested for {c.User.GetFullUsername()} ({raceName} | {className})");
			await c.Channel.DeleteMessageAsync(c.Message);

			if (ContentManager.Races.ContainsKey(raceName) == false)
			{
				c.LogDebug($"{c.User.GetFullUsername()} used a wrong race ({raceName})");
				var validRaces = string.Join(", ", ContentManager.Races
					.Where(rce => rce.Value.IsPlayable)
					.Select(rce => rce.Key)
					.ToList());
				await c.RespondWithDmAsync($"{raceName} is not a valid race. Valid choices: {validRaces}");
				return;
			}

			if (ContentManager.Classes.ContainsKey(className) == false)
			{
				c.LogDebug($"{c.User.GetFullUsername()} used a wrong class ({className})");
				var validClasses = string.Join(", ", ContentManager.Classes
					.Where(cls => cls.Value.IsStarter)
					.Select(cls => cls.Key)
					.ToList());
				await c.RespondWithDmAsync($"{className} is not a valid class. Valid choices: {validClasses}");
				return;
			}

			var player = new Player
			{
				DiscordId = c.User.Id,
				Race = ContentManager.Races[raceName].RaceName,
				Class = ContentManager.Classes[className].ClassName
			};

			using (var db = ContentManager.GetDb())
			{
				var players = db.GetPlayerTable();
				if (players.Exists(p => p.DiscordId == player.DiscordId))
				{
					await c.Message.DeleteAsync();
					await c.RespondWithDmAsync("You already have registered a character. You can remove it by using the \"rpgchar unregister\" command");
					c.LogDebug($"{c.User.GetFullUsername()} already registered a character");
					return;
				}

				players.Insert(player.DiscordId, player);
				c.LogDebug($"Registered new player for {c.User.GetFullUsername()} ({player.DiscordId})");
			}

			await c.RespondAsync($"{player.Race} {player.Class} {c.User.Mention} has joined the realm!");
		}

		[Command("unregister"), Description("Unregister your character")]
		public async Task UnregisterCharacter(CommandContext c)
		{
			c.LogDebug($"Received \"rpg unregister\" from {c.User.GetFullUsername()}");

			await c.Channel.DeleteMessageAsync(c.Message);
			using (var db = ContentManager.GetDb())
			{
				var players = db.GetPlayerTable();

				if (players.Exists(p => p.DiscordId == c.User.Id) == false)
				{
					await c.Channel.DeleteMessageAsync(c.Message);
					return;
				}

				players.Delete(p => p.DiscordId == c.User.Id);
			}

			await c.RespondWithDmAsync("You are no longer part of the realm");
		}
	}
}
