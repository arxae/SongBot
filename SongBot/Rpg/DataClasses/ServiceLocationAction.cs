namespace SongBot.Rpg.DataClasses
{
	using System.Collections.Generic;

	public class ServiceLocationAction
	{
		public string Description { get; set; }
		public string ReactionIcon { get; set; }
		public List<string> ActionCommands { get; set; }
	}
}
