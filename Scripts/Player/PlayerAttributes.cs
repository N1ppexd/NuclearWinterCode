
namespace NuclearWinter
{
    public class PlayerAttributes
    {
        private PlayerStat medicine, warmth, food;
        public PlayerAttributes(PlayerStat medicine, PlayerStat warmth, PlayerStat food)
        {
            this.medicine = medicine;
            this.warmth = warmth;
            this.food = food;
        }


        public PlayerStat GetStat(StatsSO statsSO)
        {
            if (statsSO == medicine.StatSettings) { return medicine; }
            else if (statsSO == food.StatSettings) { return food; }
            else if (statsSO == warmth.StatSettings) { return warmth; }

            else return null;
        }


        public PlayerStat Medicine => medicine;
        public PlayerStat Warmth=> warmth;
        public PlayerStat Food => food;
    }


    public class PlayerStat
    {
        private int amount;
        private StatsSO statSettings;
        private float multiplier;

        private int goal;

        #region getters
        public int StatAmount => amount;
        public float Multiplier => multiplier;

        public int Goal => goal;

        public StatsSO StatSettings => statSettings;
        #endregion

        public PlayerStat(StatsSO statsSO, int amount)
        {
            this.amount = amount;
            this.multiplier = 1f;
            this.statSettings = statsSO;
        }

        public void SetMultiplier(float multiplier) => this.multiplier = multiplier;

        /// <summary>
        /// Sets the attribute amount to this number
        /// </summary>
        /// <param name="amount"></param>
        public void SetAmount(int amount) => this.amount = amount;

        /// <summary>
        /// Adds the amount of shit.
        ///  if negative, substract
        /// </summary>
        /// <param name="amount"></param>
        public void AddAmount(int amount)
        {
            int value = amount + (int)(amount * multiplier);
            this.amount = value < 0 ? 0 : value;
        }

        /// <summary>
        /// Set the goal which needs to be achieved during the round
        /// </summary>
        /// <param name="goal"></param>
        public void SetGoal(int goal) => this.goal = goal;
    }
}
