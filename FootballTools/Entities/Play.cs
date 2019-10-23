using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FootballTools.Entities
{
    [DataContract]
    public class Play
    {
        [DataMember(IsRequired = false, Name = "id")]
        public long Id = 0;
        [DataMember(IsRequired = false, Name = "offense")]
        public string Offense;
        [DataMember(IsRequired = false, Name = "offense_conference")]
        public string OffenseConference;
        [DataMember(IsRequired = false, Name = "defense")]
        public string Defense;
        [DataMember(IsRequired = false, Name = "defense_conference")]
        public string DefenseConference;
        [DataMember(IsRequired = false, Name = "home")]
        public string Home;
        [DataMember(IsRequired = false, Name = "away")]
        public string Away;
        [DataMember(IsRequired = false, Name = "offense_score")]
        public int OffenseScore;
        [DataMember(IsRequired = false, Name = "defense_score")]
        public int DefenseScore;
        [DataMember(IsRequired = false, Name = "drive_id")]
        public long DriveId;
        [DataMember(IsRequired = false, Name = "period")]
        public int Period;
        [DataMember(IsRequired = false, Name = "clock")]
        public ClockTime Clock;
        [DataMember(IsRequired = false, Name = "yard_line")]
        public int YardLine;
        [DataMember(IsRequired = false, Name = "down")]
        public int Down;
        [DataMember(IsRequired = false, Name = "distance")]
        public int Distance;
        [DataMember(IsRequired = false, Name = "yards_gained")]
        public int YardsGained;
        [DataMember(IsRequired = false, Name = "play_type")]
        public string PlayType;
        [DataMember(IsRequired = false, Name = "play_text")]
        public string PlayText;
    }
}
