//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Runtime.Serialization;

//namespace FootballTools.NCAA
//{
//    [DataContract]
//    public class JSONGame
//    {
//        [DataMember(IsRequired=false)]
//        public int id = -1;
//        [DataMember(IsRequired = false)]
//        public string conference = null;
//        [DataMember(IsRequired = false)]
//        public string gameState = null;
//        [DataMember(IsRequired = false)]
//        public string startDate = null;
//        [DataMember(IsRequired = false)]
//        public string startDateDisplay = null;
//        [DataMember(IsRequired = false)]
//        public string startTime = null;
//        [DataMember(IsRequired = false)]
//        public string startTimeEpoch = null;
//        [DataMember(IsRequired = false)]
//        public string currentPeriod = null;
//        [DataMember(IsRequired = false)]
//        public string finalMessage = null;
//        [DataMember(IsRequired = false)]
//        public string gameStatus = null;
//        [DataMember(IsRequired = false)]
//        public string periodStatus = null;
//        [DataMember(IsRequired = false)]
//        public string downToGo = null;
//        [DataMember(IsRequired = false)]
//        public string timeclock = null;
//        [DataMember(IsRequired = false)]
//        public string location = null;
//        [DataMember(IsRequired = false)]
//        public string contestName = null;
//        //[DataMember(IsRequired=false)]
//        //public string url = null;
//        [DataMember(IsRequired = false)]
//        public string highlightsUrl = null;
//        [DataMember(IsRequired = false)]
//        public string liveAudioUrl = null;
//        [DataMember(IsRequired = false)]
//        public string gameCenterUrl = null;
//        //[DataMember(IsRequired=false)]
//        //string champInfo = null;
//        [DataMember(IsRequired = false)]
//        public string[] videos = null;
//        [DataMember(IsRequired = false)]
//        public string[] scoreBreakdown = null;
//        [DataMember(IsRequired = false)]
//        public JSONTeam home = null;
//        [DataMember(IsRequired = false)]
//        public JSONTeam away = null;
//        [DataMember(IsRequired = false)]
//        public string[] status = null;
//        [DataMember(IsRequired = false)]
//        public string[] alerts = null;
//        [DataMember(IsRequired = false)]
//        public int upset = 0;
//        [DataMember(IsRequired = false)]
//        public int redzone = 0;
//    }
//}
