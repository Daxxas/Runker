using System;
using KinematicCharacterController;
using UnityEditor;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(KinematicCharacterMotor))]
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimatorController : MonoBehaviour
    {
        private CharacterController characterController;
        private KinematicCharacterMotor characterMotor;
        private Animator animator;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            characterMotor = GetComponent<KinematicCharacterMotor>();
        }

        private void Update()
        {
            animator.SetFloat("speedMagnitude", characterMotor.Velocity.magnitude);
            
            animator.SetFloat("directionX", characterController.InputProvider.MoveDirection.x);
            animator.SetFloat("directionZ", characterController.InputProvider.MoveDirection.y);
            animator.SetBool("isSliding", characterController.CharacterMovementMode == CharacterController.MovementMode.Slide);
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;
            
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, characterController.ForwardFromCamera);
        }
    }
}