using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NuclearWinter
{
    public abstract class ParametrizedGameEventListener<T> : AbstractListener, ISerializationCallbackReceiver
    {
        protected ParametrizedGameEvent<T> _event;
        public ParametrizedGameEvent<T> Event
        {
            get { return _event; }
            set
            {
                _event?.UnregisterListener(this);
                _event = value;
                _event?.RegisterListener(this);
            }
        }

        // C# Event
        public event Action<T> Response;

        // Unity Event
        public event UnityAction<T> UnityEventResponse
        {
            add { AddListener(value); }
            remove => RemoveListener(value);
        }

        #region Event Serialization
        [SerializeField] ScriptableObject _serializedEvent = null;

        public void OnBeforeSerialize()
        {
            // Nothing to do before serialization
            // (because _serializedEvent acts already as our serialization)
        }

        public void OnAfterDeserialize()
        {
            // Assign Event value from serialized event
            //_event = _serializedEvent as ParametrizedGameEvent<T>;
        }
        #endregion

        protected virtual void OnEnable()
        {
            _event = _serializedEvent as ParametrizedGameEvent<T>;
            Event = _event;

            Event?.RegisterListener(this);
        }

        protected virtual void OnDisable()
        {
            Event?.UnregisterListener(this);
        }

        public void OnEventRaised(T value)
        {
            switch (_responseActivationMode)
            {
                case ResponseMode.InvokeUnityEvents:
                    InvokeUnityEventResponse(value);
                    break;
                case ResponseMode.InvokeCSharpEvents:
                    Response?.Invoke(value);
                    break;
            }
        }

        protected abstract void InvokeUnityEventResponse(T value);
        protected abstract void AddListener(UnityAction<T> action);
        protected abstract void RemoveListener(UnityAction<T> action);
    }
}
