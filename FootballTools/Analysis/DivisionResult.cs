using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FootballTools.Analysis
{
    public class DivisionResult
    {
        public int WinnerId { get; set; }
        public List<int> DivisionFinalistIds { get; set; }
        public double Milliseconds { get; set; }
    }
}
