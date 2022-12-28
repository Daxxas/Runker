using System;
using KinematicCharacterController;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(KinematicCharacterMotor))]
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimatorController : MonoBehaviour
    {
        private CharacterController characterController;
        private KinematicCharacterMotor characterMotor;
        [SerializeField] private Transform camera;
        [SerializeField] private float smoothAnimation = 1f;
        private Animator animator;
        private Vector2 direction = Vector2.zero;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            characterMotor = GetComponent<KinematicCharacterMotor>();
        }

        private void Update()
        {
            animator.SetFloat("speedMagnitude", characterMotor.Velocity.magnitude);

            direction = Vector2.Lerp(direction, characterController.InputProvider.MoveDirection, smoothAnimation * Time.deltaTime);

            animator.SetFloat("directionX", direction.x);
            animator.SetFloat("directionZ", direction.y);
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