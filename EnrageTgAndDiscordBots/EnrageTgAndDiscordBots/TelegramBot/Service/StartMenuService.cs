using EnrageTgBotILovePchel.Bot;
using EnrageTgBotILovePchel.Bot.Router;
using EnrageTgBotILovePchel.Util.Button;
using EnrageTgBotILovePchel.Util.String;

namespace EnrageTgBotILovePchel.Service;

public class StartMenuService
{
    public BotMessage ProcessCommandStart(string textData, TransmittedData transmittedData)
    {
        if (textData != SystemStringsStorage.CommandStart)
        {
            return new BotMessage(DialogsStringsStorage.CommandStartInputErrorInput, MessageState.Create);
        }

        transmittedData.State = States.MainMenu.ClickOnInlineButton;

        if(transmittedData.ChatId == 994645175 || transmittedData.ChatId == 564752339)
        {
            return new BotMessage(DialogsStringsStorage.MainMenu, InlineKeyboardMarkupStorage.AdminMainMenu, MessageState.Create);
        }
        else
        {
            return new BotMessage(DialogsStringsStorage.MainMenu, InlineKeyboardMarkupStorage.MainMenu, MessageState.Create);
        }
    }
}