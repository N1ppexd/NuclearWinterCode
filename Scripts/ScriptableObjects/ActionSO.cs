using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NuclearWinter
{
    [CreateAssetMenu(fileName = "ActionSO", menuName = "ScriptableObject/ActionSO")]
    [InlineEditor]
    public class ActionSO : ScriptableObject
    {
        [PropertySpace(10)]
        [SerializeField] string actionName;
        [TextArea(15, 10)]
        [SerializeField] string actionDescription;


        //either gain or remove resource based on wether this is negative or positive
        [Space(10)]
        [BoxGroup("Settings")]
        [SerializeField] int succesResourceChange, failResourceChange;
        [Space(10)]
        [BoxGroup("Settings")]
        [SerializeField] StatsSO succesResources, failResources;

        [BoxGroup("Settings")]
        [Space(20)]
        [SerializeField] private CardActionEvent failEvent;
        [BoxGroup("Settings")]
        [SerializeField] private CardActionEvent successEvent;

        

        [BoxGroup("Settings")]
        [Space(10)]
        [Header("Chance of succes")]
        [Range(0, 100)]
        [SerializeField] private float succesChance;

        [Space(10)]
        [BoxGroup("Settings")]
        [SerializeField] private string succesText;
        [BoxGroup("Settings")]
        [SerializeField] private string failText;

        [BoxGroup("Settings")]
        [Space(20)]
        [InfoBox("When enabled, an item is used")]
        [SerializeField] private bool isItemAction;
        [BoxGroup("Settings")]
        [Space(5)]
        [ShowIf("isItemAction")]
        [SerializeField] private ItemType itemType;


        public string ActionName => actionName;
        public string ActionDescription => actionDescription;
        public int SuccesResourceChange => succesResourceChange;
        public int FailResourceChange => failResourceChange;


        public string SuccesText => succesText;
        public string FailText => failText;

        //the changes of succes in percentages
        public float SuccesChance => succesChance;
        public StatsSO SuccesResources => succesResources;
        public StatsSO FailResources => failResources;



        public virtual void UseAction(Inventory inventory)
        {

            bool canSucceed = true;

            if(isItemAction)
            {
                bool hasItem = false;

                foreach(var item in inventory.Cards)
                {
                    if(item.card.ItemType == itemType)
                    {
                        hasItem = true;
                        item.card.LoseCard();
                        break;
                    }
                }

                if (!hasItem)
                {
                    canSucceed = false;
                }
            }
            if (!canSucceed)
            {
                FailAction();
                return;
            }

            if(CheckSucces())
            {
                SucceedAction();
            }
            else
            {
                FailAction();
            }
        }


        private void FailAction()
        {

            failResources.ChangeAmount(FailResourceChange);
            failEvent?.Raise(this);
            //GameEvents.UseResources(failResources, FailResourceChange);
        }

        private void SucceedAction()
        {

            succesResources.ChangeAmount(SuccesResourceChange);
            successEvent?.Raise(this);
            //.UseResources(succesResources, SuccesResourceChange);
        }


        private bool CheckSucces() => Random.Range(0, 100f) <= succesChance;
    }
}
