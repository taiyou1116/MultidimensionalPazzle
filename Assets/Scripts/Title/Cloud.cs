using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    // 発射地点はランダム4か所 奥,手前,上下


    // スタートポジション
    Vector3[] startPoss = {new Vector3(-100, 30, 75), new Vector3(-100, -20, 75),
                           new Vector3(-100, 30, -75), new Vector3(-100, -20, -75)};

    void Start()
    {
        int randomPos = Random.Range(0,startPoss.Length);
        transform.position = startPoss[randomPos];
    }

    void Update()
    {
        transform.Translate(new Vector3(0,0,1f * Time.deltaTime));
    }
}
