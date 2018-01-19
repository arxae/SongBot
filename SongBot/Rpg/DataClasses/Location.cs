using SongBot.Rpg.Inventory;

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
		public string Hostility { get; set; }
		public List<string> LocationConnections { get; set; }
		public Dictionary<string, Place> Places { get; set; }
		public List<Item> Items { get; set; }

		public Location()
		{
			Places = new Dictionary<string, Place>(System.StringComparer.OrdinalIgnoreCase);
		}

		public DiscordEmbed GetLocationEmbed()
		{
			var builder = new DiscordEmbedBuilder()
				.WithTitle(DisplayName);
			//.WithDescription(Description);

			var desc = new System.Text.StringBuilder(Description);

			var footer = new System.Text.StringBuilder();
			footer.Append("Cmd Help: .travelto <name>");

			// Hostility Colors
			switch (Hostility.ToLower())
			{
				case "sanctuary": builder.WithColor(DiscordColor.Gold); break;
				case "friendly": builder.WithColor(DiscordColor.SpringGreen); break;
				case "neutral": builder.WithColor(DiscordColor.CornflowerBlue); break;
				case "caution": builder.WithColor(DiscordColor.Orange); break;
				case "dangerous": builder.WithColor(DiscordColor.IndianRed); break;
				case "hostile": builder.WithColor(DiscordColor.Red); break;
				default: builder.WithColor(DiscordColor.White); break;
			}

			// Services
			if (Places.Count > 0)
			{
				desc.AppendLine();
				desc.AppendLine("**Places**");
				desc.AppendLine(string.Join(", ", Places.Keys));

				footer.Append(", .enter <name>");
			}

			// Exits
			desc.AppendLine();
			desc.AppendLine("**Exits**");
			desc.AppendLine(string.Join(", ", LocationConnections));

			builder.WithDescription(desc.ToString());
			builder.WithFooter(footer.ToString());

			return builder.Build();
		}
	}
}
