namespace SongBot.Rpg.Commands
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;

	using DSharpPlus.CommandsNext;
	using DSharpPlus.CommandsNext.Attributes;

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
		public async Task EnterPlace(CommandContext c, string placeName)
		{
			c.LogDebug($"{c.User.GetFullUsername()} -> {nameof(EnterPlace)} ({nameof(placeName)}: {placeName})");

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

				if (loc.Places.ContainsKey(placeName) == false)
				{
					Serilog.Log.ForContext<LocationCommands>().Error("[ENTER] Location service {sv} not found", placeName);
					return;
				}

				var implName = loc.Places[placeName].ServiceImpl;
				Type serviceType;
				try
				{
					serviceType = ContentManager.PlaceActionImplementations[implName];
				}
				catch
				{
					Serilog.Log.ForContext<LocationCommands>()
						.Error("[ENTER] Location service implementation {sv} not found", placeName);
					return;

				}

				var srv = (Places.IPlace) Activator.CreateInstance(serviceType);

				await srv.EnterLocation(c, loc.Places[placeName]);
			}
		}
	}
}