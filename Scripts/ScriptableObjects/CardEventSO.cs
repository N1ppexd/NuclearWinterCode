using Sirenix.OdinInspector;
using UnityEngine;

namespace NuclearWinter
{
    [CreateAssetMenu(fileName = "new Event", menuName = "Cards/Event")]
    [InlineEditor]
    public class CardEventSO : CardSO
    {

        [SerializeField] private ActionSO[] actions;

        public ActionSO[] CardActions => actions;


        public override CardEventSO CardEvent => this;

        public override void UseCard(int amount = 0)
        {
            Debug.Log("Event card tried to use");
            cardEvent.Raise(this);
        }
    }
}

