using System;
using DSharpPlus.Interactivity;

namespace SongBot.Rpg.Commands
{
	using System.Linq;
	using System.Threading.Tasks;

	using DSharpPlus.CommandsNext;
	using DSharpPlus.CommandsNext.Attributes;
	using DSharpPlus.Entities;

	using DSharpPlus.Interactivity;

	public class LocationCommands
	{
		[Command("look"), Description("Look at your surroundings")]
		public async Task LookLocation(CommandContext c)
		{
			await c.Message.DeleteAsync();

			using (var db = ContentManager.GetDb())
			{
				var players = db.GetPlayerTable();
				if (players.Exists(p => p.DiscordId == c.User.Id) == false) return;

				var player = players.FindOne(p => p.DiscordId == c.User.Id);
				var locEmbed = ContentManager.Locations[player.CurrentLocation].GetLocationEmbed();
				await c.RespondWithDmAsync(embed: locEmbed);
			}
		}

		[Command("travelto"), Description("Travel to a specific location")]
		public async Task TravelTo(CommandContext c, string destination)
		{
			using (var db = ContentManager.GetDb())
			{
				var players = db.GetPlayerTable();
				if (players.Exists(p => p.DiscordId == c.User.Id) == false) return;

				var player = players.FindOne(p => p.DiscordId == c.User.Id);
				var currLoc = ContentManager.Locations[player.CurrentLocation];

				if (currLoc.LocationLinks.Contains(destination, StringComparer.OrdinalIgnoreCase))
				{
					if (player.SetAction(ContentManager.RpgAction.Travel, destination) == ContentManager.RpgActionResult.CannotInterrupt)
					{
						await c.RespondWithDmAsync("Your current action cannot be interrupted");
						await c.Message.DeleteAsync();
						return;
					}

					players.Update(player.DiscordId, player);
				}
			}
		}
	}
}
