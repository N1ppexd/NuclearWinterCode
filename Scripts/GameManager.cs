using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace NuclearWinter
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }


        [MinValue(0)]
        [SerializeField] private int maxInventorySlots;

        [Required]
        [SerializeField] private CycleSettingsSO cycleSettingsSO;


        private int maxRounds;
        private Inventory inventory;
        private PlayerAttributes playerStats;
        private int round;

        [Required]
        [SerializeField] private StatsSO healthStatSO, warmthStatSO, foodStatSO;

        [Required]
        [SerializeField] private StatEvent statEvent;


        //true when the target goal is reached
        private bool hasReachedTarget;


        [Space(10)]
        [Required]
        [SerializeField]
        private GameEvent targetReachedEvent;
        [Required]
        [SerializeField]
        private GameEvent gameFailedEvent;

        [Required]
        [SerializeField]
        private GameEvent gameWonEvent;

        public int Round
        {
            get => round; 
            
            set
            {
                if(value < 1) value = 1;
                if(value > maxRounds) value = maxRounds;

                round = value;
            }
        }


        public Inventory Inventory => inventory;
        public PlayerAttributes PlayerStats => playerStats;


        private CustomTile originalBaseTile;
        private readonly List<CustomTile> baseTiles  = new();




        private void OnEnable()
        {
            GameEvents.onBaseTilesSet += SetBaseTile;
            GameEvents.onBaseTileAdded += AddBaseTiles;
            GameEvents.onResourcesUsed += ChangeStat;
        }

        private void OnDisable()
        {
            GameEvents.onBaseTilesSet -= SetBaseTile;
            GameEvents.onBaseTileAdded -= AddBaseTiles;
            GameEvents.onResourcesUsed -= ChangeStat;
        }



        private void ChangeStat(StatsSO stat, int amount)
        {
            playerStats.GetStat(stat).AddAmount(amount);

            Debug.Log("stat = " + stat.StatName);


            if(playerStats.Warmth.StatAmount >= playerStats.Warmth.Goal
                && playerStats.Food.StatAmount >= playerStats.Food.Goal
                && playerStats.Medicine.StatAmount >= playerStats.Medicine.Goal)
            {
                hasReachedTarget = true;
                targetReachedEvent?.Raise();
            }
        }


        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else Destroy(gameObject);


            if(cycleSettingsSO == null)
            {
                Debug.LogError("cycle settings so is null wtf");
                return;
            }

            //Set the number of rounds and the current round....
            SetCycles(cycleSettingsSO);

            inventory = new Inventory(maxInventorySlots);
            PlayerStat startMedicine = new(healthStatSO, healthStatSO.StartAmount);
            PlayerStat startWarmth = new(warmthStatSO, warmthStatSO.StartAmount);
            PlayerStat startFood = new(foodStatSO, foodStatSO.StartAmount);

            playerStats = new PlayerAttributes(startMedicine, startWarmth, startFood);

            //Set the current round stats...
            SetCurrentCycle(playerStats, cycleSettingsSO, round);

            GameEvents.InstantiateStatistics(playerStats);

        }


        private void SetCycles(CycleSettingsSO cycleSettingsSO)
        {
            maxRounds = cycleSettingsSO.Cycles.Length;
            round = 1;
        }


        /// <summary>
        /// Set the attributes to the current cycle....
        /// </summary>
        /// <param name="playerStats"></param>
        private void SetCurrentCycle(PlayerAttributes playerStats, CycleSettingsSO cycleSettingsSO, int round)
        {
            if(playerStats == null)
            {
                Debug.LogError("PlayerAttributes can't be null");
                return;
            }

            playerStats.Warmth.AddAmount(-playerStats.Warmth.Goal);
            playerStats.Food.AddAmount(-playerStats.Food.Goal);
            playerStats.Medicine.AddAmount(-playerStats.Medicine.Goal);

            playerStats.Warmth.SetGoal(cycleSettingsSO.Cycles[round].WarthGoal);
            playerStats.Food.SetGoal(cycleSettingsSO.Cycles[round].FoodGoal);
            playerStats.Warmth.SetGoal(cycleSettingsSO.Cycles[round].MedicineGoal);


            foreach (var card in inventory.Cards)
                card.card.LoseCard();

            foreach (var card in cycleSettingsSO.Cycles[round].StartingCards)
            {
                inventory.AddCard(card);
            }


            GameUI.Instance?.NewRound(playerStats);
        }



        #region terminal commands
        [TerminalCommand("RestartRound", "Restarts the round")]
        public void RestartRound()
        {
            SetCurrentCycle(playerStats, cycleSettingsSO, round);
            GameEvents.RestartRound();
        }


        [TerminalCommand("SetFood", "SetFood(int number) sets the amount of food to that value")]
        public void SetFood(int amount) => playerStats.Food.SetAmount(amount);

        [TerminalCommand("SetMedicine", "SetMedicine(int number) sets the amount of medicine to that value")]
        public void SetMedicine(int amount) => playerStats.Medicine.SetAmount(amount);


        [TerminalCommand("SetWarmth", "SetWarmth(int number) sets the amount of warmth to that value")]
        public void SetWarmth(int amount) => playerStats.Warmth.SetAmount(amount);

        #endregion


        public void SetMedicineMultiplier(int multiplier) => playerStats.Medicine.SetMultiplier(multiplier);



        /// <summary>
        /// Start a new round
        /// </summary>
        public void NewRound()
        {
            if (!hasReachedTarget)
            {
                gameFailedEvent?.Raise();

                return;
            }

            

            Round++;


            if(Round >= cycleSettingsSO.Cycles.Length)
            {
                gameWonEvent?.Raise();
                return;
            }


            RestartRound();

            List<CustomTile> newCustomTiles = HexagonOperations.GetTilesInRange(CustomGrid.CurrentGrid, originalBaseTile, Round);

            GameEvents.AddBaseTiles(newCustomTiles.ToArray());

        }


        /// <summary>
        /// Set the origin base tile to <paramref name="baseTile"/>
        /// </summary>
        /// <param name="baseTile"></param>
        public void SetBaseTile(CustomTile baseTile)
        {
            Debug.Log("set base tile");
            try
            {
                originalBaseTile = baseTile;
                Player.Instance.visitedTiles.Add(originalBaseTile);
            }
            catch
            {
                Debug.LogError("wtf?");
            }
            
        }


        /// <summary>
        /// Add the fucking base tiles
        /// </summary>
        /// <param name="baseTiles"></param>
        public void AddBaseTiles(CustomTile[] baseTiles)
        {
            Debug.Log("add base tiles");
            foreach (CustomTile tile in baseTiles)
            {
                if(!this.baseTiles.Contains(tile))
                    this.baseTiles.Add(tile);
            }


            foreach(CustomTile tile in this.baseTiles)
            {
                try
                {
                    Destroy(tile.HexTile.gameObject);

                    AreaGenerator.SetTile(originalBaseTile.TileSO, tile, PathGenerator.Instance, CustomGrid.CurrentGrid, out GameObject newBaseTile);

                    //update base event
                    GameEvents.UpdateBase();
                }
                catch
                {
                    Debug.LogError("Something is wrong! I dont know what. Look it up idiot");
                }
            }
        }
    }
}
