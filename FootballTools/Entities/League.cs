using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace FootballTools.Entities
{
    public class League
    {
        public List<Conference> Conferences { get; set; }

        public League(GameList games)
        {
            Conferences = LoadFromFiles();

            IntegrateGameInfo(games);
        }
        

        #region Loading

        public static List<Conference> LoadFromFiles()
        {
            List<Conference> ret;

            int divisionIdAssigner = 1;

            //Load all conferences from file
            using (Stream stream = new FileStream("ActualData/conferences.json", FileMode.Open, FileAccess.Read))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Conference>));
                ret = (List<Conference>)serializer.ReadObject(stream);
            }

            //Add a "None" conference for the leftover teams, and initialize each conference's list of divisions
            ret.Add(new Conference(9999, "None"));
            foreach (Conference conference in ret)
            {
                conference.Divisions = new List<Division>();
            }

            //Load all teams from file
            List<Team> allTeams;
            string teamsText = File.ReadAllText("ActualData/teams.json");
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(teamsText)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Team>));
                allTeams = (List<Team>)serializer.ReadObject(stream);
            }

            //Sort the teams into their conferences
            foreach (Team team in allTeams)
            {
                team.Schedule = new GameList();

                string conferenceName = team.ConferenceName ?? "None";
                string divisionName = team.DivisionName ?? "None";

                //See if we already setup the division
                Conference conference = null;
                foreach (Conference searchConference in ret)
                {
                    if (searchConference.Name.Equals(conferenceName))
                    {
                        conference = searchConference;
                        break;
                    }
                }

                Division division = null;
                foreach (Division searchDivision in conference.Divisions)
                {
                    if (searchDivision.Name.Equals(divisionName))
                    {
                        division = searchDivision;
                        break;
                    }
                }

                //Create a new division
                if (division == null)
                {
                    division = new Division(divisionIdAssigner, divisionName, conferenceName);
                    conference.Divisions.Add(division);
                    divisionIdAssigner++;
                }

                //Add the team to the division
                division.Teams.Add(team);
            }

            //Filter out any conferences that don't have any teams
            for (int i = 0; i < ret.Count; i++)
            {
                int index = ret.Count - 1 - i;
                if (ret[index].Divisions.Count == 0)
                {
                    ret.RemoveAt(index);
                }
            }

            return ret;
        }

        private void IntegrateGameInfo(GameList games)
        {
            foreach (Game game in games)
            {
                Team homeTeam = FindTeam(game.home_team);
                Team awayTeam = FindTeam(game.away_team);

                if (homeTeam != null)
                {
                    game.HomeTeamId = homeTeam.Id;
                }
                if (awayTeam != null)
                {
                    game.AwayTeamId = awayTeam.Id;
                }

                game.DivisionGame = homeTeam?.DivisionName != null && awayTeam?.DivisionName != null && homeTeam.DivisionName.Equals(awayTeam.DivisionName);

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
                        team.Schedule.Sort();
                    }
                }
            }
        }

        #endregion


        #region Access Helpers

        public Conference this[string conferenceName] => FindConference(conferenceName);

        public Conference this[int index] => Conferences[index];

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

        public Team FindTeam(string teamName)
        {
            foreach (Conference conference in Conferences)
            {
                foreach (Division division in conference.Divisions)
                {
                    foreach (Team team in division.Teams)
                    {
                        if (team.Name.Equals(teamName))
                        {
                            return team;
                        }
                    }
                }
            }

            return null;
        }

        public Team FindTeam(int id)
        {
            foreach (Conference conference in Conferences)
            {
                foreach (Division division in conference.Divisions)
                {
                    foreach (Team team in division.Teams)
                    {
                        if (team.Id == id)
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

        public GameList AllGames
        {
            get
            {
                GameList games = new GameList();
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

                games.Sort();

                return games;
            }
        }

        #endregion
    }
}
