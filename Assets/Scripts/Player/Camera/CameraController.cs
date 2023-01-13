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

        [FormerlySerializedAs("tiltVelocityCoef")]
        [FormerlySerializedAs("tiltYCoef")]
        [Header("Tilt")] 
        [SerializeField] private float tiltPower = 1f;
        [SerializeField] private float tiltMax = 10f;
        [SerializeField] private float tiltSharpness = 1f;
        
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
            characterForwardUpdate = characterController.transform.forward;

            targetTilt = Mathf.Clamp(-characterController.Motor.Velocity.y, -tiltMax, tiltMax) * tiltPower;
            cinemachineRecomposer.m_Tilt = Mathf.Lerp(cinemachineRecomposer.m_Tilt, targetTilt, Time.deltaTime * tiltSharpness);

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
                cinemachine.m_XAxis.Value += Vector3.SignedAngle(characterForwardUpdate, characterController.transform.forward, characterController.Motor.CharacterUp);
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