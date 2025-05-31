using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using EnrageDiscordTournamentBot.Log;

namespace EnrageDiscordTournamentBot.Modules
{
    
    public class TeamsModule : InteractionModuleBase<SocketInteractionContext>
    {
        private Logger _logger;
        private DiscordSocketClient _client;
        private SocketUser _interactedUser;
        private ITextChannel _lastUseChannel;
        
        public TeamsModule(ConsoleLogger logger, DiscordSocketClient client)
        {
            _logger = logger;
            _client = client;
        }
        
        [DefaultMemberPermissions(GuildPermission.Administrator)]
        [SlashCommand("clear-teams-category", "Удаление всех турнирных каналов и ролей команд")]
        public async Task ClearTeamsCategory()
        {
            ComponentBuilder buttons = new ComponentBuilder()
                .WithButton("Да", "yes_clear_button", ButtonStyle.Danger)
                .WithButton("Нет", "no_clear_button", ButtonStyle.Success);

          await RespondAsync("Вы уверены что хотите удалить турнирные каналы и роли ?", components: buttons.Build(), ephemeral: true);
        }

        [ComponentInteraction("yes_clear_button")]
        private async Task ConfirmClearChannel()
        {
            
            await RespondAsync("Удаление каналов было начато. Ожидайте!", ephemeral: true);
            
            SocketGuild guild = _client.GetGuild(1075718003578126386);
            SocketCategoryChannel firstCategory = guild.GetCategoryChannel(1209193088581505074);
            var channelsInFirstCategory = firstCategory.Channels;
            // SocketCategoryChannel secondCategory = guild.GetCategoryChannel(1209217906215358584);
            // var channelsInSecondCategory = secondCategory.Channels;
            var guildRoles = guild.Roles;

            foreach (var item in channelsInFirstCategory)
            {
                foreach (var role in guildRoles)
                {
                    if (role.Name == item.Name)
                    {
                        await role.DeleteAsync();
                    }
                }

                await item.DeleteAsync();
            }

            // foreach (var item in channelsInSecondCategory)
            // {
            //     foreach (var role in guildRoles)
            //     {
            //         if (role.Name == item.Name)
            //         {
            //             await role.DeleteAsync();
            //         }
            //     }
            //
            //     await item.DeleteAsync();
            // }

            await RespondAsync("Каналы и роли успешно удалены !!!", ephemeral: true);
        }

        [ComponentInteraction("no_clear_button")]
        public async Task CancelClearChannel()
        {
            await RespondAsync("Операция отменена", ephemeral: true);
        }
        
        [SlashCommand("add-suspicion-command", "Пометить подозрительную команду")]
        public async Task UpdateTeam(SocketRole role)
        {
            IEnumerable<SocketGuildUser> teamUsers = role.Members;
            List<SocketGuildUser> users = new List<SocketGuildUser>();
            int counter = 0;

            foreach (var item in teamUsers)
            {
                users[counter] = item;
                counter++;
            }

            for (int i = 0; i < users.Count; i++)
            {
                await users[i].AddRoleAsync(1160513583352905748);
            }

            await RespondAsync("Команда добавлена в список подозриельных", ephemeral: true);
        }
        
        [DefaultMemberPermissions(GuildPermission.Administrator)]
        [SlashCommand("add-team", "Добавление ролей и комнат для команд")]
        public async Task AddTeam(string teamName,
                                   SocketGuildUser user1 = null,
                                   SocketGuildUser user2 = null,
                                   SocketGuildUser user3 = null,
                                   SocketGuildUser user4 = null,
                                   SocketGuildUser user5 = null,
                                   SocketGuildUser user6 = null,
                                   SocketGuildUser user7 = null,
                                   SocketGuildUser user8 = null)
        {
            SocketGuild guild = _client.GetGuild(1075718003578126386);
            IRole everyoneRole = guild.GetRole(1075718003578126386);
            IRole justiceManRole = guild.GetRole(1096375522482724917);
            IRole unverifyRole = guild.GetRole(1206360732292223016);
            OverwritePermissions everyoneTeamVoicePermissions = new OverwritePermissions(
                PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Allow, PermValue.Deny,
                PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Allow, PermValue.Deny, PermValue.Deny,
                PermValue.Deny, PermValue.Allow, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Allow,
                PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Allow, PermValue.Allow, PermValue.Allow,
                PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny,
                PermValue.Deny, PermValue.Deny, PermValue.Deny);
            OverwritePermissions teamVoicePermissions = new OverwritePermissions(
                PermValue.Allow, PermValue.Deny, PermValue.Deny, PermValue.Allow, PermValue.Allow, PermValue.Deny,
                PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Allow, PermValue.Deny, PermValue.Deny,
                PermValue.Allow, PermValue.Allow, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Allow,
                PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Allow, PermValue.Allow, PermValue.Allow,
                PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny,
                PermValue.Deny, PermValue.Deny, PermValue.Deny);
            OverwritePermissions unverifyTeamVoicePermissions = new OverwritePermissions(
                PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Allow, PermValue.Deny,
                PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Allow, PermValue.Deny, PermValue.Deny,
                PermValue.Deny, PermValue.Allow, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Allow,
                PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Allow, PermValue.Allow, PermValue.Allow,
                PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny,
                PermValue.Deny, PermValue.Deny, PermValue.Deny);
            
            RestRole teamRole = await guild.CreateRoleAsync(teamName);
            await RespondAsync("Канал и роль успешно созданы и настроены!", ephemeral:true);

            if (user1 != null)
                await user1.AddRoleAsync(teamRole.Id);
            if (user2 != null)
                await user2.AddRoleAsync(teamRole.Id);
            if (user3 != null)
                await user3.AddRoleAsync(teamRole.Id);
            if (user4 != null)
                await user4.AddRoleAsync(teamRole.Id);
            if (user5 != null)
                await user5.AddRoleAsync(teamRole.Id);
            if (user6 != null)
                await user6.AddRoleAsync(teamRole.Id);
            if (user7 != null)
                await user7.AddRoleAsync(teamRole.Id);
            if (user8 != null)
                await user8.AddRoleAsync(teamRole.Id);

            SocketCategoryChannel firstCategory = guild.GetCategoryChannel(1209193088581505074);
            int channelsInFirstCategoryCount = firstCategory.Channels.Count();
            //SocketCategoryChannel secondCategory = guild.GetCategoryChannel(1209193088581505074);
            //int channelsInSecondCategoryCount = secondCategory.Channels.Count();

            IGuildChannel teamVoice= await guild.CreateVoiceChannelAsync(teamName);
            await teamVoice.AddPermissionOverwriteAsync(everyoneRole, everyoneTeamVoicePermissions);
            await teamVoice.AddPermissionOverwriteAsync(teamRole, teamVoicePermissions);
            await teamVoice.AddPermissionOverwriteAsync(justiceManRole, teamVoicePermissions);
            await teamVoice.AddPermissionOverwriteAsync(unverifyRole, unverifyTeamVoicePermissions);
            if (channelsInFirstCategoryCount < 50)
                await teamVoice.ModifyAsync(x => x.CategoryId = 1209193088581505074);
            else 
                await teamVoice.ModifyAsync(x => x.CategoryId = 1209217906215358584);
        }
    }
}