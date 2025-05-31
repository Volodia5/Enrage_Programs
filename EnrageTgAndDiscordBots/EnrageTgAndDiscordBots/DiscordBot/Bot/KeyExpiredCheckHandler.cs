using System.Text;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using EnrageTgAndDiscordBots.Db.Repositories.Implemintations;
using EnrageTgAndDiscordBots.Db.Repositories.Interfaces;
using EnrageTgAndDiscordBots.DbConnector;
using EnrageTgAndDiscordBots.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ZstdSharp.Unsafe;

namespace EnrageDiscordTournamentBot.Bot;

public class KeyExpiredCheckHandler : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IServiceProvider _serviceProvider;
    private CancellationTokenSource _cancellationTokenSource;
    private EnrageBotVovodyaDbContext _db = new EnrageBotVovodyaDbContext();
    private IProgramActivationCodeDatasRepository _keyData;
    
    public KeyExpiredCheckHandler(DiscordSocketClient client, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _keyData = new ProgramActivationCodeDatasRepository(_db);

        _cancellationTokenSource = new CancellationTokenSource();
        Task.Run(() => CheckKeyExpireDate(_cancellationTokenSource.Token));
    }

    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [SlashCommand("create-program-key", "создания ключа для активации enrage checker")]
    private async Task AddProgramKey(DateTime expiredDateTime)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+[]{}|;:,.<>?";
        var random = new Random();
        var result = new StringBuilder(20);

        for (int i = 0; i < 20; i++)
        {
            result.Append(chars[random.Next(chars.Length)]);
        }

        _keyData.AddKey(result.ToString(), expiredDateTime);
        
        await RespondAsync($"Сгенерированный код: \n``` {result} ```\n Дата удаления кода: \n```{expiredDateTime}```", ephemeral:true);
    }
    
    
    private async Task? CheckKeyExpireDate(CancellationToken token)
    {
        Console.WriteLine("Зашли проверить ключи");
        
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EnrageBotVovodyaDbContext>();

        var expiredKeys = _keyData.GetAllKeys();

        foreach (var item in expiredKeys)
        {
            if(item.ExpiredTime < DateTime.Now)
                _keyData.DeleteKey(item.Key);
            // dbContext.ProgramKeys.RemoveRange(expiredKeys);
            // await dbContext.SaveChangesAsync();
        }
        
        // while (!token.IsCancellationRequested)
        // {
        //     var keys = _keyData.GetAllKeys();
        //     
        //     for (int i = 0; i < keys.Count; i++)
        //     {
        //         if (keys[i].ExpiredTime <= DateTime.Now)
        //         {
        //             _keyData.DeleteKey(keys[i].Key);
        //         }
        //     }
        // }

        await Task.Delay(TimeSpan.FromHours(1), token);
    }
}