using TMPro;
using UnityEngine;

namespace UI
{
    public class FPSCounter : MonoBehaviour
    {
        public int avgFrameRate;
        public TextMeshProUGUI TMPText;
 
        public void Update ()
        {
            float current = 0;
            current = Time.frameCount / Time.time;
            avgFrameRate = (int)current;
            TMPText.text = avgFrameRate.ToString() + " FPS";
        }
    }
}