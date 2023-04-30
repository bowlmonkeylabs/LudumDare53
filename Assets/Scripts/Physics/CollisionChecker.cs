using System.Collections.Generic;
using System.Linq;
using BML.ScriptableObjectCore.Scripts.Variables;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace BML.Scripts
{
    public class CollisionChecker : MonoBehaviour
    {
        [SerializeField] private FloatReference CollisionRadius;
        [SerializeField] private LayerMask CollisionMask;

        [SerializeField] private bool ignoreTriggers = true;
        [SerializeField] private bool oneCollisionPerObject = true;

        [SerializeField] private UnityEvent<List<Collider>> HandleCollisions;

        private Vector3 previousPosition;
        private List<GameObject> hitObjects = new List<GameObject>();

        private void OnEnable()
        {
            previousPosition = transform.position;
            hitObjects = new List<GameObject>();
        }

        private void Update()
        {
            CheckCollisions();
            previousPosition = transform.position;
        }

        private void CheckCollisions()
        {
            var triggerInteraction =
                ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.UseGlobal;
            List<Collider> colliderList =
                Physics.OverlapSphere(transform.position, CollisionRadius.Value, CollisionMask, triggerInteraction).ToList();

            RaycastHit[] hits = Physics.SphereCastAll(previousPosition, CollisionRadius.Value,
                (transform.position - previousPosition).normalized,
                Vector3.Distance(previousPosition, transform.position), CollisionMask,
                triggerInteraction);

            foreach (var hit in hits)
            {
                if (hitObjects.Contains(hit.collider.gameObject))
                    continue;
                
                colliderList.Add(hit.collider);
                hitObjects.Add(hit.collider.gameObject);
            }

            if (hits.Length > 0)
                HandleCollisions.Invoke(colliderList);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, CollisionRadius.Value);
        }
    }
}