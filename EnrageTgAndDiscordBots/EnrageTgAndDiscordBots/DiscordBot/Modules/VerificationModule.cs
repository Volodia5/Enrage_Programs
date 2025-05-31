using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DotA2StatsParser.Model.Dotabuff;
using DSharpPlus.Entities;
using EnrageDiscordTournamentBot.Bot;
using EnrageDiscordTournamentBot.Log;
using System;
using System.Reflection.Emit;

namespace EnrageDiscordTournamentBot.Modules
{
    public class VerificationModule : InteractionModuleBase<SocketInteractionContext>
    {
        private Logger _logger;
        private DiscordSocketClient _client;
        StratzParserModule _stratzParser = new StratzParserModule();

        public VerificationModule(ConsoleLogger logger, DiscordSocketClient client)
        {
            _logger = logger;
            _client = client;

            _client.MessageReceived += OnMessageRecieved;
            _client.SelectMenuExecuted += OnSelectMenuExecuted;
        }

        private async Task OnSelectMenuExecuted(SocketMessageComponent component)
        {
            var guild = _client.GetGuild(1075718003578126386);
            string userId = new string(component.Message.Content.Where(x => char.IsDigit(x)).ToArray());
            SocketGuildUser guildUser = guild.GetUser(ulong.Parse(userId));
            ulong roleId = 0;

            foreach (var item in component.Data.Values)
            {
                roleId = ulong.Parse(item.Where(x => char.IsDigit(x)).ToArray());
            }

            await guildUser.RemoveRoleAsync(1206360732292223016);
            await guildUser.AddRoleAsync(1096163693806502018);
            await guildUser.AddRoleAsync(roleId); 
            await component.RespondAsync($"Роль <@&{roleId}> была выдана пользователю <@{guildUser.Id}> успешно!", ephemeral: true);
        }

        [DefaultMemberPermissions(GuildPermission.Administrator)]
        [SlashCommand("test-stratz-query", "тестовый запрос стратз")]
        public async Task TestQuery(string accountId)
        {
            StratzParserModule parserModule = new StratzParserModule();
            string rank = await parserModule.GetRank(accountId);
            await RespondAsync(rank, ephemeral: true);
        }

        [DefaultMemberPermissions(GuildPermission.Administrator)]
        [SlashCommand("verif-user", "Верифицирует определенного игрока")]
        public async Task VerifUserOnCommand(SocketGuildUser user)
        {
            var guild = _client.GetGuild(1075718003578126386);
            List<ulong> roles = new List<ulong>()
            {
                1096124412832530452,
                1096124472983031878,
                1096124783382507530,
                1096124828743909446,
                1096124878110851082,
                1096124920154574868,
                1096124957379002380,
                1096125008931192832,
                1096125062702178454,
                1096125287273598987,
                1096125371709128814,
                1096125446392918086,
                1096125508376334336
            };
            EmbedBuilder embedBuilder = new EmbedBuilder()
                .WithAuthor("Enrage bot")
                .WithDescription("``` Выдача роли ```")
                .WithThumbnailUrl(user.GetAvatarUrl())
                .AddField(name: "> Пользователь: ", value: $"- <@{user.Id}>\n- {user.Id}");
            
            var selectMenu = new SelectMenuBuilder()
                .WithCustomId("select-menu-roles")
                .WithPlaceholder("Выберите роль для выдачи")
                .WithMinValues(1)
                .WithMaxValues(1);

            foreach (var item  in roles)
            {
                SocketRole role = guild.GetRole(item);
                selectMenu.AddOption($"{role.Name}", $"Id роли: {role.Id}");
            }

            var builder = new ComponentBuilder().WithSelectMenu(selectMenu);

            await RespondAsync($"Выберите роль для выдачи пользователю <@{user.Id}>", embed: embedBuilder.Build() ,components: builder.Build(), ephemeral: true);
        }

        [DefaultMemberPermissions(GuildPermission.Administrator)]
        [SlashCommand("verif-users", "Верифицирует игроков")]
        public async Task VerifUsers(int countmessages)
        {
            if (Context.Channel.Id == 1286991552526417992)
            {
                var guild = _client.GetGuild(1075718003578126386);
                ITextChannel finishRegistrationChannel = (ITextChannel)_client.GetChannel(1286991552526417992);
                IEnumerable<IMessage> messages = await finishRegistrationChannel
                    .GetMessagesAsync(countmessages, CacheMode.AllowDownload).FlattenAsync();

                foreach (var item in messages)
                {
                    VerifOnCommand(item);
                }

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

            if (userPermissoions.ManageChannel != true)
            {
                VerifOnCommand(message);
            }
            else
            {
                return;
            }
        }

        private async Task VerifOnCommand(IMessage message)
        {
            string splitString;
            string userAccountRatingString;

            if (message.Channel.Id == 1286991552526417992)
            {
                string messageText = message.Content.ToString();
                int tgPhotoInMessage = message.Attachments.Count;

                try
                {
                    if (tgPhotoInMessage == 1)
                    {
                        splitString = messageText.Split("1)")[1];
                        char splitSymbol = ')';

                        await CheckIsCorrectId(message, splitString, messageText, "2" + splitSymbol);

                        return;
                    }
                    else
                    {
                        await WriteExeptIncorrectVerifMessage(message, messageText);

                        return;
                    }
                }
                catch (Exception exept)
                {
                    try
                    {
                        if (tgPhotoInMessage == 1)
                        {
                            splitString = messageText.Split("1.")[1];
                            char splitSymbol = '.';

                            await CheckIsCorrectId(message, splitString, messageText, "2" + splitSymbol);

                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            if (tgPhotoInMessage == 1)
                            {
                                userAccountRatingString = messageText.Split("\n")[0];
                                string splitSymbol = "\n";

                                await CheckIsCorrectId(message, userAccountRatingString, messageText, splitSymbol);

                                return;
                            }
                        }
                        catch (Exception e)
                        {
                            if (tgPhotoInMessage == 1)
                            {
                                using (var httpClient = new System.Net.Http.HttpClient())
                                {
                                    using (var attachment =
                                           await httpClient.GetStreamAsync(message.Attachments.First().Url))
                                    {
                                        await WriteExeptIncorrectVerifMessage(message, messageText);

                                        return;
                                    }
                                }
                            }
                            else
                            {
                                await WriteExeptIncorrectVerifMessage(message, messageText);

                                return;
                            }
                        }
                    }
                }
            }
        }

        public async Task CheckIsCorrectId(IMessage message, string splitString, string messageText, string splitSymbol)
        {
            string userAccountRatingString;
            int userAccountRatingInt;
            userAccountRatingString = splitString.Split("\n")[0];
            string userParsedRank = await _stratzParser.GetRank(userAccountRatingString);

            if (userParsedRank == ErrorApiCodes.SeasonRank.ToString())
            {

                userAccountRatingString = messageText.Split(splitSymbol)[1];
                userAccountRatingInt = int.Parse(userAccountRatingString.Split('\n')[0]);

                await AddingRolesToUser(message, userAccountRatingInt);
                await message.Author.SendMessageAsync("Вы успешно верифицированы. Прекрасного времяпрепровождения на сервере!");

                return;
            }
            else if (userParsedRank == ErrorApiCodes.InvalidOrHideId.ToString())
            {
                await WriteExeptIncorrectVerifMessage(message, messageText, ErrorApiCodes.InvalidOrHideId);

                return;
            }
            else if (userParsedRank == ErrorApiCodes.SteamId.ToString())
            {
                await WriteExeptIncorrectVerifMessage(message, messageText, ErrorApiCodes.SteamId);

                return;
            }
            else
            {
                userParsedRank = userParsedRank.ToString().Substring(0, 1);
                userAccountRatingInt = int.Parse(userParsedRank);

                await AddingRolesToUser(message, userAccountRatingInt);
                await message.Author.SendMessageAsync("Вы успешно верифицированы. Прекрасного времяпрепровождения на сервере!");
            }
        }

        public async Task WriteExeptIncorrectVerifMessage(IMessage? iMessage, string messageText, ErrorApiCodes? errorCode = null)
        {
            string photoOpeningDotaAccountServerPath = "/root/EnrageTgAndDiscordBots/EnrageTgAndDiscordBots/openingAccountInfo.jpg";
            string photoOpeningDotaAccountPCPath = "C:\\Users\\Пользователь\\Desktop\\openingAccountInfo.jpg";
            string photoFindingDotaAccIdServerPath = "/root/EnrageTgAndDiscordBots/EnrageTgAndDiscordBots/howToFindAccId.jpg";
            string photoFindingDotaAccIdPCPath = "C:\\Users\\Пользователь\\Desktop\\howToFindAccId.jpg";

            if (errorCode != null)
            {
                if (errorCode == ErrorApiCodes.InvalidOrHideId)
                {
                    await iMessage.Author.SendMessageAsync("## При возникновении проблем обращайтесь в телеграм @vladimirrogozn\n\nВаш Dota аккаунт помечен как приватный. Сделайте ваш аккаунт публичным в настройках Dota2 (как это сделать показано на вложении ниже 👇) и **повторите попытку через +- 1,5 часа.**");
                    await iMessage.Author.SendFileAsync(photoOpeningDotaAccountServerPath);
                    await iMessage.Author.SendMessageAsync($"## При возникновении проблем обращайтесь в телеграм @vladimirrogozn\n\nВаша заявка была - {messageText}");
                    await iMessage.Author.SendMessageAsync("## При возникновении проблем обращайтесь в телеграм @vladimirrogozn\n\nЕсли вас уже верифицировали и это сообщение пришло ошибочно - проигнорируйте его.");
                    await iMessage.DeleteAsync();

                    return;
                }
                else
                {
                    await iMessage.Author.SendMessageAsync("## При возникновении проблем обращайтесь в телеграм @vladimirrogozn\n\nВы указали неверный ID аккаунта Dota2. Как найти ID акканута показано на вложении ниже 👇.");
                    await iMessage.Author.SendFileAsync(photoFindingDotaAccIdServerPath);
                    await iMessage.Author.SendMessageAsync($"## При возникновении проблем обращайтесь в телеграм @vladimirrogozn\n\nВаша заявка была - {messageText}");
                    await iMessage.Author.SendMessageAsync("## При возникновении проблем обращайтесь в телеграм @vladimirrogozn\n\nЕсли вас уже верифицировали и это сообщение пришло ошибочно - проигнорируйте его.");
                    await iMessage.DeleteAsync();

                    return;
                }
            }
            else if (iMessage.Attachments.Count == 1)
            {
                using (var httpClient = new System.Net.Http.HttpClient())
                {
                    using (var attachment = await httpClient.GetStreamAsync(iMessage.Attachments.First().Url))
                    {
                        await iMessage.Author.SendMessageAsync(
                            $"## При возникновении проблем обращайтесь в телеграм @vladimirrogozn\n\nНекорректная заявка на верификацию. Укажите свои данные еще раз. Просьба указывать свои данные согласно примеру:\n1) id аккаунта\n2) количество ммр\n3) прикрепленний скриншот подписки на наш тг канал. \n\n Ваша заявка была - {messageText} \n**Заявка пишется 1 целым сообщением**");
                        await iMessage.Author.SendMessageAsync("## При возникновении проблем обращайтесь в телеграм @vladimirrogozn\n\nВаш скриншот  👇");
                        await iMessage.Author.SendFileAsync(attachment, "our_screenshot.png");
                        await iMessage.Author.SendMessageAsync(
                            "## При возникновении проблем обращайтесь в телеграм @vladimirrogozn\n\nЕсли вас уже верифицировали и это сообщение пришло ошибочно - проигнорируйте его");
                        await iMessage.DeleteAsync();

                        return;
                    }
                }
            }
            else
            {
                await iMessage.Author.SendMessageAsync(
                    $"## При возникновении проблем обращайтесь в телеграм @vladimirrogozn\n\nНекорректная заявка на верификацию. Укажите свои данные еще раз. Просьба указывать свои данные согласно примеру:\n1) id аккаунта\n2) количество ммр\n3) прикрепленний скриншот подписки на наш тг канал. \n\n Ваша заявка была - {messageText} \n**Заявка пишется 1 целым сообщением**");
                await iMessage.Author.SendMessageAsync(
                    "## При возникновении проблем обращайтесь в телеграм @vladimirrogozn\n\nЕсли вас уже верифицировали и это сообщение пришло ошибочно - проигнорируйте его");
                await iMessage.DeleteAsync();

                return;
            }
        }

        private async Task AddingRolesToUser(IMessage? iMessage, int userRating)
        {
            List<ulong> roles = new List<ulong>()
            {
                1096124412832530452,
                1096124472983031878,
                1096124783382507530,
                1096124828743909446,
                1096124878110851082,
                1096124920154574868,
                1096124957379002380,
                1096125008931192832,
                1096125062702178454,
                1096125287273598987,
                1096125371709128814,
                1096125446392918086,
                1096125508376334336
            };
            var guild = _client.GetGuild(1075718003578126386);
            IEmote emote = await guild.GetEmoteAsync(1210198064669917184);
            ulong userId = iMessage.Author.Id;
            SocketGuildUser user = guild.GetUser(userId);
            var userRoles = user.Roles.ToList();

            foreach (var userRoleId in userRoles)
            {
                foreach (var item in roles)
                {
                    if (userRoleId.Id == item)
                    {
                        await user.RemoveRoleAsync(item);
                    }
                }

            }

            if (userRating == 0)
            {
                await user.RemoveRoleAsync(1206360732292223016);
                await user.AddRoleAsync(1096163693806502018);
                await iMessage.AddReactionAsync(emote);
            }
            else if (userRating == 1)
            {
                await user.RemoveRoleAsync(1206360732292223016);
                await user.AddRoleAsync(1096163693806502018);
                await user.AddRoleAsync(1096124412832530452);
                await iMessage.AddReactionAsync(emote);
            }
            else if (userRating == 2)
            {
                await user.RemoveRoleAsync(1206360732292223016);
                await user.AddRoleAsync(1096163693806502018);
                await user.AddRoleAsync(1096124472983031878);
                await iMessage.AddReactionAsync(emote);
            }
            else if (userRating == 3)
            {
                await user.RemoveRoleAsync(1206360732292223016);
                await user.AddRoleAsync(1096163693806502018);
                await user.AddRoleAsync(1096124783382507530);
                await iMessage.AddReactionAsync(emote);
            }
            else if (userRating == 4)
            {
                await user.RemoveRoleAsync(1206360732292223016);
                await user.AddRoleAsync(1096163693806502018);
                await user.AddRoleAsync(1096124828743909446);
                await iMessage.AddReactionAsync(emote);
            }
            else if (userRating == 5)
            {
                await user.RemoveRoleAsync(1206360732292223016);
                await user.AddRoleAsync(1096163693806502018);
                await user.AddRoleAsync(1096124878110851082);
                await iMessage.AddReactionAsync(emote);
            }
            else if (userRating == 6)
            {
                await user.RemoveRoleAsync(1206360732292223016);
                await user.AddRoleAsync(1096163693806502018);
                await user.AddRoleAsync(1096124920154574868);
                await iMessage.AddReactionAsync(emote);
            }
            else if (userRating == 7)
            {
                await user.RemoveRoleAsync(1206360732292223016);
                await user.AddRoleAsync(1096163693806502018);
                await user.AddRoleAsync(1096124957379002380);
                await iMessage.AddReactionAsync(emote);
            }
            else if (userRating == 8)
            {
                await user.RemoveRoleAsync(1206360732292223016);
                await user.AddRoleAsync(1096163693806502018);
                await user.AddRoleAsync(1096125008931192832);
                await iMessage.AddReactionAsync(emote);
            }
            else if (userRating > 6000 && userRating <= 7000)
            {
                await user.RemoveRoleAsync(1206360732292223016);
                await user.AddRoleAsync(1096163693806502018);
                await user.AddRoleAsync(1096125062702178454);
                await iMessage.AddReactionAsync(emote);
            }
            else if (userRating > 7000 && userRating <= 8000)
            {
                await user.RemoveRoleAsync(1206360732292223016);
                await user.AddRoleAsync(1096163693806502018);
                await user.AddRoleAsync(1096125287273598987);
                await iMessage.AddReactionAsync(emote);
            }
            else if (userRating > 8000 && userRating <= 9000)
            {
                await user.RemoveRoleAsync(1206360732292223016);
                await user.AddRoleAsync(1096163693806502018);
                await user.AddRoleAsync(1096125371709128814);
                await iMessage.AddReactionAsync(emote);
            }
            else if (userRating > 9000 && userRating <= 10000)
            {
                await user.RemoveRoleAsync(1206360732292223016);
                await user.AddRoleAsync(1096163693806502018);
                await user.AddRoleAsync(1096125446392918086);
                await iMessage.AddReactionAsync(emote);
            }
            else if (userRating > 10000)
            {
                await user.RemoveRoleAsync(1206360732292223016);
                await user.AddRoleAsync(1096163693806502018);
                await user.AddRoleAsync(1096125508376334336);
                await iMessage.AddReactionAsync(emote);
            }

            //if (userId == 0)
            //{
            //    await user.AddRoleAsync(1096163693806502018);
            //    await user.RemoveRoleAsync(1206360732292223016);
            //    await iMessage.AddReactionAsync(emote);
            //}
            //else if (userId > 1 && userId <= 770)
            //{
            //    await user.AddRoleAsync(1096163693806502018);
            //    await user.AddRoleAsync(1096124412832530452);
            //    await user.RemoveRoleAsync(1206360732292223016);
            //    await iMessage.AddReactionAsync(emote);
            //}
            //else if (userId > 770 && userId <= 1540)
            //{
            //    await user.AddRoleAsync(1096163693806502018);
            //    await user.AddRoleAsync(1096124472983031878);
            //    await user.RemoveRoleAsync(1206360732292223016);
            //    await iMessage.AddReactionAsync(emote);
            //}
            //else if (userId > 1540 && userId <= 2310)
            //{
            //    await user.AddRoleAsync(1096163693806502018);
            //    await user.AddRoleAsync(1096124783382507530);
            //    await user.RemoveRoleAsync(1206360732292223016);
            //    await iMessage.AddReactionAsync(emote);
            //}
            //else if (userId > 2310 && userId <= 3080)
            //{
            //    await user.AddRoleAsync(1096163693806502018);
            //    await user.AddRoleAsync(1096124828743909446);
            //    await user.RemoveRoleAsync(1206360732292223016);
            //    await iMessage.AddReactionAsync(emote);
            //}
            //else if (userId > 3080 && userId <= 3850)
            //{
            //    await user.AddRoleAsync(1096163693806502018);
            //    await user.AddRoleAsync(1096124878110851082);
            //    await user.RemoveRoleAsync(1206360732292223016);
            //    await iMessage.AddReactionAsync(emote);
            //}
            //else if (userId > 3850 && userId <= 4620)
            //{
            //    await user.AddRoleAsync(1096163693806502018);
            //    await user.AddRoleAsync(1096124920154574868);
            //    await user.RemoveRoleAsync(1206360732292223016);
            //    await iMessage.AddReactionAsync(emote);
            //}
            //else if (userId > 4620 && userId <= 6000)
            //{
            //    await user.AddRoleAsync(1096163693806502018);
            //    await user.AddRoleAsync(1096125008931192832);
            //    await user.RemoveRoleAsync(1206360732292223016);
            //    await iMessage.AddReactionAsync(emote);
            //}
            //else if (userId > 6000 && userId <= 7000)
            //{
            //    await user.AddRoleAsync(1096163693806502018);
            //    await user.AddRoleAsync(1096125062702178454);
            //    await user.RemoveRoleAsync(1206360732292223016);
            //    await iMessage.AddReactionAsync(emote);
            //}
            //else if (userId > 7000 && userId <= 8000)
            //{
            //    await user.AddRoleAsync(1096163693806502018);
            //    await user.AddRoleAsync(1096125287273598987);
            //    await user.RemoveRoleAsync(1096125508376334336);
            //    await iMessage.AddReactionAsync(emote);
            //}
            //else if (userId > 8000 && userId <= 9000)
            //{
            //    await user.AddRoleAsync(1096163693806502018);
            //    await user.AddRoleAsync(1096125371709128814);
            //    await user.RemoveRoleAsync(1096125508376334336);
            //    await iMessage.AddReactionAsync(emote);
            //}
            //else if (userId > 9000 && userId <= 10000)
            //{
            //    await user.AddRoleAsync(1096163693806502018);
            //    await user.AddRoleAsync(1096125446392918086);
            //    await user.RemoveRoleAsync(1096125508376334336);
            //    await iMessage.AddReactionAsync(emote);
            //}
            //else if (userId > 10000)
            //{
            //    await user.AddRoleAsync(1096163693806502018);
            //    await user.AddRoleAsync(1096125508376334336);
            //    await user.RemoveRoleAsync(1096125508376334336);
            //    await iMessage.AddReactionAsync(emote);
            //}
        }
    }
}