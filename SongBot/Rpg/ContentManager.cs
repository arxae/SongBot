namespace SongBot.Rpg
{
	using System;
	using System.Collections.Generic;
	using System.Timers;
	using System.IO;
	using System.Linq;

	using DataClasses;
	using Inventory;

	using _JSON = Newtonsoft.Json.JsonConvert;

	public static class ContentManager
	{
		public const string DB_NAME = "songbot.app.db";
		public const string DB_PLAYER_TABLE = "players";
		public const string DB_LOCINV_TABLE = "location_inventory";

		public const string EMOJI_GREEN_CHECK = ":white_check_mark:";
		public const string EMOJI_BLACK_CHECK = ":heavy_check_mark:";
		public const string EMOJI_BLACK_CROSS = ":heavy_multiplication_x:";
		public const string EMOJI_RED_CROSS = ":x:";
		public const string EMOJI_FACE_HAPPY = ":smile:";
		public const string EMOJI_FACE_SAD = ":frowning:";

		public static GameConfig Config { get; private set; }
		public static DSharpPlus.Entities.DiscordChannel RpgChannel { get; set; }
		public static Random Rng { get; private set; }

		public static Dictionary<string, Race> Races { get; private set; }
		public static Dictionary<string, CharClass> Classes { get; private set; }
		public static Dictionary<string, Location> Locations { get; private set; }
		public static Dictionary<string, PlaceAction> PlaceActions { get; private set; }
		public static Dictionary<string, Type> PlaceActionImplementations { get; private set; }
		public static Dictionary<string, Item> Items { get; private set; }
		public static Dictionary<string, PlayerAction> PlayerActions { get; private set; }

		public static LiteDB.LiteDatabase GetDb() => new LiteDB.LiteDatabase(DB_NAME);

		public static Timer TickTimer;

		public static void Initialize()
		{
			Console.Write("Loading");
			var sw = new System.Diagnostics.Stopwatch();
			sw.Start();

			// ----- Config
			var configJson = File.ReadAllText("Data/Config.json");
			Config = _JSON.DeserializeObject<GameConfig>(configJson);
			Console.Write("...Config");

			// ----- ServiceLocationActions
			var slaJson = File.ReadAllText("Data/PlaceActions.json");
			PlaceActions = _JSON.DeserializeObject<Dictionary<string, PlaceAction>>(slaJson);
			Console.Write($"...SLAs({PlaceActions.Count})");

			// ----- ServiceLocationImplementations
			var interfaceType = typeof(Places.IPlace);
			PlaceActionImplementations = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(s => s.GetTypes())
				.Where(t => t.GetInterfaces().Contains(interfaceType) && t.Name.EndsWith("Service"))
				.ToDictionary(k => k.Name.Replace("Service", ""), v => v);
			Console.Write($"...SLA Impl({PlaceActionImplementations.Count})");

			// ----- Races
			Races = new Dictionary<string, Race>(StringComparer.OrdinalIgnoreCase);
			var racesJson = File.ReadAllText("Data/Races.json");
			var _races = _JSON.DeserializeObject<List<Race>>(racesJson);
			foreach (var r in _races) { Races.Add(r.RaceName, r); }
			Console.Write($"...Races({_races.Count})");

			// ----- Classes
			Classes = new Dictionary<string, CharClass>(StringComparer.OrdinalIgnoreCase);
			var classesJson = File.ReadAllText("Data/Classes.json");
			var _classes = _JSON.DeserializeObject<List<CharClass>>(classesJson);
			foreach (var c in _classes) { Classes.Add(c.ClassName, c); }
			Console.Write($"...Classes({_classes.Count})");

			// ----- Locations
			Locations = new Dictionary<string, Location>(StringComparer.OrdinalIgnoreCase);
			var locationsJson = File.ReadAllText("Data/Locations.json");
			var _locations = _JSON.DeserializeObject<List<Location>>(locationsJson);
			foreach (var loc in _locations) { Locations.Add(loc.LocationId, loc); }
			Console.Write($"...Locations({_locations.Count})");

			// ----- Player Actions
			PlayerActions = new Dictionary<string, PlayerAction>(StringComparer.OrdinalIgnoreCase);
			var playerActJson = File.ReadAllText("Data/PlayerActions.json");
			var _playerActs = _JSON.DeserializeObject<List<PlayerAction>>(playerActJson);
			foreach(var act in _playerActs) { PlayerActions.Add(act.Id, act); }
			Console.Write($"...PlayerActions({_playerActs.Count})");

			// ----- Items
			Items = new Dictionary<string, Item>(StringComparer.OrdinalIgnoreCase);
			var itemsJson = File.ReadAllText("Data/Items.json");
			var _items = _JSON.DeserializeObject<List<Item>>(itemsJson);
			foreach (var i in _items) { Items.Add(i.Id, i); }
			Console.Write($"...Items({_items.Count})");

			// ----- Misc
			Rng = new Random();
			Console.Write("...Misc");

			sw.Stop();
			Console.WriteLine($"...Done in {sw.Elapsed}.");

			TickTimer = new Timer(Config.TickIntervalMs);
			TickTimer.Elapsed += TickTimer_Elapsed;
			Console.WriteLine($"Tick Timer started with {TickTimer.Interval}ms interval...");
		}

		private static void TickTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			throw new NotImplementedException();
		}
	}
}
