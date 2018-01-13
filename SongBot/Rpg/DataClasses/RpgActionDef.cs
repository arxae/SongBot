namespace SongBot.Rpg.DataClasses
{
	using System.ComponentModel;

	using Newtonsoft.Json;

	public class RpgActionDef
	{
		public string ActionName { get; set; }
		public string DisplayName { get; set; }
		public int TicksNeeded { get; set; }

		[DefaultValue(true), JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public bool IsInterruptable { get; set; }

		[DefaultValue(-1), JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int InterruptionTickCost { get; set; }
	}
}
