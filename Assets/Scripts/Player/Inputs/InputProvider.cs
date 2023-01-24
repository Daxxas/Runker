using System;
using UnityEngine;

namespace Player.Inputs
{
    public abstract class InputProvider : MonoBehaviour
    {
        public Action<Vector2> onMove;
        public Action<bool> onCrouch;
        public Action<bool> onRun;
        public Action<bool> onJump;
        public Action<bool> onGrapple;
        public Action<float> onScroll;
        public Action<Vector2> onEscape;

        protected Vector2 moveDirection;
        protected  Vector3 moveDirectionV3;
        public Vector2 MoveDirection => moveDirection;
        public Vector3 MoveDirectionV3 => moveDirectionV3;

    }
}