namespace SongBot.Rpg.DataClasses
{
	using System.Collections.Generic;
	using System.Linq;

	using DSharpPlus.Entities;

	public class Location
	{
		public string LocationId { get; set; }
		public string DisplayName { get; set; }
		public string Description { get; set; }
		public List<string> LocationConnections { get; set; }
		public Dictionary<string, ServiceLocation> Services { get; set; }

		public Location()
		{
			Services = new Dictionary<string, ServiceLocation>(System.StringComparer.OrdinalIgnoreCase);
		}

		public DiscordEmbed GetLocationEmbed()
		{
			var builder = new DiscordEmbedBuilder()
				.WithTitle(DisplayName)
				.WithDescription(Description);

			// Services
			if (Services.Count > 0)
			{
				builder.AddField("Services", string.Join(", ", Services.Select(c => c.Key)), true);
			}

			// Exits
			builder.AddField("Exits", string.Join(", ", LocationConnections), true);

			return builder.Build();
		}
	}
}
