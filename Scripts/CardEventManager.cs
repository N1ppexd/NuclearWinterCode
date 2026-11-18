using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NuclearWinter
{
    public class CardEventManager : MonoBehaviour
    {
        public static CardEventManager Instance { get; private set; }


        [SerializeField] private CardScreen cardScreen;
        [Space(10)]
        [SerializeField] private CardScreen eventCardScreen;

        [Space(5)]
        [Header("Card actions spawn under this transform")]
        [SerializeField] private Transform actionParent;

        [Space(5)]
        [Header("This is the prefab of the action button....")]
        [SerializeField]
        private GameObject actionButtonPrefab;


        [Space(10)]
        [Required]
        [SerializeField] private TextMeshProUGUI succesText, failureText;



        private List<GameObject> actions = new List<GameObject>();


        #region Singleton
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
        #endregion



        public void SetActionFeedback(ActionSO action)
        {

            succesText.text = action.SuccesText;
            failureText.text = action.FailText;
        }


        /// <summary>
        /// Show an event card <paramref name="card"/>
        /// </summary>
        /// <param name="card"></param>
        public void EventCardShow(CardSO card)
        {
            Debug.Log("SHOW THE FUKCING CARD! 2");
            if (card is CardEventSO)
            {
                Debug.Log("SHOW THE FUKCING CARD!");
                eventCardScreen.ShowCard(card);
            }
            else
            {
                Debug.LogError("Card is not an event card");
            }

            foreach(GameObject action in new List<GameObject>(actions)) Destroy(action);

            actions.Clear();

            foreach(ActionSO cardAction in card.CardEvent.CardActions)
            {
                GameObject actionButton = Instantiate(actionButtonPrefab, actionParent);

                actions.Add(actionButton);

                if(actionButton.TryGetComponent(out ActionButton actionButtonScript))
                {
                    actionButtonScript.SetAction(cardAction);
                }
                else
                {
                    Debug.LogError("actionButtonScript is null");
                }

                //get the script from the button and set the cardAction to it..
            }
        }
    }




    [System.Serializable]
    public class CardScreen
    {
        [SerializeField] private GameObject cardScreen;
        [SerializeField] private Image cardImage;
        [SerializeField] private TextMeshProUGUI cardName;
        [SerializeField] private TextMeshProUGUI cardDescription;


        /// <summary>
        /// Shows the fucking card
        /// </summary>
        /// <param name="card"></param>
        public void ShowCard(CardSO card)
        {
            cardScreen.SetActive(true);
            cardImage.sprite = card.CardSprite;
            cardName.text = card.CardName;
            cardDescription.text = card.CardDescription;
        }
    }
}
