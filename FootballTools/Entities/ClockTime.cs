using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FootballTools.Entities
{
    [DebuggerDisplay("{Minutes}:{Seconds}")]
    [DataContract]
    public class ClockTime
    {
        [DataMember(IsRequired = false, Name = "minutes")]
        public int Minutes = 0;

        [DataMember(IsRequired = false, Name = "seconds")]
        public int Seconds = 0;
    }
}
