﻿using System;

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

		public Player() { }

		public Player(ulong discordId, string race, string _class)
		{
			DiscordId = discordId;
			Race = race;
			Class = _class;

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

			if (newAction == ContentManager.RpgAction.Idle)
			{
				ActionTicksRemaining = -1;
			}
			else
			{
				int newCost = nextAction.TicksNeeded;

				if (nextAction.InterruptionTickCost > 0)
				{
					newCost = newCost + nextAction.InterruptionTickCost;
				}

				ActionTicksRemaining = newCost;

				if (ActionTicksRemaining == 0) return ContentManager.RpgActionResult.DoneInstant;
			}

			return ContentManager.RpgActionResult.Done;
		}

		public void ExecuteCurrentAction()
		{
			switch (CurrentAction)
			{
				case ContentManager.RpgAction.Idle: return;
				case ContentManager.RpgAction.Travel: CurrentLocation = ContentManager.Locations[ActionTarget].LocationId; break;
				default: throw new ArgumentOutOfRangeException();
			}
		}
	}
}
