using Discord.Interactions;
using Discord.WebSocket;
using EnrageDiscordTournamentBot.Log;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration.Json;
using Discord.Commands;
using Discord;
using System.Reflection;
using EnrageDiscordTournamentBot.Bot;
using EnrageTgAndDiscordBots.DbConnector;
using MySql.Data.MySqlClient;
//using EnrageDiscordTournamentBot.DBModels;
using EnrageTgBotILovePchel.Bot;
using Microsoft.EntityFrameworkCore;
using InteractionHandler = EnrageDiscordTournamentBot.InteractionHandler;
using PrefixHandler = EnrageDiscordTournamentBot.PrefixHandler;

namespace EnrageTgAndDiscordBots
{
    public class Program
    {
        private DiscordSocketClient _client;

        public static Task Main(string[] args) => new Program().MainAsync();

        public async Task MainAsync()
        {
            BotInitializer bot = new BotInitializer();
            bot.Start();
            TaskCompletionSource tcs = new TaskCompletionSource();

            var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("config.json")
            .Build();

            AppDomain.CurrentDomain.ProcessExit += (_, _) =>
            {
                bot.Stop();
                Console.WriteLine("Bot stopped");
                tcs.SetResult();
            };

            IHost host = Hosting(config);
            
            await RunAsync(host, tcs);
        }

        private static IHost Hosting(IConfigurationRoot config)
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
            services
            .AddSingleton(config)
            .AddDbContext<EnrageBotVovodyaDbContext>(options => options.UseSqlServer(config.GetConnectionString("Host=83.147.246.87:5432;Database=enrage_bot_vovodya_db;Username=enrage_bot_vovodya_user;Password=12345")))
            .AddSingleton<KeyExpiredCheckHandler>()
            .AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.All,
                UseInteractionSnowflakeDate = false,
                HandlerTimeout = null,
                LogGatewayIntentWarnings = false,
                AlwaysDownloadUsers = true,
                LogLevel = LogSeverity.Debug
            }))
            .AddTransient<ConsoleLogger>()
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<InteractionHandler>()
            .AddSingleton(x => new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Debug,
                DefaultRunMode = Discord.Commands.RunMode.Async
            }))
            .AddSingleton<PrefixHandler>())
            .Build();
        }

        public async Task RunAsync(IHost host, TaskCompletionSource tcs)
        {
            IServiceScope serviceScope;
            IServiceProvider provider;
            ServiceScoping(host, out serviceScope, out provider);

            var commands = provider.GetRequiredService<InteractionService>();
            _client = provider.GetRequiredService<DiscordSocketClient>();
            var config = provider.GetRequiredService<IConfigurationRoot>();

            await provider.GetRequiredService<InteractionHandler>().InitializeAsync();

            var prefixCommands = provider.GetRequiredService<PrefixHandler>();
            prefixCommands.AddModule<EnrageDiscordTournamentBot.Modules.PrefixModule>();
            await prefixCommands.InitializeAsync();

            _client.Log += _ => provider.GetRequiredService<ConsoleLogger>().Log(_);
            commands.Log += _ => provider.GetRequiredService<ConsoleLogger>().Log(_);

            _client.Ready += async () =>
            {
                if (IsDebug())
                    await commands.RegisterCommandsToGuildAsync(ulong.Parse(config["testGuild"]), true);
                else
                    await commands.RegisterCommandsGloballyAsync(true);
            };

            await _client.LoginAsync(TokenType.Bot, config["tokens"]);
            await _client.StartAsync();
            await tcs.Task;

            await Task.Delay(-1);
        }

        private static void ServiceScoping(IHost host, out IServiceScope serviceScope, out IServiceProvider provider)
        {
            serviceScope = host.Services.CreateScope();
            provider = serviceScope.ServiceProvider;
        }

        // public async Task ConnectToDb()
        // {
        //     try
        //     {
        //         MySqlConnection connection = DBUtils.GetDBConnection();
        //         await connection.OpenAsync();
        //         Console.WriteLine("Connected to DaB !");
        //         await connection.CloseAsync();
        //     }
        //     catch
        //     {
        //         Console.WriteLine("Can`t connect to DaB !");
        //     }
        // }

        static bool IsDebug()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}