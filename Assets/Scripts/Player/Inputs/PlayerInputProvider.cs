using UnityEngine;

namespace Player.Inputs
{
    public class PlayerInputProvider : InputProvider
    {
        private MainInputs mainInputs;

        private void Awake()
        {
            mainInputs = new MainInputs();
            mainInputs.Main.Move.performed += context => onMove?.Invoke(context.ReadValue<Vector2>());
            mainInputs.Main.Move.canceled += context => onMove?.Invoke(context.ReadValue<Vector2>());

            mainInputs.Main.Jump.performed += context => onJump?.Invoke(context.ReadValue<float>() != 0f);
            mainInputs.Main.Jump.canceled += context => onJump?.Invoke(context.ReadValue<float>() != 0f);
            
            mainInputs.Main.Crouch.performed += context => onCrouch?.Invoke(context.ReadValue<float>() != 0f);
            mainInputs.Main.Crouch.canceled += context => onCrouch?.Invoke(context.ReadValue<float>() != 0f);
            
            mainInputs.Main.Run.performed += context => onRun?.Invoke(context.ReadValue<float>() != 0f);
            mainInputs.Main.Run.canceled += context => onRun?.Invoke(context.ReadValue<float>() != 0f);

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