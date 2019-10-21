using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FootballTools.Entities;

namespace FootballTools.Analysis.DivisionTiebreakers
{
    class MwcTiebreaker : ITiebreaker
    {
        /*
          1) Winning percentage in games played among the tied teams.
		  2) Winning percentage in games played against division opponents.
		  3) Winning percentage against the next highest‐placed team in the division (based upon the team's record in all games played in the conference), proceeding through the division.
		  4) Winning percentage against common conference opponents.
		  5) Highest CFP ranking (or the composite of selected computer rankings if neither team is ranked in the CFP rankings) following the final week of conference regular‐season games
         */

        public int BreakTie(GameList games, List<int> winners, List<TeamResult> teamResults, List<int> teamIds, Division division)
        {
            return -1;
        }
    }
}
