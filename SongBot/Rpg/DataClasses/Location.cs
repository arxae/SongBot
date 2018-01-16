namespace SongBot.Rpg.DataClasses
{
	using System.Collections.Generic;

	using DSharpPlus.Entities;

	public class Location
	{
		public string LocationId { get; set; }
		public string DisplayName { get; set; }
		public string Description { get; set; }
		public List<string> LocationConnections { get; set; }
		public List<string> Services { get; set; }

		public DiscordEmbed GetLocationEmbed()
		{
			var builder = new DiscordEmbedBuilder()
				.WithTitle(DisplayName)
				.WithDescription(Description);

			// Services
			if (Services.Count > 0)
			{
				builder.AddField("Services", string.Join(", ", Services), true);
			}

			// Exits
			builder.AddField("Exits", string.Join(", ", LocationConnections), true);

			return builder.Build();
		}
	}
}
