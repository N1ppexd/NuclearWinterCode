using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace NuclearWinter
{
    public class CardEventListener : ParametrizedGameEventListener<CardSO>
    {
        [Serializable]
        private class UnityEvent : UnityEvent<CardSO> { }

        [SerializeField] UnityEvent unityEventResponse;

        [SerializeField] private CardEventType cardEventType;

        private void Awake()
        {
            if (unityEventResponse == null) unityEventResponse = new UnityEvent();
        }

        protected override void AddListener(UnityAction<CardSO> action) => unityEventResponse.AddListener(action);
        protected override void RemoveListener(UnityAction<CardSO> action) => unityEventResponse.RemoveListener(action);

        protected override void InvokeUnityEventResponse(CardSO value)
        {
            switch (cardEventType)
            {
                case CardEventType.eventCards:
                    if (value is CardEventSO)
                    {
                        unityEventResponse?.Invoke(value);
                    }

                    break;
                case CardEventType.itemCards:
                    if (value is CardItemSO)
                    {
                        unityEventResponse?.Invoke(value);
                    }
                    break;
            }
        }


        public enum CardEventType
        {
            eventCards,
            itemCards,
        }

    }
    
}
