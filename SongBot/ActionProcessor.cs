using System.Collections.Generic;

namespace SongBot
{
	using System.Threading.Tasks;

	using DSharpPlus.CommandsNext;
	using DSharpPlus.Entities;

	using Rpg;

	public class ActionProcessor
	{
		readonly CommandContext cmdContext;
		public DiscordMessage AssociatedMsg { get; set; }

		public ActionProcessor(CommandContext c)
		{
			cmdContext = c;
		}

		public ActionProcessor(CommandContext c, DiscordMessage associatedMsg) : this(c)
		{
			AssociatedMsg = associatedMsg;
		}

		public async Task ProcessActionList(List<string> actions)
		{
			for (int i = 0; i < actions.Count; i++)
			{
				await ExecuteActionAsync(actions[i]);
			}
		}

		public async Task ExecuteActionAsync(string action)
		{
			try
			{
				// Sets the associatedMsg to a specified value
				if (action.StartsWith("setmsg->"))
				{
					string msg = ParseMessageMentions(cmdContext, $"{action.Substring("setmsg->".Length)}");
					if (AssociatedMsg == null) return;
					await AssociatedMsg.ModifyAsync(msg, null);
					return;
				}

				// Heal the person who requested the command
				if (action.StartsWith("healself->"))
				{
					var parts = action.Split(new[] { "->" }, System.StringSplitOptions.None);
					if (int.TryParse(parts[1], out int healAmount) == false)
					{
						Serilog.Log.ForContext<ActionProcessor>().Error("Error processing action params: {cmd}", action);
						return;
					}

					HealPlayer(cmdContext.User.Id, healAmount);
					return;
				}

				// Heal all health
				if (action.StartsWith("healselffull->"))
				{
					FullHealPlayer(cmdContext.User.Id);
					return;
				}

				// Respond with a message
				if (action.StartsWith("addmsg->"))
				{
					var msg = ParseMessageMentions(cmdContext, action.Substring("addmsg->".Length));
					await cmdContext.RespondAsync(msg);
					return;
				}

				// Delete the original message from the user
				if (action.StartsWith("deletemsg->"))
				{
					await cmdContext.Message.DeleteAsync();
					return;
				}

				// Delete the associated message (reply from user command)
				if (action.StartsWith("deleteassocmsg->"))
				{
					if (AssociatedMsg == null) return;
					await AssociatedMsg.DeleteAsync();
					return;
				}

				Serilog.Log.ForContext<ActionProcessor>().Warning("Unrecognized action: {cmd}", action);
			}
			catch
			{
				Serilog.Log.ForContext<ActionProcessor>().Error("Error processing command: {cmd}", action);
			}
		}

		void HealPlayer(ulong playerId, int amount)
		{
			using (var db = ContentManager.GetDb())
			{
				var player = db.GetPlayer(playerId);
				if (player == null) return;

				player.HpCurrent += amount;
				if (player.HpCurrent > player.HpMax)
				{
					player.HpCurrent = player.HpMax;
				}

				db.GetPlayerTable().Update(player.DiscordId, player);
			}
		}

		void FullHealPlayer(ulong playerId)
		{
			using (var db = ContentManager.GetDb())
			{
				var player = db.GetPlayer(playerId);
				if (player == null) return;

				player.HpCurrent = player.HpMax;
				db.GetPlayerTable().Update(player.DiscordId, player);
			}
		}

		string ParseMessageMentions(CommandContext c, string msg)
		{
			return msg.Replace("@mentionself", c.User.Mention);
		}
	}
}
