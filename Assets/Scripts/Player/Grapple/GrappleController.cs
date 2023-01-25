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
        [SerializeField] private float grappleStopMomentumThreshold = 1.1f;
        public float GrappleStopMomentumThreshold => grappleStopMomentumThreshold;
        [SerializeField] private float grappleMinHoldPower = 1.1f;
        public float GrappleMinHoldPower => grappleMinHoldPower;
        [SerializeField] private float grappleThrowSpeed = 1f;

        private float currentThrowDistance = 0f;
        private Vector3 grappleTargetPoint = Vector3.zero;
        
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

        private GrappleState currentGrappleState;
        public GrappleState CurrentGrappleState => currentGrappleState;

        public enum GrappleState
        {
            None,
            Throw,
            Lock
        }
        
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
                else
                {
                    ReleaseGrapple();
                }
            };
        }

        private void Update()
        {
            if (currentGrappleState == GrappleState.Throw)
            {
                Vector3 grappleDirection = (grappleTargetPoint - grappleOrigin.position).normalized;
                
                currentThrowDistance += grappleThrowSpeed * Time.deltaTime;
                grapplePoint = grappleOrigin.position + grappleDirection * currentThrowDistance;
                
                // Grapple hit nothing
                if (currentThrowDistance > grappleMaxDistance)
                {
                    ReleaseGrapple();
                }
                
                // Grapple hit something
                if (Physics.Raycast(grappleOrigin.position, grappleDirection, out var hit, currentThrowDistance, grappleMask))
                {
                    currentGrappleState = GrappleState.Lock;
                    onGrapple?.Invoke();
                    grappleHit = true;
                    grapplePoint = hit.point;
                }
            }
        }
        
        private void ThrowGrapple()
        {
            currentGrappleState = GrappleState.Throw;
            grappleTargetPoint = grappleOrigin.position + camera.forward * grappleMaxDistance;
            
            Color debugColor = Color.red;
            Debug.DrawLine(grappleOrigin.position, grappleOrigin.position + (camera.forward * grappleMaxDistance), debugColor, 2f);
        }

        public void ReleaseGrapple()
        {
            currentThrowDistance = 0f;
            currentGrappleState = GrappleState.None;
            grappleHit = false;
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(grappleTargetPoint, 1f);
        }
        #endif
    }
}