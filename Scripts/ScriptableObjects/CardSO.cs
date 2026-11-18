using UnityEngine;
using Sirenix.OdinInspector;

namespace NuclearWinter
{
    public abstract class CardSO : ScriptableObject
    {
        [PreviewField(100)]
        [SerializeField] private Sprite cardSprite;
        [Space(10)]
        [SerializeField] private string cardName;

        [Space(10)]
        [TextArea(15, 20)]
        [SerializeField] private string cardDescription;


        [Space(20)]
        [SerializeField] protected CardGameEvent cardEvent;

        [Space(10)]
        [SerializeField] private AudioClip eventAudio;

        public Sprite CardSprite => cardSprite;
        public string CardName => cardName;
        public string CardDescription => cardDescription;


        public AudioClip EventAudio => eventAudio;

        /// <summary>
        /// Use the card... <paramref name="amount"/> is the amount of times the card is used if it's over 0
        /// It's primarly used in resource cards..
        /// </summary>
        /// <param name="amount"></param>
        public abstract void UseCard(int amount = 0);

        public virtual void ShowCard() { }


        public abstract CardEventSO CardEvent { get; }
    }
}
