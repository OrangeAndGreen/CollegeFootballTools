using FootballTools.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;

namespace FootballTools.CFBData
{
    /*
     * All API calls:
     * /games	                Get game information
     * ​/games​/players	        Get player statistics by game
     * ​/games​/teams	            Get team statistics by game
     * ​/game​/box​/advanced	    Get advanced box score
     * ​/drives	                Get drive information
     * /plays	                Get play information. Requires either a week or team to be specified.
     * /play​/types	            Get play type list
     * /play​/stats	            Get play statistics
     * /play​/stat​/types	        Get play stat type lists
     * ​/teams	                Get team information
     * ​/teams​/fbs	            Get list of major division teams for a given year
     * ​/roster	                Get team roster
     * ​/talent	                Get team talent rankings
     * ​/teams​/matchup	        Get matchup history
     * ​/conferences	            Get conference list
     * ​/venues	                Get venue information
     * ​/coaches	                Get coach records and school history
     * ​/player​/search	        Search for players
     * ​/player​/usage	        Get player usage metrics for the season
     * ​/rankings	            Get historical rankings and polls
     * ​/lines	                Get betting line information
     * ​/recruiting​/players	    Get player recruiting rankings and data. Requires either a year or team to be specified.
     * ​/recruiting​/teams	    Get team recruiting rankings
     * ​/recruiting​/groups	    Get position group aggregated ratings
     * ​/ratings​/sp	            Get S&P+ historical rating data (requires either a year or team specified)
     * ​/ratings​/sp​/conferences	Get average S&P+ historical rating data by conference
     * ​/ppa​/predicted	        Calculate Predicted Points
     * ​/ppa​/teams	            Get team averages for Predicted Points Added (PPA)
     * ​/ppa​/games	            Get team game averages for Predicted Points Added (PPA)
     * ​/ppa​/players​/games	    Get player game averages for Predicted Points Added (PPA)
     * ​/ppa​/players​/season	    Get player season averages for Predicted Points Added (PPA)
     * ​/metrics​/wp	            Get win probability chart data
     * ​/stats​/season	        Get season team stats
     * ​/stats​/season​/advanced	Get advanced season team stats
     * ​/stats​/game​/advanced	    Get advanced game team stats
     * /stats​/categories	    Get stat category list
     */

    /// <summary>
    /// Handles downloading and caching for data from the collegefootballdata API
    /// API documentation is here:
    /// https://api.collegefootballdata.com/api/docs/?url=/api-docs.json
    /// </summary>
    public class CfbDownloader
    {
        private static readonly string CacheDirectory = "Cache";

        private static string GetCacheFilepath(int year, int week)
        {
            return Path.Combine(CacheDirectory, $"{year}_{week}.json");
        }

        public static GameList RetrieveSeason(int year, bool forceDownload = false)
        {
            //Download all games for the specified year
            GameList games = new GameList();

            for (int week = 1; week <= 15; week++)
            {
                try
                {
                    GameList weekGames = RetrieveWeeklyGameList(year, week, forceDownload);

                    if (weekGames?.Count > 0)
                    {
                        games.AddRange(weekGames);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }
            }

            return games;
        }
        
        #region Individual download methods

        public static GameList RetrieveWeeklyGameList(int year, int week, bool forceDownload = false)
        {
            //Other possible inputs for this API call:
            //seasonType (regular/postseason), team, home, away, conference (all strings)

            string cacheName = GetCacheFilepath(year, week);

            GameList weekGames = null;
            if (!forceDownload)
            {
                weekGames = CacheHelper.RetrieveItemFromCache<GameList>(cacheName);
            }

            if (weekGames == null)
            {
                Console.WriteLine($"Downloading week {year}-{week}");
                string url = $"https://api.collegefootballdata.com/games?year={year}&week={week}&seasonType=regular";
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;


                //Get the data from the HTTP request
                using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(GameList));
                    weekGames = (GameList) serializer.ReadObject(stream);
                    if (weekGames != null && weekGames.Count > 0)
                    {
                        CacheHelper.Cache(cacheName, weekGames);
                    }
                }
            }
            else
            {
                Console.WriteLine($"Retrieved week {year}-{week} from cache");
            }

            return weekGames;
        }



        #endregion
    }
}
