using System;
using EnrageTgBotILovePchel.Service;
using NLog;

namespace EnrageTgBotILovePchel.Bot.Router;

public class ChatsRouter
{
    private static ILogger Logger = LogManager.GetCurrentClassLogger();

    private ChatTransmittedDataPairs _chatTransmittedDataPairs;
    private ServiceManager _servicesManager;

    public ChatsRouter()
    {
        _servicesManager = new ServiceManager();
        _chatTransmittedDataPairs = new ChatTransmittedDataPairs();
    }

    public BotMessage Route(long chatId, string textData, string userName)
    {
        if (_chatTransmittedDataPairs.ContainsKey(chatId) == false)
        {
            _chatTransmittedDataPairs.CreateNew(chatId, userName);
        }

        TransmittedData transmittedData = _chatTransmittedDataPairs.GetByChatId(chatId);
        transmittedData.TgUsername = userName;

        Logger.Info($"ROUTER chatId = {chatId}; State = {transmittedData.State}");

        return _servicesManager.ProcessBotUpdate(textData, transmittedData);
    }
}