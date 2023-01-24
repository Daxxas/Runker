using System;
using UnityEngine;

namespace Player.Grapple
{
    public class GrappleGraphic : MonoBehaviour
    {
        [SerializeField] private GrappleThrower grappleThrower;
        private LineRenderer lineRenderer;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        private void Update()
        {
            if (grappleThrower.GrappleHit)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPositions(new []{grappleThrower.GrappleOrigin.position, grappleThrower.GrapplePoint});
            }
            else
            {
                lineRenderer.positionCount = 0;
            }
        }
    }
}