using System;
using KinematicCharacterController;
using Player.Inputs;
using UnityEditor;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(InputProvider))]
    [RequireComponent(typeof(KinematicCharacterMotor))]
    public class CharacterController : MonoBehaviour, ICharacterController
    {
        [Header("References")]
        [SerializeField] private Transform cameraTransform;
        [Header("Parameters")]
        [Header("General")]
        [SerializeField] private float gravity = 30f;
        [Header("Walk")]
        [SerializeField] private float groundedMoveSpeed = 5f;
        [SerializeField] private float orientationSharpness = 10f;
        [SerializeField] private float walkSharpness = 10f;
        [SerializeField] private float groundDrag = 0.1f;
        [Header("Slide")]
        [SerializeField] private float slideSharpness = 10f;
        [SerializeField] private float slideBoost = 5f;
        [SerializeField] private float momentumMinimum = 0.01f;
        [SerializeField] private float mass = 10f;

        // Components
        private InputProvider inputProvider;
        public InputProvider InputProvider => inputProvider;
        private KinematicCharacterMotor motor;
        public KinematicCharacterMotor Motor => motor;

        // Orientation
        private Vector3 currentCharacterOrientation;
        public Vector3 CharacterOrientation => currentCharacterOrientation;

        private Vector3 forwardFromCamera;
        public Vector3 ForwardFromCamera => forwardFromCamera;
        
        // Movement
        private Vector3 momentum = Vector3.zero;

        private bool inputSliding = false;
        
        private MovementMode characterMovementMode = MovementMode.Airborn;
        public MovementMode CharacterMovementMode => characterMovementMode;

        public enum MovementMode
        {
            Slide,
            Walk,
            Airborn
        }
        
        private void Awake()
        {
            inputProvider = GetComponent<InputProvider>();
            motor = GetComponent<KinematicCharacterMotor>();
            motor.CharacterController = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            inputProvider.onCrouch += UpdateCrouch;
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void UpdateState()
        {
            if (motor.GroundingStatus.IsStableOnGround)
            {
                if (inputSliding)
                {
                    characterMovementMode = MovementMode.Slide;
                }
                else
                {
                    characterMovementMode = MovementMode.Walk;
                }
            }
            else
            {
                // Keep sliding airborn if started sliding
                if(characterMovementMode != MovementMode.Slide)
                    characterMovementMode = MovementMode.Airborn;
            }
        }

        private void UpdateCrouch(bool isCrouch)
        {
            inputSliding = isCrouch;
            
            // Force state update to update movement mode
            UpdateState();
            
            // On slide
            if (characterMovementMode == MovementMode.Slide)
            {
                momentum = motor.Velocity;
                // Add slide boost
                momentum += momentum.normalized * slideBoost;

                motor.BaseVelocity = Vector3.zero;
            }
        }

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            if (characterMovementMode == MovementMode.Slide)
                return;

            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(cameraTransform.rotation * Vector3.forward, motor.CharacterUp).normalized;
            if (cameraPlanarDirection.sqrMagnitude == 0f)
            {
                // Get Target Direction
                cameraPlanarDirection = Vector3.ProjectOnPlane(cameraTransform.rotation * Vector3.up, motor.CharacterUp).normalized;
            }
            
            Vector3 smoothedLookInputDirection = Vector3.Slerp(motor.CharacterForward, cameraPlanarDirection, 1 - Mathf.Exp(-orientationSharpness * deltaTime)).normalized;

            currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, motor.CharacterUp);

        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            UpdateState();
            
            Vector3 effectiveGroundNormal = motor.GroundingStatus.GroundNormal;
            
            // Adjust velocity to ground normal
            currentVelocity = motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocity.magnitude;
            
            // Round momentum to zero if too small
            if (momentum.magnitude < momentumMinimum)
            {
                momentum = Vector3.zero;
            }
            
            if (characterMovementMode == MovementMode.Slide) 
            {
                // calculate slope angle and make it a coefficient
                float slopeAngle = Vector3.Angle(Vector3.up, effectiveGroundNormal);
                float slopeSin = Mathf.Sin(Mathf.Deg2Rad * slopeAngle);
                
                // Make controller slide along slope
                Vector3 targetVelocity = mass * -gravity * slopeSin * -effectiveGroundNormal;
                
                // Redirect target velocity along ramp instead of being perpendicular to ramp
                targetVelocity = motor.GetDirectionTangentToSurface(targetVelocity, effectiveGroundNormal) * targetVelocity.magnitude;
                
                momentum = Vector3.Slerp(momentum, targetVelocity, 1f - Mathf.Exp(-slideSharpness * deltaTime));
                currentVelocity = momentum;
            } 
            else if(characterMovementMode == MovementMode.Walk)
            {
                momentum *= 1f / (1f + (groundDrag * deltaTime));
                // Get forward according to camera
                forwardFromCamera = cameraTransform.rotation * inputProvider.MoveDirectionV3;
                Vector3 inputRight = Vector3.Cross(forwardFromCamera, motor.CharacterUp);
                Vector3 characterOrientation = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * forwardFromCamera.magnitude;
             
                Vector3 targetVelocity = characterOrientation * groundedMoveSpeed;
                currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, 1f - Mathf.Exp(-walkSharpness * deltaTime));
            }
            
            if (characterMovementMode is MovementMode.Airborn or MovementMode.Slide)
            { 
                // Gravity
                if (!motor.GroundingStatus.IsStableOnGround)
                {
                    momentum += Vector3.down * (gravity * deltaTime);
                    currentVelocity = momentum;
                }
            }
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
        
        }

        public void PostGroundingUpdate(float deltaTime)
        {
        
        }

        public void AfterCharacterUpdate(float deltaTime)
        {
        
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            return true;
        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport)
        {
        
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition,
            Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        
        }
        
        #if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;
            
            Gizmos.color = Color.cyan;
            Vector3 effectiveGroundNormal = motor.GroundingStatus.GroundNormal;

            Vector3 tangent = motor.GetDirectionTangentToSurface(motor.Velocity, effectiveGroundNormal);
            Gizmos.DrawRay(transform.position, tangent);

            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position + Vector3.up * 2f, motor.Velocity);

        }
#endif
    }
}
