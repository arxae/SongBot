namespace SongBot.Rpg.DataClasses
{
	using System.Collections.Generic;

	using DSharpPlus.Entities;

	public class Location
	{
		public string LocationId { get; set; }
		public string DisplayName { get; set; }
		public string Description { get; set; }
		public List<string> LocationLinks { get; set; }

		public DiscordEmbed GetLocationEmbed()
		{
			var builder = new DiscordEmbedBuilder();

			builder.WithTitle(DisplayName);

			var sb = new System.Text.StringBuilder();
			sb.AppendLine(Description);
			sb.AppendLine(System.Environment.NewLine);
			sb.Append("**Exits:** ");
			sb.AppendLine(string.Join(", ", LocationLinks));

			builder.WithDescription(sb.ToString());

			return builder.Build();
		}
	}
}
