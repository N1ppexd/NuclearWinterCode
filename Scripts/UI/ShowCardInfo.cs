using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

namespace NuclearWinter
{
    public class ShowCardInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI cardName;
        [SerializeField]
        private TextMeshProUGUI cardDescription;
        [SerializeField]
        private Image cardImage;


        public void ShowCard(CardSO card)
        {
            cardName.text = card.CardName;
            cardDescription.text = card.CardDescription;
            cardImage.sprite = card.CardSprite;
        }
    }
}
