using System;
using Cinemachine;
using Player.Camera;
using UnityEngine;

namespace Player
{
    public class CameraController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterController characterController;
        [SerializeField] private CinemachineFreeLook cinemachine;
        [SerializeField] private CinemachineCameraOffset cinemachineCameraOffset;
        [SerializeField] private CinemachineRecomposer cinemachineRecomposer;

        [Header("Impulses")] 
        [SerializeField] private CameraTiltImpulse landImpulse;
        [SerializeField] private float landImpulseAmplitude = 1f;
        
        [Header("Offsets")]
        [SerializeField] private CameraOffset slideOffset;
        [SerializeField] private CameraOffset runOffset;
        [SerializeField] private CameraOffset wallrunOffset;
        
        private CameraOffset targetOffset;

        private void Start()
        {
            characterController.onLand += () =>
            {
                landImpulse.Impulse();
            };
        }
        Vector3 characterForwardUpdate;

        private void Update()
        {
            characterForwardUpdate = characterController.transform.forward;

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
                // Adjust camera rotation when wallrunning when it's not straight walls
                if (characterController.TouchingWall == CharacterController.TouchingWallState.Right)
                {
                    cinemachine.m_XAxis.Value += Vector3.Angle(characterForwardUpdate, characterController.transform.forward);
                }
                else
                {
                    cinemachine.m_XAxis.Value -= Vector3.Angle(characterForwardUpdate, characterController.transform.forward);
                }
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