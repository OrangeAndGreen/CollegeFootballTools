using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FootballTools.Entities;

namespace FootballTools.Analysis.DivisionTiebreakers
{
    class CUsaTiebreaker : ITiebreaker
    {
        /*
          1) Highest regular season winning percentage based on overall Conference USA play.
		  2) If tied, head-to-head between tied teams.
		  3) If still tied, highest winning percentage within division.
		  4) If still tied, compare records against divisional opponents in descending order of finish.
		  5) If still tied, compare records with common cross-divisional opponents.
		  6) If still tied, compare records against cross-divisional opponents in descending order of finish.
		  7) If still tied, team with highest (College Football Playoff) ranking.
		  8) If still tied, the representative will be the team that has not participated in the championship game most recently.
         */

        public int BreakTie(GameList games, List<int> winners, List<TeamResult> teamResults, List<int> teamIds, Division division)
        {
            return -1;
        }
    }
}
