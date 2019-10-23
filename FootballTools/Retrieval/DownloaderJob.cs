using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FootballTools.Retrieval
{
    public class DownloaderJob
    {
        public DownloadType Type { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
    }
}
