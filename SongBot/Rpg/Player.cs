namespace SongBot.Rpg
{
	using System.Collections.Generic;

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

		// Actions
		public string NextAction { get; set; }
		public Dictionary<string, object> NextActionParameters;
		public int NextActionTicksRemaining { get; set; }

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

			var idleAct = ContentManager.PlayerActions["idle"];
			NextAction = idleAct.Id;
			NextActionTicksRemaining = idleAct.ReqTicks;
			NextActionParameters = new Dictionary<string, object>(System.StringComparer.OrdinalIgnoreCase);
		}

		public bool SetAction(string actionId, Dictionary<string, object> parameters = null)
		{
			var act = ContentManager.PlayerActions[actionId];

			if (act == null)
			{
				Serilog.Log.ForContext<Player>().Error("Could not find action {actId} for player {playerId}", actionId, DiscordId);
				return false;
			}

			NextAction = act.Id;
			NextActionTicksRemaining = act.ReqTicks;

			if (parameters != null)
			{
				NextActionParameters = parameters;
			}
			else
			{
				NextActionParameters.Clear();
			}

			return true;
		}

		public bool SetSkillAction(string skillId, Dictionary<string, object> parameters = null) => SetAction($"skill:{skillId}", parameters);
	}
}
