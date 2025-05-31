using EnrageTgAndDiscordBots.Db.Repositories.Interfaces;
using EnrageTgAndDiscordBots.Models;
using EnrageTgBotILovePchel.Bot;
using EnrageTgBotILovePchel.Bot.Router;
using EnrageTgBotILovePchel.Util.String;
using EnrageTgBotILovePchel.Util.Button;
using Telegram.Bot.Types.ReplyMarkups;

namespace EnrageTgBotILovePchel.Service
{
    public class MainMenuService
    { 
        private IUsersDatasRepository _usersDatas;

        public MainMenuService(IUsersDatasRepository usersDatas)
        {
            _usersDatas = usersDatas;
        }

        public BotMessage ProcessClickOnInlineButton(string textData, TransmittedData transmittedData)
        {
            //TournamentsDatum tournamentsData = _tournamentDatasRepository.GetTournamentsData();
            UsersDatum userData = _usersDatas.GetLastUserDataByChatId(transmittedData.ChatId);

            if (textData == ConstraintStringsStorage.ChangeTournamentData)
            {
                transmittedData.State = States.TournamentMenu.ProcessInputTournamentRules;

                return new BotMessage(DialogsStringsStorage.ChangeTournamentData, InlineKeyboardMarkup.Empty(), MessageState.Create);
            }

            if (textData == ConstraintStringsStorage.WhoWeAre)
            {
                transmittedData.State = States.MainMenu.ClickOnInlineButton;
                return new BotMessage(DialogsStringsStorage.WhoWeAre, InlineKeyboardMarkupStorage.MenuWithBackButton, MessageState.Edit);
            }

            if (textData == ConstraintStringsStorage.FindTeammate)
            {
                if (userData != null)
                {
                    transmittedData.State = States.SearchTeammateMenu.WatchingOnUserQuestionnaire;
                    return new BotMessage(DialogsStringsStorage.UserQuestionnaire(userData), InlineKeyboardMarkupStorage.ChooseFindingMenu, MessageState.Edit);
                }
                else
                {
                    transmittedData.State = States.SearchTeammateMenu.InputName;
                    return new BotMessage(DialogsStringsStorage.NewQuestionnaireNameInput, InlineKeyboardMarkup.Empty(), true, MessageState.Create);
                }
            }

            if (textData == ConstraintStringsStorage.Rules)
            {
                transmittedData.State = States.MainMenu.ClickOnInlineButton;
                return new BotMessage(DialogsStringsStorage.TournamentsRules, InlineKeyboardMarkupStorage.MenuWithBackButton, MessageState.Edit);
            }

            if(textData == ConstraintStringsStorage.Back)
            {
                transmittedData.State = States.MainMenu.ClickOnInlineButton;
                if(transmittedData.ChatId == 994645175 || transmittedData.ChatId == 564752339)
                {
                    return new BotMessage(DialogsStringsStorage.MainMenu,
                        InlineKeyboardMarkupStorage.AdminMainMenu, MessageState.Edit);
                }
                else
                {
                    return new BotMessage(DialogsStringsStorage.MainMenu,
                        InlineKeyboardMarkupStorage.MainMenu, MessageState.Edit);
                }
            }

            throw new Exception("Неизвестная ошибка в ProcessClickOnInlineButton");
        }
    }
}