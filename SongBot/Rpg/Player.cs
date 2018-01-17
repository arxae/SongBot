using System;

namespace SongBot.Rpg
{
	public class Player
	{
		// General
		public ulong DiscordId { get; set; }
		public string Race { get; set; }
		public string Class { get; set; }

		// Status
		public int Level { get; set; }
		public int HpCurrent { get; set; }
		public int HpMax { get; set; }
		public int XpCurrent { get; set; }
		public int XpNext { get; set; }

		// Location
		public string CurrentLocation { get; set; }

		public Player() { }

		public Player(ulong discordId, string race, string _class)
		{
			DiscordId = discordId;
			Race = race;
			Class = _class;

			CurrentLocation = ContentManager.Config.StartingLocation;

			Level = 1;
			HpMax = 10;
			HpCurrent = 10;
		}
	}
}
