using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace FootballTools.Entities
{
    public class League
    {
        public List<Conference> Conferences { get; set; }

        public League(List<Conference> conferences, List<Team> teams, GameList games)
        {
            Conferences = Load(conferences, teams);

            IntegrateGames(games);
        }
        

        #region Loading

        public static List<Conference> Load(List<Conference> conferences, List<Team> teams)
        {
            List<Conference> ret = conferences;

            int divisionIdAssigner = 1;

            //Add a "None" conference for the leftover teams, and initialize each conference's list of divisions
            ret.Add(new Conference(9999, "None"));
            foreach (Conference conference in ret)
            {
                conference.Divisions = new List<Division>();
            }

            //Sort the teams into their conferences
            foreach (Team team in teams)
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

        public void IntegrateGames(GameList games)
        {
            foreach (Game game in games)
            {
                Team homeTeam = FindTeam(game.home_team);
                Team awayTeam = FindTeam(game.away_team);

                if (homeTeam != null)
                {
                    game.HomeTeamId = homeTeam.Id;
                }
                else
                {
                    Console.WriteLine("Check");
                }
                if (awayTeam != null)
                {
                    game.AwayTeamId = awayTeam.Id;
                }
                else
                {
                    Console.WriteLine("Check");
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

        public void IntegratPlays(PlayList plays)
        {
            foreach (Play play in plays)
            {
                Team homeTeam = FindTeam(play.Home);
                Team awayTeam = FindTeam(play.Away);
                Game game = homeTeam.Schedule.FindMatchup(homeTeam.Id, awayTeam.Id);

                if (game.Plays == null)
                {
                    game.Plays = new PlayList();
                }

                game.Plays.Add(play);
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
