using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FootballTools.Entities
{
    [DebuggerDisplay("{mPlays.Count} plays")]
    public class PlayList : IEnumerable<Play>
    {
        private List<Play> mPlays { get; set; }

        public PlayList()
        {
            mPlays = new List<Play>();
        }

        public PlayList(PlayList plays)
            : this()
        {
            foreach (Play play in plays)
            {
                Add(play);
            }
        }

        public Play this[int index] => mPlays[index];

        public int Count => mPlays.Count;

        public void Add(Play play)
        {
            mPlays.Add(play);
        }

        public void AddRange(IEnumerable<Play> games)
        {
            foreach (Play game in games)
            {
                Add(game);
            }
        }

        public bool Contains(Play game)
        {
            return mPlays.Contains(game);
        }

        public IEnumerator<Play> GetEnumerator()
        {
            return ((IEnumerable<Play>)mPlays).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Play>)mPlays).GetEnumerator();
        }
    }
}
