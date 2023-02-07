using KinematicCharacterController;
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
        [SerializeField] private float smoothAnimation = 1f;
        private Animator animator;
        private Vector2 direction = Vector2.zero;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            characterMotor = GetComponent<KinematicCharacterMotor>();
        }

        private void Start()
        {
            characterController.onJump += () => { animator.SetTrigger("onJump");};
            characterController.onEscape += (Vector2 direction) =>
            {
                animator.SetFloat("escapeDirectionX", direction.x);
                animator.SetFloat("escapeDirectionY", direction.y);
                animator.SetTrigger("onEscape");
            };
            // characterController.onLedgeClimb += () =>
            // {
            //     animator.SetTrigger("onLedgeClimb");
            // };
        }

        private void Update()
        {
            animator.SetFloat("speedMagnitude", characterController.Momentum.magnitude);
            animator.SetFloat("velocityVertical", characterController.Momentum.y);
            animator.SetFloat("velocityHorizontal", new Vector2(characterController.Momentum.x, characterController.Momentum.z).magnitude);
            animator.SetBool("isGrounded", characterMotor.GroundingStatus.IsStableOnGround);
            animator.SetBool("isWallRunning", characterController.ShouldWallRun);
            animator.SetBool("isWallRight", characterController.TouchingWall == CharacterController.TouchingWallState.Right);
            animator.SetBool("verticalWallrun", characterController.TouchingWall == CharacterController.TouchingWallState.Front);
            animator.SetBool("isGrounded", characterMotor.GroundingStatus.IsStableOnGround);

            direction = Vector2.Lerp(direction, characterController.InputProvider.MoveDirection, smoothAnimation * Time.deltaTime);

            animator.SetFloat("directionX", direction.x);
            animator.SetFloat("directionZ", direction.y);
            animator.SetBool("isSliding", characterController.CharacterMovementMode == CharacterController.MovementMode.Slide);
            animator.SetBool("isRunning", characterController.IsRunning);
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