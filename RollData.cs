using System.Collections.Generic;

namespace Roll_stats
{
    /// <summary>
    /// Represents the roll data for a specific base dice roll key, including all roll results.
    /// </summary>
    public class RollData
    {
        /// <summary>
        /// The base dice roll key (e.g., "1d8", "2d6").
        /// </summary>
        public string DiceRollKey { get; set; }

        /// <summary>
        /// A list of roll results for this dice roll key.
        /// </summary>
        public List<RollResult> Results { get; set; } = new List<RollResult>();
    }
}
