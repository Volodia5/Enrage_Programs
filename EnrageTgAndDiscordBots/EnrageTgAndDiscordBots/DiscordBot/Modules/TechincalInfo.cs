using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using EnrageDiscordTournamentBot.Bot;
using EnrageDiscordTournamentBot.Log;
using EnrageDiscordTournamentBot.Modules;

namespace EnrageTgAndDiscordBots.DiscordBot.Modules;

public class TechincalInfo : InteractionModuleBase<SocketInteractionContext>
{
    private Logger _logger;
    private DiscordSocketClient _client;
    private VerificationModule _verificationModule;

    public TechincalInfo(ConsoleLogger logger, DiscordSocketClient client)
    {
        _logger = logger;
        _client = client;
    }
    
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [SlashCommand("bot-ping", "Состояние бота")]
    public async Task GetRealLifeInfo()
    {
        var latency = _client.Latency;
        var status = _client.Status;

        // Создаем EmbedBuilder
        var embedBuilder = new EmbedBuilder()
            .WithTitle("Состояние бота")
            .AddField("Состояние:", $"``` {status} ```", true)
            .AddField("Пинг:", $"``` {latency}ms ```", true)
            .WithColor(Color.Green);

        await RespondAsync("", embed: embedBuilder.Build(), ephemeral:true);
    }
}