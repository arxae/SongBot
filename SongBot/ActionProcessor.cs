using System.Collections.Generic;
using SongBot.Rpg;

namespace SongBot
{
	using System.Threading.Tasks;

	using DSharpPlus.CommandsNext;
	using DSharpPlus.Entities;

	public class ActionProcessor
	{
		/// <summary>
		/// Execute a list of commands
		/// </summary>
		/// <param name="c">CommandContext of the message requesting something that includes the action</param>
		/// <param name="actionList">List of actions to be parsed</param>
		/// <param name="associatedMsg">A reply message from the bot to the user (used with interactivity)</param>
		/// <returns></returns>
		public static async Task ExecuteActionListAsync(CommandContext c, List<string> actionList, DiscordMessage associatedMsg = null)
		{
			foreach (var act in actionList)
			{
				await ExecuteActionAsync(c, act, associatedMsg);
			}
		}

		/// <summary>
		/// Execute a single command
		/// </summary>
		/// <param name="c">CommandContext of the message requesting something that includes the action</param>
		/// <param name="action">Actions to be parsed</param>
		/// <param name="associatedMsg">A reply message from the bot to the user (used with interactivity)</param>
		/// <returns></returns>
		public static async Task ExecuteActionAsync(CommandContext c, string action, DiscordMessage associatedMsg = null)
		{
			try
			{
				// Sets the associatedMsg to a specified value
				if (action.StartsWith("setmsg::"))
				{
					string msg = ParseMessageMentions(c, $"{action.Substring("setmsg::".Length)}");
					if (associatedMsg == null) return;
					await associatedMsg.ModifyAsync(msg, null);
					return;
				}

				// Heal the person who requested the command
				if (action.StartsWith("healself::"))
				{
					var parts = action.Split(new[] {"::"}, System.StringSplitOptions.None);
					if (int.TryParse(parts[1], out int healAmount) == false)
					{
						Serilog.Log.ForContext<ActionProcessor>().Error("Error processing action params: {cmd}", action);
						return;
					}

					HealPlayer(c.User.Id, healAmount);
					return;
				}

				// Heal all health
				if (action.StartsWith("healselffull::"))
				{
					FullHealPlayer(c.User.Id);
					return;
				}

				// Respond with a message
				if (action.StartsWith("addmsg::"))
				{
					var msg = ParseMessageMentions(c, action.Substring("addmsg::".Length));
					await c.RespondAsync(msg);
					return;
				}

				// Delete the original message from the user
				if (action.StartsWith("deletemsg::"))
				{
					await c.Message.DeleteAsync();
					return;
				}

				// Delete the associated message (reply from user command)
				if (action.StartsWith("deleteassocmsg::"))
				{
					if (associatedMsg == null) return;
					await associatedMsg.DeleteAsync();
					return;
				}
				
				Serilog.Log.ForContext<ActionProcessor>().Warning("Unrecognized action: {cmd}", action);
			}
			catch
			{
				Serilog.Log.ForContext<ActionProcessor>().Error("Error processing command: {cmd}", action);
			}
		}

		static void HealPlayer(ulong playerId, int amount)
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

		static void FullHealPlayer(ulong playerId)
		{
			using (var db = ContentManager.GetDb())
			{
				var player = db.GetPlayer(playerId);
				if (player == null) return;

				player.HpCurrent = player.HpMax;
				db.GetPlayerTable().Update(player.DiscordId, player);
			}
		}

		static string ParseMessageMentions(CommandContext c, string msg)
		{
			return msg.Replace("@mentionself", c.User.Mention);
		}
	}
}
