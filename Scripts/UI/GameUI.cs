using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.SceneManagement;

namespace NuclearWinter
{
    public class GameUI : MonoBehaviour
    {
        public static GameUI Instance { get; private set; }


        [Required]
        [SerializeField] private TextMeshProUGUI medicineText, warmthText, foodText;

        private PlayerAttributes playerAttributes;


        [Space(10)]
        [Required]
        [SerializeField]
        private MMF_Player medicineFeedback, warmthFeedback, foodFeedback;



        [Space(10)]
        [Required]
        [SerializeField]
        private StatsSO medicine;
        [Required]
        [SerializeField]
        private StatsSO warmth;
        [Required]
        [SerializeField]
        private StatsSO food;


        [Space(10)]
        [Required]
        [SerializeField]
        private RoundTargetPanel newRoundPanel;


        [Space(10)]
        [Required]
        [SerializeField]
        private string mainMenuSceneName;

        [SerializeField]
        private GameObject terrainTutorialPanel;

        //used to track the amount of times the player has gone to terrain to show the tutorial screen only on the first time
        private int timesWentToTerrain;



        private PlayerInput playerInput;


        [SerializeField] private GameObject pauseScreen;



        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
                return;
            }
        }

        private void OnEnable()
        {
            GameEvents.onStatisticsInstantiated += SetStatistics;
            GameEvents.onResourcesUsed += ChangeStat;
            GameEvents.onPlayerGoToTerrain += GoToTerrain;

            playerInput = new PlayerInput();

            playerInput.Enable();


            playerInput.UI.Cancel.performed += _ => pauseScreen.SetActive(!pauseScreen.activeSelf);

        }

        private void OnDisable()
        {
            GameEvents.onStatisticsInstantiated -= SetStatistics;
            GameEvents.onResourcesUsed -= ChangeStat;
            GameEvents.onPlayerGoToTerrain -= GoToTerrain;

            playerInput.UI.Cancel.performed -= _ => pauseScreen.SetActive(!pauseScreen.activeSelf);

            playerInput.Disable();
        }


        public void ChangeMedicine(int amount) => GameEvents.UseResources(medicine, amount);
        public void ChangeFood(int amount) => GameEvents.UseResources(food, amount);
        public void ChangeWarmth(int amount) => GameEvents.UseResources(warmth, amount);


        private void GoToTerrain()
        {/*
            if(timesWentToTerrain > 0)
            {
                return;
            }

            timesWentToTerrain++;
            terrainTutorialPanel.SetActive(true);*/
        }



        public void SetStatistics(PlayerAttributes playerAttributes)
        {
            this.playerAttributes = playerAttributes;

            medicineText.text = playerAttributes.Medicine.StatAmount + " / " + playerAttributes.Medicine.Goal;
            warmthText.text = playerAttributes.Warmth.StatAmount + " / " + playerAttributes.Warmth.Goal;
            foodText.text = playerAttributes.Food.StatAmount + " / " + playerAttributes.Food.Goal;
        }

        private void ChangeStat(StatsSO stat, int amount)
        {
            if (playerAttributes == null) return;

            medicineText.text = playerAttributes.Medicine.StatAmount + " / " + playerAttributes.Medicine.Goal;
            warmthText.text = playerAttributes.Warmth.StatAmount + " / " + playerAttributes.Warmth.Goal;
            foodText.text = playerAttributes.Food.StatAmount + " / " + playerAttributes.Food.Goal;

            Debug.Log("warmth = " + playerAttributes.Warmth.StatAmount);


            if (playerAttributes.GetStat(stat) == playerAttributes.Medicine)
            {
                medicineFeedback.PlayFeedbacks();
            }
            if (playerAttributes.GetStat(stat) == playerAttributes.Warmth)
            {
                warmthFeedback.PlayFeedbacks();
            }
            if (playerAttributes.GetStat(stat) == playerAttributes.Food)
            {
                foodFeedback.PlayFeedbacks();
            }
        }


        public void NewRound(PlayerAttributes playerAttributes)
        {
            SetStatistics(playerAttributes);

            newRoundPanel.gameObject.SetActive(true);

            newRoundPanel.SetTargets(playerAttributes);
        }


        [TerminalCommand("RestartGame", "Restarts the game")]
        public void RestartGame() => MMSceneLoadingManager.LoadScene(SceneManager.GetActiveScene().name, "LoadingScreen");


        public void QuitGame() => Application.Quit();


        public void MainMenu() => MMSceneLoadingManager.LoadScene(mainMenuSceneName, "LoadingScreen");
    }
}
