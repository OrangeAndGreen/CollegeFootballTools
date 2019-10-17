//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Runtime.Serialization.Json;
//using System.Text;
//using FootballTools.CFBData;

///*
//ncaa.com has a large JSON API that I use for data collection during the season.There's no documentation, but you can navigate it pretty easily.
//Here's some examples:

//2016 FBS list of games
//http://data.ncaa.com/sites/default/files/data/scoreboard/football/fbs/2016/01/scoreboard.json

//An example of game data from 2015
//http://data.ncaa.com/sites/default/files/data/game/football/fbs/2015/09/03/north-carolina-south-carolina/gameinfo.json

//Play-by-play data(not sure how this works during a live game)
//http://data.ncaa.com/sites/default/files/data/game/football/fbs/2015/09/03/north-carolina-south-carolina/pbp.json

//edit: looks like they have data back to 2011:
//http://data.ncaa.com/sites/default/files/data/game/football/fbs/2011/09/01/murray-st-louisville/teamStats.json
//*/
//namespace FootballTools.NCAA
//{
//    public class NcaaDownloader
//    {
//        //NCAA URL, not updating as of 20191014
//        //string url = string.Format("http://data.ncaa.com/sites/default/files/data/scoreboard/football/fbs/{0}/{1:00}/scoreboard.json", season, week);


/*
 * NOTE: Not using this anymore, but hanging on to the old code in case I revive it later
 * Really just need the NCAA API URL, the rest of the code had already changed to support CFBData instead
 */



//        public List<Game> AllGames { get; set; }
//        public List<string> Teams { get; set; }
//        public Dictionary<string, List<string>> Conferences { get; set; }
//        public List<Conference> AllConferences { get; set; }

//        public void Analyze(int year)
//        {
//            //Download all games for the specified year
//            AllGames = new List<Game>();

//            Teams = new List<string>();
//            Conferences = new Dictionary<string, List<string>>();
//            for (int week = 1; week <= 15; week++)
//            {
//                Console.WriteLine($"Downloading week {week}");
//                //NCAA URL, not updating as of 20191014
//                //string url = string.Format("http://data.ncaa.com/sites/default/files/data/scoreboard/football/fbs/{0}/{1:00}/scoreboard.json", season, week);
//                string url = $"https://api.collegefootballdata.com/games?year={year}&week={week}&seasonType=regular";
//                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
//                request.AutomaticDecompression = DecompressionMethods.GZip;

//                try
//                {
//                    List<Game> games = new List<Game>();
//                    //Get the data from the HTTP request
//                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
//                    using (Stream stream = response.GetResponseStream())
//                    {
//                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Game>));
//                        games = (List<Game>)serializer.ReadObject(stream);
//                    }

//                    if (games != null)
//                    {
//                        foreach (Game game in games)
//                        {
//                            AllGames.Add(game);

//                            string homeConf = game.home_conference;
//                            string awayConf = game.away_conference;

//                            if (homeConf != null)
//                            {
//                                if (!Conferences.ContainsKey(homeConf))
//                                {
//                                    Conferences[homeConf] = new List<string>();
//                                }
//                                if (game.home_team != null && !Conferences[homeConf].Contains(game.home_team))
//                                {
//                                    Conferences[homeConf].Add(game.home_team);
//                                }
//                            }

//                            if (awayConf != null)
//                            {
//                                if (!Conferences.ContainsKey(awayConf))
//                                {
//                                    Conferences[awayConf] = new List<string>();
//                                }
//                                if (game.away_team != null && !Conferences[awayConf].Contains(game.away_team))
//                                {
//                                    Conferences[awayConf].Add(game.away_team);
//                                }
//                            }

//                            if (game.home_team != null && !Teams.Contains(game.home_team))
//                            {
//                                Teams.Add(game.home_team);
//                            }

//                            if (game.away_team != null && !Teams.Contains(game.away_team))
//                            {
//                                Teams.Add(game.away_team);
//                            }
//                        }
//                    }
//                }
//                catch (Exception e)
//                {
//                    Console.WriteLine("Exception: " + e.Message);
//                }
//            }

//            Console.WriteLine("Conferences: {0}", Conferences.Count);
//            Console.WriteLine("Games: {0}", AllGames.Count);
//            Console.WriteLine("Teams: {0}", Teams.Count);

//            BuildConferenceStructure();
//        }

//        private void BuildConferenceStructure()
//        {
//            AllConferences = new List<Conference>();

//            //Load the tex file containing division info
//            string[] lines = File.ReadAllLines("Divisions.txt");
//            foreach (string line in lines)
//            {
//                //Find the conference or create a new one
//                Conference foundConference = null;
//                //Conf,team,division
//                string[] parts = line.Split(',');
//                string conferenceName = parts[0].Trim();
//                foreach (Conference conference in AllConferences)
//                {
//                    if (!conference.Name.Equals(conferenceName))
//                    {
//                        continue;
//                    }
//                    foundConference = conference;
//                    break;
//                }

//                if (foundConference == null)
//                {
//                    foundConference = new Conference(conferenceName);
//                    AllConferences.Add(foundConference);
//                }

//                //Find the division or create a new one
//                Division foundDivision = null;
//                string divisionName = parts[2].Trim();
//                foreach (Division division in foundConference.Divisions)
//                {
//                    if (!division.Name.Equals(divisionName))
//                    {
//                        continue;
//                    }
//                    foundDivision = division;
//                    break;
//                }

//                if (foundDivision == null)
//                {
//                    foundDivision = new Division(divisionName);
//                    foundConference.Divisions.Add(foundDivision);
//                }

//                foundDivision.Teams.Add(new Team(parts[1].Trim()));
//            }


//            //foreach (string conferenceName in Conferences.Keys)
//            //{
//            //    List<Team> teams = new List<Team>();
//            //    foreach (string team in Conferences[conferenceName])
//            //    {
//            //        teams.Add(new Team {Name = team});
//            //    }

//            //    Conference conference = new Conference
//            //    {
//            //        Name = conferenceName,
//            //    };

//            //    AllConferences.Add(conference);
//            //}
//        }
//    }
//}
