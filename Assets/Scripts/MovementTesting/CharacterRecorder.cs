using System;
using TMPro;
using UnityEngine;
using CharacterController = Player.CharacterController;

namespace MovementTesting
{
    public class CharacterRecorder : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        private TextMeshPro tmp;

        private float maxY = 0;
        
        private void Start()
        {
            tmp = GetComponent<TextMeshPro>();
            characterController.onJump += () => RecordJumpDistance(true);
            characterController.onLand += () => RecordJumpDistance(false);
        }

        public void Reset()
        {
            maxY = 0;
        }
        
        private void Update()
        {
            if(characterController.transform.position.y > maxY)
                maxY = characterController.transform.position.y;

            tmp.text = "Highest height recorded " + maxY;
            tmp.text += "\n";
            tmp.text += "Jump Distance " + bestJumpDistance;
        }

        private Vector3 lastJumpPosition;
        private float bestJumpDistance = 0;
        
        private void RecordJumpDistance(bool start)
        {
            if(start)
                lastJumpPosition = characterController.transform.position;
            else
                bestJumpDistance = Vector3.Distance(lastJumpPosition, characterController.transform.position);
        }
    }
}