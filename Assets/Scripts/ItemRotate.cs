using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class ItemRotate : MonoBehaviour
{
    public float z = 0;
    public float rotateTime = 6f;
    void Start()
    {
        transform.DOLocalRotate(new Vector3(0, 360f, z), rotateTime, RotateMode.FastBeyond360)  
        .SetEase(Ease.Linear)  
        .SetLoops(-1, LoopType.Restart);  
    }
}
