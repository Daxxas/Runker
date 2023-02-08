using System;
using UnityEngine;

namespace Game.Level
{
    public class LevelTimer : MonoBehaviour
    {
        private float currentTime = 0f;
        private bool timerStarted = false;
        
        public void StartTimer()
        {
            timerStarted = true;
        }
        
        public void StopTimer()
        {
            timerStarted = false;
        }
        
        public void ResetTimer()
        {
            currentTime = 0f;
        }
        
        private void Update()
        {
            if (timerStarted)
            {
                currentTime += Time.deltaTime;
            }
        }
    }
}