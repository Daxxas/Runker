using System;
using Player.Inputs;
using UnityEngine;

namespace Player.Grapple
{
    public class GrappleThrower : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Transform grappleOrigin;
        [SerializeField] private Transform camera;
        [SerializeField] private float grappleMaxDistance = 15f;
        [SerializeField] private LayerMask grappleMask;
        
        private InputProvider inputProvider;

        private void Start()
        {
            // Setup input
            inputProvider = GetComponent<InputProvider>();
            inputProvider.onGrapple += buttonActive =>
            {
                if (buttonActive)
                {
                    ThrowGrapple();
                }
            };
        }

        private void ThrowGrapple()
        {
            Color debugColor = Color.red;
            if (Physics.Raycast(grappleOrigin.position, camera.forward, out var hit, grappleMaxDistance, grappleMask))
            {
                debugColor = Color.blue;
            }
            
            Debug.DrawLine(grappleOrigin.position, grappleOrigin.position + (camera.forward * grappleMaxDistance), debugColor, 2f);

        }
    }
}