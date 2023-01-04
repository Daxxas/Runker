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
        [SerializeField] private float coefficientOfRestitution = 0.5f;
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
        [Header("Jump")] 
        [SerializeField] [Range(0f, 90f)] private float jumpAngle = 90f; 
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float jumpHoldGravityMultiplier  = 5f;
        [SerializeField] private float airDrag = 0.01f;
        [SerializeField] private float airControlForce = 0.01f;
        [Header("Wallrun")]
        [SerializeField] private float wallRunGravity = 9.81f;
        [SerializeField] private float wallRunSpeed = 7f;
        [SerializeField] private float wallRunYBoost = 2f;
        [SerializeField] private float wallRunReleaseVelocity = 2f;
        [SerializeField] private float wallJumpVelocity = 2f;
        [SerializeField] [Range(0f, 90f)] private float wallJumpAngle = 2f;
        [SerializeField] private float wallRunHoldDuration = 3f;
        [SerializeField] private float wallRunCooldown = 3f;
        [SerializeField] private float wallRunDrag = 0.1f;
        [SerializeField] [Range(80f,90f)] private float wallRunMinAngle = 88f;
        [SerializeField] private float wallRunDetectionDistance = 0.2f;
        [SerializeField] private float wallRunGripStrength = 2f;
        [SerializeField] private float wallRotationSharpness = 2f;
        
        // Events for external uses
        public Action onJump;
        
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
        private bool jumpRequest = false;
        private bool jumpHold  = false;
        
        // Wallrun
        RaycastHit wallHit;
        private TouchingWallState touchingWall = TouchingWallState.None;

        public TouchingWallState TouchingWall => touchingWall;
        private TouchingWallState previousTouchWall = TouchingWallState.None;
        private Vector3 previousVelocity = Vector3.zero;
        private float wallRunStartTime = 0f;
        private float wallRunCooldownTime = 0f;
        private bool hasTouchedGroundSinceWallrun = false;
        public Action onWallRunTouch;
        public Action onWallRunRelease;
        private bool shouldWallRun = false;
        public bool ShouldWallRun => shouldWallRun;


        private MovementMode characterMovementMode = MovementMode.Airborn;
        public MovementMode CharacterMovementMode => characterMovementMode;

        public enum MovementMode
        {
            Slide,
            Walk,
            Airborn,
            Wallrun
        }
        
        public enum TouchingWallState
        {
            None,
            Left,
            Right
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
            inputProvider.onJump += PerformJump;
        }
        
        private void UpdateState()
        {
            if (shouldWallRun)
            {
                characterMovementMode = MovementMode.Wallrun;
                return;
            }
            
            if (inputSliding && motor.GroundingStatus.IsStableOnGround)
            {
                characterMovementMode = MovementMode.Slide;
                return;
            }
            
            if (motor.GroundingStatus.IsStableOnGround)
            {
                characterMovementMode = MovementMode.Walk;
            }
            else
            {
                characterMovementMode = MovementMode.Airborn;
            }
        }

        private void PerformJump(bool jumpPressed)
        {
            if (jumpPressed)
            {
                jumpRequest = true;
                jumpHold = true;
            }
            else
            {
                jumpHold = false;
            }
        }

        private void UpdateCrouch(bool isCrouch)
        {
            inputSliding = isCrouch; // Save input for controller
            UpdateState(); // Force state update to update movement mode
            
            // On slide grounded
            if (characterMovementMode == MovementMode.Slide && motor.GroundingStatus.IsStableOnGround) 
            {
                momentum = motor.Velocity; // Transfer velocity to momentum when sliding
                motor.BaseVelocity = Vector3.zero;
                // TODO : No boost when coming from air + CD on boost
                momentum += momentum.normalized * slideBoost; // Add slide boost
            }
        }

        private void WallRunStart()
        {
            wallRunStartTime = Time.time; // Used to calculate wallrun hold duration
            Vector3 velocityDirection = motor.GetDirectionTangentToSurface(motor.Velocity, motor.GroundingStatus.GroundNormal); // Get Velocity direction tangent to ground
            Vector3 wallRunDirection = Vector3.ProjectOnPlane(velocityDirection, wallHit.normal); // Apply velocity direction to wall
            momentum = wallRunDirection.normalized; // Kill momentum when wallrunning
            momentum.y = Mathf.Max(momentum.y, wallRunYBoost); // Boost Y velocity at start of wallrun
        }

        private void WallRunEnd()
        {
            hasTouchedGroundSinceWallrun = false; 
            momentum = motor.Velocity; // Keep wall momentum when releasing wallrun
            if (jumpRequest) 
            {
                jumpRequest = false;
                wallRunCooldownTime = 0f; // Don't put wallrun on cooldown
                
                float wallJumpAngleCoef = wallJumpAngle / 90f; // Calculate walljump angle coefficient
                Vector3 jumpDirectionFromWall = (motor.CharacterUp * (1-wallJumpAngleCoef) + wallHit.normal * wallJumpAngleCoef).normalized; // Calculate walljump direction with coeffecient
                momentum = jumpDirectionFromWall * wallJumpVelocity; // Apply walljump velocity
                
            }
            else
            {
                wallRunCooldownTime = Time.time; // Put wallrun on cooldown
                momentum += wallHit.normal * wallRunReleaseVelocity;
            }
        }

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            // Don't update rotation when sliding
            if (characterMovementMode == MovementMode.Slide && motor.GroundingStatus.IsStableOnGround)
                return;

            if (characterMovementMode != MovementMode.Wallrun)
            {
                Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(cameraTransform.rotation * Vector3.forward, motor.CharacterUp).normalized;
                if (cameraPlanarDirection.sqrMagnitude == 0f)
                {
                    // Get Target Direction
                    cameraPlanarDirection = Vector3.ProjectOnPlane(cameraTransform.rotation * Vector3.up, motor.CharacterUp).normalized;
                }
            
                Vector3 smoothedLookInputDirection = Vector3.Slerp(motor.CharacterForward, cameraPlanarDirection, 1 - Mathf.Exp(-orientationSharpness * deltaTime)).normalized;

                currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, motor.CharacterUp);
            }
            else
            {
                Vector3 velocityDirection = motor.GetDirectionTangentToSurface(motor.Velocity, motor.GroundingStatus.GroundNormal);
                Vector3 wallrunDirection = Vector3.ProjectOnPlane(velocityDirection, wallHit.normal);
                wallrunDirection.y = 0;
                
                Quaternion targetRotation = Quaternion.LookRotation(wallrunDirection.normalized, motor.CharacterUp);
                currentRotation = Quaternion.Lerp(currentRotation, targetRotation, 1f - Mathf.Exp(-wallRotationSharpness * deltaTime));
            }
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
            
            // Get forward according to camera
            forwardFromCamera = cameraTransform.rotation * inputProvider.MoveDirectionV3;
            Vector3 inputRight = Vector3.Cross(forwardFromCamera, motor.CharacterUp);
            Vector3 characterOrientation = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * forwardFromCamera.magnitude;
            
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
            
            if(characterMovementMode == MovementMode.Walk)
            {
                momentum *= 1f / (1f + (groundDrag * deltaTime));
             
                currentVelocity += momentum;
                Vector3 targetVelocity = characterOrientation * groundedMoveSpeed;
                currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, 1f - Mathf.Exp(-walkSharpness * deltaTime));
            }
            
            if (characterMovementMode is MovementMode.Airborn or MovementMode.Slide) // Can slide and still be in the air
            { 
                // Gravity
                if (!motor.GroundingStatus.IsStableOnGround)
                {
                    float adaptedGravity = gravity;
                    if (!jumpHold && momentum.y > 0f)
                    {
                        adaptedGravity = gravity * jumpHoldGravityMultiplier;
                    }
                    
                    momentum += Vector3.down * (adaptedGravity * mass * deltaTime);
                    momentum += forwardFromCamera * (airControlForce * deltaTime);
                    currentVelocity = momentum;
                }
            }

            if (characterMovementMode is MovementMode.Wallrun)
            {
                momentum *= 1f / (1f + (wallRunDrag * deltaTime));
                momentum.y += -wallRunGravity * mass * deltaTime; // Apply gravity
                
                // Calculate wallrun direction
                Vector3 wallRunDirection = motor.CharacterForward;

                Vector3 targetVelocity = wallRunDirection * (wallRunSpeed * inputProvider.MoveDirection.y);
                Debug.DrawRay(transform.position - Vector3.up * 0.2f, targetVelocity, Color.red);
                currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, 1f - Mathf.Exp(-walkSharpness * deltaTime));
                currentVelocity += wallRunGripStrength * -wallHit.normal; // Apply grip velocity to stick on wall
                currentVelocity += momentum;
            }
            
            // Jump handling when grounded
            if (jumpRequest && motor.GroundingStatus.IsStableOnGround)
            {
                jumpRequest = false;
                onJump?.Invoke();
                float angleCoef = jumpAngle / 90f;
                Vector3 jumpDirectionFromGround = (motor.CharacterForward * (inputProvider.MoveDirection.y * (1-angleCoef)) + effectiveGroundNormal * angleCoef).normalized;
                // Redirect momentum to be along ground in case jump is requested mid air (the momentum would going downward otherwise, and adding the jump momentum make the jump weird)
                momentum = motor.GetDirectionTangentToSurface(momentum, effectiveGroundNormal) * momentum.magnitude;
                momentum += jumpDirectionFromGround.normalized * jumpForce;
                motor.ForceUnground();
                currentVelocity = momentum; 
            }
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {

            if (motor.GroundingStatus.IsStableOnGround)
                hasTouchedGroundSinceWallrun = true;
            
            Ray toWallRightRay = new Ray()
            {
                origin = motor.TransientPosition,
                direction = motor.CharacterRight
            };
                
            Ray toWallLeftRay = new Ray()
            {
                origin = motor.TransientPosition,
                direction = -motor.CharacterRight
            };


            // if wall run on cooldown
            bool leftWall = Physics.Raycast(toWallLeftRay, out RaycastHit leftHit, motor.Capsule.radius + wallRunDetectionDistance);
            bool rightWall = Physics.Raycast(toWallRightRay, out RaycastHit rightHit, motor.Capsule.radius + wallRunDetectionDistance);
            
            if (leftWall)
            {
                wallHit = leftHit;
                if (Vector3.Angle(wallHit.normal, Vector3.up) >= wallRunMinAngle)
                {
                    touchingWall = TouchingWallState.Left;
                }
                else
                {
                    touchingWall = TouchingWallState.None;
                }
            }
            else if (rightWall)
            {
                wallHit = rightHit;
                if (Vector3.Angle(wallHit.normal, Vector3.up) >= wallRunMinAngle)
                {
                    touchingWall = TouchingWallState.Right;
                }
                else
                {
                    touchingWall = TouchingWallState.None;
                }
            }
            else
            {
                touchingWall = TouchingWallState.None;
            }


            bool wallRunOnCooldown = Time.time < wallRunCooldownTime + wallRunCooldown;
            bool canHoldOnWall = Time.time < wallRunStartTime + wallRunHoldDuration;

            
            if (touchingWall != TouchingWallState.None && // if touching wall
                !motor.GroundingStatus.IsStableOnGround && // if not grounded
                (!wallRunOnCooldown || hasTouchedGroundSinceWallrun) && // if not on cooldown
                (canHoldOnWall || characterMovementMode != MovementMode.Wallrun)) // if can still hold the wall run
            {
                shouldWallRun = true;
                // if wall run just started
                if(previousTouchWall != touchingWall || characterMovementMode == MovementMode.Airborn)
                {
                    onWallRunTouch?.Invoke();
                    WallRunStart();
                    Debug.Log((touchingWall != TouchingWallState.None) + " " + (!motor.GroundingStatus.IsStableOnGround) + " " + (!wallRunOnCooldown) + " " + (canHoldOnWall || characterMovementMode != MovementMode.Wallrun));

                }
            }
            else 
            {
                shouldWallRun = false;
            }
            
            if(characterMovementMode == MovementMode.Wallrun && ((!canHoldOnWall) || jumpRequest))
            {
                Debug.Log("Release");
                onWallRunRelease?.Invoke();
                WallRunEnd();
            }

            Debug.DrawRay(toWallRightRay.origin, toWallRightRay.direction, Color.magenta);
            Debug.DrawRay(toWallLeftRay.origin, toWallLeftRay.direction, Color.magenta);
            previousVelocity = motor.Velocity;
            previousTouchWall = touchingWall;
        }

        
        public void AfterCharacterUpdate(float deltaTime)
        {
            
        }
        
        public void PostGroundingUpdate(float deltaTime)
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
            if (Motor.GroundingStatus.IsStableOnGround || hitStabilityReport.IsStable) return;

            float angle = Vector3.Angle(motor.CharacterUp, hitNormal);
                
            if (angle <= 90f && angle > motor.MaxStableSlopeAngle)
            {
                wallHit = new RaycastHit()
                {
                    normal = hitNormal,
                    point = hitPoint,
                };
            }
                
            // momentum = Vector3.ProjectOnPlane(momentum, hitNormalFlat).normalized * momentum.magnitude;
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
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position + Vector3.up * 2.1f, momentum);

        }
#endif
    }
}
