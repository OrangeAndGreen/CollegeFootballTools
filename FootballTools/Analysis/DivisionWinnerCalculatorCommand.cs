using FootballTools.Entities;
using System;
using System.Collections.Generic;

namespace FootballTools.Analysis
{
    public enum DivisionWinnerCalculatorCommandType
    {
        Unknown = 0,
        SetData = 1,
        Judge = 2,
        Signal = 3,
        Shutdown = 4
    }

    public class DivisionWinnerCalculatorCommand
    {
        public DivisionWinnerCalculatorCommandType CommandType { get; set; }
        public League ActiveLeague { get; set; }
        public Division ActiveDivision { get; set; }
        public List<Game> Games { get; set; }
        public List<string> Winners { get; set; }
        public Action<string, List<string>> Callback { get; set; }

        public DivisionWinnerCalculatorCommand(DivisionWinnerCalculatorCommandType commandType, League league, Division division, List<Game> games, List<string> winners, Action<string, List<string>> callback)
        {
            CommandType = commandType;
            ActiveLeague = league;
            ActiveDivision = division;
            Games = games;
            Winners = winners;
            Callback = callback;
        }

        public static DivisionWinnerCalculatorCommand CreateSetDataCommand(League league, Division division, List<Game> games, Action<string, List<string>> callback)
        {
            return new DivisionWinnerCalculatorCommand(DivisionWinnerCalculatorCommandType.SetData, league, division, games, null, callback);
        }

        public static DivisionWinnerCalculatorCommand CreateJudgeCommand(List<string> winners)
        {
            return new DivisionWinnerCalculatorCommand(DivisionWinnerCalculatorCommandType.Judge, null, null, null, winners, null);
        }

        public static DivisionWinnerCalculatorCommand CreateSignalCommand(Action<string, List<string>> callback)
        {
            return new DivisionWinnerCalculatorCommand(DivisionWinnerCalculatorCommandType.Signal, null, null, null, null, callback);
        }

        public static DivisionWinnerCalculatorCommand CreateShutdownCommand()
        {
            return new DivisionWinnerCalculatorCommand(DivisionWinnerCalculatorCommandType.Shutdown, null, null, null, null, null);
        }
    }
}
