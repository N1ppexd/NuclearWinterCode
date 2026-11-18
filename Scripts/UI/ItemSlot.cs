using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace NuclearWinter
{
    public class ItemSlot : MonoBehaviour
    {
        [SerializeField] private Image itemSprite;

        [SerializeField] private TextMeshProUGUI itemAmountText;

        private int itemAmount;

        private CardItemSO cardItemSO;

        public void SetAmount(int amount)
        {
            itemAmount += amount;
            itemAmountText.text = amount.ToString();
        } 
        public CardItemSO CardItemSO => cardItemSO;
        public Sprite ItemSprite => itemSprite.sprite;

        public void SetItem(CardItemSO item)
        {
            cardItemSO = item;
            SetItemSprite(item.CardSprite);
        }
        public void SetItemSprite(Sprite sprite)
        {
            itemSprite.sprite = sprite;
        }


        public void ClickCard()
        {
            Debug.Log("show the fucking card");
            cardItemSO?.ShowCard();
        }
    }
}
