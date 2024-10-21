namespace Roll_stats
{
    /// <summary>
    /// Represents a single roll result, including the flavor (reason), die roll value, modifier value, and total value.
    /// </summary>
    public class RollResult
    {
        /// <summary>
        /// The flavor or reason for the roll (e.g., "Attack Roll").
        /// </summary>
        public string Flavor { get; set; }

        /// <summary>
        /// The sum of die roll values.
        /// </summary>
        public double DieRollValue { get; set; }

        /// <summary>
        /// The sum of modifier values.
        /// </summary>
        public double ModifierValue { get; set; }

        /// <summary>
        /// The total value of the roll (die rolls + modifiers).
        /// </summary>
        public double TotalValue { get; set; }

        /// <summary>
        /// The full formula of the roll, including modifiers.
        /// </summary>
        public string FullFormula { get; set; }
    }
}
