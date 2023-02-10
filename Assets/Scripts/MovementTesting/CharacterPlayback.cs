using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using CharacterController = Player.CharacterController;

namespace MovementTesting
{
    public class CharacterPlayback : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Transform targetTransform;
        
        [Header("Parameters")]
        [SerializeField] private float minimumDistanceBetweenPoints = .05f;
        [SerializeField] public List<CharacterPosition> recordedPositions = new List<CharacterPosition>();

        public bool recording = false;

        private void Start()
        {
            
        }

        public void Record(bool recording)
        {
            this.recording = recording;
            
            if (recording)
            {
                recordedPositions.Clear();
                lineRenderer.positionCount = 0;
                CreatePosition();
            }
        }
        
        private void Update()
        {
            if (recording)
            {
                float delta;   
                if (recordedPositions.Count == 0)
                    delta = Mathf.Infinity;
                else 
                    delta = (recordedPositions[recordedPositions.Count - 1].position - targetTransform.position).magnitude;
                
                if (delta > minimumDistanceBetweenPoints)
                {
                    CreatePosition();
                }
            }
        }

        private void CreatePosition()
        {
            recordedPositions.Add(new CharacterPosition()
            {
                position = targetTransform.position,
                rotation = targetTransform.eulerAngles,
                time = Time.time
            });
            lineRenderer.positionCount += 1;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, targetTransform.position);
        }

        private void OnApplicationQuit()
        {
            lineRenderer.positionCount = recordedPositions.Count;
            for (int i = 0; i < recordedPositions.Count; i++)
            {
                lineRenderer.SetPosition(i, recordedPositions[i].position);
            }
        }
    }
    
    [Serializable]
    public struct CharacterPosition
    {
        public Vector3 position;
        public Vector3 rotation;
        public float time;
    }
}