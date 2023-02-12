using System;
using Player.Inputs;
using UnityEngine;

namespace Game.Level
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private PlayerInputProvider playerInputProvider;
        
        public static Action onLevelStart;
        public static Action onLevelEnd;
        public static bool LevelStarted { get; private set; }
        
        private MainInputs inputs;
        
        private void Awake()
        {
            inputs = new MainInputs();
            inputs.Enable();
            inputs.UI.Confirm.performed += _ => StartLevel();
        }

        private void Start()
        {
            playerInputProvider.enabled = false;
        }

        public void StartLevel()
        {
            onLevelStart?.Invoke();
            LevelStarted = true;
            playerInputProvider.enabled = true;
            DisplayCursor(false);
        }

        public void EndLevel()
        {

            Debug.Log("End level");
            onLevelEnd?.Invoke();
            LevelStarted = false;
            playerInputProvider.enabled = false;
            DisplayCursor(true);
        }

        private void OnDisable()
        {
            inputs.Disable();
            inputs.UI.Confirm.performed -= _ => StartLevel();
        }

        private void DisplayCursor(bool display)
        {
            Cursor.visible = display;
            Cursor.lockState = display ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}