using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    [SerializeField] AudioSource[] pickaxese;
    public void PickaxeSE()
    {
        int randomCnt = Random.Range(0,2);
        pickaxese[randomCnt].Play();
    }
}
