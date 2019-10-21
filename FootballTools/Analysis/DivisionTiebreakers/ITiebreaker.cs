using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FootballTools.Entities;

namespace FootballTools.Analysis.DivisionTiebreakers
{
    public interface ITiebreaker
    {
        int BreakTie(GameList games, List<int> winners, List<TeamResult> teamResults, List<int> teamIds, Division division);
    }
}
