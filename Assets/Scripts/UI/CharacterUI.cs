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
            speedText.text = characterController.HorizontalVelocity.magnitude.ToString("F2");
        }
    }
}