using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roll_stats
{
    public class PlayerData
    {
        public string PlayerName { get; set; }
        public Dictionary<string, RollData> RollDataByKey { get; set; } = new Dictionary<string, RollData>();
    }

}
