using Newtonsoft.Json.Converters;

namespace SongBot.Rpg.Commands
{
	using System.Threading.Tasks;

	using DSharpPlus.CommandsNext;
	using DSharpPlus.CommandsNext.Attributes;
	using DSharpPlus.Entities;

	[Group("db"), RequireRoles(RoleCheckMode.All, "Realm Admin")]
	[Description("Various database commands")]
	public class DbCommands
	{
		[Command("getplayerinfo"), Description("Gets playerinformation")]
		public async Task DbGetPlayerInfo(CommandContext c, DiscordUser user)
		{
			c.LogDebug($"Received \"db getplayerinfo\" command from {c.User.GetFullUsername()}");
			await c.Channel.DeleteMessageAsync(c.Message);

			using (var db = ContentManager.GetDb())
			{
				var players = db.GetPlayerTable();

				if (players.Exists(user.Id) == false)
				{
					await c.RespondWithDmAsync("User not found");
					return;
				}

				var json = Newtonsoft.Json.JsonConvert.SerializeObject(players[user.Id], Newtonsoft.Json.Formatting.Indented);
				await c.RespondWithDmAsync(json);
			}
		}

		[Command("getplayerinfobyid"), Description("Get player information by player id")]
		public async Task DbGetPlayerInfoById(CommandContext c, string dId)
		{
			c.LogDebug($"Received \"db getplayerinfobyid\" command from {c.User.GetFullUsername()}");
			await c.Channel.DeleteMessageAsync(c.Message);

			using (var db = ContentManager.GetDb())
			{
				if (ulong.TryParse(dId, out ulong id) == false)
				{
					await c.RespondWithDmAsync("Invalid player id");
					return;
				}

				var players = db.GetPlayerTable();

				if (players.Exists(id) == false)
				{
					await c.RespondWithDmAsync($"Player id {id} not found");
					return;
				}

				var json = Newtonsoft.Json.JsonConvert.SerializeObject(players[id], Newtonsoft.Json.Formatting.Indented);
				await c.RespondWithDmAsync(json);
			}
		}

		[Command("deleteplayer"), Description("Remove a player")]
		public async Task DbRemovePlayer(CommandContext c, DiscordUser user)
		{
			c.LogDebug($"Received \"db deleteplayer\" command from {c.User.GetFullUsername()}");
			await c.Channel.DeleteMessageAsync(c.Message);

			using (var db = ContentManager.GetDb())
			{
				var players = db.GetPlayerTable();

				if (players.Exists(user.Id) == false)
				{
					await c.RespondWithDmAsync("User not found");
					return;
				}

				players.Delete(user.Id);
				c.LogWarning($"Player {user.GetFullUsername()}({user.Id}) has been deleted by {c.User.GetFullUsername()}");

				await c.RespondWithDmAsync($"User {user.GetFullUsername()} ({user.Id}) has been deleted");
			}
		}

		[Command("getplayers"), Description("Get all players")]
		public async Task DbGetPlayers(CommandContext c)
		{
			c.LogDebug($"Received \"db getplayers\" command from {c.User.GetFullUsername()}");
			await c.Channel.DeleteMessageAsync(c.Message);

			var sb = new System.Text.StringBuilder();
			sb.AppendLine("Player List");
			sb.AppendLine("DiscordId - Username");

			using (var db = ContentManager.GetDb())
			{
				var players = db.GetPlayerTable();

				foreach (var p in players)
				{
					var user = await c.Guild.GetMemberAsync(p.Key);
					sb.Append($"{user.Id} - {user.GetFullUsername()}");
				}
			}

			await c.RespondWithDmAsync(sb.ToString());
		}
	}
}
