using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using EnrageDiscordTournamentBot.Log;

namespace EnrageDiscordTournamentBot.Modules
{
    //INSERT team_data (user_id, team_description) VALUES({Context.User.Id}, '{message}');
    //SELECT team_description FROM team_data td WHERE td.user_id = {Context.User.Id};
    //DELETE team_data FROM team_data WHERE team_data.user_id = {Context.User.Id};
    //
    //SELECT count FROM counter c WHERE c.userid = 
    //

    public class RegistrationModalModule : InteractionModuleBase<SocketInteractionContext>
    {
        public InteractionService Commands { get; set; }
        private Logger _logger;
        private DiscordSocketClient _client;
        private SocketUser _interactedUser;
        private ITextChannel _lastUseChannel;
        public string newRegistrationTeam;
        public int slotsCount;
        public int iteractionsCount = 0;
        public ulong messageId;

        public RegistrationModalModule(ConsoleLogger logger, DiscordSocketClient client)
        {
            _logger = logger;
            _client = client;

            if (iteractionsCount == 0)
                _client.ButtonExecuted += ButtonsHandler;
            else
                return;

            iteractionsCount++;
        }

        public async Task ButtonsHandler(SocketMessageComponent component)
        {
            switch (component.Data.CustomId)
            {
                case "no_truobles":
                    //await FinishingPay(component);
                    break;
                case "confirm_pay":
                    await ConfirmingPay(component);
                    break;
                default:
                    return;
                    break;
            }
        }
        
        [DefaultMemberPermissions(GuildPermission.Administrator)]
        [SlashCommand("open-registration", "открывает регистрацию на мини турниры")]
        private async Task ClearRegChannel()
        {
            ITextChannel finishRegistrationChannel = (ITextChannel)_client.GetChannel(1161007275213869216);
            var messages = finishRegistrationChannel.GetMessagesAsync(100);
            IUserMessage pinnedMessage =
                (IUserMessage)await finishRegistrationChannel.GetMessageAsync(1176155447640723476);
            IEnumerable<IMessage> messagesInRegChannel =
                await finishRegistrationChannel.GetMessagesAsync(100, CacheMode.AllowDownload).FlattenAsync();
            int messagesCount = messagesInRegChannel.Count();
        
            await pinnedMessage.ModifyAsync(x =>
                x.Content = $"**Занято {0}** слотов, **осталось {9 - messagesCount}** слотов/{0} слотов забронированы");
        
            await RespondWithFileAsync(@"C:\Users\User\Desktop\for Enrage DS server\rega_otkryta.png", null,
                "**Регистрация на мини турниры открыта, мы ждем именно тебя !**");
        }

        [DefaultMemberPermissions(GuildPermission.Administrator)]
        [SlashCommand("check-places", "кол-во мест")]
        private async Task GetFreePlaces()
        {
            IEnumerable<IMessage> messages =
                await Context.Channel.GetMessagesAsync(100, CacheMode.AllowDownload).FlattenAsync();
            int messagesCount = messages.Count();
        
            await Context.Channel.SendMessageAsync(
                $"На данный момент **занято {messagesCount}** слотов, **осталось {8 - messagesCount}** свободных слотов");
        }

        [DefaultMemberPermissions(GuildPermission.Administrator)]
        [SlashCommand("add-registration-button", "добавляет кнопку для регистрации на турниры через Ds")]
        private async Task AddRegButton()
        {
            var mcb = new ComponentBuilder()
                .WithButton("ЗАРЕГИСТРИРОВАТЬСЯ !", "reg_button", ButtonStyle.Success);
        
            await Context.Channel.SendMessageAsync("Зарегистрируйте свою команду не предстоящий турнир !",
                components: mcb.Build());
        }

        [ComponentInteraction("reg_button")]
        public async Task RespondRegistrationModal()
        {
            await RespondWithModalAsync<RegistrationModal>("modal_input_demo");
        }

        [SlashCommand("registration", "Зарегистрируйте вашу команду на предстоящий турнир!")]
        public async Task ModalInput()
        {
            ITextChannel finishRegistrationChannel = (ITextChannel)_client.GetChannel(1161007275213869216);
            IEnumerable<IMessage> messages = await finishRegistrationChannel
                .GetMessagesAsync(100, CacheMode.AllowDownload).FlattenAsync();
            int messagesCount = messages.Count();
        
            if (messagesCount == 19)
            {
                await RespondWithFileAsync(@"C:\Users\User\Desktop\for Enrage DS server\rega_zakryta.png", null,
                    "Достигнут лимит команд на данный турнир, увидимся через неделю !", ephemeral: true);
            }
            else if (messagesCount == 2)
            {
                await RespondWithFileAsync(@"C:\Users\User\Desktop\for Enrage DS server\rega_zakryta.png", null,
                    "В данный момент регистрация на турнир закрыта!", ephemeral: true);
            }
            else
            {
                await Context.Interaction.RespondWithModalAsync<RegistrationModal>("modal_input_demo");
            }
        }

        // [ModalInteraction("troubles_or_replacement")]
        // public async Task TroublesModalResponse(TeamTroublesOrReplacementModal modal)
        // {
        //     string firstPlayerDB = string.Empty;
        //     string secondPlayerDB = string.Empty;
        //     string thirdPlayerDB = string.Empty;
        //     string firstPlayerVK = string.Empty;
        //     string secondPlayerVK = string.Empty;
        //     string thirdPlayerVK = string.Empty;
        //     string firstPlayerDS = string.Empty;
        //     string secondPlayerDS = string.Empty;
        //     string thirdPlayerDS = string.Empty;
        //     string teamTroubles = $"{modal.TeamTroublesDescription}";
        //     string teamDB = $"{modal.ReplacementPlayersDotabuff}";
        //     string teamVK = $"{modal.ReplacementPlayersVK}";
        //     string teamDS = $"{modal.ReplacementPlayersDiscord}";
        //     string[] usersDB = teamDB.Split("\n");
        //     string[] usersVK = teamVK.Split("\n");
        //     string[] usersDS = teamDS.Split("\n");
        //     string newTeamTroubles = string.Empty;
        //     string newTeam = string.Empty;
        //     string selectNewTeamData =
        //         $"SELECT team_description FROM team_data td WHERE td.user_id = {Context.User.Id};";
        //     var guild = _client.GetGuild(1075718003578126386);
        //     ITextChannel registrationConfirmChannel = (ITextChannel)guild.GetChannel(1160654502123290655);
        //
        //     MySqlConnection connection = DBUtils.GetDBConnection();
        //
        //     await connection.OpenAsync();
        //     DataTable table = new DataTable();
        //     MySqlCommand command = new MySqlCommand(selectNewTeamData, connection);
        //     MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync();
        //
        //     while (await reader.ReadAsync())
        //     {
        //         newTeam = reader[0].ToString();
        //     }
        //
        //     await reader.CloseAsync();
        //     await connection.CloseAsync();
        //
        //     for (int i = 0; i < usersDB.Length; i++)
        //     {
        //         switch (i)
        //         {
        //             case 0:
        //                 firstPlayerDB = usersDB[i];
        //                 break;
        //             case 1:
        //                 secondPlayerDB = usersDB[i];
        //                 break;
        //             case 2:
        //                 thirdPlayerDB = usersDB[i];
        //                 break;
        //             default:
        //                 break;
        //         }
        //     }
        //
        //     for (int i = 0; i < usersVK.Length; i++)
        //     {
        //         switch (i)
        //         {
        //             case 0:
        //                 firstPlayerVK = usersVK[i];
        //                 break;
        //             case 1:
        //                 secondPlayerVK = usersVK[i];
        //                 break;
        //             case 2:
        //                 thirdPlayerVK = usersVK[i];
        //                 break;
        //             default:
        //                 break;
        //         }
        //     }
        //
        //     for (int i = 0; i < usersDS.Length; i++)
        //     {
        //         switch (i)
        //         {
        //             case 0:
        //                 firstPlayerDS = usersDS[i];
        //                 break;
        //             case 1:
        //                 secondPlayerDS = usersDS[i];
        //                 break;
        //             case 2:
        //                 thirdPlayerDS = usersDS[i];
        //                 break;
        //             default:
        //                 break;
        //         }
        //     }
        //
        //     if (firstPlayerDB == "" || firstPlayerDB == "-" && firstPlayerVK == "" ||
        //         firstPlayerVK == "-" && firstPlayerDS == "" || firstPlayerDS == "-")
        //     {
        //         if (teamTroubles == "-")
        //         {
        //             var exeptMessage =
        //                 await Context.Channel.SendMessageAsync("Данные были введены неверно, повторите попытку");
        //             Thread.Sleep(2000);
        //             await exeptMessage.DeleteAsync();
        //         }
        //         else if (teamTroubles != "-")
        //         {
        //             newTeamTroubles = teamTroubles;
        //         }
        //     }
        //     else if (firstPlayerDB != "" && firstPlayerVK != "" || firstPlayerDS != "")
        //     {
        //         if (teamTroubles == "-")
        //         {
        //             if (firstPlayerDB != "" && firstPlayerVK != "" && firstPlayerDS != "")
        //             {
        //                 newTeam +=
        //                     $"\n\nЗамены:\n\n>>> 1.1)db/startz - {firstPlayerDB}, \n vk - {firstPlayerVK}, \n ds - {firstPlayerDS}";
        //             }
        //
        //             if (secondPlayerDB != "" && secondPlayerVK != "" && secondPlayerDS != "")
        //             {
        //                 newTeam +=
        //                     $"\n\n>>> 1.2)db/startz - {secondPlayerDB}, \n vk - {secondPlayerVK}, \n ds - {secondPlayerDS}";
        //             }
        //
        //             if (thirdPlayerDB != "" && thirdPlayerVK != "" && thirdPlayerDS != "")
        //             {
        //                 newTeam +=
        //                     $"\n\n>>> 1.3)db/startz - {thirdPlayerDB}, \n vk - {thirdPlayerVK}, \n ds - {thirdPlayerDS}";
        //             }
        //         }
        //
        //         if (teamTroubles != "-")
        //         {
        //             if (firstPlayerDB != "" && firstPlayerVK != "" && firstPlayerDS != "")
        //             {
        //                 newTeam +=
        //                     $"\n\nЗамены:\n>>> 1.1)db/startz - {firstPlayerDB}, \n vk - {firstPlayerVK}, \n ds - {firstPlayerDS}";
        //             }
        //
        //             if (secondPlayerDB != "" && secondPlayerVK != "" && secondPlayerDS != "")
        //             {
        //                 newTeam +=
        //                     $"\n>>> 1.2)db/startz - {secondPlayerDB}, \n vk - {secondPlayerVK}, \n ds - {secondPlayerDS}";
        //             }
        //
        //             if (thirdPlayerDB != "" && thirdPlayerVK != "" && thirdPlayerDS != "")
        //             {
        //                 newTeam +=
        //                     $"\n>>> 1.3)db/startz - {thirdPlayerDB}, \n vk - {thirdPlayerVK}, \n ds - {thirdPlayerDS}";
        //             }
        //         }
        //     }
        //     else
        //     {
        //         var exeptMessage =
        //             await Context.Channel.SendMessageAsync("Данные были введены неверно, повторите попытку");
        //         Thread.Sleep(2000);
        //         await exeptMessage.DeleteAsync();
        //
        //         return;
        //     }
        //
        //     var bb = new ComponentBuilder()
        //         .WithButton("Подтвердить оплату", "confirm_pay", ButtonStyle.Success);
        //
        //     if (teamTroubles == "")
        //     {
        //         await registrationConfirmChannel.SendMessageAsync(newTeam, components: bb.Build());
        //     }
        //     else
        //     {
        //         await registrationConfirmChannel.SendMessageAsync(newTeam +
        //                                                           $"\n\nУ команды имеются проблемы:\n{teamTroubles} \n",
        //             components: bb.Build());
        //     }
        //
        //     var message = await Context.User.SendMessageAsync(
        //         $"<@{Context.User.Id}>\nПродолжая регистрацию вы соглашаетесь с правилами организации ENRAGE ( <https://vk.com/topic-219738248_49296264)> )\n" +
        //         "Для завершения регистрации нужно оплатить стартовый взнос в размере:\n500 рублей.\n" +
        //         "Реквизиты:\n4276 3100 3033 6137 (Сбербанк | Кузьмин Павел Викторович)\n2200 7007 1123 1646(Тинькофф / Кузьмин Павел Викторович).\n" +
        //         $"После оплаты написать <https://vk.com/id132681884> - вк или @sewpho - дискорд для подтверждения оплаты\n\nНа данный момент занят(-о) {slotsCount} слот(-ов)");
        // }

        // [ComponentInteraction("yes_truobles")]
        // public async Task TeamHaveTroubles()
        // {
        //     await RespondWithModalAsync<TeamTroublesOrReplacementModal>("troubles_or_replacement");
        // }
        //
        // [ModalInteraction("modal_input_demo")]
        // public async Task ModalResponse(RegistrationModal modal)
        // {
        //     string firstPlayerDB = string.Empty;
        //     string secondPlayerDB = string.Empty;
        //     string thirdPlayerDB = string.Empty;
        //     string fourthPlayerDB = string.Empty;
        //     string fifthPlayerDB = string.Empty;
        //     string firstPlayerVK = string.Empty;
        //     string secondPlayerVK = string.Empty;
        //     string thirdPlayerVK = string.Empty;
        //     string fourthPlayerVK = string.Empty;
        //     string fifthPlayerVK = string.Empty;
        //     string firstPlayerDS = string.Empty;
        //     string secondPlayerDS = string.Empty;
        //     string thirdPlayerDS = string.Empty;
        //     string fourthPlayerDS = string.Empty;
        //     string fifthPlayerDS = string.Empty;
        //     string teamDB = $"{modal.PlayersDotabuff}";
        //     string teamVK = $"{modal.PlayersVK}";
        //     string teamDS = $"{modal.PlayersDiscord}";
        //     string[] usersDB = teamDB.Split("\n");
        //     string[] usersVK = teamVK.Split("\n");
        //     string[] usersDS = teamDS.Split("\n");
        //     string teamName = $"{modal.TeamName}";
        //     var guild = _client.GetGuild(1075718003578126386);
        //     AllowedMentions mentions = new();
        //     MySqlConnection connection = DBUtils.GetDBConnection();
        //
        //     for (int i = 0; i < usersDB.Length; i++)
        //     {
        //         switch (i)
        //         {
        //             case 0:
        //                 firstPlayerDB = usersDB[i];
        //                 break;
        //             case 1:
        //                 secondPlayerDB = usersDB[i];
        //                 break;
        //             case 2:
        //                 thirdPlayerDB = usersDB[i];
        //                 break;
        //             case 3:
        //                 fourthPlayerDB = usersDB[i];
        //                 break;
        //             case 4:
        //                 fifthPlayerDB = usersDB[i];
        //                 break;
        //             default:
        //                 break;
        //         }
        //     }
        //
        //     for (int i = 0; i < usersVK.Length; i++)
        //     {
        //         switch (i)
        //         {
        //             case 0:
        //                 firstPlayerVK = usersVK[i];
        //                 break;
        //             case 1:
        //                 secondPlayerVK = usersVK[i];
        //                 break;
        //             case 2:
        //                 thirdPlayerVK = usersVK[i];
        //                 break;
        //             case 3:
        //                 fourthPlayerVK = usersVK[i];
        //                 break;
        //             case 4:
        //                 fifthPlayerVK = usersVK[i];
        //                 break;
        //             default:
        //                 break;
        //         }
        //     }
        //
        //     for (int i = 0; i < usersDS.Length; i++)
        //     {
        //         switch (i)
        //         {
        //             case 0:
        //                 firstPlayerDS = usersDS[i];
        //                 break;
        //             case 1:
        //                 secondPlayerDS = usersDS[i];
        //                 break;
        //             case 2:
        //                 thirdPlayerDS = usersDS[i];
        //                 break;
        //             case 3:
        //                 fourthPlayerDS = usersDS[i];
        //                 break;
        //             case 4:
        //                 fifthPlayerDS = usersDS[i];
        //                 break;
        //             default:
        //                 break;
        //         }
        //     }
        //
        //     if (firstPlayerDB != "" && secondPlayerDB != "" && thirdPlayerDB != "" && fourthPlayerDB != "" &&
        //         fifthPlayerDB != ""
        //         && firstPlayerVK != "" && secondPlayerVK != "" && thirdPlayerVK != "" && fourthPlayerVK != "" &&
        //         fifthPlayerVK != ""
        //         && firstPlayerDS != "" && secondPlayerDS != "" && thirdPlayerDS != "" && fourthPlayerDS != "" &&
        //         fifthPlayerDS != "")
        //     {
        //         string message =
        //             $"## Новая регистрация команды от игрока @{Context.User.Username}\n\n 1)\n> **Команда:** {teamName}\n\n 2)\n> **Ссылка на капитана:** <{modal.CapitanLink}>\n" +
        //             $"\n3)" +
        //             $"\n 1.\n> **db/startz** - <{firstPlayerDB}>, \n> **vk** - <{firstPlayerVK}>, \n> **ds** - {firstPlayerDS}\n" +
        //             $"\n 2.\n> **db/startz** - <{secondPlayerDB}>, \n> **vk** - <{secondPlayerVK}>, \n> **ds** - {secondPlayerDS}\n" +
        //             $"\n 3.\n> **db/startz** - <{thirdPlayerDB}>, \n> **vk** - <{thirdPlayerVK}>, \n> **ds** - {thirdPlayerDS}\n" +
        //             $"\n 4.\n> **db/startz** - <{fourthPlayerDB}>, \n> **vk** - <{fourthPlayerVK}>, \n> **ds** - {fourthPlayerDS}\n" +
        //             $"\n 5.\n> **db/startz** - <{fifthPlayerDB}>, \n> **vk** - <{fifthPlayerVK}>, \n> **ds** - {fifthPlayerDS}\n";
        //
        //         newRegistrationTeam = message;
        //
        //         await connection.OpenAsync();
        //
        //         string addNewTeamData =
        //             $"INSERT team_data (user_id, team_description) VALUES({Context.User.Id}, '{message}');";
        //         DataTable table = new DataTable();
        //         MySqlDataAdapter adapter = new MySqlDataAdapter(addNewTeamData, connection);
        //         MySqlCommand command = new MySqlCommand(addNewTeamData);
        //         adapter.InsertCommand = command;
        //         await adapter.FillAsync(table);
        //
        //         await connection.CloseAsync();
        //
        //         message =
        //             "Подскажите, имеются ли у вас замены или спорные ситуации, о которых вы хотели бы сообщить администрации?";
        //
        //         var bb = new ComponentBuilder()
        //             .WithButton("ДА", "yes_truobles", ButtonStyle.Danger)
        //             .WithButton("НЕТ", "no_truobles", ButtonStyle.Success);
        //
        //         mentions.AllowedTypes = AllowedMentionTypes.Users;
        //
        //         await _logger.Log(new LogMessage(LogSeverity.Info, "HelloModalModule : modal_input_demo",
        //             $"User: {Context.User.Username}, modal input: {newRegistrationTeam}"));
        //         await RespondAsync(message, components: bb.Build(), allowedMentions: mentions, ephemeral: true);
        //     }
        // }

        // private async Task FinishingPay(SocketMessageComponent component)
        // {
        //     var guild = _client.GetGuild(1075718003578126386);
        //     string newTeam = string.Empty;
        //
        //     ITextChannel registrationConfirmChannel = (ITextChannel)guild.GetChannel(1160654502123290655);
        //
        //     var bb = new ComponentBuilder()
        //         .WithButton("Подтвердить оплату", "confirm_pay", ButtonStyle.Success);
        //
        //
        //     string selectNewTeamData =
        //         $"SELECT team_description FROM team_data td WHERE td.user_id = {component.User.Id};";
        //     MySqlConnection connection = DBUtils.GetDBConnection();
        //
        //     await connection.OpenAsync();
        //     DataTable table = new DataTable();
        //     MySqlCommand command = new MySqlCommand(selectNewTeamData, connection);
        //     MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync();
        //
        //     while (await reader.ReadAsync())
        //     {
        //         newTeam = reader[0].ToString();
        //     }
        //
        //     await reader.CloseAsync();
        //
        //     string deleteNewTeamData =
        //         $"DELETE team_data FROM team_data WHERE team_data.user_id = {component.User.Id};";
        //     table = new DataTable();
        //     MySqlDataAdapter adapter = new MySqlDataAdapter(deleteNewTeamData, connection);
        //     command = new MySqlCommand(deleteNewTeamData);
        //     adapter.DeleteCommand = command;
        //     await adapter.FillAsync(table);
        //
        //     await connection.CloseAsync();
        //
        //     if (newTeam != "")
        //     {
        //         var message = await component.User.SendMessageAsync(
        //             $"<@{component.User.Id}>\nПродолжая регистрацию вы соглашаетесь с правилами организации ENRAGE ( <https://vk.com/topic-219738248_49296264)> )\n" +
        //             "Для завершения регистрации нужно оплатить стартовый взнос в размере:\n300 рублей если вы находитесь в 1-32 слоте\n350 рублей 33+ слоте.\n" +
        //             "Реквизиты:\n4276 3100 3033 6137 (Сбербанк | Кузьмин Павел Викторович)\n2200 7007 1123 1646(Тинькофф / Кузьмин Павел Викторович).\n" +
        //             $"После оплаты написать <https://vk.com/id132681884> - вк или @sadpasha - дискорд для подтверждения оплаты\n\nНа данный момент занят(-о) {slotsCount} слот(-ов)");
        //
        //         await registrationConfirmChannel.SendMessageAsync(newTeam, components: bb.Build());
        //     }
        // }

        private async Task ConfirmingPay(SocketMessageComponent component)
        {
            var guild = _client.GetGuild(1075718003578126386);
            ITextChannel finishRegistrationChannel = (ITextChannel)_client.GetChannel(1161007275213869216);
            ITextChannel registrationTextChannel = (ITextChannel)guild.GetChannel(1161007275213869216);
            int teamCount = await registrationTextChannel.GetMessagesAsync().CountAsync();
            IEnumerable<IMessage> messages = await finishRegistrationChannel
                .GetMessagesAsync(100, CacheMode.AllowDownload).FlattenAsync();
            IUserMessage message = (IUserMessage)await finishRegistrationChannel.GetMessageAsync(1176155447640723476);
            int messagesCount = messages.Count();

            var eb = new EmbedBuilder()
                .WithColor(16777215)
                .WithDescription(component.Message.Content);

            if (messagesCount == 2)
                await message.ModifyAsync(x =>
                    x.Content = $"**Занят  {9 - messagesCount}** слот, **осталось {8 - messagesCount}** слотов");
            else if (messagesCount == 3 || messagesCount == 4)
                await message.ModifyAsync(x =>
                    x.Content = $"**Занято {9 - messagesCount}** слота, **осталось {8 - messagesCount}** слотов");
            else if (messagesCount == 5)
                await message.ModifyAsync(x =>
                    x.Content = $"**Занято {9 - messagesCount}** слота, **осталось {8 - messagesCount}** слота");
            else if (messagesCount == 6 || messagesCount == 7)
                await message.ModifyAsync(x =>
                    x.Content = $"**Занято {9 - messagesCount}** слотов, **осталось {8 - messagesCount}** слота");
            else if (messagesCount == 8)
                await message.ModifyAsync(x =>
                    x.Content = $"**Занято {9 - messagesCount}** слотов, **остался {8 - messagesCount}** слот");
            else if (messagesCount == 9)
                await message.ModifyAsync(x =>
                    x.Content = $"**Занято {9 - messagesCount}** слотов, **осталось {8 - messagesCount}** слотов");
            else if (messagesCount == 11 || messagesCount == 12)
                await message.ModifyAsync(x =>
                    x.Content =
                        $"**Занято {8}** слотов, **осталось {0}** слотов/**{19 - messagesCount} слотов резерва**");
            else if (messagesCount == 13)
                await message.ModifyAsync(x =>
                    x.Content =
                        $"**Занято {8}** слотов, **осталось {0}** слотов/**{19 - messagesCount} слотов резерва**");
            else if (messagesCount == 14 || messagesCount == 15)
                await message.ModifyAsync(x =>
                    x.Content =
                        $"**Занято {8}** слотов, **осталось {0}** слотов/**{19 - messagesCount} слота резерва**");
            else if (messagesCount == 16)
                await message.ModifyAsync(x =>
                    x.Content =
                        $"**Занято {8}** слотов, **осталось {0}** слотов/**{19 - messagesCount} слота резерва**");
            else if (messagesCount == 17)
                await message.ModifyAsync(x =>
                    x.Content =
                        $"**Занято {8}** слотов, **осталось {0}** слотов/**{19 - messagesCount} слот резерва**");
            else if (messagesCount == 18)
                await message.ModifyAsync(x =>
                    x.Content =
                        $"**Занято {8}** слотов, **осталось {0}** слотов/**{19 - messagesCount} слотов резерва**");

            await component.Message.DeleteAsync();
            await finishRegistrationChannel.SendMessageAsync(embed: eb.Build());

            messages = await finishRegistrationChannel.GetMessagesAsync(100, CacheMode.AllowDownload).FlattenAsync();
            messagesCount = messages.Count() - 1;

            if (messagesCount == 9)
            {
                await finishRegistrationChannel.SendMessageAsync("**------ ДАЛЬШЕ ИДЕТ РЕЗЕРВ!!! ------**");
            }

            if (messagesCount == 19)
            {
                await finishRegistrationChannel.SendFileAsync(
                    @"C:\Users\User\Desktop\for Enrage DS server\rega_zakryta.png", "Увидимся через неделю !");
            }
        }
    }

    public class RegistrationModal : IModal
    {
        public string Title => "Регистрация на турнир";

        [InputLabel("Название команды")]
        [ModalTextInput("team_name", TextInputStyle.Short, "Кто вы ?", 1, 50, null)]
        public string TeamName { get; set; }

        [InputLabel("Указать ссылку для связи с капитаном (ВК)")]
        [ModalTextInput("capitan_link", TextInputStyle.Short, "Укажите ссылку", 1, 100, null)]
        public string CapitanLink { get; set; }

        [InputLabel("Dotabuff/Stratz игроков через ENTER")]
        [ModalTextInput("players_db", TextInputStyle.Paragraph, "Профили должен быть открыты", 1, 4000, null)]
        public string PlayersDotabuff { get; set; }

        [InputLabel("Вк игроков через ENTER")]
        [ModalTextInput("players_vk", TextInputStyle.Paragraph, "если нет вк - написать не использует", 1, 4000, null)]
        public string PlayersVK { get; set; }

        [InputLabel("Discord игроков через ENTER")]
        [ModalTextInput("players_dis", TextInputStyle.Paragraph, "именно nickname а не профиль на сервере", 1, 4000,
            null)]
        public string PlayersDiscord { get; set; }
    }

    public class TeamTroublesOrReplacementModal : IModal
    {
        public string Title => "Замены или спорная ситуация";

        [InputLabel("Dotabuff/Stratz игроков через ENTER")]
        [ModalTextInput("players_db", TextInputStyle.Paragraph, "Профили должен быть открыты", 1, 4000, null)]
        public string ReplacementPlayersDotabuff { get; set; }

        [InputLabel("Вк игроков через ENTER")]
        [ModalTextInput("players_vk", TextInputStyle.Paragraph, "если нет вк - написать 'не использует'", 1, 4000,
            null)]
        public string ReplacementPlayersVK { get; set; }

        [InputLabel("Discord игроков через ENTER")]
        [ModalTextInput("players_dis", TextInputStyle.Paragraph, "именно nickname а не профиль на сервере", 1, 4000,
            null)]
        public string ReplacementPlayersDiscord { get; set; }

        [InputLabel("Если возникла спорная ситуация описать её")]
        [ModalTextInput("capitan_link", TextInputStyle.Paragraph, "Спорная ситуация(при отсутствии поставить '-')", 1,
            4000, null)]
        public string TeamTroublesDescription { get; set; }
    }
}