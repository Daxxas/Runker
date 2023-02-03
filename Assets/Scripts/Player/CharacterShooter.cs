using System;
using Player.Inputs;
using UnityEngine;

namespace Player
{
    public class CharacterShooter : MonoBehaviour
    {
        [SerializeField] private InputProvider inputProvider;

        private bool isAiming = false;
        public bool IsAiming => isAiming;
        public Action<bool> onAim;

        
        private void Start()
        {
            inputProvider.onAim += isAiming =>
            {
                onAim?.Invoke(isAiming);
                this.isAiming = isAiming;
            };
        }
    }
}