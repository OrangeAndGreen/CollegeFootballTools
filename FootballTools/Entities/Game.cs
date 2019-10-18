using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace FootballTools.Entities
{
    [DebuggerDisplay("Game: {home_team} vs. {away_team}   ({home_points}-{away_points})")]
    [DataContract]
    public class Game
    {
        [DataMember(IsRequired = false)]
        public int id = 0;
        [DataMember(IsRequired = false)]
        public int season = 0;
        [DataMember(IsRequired = false)]
        public int week = 0;
        [DataMember(IsRequired = false)]
        public string season_type = null;
        [DataMember(IsRequired = false)]
        public string start_date;
        [DataMember(IsRequired = false)]
        public bool neutral_site;
        [DataMember(IsRequired = false)]
        public bool conference_game;
        [DataMember(IsRequired = false)]
        public int? attendance;
        [DataMember(IsRequired = false)]
        public int venue_id;
        [DataMember(IsRequired = false)]
        public string venue;
        [DataMember(IsRequired = false)]
        public string home_team;
        [DataMember(IsRequired = false)]
        public string home_conference;
        [DataMember(IsRequired = false)]
        public int? home_points;
        [DataMember(IsRequired = false)]
        public int[] home_line_scores;
        [DataMember(IsRequired = false)]
        public string away_team;
        [DataMember(IsRequired = false)]
        public string away_conference;
        [DataMember(IsRequired = false)]
        public int? away_points;
        [DataMember(IsRequired = false)]
        public int[] away_line_scores;

        public DateTime GameDate
        {
            get
            {
                try
                {
                    //yyyy-MM-ddTHH:mm:ss.SSSZ
                    int year = int.Parse(start_date.Substring(0, 4));
                    int month = int.Parse(start_date.Substring(5, 2));
                    int day = int.Parse(start_date.Substring(8, 2));
                    int hour = int.Parse(start_date.Substring(11, 2));
                    int minute = int.Parse(start_date.Substring(14, 2));
                    int second = int.Parse(start_date.Substring(17, 2));

                    return new DateTime(year, month, day, hour, minute, second);
                }
                catch(Exception)
                {
                    return DateTime.Now;
                }
            }
        }

        public bool ConferenceGame => (home_conference?.Equals(away_conference ?? string.Empty)) ?? false;
        public bool GameAlreadyPlayed => home_points.HasValue && away_points.HasValue;

        public bool? HomeWin => GameAlreadyPlayed ? (home_points.Value > away_points.Value) : (bool?)null;
        public bool? AwayWin => GameAlreadyPlayed ? (away_points.Value > home_points.Value) : (bool?)null;

        public string Winner => GameAlreadyPlayed ? (home_points.Value > away_points.Value ? home_team : away_team) : null;

        public bool InvolvesTeam(string team)
        {
            return home_team.Equals(team) || away_team.Equals(team);
        }

        public static List<Game> FilterGamesByTeams(List<Game> games, List<string> teams)
        {
            List<Game> filtered = new List<Game>();

            foreach(Game game in games)
            {
                foreach(string team in teams)
                {
                    if(game.InvolvesTeam(team))
                    {
                        filtered.Add(game);
                        break;
                    }
                }
            }

            return filtered;
        }

        public static void SortGameList(List<Game> games)
        {
            games.Sort(
                    delegate (Game g1, Game g2)
                    {
                        int compareDate = g1.week.CompareTo(g2.week);
                        if (compareDate == 0)
                        {
                            return g1.GameDate.CompareTo(g2.GameDate);
                        }
                        return compareDate;
                    }
                );
        }
    }
}
