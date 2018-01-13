namespace SongBot.Rpg
{
	public class Player
	{
		// General
		public ulong DiscordId { get; set; }
		public string Race { get; set; }
		public string Class { get; set; }

		// Location
		public string CurrentLocation { get; set; }

		// Actions
		public ContentManager.RpgAction CurrentAction { get; set; }
		public string ActionTarget { get; set; }
		public int ActionTicksRemaining { get; set; }

		public Player()
		{
			CurrentLocation = ContentManager.Config.StartingLocation;
			CurrentAction = ContentManager.RpgAction.Idle;
			ActionTicksRemaining = -1;
		}

		public ContentManager.RpgActionResult SetAction(ContentManager.RpgAction newAction, string actionTarget)
		{
			var currAction = ContentManager.Actions[CurrentAction];

			if (currAction.IsInterruptable == false) return ContentManager.RpgActionResult.CannotInterrupt;

			var nextAction = ContentManager.Actions[newAction];
			CurrentAction = newAction;
			ActionTarget = actionTarget;

			int newCost = nextAction.TicksNeeded;

			if (nextAction.InterruptionTickCost > 0)
			{
				newCost = newCost + nextAction.InterruptionTickCost;
			}

			ActionTicksRemaining = newCost;

			return ContentManager.RpgActionResult.Done;
		}
	}
}
