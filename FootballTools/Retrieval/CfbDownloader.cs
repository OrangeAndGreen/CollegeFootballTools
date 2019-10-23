using FootballTools.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;

namespace FootballTools.Retrieval
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
        #region Individual download methods

        public static List<Conference> RetrieveConferences(bool forceDownload = false)
        {
            string cacheName = "Conferences";

            List<Conference> conferences = null;
            if (!forceDownload)
            {
                conferences = CacheHelper.RetrieveItemFromCache<List<Conference>>(cacheName);
            }

            if (conferences == null)
            {
                Console.WriteLine($"Downloading conferences");
                string url = $"https://api.collegefootballdata.com/conferences";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;

                //Get the data from the HTTP request
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Conference>));
                    conferences = (List<Conference>)serializer.ReadObject(stream);
                    if (conferences != null && conferences.Count > 0)
                    {
                        CacheHelper.Cache(cacheName, conferences);
                    }
                }
            }
            else
            {
                Console.WriteLine($"Retrieved conferences from cache");
            }

            return conferences;
        }

        public static List<Team> RetrieveTeams(bool forceDownload = false)
        {
            string cacheName = "Teams";

            List<Team> teams = null;
            if (!forceDownload)
            {
                teams = CacheHelper.RetrieveItemFromCache<List<Team>>(cacheName);
            }

            if (teams == null)
            {
                Console.WriteLine($"Downloading teams");
                string url = $"https://api.collegefootballdata.com/teams";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;

                //Get the data from the HTTP request
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Team>));
                    teams = (List<Team>)serializer.ReadObject(stream);
                    if (teams != null && teams.Count > 0)
                    {
                        CacheHelper.Cache(cacheName, teams);
                    }
                }
            }
            else
            {
                Console.WriteLine($"Retrieved teams from cache");
            }

            return teams;
        }

        public static GameList RetrieveSeasonGameList(int year, bool forceDownload = false)
        {
            //Other possible inputs for this API call:
            //seasonType (regular/postseason), team, home, away, conference (all strings)

            string cacheName = $"Games_{year}";

            GameList games = null;
            if (!forceDownload)
            {
                games = CacheHelper.RetrieveItemFromCache<GameList>(cacheName);
            }

            if (games == null)
            {
                Console.WriteLine($"Downloading season {year} games");
                string url = $"https://api.collegefootballdata.com/games?year={year}";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;

                //Get the data from the HTTP request
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(GameList));
                    games = (GameList)serializer.ReadObject(stream);
                    if (games != null && games.Count > 0)
                    {
                        CacheHelper.Cache(cacheName, games);
                    }
                }
            }
            else
            {
                Console.WriteLine($"Retrieved season {year} games from cache");
            }

            return games;
        }

        public static GameList RetrieveWeeklyGameList(int year, int week, bool forceDownload = false)
        {
            //Other possible inputs for this API call:
            //seasonType (regular/postseason), team, home, away, conference (all strings)

            string cacheName = $"Games_{year}_{week}";

            GameList weekGames = null;
            if (!forceDownload)
            {
                weekGames = CacheHelper.RetrieveItemFromCache<GameList>(cacheName);
            }

            if (weekGames == null)
            {
                Console.WriteLine($"Downloading week {year}-{week} games");
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
                Console.WriteLine($"Retrieved week {year}-{week} games from cache");
            }

            return weekGames;
        }

        public static PlayList RetrieveSeasonPlayList(int year, bool forceDownload = false)
        {
            string cacheName = $"Plays_{year}";

            PlayList plays = null;
            if (!forceDownload)
            {
                plays = CacheHelper.RetrieveItemFromCache<PlayList>(cacheName);
            }

            if (plays == null)
            {
                Console.WriteLine($"Downloading season {year} plays");
                string url = $"https://api.collegefootballdata.com/plays?year={year}";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;

                //Get the data from the HTTP request
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(PlayList));
                    plays = (PlayList)serializer.ReadObject(stream);
                    if (plays != null && plays.Count > 0)
                    {
                        CacheHelper.Cache(cacheName, plays);
                    }
                }
            }
            else
            {
                Console.WriteLine($"Retrieved season {year} plays from cache");
            }

            return plays;
        }



        #endregion
    }
}
