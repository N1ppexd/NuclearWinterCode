using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NuclearWinter
{
    public class Inventory
    {

        int maxSlots;


        public int MaxSlots => maxSlots;


        public Inventory(int maxSlots) { this.maxSlots = maxSlots; }
        public Inventory(List<CardItemSO> cards, int maxSlots)
        {
            foreach (CardItemSO card in cards)
            {
                Cards.Add(new InventoryCard(card, 0));
            }
            this.maxSlots = maxSlots;
        }


        public InventoryCard GetCard(CardItemSO card)
        {
            foreach(var cardItem in Cards)
            {
                if(cardItem.card == card)
                    return cardItem;
            }

            return null;
        }

        public List<InventoryCard> Cards { get; private set; } = new List<InventoryCard>();


        public void AddCard(CardItemSO card)
        {
            foreach(var cardItem in Cards)
            {
                if(cardItem.card == card)
                {
                    cardItem.amount++;
                    return;
                }
            }

            Cards.Add(new InventoryCard(card, 1));
        }
        public bool TryRemoveCard(CardItemSO card, out CardItemSO resultCard)
        {
            resultCard = card;
            foreach (var cardItem in Cards)
            {
                if (cardItem.card == card)
                {
                    cardItem.amount++;
                    return true;
                }
            }

            return false;
        }

    }



    public class InventoryCard
    {
        public CardItemSO card;
        public int amount;


        public InventoryCard(CardItemSO card, int amount)
        {
            this.card = card;
            this.amount = amount;
        }
    }
}
