using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FootballTools.Entities;

namespace FootballTools.Analysis.DivisionTiebreakers
{
    class Big12Tiebreaker: ITiebreaker

    {
    /*
      1) The conference records of the three or more teams will be compared against each other.
      2) The conference records of the three or more teams will be compared against the next highest placed team(s) in the conference.
      3) The team with the worst scoring differential among the tied teams is out of the running.
      4) If the tie cannot be broken by steps 1-3, they draw numbers.
     */

    public string BreakTie(List<Game> games, List<string> winners, List<string> teamNames, Division division)
    {
        return null;
    }
    }
}
