using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    public static Sounds instance;
    void Awake()
    {
        instance = this;
    }
    public AudioSource[] se;
    public AudioSource[] bgm;
    public GameObject[] effects;
}
