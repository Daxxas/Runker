using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class TriggerEvent : MonoBehaviour
    {
        [SerializeField] private LayerMask triggerMask;
        
        public UnityEvent<Collider> onTriggerEnter;
        public UnityEvent<Collider> onTriggerStay;
        public UnityEvent<Collider> onTriggerExit;

        private void OnTriggerEnter(Collider other)
        {
            if ((triggerMask & (1 << other.gameObject.layer)) != 0)
            {
                onTriggerEnter?.Invoke(other);
            }
        }
        
        private void OnTriggerStay(Collider other)
        {
            if ((triggerMask & (1 << other.gameObject.layer)) != 0)
            {
                onTriggerStay?.Invoke(other);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if ((triggerMask & (1 << other.gameObject.layer)) != 0)
            {
                onTriggerExit?.Invoke(other);
            }
        }
    }
}