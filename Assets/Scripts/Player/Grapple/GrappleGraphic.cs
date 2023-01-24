using System;
using UnityEngine;

namespace Player.Grapple
{
    public class GrappleGraphic : MonoBehaviour
    {
        [SerializeField] private GrappleController grappleController;
        private LineRenderer lineRenderer;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        private void Update()
        {
            if (grappleController.GrappleHit)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPositions(new []{grappleController.GrappleOrigin.position, grappleController.GrapplePoint});
            }
            else
            {
                lineRenderer.positionCount = 0;
            }
        }
    }
}