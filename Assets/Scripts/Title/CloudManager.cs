using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    [SerializeField] GameObject[] clouds;

    void Start()
    {
        InvokeRepeating(nameof(InstantiateColouds), 1, 10);
    }

    private void InstantiateColouds()
    {
        int randomC = Random.Range(0, clouds.Length);
        GameObject cloud = Instantiate(clouds[randomC]);
    }
}
