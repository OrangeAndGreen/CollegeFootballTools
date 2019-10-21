using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FootballTools.Entities;

namespace FootballTools.Analysis.DivisionTiebreakers
{
    class SunBeltTiebreaker : ITiebreaker
    {
        /*
          1) Composite records of each team in competition vs. the other teams involved in the three-way tie.
		  2) Records of each team vs. the first-place team, or their composite records against any teams tying for first place.
		  3) Records of each team vs. the second-place team, or their composite records against any teams tying for second place.
		  4) This process continues with records vs. the third-place team, fourth-place team, etc., as necessary, until the tie is broken and seeding is complete.
		  5) If a tie still exists, the Conference office shall conduct a coin toss.
         */

        public int BreakTie(GameList games, List<int> winners, List<TeamResult> teamResults, List<int> teamIds, Division division)
        {
            return -1;
        }
    }
}
