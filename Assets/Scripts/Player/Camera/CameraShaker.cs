using System;
using UnityEngine;

namespace Player.Camera
{
    public class CameraShaker : MonoBehaviour
    {
        [SerializeField] private CameraController cameraController;
        [SerializeField] private float shakeReductionRate = .1f;
        [SerializeField] private float perlinFrequency = 1f;
        [SerializeField] [Range(0f, 90f)] private float dutchRange;
        [SerializeField] [Range(0f, 90f)] private float tiltRange;
        [SerializeField] [Range(0f, 90f)] private float panRange;
        
        
        private float currentShake = 0f;
        private float fixedShake = 0f;

        public float CurrentShake
        {
            get => currentShake + fixedShake;
            set => currentShake = Mathf.Clamp01(value);
        }

        public void AddShake(float value)
        {
            Debug.Log("Add Shake " + value);
            CurrentShake += value;
        }
        
        public void SetFixedShake(float value)
        {
            fixedShake = value;
        }

        [ContextMenu("Debug Shake")]
        public void DebugShake()
        {
            SetFixedShake(.4f);
        }
        
        private float GetShake(int seed, float time, float frequency = 1f)
        {
            return Mathf.PerlinNoise((seed + time) * frequency, (seed + time) * frequency) * 2f - 1f;
        }

        private void Update()
        {
            currentShake = Mathf.Clamp01(currentShake - shakeReductionRate * Time.deltaTime);
            float tiltShake = tiltRange * Mathf.Pow(CurrentShake, 2) * GetShake(1, Time.time, perlinFrequency);
            float panShake = panRange * Mathf.Pow(CurrentShake, 2) * GetShake(2, Time.time, perlinFrequency);
            float dutchShake = dutchRange * Mathf.Pow(CurrentShake, 2) * GetShake(3, Time.time, perlinFrequency);

            cameraController.SetShake(dutchShake, tiltShake, panShake);
        }
    }
}