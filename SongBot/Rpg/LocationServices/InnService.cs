namespace SongBot.Rpg.LocationServices
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;

	using DSharpPlus.CommandsNext;
	using DSharpPlus.Entities;
	using DSharpPlus.Interactivity;

	using DataClasses;

	public class InnService : ILocationService
	{
		public void Test()
		{
			Console.WriteLine("Inn test");
		}

		public static async Task EnterInn(CommandContext c, ServiceLocation loc)
		{
			var desc = new System.Text.StringBuilder();

			foreach (var act in loc.Actions)
			{
				var sla = ContentManager.ServiceLocationActions[act];
				desc.AppendLine(sla.Description);
			}

			var embed = new DiscordEmbedBuilder()
				.WithColor(DiscordColor.Blurple)
				.WithTitle(loc.Name)
				.WithDescription(desc.ToString());
			var msg = await c.RespondAsync(embed: embed);

			foreach (var act in loc.Actions)
			{
				var sla = ContentManager.ServiceLocationActions[act];
				await msg.CreateReactionAsync(DiscordEmoji.FromName(c.Client, sla.ReactionIcon));
			}

			await c.ConfirmMessage();

			var inter = c.Client.GetInteractivity();
			var response = await inter.WaitForMessageReactionAsync(msg, c.User, TimeSpan.FromSeconds(10));

			// If no response, remove request + Action dialog
			if (response == null)
			{
				Serilog.Log.ForContext<InnService>()
					.Debug("Action timeout, removing all messages concerning this service (user: {usr})", c.User.GetFullUsername());
				await c.Message.DeleteAsync();
				await msg.DeleteAsync();
				return;
			}

			await msg.DeleteAllReactionsAsync();

			var responseName = response.Emoji.GetDiscordName().ToLower();

			string actionToPerform = string.Empty;
			foreach (var act in loc.Actions)
			{
				var sla = ContentManager.ServiceLocationActions[act];

				if (sla.ReactionIcon == responseName)
				{
					actionToPerform = sla.ActionCommand;
					break;
				}
			}

			await ActionProcessor.ExecuteAction(c, actionToPerform, msg);
		}
	}
}
