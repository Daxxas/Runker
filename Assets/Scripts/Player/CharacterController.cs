using System;
using KinematicCharacterController;
using Player.Grapple;
using Player.Inputs;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    [RequireComponent(typeof(InputProvider))]
    [RequireComponent(typeof(KinematicCharacterMotor))]
    public class CharacterController : MonoBehaviour, ICharacterController
    {
        [Header("References")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private GrappleController grappleController;
        [Header("Parameters")]
        [Header("General")]
        [SerializeField] private float gravity = 30f;
        [SerializeField] private float coefficientOfRestitution = 0.5f;
        [SerializeField] private float momentumMinimum = 0.01f;
        [SerializeField] private float mass = 10f;
        [SerializeField] private float slideCapsuleHeight = 1f;
        [SerializeField] private float slideCapsuleOffset = .5f;
        [SerializeField] private float normalCapsuleHeight = 2f;
        [SerializeField] private float normalCapsuleOffset = 1f;
        [Header("Walk")]
        [SerializeField] private float groundedMoveSpeed = 5f;
        [SerializeField] private float orientationSharpness = 10f;
        [SerializeField] private float walkSharpness = 10f;
        [Header("Run")]
        [SerializeField] private float groundedRunSpeed = 7f;
        [SerializeField] private float runSharpness = 10f;
        // [SerializeField] private float groundDrag = 0.1f;
        [Header("Slide")]
        [SerializeField] private float slideSharpness = 10f;
        [SerializeField] private float slideBoost = 5f;
        [SerializeField] private float slideBoostMinimumHorizontalVelocity = 5f;
        [SerializeField] private float slideGroundTimeMinimum = 3f;
        [SerializeField] private float slideMass = 3f;
        [Header("Jump")] 
        [SerializeField] [Range(0f, 90f)] private float jumpAngle = 90f; 
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float jumpHoldGravityMultiplier  = 5f;
        [SerializeField] private float airDrag = 0.01f;
        [SerializeField] private float airControlForce = 0.01f;
        [SerializeField] private int aerialJumpMax = 1;
        [SerializeField] [Range(0f, 1f)] private float aerialJumpDirectionInfluence = 1f;
        [SerializeField] private float jumpBufferDuration = .2f;
        [Header("Wallrun")]
        [SerializeField] private float wallRunGravity = 9.81f;
        [SerializeField] private float wallRunSpeed = 7f;
        [SerializeField] private float wallRunYBoost = 2f;
        [SerializeField] private float wallRunMinimumHorizontalVelocity = 0.1f;
        [SerializeField] private float wallRunReleaseVelocity = 2f;
        [SerializeField] private float wallRunHoldDuration = 3f;
        [SerializeField] private float wallRunCooldown = 3f;
        [SerializeField] private float wallRunDrag = 0.1f;
        [SerializeField] [Range(80f,90f)] private float wallRunMinAngle = 88f;
        [SerializeField] private float wallRunDetectionDistance = 0.2f;
        [SerializeField] private float wallRunGripStrength = 2f;
        [SerializeField] private float wallRotationSharpness = 2f;
        [Header("Walljump")]
        [SerializeField] private float wallJumpVelocity = 2f;
        [SerializeField] private float wallJumpForwardBoost = 2f;
        [SerializeField] [Range(0f, 90f)] private float wallJumpAngle = 2f;
        [SerializeField] private int wallJumpDisabledFrames = 5;
        [Header("Escape")] 
        [SerializeField] private float escapeVelocity = 5f;
        [SerializeField] private float escapeCooldown = 1f;
        
        // TODO : Implement coyote time
        [Header("Coyote")] 
        [SerializeField] private float coyoteTime = 0.2f;
        // Events for external uses
        public Action onJump;
        public Action onLand;
        public Action onSlide;
        public Action onLedgeClimb;
        public Action<Vector2> onEscape;
        
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
        public Vector2 HorizontalVelocity => new Vector2(motor.Velocity.x, motor.Velocity.z);
        private Vector3 momentum = Vector3.zero;

        private float groundTime = 0f;
        private bool isRunning = false;

        public bool IsRunning => isRunning;
        private bool inputSliding = false;
        private bool jumpRequest = false;
        private bool jumpHold  = false;
        private float lastJumpBufferTime = 0f;
        private int aerialJumpCount = 0;
        private bool JumpBufferIsValid => Time.time < lastJumpBufferTime + jumpBufferDuration;
        
        // Escape
        private float lastEscapeTime;
        private Vector2 lastEscapeDirection;
        
        // Wall Jump
        private int wallJumpFrameCount = 0;
        private bool wallJumpPreventingWallRun => wallJumpFrameCount < wallJumpDisabledFrames;

        // Wallrun
        RaycastHit wallHit;
        public RaycastHit WallHit => wallHit;
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
            Grounded,
            Airborn,
            Wallrun,
            Grapple
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
            inputProvider.onRun += UpdateRun;
            inputProvider.onJump += PerformJump;
            inputProvider.onEscape += PerformEscape;
            grappleController.onGrapple += () =>
            {
                motor.ForceUnground();

                if (momentum.y < 0)
                    momentum.y = 0;
            };
        }

        private void Update()
        {
            if (characterMovementMode == MovementMode.Slide)
            {
                motor.SetCapsuleDimensions(motor.Capsule.radius, slideCapsuleHeight, slideCapsuleOffset);
            }
            else
            {
                motor.SetCapsuleDimensions(motor.Capsule.radius, normalCapsuleHeight, normalCapsuleOffset);
            }
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            UpdateState();
            
            Vector3 effectiveGroundNormal = motor.GroundingStatus.GroundNormal;
            
            // Adjust velocity to ground normal
            currentVelocity = motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * HorizontalVelocity.magnitude;
            
            // Round momentum to zero if too small
            if (momentum.magnitude < momentumMinimum)
            {
                momentum = Vector3.zero;
            }
            // Get forward according to camera
            Vector3 correctedInput = GetInputOrientationAccordingToCharacterForward(inputProvider.MoveDirectionV3);
            
            if (characterMovementMode == MovementMode.Slide) 
            {
                groundTime = 0f;

                // calculate slope angle and make it a coefficient
                float slopeAngle = Vector3.Angle(Vector3.up, effectiveGroundNormal);
                float slopeSin = Mathf.Sin(Mathf.Deg2Rad * slopeAngle);
                
                // Make controller slide along slope
                Vector3 targetVelocity = mass * -gravity * slopeSin * -effectiveGroundNormal;
                
                // Redirect target velocity along ramp instead of being perpendicular to ramp
                targetVelocity = motor.GetDirectionTangentToSurface(targetVelocity, effectiveGroundNormal) * targetVelocity.magnitude;

                // momentum = targetVelocity.normalized * momentum.magnitude;
                
                momentum = Vector3.Slerp(momentum, targetVelocity, 1f - Mathf.Exp(-slideSharpness * deltaTime));

                currentVelocity = momentum;
            } 
            
            if(characterMovementMode == MovementMode.Grounded)
            {
                groundTime += deltaTime;
                // Everything is momentum, even walking
                float targetSpeed = !isRunning ? groundedMoveSpeed : groundedRunSpeed;
                float lerpSharpness = !isRunning ? walkSharpness : runSharpness;
                Vector3 targetVelocity = correctedInput * targetSpeed;
                momentum = Vector3.Lerp(momentum, targetVelocity, 1f - Mathf.Exp(-lerpSharpness * deltaTime));
                currentVelocity = momentum;
            }
            
            if (characterMovementMode is MovementMode.Airborn or MovementMode.Slide) // Can slide and still be in the air
            {
                // Gravity
                if (!motor.GroundingStatus.IsStableOnGround)
                {
                    float adaptedGravity = gravity;
                    float adaptedMass = mass;
                    if (!jumpHold && momentum.y > 0f)
                    {
                        adaptedGravity = gravity * jumpHoldGravityMultiplier;
                    }

                    if (characterMovementMode == MovementMode.Slide)
                    {
                        adaptedMass = slideMass;
                    }
                    
                    momentum += Vector3.down * (adaptedGravity * adaptedMass * deltaTime);
                    
                    // Air control
                    Vector3 airControl = forwardFromCamera * (airControlForce * deltaTime);
                    float momentumMagnitude = momentum.magnitude; // Save magnitude to restore it later
                    momentum += airControl; 
                    momentum = momentum.normalized * momentumMagnitude; // Restore magnitude after adding air control to avoid air control creating momentum

                    momentum *= 1f / (1f + (airDrag * deltaTime));
                    currentVelocity = momentum;
                }
            }

            if (characterMovementMode is MovementMode.Wallrun)
            {
                groundTime += deltaTime;

                momentum *= 1f / (1f + (wallRunDrag * deltaTime));
                momentum.y += -wallRunGravity * mass * deltaTime; // Apply gravity
                
                // Calculate wallrun direction
                Vector3 wallRunDirection = motor.CharacterForward;
                Vector3 targetVelocity = wallRunDirection * (wallRunSpeed * inputProvider.MoveDirection.y);
                Debug.DrawRay(transform.position - Vector3.up * 0.2f, targetVelocity, Color.red);
                currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, 1f - Mathf.Exp(-walkSharpness * deltaTime));
                currentVelocity += momentum;
                if (!wallJumpPreventingWallRun)
                {
                    currentVelocity += wallRunGripStrength * -wallHit.normal; // Apply grip velocity to stick on wall
                }
            }

            if (characterMovementMode is MovementMode.Grapple)
            {
                momentum += (grappleController.GrapplePoint - motor.transform.position).normalized * (grappleController.GrappleAcceleration * deltaTime);
                
                Vector3 grappleAirControlAcceleration = correctedInput * grappleController.GrappleAirControl;
                momentum += grappleAirControlAcceleration;

                
                currentVelocity = momentum;
            }
            
            // Jump handling 
            if (jumpRequest && JumpBufferIsValid && (motor.GroundingStatus.IsStableOnGround || aerialJumpCount < aerialJumpMax))
            {
                if (!motor.GroundingStatus.IsStableOnGround)
                {
                    momentum = momentum * (1f - aerialJumpDirectionInfluence) + (aerialJumpDirectionInfluence * momentum.magnitude * motor.CharacterForward);
                    aerialJumpCount++;
                }
                
                jumpRequest = false;
                onJump?.Invoke();
                float angleCoef = jumpAngle / 90f;
                Vector3 jumpDirectionFromGround = Vector3.up;
                // We want to be jump perpendicular to the ground when sliding
                if (characterMovementMode == MovementMode.Slide)
                {
                    jumpDirectionFromGround = (motor.CharacterForward * (inputProvider.MoveDirection.y * (1-angleCoef)) + effectiveGroundNormal * angleCoef).normalized;
                }
                // Redirect momentum to be along ground in case jump is requested mid air (the momentum would going downward otherwise, and adding the jump momentum make the jump weird)
                momentum = motor.GetDirectionTangentToSurface(momentum, effectiveGroundNormal) * momentum.magnitude;
                momentum += jumpDirectionFromGround.normalized * jumpForce;
                motor.ForceUnground();
                currentVelocity = momentum; 
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

        public void BeforeCharacterUpdate(float deltaTime)
        {
            if (wallJumpPreventingWallRun)
            {
                wallJumpFrameCount++;
            }
            
            if (motor.GroundingStatus.IsStableOnGround)
                hasTouchedGroundSinceWallrun = true;
            
            
            // Ray casting for walls
            
            // Lot of copy paste here :(
            Ray toWallRightRay = new Ray()
            {
                origin = motor.TransientPosition + Vector3.up * (motor.Capsule.height / 2),
                direction = motor.CharacterRight
            };
            Ray toWallRightForwardRay = new Ray()
            {
                origin = motor.TransientPosition + Vector3.up * (motor.Capsule.height / 2),
                direction = motor.CharacterRight + motor.CharacterForward * 0.7f
            };
            
            Ray toWallLeftRay = new Ray()
            {
                origin = motor.TransientPosition + Vector3.up * (motor.Capsule.height / 2),
                direction = -motor.CharacterRight
            };
            Ray toWallLeftForwardRay = new Ray()
            {
                origin = motor.TransientPosition + Vector3.up * (motor.Capsule.height / 2),
                direction = -motor.CharacterRight + motor.CharacterForward * 0.7f
            };
            
            Debug.DrawRay(toWallRightRay.origin, toWallRightRay.direction, Color.magenta);
            Debug.DrawRay(toWallRightForwardRay.origin, toWallRightForwardRay.direction, Color.magenta);
            Debug.DrawRay(toWallLeftRay.origin, toWallLeftRay.direction, Color.magenta);
            Debug.DrawRay(toWallLeftForwardRay.origin, toWallLeftForwardRay.direction, Color.magenta);
            
            bool leftWall = Physics.Raycast(toWallLeftRay, out RaycastHit leftHit, motor.Capsule.radius + wallRunDetectionDistance);
            bool leftForwardWall = Physics.Raycast(toWallLeftForwardRay, out RaycastHit leftForwardHit, motor.Capsule.radius + wallRunDetectionDistance);
            bool rightWall = Physics.Raycast(toWallRightRay, out RaycastHit rightHit, motor.Capsule.radius + wallRunDetectionDistance);
            bool rightForwardWall = Physics.Raycast(toWallRightForwardRay, out RaycastHit rightForwardHit, motor.Capsule.radius + wallRunDetectionDistance);
            
            if (leftWall && Vector3.Angle(leftHit.normal, Vector3.up) >= wallRunMinAngle)
            {
                wallHit = leftHit;
                touchingWall = TouchingWallState.Left;
            }
            else if (leftForwardWall && Vector3.Angle(leftForwardHit.normal, Vector3.up) >= wallRunMinAngle)
            {
                wallHit = leftForwardHit;
                touchingWall = TouchingWallState.Left;
            }
            else if (rightWall && Vector3.Angle(rightHit.normal, Vector3.up) >= wallRunMinAngle)
            {
                wallHit = rightHit;
                touchingWall = TouchingWallState.Right;
            }
            else if (rightForwardWall && Vector3.Angle(rightForwardHit.normal, Vector3.up) >= wallRunMinAngle)
            {
                wallHit = rightForwardHit;
                touchingWall = TouchingWallState.Right;
            }
            else
            {
                touchingWall = TouchingWallState.None;
            }

            // Edge case if try to wallrun on a corner
            if (leftForwardWall && rightForwardWall)
            {
                touchingWall = TouchingWallState.None;
            }

            bool wallRunOnCooldown = Time.time < wallRunCooldownTime + wallRunCooldown;
            bool canHoldOnWall = Time.time < wallRunStartTime + wallRunHoldDuration;
            
            if (touchingWall != TouchingWallState.None && // if touching wall
                !motor.GroundingStatus.IsStableOnGround && // if not grounded
                (!wallRunOnCooldown || hasTouchedGroundSinceWallrun) && // if not on cooldown
                (canHoldOnWall || characterMovementMode != MovementMode.Wallrun) && // if can still hold the wall run
                !wallJumpPreventingWallRun) 
            {
                shouldWallRun = true;
                // if wall run just started
                if(previousTouchWall != touchingWall || characterMovementMode == MovementMode.Airborn)
                {
                    onWallRunTouch?.Invoke();
                    WallRunStart();
                }
            }
            else 
            {
                shouldWallRun = false;
            }
            
            // Inputs according to character forward
            Vector3 forwardFromCharacter = transform.rotation * inputProvider.MoveDirectionV3;
            Vector3 inputRight = Vector3.Cross(forwardFromCharacter, motor.CharacterUp);
            Vector3 inputDirection = Vector3.Cross(motor.GroundingStatus.GroundNormal, inputRight).normalized;
            bool inputAwayFromWall = (inputDirection - wallHit.normal).magnitude < 1f;
            
            if(characterMovementMode == MovementMode.Wallrun && !wallJumpPreventingWallRun && (!canHoldOnWall || inputAwayFromWall || !shouldWallRun ||
                   (jumpRequest && JumpBufferIsValid)))
            {
                onWallRunRelease?.Invoke();
                WallRunEnd();
            }

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
            // Debug.Log(motor.Velocity);
            aerialJumpCount = 0;
        }
        
        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport)
        {
            // Register wall hit for wall run
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
                
            // Check for ledge climb
            bool footWall = Physics.Raycast(transform.position, motor.CharacterForward, out RaycastHit footHit, motor.Capsule.radius + wallRunDetectionDistance);
            bool headWall = Physics.Raycast(transform.position + Vector3.up * motor.Capsule.height, motor.CharacterForward, out RaycastHit headHit, motor.Capsule.radius + wallRunDetectionDistance);

            if (footWall && !headWall)
            {
                // Ledge grab is just momentum boost on Y ?
                onLedgeClimb?.Invoke();
                momentum = motor.CharacterForward * 10f;
                momentum.y = 10f;
            }
            
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition,
            Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {

        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
            
        }

        private void UpdateState()
        {
            if(grappleController.GrappleHit)
            {
                characterMovementMode = MovementMode.Grapple;
                return;
            }
            
            if (shouldWallRun)
            {
                characterMovementMode = MovementMode.Wallrun;
                return;
            }
            
            if (inputSliding && motor.GroundingStatus.IsStableOnGround)
            {
                // If we were airborn, it means we are landing
                if(characterMovementMode == MovementMode.Airborn) 
                    onLand?.Invoke();

                if (characterMovementMode != MovementMode.Slide)
                {
                    onSlide?.Invoke();
                    SlideStart();
                }
                
                characterMovementMode = MovementMode.Slide;
                return;
            }
            
            if (motor.GroundingStatus.IsStableOnGround)
            {
                // TODO : Rework the if to avoid copy/paste code
                // If we were airborn, it means we are landing
                if(characterMovementMode == MovementMode.Airborn) 
                    onLand?.Invoke();
                
                characterMovementMode = MovementMode.Grounded;
            }
            else
            {
                characterMovementMode = MovementMode.Airborn;
            }
        }

        private void PerformEscape(Vector2 direction)
        {
            if (Time.time < lastEscapeTime + escapeCooldown)
                return;

            if (characterMovementMode == MovementMode.Wallrun)
                return;

            if (motor.GroundingStatus.IsStableOnGround && direction.y != 0)
                return;
            
            Vector3 correctedDirection = new Vector3(direction.x, 0, direction.y);
            correctedDirection = GetInputOrientationAccordingToCharacterForward(correctedDirection);
            momentum = correctedDirection * escapeVelocity;
            lastEscapeTime = Time.time;
            lastEscapeDirection = direction;
            onEscape?.Invoke(lastEscapeDirection);
        }
        
        private void PerformJump(bool jumpPressed)
        {
            if (jumpPressed)
            {
                lastJumpBufferTime = Time.time;
                jumpRequest = true;
                jumpHold = true;
            }
            else
            {
                jumpHold = false;
            }
        }

        private void UpdateRun(bool isRun)
        {
            isRunning = isRun;
        }

        private void SlideStart()
        {
            // Redirect momentum along tangent to avoid momentum dragging player down when falling off a slope
            momentum = motor.GetDirectionTangentToSurface(momentum, motor.GroundingStatus.GroundNormal) * momentum.magnitude;
        }

        private void UpdateCrouch(bool isCrouch)
        {
            inputSliding = isCrouch; // Save input for controller
            UpdateState(); // Force state update to update movement mode
            
            // On slide grounded
            if (characterMovementMode == MovementMode.Slide && motor.GroundingStatus.IsStableOnGround) 
            {

                // Debug.Log("Slide ! " + HorizontalVelocity.magnitude + " > " + slideBoostMinimumHorizontalVelocity);
                momentum = motor.Velocity; // Transfer velocity to momentum when sliding
                if (HorizontalVelocity.magnitude > slideBoostMinimumHorizontalVelocity && groundTime > slideGroundTimeMinimum)
                {
                    // Debug.Log("Boost !");
                    momentum += momentum.normalized * slideBoost; // Add slide boost
                }
                motor.BaseVelocity = Vector3.zero; // Velocity has been transfered to momentum
                groundTime = 0f;
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
            aerialJumpCount = 0; // Reset aerials after wallrun
            wallJumpFrameCount = 0; // Reset walljump frame count
            hasTouchedGroundSinceWallrun = false; 
            momentum = motor.Velocity; // Keep wall momentum when releasing wallrun
            if (jumpRequest)
            {
                jumpRequest = false;
                wallRunCooldownTime = 0f; // Don't put wallrun on cooldown
                
                float wallJumpAngleCoef = wallJumpAngle / 90f; // Calculate walljump angle coefficient
                Vector3 jumpDirectionFromWall = (motor.CharacterUp * (1-wallJumpAngleCoef) + wallHit.normal * wallJumpAngleCoef).normalized; // Calculate walljump direction with coeffecient
                momentum += jumpDirectionFromWall * wallJumpVelocity; // Apply walljump velocity
                momentum += motor.CharacterForward * wallJumpForwardBoost; // Add forward boost

            }
            else
            {
                wallRunCooldownTime = Time.time; // Put wallrun on cooldown
                momentum += wallHit.normal * wallRunReleaseVelocity;
            }
        }
        
        private Vector3 GetInputOrientationAccordingToCharacterForward(Vector3 inputDirection)
        {
            forwardFromCamera = cameraTransform.rotation * inputDirection;
            Vector3 inputRight = Vector3.Cross(forwardFromCamera, motor.CharacterUp);
            Vector3 correctedInput = Vector3.Cross(motor.GroundingStatus.GroundNormal, inputRight).normalized * forwardFromCamera.magnitude;
            return correctedInput;
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
