using System;
using UnityEngine;
using UnityEngine.Events;

namespace NuclearWinter
{
    public class StringEventListener : ParametrizedGameEventListener<string>
    {
        [Serializable]
        private class UnityEvent : UnityEvent<string> { }
        [SerializeField] UnityEvent unityEventResponse;

        private void Awake()
        {
            if (unityEventResponse == null) unityEventResponse = new UnityEvent();
        }


        protected override void AddListener(UnityAction<string> action)
        {
            unityEventResponse.AddListener(action);
        }

        protected override void RemoveListener(UnityAction<string> action)
        {
            unityEventResponse.RemoveListener(action);
        }

        protected override void InvokeUnityEventResponse(string value)
        {
            unityEventResponse?.Invoke(value);
        }

       
    }
}
