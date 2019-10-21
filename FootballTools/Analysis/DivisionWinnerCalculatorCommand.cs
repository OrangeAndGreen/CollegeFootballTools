using FootballTools.Entities;
using System;
using System.Collections.Generic;

namespace FootballTools.Analysis
{
    public enum DivisionWinnerCalculatorCommandType
    {
        Unknown = 0,
        Judge = 1,
        Finish = 2
    }

    public class DivisionWinnerCalculatorCommand
    {
        public DivisionWinnerCalculatorCommandType CommandType { get; set; }
        public List<int> WinnerIds { get; set; }
        public List<TeamResult> TeamResults { get; set; }

        public DivisionWinnerCalculatorCommand(DivisionWinnerCalculatorCommandType commandType, List<int> winnerIds, List<TeamResult> teamResults)
        {
            CommandType = commandType;
            TeamResults = teamResults;
            WinnerIds = winnerIds;
        }

        public static DivisionWinnerCalculatorCommand CreateJudgeCommand(List<int> winnerIds, List<TeamResult> teamResults)
        {
            return new DivisionWinnerCalculatorCommand(DivisionWinnerCalculatorCommandType.Judge, winnerIds, teamResults);
        }

        public static DivisionWinnerCalculatorCommand CreateFinishSessionCommand()
        {
            return new DivisionWinnerCalculatorCommand(DivisionWinnerCalculatorCommandType.Finish, null, null);
        }
    }
}
