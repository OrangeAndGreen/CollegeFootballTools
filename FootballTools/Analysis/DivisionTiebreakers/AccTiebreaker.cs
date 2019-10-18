using System;
using System.Collections.Generic;
using System.Linq;
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

        public string BreakTie(List<Game> games, List<string> winners, List<string> teamNames, Division division)
        {
            //1) See if any team wins the combined head-to-head matchups
            int[] headToHeadWins = new int[teamNames.Count];
            for(int gameIndex = 0; gameIndex < games.Count; gameIndex++)
            {
                Game game = games[gameIndex];
                bool gameProcessed = false;
                for (int index1= 0; index1 < teamNames.Count && !gameProcessed; index1++)
                {
                    string team1 = teamNames[index1];
                    if (!game.InvolvesTeam(team1))
                    {
                        continue;
                    }

                    for (int index2 = index1 + 1; index2 < teamNames.Count; index2++)
                    {
                        string team2 = teamNames[index2];
                        if (!game.InvolvesTeam(team2))
                        {
                            continue;
                        }

                        string winner = null;
                        if (winners.Count > gameIndex)
                        {
                            winner = winners[gameIndex];
                        }
                        else if (game.GameAlreadyPlayed)
                        {
                            winner = game.Winner;
                        }

                        if (winner != null)
                        {
                            int winnerIndex = winners[gameIndex].Equals(team1) ? index1 : index2;
                            headToHeadWins[winnerIndex]++;
                        }
                        
                        gameProcessed = true;
                        break;
                    }
                }
            }

            int maxWins = -1;
            List<string> tiedTeams = new List<string>();
            for(int i=0; i< teamNames.Count; i++)
            {
                int headToHead = headToHeadWins[i];
                if (headToHead > maxWins)
                {
                    maxWins = headToHead;
                    tiedTeams.Clear();
                    tiedTeams.Add(teamNames[i]);
                }
                else if (headToHead == maxWins)
                {
                    tiedTeams.Add(teamNames[i]);
                }
            }

            if(tiedTeams.Count == 1)
            {
                return tiedTeams[0];
            }

            //2) See if any team win has the best record within the division

            //Build the list of team names for the division
            List<string> divisionTeamNames = new List<string>();
            foreach (Team team in division.Teams)
            {
                divisionTeamNames.Add(team.Name);
            }

            List<Game> divisionGames = Game.FilterGamesByTeams(division.AllGames, divisionTeamNames);
            int[] divisionWins = new int[divisionTeamNames.Count];
            for(int gameIndex = 0; gameIndex < divisionGames.Count; gameIndex++)
            {
                Game game = divisionGames[gameIndex];
                bool gameProcessed = false;
                for (int index1 = 0; index1 < divisionTeamNames.Count && !gameProcessed; index1++)
                {
                    string team1 = divisionTeamNames[index1];
                    if (game.GameAlreadyPlayed && game.InvolvesTeam(team1))
                    {
                        for (int index2 = 0; index2 < divisionTeamNames.Count; index2++)
                        {
                            string team2 = divisionTeamNames[index2];
                            if (game.InvolvesTeam(team2))
                            {
                                string winner = null;
                                if (winners.Count > gameIndex)
                                {
                                    winner = winners[gameIndex];
                                }
                                else if (game.GameAlreadyPlayed)
                                {
                                    winner = game.Winner;
                                }

                                if (winner != null)
                                {
                                    int winnerIndex = winner.Equals(team1) ? index1 : index2;
                                    divisionWins[winnerIndex]++;
                                }

                                gameProcessed = true;
                                break;
                            }
                        }
                    }
                }
            }

            maxWins = -1;
            tiedTeams = new List<string>();
            for (int i = 0; i < divisionTeamNames.Count; i++)
            {
                int divisionWin = divisionWins[i];
                if (divisionWin > maxWins)
                {
                    maxWins = divisionWin;
                    tiedTeams.Clear();
                    tiedTeams.Add(divisionTeamNames[i]);
                }
                else if (divisionWin == maxWins)
                {
                    tiedTeams.Add(divisionTeamNames[i]);
                }
            }

            if (tiedTeams.Count == 1)
            {
                return tiedTeams[0];
            }

            return null;
        }
    }
}
