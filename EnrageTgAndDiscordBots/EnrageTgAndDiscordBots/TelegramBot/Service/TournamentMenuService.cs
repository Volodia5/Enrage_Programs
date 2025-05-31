using EnrageTgBotILovePchel.Bot;
using EnrageTgBotILovePchel.Bot.Router;
using EnrageTgBotILovePchel.Util.Button;
using EnrageTgBotILovePchel.Util.String;
using Telegram.Bot.Types.ReplyMarkups;

namespace EnrageTgBotILovePchel.Service
{
    public class TournamentMenuService
    {
        // private ITournamentDatasRepository _tournamentDatasRepository;
        //
        // public TournamentMenuService(ITournamentDatasRepository tournamentDatasRepository)
        // {
        //     _tournamentDatasRepository = tournamentDatasRepository;
        // }
        //
        // public BotMessage ProcessInputTournamentRules(string textData, TransmittedData transmittedData)
        // {
        //     //TournamentsDatum tournamentData = _tournamentDatasRepository.GetTournamentsData();
        //     // _tournamentDatasRepository.UpdateTournamentRules(tournamentData.TournId, textData);
        //     transmittedData.State = States.MainMenu.ClickOnInlineButton;
        //     
        //     return new BotMessage("Данные о предстоящем турнире успешно изменены !!!" + DialogsStringsStorage.MainMenu, InlineKeyboardMarkupStorage.AdminMainMenu, MessageState.Create);
        // }
    }
}