using EnrageTgAndDiscordBots.Models;

namespace EnrageTgAndDiscordBots.Db.Repositories.Interfaces
{
    public interface IProgramActivationCodeDatasRepository
    {
        ProgramKey GetKey(string key);
        List<ProgramKey> GetAllKeys();
        void AddKey(string key, DateTime expiredDate);
        void DeleteKey(string key);
    }
}
