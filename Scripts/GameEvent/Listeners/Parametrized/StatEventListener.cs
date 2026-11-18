using System;
using UnityEngine;
using UnityEngine.Events;

namespace NuclearWinter
{
    public class StatEventListener : ParametrizedGameEventListener<PlayerAttributes>
    {
        [Serializable]
        private class UnityEvent : UnityEvent<PlayerAttributes> { }
        [SerializeField] UnityEvent unityEventResponse;

        private void Awake()
        {
            if (unityEventResponse == null) unityEventResponse = new UnityEvent();
        }


        protected override void AddListener(UnityAction<PlayerAttributes> action)
        {
            unityEventResponse.AddListener(action);
        }

        protected override void RemoveListener(UnityAction<PlayerAttributes> action)
        {
            unityEventResponse.RemoveListener(action);
        }

        protected override void InvokeUnityEventResponse(PlayerAttributes value)
        {
            unityEventResponse?.Invoke(value);
        }
    }
}
