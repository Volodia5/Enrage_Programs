using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using EnrageDiscordTournamentBot.Log;

namespace EnrageDiscordTournamentBot.Modules;

public class ChannelsModule : InteractionModuleBase<SocketInteractionContext>
{
    public InteractionService Commands { get; set; }
    private Logger _logger;
    private DiscordSocketClient _client;

    public ChannelsModule(ConsoleLogger logger, DiscordSocketClient client)
    {
        _logger = logger;
        _client = client;
    }

    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [SlashCommand("clear-channel", "очищает канал в котором было прописано сообщение")]
    private async Task ClearChannel()
    {
        ComponentBuilder buttons = new ComponentBuilder()
            .WithButton("Да", "yes_button", ButtonStyle.Danger)
            .WithButton("Нет", "no_button", ButtonStyle.Success);

        await RespondAsync("Вы уверены что хотите очистить канал ?", components: buttons.Build(), ephemeral: true);
    }

    [ComponentInteraction("yes_button")]
    private async Task ConfirmClearChannel()
    {
        var messages = Context.Channel.GetMessagesAsync();

        await foreach (var item in messages)
        {
            foreach (var message in item)
            {
                if (!message.IsPinned)
                    await message.DeleteAsync();
            }
        }

        await RespondAsync("Канал успешно очищен !!!", ephemeral: true);
    }

    [ComponentInteraction("no_button")]
    public async Task CancelClearChannel()
    {
        await RespondAsync("Операция отменена", ephemeral: true);
    }

    public static Task SendNewsMessage()
    {

        return null;
    }
}