using UnityEngine;

namespace Enemies
{
    public class GrapplePull : MonoBehaviour
    {
        [SerializeField] private Rigidbody rigidbody;
        
        public void Pull(Vector3 direction, float force)
        {
            rigidbody.AddForce(direction * force, ForceMode.Acceleration);
        }
    }
}