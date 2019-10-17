using System.Collections.Generic;
using System.IO;

namespace FootballTools.Entities
{
    public class League
    {
        public List<Conference> Conferences { get; set; }

        public League(List<Game> games, string divisionFile)
        {
            Conferences = StructureFromFile(divisionFile);

            IntegrateGameInfo(games);
        }

        public Conference this[string conferenceName] => FindConference(conferenceName);

        public Conference this[int index] => Conferences[index];

        private static List<Conference> StructureFromFile(string divisionFile)
        {
            List<Conference> ret = new List<Conference>();

            //Load the tex file containing division info
            string[] lines = File.ReadAllLines(divisionFile);
            foreach (string line in lines)
            {
                //Find the conference or create a new one
                Conference foundConference = null;
                //Conf,team,division
                string[] parts = line.Split(',');
                string conferenceName = parts[0].Trim();
                foreach (Conference conference in ret)
                {
                    if (!conference.Name.Equals(conferenceName))
                    {
                        continue;
                    }
                    foundConference = conference;
                    break;
                }

                if (foundConference == null)
                {
                    foundConference = new Conference(conferenceName);
                    ret.Add(foundConference);
                }

                //Find the division or create a new one
                Division foundDivision = null;
                string divisionName = parts[2].Trim();
                foreach (Division division in foundConference.Divisions)
                {
                    if (!division.Name.Equals(divisionName))
                    {
                        continue;
                    }
                    foundDivision = division;
                    break;
                }

                if (foundDivision == null)
                {
                    foundDivision = new Division(divisionName);
                    foundConference.Divisions.Add(foundDivision);
                }

                foundDivision.Teams.Add(new Team(parts[1].Trim()));
            }

            ret.Sort((x, y) => x.Name.CompareTo(y.Name));

            foreach(Conference conference in ret)
            {
                conference.Divisions.Sort((x, y) => x.Name.CompareTo(y.Name));
                foreach (Division division in conference.Divisions)
                {
                    division.Teams.Sort((x, y) => x.Name.CompareTo(y.Name));
                }
            }

            return ret;
        }

        private  void IntegrateGameInfo(List<Game> games)
        {
            foreach (Game game in games)
            {
                Team homeTeam = FindTeam(game.home_team);
                Team awayTeam = FindTeam(game.away_team);

                homeTeam?.Schedule.Add(game);
                awayTeam?.Schedule.Add(game);

                if (!game.GameAlreadyPlayed)
                {
                    continue;
                }

                if (game.HomeWin.Value)
                {
                    homeTeam?.RecordWin(game.ConferenceGame);
                    awayTeam?.RecordLoss(game.ConferenceGame);
                }
                else if (game.AwayWin.Value)
                {
                    homeTeam?.RecordLoss(game.ConferenceGame);
                    awayTeam?.RecordWin(game.ConferenceGame);
                }
                else
                {
                    homeTeam?.RecordTie(game.ConferenceGame);
                    awayTeam?.RecordTie(game.ConferenceGame);
                }
            }

            foreach (Conference conference in Conferences)
            {
                foreach (Division division in conference.Divisions)
                {
                    foreach (Team team in division.Teams)
                    {
                        Game.SortGameList(team.Schedule);
                    }
                }
            }
        }

        public Conference FindConference(string name)
        {
            foreach (Conference conference in Conferences)
            {
                if (conference.Name.Equals(name))
                {
                    return conference;
                }
            }

            return null;
        }

        public Division FindDivision(string conferenceName, string divisionName)
        {
            foreach (Conference conference in Conferences)
            {
                if (conference.Name.Equals(conferenceName))
                {
                    foreach (Division division in conference.Divisions)
                    {
                        if (division.Name.Equals(divisionName))
                        {
                            return division;
                        }
                    }
                }
            }

            return null;
        }

        public Team FindTeam(string name)
        {
            foreach (Conference conference in Conferences)
            {
                foreach (Division division in conference.Divisions)
                {
                    foreach (Team team in division.Teams)
                    {
                        if (team.Name.Equals(name))
                        {
                            return team;
                        }
                    }
                }
            }

            return null;
        }

        public List<Team> AllTeams
        {
            get
            {
                List<Team> ret = new List<Team>();

                foreach(Conference conference in Conferences)
                {
                    ret.AddRange(conference.AllTeams);
                }

                return ret;
            }
        }

        public List<Game> AllGames
        {
            get
            {
                List<Game> games = new List<Game>();
                foreach (Conference conference in Conferences)
                {
                    foreach (Division division in conference.Divisions)
                    {
                        foreach (Team team in division.Teams)
                        {
                            foreach (Game game in team.Schedule)
                            {
                                if (!games.Contains(game))
                                {
                                    games.Add(game);
                                }
                            }
                        }
                    }
                }

                Game.SortGameList(games);

                return games;
            }
        }
    }
}
