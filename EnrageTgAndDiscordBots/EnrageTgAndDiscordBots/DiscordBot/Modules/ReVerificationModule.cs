using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using EnrageDiscordTournamentBot.Bot;
using EnrageDiscordTournamentBot.Log;
using EnrageDiscordTournamentBot.Modules;

namespace EnrageTgAndDiscordBots.DiscordBot.Modules
{
    public class ReVerificationModule : InteractionModuleBase<SocketInteractionContext>
    {
        private Logger _logger;
        private DiscordSocketClient _client;
        private VerificationModule _verificationModule;

        public ReVerificationModule(ConsoleLogger logger, DiscordSocketClient client)
        {
            _logger = logger;
            _client = client;
            _verificationModule = new VerificationModule(logger, client);

            _client.MessageReceived += OnMessageRecieved;
        }

        [DefaultMemberPermissions(GuildPermission.Administrator)]
        [SlashCommand("re-verif-users", "Повторная верификация игроков")]
        public async Task VerifUsers(int countmessages)
        {
            if (Context.Channel.Id == 1323698110891032676)
            {
                await ReVerification(null, Context, countmessages);
                await RespondAsync("Пользователи успешно верифицированы", ephemeral: true);
            }
            else
            {
                await RespondAsync("Вы использовали команду не в том чате!", ephemeral: true);
            }

        }

        private async Task OnMessageRecieved(SocketMessage message)
        {
            var guild = _client.GetGuild(1075718003578126386);
            ulong userId = message.Author.Id;
            SocketGuildUser user = guild.GetUser(userId);
            var userPermissoions = user.GetPermissions((IGuildChannel)message.Channel);

            if (message.Channel.Id == 1323698110891032676)
            {
                if (userPermissoions.ManageChannel != true)
                {
                    await ReVerification(message);
                }
            }
            else
            {
                return;
            }
        }

        private async Task ReVerification(SocketMessage? message = null, SocketInteractionContext? context = null, int countMessages = 0)
        {
            var guild = _client.GetGuild(1075718003578126386);
            ulong userId = 0;
            SocketGuildUser user;
            ChannelPermissions userPermissoions;

            if (context == null)
            {
                userId = message.Author.Id;
                user = guild.GetUser(userId);
                userPermissoions = user.GetPermissions((IGuildChannel)message.Channel);
                string messageText = message.Content.ToString();
                await Splitting(message, null);
            }
            else
            {
                userId = context.User.Id;
                user = guild.GetUser(userId);
                userPermissoions = user.GetPermissions((IGuildChannel)context.Channel);
                ITextChannel finishRegistrationChannel = (ITextChannel)_client.GetChannel(1323698110891032676);
                IEnumerable<IMessage> messages = await finishRegistrationChannel.GetMessagesAsync(countMessages, CacheMode.AllowDownload).FlattenAsync();

                foreach (var item in messages)
                {
                    await Splitting(null, item);
                }
            }
        }

        private async Task Splitting(SocketMessage? message, IMessage? discordMessage)
        {
            string splitString;
            string userAccountRatingString;
            string messageText;

            if (message != null)
                messageText = message.Content;
            else
                messageText = discordMessage.Content;

            try
            {
                splitString = messageText.Split("1)")[1];
                char splitSymbol = ')';

                if (message != null)
                    await _verificationModule.CheckIsCorrectId(message, splitString, messageText, "2" + splitSymbol);
                else
                    await _verificationModule.CheckIsCorrectId(discordMessage, splitString, messageText, "2" + splitSymbol);

                return;
            }
            catch (Exception exept)
            {
                try
                {
                    splitString = messageText.Split("1.")[1];
                    char splitSymbol = '.';

                    if (message != null)
                        await _verificationModule.CheckIsCorrectId(message, splitString, messageText, "2" + splitSymbol);
                    else
                        await _verificationModule.CheckIsCorrectId(discordMessage, splitString, messageText, "2" + splitSymbol);

                    return;
                }
                catch (Exception ex)
                {
                    try
                    {
                        userAccountRatingString = messageText.Split("\n")[0];
                        string splitSymbol = "\n";

                        if (message != null)
                            await _verificationModule.CheckIsCorrectId(message, userAccountRatingString, messageText, "2" + splitSymbol);
                        else
                            await _verificationModule.CheckIsCorrectId(discordMessage, userAccountRatingString, messageText, "2" + splitSymbol);

                        return;
                    }
                    catch (Exception e)
                    {
                        if (message != null)
                            await _verificationModule.WriteExeptIncorrectVerifMessage(message, messageText);
                        else
                            await _verificationModule.WriteExeptIncorrectVerifMessage(discordMessage, messageText);
                    }
                }
            }
        }
    }
}
