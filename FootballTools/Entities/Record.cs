using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FootballTools.Entities
{
    [DebuggerDisplay("{Wins}-{Losses}")]
    public class Record
    {
        public int Wins { get; set; }
        public int Losses { get; set; }

        public Record()
        {

        }

        public Record(Record record)
        {
            Wins = record.Wins;
            Losses = record.Losses;
        }

        public Record(int wins, int losses)
        {
            Wins = wins;
            Losses = losses;
        }

        public override string ToString()
        {
            return $"{Wins}-{Losses}";
        }
    }
}
