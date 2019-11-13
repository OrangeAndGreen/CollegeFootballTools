using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using FootballTools.Entities;

namespace FootballTools.Analysis.DivisionTiebreakers
{
    class AccTiebreaker : ITiebreaker
    {
        /*
          1) Combined head-to-head winning percentage among the tied teams.
	      2) Winning percentage of the tied teams within the division.
		  3) Head-to-head competition versus the team within the division with the best overall (divisional and non-divisional) conference winning percentage, and proceeding through the division.
		        Multiple ties within the division will be broken first to last, using the league's tie-breaking procedures.
		  4) Combined winning percentage versus all common non-divisional opponents.
		  5) Combined winning percentage versus all non-divisional opponents.
		  6) Winning percentage versus common non-divisional opponents based upon their order of finish (overall conference winning percentage) and proceeding through other common non-divisional opponents based upon their divisional order of finish. The tied team with the highest ranking in the full (College Football Playoff) standings following the conclusion of regular season games, unless the second of the tied teams is ranked within five-or-fewer places of the highest ranked tied team. In this case, the two-team tiebreaking procedure shall be applied between the top two ranked tied teams. If all tied teams are not ranked in the full (College Football Playoff) Standings, the computer ranking portion of the Standings will be used, eliminating the high and the low computer ranking, and averaging the remaining rankings. (This portion of the publicly available rule is outdated, as the College Football Playoff no longer uses BCS computers.)
		  7) The representative shall be chosen by a draw as administered by the Commissioner or Commissioner's designee.
         */

        //public int OldBreakTie(GameList games, List<int> winners, List<TeamResult> teamResults, List<int> teamIds, Division division)
        //{
        //    //1) See if any team wins the combined head-to-head matchups
        //    List<int> tieWinners = games.GetWinnersFromTeams(teamIds, false, winners);

        //    if(tieWinners.Count == 1)
        //    {
        //        return tieWinners[0];
        //    }

        //    //2) See if any team win has the best record within the division

        //    //Build the list of team names for the division
        //    //List<int> divisionTeamIds = Team.GetTeamIds(division.Teams);

        //    tieWinners = TeamResult.GetWinningTeamIds(teamResults, RecordType.Division, winners);// games.GetWinnersFromTeams(divisionTeamIds, false, winners);

        //    if (tieWinners.Count == 1)
        //    {
        //        return tieWinners[0];
        //    }

        //    return -1;
        //}

        public int BreakTie(GameList games, List<int> winners, List<TeamResult> teamResults, List<int> teamIds, Division division)
        {
            List<int> finalists = new List<int>(teamIds);

            while (true)
            {
                int startCount = finalists.Count;
                switch (startCount)
                {
                    case 1:
                        return finalists[0];
                    case 2:
                    {
                        return BreakTwoWayTie(games, winners, finalists[0], finalists[1]);
                    }
                    default:
                        finalists = BreakLargeTie(games, winners, finalists, division);
                        break;
                }

                if (finalists.Count == startCount)
                {
                    //Can't break the tie
                    return -1;
                }
            }
        }

        public int BreakTwoWayTie(GameList games, List<int> winners, int team1, int team2)
        {
            int index = games.FindMatchupIndex(team1, team2);
            return winners[index];
        }

        public List<int> BreakLargeTie(GameList games, List<int> winners, List<int> finalists, Division division)
        {
            List<int> newFinalists = new List<int>(finalists);

            //Build some records for the finalists
            Dictionary<int, Record> headToHeadRecords = new Dictionary<int, Record>();
            int mostHeadToHeadWins = 0;
            Dictionary<int, Record> divisionRecords = new Dictionary<int, Record>();
            int mostDivisionWins = 0;
            foreach (int finalistId in newFinalists)
            {
                headToHeadRecords[finalistId] = new Record(0, 0);
                divisionRecords[finalistId] = new Record(0, 0);
            }

            for (int i = 0; i < games.Count; i++)
            {
                Game game = games[i];
                int winnerId = winners[i];
                foreach (int finalistId in newFinalists)
                {
                    if (game.InvolvesTeam(finalistId))
                    {
                        int otherTeamId = game.HomeTeamId == finalistId ? game.AwayTeamId : game.HomeTeamId;
                        if (newFinalists.Contains(otherTeamId))
                        {
                            if (winnerId == finalistId)
                            {
                                headToHeadRecords[finalistId].Wins++;
                                mostHeadToHeadWins = Math.Max(mostHeadToHeadWins, headToHeadRecords[finalistId].Wins);
                            }
                            else
                            {
                                headToHeadRecords[finalistId].Losses++;
                            }
                        }

                        if (game.DivisionGame)
                        {
                            if (winnerId == finalistId)
                            {
                                divisionRecords[finalistId].Wins++;
                                mostDivisionWins = Math.Max(mostDivisionWins, divisionRecords[finalistId].Wins);
                            }
                            else
                            {
                                divisionRecords[finalistId].Losses++;
                            }
                        }
                    }
                }
            }

            //Now see if we can rule out any teams based on records
            bool excludedFinalist = false;
            for (int i = newFinalists.Count - 1; i >= 0; i--)
            {
                if (headToHeadRecords[newFinalists[i]].Wins < mostHeadToHeadWins)
                {
                    newFinalists.RemoveAt(i);
                    excludedFinalist = true;
                }
            }

            if (!excludedFinalist)
            {
                for (int i = newFinalists.Count - 1; i >= 0; i--)
                {
                    if (divisionRecords[newFinalists[i]].Wins < mostDivisionWins)
                    {
                        newFinalists.RemoveAt(i);
                        excludedFinalist = true;
                    }
                }
            }

            if (!excludedFinalist)
            {
                if (true)//newFinalists.Contains(2390))
                {
                    Console.WriteLine("Couldn't break tie:");
                    Console.WriteLine("Head to head records:");
                    foreach (int teamId in newFinalists)
                    {
                        Team team = division.FindTeam(teamId);
                        Console.WriteLine($"   {team.Name}: {headToHeadRecords[teamId]}");
                    }

                    Console.WriteLine("Division records:");
                    foreach (int teamId in newFinalists)
                    {
                        Team team = division.FindTeam(teamId);
                        Console.WriteLine($"   {team.Name}: {divisionRecords[teamId]}");
                    }

                    Console.WriteLine();
                }
            }

            return newFinalists;
        }
    }
}
