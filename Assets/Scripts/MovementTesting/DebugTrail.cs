using System;
using Unity.VisualScripting;
using UnityEngine;

namespace MovementTesting
{
    public class DebugTrail : MonoBehaviour
    {
        private LineRenderer trailRenderer;

        [SerializeField] private Transform target;
        [SerializeField] private float sampleRate = 0.1f;
        private float lastPointTime = 0f;
        
        private void Start()
        {
            trailRenderer = GetComponent<LineRenderer>();
        }

        private void Update()
        {
            if(Time.time - lastPointTime > sampleRate)
            {
                AddPosition(target.position);
                lastPointTime = Time.time;
            }
        }

        private void AddPosition(Vector3 position)
        {
            trailRenderer.positionCount += 1;
            trailRenderer.SetPosition(trailRenderer.positionCount-1, position);
        }
    }
}