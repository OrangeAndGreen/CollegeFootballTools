using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FootballTools.Entities;

namespace FootballTools.Analysis.DivisionTiebreakers
{
    class SecTiebreaker : ITiebreaker
    {
        /*
          1) Combined head-to-head record among the tied teams.
		  2) Record of the tied teams within the division.
		  3) Head-to-head competition against the team within the division with the best overall Conference record (divisional and non divisional) and proceeding through the division (multiple ties within the division will be broken from first to last and a tie for first place will be broken before a tie for fourth place).
		  4) Overall conference record against non divisional teams.
		  5) Combined record against all common non divisional teams.
		  6) Record against the common non divisional team with the best overall conference record (divisional and non divisional) and proceeding through other common non divisional teams based on their order of finish within their division; and
		  7) Best cumulative Conference winning percentage of non-divisional opponents (Note: If two teams' non-divisional opponents have the same cumulative record, then the two-team tiebreaker procedures apply. If four teams are tied and three teams' non-divisional opponents have the same cumulative record, the three-team tiebreaker procedures will be used beginning with 2.A.)
		  8) Coin flip of the tied teams with the team with the odd result being the representative (Example: If there are two teams with tails and one team with heads, the team with heads is the representative).
         */

        public string BreakTie(List<string> teamNames, Division division)
        {
            return null;
        }
    }
}
