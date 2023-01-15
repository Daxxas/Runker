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
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }
    
            Vector3 dir = target.position - transform.position;
            float distanceThisFrame = speed * Time.deltaTime;
    
            if (dir.magnitude <= distanceThisFrame)
            {
                // HitTarget();
                return;
            }
    
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * angularSpeed).eulerAngles;
            transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    
            transform.Translate(dir.normalized * distanceThisFrame, Space.World);
        }
    }
}