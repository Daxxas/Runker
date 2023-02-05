using System;
using Cinemachine;
using Player.Camera;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class CameraController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterController characterController;
        [SerializeField] private CharacterShooter characterShooter;
        [SerializeField] private CinemachineFreeLook cinemachine;
        [SerializeField] private CinemachineCameraOffset cinemachineCameraOffset;
        [SerializeField] private CinemachineRecomposer cinemachineRecomposer;

        [Header("FoV")] 
        [SerializeField] private float minFov = 40f;
        [SerializeField] private float maxFov = 50f;
        [SerializeField] private float maxSpeedFov = 50f;
        [SerializeField] private float sharpnessFov = 1f;
        [SerializeField] private AnimationCurve interpolationFov;

        [Header("Wallrun")] 
        [SerializeField] private float maximumWallAngle = 30f;
        [SerializeField] private float wallDetectionDistance = 2f;
        [SerializeField] private LayerMask wallMask;
        [SerializeField] private float wallAdjustmentLerp = 5f;
        
        [Header("Tilt")] 
        [SerializeField] private float tiltPower = 1f;
        [SerializeField] private float tiltMax = 10f;
        [SerializeField] private float tiltSharpness = 1f;
        [SerializeField] private float velocityTiltMultiplier = 0.5f;
        
        [Header("Impulses")] 
        [SerializeField] private CameraTiltImpulse landImpulse;
        [SerializeField] private float landImpulseAmplitude = 1f;
        
        [Header("Offsets")]
        [SerializeField] private CameraOffset slideOffset;
        [SerializeField] private CameraOffset runOffset;
        [SerializeField] private CameraOffset wallrunOffset;
        [SerializeField] private CameraOffset verticalWallrunOffset;
        [SerializeField] private CameraOffset aimOffset;
        
        private CameraOffset targetOffset;
        private float targetTilt = 0;
        
        private CinemachineComposer[] composer = new CinemachineComposer[3];
        private float[] initialScreenX = new float[3];
        private float targetScreenX = 0f;
        
        private void Start()
        {
            for (int i = 0; i < 3; i++)
            {
                composer[i] = cinemachine.GetRig(i).GetCinemachineComponent<CinemachineComposer>();
                initialScreenX[i] = composer[i].m_ScreenX;
            }

            characterController.onWallRunStart += wallNormal =>
            {
                if (characterController.TouchingWall == CharacterController.TouchingWallState.Front)
                {
                    PlaceCameraUnderWall(wallNormal);
                } 
            };
        }
        Vector3 characterForwardUpdate;

        private void Update()
        {
            float targetFov = Mathf.Lerp(minFov, maxFov, interpolationFov.Evaluate(characterController.HorizontalVelocity.magnitude / maxSpeedFov));
            cinemachine.m_Lens.FieldOfView = Mathf.Lerp(cinemachine.m_Lens.FieldOfView, targetFov, Time.deltaTime * sharpnessFov);
            
            characterForwardUpdate = characterController.transform.forward;

            // targetTilt = Mathf.Clamp(-characterController.Motor.Velocity.y * velocityTiltMultiplier, -tiltMax, tiltMax) * tiltPower;
            // cinemachineRecomposer.m_Tilt = Mathf.Lerp(cinemachineRecomposer.m_Tilt, targetTilt, Time.deltaTime * tiltSharpness);

            if (characterShooter.IsAiming)
            {
                targetOffset = aimOffset;
            }
            else
            {
                if (characterController.CharacterMovementMode == CharacterController.MovementMode.Slide)
                {
                    targetOffset = slideOffset;
                }
                else if (characterController.CharacterMovementMode is CharacterController.MovementMode.Wallrun or CharacterController.MovementMode.Airborn)
                {
                    RaycastHit rightHit;
                    RaycastHit leftHit;
                    float dutchAmount = 0f;

                    targetOffset.position = runOffset.position;
                    targetOffset.dutch = wallrunOffset.dutch;
                    targetOffset.lerp = wallrunOffset.lerp;
                    
                    if (characterController.CharacterMovementMode == CharacterController.MovementMode.Wallrun)
                    {
                        targetOffset.position = wallrunOffset.position;
                    }
                    
                    if (Physics.Raycast(characterController.transform.position, characterController.transform.right, out rightHit,
                            wallDetectionDistance, wallMask))
                    {
                        dutchAmount = (1f - (rightHit.distance / wallDetectionDistance)) * targetOffset.dutch;
                    }
                    else if (Physics.Raycast(characterController.transform.position, -characterController.transform.right, out leftHit,
                        wallDetectionDistance, wallMask))
                    {
                        dutchAmount = (1f - (leftHit.distance / wallDetectionDistance)) * targetOffset.dutch; 
                        dutchAmount = -dutchAmount;
                    }
                    
                    targetOffset.dutch = dutchAmount;

                    if (characterController.TouchingWall is CharacterController.TouchingWallState.Right)
                    {
                        targetOffset.position.x = -targetOffset.position.x;
                    }
                    else if (characterController.TouchingWall is CharacterController.TouchingWallState.Front)
                    {
                        // Override offset in case of front wallrun
                        targetOffset = verticalWallrunOffset;
                    }
                }
                else
                {
                    targetOffset = runOffset;
                }
            }
            
            

            UpdateCameraOffset();
        }

        private void LateUpdate()
        {
            if (characterController.CharacterMovementMode == CharacterController.MovementMode.Wallrun)
            {
                float angleDelta = Vector3.SignedAngle(characterForwardUpdate, characterController.transform.forward, characterController.Motor.CharacterUp);

                float wallAngle = Vector3.Angle(characterController.WallHit.normal, characterController.transform.forward);

                wallAngle = Mathf.Abs(90f - wallAngle);
                
                if(wallAngle < maximumWallAngle)
                    cinemachine.m_XAxis.Value += angleDelta;
            }
        }

        private void UpdateCameraOffset()
        {
            Vector3 leftRayOrigin = characterController.transform.position + characterController.Motor.Capsule.center + (characterController.transform.right * characterController.Motor.Capsule.radius);
            Physics.Raycast(leftRayOrigin, characterController.transform.right, out var hit, wallDetectionDistance, wallMask);

            float hitRatio = 0f;
            if (hit.collider != null)
            {
                hitRatio += 1f - hit.distance / wallDetectionDistance;
            }

            Debug.Log(hitRatio);

            for (int i = 0; i < composer.Length; i++)
            {
                // Find target camera pos
                float mirrorScreenX = .5f - (initialScreenX[i] - .5f);
                targetScreenX = Mathf.Lerp(initialScreenX[i], mirrorScreenX, hitRatio);
                
                // Lerp to target pos to avoid sudden camera jumps
                composer[i].m_ScreenX = Mathf.Lerp(composer[i].m_ScreenX, targetScreenX, Time.deltaTime * wallAdjustmentLerp);
            }
            
            
            cinemachineCameraOffset.m_Offset = Vector3.Lerp(cinemachineCameraOffset.m_Offset, targetOffset.position, targetOffset.lerp * Time.deltaTime);
            cinemachineRecomposer.m_Dutch = Mathf.Lerp(cinemachineRecomposer.m_Dutch, targetOffset.dutch, targetOffset.lerp * Time.deltaTime);
        }

        private void PlaceCameraUnderWall(Vector3 wallNormal)
        {
            // TODO : Make this work
            // Vector3 wallUp = Vector3.Cross(characterController.transform.forward, wallNormal);
            
            // cinemachine.m_XAxis.Value = Vector3.SignedAngle(wallNormal, cinemachine.transform.forward, characterController.transform.forward);
            // cinemachine.m_YAxis.Value = 0f; // Vector3.SignedAngle(wallUp, wallNormal, Vector3.up);
        }
    }
}

[Serializable]
public struct CameraOffset
{
    public Vector3 position;
    public float lerp;
    public float dutch;
}