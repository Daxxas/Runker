using System;
using UnityEngine;

namespace Player.Camera
{
    public class CameraShaker : MonoBehaviour
    {
        [SerializeField] private int channel = 0;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private float amplitude = 1f;
        [SerializeField] private float shakeReductionRate = .1f;
        
        private float currentShake = 0f;

        public float CurrentShake
        {
            get => currentShake;
            set => currentShake = Mathf.Clamp01(value);
        }

        public void AddShake(float value)
        {
            CurrentShake += value;
        }
        
        private float GetShake(int seed, float time, float amplitude)
        {
            return Mathf.PerlinNoise(seed + time, seed + time) * Mathf.Pow(amplitude, 2);
        }
        
        private void Update()
        {
            CurrentShake -= shakeReductionRate;
            cameraController.SetShake(GetShake(1, Time.time, amplitude), CurrentShake * amplitude, CurrentShake * amplitude);
        }
    }
}