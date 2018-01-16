namespace SongBot.Rpg.DataClasses
{
    public class GameConfig
    {
        public string StartingLocation { get; set; }
		public string StartingClass { get; set; }
		public ulong SongDiscordGuildId { get; set; }
		public string RpgChannel { get; set; }
		public bool NeverDmResults { get; set; }
	}
}
