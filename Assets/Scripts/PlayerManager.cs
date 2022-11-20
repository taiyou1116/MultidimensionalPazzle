using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
public class PlayerManager : MonoBehaviour
{
    private Animator animator;
    private float thinkingTime;
    public CinemachineVirtualCamera vc;
    public GameObject key;
    public int pickaxeCount{get; set;}
    public int stoneCount{get; set;}

    public Animator Anim {
        get {return animator;}
    }

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        vc = GetComponentInChildren<CinemachineVirtualCamera>();
    }
    public void Move(Vector3 position)
    {
        transform.DOPath(new Vector3[]{transform.position,position},0.5f,PathType.Linear);
    }
}
