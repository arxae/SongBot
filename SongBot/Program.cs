namespace SongBot
{
	class Program
	{
		static void Main(string[] args)
		{
			Rpg.ContentManager.Initialize();
			new Bot().StartBotAsync()
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult();
		}
	}
}