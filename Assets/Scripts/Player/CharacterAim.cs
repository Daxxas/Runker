using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAim : MonoBehaviour
{
    [SerializeField] private Transform aimTargetIK;
    [SerializeField] private Vector3 targetOffset;
    [SerializeField] private Vector3 rayStartPositionOffset;
    [SerializeField] private Camera camera;
    [SerializeField] private float aimTargetMaxDistance = 10f;
    [SerializeField] private float aimSharpness = 1f;
    [SerializeField] private LayerMask aimable;

    private Vector3 aimTargetPosition;
    
    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + rayStartPositionOffset  + targetOffset, camera.transform.forward, out hit, aimTargetMaxDistance, aimable))
        {
            aimTargetPosition = hit.point + targetOffset;
        }
        else
        {
            Vector3 cameraForward = camera.transform.forward * aimTargetMaxDistance;
            aimTargetPosition = transform.position + cameraForward + targetOffset;
        }

        aimTargetIK.position = Vector3.Slerp(aimTargetIK.position, aimTargetPosition, aimSharpness * Time.deltaTime);
    }
}
