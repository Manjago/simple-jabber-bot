using System;
using System.Collections.Generic;
using System.Text;

namespace Temnenkov.SJB.LogBase.Business
{
    internal class Chain
    {
        private List<PersistentLine> Items { get; set; }
        private bool IsDelay { get; set; }

        private Chain()
        {
            Items = new List<PersistentLine>();
        }

        internal static List<Chain> ToChains(List<PersistentLine> Lines)
        {
            var result = new List<Chain>();

            foreach (var line in Lines)
            {
                if ((result.Count == 0) || (line.IsDelayed != result[result.Count - 1].IsDelay))
                {
                    var newChain = new Chain { IsDelay = line.IsDelayed };
                    newChain.Items.Add(line);
                    result.Add(newChain);
                }
                else
                {
                    result[result.Count - 1].Items.Add(line);
                }

            }

            return result;
        }


        internal bool IsBad
        {
            get
            {
                if (!IsDelay) return false;

                PersistentLineDataLayer dal = new PersistentLineDataLayer();

                // по любому запасемся праймерами
                var primaryies = dal.GetSameLines(Items[0]);

                if (primaryies.Count == 0) return false;

                foreach(var item in primaryies)
                    if (item.IsSameChain(this)) return true;

                return false;
            }
        }

        internal IEnumerable<PersistentLine> Lines()
        {
            return Items;
        }

        private bool IsSameChain(Chain mold)
        {
            throw new NotImplementedException();

        }
    }
}
