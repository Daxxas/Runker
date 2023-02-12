using TMPro;
using UnityEngine;
using Player;
using CharacterController = Player.CharacterController;

namespace UI
{
    public class CharacterUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI speedText;
        [SerializeField] private CharacterController characterController;
        
        private void Update()
        {
            speedText.text = " Horizontal: " + characterController.HorizontalVelocity.magnitude.ToString("F2");
            speedText.text += "\n";
            speedText.text += "Vertical: " + characterController.Motor.Velocity.y.ToString("F2");

        }
    }
}