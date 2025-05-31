using System;
using System.Threading;
using EnrageTgBotILovePchel.Bot.Router;
using EnrageTgBotILovePchel.Util.String;
using NLog;
using NLog.Fluent;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EnrageTgBotILovePchel.Bot;

public class BotInitializer
{
    private static ILogger Logger = LogManager.GetCurrentClassLogger();

    private TelegramBotClient _botClient;
    private CancellationTokenSource _cancellationTokenSource;

    public BotInitializer()
    {
        _botClient = new TelegramBotClient(SystemStringsStorage.TelegramToken);
        _cancellationTokenSource = new CancellationTokenSource();

        Logger.Info(
            $"Выполнена инициализация Бота с токеном {SystemStringsStorage.TelegramToken}");
    }

    public void Start()
    {
        ReceiverOptions receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        BotRequestHandlers botRequestHandlers = new BotRequestHandlers();

        _botClient.StartReceiving(
            botRequestHandlers.HandleUpdateAsync,
            botRequestHandlers.HandlePollingErrorAsync,
            receiverOptions,
            _cancellationTokenSource.Token
        );

        Logger.Info("Бот запущен");
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();

        Logger.Info("Бот остановлен");
    }
}