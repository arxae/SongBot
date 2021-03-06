﻿using System.Collections.Generic;

namespace SongBot.Rpg.Places
{
	using System;
	using System.Threading.Tasks;

	using DSharpPlus.CommandsNext;
	using DSharpPlus.Entities;
	using DSharpPlus.Interactivity;

	using DataClasses;

	public class InnService : IPlace
	{
		public async Task EnterLocation(CommandContext c, Place loc)
		{
			var desc = new System.Text.StringBuilder();

			foreach (var act in loc.Actions)
			{
				var sla = ContentManager.PlaceActions[act];
				desc.AppendLine(sla.Description);
			}

			var embed = new DiscordEmbedBuilder()
				.WithColor(DiscordColor.Blurple)
				.WithTitle(loc.Name)
				.WithDescription(desc.ToString());
			var msg = await c.RespondAsync(embed: embed);

			foreach (var act in loc.Actions)
			{
				var sla = ContentManager.PlaceActions[act];
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

			var actionsToPerform = new List<string>();

			foreach (var act in loc.Actions)
			{
				var sla = ContentManager.PlaceActions[act];

				if (sla.ReactionIcon == responseName)
				{
					actionsToPerform.AddRange(sla.ActionCommands);
				}
			}

			await new ActionProcessor(c, msg).ProcessActionList(actionsToPerform);
		}
	}
}
