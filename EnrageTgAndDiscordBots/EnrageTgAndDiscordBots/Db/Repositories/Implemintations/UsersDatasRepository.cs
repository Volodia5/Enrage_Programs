using EnrageTgAndDiscordBots.Db.Repositories.Interfaces;
using EnrageTgAndDiscordBots.DbConnector;
using EnrageTgAndDiscordBots.Models;

namespace EnrageTgAndDiscordBots.Db.Repositories.Implemintations
{
    class UsersDatasRepository : IUsersDatasRepository
    {
        private EnrageBotVovodyaDbContext _dbContext;

        public UsersDatasRepository(EnrageBotVovodyaDbContext db)
        {
            _dbContext = db;
        }

        public List<UsersDatum> GetAllUserExcept(long chatId, int? pos = 0, int? ratingFrom = -1, int? ratingTo = -1)
        {
            List<UsersDatum> usersDatasList = new List<UsersDatum>();
            IQueryable<UsersDatum> usersDatasFromDb;

            if (pos == 0 && ratingFrom == -1 && ratingTo == -1)
            {
                usersDatasFromDb = _dbContext.UsersData.Where(x => x.ChatId != chatId);

                foreach (var item in usersDatasFromDb)
                {
                    usersDatasList.Add(item);
                }
            }

            if (pos != 0 && ratingFrom != -1 && ratingTo != -1)
            {
                usersDatasFromDb = _dbContext.UsersData.Where(x => x.ChatId != chatId && x.PlayerPosition == pos && x.PlayerRating >= ratingFrom && x.PlayerPosition <= ratingTo);

                foreach (var item in usersDatasFromDb)
                {
                    usersDatasList.Add(item);
                }
            }
            
            // if (pos != 0)
            // {
            //    usersDatasFromDb =
            //         _dbContext.UsersData.Where(x => x.ChatId != chatId && x.PlayerPosition == pos);
            //
            //     foreach (var item in usersDatasFromDb)
            //     {
            //         usersDatasList.Add(item);
            //     }
            // }
            
            if (ratingTo != -1)
            {
                usersDatasFromDb = _dbContext.UsersData.Where(x =>
                    x.ChatId != chatId && x.PlayerRating >= ratingFrom && x.PlayerRating <= ratingTo);
            }
            else
            {
                usersDatasFromDb = _dbContext.UsersData.Where(x =>
                    x.ChatId != chatId && x.PlayerRating >= ratingFrom);
            }

            foreach (var item in usersDatasFromDb)
            {
                usersDatasList.Add(item);
            }

            return usersDatasList;
        }

        public UsersDatum GetLastUserDataByChatId(long chatId)
        {
            var user = _dbContext.UsersData.Where(x => x.ChatId == chatId).OrderBy(x => x.Id).LastOrDefault();
            if (user == null)
            {
                return null;
            }
            else
            {
                return user;
            }
        }

        public UsersDatum GetFirstUserDataByChatId(long chatId)
        {
            var user = _dbContext.UsersData.Where(x => x.ChatId == chatId).FirstOrDefault();
            if (user == null)
            {
                return null;
            }
            else
            {
                return user;
            }
        }

        public void AddUser(string playerName, int playerRating, int playerPos, string playerTgNick, long chatId,
            string playerDescription)
        {
            _dbContext.UsersData.Add(new UsersDatum()
            {
                PlayerName = playerName,
                PlayerPosition = playerPos,
                PlayerRating = playerRating,
                PlayerTgNick = playerTgNick,
                ChatId = chatId,
                PlayerDescription = playerDescription
            });

            _dbContext.SaveChanges();
        }

        public void DeleteUser(UsersDatum userData)
        {
            UsersDatum userDatum = _dbContext.UsersData.Where(x => x.Id == userData.Id).FirstOrDefault();

            _dbContext.UsersData.Remove(userDatum);
            _dbContext.SaveChanges();
        }

        public void UpdateUser(UsersDatum inputUsersData)
        {
            UsersDatum usersDatumFromBd = _dbContext.UsersData.Where(x => x.Id == inputUsersData.Id).FirstOrDefault();
            usersDatumFromBd.PlayerPosition = inputUsersData.PlayerPosition;

            _dbContext.UsersData.Update(usersDatumFromBd);
            _dbContext.SaveChanges();
        }
    }
}