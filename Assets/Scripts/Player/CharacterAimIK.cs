using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using Player.Inputs;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CharacterAimIK : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Rig rig;
    [SerializeField] private CharacterShooter characterShooter;
    [SerializeField] private Animator animator;
    
    [SerializeField] private Transform aimTargetIK;
    [SerializeField] private Vector3 targetOffset;
    [SerializeField] private Vector3 rayStartPositionOffset;
    [SerializeField] private Camera camera;
    [SerializeField] private float aimTargetMaxDistance = 10f;
    [SerializeField] private float aimSharpness = 1f;
    [SerializeField] private float ikTransitionSharpness = 1f;
    [SerializeField] private LayerMask aimable;

    private Vector3 aimTargetPosition;
    private float targetRigWeight = 0f;
    
    private void Start()
    {
        characterShooter.onAim += SetEnableAimIK;
    }

    private void SetEnableAimIK(bool enable)
    {
        //rig.enabled = enable;
        targetRigWeight = enable ? 1f : 0f;
        animator.SetBool("isAiming", enable);
    }
    
    // Update is called once per frame
    void Update()
    {
        // Update aim target
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
        
        // Update rig weight 
        rig.weight = Mathf.Lerp(rig.weight, targetRigWeight, ikTransitionSharpness * Time.deltaTime);
    }
}
