using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roll_stats
{
    public class RollData
    {
        public string DiceRollKey { get; set; }
        public List<RollResult> Results { get; set; } = new List<RollResult>();
    }

}
