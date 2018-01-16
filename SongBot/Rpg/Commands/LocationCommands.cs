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
					var res = player.SetAction(ContentManager.RpgAction.Travel, destination);

					if (res == ContentManager.RpgActionResult.CannotInterrupt)
					{
						await c.RespondWithDmAsync("Your current action cannot be interrupted");
						await c.Message.DeleteAsync();
						return;
					}

					if (res == ContentManager.RpgActionResult.DoneInstant)
					{
						player.ExecuteCurrentAction();
					}

					players.Update(player.DiscordId, player);

					await c.ConfirmMessage();
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

				if (loc.Services.Contains(serviceName) == false) return;

				var desc = new System.Text.StringBuilder();
				desc.AppendLine("A cozy looking inn, you could get some beer and shit");
				desc.AppendLine(":beer: Drink a beer");
				desc.AppendLine(":beers: Buy a beer for everyone currently here");
				desc.AppendLine(":bed: Take a rest for the night");

				var embed = new DiscordEmbedBuilder()
					.WithColor(DiscordColor.Blurple)
					.WithTitle("Inn")
					.WithDescription(desc.ToString());

				var msg = await c.RespondAsync(embed: embed);

				await msg.CreateReactionAsync(DiscordEmoji.FromName(c.Client, ":beer:"));
				await msg.CreateReactionAsync(DiscordEmoji.FromName(c.Client, ":beers:"));
				await msg.CreateReactionAsync(DiscordEmoji.FromName(c.Client, ":bed:"));
				await c.ConfirmMessage();

				var inter = c.Client.GetInteractivity();
				var reactions = await inter.CollectReactionsAsync(msg, TimeSpan.FromSeconds(10));
				//var response = await inter.WaitForMessageReactionAsync(msg, c.User, TimeSpan.FromSeconds(5));

				// Confirm message
				await msg.DeleteAllReactionsAsync();
				await msg.CreateReactionAsync(DiscordEmoji.FromName(c.Client, ContentManager.EMOJI_GREEN_CHECK));

				// Do reactions
				//var responseName = response.Emoji.GetDiscordName().ToLower();

			}
		}
	}
}
