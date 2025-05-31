using EnrageTgAndDiscordBots.Models;
using EnrageTgAndDiscordBots.DbConnector;
using EnrageTgAndDiscordBots.Db.Repositories.Interfaces;

namespace EnrageTgAndDiscordBots.Db.Repositories.Implemintations
{
    public class ProgramActivationCodeDatasRepository : IProgramActivationCodeDatasRepository
    {
        private EnrageBotVovodyaDbContext _dbContext;

        public ProgramActivationCodeDatasRepository(EnrageBotVovodyaDbContext db)
        {
            _dbContext = db;
        }

        public List<ProgramKey> GetAllKeys()
        {
            var dbKeys =  _dbContext.ProgramKeys.ToList();
            _dbContext.SaveChanges();
            return dbKeys;
        }
        
        public ProgramKey GetKey(string key)
        {
            ProgramKey fullKey = _dbContext.ProgramKeys.Where(x => x.Key == key).FirstOrDefault();

            return fullKey;
        }

        public void AddKey(string key, DateTime expiredDate)
        {
            _dbContext.ProgramKeys.Add(new ProgramKey()
            {
                Key = key,
                ExpiredTime = expiredDate
            });

            _dbContext.SaveChanges();
        }

        public void DeleteKey(string key)
        {
            _dbContext.ProgramKeys.Remove(_dbContext.ProgramKeys.Where(x => x.Key == key).FirstOrDefault());
            _dbContext.SaveChanges();
        }
    }
}
