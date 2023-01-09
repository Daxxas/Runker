using Player.Camera;
using UnityEngine;

namespace Player.Camera
{
    public class CameraTiltImpulse : MonoBehaviour
    {
        // [SerializeField] private float amplitude = 1f;
        [SerializeField] private int channel = 0;
        [SerializeField] private AnimationCurve tiltCurve;
        [SerializeField] private Vector2 direction;
        [SerializeField] private float amplitude;
        [SerializeField] private float duration;

        public AnimationCurve TiltCurve => tiltCurve;
        public Vector2 Direction => direction;
        public float Amplitude => amplitude;
        public float Duration => duration;
        
        [ContextMenu("Impulse")]
        public void Impulse()
        {
            CameraTiltImpulseListener.Impulse(channel, this);
        }
    }
}