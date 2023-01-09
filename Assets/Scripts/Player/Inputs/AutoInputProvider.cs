using System;
using KinematicCharacterController;
using UnityEngine;

namespace Player.Inputs
{
    public class AutoInputProvider : InputProvider
    {
        [SerializeField] private Vector2 inputDirection;
        public bool moveForward = false;
        public bool runningCommand = false;
        public bool jumpingCommand = false;

        private bool isRunning = false;
        private bool isJumping = false;

        private Vector3 startPosition;

        private void Start()
        {
            startPosition = transform.position;
        }

        [ContextMenu("Reset")]
        public void ResetPosition()
        {
            GetComponent<KinematicCharacterMotor>().SetPosition(startPosition);
        }
        
        private void Update()
        {
            // Jumping
            if (jumpingCommand && !isJumping)
            {
                isJumping = true;
                onJump?.Invoke(true);
            }
            
            if(!jumpingCommand && isJumping)
            {
                isJumping = false;
                onJump?.Invoke(false);
            }
            
            // Running
            if (runningCommand && !isRunning)
            {
                isRunning = true;
                onRun?.Invoke(true);
            }
            
            if(!runningCommand && isRunning)
            {
                isRunning = false;
                onRun?.Invoke(false);
            }
            
            // Moving
            Vector2 forward;

            if (moveForward)
            {
                forward = inputDirection;
            }
            else
            {
                forward = Vector2.zero;
            }
            
            onMove?.Invoke(forward);
            moveDirection = forward;
            moveDirectionV3 = new Vector3(forward.x, 0, forward.y);
        }
    }
}