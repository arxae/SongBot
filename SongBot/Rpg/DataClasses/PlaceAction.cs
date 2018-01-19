namespace SongBot.Rpg.DataClasses
{
	using System.Collections.Generic;

	public class PlaceAction
	{
		public string Description { get; set; }
		public string ReactionIcon { get; set; }
		public List<string> ActionCommands { get; set; }
	}
}
