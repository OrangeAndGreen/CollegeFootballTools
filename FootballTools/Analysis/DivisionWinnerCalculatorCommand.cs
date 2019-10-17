using FootballTools.Entities;
using System;
using System.Collections.Generic;

namespace FootballTools.Analysis
{
    public class DivisionWinnerCalculatorCommand
    {
        public List<Game> Games { get; set; }
        public List<bool> HomeWinners { get; set; }
        public Division ActiveDivision { get; set; }
        public Action<string, List<string>> Callback { get; set; }

        public DivisionWinnerCalculatorCommand(Division division, List<Game> games, List<bool> homeWinners, Action<string, List<string>> callback)
        {
            Games = games;
            HomeWinners = homeWinners;
            ActiveDivision = division;
            Callback = callback;
        }
    }
}
