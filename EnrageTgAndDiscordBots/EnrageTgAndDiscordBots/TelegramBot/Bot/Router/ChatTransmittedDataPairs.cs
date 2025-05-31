using System.Collections.Generic;

namespace EnrageTgBotILovePchel.Bot.Router;

public class ChatTransmittedDataPairs
{
    private Dictionary<long, TransmittedData> _chatTransmittedDataPairs;

    public ChatTransmittedDataPairs()
    {
        _chatTransmittedDataPairs = new Dictionary<long, TransmittedData>();
    }

    public bool ContainsKey(long chatId)
    {
        return _chatTransmittedDataPairs.ContainsKey(chatId);
    }

    public void CreateNew(long chatId, string userName)
    {
        _chatTransmittedDataPairs[chatId] = new TransmittedData(chatId, 0 , -1, userName);
    }

    public TransmittedData GetByChatId(long chatId)
    {
        return _chatTransmittedDataPairs[chatId];
    }
}
