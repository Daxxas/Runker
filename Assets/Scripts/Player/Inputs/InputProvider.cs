using System;
using UnityEngine;

namespace Player.Inputs
{
    public class InputProvider : MonoBehaviour
    {
        private MainInputs mainInputs;

        public Action<Vector2> onMove;
        public Action<bool> onCrouch;
        public Action<bool> onJump;
        public Action<float> onScroll;

        private Vector2 moveDirection;
        private Vector3 moveDirectionV3;
        public Vector2 MoveDirection => moveDirection;
        public Vector3 MoveDirectionV3 => moveDirectionV3;

        private void Awake()
        {
            mainInputs = new MainInputs();
            mainInputs.Main.Move.performed += context => onMove?.Invoke(context.ReadValue<Vector2>());
            mainInputs.Main.Move.canceled += context => onMove?.Invoke(context.ReadValue<Vector2>());

            mainInputs.Main.Jump.performed += context => onJump?.Invoke(context.ReadValue<float>() != 0f);
            mainInputs.Main.Jump.canceled += context => onJump?.Invoke(context.ReadValue<float>() != 0f);
            
            mainInputs.Main.Crouch.performed += context => onCrouch?.Invoke(context.ReadValue<float>() != 0f);
            mainInputs.Main.Crouch.canceled += context => onCrouch?.Invoke(context.ReadValue<float>() != 0f);

            mainInputs.Main.Zoom.performed += context => onScroll?.Invoke(context.ReadValue<float>());
        }

        private void Update()
        {
            moveDirection = mainInputs.Main.Move.ReadValue<Vector2>();
            moveDirectionV3 = new Vector3(moveDirection.x, 0, moveDirection.y);
        }
        
        private void OnEnable()
        {
            mainInputs.Enable();
        }

        private void OnDisable()
        {
            mainInputs.Disable();
        }
    }
}