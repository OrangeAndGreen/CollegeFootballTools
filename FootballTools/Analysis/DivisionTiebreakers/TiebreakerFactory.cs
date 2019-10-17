using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FootballTools.Entities;

namespace FootballTools.Analysis.DivisionTiebreakers
{
    class TiebreakerFactory
    {
        private static TiebreakerFactory mInstance;
        private readonly Dictionary<string, ITiebreaker> mTiebreakers;

        public static string BreakTie(List<string> teamNames, League league)
        {
            if (mInstance == null)
            {
                mInstance = new TiebreakerFactory();
            }

            return mInstance.BreakTieInternal(teamNames, league);
        }

        private TiebreakerFactory()
        {
            mTiebreakers = new Dictionary<string, ITiebreaker>
            {
                ["ACC"] = new AccTiebreaker(),
                ["SEC"] = new SecTiebreaker(),
                ["Mountain West"] = new MwcTiebreaker(),
                ["Pac-12"] = new Pac12Tiebreaker(),
                ["Mid-American"] = new MacTiebreaker(),
                ["Conference USA"] = new CUsaTiebreaker(),
                ["Sun Belt"] = new SunBeltTiebreaker(),
                ["Big Ten"] = new Big10Tiebreaker(),
                ["Big 12"] = new Big12Tiebreaker()
            };
        }

        private string BreakTieInternal(List<string> teamNames, League league)
        {
            foreach (Conference conference in league.Conferences)
            {
                foreach (Division division in conference.Divisions)
                {
                    if (division.FindTeam(teamNames[0]) != null)
                    {
                        return mTiebreakers[conference.Name].BreakTie(teamNames, league);
                    }
                }
            }

            return null;
        }
    }
}
