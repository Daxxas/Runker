using System;
using Player.Inputs;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(InputProvider))]
    public class PhysicCharacterController : MonoBehaviour
    {
        [SerializeField] private float speed;
        
        private Rigidbody rigidbody;
        private InputProvider inputProvider;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            inputProvider = GetComponent<InputProvider>();
        }

        private void Update()
        {
            rigidbody.AddForce(inputProvider.MoveDirectionV3.normalized * (speed * Time.deltaTime), ForceMode.Acceleration);

        }
    }
}