using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NuclearWinter
{
    public abstract class AbstractListener : MonoBehaviour
    {
        [Serializable]
        public enum ResponseMode
        {
            InvokeUnityEvents,
            InvokeCSharpEvents
        }

        [SerializeField] protected ResponseMode _responseActivationMode;

        public ResponseMode ResponseActivationMode { get => _responseActivationMode; set => _responseActivationMode = value; }
    }
}
