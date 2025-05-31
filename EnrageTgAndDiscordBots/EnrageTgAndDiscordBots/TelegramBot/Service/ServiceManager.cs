using EnrageTgAndDiscordBots.Db.Repositories.Implemintations;
using EnrageTgAndDiscordBots.Db.Repositories.Interfaces;
using EnrageTgAndDiscordBots.DbConnector;
using EnrageTgAndDiscordBots.Models;
using EnrageTgBotILovePchel.Bot;
using EnrageTgBotILovePchel.Bot.Router;
using EnrageTgBotILovePchel.Util.Button;
using EnrageTgBotILovePchel.Util.String;

namespace EnrageTgBotILovePchel.Service;

public class ServiceManager
{
    private Dictionary<string, Func<string, TransmittedData, BotMessage>>
        _methods;

    private EnrageBotVovodyaDbContext db = new EnrageBotVovodyaDbContext();

    private IUsersDatasRepository usersDataRepository;

    public ServiceManager()
    {
        usersDataRepository = new UsersDatasRepository(db);
        StartMenuService startMenuService = new StartMenuService();
        MainMenuService mainMenuService = new MainMenuService(usersDataRepository);
        SearchTeammateMenuService searchTeammateMenuService =
            new SearchTeammateMenuService(usersDataRepository);
        TournamentMenuService tournamentMenuService = new TournamentMenuService();

        _methods =
            new Dictionary<string, Func<string, TransmittedData, BotMessage>>();

        #region StartMenu

        _methods[States.StartMenu.CommandStart] = startMenuService.ProcessCommandStart;

        #endregion

        #region MainMenu

        _methods[States.MainMenu.ClickOnInlineButton] = mainMenuService.ProcessClickOnInlineButton;

        #endregion

        #region TournamentMenu

        // _methods[States.TournamentMenu.ProcessInputTournamentRules] =
        //     tournamentMenuService.ProcessInputTournamentRules;

        #endregion

        #region SearchTeammateMenu

        _methods[States.SearchTeammateMenu.InputName] = searchTeammateMenuService.ProcessInputName;
        _methods[States.SearchTeammateMenu.UpdateName] = searchTeammateMenuService.ProcessInputName;
        _methods[States.SearchTeammateMenu.InputRating] = searchTeammateMenuService.ProcessInputRating;
        _methods[States.SearchTeammateMenu.UpdateRating] = searchTeammateMenuService.ProcessInputRating;
        _methods[States.SearchTeammateMenu.InputNickname] = searchTeammateMenuService.ProcessInputTgUsername;
        _methods[States.SearchTeammateMenu.UpdateNickname] = searchTeammateMenuService.ProcessInputTgUsername;
        _methods[States.SearchTeammateMenu.InputUserInformation] =
            searchTeammateMenuService.ProcessInputUserInformation;
        _methods[States.SearchTeammateMenu.UpdateUserInformation] =
            searchTeammateMenuService.ProcessInputUserInformation;
        _methods[States.SearchTeammateMenu.QuestionnaireInputCreateConfirmation] =
            searchTeammateMenuService.ConfirmingCreateQuestionnaire;
        _methods[States.SearchTeammateMenu.QuestionnaireUpdateCreateConfirmation] =
            searchTeammateMenuService.ConfirmingCreateQuestionnaire;
        _methods[States.SearchTeammateMenu.UpdatePosition] =
            searchTeammateMenuService.ProcessClickOnInlineButtonPosition;
        _methods[States.SearchTeammateMenu.InputPosition] =
            searchTeammateMenuService.ProcessClickOnInlineButtonPosition;
        _methods[States.SearchTeammateMenu.WatchingOnUserQuestionnaire] =
            searchTeammateMenuService.ProcessClickOnButtonActionQuestionnaire;
        _methods[States.SearchTeammateMenu.ClickInlineButtonInActionWithGameMenu] =
            searchTeammateMenuService.ProcessClickOnButtonActionQuestionnaire;
        _methods[States.SearchTeammateMenu.QuestionnaireInputDeleteConfirmation] =
            searchTeammateMenuService.ProcessClickOnButtonDeleteQuestionnaireConfirmation;
        _methods[States.SearchTeammateMenu.FindingTeammate] = searchTeammateMenuService.SearchTeammateControlMenuAction;
        _methods[States.SearchTeammateMenu.SelectSearchFilter] = searchTeammateMenuService.SelectSearchFilter;
        _methods[States.SearchTeammateMenu.ProcessClickOnInlineButtonFilter] =
            searchTeammateMenuService.ProcessClickOnInlineButtonFilter;

        #endregion
    }

    public BotMessage ProcessBotUpdate(string textData, TransmittedData transmittedData)
    {
        if (textData == SystemStringsStorage.CommandReset)
        {
            if (transmittedData.State == States.SearchTeammateMenu.UpdateRating ||
                transmittedData.State == States.SearchTeammateMenu.UpdateNickname ||
                transmittedData.State == States.SearchTeammateMenu.UpdatePosition ||
                transmittedData.State == States.SearchTeammateMenu.QuestionnaireUpdateCreateConfirmation ||
                transmittedData.State == States.SearchTeammateMenu.InputRating ||
                transmittedData.State == States.SearchTeammateMenu.InputNickname ||
                transmittedData.State == States.SearchTeammateMenu.InputPosition ||
                transmittedData.State == States.SearchTeammateMenu.QuestionnaireInputCreateConfirmation)
            {
                UsersDatum user = usersDataRepository.GetLastUserDataByChatId(transmittedData.ChatId);
                usersDataRepository.DeleteUser(user);
            }

            transmittedData.State = States.MainMenu.ClickOnInlineButton;
            transmittedData.DataStorage.Clear();

            if (transmittedData.ChatId == 994645175 || transmittedData.ChatId == 564752339)
                return new BotMessage(DialogsStringsStorage.MainMenu, InlineKeyboardMarkupStorage.AdminMainMenu,
                    MessageState.Create);
            else
                return new BotMessage(DialogsStringsStorage.MainMenu, InlineKeyboardMarkupStorage.MainMenu,
                    MessageState.Create);
        }

        Func<string, TransmittedData, BotMessage> serviceMethod = _methods[transmittedData.State];
        return serviceMethod.Invoke(textData, transmittedData);
    }
}