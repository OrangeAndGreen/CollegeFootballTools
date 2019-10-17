using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FootballTools.Entities;

namespace FootballTools.Analysis.DivisionTiebreakers
{
    class MacTiebreaker : ITiebreaker
    {
        /*
          1) Head-to-head record among tied teams
		  2) Record of tied teams within the division (versus rank order, highest to lowest, of division teams)
		  3) Comparison of conference winning percentage of cross-over opponents of tied teams
         */

        public string BreakTie(List<string> teamNames, League league)
        {
            return null;
        }
    }
}
