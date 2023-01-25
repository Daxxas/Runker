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
        
        private CameraOffset targetOffset;

        private float targetTilt = 0;
        
        private void Start()
        {
            // characterController.onLand += () =>
            // {
            //     landImpulse.Impulse();
            // };
        }
        Vector3 characterForwardUpdate;

        private void Update()
        {
            float targetFov = Mathf.Lerp(minFov, maxFov, interpolationFov.Evaluate(characterController.HorizontalVelocity.magnitude / maxSpeedFov));
            cinemachine.m_Lens.FieldOfView = Mathf.Lerp(cinemachine.m_Lens.FieldOfView, targetFov, Time.deltaTime * sharpnessFov);
            
            characterForwardUpdate = characterController.transform.forward;

            // targetTilt = Mathf.Clamp(-characterController.Motor.Velocity.y * velocityTiltMultiplier, -tiltMax, tiltMax) * tiltPower;
            // cinemachineRecomposer.m_Tilt = Mathf.Lerp(cinemachineRecomposer.m_Tilt, targetTilt, Time.deltaTime * tiltSharpness);

            switch (characterController.CharacterMovementMode)
            {
                case CharacterController.MovementMode.Slide:
                    targetOffset = slideOffset;
                    break;
                case CharacterController.MovementMode.Wallrun:
                    targetOffset = wallrunOffset;
                    Vector3 cameraForward = cinemachine.VirtualCameraGameObject.transform.forward;
                    if (characterController.TouchingWall == CharacterController.TouchingWallState.Right)
                    {
                        targetOffset.position.x = -targetOffset.position.x;
                    }
                    else
                    {
                        targetOffset.dutch = -targetOffset.dutch;
                    }
                    break;
                default:
                    targetOffset = runOffset;
                    break;
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
                Debug.Log(wallAngle);
                
                if(wallAngle < maximumWallAngle)
                    cinemachine.m_XAxis.Value += angleDelta;
            }
        }

        private void UpdateCameraOffset()
        {
            cinemachineCameraOffset.m_Offset = Vector3.Lerp(cinemachineCameraOffset.m_Offset, targetOffset.position, targetOffset.lerp * Time.deltaTime);
            cinemachineRecomposer.m_Dutch = Mathf.Lerp(cinemachineRecomposer.m_Dutch, targetOffset.dutch, targetOffset.lerp * Time.deltaTime);
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