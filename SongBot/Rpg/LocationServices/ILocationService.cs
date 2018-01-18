namespace SongBot.Rpg.LocationServices
{
	using System.Threading.Tasks;

	using DSharpPlus.CommandsNext;

	using DataClasses;

	public interface ILocationService
	{
		Task EnterLocation(CommandContext c, ServiceLocation loc);
	}
}
