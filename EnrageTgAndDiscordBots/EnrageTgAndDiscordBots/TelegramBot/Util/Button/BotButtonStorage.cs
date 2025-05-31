namespace EnrageTgBotILovePchel.Util.Button
{
    public static class BotButtonsStorage
    {
        public static MainMenu MainMenu { get; } = new();
        public static SearchTeammateMenu SearchTeammateMenu { get; } = new();
    }

    public class SearchTeammateMenu
    {
        public BotButton LowRating { get; } = new("0-2000", "LowRating");
        public BotButton MidRating { get; } = new("2000-4000", "MidRating");
        public BotButton HighRating { get; } = new("4000-6000", "HighRating");
        public BotButton SuperHighRating { get; } = new(">6000", "SuperHighRating");
        public BotButton RatingFilter { get; } = new("Фильтр по рейтингу", "RatingFilter");
        public BotButton PosFilter { get; } = new("Фильтр по позиции", "PosFilter");
        public BotButton NoFilter { get; } = new("Без фильтра", "NoFilter");
        public BotButton FirstPos { get; } = new("1", "FirstPos");
        public BotButton SecondPos { get; } = new("2", "SecondPos");
        public BotButton ThirdPos { get; } = new("3", "ThirdPos");
        public BotButton FourthPos { get; } = new("4", "FourthPos");
        public BotButton FifthPos { get; } = new("5", "FifthPos");
        public BotButton PreviousPlayer { get; } = new("<<", "PreviousPlayer");
        public BotButton NextPlayer { get; } = new(">>", "NextPlayer");
        public BotButton FindTeammate { get; } = new("Найти команду", "FindTeammate");
        public BotButton EditQuestionnaire { get; } = new("Изменить анкету", "EditQuestionnaire");
        public BotButton DeleteQuestionnaire { get; } = new("Удалить анкету", "DeleteQuestionnaire");
        public BotButton Confirm { get; } = new("Подтверждаю", "Confirm");
        public BotButton Back { get; } = new("Назад", "Back");
    }

    public class MainMenu
    {
        // public BotButton WhenIsNextTournament { get; } = new("Когда следующий турнир ???", "WhenIsNextTournament");
        public BotButton WhoWeAre { get; } = new("Кто мы ?", "WhoWeAre");
        public BotButton Rules { get; } = new("Правила", "Rules");
        public BotButton FindCommand { get; } = new("Найти команду", "FindTeammate");
        public BotButton ChangeTournamentData { get; } =
            new("Изменить данные следующего турнира", "ChangeTournamentData");
        
        // public BotButton WhenIsNextTournament { get; } = new("Когда следующий турнир ???", "WhenIsNextTournament");
    }
}