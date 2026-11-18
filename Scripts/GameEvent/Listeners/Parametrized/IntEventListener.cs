using System;
using UnityEngine;
using UnityEngine.Events;

namespace NuclearWinter
{
    public class IntEventListener : ParametrizedGameEventListener<int>
    {
        [Serializable]
        private class UnityEvent : UnityEvent<int> { }
        [SerializeField] UnityEvent unityEventResponse;

        private void Awake()
        {
            if (unityEventResponse == null) unityEventResponse = new UnityEvent();
        }


        protected override void AddListener(UnityAction<int> action)
        {
            unityEventResponse.AddListener(action);
        }

        protected override void RemoveListener(UnityAction<int> action)
        {
            unityEventResponse.RemoveListener(action);
        }

        protected override void InvokeUnityEventResponse(int value)
        {
            unityEventResponse?.Invoke(value);
        }

       
    }
}
