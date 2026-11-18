using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NuclearWinter
{
    public class FloatEventListener : ParametrizedGameEventListener<float>
    {
        [Serializable]
        private class UnityEvent : UnityEvent<float> { }
        [SerializeField] UnityEvent unityEventResponse;

        private void Awake()
        {
            if (unityEventResponse == null) unityEventResponse = new UnityEvent();
        }


        protected override void AddListener(UnityAction<float> action)
        {
            unityEventResponse.AddListener(action);
        }

        protected override void RemoveListener(UnityAction<float> action)
        {
            unityEventResponse.RemoveListener(action);
        }

        protected override void InvokeUnityEventResponse(float value)
        {
            unityEventResponse?.Invoke(value);
        }
    }
}
