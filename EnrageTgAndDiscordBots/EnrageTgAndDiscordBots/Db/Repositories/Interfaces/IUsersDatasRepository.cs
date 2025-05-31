using EnrageTgAndDiscordBots.Models;

namespace EnrageTgAndDiscordBots.Db.Repositories.Interfaces
{
    public interface IUsersDatasRepository
    {
        List<UsersDatum> GetAllUserExcept(long chatId, int? pos = 0, int? ratingFrom = -1, int? ratingTo = -1);
        UsersDatum GetLastUserDataByChatId(long chatId);
        
        UsersDatum GetFirstUserDataByChatId(long chatId);
        void AddUser(string playerName, int playerRating, int playerPos, string playerTgNick, long chatId, string playerDescription);
        void UpdateUser(UsersDatum usersData);
        void DeleteUser(UsersDatum userData);
    }
}
