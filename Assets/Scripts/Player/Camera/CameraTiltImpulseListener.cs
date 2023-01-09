using System;
using UnityEngine;

namespace Player.Camera
{
    public class CameraTiltImpulseListener : MonoBehaviour
    {
        [SerializeField] private int channel = 0;
        [SerializeField] private CinemachineRecomposer recomposer;
        
        private CameraTiltImpulse currentImpulse;
        private float currentStartTime = 0f;

        private static Action<int, CameraTiltImpulse> onEvent;

        public static void Impulse(int channel, CameraTiltImpulse tiltImpulse)
        {
            onEvent?.Invoke(channel, tiltImpulse);
        }
        
        private void Start()
        {
            onEvent += ApplyImpulse;
        }

        private void ApplyImpulse(int channel, CameraTiltImpulse tiltImpulse)
        {
            if (this.channel == channel)
            {
                currentImpulse = tiltImpulse;
                currentStartTime = Time.time;
            }
        }
        
        private void Update()
        {
            if (currentImpulse != null && Time.time < currentStartTime + currentImpulse.Duration)
            {
                float impulseProgress = (Time.time - currentStartTime) / currentImpulse.Duration;

                recomposer.m_Tilt = currentImpulse.Direction.y * currentImpulse.TiltCurve.Evaluate(impulseProgress) * currentImpulse.Amplitude;
                recomposer.m_Pan = currentImpulse.Direction.x * currentImpulse.TiltCurve.Evaluate(impulseProgress) * currentImpulse.Amplitude;
            }
        }
    }
}