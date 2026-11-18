using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NuclearWinter
{
    public class ActionButton : MonoBehaviour
    {

        private ActionSO action;

        [SerializeField] private TextMeshProUGUI actionName;

        [SerializeField] private TextMeshProUGUI succesChanceText, failureChanceText;
        [SerializeField] private TextMeshProUGUI succesChangeText, failureChangeText;

        [Space(10)]
        [SerializeField] private Image actionSuccesResource, actionFailureResource;


        [Space(10)]
        [Required]
        [SerializeField]
        private TextMeshProUGUI descriptionText;


        public void SetAction(ActionSO action)
        {
            this.action = action;
            actionName.text = action.ActionName;

            actionSuccesResource.sprite = action.SuccesResources.StatSprite;
            actionFailureResource.sprite = action.FailResources.StatSprite;

            succesChanceText.text = action.SuccesChance.ToString() + " %";

            failureChanceText.text = (100f - action.SuccesChance).ToString() + " %";


            failureChangeText.text = action.FailResourceChange > 0 ? "+" + action.FailResourceChange : action.FailResourceChange.ToString();
            succesChangeText.text = action.SuccesResourceChange > 0 ? "+" + action.SuccesResourceChange : action.SuccesResourceChange.ToString();


            descriptionText.text = action.ActionDescription;
            
        }

        public void PerformAction()
        {
            if (action == null)
            {
                Debug.LogError("action is null");
                return;
            }

            if(Player.Instance == null)
            {
                Debug.LogError("player is null");
                return;
            }

            action.UseAction(GameManager.Instance.Inventory);

            Destroy(gameObject);
        }
    }
}
