using DSharpPlus.Entities;

namespace SongBot.Rpg.Commands
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;

	using DSharpPlus.CommandsNext;
	using DSharpPlus.CommandsNext.Attributes;
	using DSharpPlus.Interactivity;

	public class LocationCommands
	{
		[Command("look"), Description("Look at your surroundings")]
		public async Task LookLocation(CommandContext c)
		{
			if (await c.RpgChannelGuard() == false) return;
			c.LogDebug($"{c.User.GetFullUsername()} -> {nameof(LookLocation)}");

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

				var locEmbed = ContentManager.Locations[player.CurrentLocation].GetLocationEmbed();

				await c.RespondAsync(c.User.Mention, embed: locEmbed);
				await c.ConfirmMessage();
			}
		}

		[Command("travelto"), Description("Travel to a specific location")]
		public async Task TravelToLocation(CommandContext c, string destination)
		{
			if (await c.RpgChannelGuard() == false) return;
			c.LogDebug($"{c.User.GetFullUsername()} -> {nameof(TravelToLocation)} (destination: {destination})");

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

				var currLoc = ContentManager.Locations[player.CurrentLocation];

				if (currLoc.LocationConnections.Contains(destination, StringComparer.OrdinalIgnoreCase))
				{
					var nextLoc = ContentManager.Locations[destination];

					player.CurrentLocation = nextLoc.LocationId;

					players.Update(player.DiscordId, player);

					await c.ConfirmMessage();
					await c.RespondAsync($"{c.User.Mention} arrived in {nextLoc.DisplayName}", embed: nextLoc.GetLocationEmbed());
				}
			}
		}

		[Command("enter"), Description("Enter a service location")]
		public async Task EnterServiceLocation(CommandContext c, string serviceName)
		{
			c.LogDebug($"{c.User.GetFullUsername()} -> {nameof(EnterServiceLocation)} (serviceName: {serviceName})");

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

				var loc = ContentManager.Locations[player.CurrentLocation];

				if (loc.Services.ContainsKey(serviceName) == false) return;

				await LocationServices.InnService.EnterInn(c, loc.Services[serviceName]);
			}
		}
	}
}
