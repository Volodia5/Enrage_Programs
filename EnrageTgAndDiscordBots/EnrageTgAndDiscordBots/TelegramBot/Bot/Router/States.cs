namespace EnrageTgBotILovePchel.Bot.Router;

public class States
{
    public static StartMenu StartMenu { get; } = new();
    public static MainMenu MainMenu { get; } = new();
    public static SearchTeammateMenu SearchTeammateMenu { get; } = new();

    public static TournamentMenu TournamentMenu { get; } = new();
}

public class StartMenu
{
    public string CommandStart { get; } = "CommandStart";
}

public class MainMenu
{
    public string ClickOnInlineButton { get; } = "ClickOnInlineButton";
}

public class TournamentMenu
{
    public string ProcessInputTournamentRules { get; } = "ProcessInputTournamentRules";
    public string ProcessInputTournamentData { get; } = "ProcessEditingTournamentData";
}

public class SearchTeammateMenu
{
    public string ProcessClickOnInlineButtonFilter { get; } = "ProcessClickOnInlineButtonFilter";
    public string SelectSearchFilter { get; } = "SelectSearchFilter";
    public string QuestionnaireInputCreateConfirmation { get; } = "QuestionnaireInputCreateConfirmation";
    public string InputNickname { get; } = "InputNickname";
    public string ClickInlineButtonInActionWithGameMenu { get; } = "ClickInlineButtonInActionWithSearchMenu";
    public string InputName { get; } = "InputName";
    public string InputRating { get; } = "InputRating";
    public string InputPosition { get; } = "InputPosition";
    public string InputUserInformation { get; } = "InputUserInformation";
    public string QuestionnaireUpdateCreateConfirmation { get; } = "QuestionnaireUpdateCreateConfirmation";
    public string UpdateNickname { get; } = "UpdateNickname";
    public string UpdateName { get; } = "UpdateName";
    public string UpdateRating { get; } = "UpdateRating";
    public string UpdatePosition { get; } = "UpdatePosition";
    public string UpdateUserInformation { get; } = "UpdateUserInformation";
    public string WatchingOnUserQuestionnaire { get; } = "WatchingOnUserQuestionnaire";
    public string FindingTeammate { get; } = "FindingTeammate";
    public string QuestionnaireInputDeleteConfirmation { get; } = "QuestionnaireInputDeleteConfirmation";
}