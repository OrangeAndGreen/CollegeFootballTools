using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FootballTools.Entities
{
    public enum RecordType
    {
        Unnkown = 0,
        Overall = 1,
        Conference = 2,
        Division = 3
    }

    [DebuggerDisplay("TeamResult: {TeamName}: {OverallRecord}, {ConferenceRecord}, {DivisionRecord}")]
    public class TeamResult
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; }

        public Record OverallRecord { get; set; }
        public Record ConferenceRecord { get; set; }
        public Record DivisionRecord { get; set; }

        public Record this[RecordType recordType]
        {
            get
            {
                switch(recordType)
                {
                    case RecordType.Overall:
                        return OverallRecord;
                    case RecordType.Conference:
                        return ConferenceRecord;
                    case RecordType.Division:
                        return DivisionRecord;
                    default:
                        throw new Exception("Not ready for this case");
                }
            }
        }

        public TeamResult(int teamId, string teamName)
        {
            TeamId = teamId;
            TeamName = teamName;
            OverallRecord = new Record();
            ConferenceRecord = new Record();
            DivisionRecord = new Record();
        }

        public TeamResult(TeamResult result)
        {
            TeamId = result.TeamId;
            TeamName = result.TeamName;
            OverallRecord = new Record(result.OverallRecord);
            ConferenceRecord = new Record(result.ConferenceRecord);
            DivisionRecord = new Record(result.DivisionRecord);
        }



        public void AddWin(bool conferenceGame, bool divisionGame)
        {
            OverallRecord.Wins++;
            ConferenceRecord.Wins += conferenceGame ? 1 : 0;
            DivisionRecord.Wins += divisionGame ? 1 : 0;
        }

        public void RemoveWin(bool conferenceGame, bool divisionGame)
        {
            OverallRecord.Wins--;
            ConferenceRecord.Wins -= conferenceGame ? 1 : 0;
            DivisionRecord.Wins -= divisionGame ? 1 : 0;
        }

        public void AddLoss(bool conferenceGame, bool divisionGame)
        {
            OverallRecord.Losses++;
            ConferenceRecord.Losses += conferenceGame ? 1 : 0;
            DivisionRecord.Losses += divisionGame ? 1 : 0;
        }

        public void RemoveLoss(bool conferenceGame, bool divisionGame)
        {
            OverallRecord.Losses--;
            ConferenceRecord.Losses -= conferenceGame ? 1 : 0;
            DivisionRecord.Losses -= divisionGame ? 1 : 0;
        }

        public static List<TeamResult> FilterByTeams(List<TeamResult> teamResults, List<int> teamIds)
        {
            List<TeamResult> ret = new List<TeamResult>();

            foreach(TeamResult result in teamResults)
            {
                if(teamIds.Contains(result.TeamId))
                {
                    ret.Add(result);
                }
            }

            return ret;
        }

        public static List<int> GetWinningTeamIds(List<TeamResult> teamResults, RecordType recordType, List<int> teamIds = null)
        {
            List<int> ret = new List<int>();
            int maxWins = -1;
            int minLosses = 100;
            foreach (TeamResult result in teamResults)
            {
                //Optional filtering by teamIds
                if (teamIds == null || teamIds.Contains(result.TeamId))
                {
                    //Track most wins followed by least losses
                    int wins = result[recordType].Wins;
                    int losses = result[recordType].Losses;
                    if (wins > maxWins)
                    {
                        maxWins = wins;
                        minLosses = losses;

                        ret.Clear();
                        ret.Add(result.TeamId);
                    }
                    else if(wins == maxWins)
                    {
                        if(losses < minLosses)
                        {
                            minLosses = losses;

                            ret.Clear();
                            ret.Add(result.TeamId);
                        }
                        else if(losses == minLosses)
                        {
                            ret.Add(result.TeamId);
                        }
                        
                    }
                }
            }

            return ret;
        }
    }
}
