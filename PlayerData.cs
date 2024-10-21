using System.Collections.Generic;

namespace Roll_stats
{
    /// <summary>
    /// Represents the data associated with a player, including all their roll data.
    /// </summary>
    public class PlayerData
    {
        /// <summary>
        /// The player's name.
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// A dictionary mapping base dice roll keys to their corresponding roll data.
        /// </summary>
        public Dictionary<string, RollData> RollDataByKey { get; set; } = new Dictionary<string, RollData>();
    }
}
