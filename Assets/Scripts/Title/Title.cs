using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Title : MonoBehaviour
{
    [SerializeField] GameObject startText;
    void Start()
    {
        // クリックしてスタートのアニメーション
        startText.transform.DOScale(1.3f, 1f)
                           .SetEase(Ease.OutBounce)
                           .SetLoops(-1, LoopType.Yoyo);
    }
}
