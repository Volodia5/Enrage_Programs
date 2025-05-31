using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using MySqlX.XDevAPI;
using EnrageDiscordTournamentBot.Log;
using Discord.Rest;

namespace EnrageDiscordTournamentBot.Modules
{
    public class ActionWithUserModule : InteractionModuleBase<SocketInteractionContext>
    {
        private Logger _logger;
        private DiscordSocketClient _client;

        public ActionWithUserModule(ConsoleLogger logger, DiscordSocketClient client)
        {
            _logger = logger;
            _client = client;

            _client.ButtonExecuted += MyButtonHandler;
        }

        [SlashCommand("ban", "забанить пользователя")]
        [DefaultMemberPermissions(GuildPermission.ModerateMembers)]
        private async Task BanUser(SocketGuildUser banningUser)
        {
            await banningUser.BanAsync();
            ITextChannel banChannel = (ITextChannel)_client.GetChannel(1236300301347192882);

            await banChannel.SendMessageAsync(
                $"бан выдан юзеру <@{banningUser.Id}> администратором/модератором <@{Context.User.Id}> !");
        }

        [SlashCommand("action", "возможные действия с пользователем")]
        [DefaultMemberPermissions(GuildPermission.ManageChannels)]
        private async Task HandleActionCommand(SocketGuildUser triggeredUser)
        {
            IGuildUser actionedUser = (IGuildUser)Context.User;
            IGuildUser user = triggeredUser;
            int countAdminRolesInteractedUser = 0;
            int countAdminRolesInteractiveUser = 0;

            foreach (var item in actionedUser.RoleIds)
            {
                if (item == 1094285945706131516 || item == 1188895312723591209 || item == 1215993005564231811 ||
                    item == 1094228018949537893 || item == 1124683691893985414 || item == 1087056785778683924)
                {
                    countAdminRolesInteractedUser++;
                }
            }

            foreach (var item in triggeredUser.Roles)
            {
                if (item.Id == 1094285945706131516 || item.Id == 1188895312723591209 ||
                    item.Id == 1215993005564231811 || item.Id == 1094228018949537893 ||
                    item.Id == 1124683691893985414 || item.Id == 1087056785778683924)
                {
                    countAdminRolesInteractiveUser++;
                }
            }

            if (countAdminRolesInteractiveUser == 0)
            {
                if (countAdminRolesInteractedUser > 0)
                {
                    var buttonsMenu = new ComponentBuilder()
                        .WithButton("voice mute", "voice-mute-button", ButtonStyle.Danger)
                        .WithButton("text mute", "text-mute-button", ButtonStyle.Danger)
                        .WithButton("ban", "ban-button", ButtonStyle.Secondary);

                    await RespondAsync($"Выберите действие с юзером <@{triggeredUser.Id}>",
                        components: buttonsMenu.Build(), ephemeral: true);
                }
                else
                {
                    await RespondAsync("У вас недостаточно прав для использования данной команды !", ephemeral: true);
                }
            }
            else
            {
                await RespondAsync("У вас недостаточно прав для использования данной команды над данным юзером!",
                    ephemeral: true);
            }
        }

        private async Task MyButtonHandler(SocketMessageComponent component)
        {
            switch (component.Data.CustomId)
            {
                case "voice-mute-button":
                    await AddMute(component, 1236301111619747901);
                    break;
                case "text-mute-button":
                    await AddMute(component, 1236301051888533564);
                    break;
                case "ban-button":
                    await AddBan(component);
                    break;
            }
        }

        private async Task AddBan(SocketMessageComponent component)
        {
            string userId = new string(component.Message.Content.Where(x => char.IsDigit(x)).ToArray());
            var guild = _client.GetGuild(1075718003578126386);
            ITextChannel banChannel = (ITextChannel)_client.GetChannel(1236300301347192882);

            await component.RespondAsync($"Вы выдали бан <@{userId}>", ephemeral: true);
            await guild.AddBanAsync(ulong.Parse(userId));
            await banChannel.SendMessageAsync(
                $"Бан выдан юзеру <@{userId}> администратором/модератором <@{component.User.Id}> !");
        }

        private async Task AddMute(SocketMessageComponent component, ulong muteRoleId)
        {
            string userId = new string(component.Message.Content.Where(x => char.IsDigit(x)).ToArray());
            var guild = _client.GetGuild(1075718003578126386);
            ITextChannel muteChannel = (ITextChannel)_client.GetChannel(1236300301347192882);
            var role = guild.GetRole(muteRoleId);
            SocketGuildUser guildUser = guild.GetUser(ulong.Parse(userId));
            IReadOnlyCollection<SocketRole> muteUserRole = guildUser.Roles;
            int counter = 0;

            foreach (var item in muteUserRole)
            {
                if (item.Id == role.Id)
                    counter++;
            }

            if (counter == 1)
            {
                await guildUser.RemoveRoleAsync(role.Id);
                await component.RespondAsync($"{role.Name} был снят пользователю <@{guildUser.Id}>!", ephemeral: true);
                await muteChannel.SendMessageAsync($"{role.Name} был снят пользователю <@{guildUser.Id}>");
            }
            else
            {
                await guildUser.AddRoleAsync(role.Id);
                await component.RespondAsync($"{role.Name} был выдан пользователю <@{guildUser.Id}>!", ephemeral: true);
                await muteChannel.SendMessageAsync($"{role.Name} был выдан пользователю <@{guildUser.Id}>");
            }
        }
    }
}