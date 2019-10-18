using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FootballTools.Entities;

namespace FootballTools.Analysis.DivisionTiebreakers
{
    public interface ITiebreaker
    {
        string BreakTie(List<Game> games, List<string> winners, List<string> teamNames, Division division);
    }
}
