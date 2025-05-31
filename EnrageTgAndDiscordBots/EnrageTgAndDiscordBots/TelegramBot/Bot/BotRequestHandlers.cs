using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EnrageTgBotILovePchel.Bot;
using EnrageTgBotILovePchel.Bot.Router;
using EnrageTgAndDiscordBots.DbConnector;
using EnrageTgBotILovePchel.Util.Button;
using EnrageTgBotILovePchel.Util.String;
using NLog;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using File = Telegram.Bot.Types.File;

namespace EnrageTgBotILovePchel.Bot;

public class BotRequestHandlers
{
    private static ILogger Logger = LogManager.GetCurrentClassLogger();
    private ChatsRouter _chatsRouter;

    private Dictionary<long, int> _lastBotMessageForUser = new Dictionary<long, int>();

    public BotRequestHandlers()
    {
        _chatsRouter = new ChatsRouter();
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        long chatId = 0;
        int messageFromUserId = 0;
        string textData = "";
        string userName = "";
        bool canRoute = false;
        Message message;

        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    if (update.Message != null)
                    {
                        canRoute = true;
                        chatId = update.Message.Chat.Id;
                        messageFromUserId = update.Message.MessageId;
                        userName = update.Message.Chat.Username;
                        textData = update.Message.Text;
                        
                        //_userTgNickname[chatId] = update.Message.From.Username;

                        Console.WriteLine(
                            $"ВХОДЯЩЕЕ СОООБЩЕНИЕ UpdateType = {update.Type}; chatId = {chatId}; messageId = {messageFromUserId}; text = {textData} canRoute = {canRoute}");
                        Logger.Info(
                            $"ВХОДЯЩЕЕ СОООБЩЕНИЕ UpdateType = {update.Type}; chatId = {chatId}; messageId = {messageFromUserId}; text = {textData} canRoute = {canRoute}");
                    }

                    break;

                case UpdateType.CallbackQuery:
                    if (update.CallbackQuery != null)
                    {
                        canRoute = true;
                        chatId = update.CallbackQuery.Message.Chat.Id;
                        userName = update.CallbackQuery.Message.Chat.Username;
                        messageFromUserId = update.CallbackQuery.Message.MessageId;
                        textData = update.CallbackQuery.Data;


                        Console.WriteLine(
                                $"ВХОДЯЩЕЕ СОООБЩЕНИЕ UpdateType = {update.Type}; chatId = {chatId}; messageId = {messageFromUserId}; text = {textData} canRoute = {canRoute}");
                        Logger.Info(
                            $"ВХОДЯЩЕЕ СОООБЩЕНИЕ UpdateType = {update.Type}; chatId = {chatId}; messageId = {messageFromUserId}; text = {textData} canRoute = {canRoute}");
                    }

                    break;
            }

            if (canRoute)
            {
                BotMessage botMessage = await Task.Run(() => _chatsRouter.Route(chatId, textData, userName), cancellationToken);
                IReplyMarkup? keyboard;

                if (botMessage.HideReplyKeyboard)
                {
                    keyboard = new ReplyKeyboardRemove();
                }
                else
                {
                    keyboard = botMessage.KeyboardMarkup;
                }

                if (botMessage.WhatIsMessageState == MessageState.Create)
                {
                    message = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: botMessage.Text,
                        replyMarkup: keyboard,
                        cancellationToken: cancellationToken);

                    if (botMessage.Text != "Неопознанная ошибка")
                        _lastBotMessageForUser[chatId] = message.MessageId;
                }
                else
                {
                    int messageId = _lastBotMessageForUser[chatId];

                    if (botMessage.Text != DialogsStringsStorage.TournamentsRules)
                    {
                        try
                        {
                            message = await botClient.EditMessageTextAsync(
                                chatId: chatId,
                                messageId: messageId,
                                text: botMessage.Text,
                                replyMarkup: (InlineKeyboardMarkup)keyboard,
                                cancellationToken: cancellationToken);

                            _lastBotMessageForUser[chatId] = message.MessageId;

                            Console.WriteLine(
                                $"ИСХОДЯЩЕЕ СОООБЩЕНИЕ chatId = {chatId}; text = {botMessage.Text.Replace("\n", " ")}; Keyboard = {getKeyboardAsString(botMessage.KeyboardMarkup)}");
                            Logger.Info(
                                $"ИСХОДЯЩЕЕ СОООБЩЕНИЕ chatId = {chatId}; text = {botMessage.Text.Replace("\n", " ")}; Keyboard = {getKeyboardAsString(botMessage.KeyboardMarkup)}");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Сообщение не было изменено");
                            return;
                        }
                    }
                    else
                    {
                        string serverPath = @"/root/EnrageTgAndDiscordBots/EnrageTgAndDiscordBots/Reglament_Enrage.pdf";
                        string localhostPath = @"D:\С# программирование\Enrgae_tg_bot_i_love_pchell\Enrgae_tg_bot_i_love_pchel\Reglament_Enrage.pdf";
                        var filePath = serverPath;
                        await using Stream stream = System.IO.File.OpenRead(filePath);

                        message = await botClient.SendDocumentAsync(
                            chatId: chatId,
                            document: new InputFileStream(stream, "Reglament_Enrage.pdf"),
                            caption: botMessage.Text,
                            cancellationToken: cancellationToken);

                        Console.WriteLine(
                            $"ИСХОДЯЩЕЕ СОООБЩЕНИЕ chatId = {chatId}; text = {botMessage.Text.Replace("\n", " ")}; Keyboard = {getKeyboardAsString(botMessage.KeyboardMarkup)}");
                        Logger.Info(
                            $"ИСХОДЯЩЕЕ СОООБЩЕНИЕ chatId = {chatId}; text = {botMessage.Text.Replace("\n", " ")}; Keyboard = {getKeyboardAsString(botMessage.KeyboardMarkup)}");
                    }
                }
            }
        }
        catch (Exception e)
        {
            // try
            // {
            await botClient.DeleteMessageAsync(
                chatId: chatId,
                messageId: messageFromUserId,
                cancellationToken: cancellationToken
            );

            Console.WriteLine($"ОШИБКА chatId = {chatId}; text = {e.Message}");
            Logger.Error($"ОШИБКА chatId = {chatId}; text = {e.Message}");
            // }
            // catch (Exception exception)
            // {
            //     Console.WriteLine("Сообщение для удаления не найдено");
            //     Logger.Error("Сообщение для удаления не найдено");
            // }
        }
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine($"Ошибка поймана в методе HandlePollingErrorAsync, {errorMessage}");
        Logger.Error($"Ошибка поймана в методе HandlePollingErrorAsync, {errorMessage}");
        return Task.CompletedTask;
    }

    private string getKeyboardAsString(IReplyMarkup keyboardMarkup)
    {
        if (keyboardMarkup == null)
        {
            return "Клавиатуры в данном сообщении нет";
        }

        StringBuilder stringBuilder = new StringBuilder();

        if (keyboardMarkup is InlineKeyboardMarkup)
        {
            InlineKeyboardMarkup inlineKeyboardMarkup = keyboardMarkup as InlineKeyboardMarkup;

            foreach (var row in inlineKeyboardMarkup.InlineKeyboard)
            {
                stringBuilder.Append(row.ToList()[0].Text + ";");
            }
        }
        else if (keyboardMarkup is ReplyKeyboardMarkup)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = keyboardMarkup as ReplyKeyboardMarkup;

            foreach (var row in replyKeyboardMarkup.Keyboard)
            {
                stringBuilder.Append(row.ToList()[0].Text + ";");
            }
        }

        return stringBuilder.ToString();
    }
    
    private bool CheckIsButton(string textData)
    {
        if (textData == BotButtonsStorage.MainMenu.FindCommand.CallBackData ||
            textData == BotButtonsStorage.MainMenu.Rules.CallBackData ||
            // textData == BotButtonsStorage.MainMenu.WhenIsNextTournament.CallBackData ||
            textData == BotButtonsStorage.SearchTeammateMenu.NextPlayer.CallBackData ||
            textData == BotButtonsStorage.SearchTeammateMenu.PreviousPlayer.CallBackData ||
            textData == BotButtonsStorage.SearchTeammateMenu.DeleteQuestionnaire.CallBackData ||
            textData == BotButtonsStorage.SearchTeammateMenu.EditQuestionnaire.CallBackData ||
            textData == BotButtonsStorage.SearchTeammateMenu.FindTeammate.CallBackData)
        {
            return true;
        }

        return false;
    }
}