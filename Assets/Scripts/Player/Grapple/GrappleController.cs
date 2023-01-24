using System;
using Player.Inputs;
using UnityEngine;

namespace Player.Grapple
{
    public class GrappleController : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Transform grappleOrigin;
        public Transform GrappleOrigin => grappleOrigin;

        [SerializeField] private Transform camera;
        [SerializeField] private float grappleMaxDistance = 15f;
        [SerializeField] private LayerMask grappleMask;
        [SerializeField] private float grappleAirControl = 1f;
        public float GrappleAirControl => grappleAirControl;
        [SerializeField] private float grappleAcceleration;
        public float GrappleAcceleration => grappleAcceleration;
        [SerializeField] private float grappleResistance = 10f;
        public float GrappleResistance => grappleResistance;
        [SerializeField] private float grappleMinHoldPower = 1.1f;
        public float GrappleMinHoldPower => grappleMinHoldPower;
        
        [SerializeField] private float grappleMaxHoldPower = 1.9f;
        public float GrappleMaxHoldPower => grappleMaxHoldPower;

        private InputProvider inputProvider;

        private bool grappleHit = false;
        public bool GrappleHit
        {
            get => grappleHit;
            set => grappleHit = value;
        }

        private Vector3 grapplePoint;
        public Vector3 GrapplePoint => grapplePoint;

        public Action onGrapple;
        
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
                onGrapple?.Invoke();
                grappleHit = true;
                grapplePoint = hit.point;
            }
            
            Debug.DrawLine(grappleOrigin.position, grappleOrigin.position + (camera.forward * grappleMaxDistance), debugColor, 2f);

        }
    }
}