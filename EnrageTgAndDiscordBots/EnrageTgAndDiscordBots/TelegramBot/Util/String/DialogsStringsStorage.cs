using EnrageTgAndDiscordBots.Models;

namespace EnrageTgBotILovePchel.Util.String;

public class DialogsStringsStorage
{
    public const string CommandStartInputErrorInput = "Команда не распознана. Для начала работы с ботом введите /start";

    public const string MainMenu = "Выберите действие:";

    public const string NameInputError = "Длина вашего имени не может превышать 25 символов\n\n";

    public const string RatingInputError = "Укажите рейтинг в пределах от 0 до 12000 (ммр)\n\n";

    public const string PositionInputError = "Ваша позиция должна быть от 1 до 5";

    public const string NicknameInputError = "Ваш никнейм не может привышать 30 символов\n\n";

    public const string UserInformationInputError = "Ваше описание не может привышать 4000 символов\n\n";

    public const string QuestionnaireCreateSuccess = "Анкета создана успешно!";

    public const string QuestionnaireUpdateSuccess = "Анкета успешно обновлена!";

    public const string TgNickHide = "У вас скрыт либо не установлен username. Чтобы его установить зайдите в настройки Telegram->Мой аккаунт->имя пользователя. Установите имя пользователя и повторите попытку.";

    public static string WhoWeAre =
        "Enrage - Турниры для маленьких рангов Dota 2\n" +
        "Мы специализируемся на проведении турниров по Dota 2 для маленьких рангов, в чем нам помогает судейский контроль матчей, " +
        "тщательная организация турнира и открытое общение с игроками.\n" +
        "\nПроводи время с пользой, играя вместе с Enrage!\n" +
        "\nНаши соцсети:\n" +
        "BK - https://vk.com/enrage2x\n" +
        "Основная группа telegram - https://t.me/enragetournaments\n\n" +
        "Если вы хотите поддержать нас финансово, а также зарегистрироваться в турнире с платным вступлением,\nмы предоставляем реквизиты:\n\n" +
        "Сбер ‐ 2202 2018 7379 0477\n\nТ-банк - 2200 7010 5116 7275\n\n" +
        "Либо через Boosty: https://boosty.to/enrage (оплатить необходимо с наценкой 20% от суммы взноса, так как взимается комиссия).\n\n" +
        "Возможна оплата по USDT, уточняйте у организатора (ниже)\n\nЛибо через ВК Donut: https://vk.com/donut/enrage2x\n\n" +
        "- Если у вас возникнут вопросы или вы заинтересованы в других способах поддержки, свяжитесь с нами по\nследующим контактам:\n\n\ud83d\udce7" +
        " vk: https://vk.com/id132681884, https://vk.com/sewpho\n\ud83d\udcac Discord: discord.gg/enrage\n\ud83d\udcac" +
        " Telegram: https://t.me/enragetournaments, в лс: @sewpho\n\ud83d\udcac По вопросам по боту: https://t.me/vladimirrogozn";

    public static string WantSelectSearchFilter = "Желаете ли отфильтровать поиск ?";

    public static string SelectSearchFilter = "Выберите нужную позицию/рейтинг: ";
    
    public static string UserQuestionnaire(UsersDatum userData)
    {
        return "Ваша анкета:\n\n" +
               $"Имя - {userData.PlayerName}\n" +
               $"Рейтинг- {userData.PlayerRating}\n" +
               $"Основная позиция- {userData.PlayerPosition}\n" +
               $"Ваш ник tg- {userData.PlayerTgNick}\n" +
               $"Ваше описание - {userData.PlayerDescription}\n";
    }

    public static string FindingTeammate(UsersDatum userData)
    {
        return $"Aнкета пользователя: {userData.PlayerTgNick}\n\n" +
               $"Имя - {userData.PlayerName}\n" +
               $"Рейтинг- {userData.PlayerRating}\n" +
               $"Основная позиция- {userData.PlayerPosition}\n" +
               $"О пользователе - {userData.PlayerDescription}\n";
    }

    public static string ConfirmCreateQuestionnaire(UsersDatum userData)
    {
        return "Подтвердите создание анкеты. Ваши данные:\n " +
               $"Имя - {userData.PlayerName}\n" +
               $"Рейтинг- {userData.PlayerRating}\n" +
               $"Основная позиция- {userData.PlayerPosition}\n" +
               $"Ваш ник tg- {userData.PlayerTgNick}\n" +
               $"Ваше описание - {userData.PlayerDescription}\n";
    }

    public static string TournamentsRules =>
        $"Вы можете ознакомиться с правилама в данном файле";

    public const string QuestionnaireDeleted = "Анкета удалена :_(";

    public const string ChangeTournamentData =
        "Введите основную информацию о турнире (вместе со всеми ссылками, в том числе ссылкой на регистрацию):";

    //public const string ChangeTournamentRules = "Отправьте ссылку на правила турнира:";

    public const string QuestionnaireDeletedConfirmation = "Вы уверены что хотите удалить анкету ?";

    public const string NewQuestionnaireNameInput = "Давайте познакомимся?! Введите ваше имя: ";

    public const string NewQuestionnaireUserTgNickname =
        "Введите ваш nickname тг в формате @122345(впоследствии по нему с вами будут связываться): ";

    public const string NewQuestionnaireRatingInput = "Введите ваш рейтинг (0-12000) ммр: ";

    public const string NewQuestionnairePosition = "Укажите вашу основную позицию: ";

    public const string NewUserInformation =
        "Расскажите о себе (действительно важная информация, которая поможет найти команду): ";
}