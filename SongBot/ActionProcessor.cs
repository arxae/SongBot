namespace SongBot
{
using System.Threading.Tasks;

	using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

	public class ActionProcessor
	{
		public static async Task ExecuteAction(CommandContext c, string action, DiscordMessage associatedMsg = null)
		{
			if (action.ToLower().StartsWith("echo:"))
			{
				string msg = $"{c.User.Mention} {action.Substring(5)}";
				await associatedMsg.ModifyAsync(msg, null);
				return;
			}
		}
	}
}
