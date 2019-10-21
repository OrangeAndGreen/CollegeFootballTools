using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FootballTools.Entities;

namespace FootballTools.Analysis.DivisionTiebreakers
{
    class Big10Tiebreaker : ITiebreaker
    {
        /*
          1) The records of the three tied teams will be compared against each other.
		  2) The records of the three tied teams will be compared within their division.
		  3) The records of the three teams will be compared against the next highest placed teams in their division in order of finish (4, 5 and 6).
		  4) The records of the three teams will be compared against all common conference opponents.
		  5) The highest-ranked team in the first College Football Playoff poll following the completion of Big Ten regular season conference play shall be the representative in the Big Ten Championship Game, unless the two highest ranked tied teams are ranked within one spot of each other in the College Football Playoff poll. In this case, the head-to-head results of the top two ranked tied teams shall determine the representative in the Big Ten Championship Game.
		  6) The team with the best overall winning percentage (excluding exempted games) shall be the representative.
		  7) The representative will be chosen by random draw.
         */

        public int BreakTie(GameList games, List<int> winners, List<TeamResult> teamResults, List<int> teamIds, Division division)
        {
            return -1;
        }
    }
}
