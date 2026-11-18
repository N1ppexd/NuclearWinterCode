using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NuclearWinter
{
    [CreateAssetMenu(fileName =  "StatsSO", menuName = "ScriptableObjects/StatsSO")]
    public class StatsSO : ScriptableObject
    {
        [SerializeField] private string statName;
        [SerializeField] private Sprite statSprite;

        [SerializeField] private int startAmount;

        [SerializeField] private IntEvent statChangeEvent;

        public string StatName => statName;
        public Sprite StatSprite => statSprite;
        public int StartAmount => startAmount;

        public void ChangeAmount(int amount)
        {
            Debug.Log("amount " + amount);
            statChangeEvent.Raise(amount);
        }
    }
}
