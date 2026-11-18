using NuclearWinter;
using Sirenix.OdinInspector;
using UnityEngine;


namespace NuclearWinter
{
    [CreateAssetMenu(fileName = "new Equipment Card", menuName = "Cards/Equipment")]
    [InlineEditor]
    public class CardItemSO : CardSO
    {
        [SerializeField] ItemType itemType;
        [SerializeField] private CardGameEvent loseCardEvent, showCardEvent;



        [ShowIf("IsHarvesting")]
        [SerializeField] private float increaseAmountFloat;
        [SerializeField] private int increaseAmountInt;
        [ShowIf("IsHarvesting")]
        [SerializeField] private FloatEvent increaseAttributeEventFloat;
        [SerializeField] private FloatEvent increaseAttributeEventInt;


        private bool IsHarvesting => itemType == ItemType.Harvesting;

        public override CardEventSO CardEvent => null;

        public ItemType ItemType => itemType;

        

        public override void UseCard(int amount = 0)
        {
            cardEvent?.Raise(this);

            if(IsHarvesting)
            {
                increaseAttributeEventFloat?.Raise(increaseAmountFloat);
                increaseAttributeEventInt?.Raise(increaseAmountInt);
            }
        }

        public void LoseCard() => loseCardEvent?.Raise(this);

        public override void ShowCard() => showCardEvent?.Raise(this);
    }

    public enum ItemType { Weapon, Harvesting, none };
}

