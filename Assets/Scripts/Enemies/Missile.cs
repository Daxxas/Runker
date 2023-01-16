using UnityEngine;

namespace Enemies
{
    public class Missile : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float speed = 70f;
        [SerializeField] private float angularSpeed = 10f;
        
        public void Seek(Transform target)
        {
            this.target = target;
        }
        
        // Update is called once per frame
        void Update()
        {
            Vector3 dir = target.position - transform.position;
            float distanceThisFrame = speed * Time.deltaTime;
    
            if (dir.magnitude <= distanceThisFrame)
            {
                // HitTarget();
                return;
            }
    
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * angularSpeed).eulerAngles;
            transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
    
            transform.Translate(transform.forward * distanceThisFrame, Space.World);
        }
    }
}