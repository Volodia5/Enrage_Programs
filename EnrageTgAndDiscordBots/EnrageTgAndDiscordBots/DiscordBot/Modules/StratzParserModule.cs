using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using STRATZ;
using EnrageDiscordTournamentBot.Bot;

namespace EnrageDiscordTournamentBot.Modules
{
    public class StratzParserModule
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public async Task<string> GetRank(string userAccountId)
        {
            string responceContent;
            string responseUserId;
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.stratz.com/graphql");
            request.Headers.Add("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJTdWJqZWN0IjoiYjllYzI0MTItOThmZS00MGIzLWExNTYtZTZkMTRhNzBmZDk5IiwiU3RlYW1JZCI6IjMxNDQ2MzUwOSIsIm5iZiI6MTcyNjQwNTM4OCwiZXhwIjoxNzU3OTQxMzg4LCJpYXQiOjE3MjY0MDUzODgsImlzcyI6Imh0dHBzOi8vYXBpLnN0cmF0ei5jb20ifQ.EkKF0gaxe70M7I3gwj98EaFzuXA9zLaqgPKuVzQRBu0");
            request.Headers.Add("User-Agent", "STRATZ_API");
            var content = new StringContent("{\"query\":\"{\\r\\n  player(steamAccountId: " + userAccountId + ") {\\r\\n    ranks {\\r\\n      rank,\\r\\n    },\\r\\n    steamAccount{\\r\\n      seasonLeaderboardRank\\r\\n    }\\r\\n  } \\r\\n}\",\"variables\":{}}", null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            responceContent = await response.Content.ReadAsStringAsync();

            if (responceContent.Contains(
                    "Player Id is missing or anonymous :"))
            {
                string seasonRank = responceContent.Split("\"seasonLeaderboardRank\":")[1];
                seasonRank = seasonRank.Split('}')[0];

                if (seasonRank == "null")
                {
                    Console.WriteLine(ErrorApiCodes.InvalidOrHideId);
                    return ErrorApiCodes.InvalidOrHideId.ToString();
                }
                else
                {
                    return ErrorApiCodes.SeasonRank.ToString();
                }
            }

            try
            {
                responseUserId = responceContent.Split("{\"data\":{\"player\":{\"ranks\":[{\"rank\":")[1];
                responseUserId = responseUserId.Split("}")[0];

                return responseUserId;
            }
            catch (Exception e)
            {
                return ErrorApiCodes.SteamId.ToString();
            }
        }
    }
}