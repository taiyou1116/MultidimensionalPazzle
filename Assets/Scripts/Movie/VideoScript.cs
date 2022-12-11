using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
public class VideoScript : MonoBehaviour
{
    private RawImage videoBase;
    private VideoPlayer videoPlayer;

    void Start()
    {
        videoBase = GetComponent<RawImage>();

        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.errorReceived += ErrorReceived;
        videoPlayer.prepareCompleted += PrepareCompleted;
        videoPlayer.Prepare();
        videoPlayer.targetCameraAlpha = 1;
        videoPlayer.isLooping = true;
    }

    // エラー発生時に呼ばれる
    private void ErrorReceived(VideoPlayer vp, string message)
    {
        Debug.Log("エラー発生");
        vp.errorReceived -= ErrorReceived;
        vp.prepareCompleted -= PrepareCompleted;
        Destroy(videoPlayer);
        vp = null;
    }

    //　動画の読み込みが完了したら呼ばれる
    void PrepareCompleted(VideoPlayer vp)
    {
        vp.prepareCompleted -= PrepareCompleted;
        // Debug.Log("ロード完了");
        vp.Play();
    }
}
