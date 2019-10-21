using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FootballTools.Entities;

namespace FootballTools.Analysis.DivisionTiebreakers
{
    class Pac12Tiebreaker : ITiebreaker
    {
        /*
          1) Head-to-head results (best record in games between tied teams).
		  2) Record in inter-divisional games.
		  3) Record against the next highest placed team in the division (based on the record in all conference games), proceeding through the division.
		  4) Record in common conference games.
		  5) Highest ranking in College Football Playoff poll entering the final weekend of the regular season.
         */

        public int BreakTie(GameList games, List<int> winners, List<TeamResult> teamResults, List<int> teamIds, Division division)
        {
            return -1;
        }
    }
}
