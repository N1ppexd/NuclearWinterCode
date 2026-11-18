using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace NuclearWinter
{
    public class CardInventory : MonoBehaviour
    {
        [Required]
        [SerializeField] private GameObject cardPrefab;
        [Required]
        [SerializeField] private Transform cardParent;



        List<ItemSlot> slots = new List<ItemSlot>();


        public void AddCard(CardSO card)
        {
            if (GameManager.Instance.Inventory == null) return;

            if(GameManager.Instance.Inventory.Cards.Count + 1 >= GameManager.Instance.Inventory.MaxSlots)
            {
                Debug.Log("Inventory is full");
                return;
            }

            if(card is not CardItemSO)
            {
                Debug.LogError("Card is not a CardItemSO");
                return;
            }

            


            if(GameManager.Instance.Inventory.GetCard(card as CardItemSO) == null || GameManager.Instance.Inventory.GetCard(card as CardItemSO).amount <= 0)
            {
                GameObject cardObject = Instantiate(cardPrefab, cardParent);

                if (cardObject.TryGetComponent(out ItemSlot itemSlot))
                {
                    slots.Add(cardObject.GetComponent<ItemSlot>());
                    itemSlot.SetItem(card as CardItemSO);
                }
            }
            else
            {
                foreach(var slot in slots)
                {
                    if(slot.CardItemSO == card)
                    {
                        slot.SetAmount(GameManager.Instance.Inventory.GetCard(card as CardItemSO).amount + 1);
                    }
                }
            }

            GameManager.Instance.Inventory.AddCard(card as CardItemSO);
            



        }
    }
}
