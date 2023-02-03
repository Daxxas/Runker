using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAim : MonoBehaviour
{
    [SerializeField] private Transform handIk;
    [SerializeField] private Animator animator;
    [SerializeField] private Camera camera;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnAnimatorIK(int layerIndex)
    {
        // TODO : Watch this : https://www.youtube.com/watch?v=Q56quIB2sOg&ab_channel=TheKiwiCoder
    }
}
