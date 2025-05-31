using Discord.Commands;
using Discord;
using Discord.WebSocket;
using System.Globalization;

namespace EnrageDiscordTournamentBot.Modules
{
    public class PrefixModule : ModuleBase<SocketCommandContext>
    {
        private readonly DiscordSocketClient _client;

        public PrefixModule(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("ping")]
        public async Task Pong()
        {
            // Reply to the user's message with the response
            await Context.Message.ReplyAsync("Pong!");
        }

        [Command("mute")]
        public async Task Mute(SocketGuildUser user, string time, string reason)
        {
            string[] reasonArray = reason.Split('_');
            reason = String.Empty;

            for (int i = 0; i < reasonArray.Length; i++)
            {
                reason += $"{reasonArray[i]} ";
            }

            TimeSpan timeSpan = TimeSpan.ParseExact(time, @"h\:mm", CultureInfo.InvariantCulture);
            await user.SendMessageAsync($"Вам был выдан тайм-аут по причине: {reason}. Администратор, выдавший наказание - {Context.User}");
            await user.SetTimeOutAsync(timeSpan);
        }

        [Command("ban")]
        public async Task Ban(SocketGuildUser user, string reason)
        {
            if (Context.User.Id == 828694994499010630 || Context.User.Id == 609287904539312130)
            {
                string[] reasonArray = reason.Split('_');
                reason = String.Empty;

                for (int i = 0; i < reasonArray.Length; i++)
                {
                    reason += $"{reasonArray[i]} ";
                }

                await user.SendMessageAsync($"Вам был выдан бан по причине: {reason}. Администратор, выдавший наказание - {Context.User}");
                await user.BanAsync(0, reason);
            }
            else
            {
                await Context.Message.DeleteAsync();
            }
        }
    }
}
