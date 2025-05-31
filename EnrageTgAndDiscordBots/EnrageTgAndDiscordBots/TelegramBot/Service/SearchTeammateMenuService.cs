using EnrageTgAndDiscordBots.Db.Repositories.Interfaces;
using EnrageTgAndDiscordBots.Models;
using EnrageTgBotILovePchel.Bot;
using EnrageTgAndDiscordBots.Db.Repositories.Interfaces;
using EnrageTgBotILovePchel.Bot.Router;
using EnrageTgAndDiscordBots.Models;
using EnrageTgBotILovePchel.Util.Button;
using EnrageTgBotILovePchel.Util.String;
using Telegram.Bot.Types.ReplyMarkups;

namespace EnrageTgBotILovePchel.Service
{
    public class SearchTeammateMenuService
    {
        IUsersDatasRepository _usersDatasRepository;
        private Dictionary<ulong, List<UsersDatum>> users;
        private BotRequestHandlers _botRequestHandlers;

        public SearchTeammateMenuService(IUsersDatasRepository usersDatasRepository)
        {
            _usersDatasRepository = usersDatasRepository;
        }

        public BotMessage ProcessInputName(string textData, TransmittedData transmittedData)
        {

            //прописать интерфейс, если у пользователя username == null
            if (transmittedData.TgUsername != null)
            {
                if (CheckIsButton(textData) == false)
                {
                    //_botRequestHandlers = new BotRequestHandlers();
                    //string username = _botRequestHandlers.GetTgUsername(transmittedData.ChatId);

                    if (textData.Length > ConstraintStringsStorage.MaxUserFirstName)
                    {
                        return new BotMessage(DialogsStringsStorage.NameInputError, MessageState.Create);
                    }

                    // if (transmittedData.State == States.SearchTeammateMenu.UpdateName)
                    // {
                    //     transmittedData.State = States.SearchTeammateMenu.UpdateNickname;
                    // }
                    // else
                    // {
                    //     transmittedData.State = States.SearchTeammateMenu.InputNickname;
                    // }

                    if (transmittedData.State == States.SearchTeammateMenu.UpdateName)
                    {
                        transmittedData.State = States.SearchTeammateMenu.UpdateRating;
                    }
                    else
                    {
                        transmittedData.State = States.SearchTeammateMenu.InputRating;
                    }

                    _usersDatasRepository.AddUser(textData, 0, 0, "@" + transmittedData.TgUsername, transmittedData.ChatId,
                        String.Empty);

                    // return new BotMessage(DialogsStringsStorage.NewQuestionnaireUserTgNickname,
                    //     InlineKeyboardMarkup.Empty(), MessageState.Create);
                    return new BotMessage(DialogsStringsStorage.NewQuestionnaireRatingInput,
                        InlineKeyboardMarkup.Empty(), MessageState.Create);
                }

                return new BotMessage(DialogsStringsStorage.NameInputError, MessageState.Create);
            }

            transmittedData.State = States.MainMenu.ClickOnInlineButton;

            return new BotMessage(DialogsStringsStorage.TgNickHide + "\n\n\n" + DialogsStringsStorage.MainMenu, InlineKeyboardMarkupStorage.MainMenu, MessageState.Create);
        }

        public BotMessage ProcessInputTgUsername(string textData, TransmittedData transmittedData)
        {
            if (CheckIsButton(textData) == false)
            {
                if (textData.Length > ConstraintStringsStorage.MaxUserTgUsername)
                {
                    return new BotMessage(DialogsStringsStorage.NicknameInputError, MessageState.Create);
                }

                UsersDatum usersData = _usersDatasRepository.GetLastUserDataByChatId(transmittedData.ChatId);

                if (textData.StartsWith('@') == false)
                {
                    usersData.PlayerTgNick = "@" + textData;
                }

                _usersDatasRepository.UpdateUser(usersData);

                if (transmittedData.State == States.SearchTeammateMenu.UpdateNickname)
                {
                    transmittedData.State = States.SearchTeammateMenu.UpdateRating;
                }
                else
                {
                    transmittedData.State = States.SearchTeammateMenu.InputRating;
                }

                return new BotMessage(DialogsStringsStorage.NewQuestionnaireRatingInput,
                    InlineKeyboardMarkup.Empty(), MessageState.Create);
            }

            return new BotMessage(DialogsStringsStorage.NicknameInputError, MessageState.Create);
        }

        public BotMessage ProcessInputRating(string textData, TransmittedData transmittedData)
        {
            int intRating = 0;
            bool isNumber = false;
            isNumber = int.TryParse(textData, out intRating);

            if (isNumber == false || intRating > ConstraintStringsStorage.MaxUserRating ||
                intRating < ConstraintStringsStorage.MinUserRating)
            {
                return new BotMessage(DialogsStringsStorage.RatingInputError, MessageState.Create);
            }

            if (transmittedData.State == States.SearchTeammateMenu.UpdateRating)
            {
                transmittedData.State = States.SearchTeammateMenu.UpdatePosition;
            }
            else
            {
                transmittedData.State = States.SearchTeammateMenu.InputPosition;
            }

            UsersDatum userData = _usersDatasRepository.GetLastUserDataByChatId(transmittedData.ChatId);
            userData.PlayerRating = intRating;

            return new BotMessage(DialogsStringsStorage.NewQuestionnairePosition,
                ReplyKeyboardMarkupStorage.CreateKeyboardSelectPosition(), MessageState.Create);
        }

        public BotMessage ProcessInputUserInformation(string textData, TransmittedData transmittedData)
        {
            string userInformation = textData;

            if (textData.Length > ConstraintStringsStorage.MaxUserInformation)
            {
                return new BotMessage(DialogsStringsStorage.UserInformationInputError, MessageState.Create);
            }

            UsersDatum usersData = _usersDatasRepository.GetLastUserDataByChatId(transmittedData.ChatId);
            usersData.PlayerDescription = textData;

            _usersDatasRepository.UpdateUser(usersData);

            if (transmittedData.State == States.SearchTeammateMenu.UpdateUserInformation)
            {
                transmittedData.State = States.SearchTeammateMenu.QuestionnaireUpdateCreateConfirmation;
            }
            else
            {
                transmittedData.State = States.SearchTeammateMenu.QuestionnaireInputCreateConfirmation;
            }

            usersData = _usersDatasRepository.GetLastUserDataByChatId(transmittedData.ChatId);

            return new BotMessage(DialogsStringsStorage.ConfirmCreateQuestionnaire(usersData),
                InlineKeyboardMarkupStorage.QuestionnaireCreateConfirmation, MessageState.Create);
        }

        public BotMessage ProcessClickOnInlineButtonPosition(string textData, TransmittedData transmittedData)
        {
            if (CheckIsButton(textData) == false)
            {
                if (textData == BotButtonsStorage.SearchTeammateMenu.FirstPos.Name ||
                    textData == BotButtonsStorage.SearchTeammateMenu.SecondPos.Name ||
                    textData == BotButtonsStorage.SearchTeammateMenu.ThirdPos.Name ||
                    textData == BotButtonsStorage.SearchTeammateMenu.FourthPos.Name ||
                    textData == BotButtonsStorage.SearchTeammateMenu.FifthPos.Name)
                {
                    UsersDatum usersData = _usersDatasRepository.GetLastUserDataByChatId(transmittedData.ChatId);
                    usersData.PlayerPosition = int.Parse(textData);

                    _usersDatasRepository.UpdateUser(usersData);

                    if (transmittedData.State == States.SearchTeammateMenu.UpdatePosition)
                    {
                        transmittedData.State = States.SearchTeammateMenu.UpdateUserInformation;
                    }
                    else
                    {
                        transmittedData.State = States.SearchTeammateMenu.InputUserInformation;
                    }

                    return new BotMessage(DialogsStringsStorage.NewUserInformation,
                        InlineKeyboardMarkupStorage.QuestionnaireCreateConfirmation, true, MessageState.Create);
                }
            }

            return new BotMessage(DialogsStringsStorage.PositionInputError, MessageState.Create);
        }

        public BotMessage ProcessClickOnButtonDeleteQuestionnaireConfirmation(string textData,
            TransmittedData transmittedData)
        {
            if (textData == ConstraintStringsStorage.Confirm)
            {
                _usersDatasRepository.DeleteUser(
                    _usersDatasRepository.GetLastUserDataByChatId(transmittedData.ChatId));
                transmittedData.State = States.MainMenu.ClickOnInlineButton;

                if (transmittedData.ChatId == 994645175 || transmittedData.ChatId == 564752339)
                {
                    return new BotMessage(
                        DialogsStringsStorage.QuestionnaireDeleted + "\n\n" + DialogsStringsStorage.MainMenu,
                        InlineKeyboardMarkupStorage.AdminMainMenu, MessageState.Create);
                }
                else
                {
                    return new BotMessage(
                        DialogsStringsStorage.QuestionnaireDeleted + "\n\n" + DialogsStringsStorage.MainMenu,
                        InlineKeyboardMarkupStorage.MainMenu, MessageState.Create);
                }
            }

            if (textData == ConstraintStringsStorage.Back)
            {
                transmittedData.State = States.SearchTeammateMenu.WatchingOnUserQuestionnaire;

                return new BotMessage(
                    DialogsStringsStorage.UserQuestionnaire(
                        _usersDatasRepository.GetLastUserDataByChatId(transmittedData.ChatId)),
                    InlineKeyboardMarkupStorage.ChooseFindingMenu, MessageState.Edit);
            }

            return new BotMessage("Неопознанная ошибка",
                InlineKeyboardMarkup.Empty(), MessageState.Create);
        }

        public BotMessage ProcessClickOnButtonActionQuestionnaire(string textData, TransmittedData transmittedData)
        {
            if (textData == ConstraintStringsStorage.EditQuestionnaire)
            {
                transmittedData.State = States.SearchTeammateMenu.UpdateName;
                return new BotMessage(DialogsStringsStorage.NewQuestionnaireNameInput,
                    InlineKeyboardMarkup.Empty(), MessageState.Edit);
            }

            if (textData == ConstraintStringsStorage.DeleteQuestionnaire)
            {
                transmittedData.State = States.SearchTeammateMenu.QuestionnaireInputDeleteConfirmation;
                return new BotMessage(DialogsStringsStorage.QuestionnaireDeletedConfirmation,
                    InlineKeyboardMarkupStorage.QuestionnaireDeleteConfirmation, MessageState.Edit);
            }

            if (textData == ConstraintStringsStorage.Back)
            {
                transmittedData.State = States.MainMenu.ClickOnInlineButton;

                if (transmittedData.ChatId == 994645175 || transmittedData.ChatId == 564752339)
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

            if (textData == ConstraintStringsStorage.FindTeammate)
            {
                transmittedData.State = States.SearchTeammateMenu.SelectSearchFilter;

                return new BotMessage(DialogsStringsStorage.WantSelectSearchFilter,
                    InlineKeyboardMarkupStorage.SelectSearchFilter, MessageState.Edit);
            }

            return new BotMessage("Неопознанная ошибка", MessageState.Create);
        }

        public BotMessage SelectSearchFilter(string textData, TransmittedData transmittedData)
        {
            if (textData == ConstraintStringsStorage.NoFilter)
            {
                List<UsersDatum> usersData = _usersDatasRepository.GetAllUserExcept(transmittedData.ChatId);
                transmittedData.DataStorage.AddOrUpdate("currentPageNumber", 0);
                var pageNumber = transmittedData.DataStorage.Get("currentPageNumber");
                transmittedData.State = States.SearchTeammateMenu.FindingTeammate;

                return new BotMessage(
                    "Поиск команды:\n" + DialogsStringsStorage.FindingTeammate(usersData[(int)pageNumber]),
                    InlineKeyboardMarkupStorage.FindTeammateControlMenu, MessageState.Edit);
            }

            if (textData == ConstraintStringsStorage.PosFilter)
            {
                transmittedData.State = States.SearchTeammateMenu.ProcessClickOnInlineButtonFilter;
                return new BotMessage(DialogsStringsStorage.SelectSearchFilter, InlineKeyboardMarkupStorage.PosFilter,
                    MessageState.Edit);
            }

            if (textData == ConstraintStringsStorage.RatingFilter)
            {
                transmittedData.State = States.SearchTeammateMenu.ProcessClickOnInlineButtonFilter;
                return new BotMessage(DialogsStringsStorage.SelectSearchFilter,
                    InlineKeyboardMarkupStorage.RatingFilter,
                    MessageState.Edit);
            }

            if (textData == BotButtonsStorage.SearchTeammateMenu.Back.CallBackData)
            {
                UsersDatum userData = _usersDatasRepository.GetLastUserDataByChatId(transmittedData.ChatId);
                transmittedData.State = States.SearchTeammateMenu.WatchingOnUserQuestionnaire;
                return new BotMessage(DialogsStringsStorage.UserQuestionnaire(userData),
                    InlineKeyboardMarkupStorage.ChooseFindingMenu, MessageState.Edit);
            }

            return new BotMessage("Неопознанная ошибка", MessageState.Create);
        }

        public BotMessage ProcessClickOnInlineButtonFilter(string textData, TransmittedData transmittedData)
        {
            UsersDatum userData = _usersDatasRepository.GetLastUserDataByChatId(transmittedData.ChatId);
            List<UsersDatum> findingUsersData = _usersDatasRepository.GetAllUserExcept(transmittedData.ChatId);
            transmittedData.DataStorage.AddOrUpdate("currentPageNumber", 0);
            var pageNumber = transmittedData.DataStorage.Get("currentPageNumber");
            transmittedData.State = States.SearchTeammateMenu.FindingTeammate;

            if (textData == BotButtonsStorage.SearchTeammateMenu.FirstPos.CallBackData)
            {
                List<UsersDatum> usersData =
                    _usersDatasRepository.GetAllUserExcept(transmittedData.ChatId, 1, 0, 12000);
                transmittedData.Filter = 1;

                if (usersData.Count == 0)
                {
                    return new BotMessage(
                        "На данный момент отсутствуют пользователи по заданным критерия, повторите попытку позже!",
                        InlineKeyboardMarkupStorage.MenuWithBackButton, MessageState.Edit);
                }

                return new BotMessage(
                    "Поиск команды:\n" + DialogsStringsStorage.FindingTeammate(usersData[(int)pageNumber]),
                    InlineKeyboardMarkupStorage.FindTeammateControlMenu, MessageState.Edit);
            }

            if (textData == BotButtonsStorage.SearchTeammateMenu.SecondPos.CallBackData)
            {
                List<UsersDatum> usersData =
                    _usersDatasRepository.GetAllUserExcept(transmittedData.ChatId, 2, 0, 12000);
                transmittedData.Filter = 2;

                if (usersData.Count == 0)
                {
                    return new BotMessage(
                        "На данный момент отсутствуют пользователи по заданным критерия, повторите попытку позже!",
                        InlineKeyboardMarkupStorage.MenuWithBackButton, MessageState.Edit);
                }

                return new BotMessage(
                    "Поиск команды:\n" + DialogsStringsStorage.FindingTeammate(usersData[(int)pageNumber]),
                    InlineKeyboardMarkupStorage.FindTeammateControlMenu, MessageState.Edit);
            }

            if (textData == BotButtonsStorage.SearchTeammateMenu.ThirdPos.CallBackData)
            {
                List<UsersDatum> usersData =
                    _usersDatasRepository.GetAllUserExcept(transmittedData.ChatId, 3, 0, 12000);
                transmittedData.Filter = 3;

                if (usersData.Count == 0)
                {
                    return new BotMessage(
                        "На данный момент отсутствуют пользователи по заданным критерия, повторите попытку позже!",
                        InlineKeyboardMarkupStorage.MenuWithBackButton, MessageState.Edit);
                }

                return new BotMessage(
                    "Поиск команды:\n" + DialogsStringsStorage.FindingTeammate(usersData[(int)pageNumber]),
                    InlineKeyboardMarkupStorage.FindTeammateControlMenu, MessageState.Edit);
            }

            if (textData == BotButtonsStorage.SearchTeammateMenu.FourthPos.CallBackData)
            {
                List<UsersDatum> usersData =
                    _usersDatasRepository.GetAllUserExcept(transmittedData.ChatId, 4, 0, 12000);
                transmittedData.Filter = 4;

                if (usersData.Count == 0)
                {
                    return new BotMessage(
                        "На данный момент отсутствуют пользователи по заданным критерия, повторите попытку позже!",
                        InlineKeyboardMarkupStorage.MenuWithBackButton, MessageState.Edit);
                }

                return new BotMessage(
                    "Поиск команды:\n" + DialogsStringsStorage.FindingTeammate(usersData[(int)pageNumber]),
                    InlineKeyboardMarkupStorage.FindTeammateControlMenu, MessageState.Edit);
            }

            if (textData == BotButtonsStorage.SearchTeammateMenu.FifthPos.CallBackData)
            {
                List<UsersDatum> usersData =
                    _usersDatasRepository.GetAllUserExcept(transmittedData.ChatId, 5, 0, 12000);
                transmittedData.Filter = 5;

                if (usersData.Count == 0)
                {
                    return new BotMessage(
                        "На данный момент отсутствуют пользователи по заданным критерия, повторите попытку позже!",
                        InlineKeyboardMarkupStorage.MenuWithBackButton, MessageState.Edit);
                }

                return new BotMessage(
                    "Поиск команды:\n" + DialogsStringsStorage.FindingTeammate(usersData[(int)pageNumber]),
                    InlineKeyboardMarkupStorage.FindTeammateControlMenu, MessageState.Edit);
            }

            if (textData == BotButtonsStorage.SearchTeammateMenu.LowRating.CallBackData)
            {
                List<UsersDatum> usersData = _usersDatasRepository.GetAllUserExcept(transmittedData.ChatId, 0, 0, 2000);
                transmittedData.Filter = 0;

                if (usersData.Count == 0)
                {
                    return new BotMessage(
                        "На данный момент отсутствуют пользователи по заданным критерия, повторите попытку позже!",
                        InlineKeyboardMarkupStorage.MenuWithBackButton, MessageState.Edit);
                }

                return new BotMessage(
                    "Поиск команды:\n" + DialogsStringsStorage.FindingTeammate(usersData[(int)pageNumber]),
                    InlineKeyboardMarkupStorage.FindTeammateControlMenu, MessageState.Edit);
            }

            if (textData == BotButtonsStorage.SearchTeammateMenu.MidRating.CallBackData)
            {
                List<UsersDatum> usersData =
                    _usersDatasRepository.GetAllUserExcept(transmittedData.ChatId, 0, 2000, 4000);
                transmittedData.Filter = 2000;

                if (usersData.Count == 0)
                {
                    return new BotMessage(
                        "На данный момент отсутствуют пользователи по заданным критерия, повторите попытку позже!",
                        InlineKeyboardMarkupStorage.MenuWithBackButton, MessageState.Edit);
                }

                return new BotMessage(
                    "Поиск команды:\n" + DialogsStringsStorage.FindingTeammate(usersData[(int)pageNumber]),
                    InlineKeyboardMarkupStorage.FindTeammateControlMenu, MessageState.Edit);
            }

            if (textData == BotButtonsStorage.SearchTeammateMenu.HighRating.CallBackData)
            {
                List<UsersDatum> usersData =
                    _usersDatasRepository.GetAllUserExcept(transmittedData.ChatId, 0, 4000, 6000);
                transmittedData.Filter = 4000;

                if (usersData.Count == 0)
                {
                    return new BotMessage(
                        "На данный момент отсутствуют пользователи по заданным критерия, повторите попытку позже!",
                        InlineKeyboardMarkupStorage.MenuWithBackButton, MessageState.Edit);
                }

                return new BotMessage(
                    "Поиск команды:\n" + DialogsStringsStorage.FindingTeammate(usersData[(int)pageNumber]),
                    InlineKeyboardMarkupStorage.FindTeammateControlMenu, MessageState.Edit);
            }

            if (textData == BotButtonsStorage.SearchTeammateMenu.SuperHighRating.CallBackData)
            {
                List<UsersDatum> usersData = _usersDatasRepository.GetAllUserExcept(transmittedData.ChatId, 0, 6000);
                transmittedData.Filter = 6000;

                if (usersData.Count == 0)
                {
                    return new BotMessage(
                        "На данный момент отсутствуют пользователи по заданным критерия, повторите попытку позже!",
                        InlineKeyboardMarkupStorage.MenuWithBackButton, MessageState.Edit);
                }

                return new BotMessage(
                    "Поиск команды:\n" + DialogsStringsStorage.FindingTeammate(usersData[(int)pageNumber]),
                    InlineKeyboardMarkupStorage.FindTeammateControlMenu, MessageState.Edit);
            }

            if (textData == BotButtonsStorage.SearchTeammateMenu.Back.CallBackData)
            {
                transmittedData.State = States.SearchTeammateMenu.WatchingOnUserQuestionnaire;
                return new BotMessage(DialogsStringsStorage.UserQuestionnaire(userData),
                    InlineKeyboardMarkupStorage.ChooseFindingMenu, MessageState.Edit);
            }

            return new BotMessage("Неопознанная ошибка", MessageState.Create);
        }

        public BotMessage ConfirmingCreateQuestionnaire(string textData, TransmittedData transmittedData)
        {
            if (textData == ConstraintStringsStorage.Confirm)
            {
                if (transmittedData.State == States.SearchTeammateMenu.QuestionnaireUpdateCreateConfirmation)
                {
                    transmittedData.State = States.SearchTeammateMenu.WatchingOnUserQuestionnaire;
                    UsersDatum user = _usersDatasRepository.GetFirstUserDataByChatId(transmittedData.ChatId);
                    _usersDatasRepository.DeleteUser(user);

                    return new BotMessage(DialogsStringsStorage.QuestionnaireUpdateSuccess + "\n" +
                                          DialogsStringsStorage.UserQuestionnaire(
                                              _usersDatasRepository
                                                  .GetLastUserDataByChatId(transmittedData.ChatId)),
                        InlineKeyboardMarkupStorage.ChooseFindingMenu, MessageState.Edit);
                }
                else
                {
                    transmittedData.State = States.SearchTeammateMenu.WatchingOnUserQuestionnaire;

                    return new BotMessage(DialogsStringsStorage.QuestionnaireCreateSuccess + "\n" +
                                          DialogsStringsStorage.UserQuestionnaire(
                                              _usersDatasRepository
                                                  .GetLastUserDataByChatId(transmittedData.ChatId)),
                        InlineKeyboardMarkupStorage.ChooseFindingMenu, MessageState.Edit);
                }
            }

            if (textData == ConstraintStringsStorage.Back)
            {
                transmittedData.State = States.MainMenu.ClickOnInlineButton;
                UsersDatum user = new UsersDatum();

                if (transmittedData.State == States.SearchTeammateMenu.QuestionnaireUpdateCreateConfirmation)
                    user = _usersDatasRepository.GetFirstUserDataByChatId(transmittedData.ChatId);
                else
                    user = _usersDatasRepository.GetLastUserDataByChatId(transmittedData.ChatId);

                _usersDatasRepository.DeleteUser(user);

                if (transmittedData.ChatId == 994645175 || transmittedData.ChatId == 564752339)
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

            return new BotMessage("Неопознанная ошибка",
                InlineKeyboardMarkup.Empty(), MessageState.Create);
        }

        public BotMessage SearchTeammateControlMenuAction(string textData, TransmittedData transmittedData)
        {
            var pageNumber = transmittedData.DataStorage.Get("currentPageNumber");
            List<UsersDatum> usersData;

            if (textData == BotButtonsStorage.SearchTeammateMenu.NextPlayer.CallBackData)
            {
                usersData = CheckFilter(transmittedData);
                if ((int)pageNumber == usersData.Count() - 1)
                {
                    return new BotMessage(
                        "Это последняя страница, заходите позже чтобы увидеть новые анкеты.\n Последняя анкета была:\n" +
                        DialogsStringsStorage.FindingTeammate(usersData[(int)pageNumber]),
                        InlineKeyboardMarkupStorage.FindTeammateControlMenu, MessageState.Edit);
                }
                else
                {
                    pageNumber = (int)pageNumber + 1;
                    transmittedData.DataStorage.AddOrUpdate("currentPageNumber", (int)pageNumber);
                    return new BotMessage(DialogsStringsStorage.FindingTeammate(usersData[(int)pageNumber]),
                        InlineKeyboardMarkupStorage.FindTeammateControlMenu, MessageState.Edit);
                }
            }

            if (textData == BotButtonsStorage.SearchTeammateMenu.PreviousPlayer.CallBackData)
            {
                usersData = CheckFilter(transmittedData);
                if ((int)pageNumber == 0)
                {
                    return new BotMessage(
                        "Это первая страница !\n" +
                        DialogsStringsStorage.FindingTeammate(usersData[(int)pageNumber]),
                        InlineKeyboardMarkupStorage.FindTeammateControlMenu, MessageState.Edit);
                }
                else
                {
                    pageNumber = (int)pageNumber - 1;
                    transmittedData.DataStorage.AddOrUpdate("currentPageNumber", (int)pageNumber);
                    return new BotMessage(DialogsStringsStorage.FindingTeammate(usersData[(int)pageNumber]),
                        InlineKeyboardMarkupStorage.FindTeammateControlMenu, MessageState.Edit);
                }
            }

            if (textData == BotButtonsStorage.SearchTeammateMenu.Back.CallBackData)
            {
                UsersDatum userData = _usersDatasRepository.GetLastUserDataByChatId(transmittedData.ChatId);
                transmittedData.State = States.SearchTeammateMenu.WatchingOnUserQuestionnaire;
                transmittedData.Filter = -1;
                return new BotMessage(DialogsStringsStorage.UserQuestionnaire(userData),
                    InlineKeyboardMarkupStorage.ChooseFindingMenu, MessageState.Edit);
            }

            return new BotMessage("Неопознанная ошибка", MessageState.Create);
        }

        private List<UsersDatum> CheckFilter(TransmittedData transmittedData)
        {
            if (transmittedData.Filter == -1)
            {
                return _usersDatasRepository.GetAllUserExcept(transmittedData.ChatId);
            }

            if (transmittedData.Filter == 1)
            {
                return _usersDatasRepository.GetAllUserExcept(transmittedData.ChatId, 1, 0, 12000);
            }

            if (transmittedData.Filter == 2)
            {
                return _usersDatasRepository.GetAllUserExcept(transmittedData.ChatId, 2, 0, 12000);
            }

            if (transmittedData.Filter == 3)
            {
                return _usersDatasRepository.GetAllUserExcept(transmittedData.ChatId, 3, 0, 12000);
            }

            if (transmittedData.Filter == 4)
            {
                return _usersDatasRepository.GetAllUserExcept(transmittedData.ChatId, 4, 0, 12000);
            }

            if (transmittedData.Filter == 5)
            {
                return _usersDatasRepository.GetAllUserExcept(transmittedData.ChatId, 5, 0, 12000);
            }

            if (transmittedData.Filter == 0)
            {
                return _usersDatasRepository.GetAllUserExcept(transmittedData.ChatId, 0, 0, 2000);
            }

            if (transmittedData.Filter == 2000)
            {
                return _usersDatasRepository.GetAllUserExcept(transmittedData.ChatId, 0, 2000, 4000);
            }

            if (transmittedData.Filter == 4000)
            {
                return _usersDatasRepository.GetAllUserExcept(transmittedData.ChatId, 0, 4000, 6000);
            }

            if (transmittedData.Filter == 6000)
            {
                return _usersDatasRepository.GetAllUserExcept(transmittedData.ChatId, 0, 6000);
            }

            return new List<UsersDatum>();
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
}