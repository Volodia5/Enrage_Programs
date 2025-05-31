using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using EnrageDiscordTournamentBot.Log;

namespace EnrageDiscordTournamentBot.Modules;

public class GameModule : InteractionModuleBase<SocketInteractionContext>
{
    private Logger _logger;
    private DiscordSocketClient _client;
    private SocketUser _interactedUser;
    private ITextChannel _lastUseChannel;
        
    public GameModule(ConsoleLogger logger, DiscordSocketClient client)
    {
        _logger = logger;
        _client = client;
    }
    
    [SlashCommand("ping", "а я не скажу что произойдет:)")]
    private async Task BanUser(SocketGuildUser banningUser)
    {
        RespondAsync("pong!", ephemeral: true);
    }
}