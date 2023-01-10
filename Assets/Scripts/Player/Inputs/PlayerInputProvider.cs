using UnityEngine;

namespace Player.Inputs
{
    public class PlayerInputProvider : InputProvider
    {
        private MainInputs mainInputs;

        [SerializeField] private float escapePressMaxTime = 0.2f;
        private Vector2 previousEscape = Vector2.zero;
        private float lastEscapePressTime = 0f;

        private void Awake()
        {
            mainInputs = new MainInputs();
            mainInputs.Main.Move.performed += context => onMove?.Invoke(context.ReadValue<Vector2>());
            mainInputs.Main.Move.canceled += context => onMove?.Invoke(context.ReadValue<Vector2>());
            
            // TODO : Code double tap behaviour
            mainInputs.Main.EscapeY.performed += context =>
            {                
                EscapePress(new Vector2(0, context.ReadValue<float>()));

            };
            mainInputs.Main.EscapeX.performed += context =>
            {
                EscapePress(new Vector2(context.ReadValue<float>(), 0));
            };

            mainInputs.Main.Jump.performed += context => onJump?.Invoke(context.ReadValue<float>() != 0f);
            mainInputs.Main.Jump.canceled += context => onJump?.Invoke(context.ReadValue<float>() != 0f);
            
            mainInputs.Main.Crouch.performed += context => onCrouch?.Invoke(context.ReadValue<float>() != 0f);
            mainInputs.Main.Crouch.canceled += context => onCrouch?.Invoke(context.ReadValue<float>() != 0f);
            
            mainInputs.Main.Run.performed += context => onRun?.Invoke(context.ReadValue<float>() != 0f);
            mainInputs.Main.Run.canceled += context => onRun?.Invoke(context.ReadValue<float>() != 0f);

            mainInputs.Main.Zoom.performed += context => onScroll?.Invoke(context.ReadValue<float>());
        }

        private void EscapePress(Vector2 direction)
        {
            if (direction == previousEscape && Time.time < lastEscapePressTime + escapePressMaxTime)
            {
                onEscape?.Invoke(direction);
            }
            
            
            previousEscape = direction;
            lastEscapePressTime = Time.time;
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