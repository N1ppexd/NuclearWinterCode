using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NuclearWinter
{
    public class ParametrizedGameEvent<T> : AbstractGameEvent
    {
        protected HashSet<ParametrizedGameEventListener<T>> _listeners = new HashSet<ParametrizedGameEventListener<T>>();

        public IReadOnlyCollection<ParametrizedGameEventListener<T>> Listeners => _listeners;

        public bool RegisterListener(ParametrizedGameEventListener<T> listener) => _listeners.Add(listener);

        public void UnregisterListener(ParametrizedGameEventListener<T> listener) => _listeners.Remove(listener);

        public void Raise(T value)
        {
            Debug.Log("Raising event");
            foreach (ParametrizedGameEventListener<T> listener in _listeners)
            {
                listener.OnEventRaised(value);
            }
        }
    }
}
