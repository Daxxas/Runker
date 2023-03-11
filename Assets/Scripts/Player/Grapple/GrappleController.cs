﻿using System;
using Enemies;
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
        [Tooltip("Reach of the grapple")]
        [SerializeField] private float grappleMaxDistance = 15f;        
        [Tooltip("Layers the grapple can hook on")]
        [SerializeField] private LayerMask grappleMask;
        [Tooltip("Air control while being pulled by the grapple")]
        [SerializeField] private float grappleAirControl = 1f;
        [Tooltip("Bad variable name")]
        [SerializeField] private float grappleForceReleaseMaxDistance = .5f;
        [Tooltip("Minimum distance between grapple origin point and grapple point, otherwise grapple is released")]
        [SerializeField] private float grappleOriginMinDistanceFromGrapplePoint = .5f;
        [Tooltip("Aim assist radius for grapple")]
        [SerializeField] private float grappleAimAssistRadius = 1f;
        public float GrappleAirControl => grappleAirControl;
        [SerializeField] private float grappleAcceleration;
        public float GrappleAcceleration => grappleAcceleration;
        [SerializeField] private float grappleThrowTime = 1f;
        private float grappleThrowSpeed => grappleMaxDistance / grappleThrowTime;

        private float currentThrowDistance = 0f;
        private Vector3 grappleTargetPoint = Vector3.zero;
        
        private InputProvider inputProvider;

        private Collider grappleHitCollider;
        private Vector3 lastGrappleHitColliderPosition;
        
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
                    lastGrappleHitColliderPosition = hit.transform.position;
                    grappleHitCollider = hit.collider;
                }
            }
            else if (currentGrappleState == GrappleState.Lock)
            {
                grapplePoint += grappleHitCollider.transform.position - lastGrappleHitColliderPosition;
                
                if(grappleHitCollider.TryGetComponent(out GrapplePull pull))
                {
                    pull.Pull((grapplePoint - grappleOrigin.position).normalized, -grappleAcceleration);
                }
                
                // Check if there's something between grapple point and grapple origin
                if (Physics.Raycast(grappleOrigin.position, grapplePoint - grappleOrigin.position, out var deltaHit, currentThrowDistance, grappleMask))
                {
                    if((deltaHit.point - grapplePoint).magnitude > grappleForceReleaseMaxDistance)
                        ReleaseGrapple();
                }
                
                // Check if grapple origin is too close from grapple point
                if ((grappleOrigin.position - grapplePoint).magnitude < grappleOriginMinDistanceFromGrapplePoint)
                {
                    ReleaseGrapple();
                }

                lastGrappleHitColliderPosition = grappleHitCollider.transform.position;
            }
        }
        
        private void ThrowGrapple()
        {
            currentGrappleState = GrappleState.Throw;

            if (Physics.SphereCast(grappleOrigin.position, grappleAimAssistRadius, camera.forward, out var hit, grappleMaxDistance, grappleMask))
            {
                Debug.DrawLine(grappleOrigin.position, hit.point, Color.yellow, 2f);
                grappleTargetPoint = hit.point;
            }
            else
            {
                grappleTargetPoint = grappleOrigin.position + camera.forward * grappleMaxDistance;
            }
            
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