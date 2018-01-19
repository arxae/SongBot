namespace SongBot.Rpg.Places
{
	using System.Threading.Tasks;

	using DSharpPlus.CommandsNext;

	using DataClasses;

	public interface IPlace
	{
		Task EnterLocation(CommandContext c, Place loc);
	}
}
