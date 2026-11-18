using System;
using UnityEngine;
using UnityEngine.Events;

namespace NuclearWinter
{
    public class CardActionListener : ParametrizedGameEventListener<ActionSO>
    {
        [Serializable]
        private class UnityEvent : UnityEvent<ActionSO> { }

        [SerializeField] UnityEvent unityEventResponse;

        private void Awake()
        {
            if (unityEventResponse == null) unityEventResponse = new UnityEvent();
        }

        protected override void AddListener(UnityAction<ActionSO> action) => unityEventResponse.AddListener(action);
        protected override void RemoveListener(UnityAction<ActionSO> action) => unityEventResponse.RemoveListener(action);
        protected override void InvokeUnityEventResponse(ActionSO value) => unityEventResponse.Invoke(value);
    }
}
