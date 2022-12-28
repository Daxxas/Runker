using System;
using Cinemachine;
using UnityEngine;

namespace Player
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private CinemachineFreeLook cinemachine;
        [SerializeField] private float scrollSensitivity = 1f;
        private void Start()
        {
            characterController.InputProvider.onScroll += UpdateCameraDistance;
        }

        private void UpdateCameraDistance(float delta)
        {
            Debug.Log("Update camera distance " + delta);
            for (var i = 0; i < cinemachine.m_Orbits.Length; i++)
            {
                cinemachine.m_Orbits[i].m_Radius += Mathf.Clamp(delta, -1, 1) * scrollSensitivity;
            }
        }
    }
}