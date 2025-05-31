using Telegram.Bot.Types.ReplyMarkups;

namespace EnrageTgBotILovePchel.Util.Button
{
    public class ReplyKeyboardMarkupStorage
    {
        public static ReplyKeyboardMarkup CreateKeyboardSelectPosition()
        {
            var rows = new[]
            {
                new KeyboardButton(BotButtonsStorage.SearchTeammateMenu.FirstPos.Name),
                new KeyboardButton(BotButtonsStorage.SearchTeammateMenu.SecondPos.Name),
                new KeyboardButton(BotButtonsStorage.SearchTeammateMenu.ThirdPos.Name),
                new KeyboardButton(BotButtonsStorage.SearchTeammateMenu.FourthPos.Name),
                new KeyboardButton(BotButtonsStorage.SearchTeammateMenu.FifthPos.Name)
            };

            return new ReplyKeyboardMarkup(rows);
        }
    }
}