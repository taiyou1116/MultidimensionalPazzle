using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class EditOperate : MonoBehaviour
{
    public EditSound sound;
    [SerializeField] Camera camera2D;
    [SerializeField] CinemachineVirtualCamera[] virtualCamera;
    public int cameraNumber {get; private set;}
    private bool stage2D = false;

    public void Change()
    {
        if (stage2D)
        {
            camera2D.depth = -1;
            stage2D = false;
            
            sound.editAudios[5].Play();
            return;
        }
        if (!stage2D)
        {
            switch (cameraNumber) {
                case 0:
                    camera2D.transform.eulerAngles = new Vector3(90,0,0);
                break;
                case 1:
                    camera2D.transform.eulerAngles = new Vector3(90,-90,0);
                break;
                case 2:
                    camera2D.transform.eulerAngles = new Vector3(90,180,0);
                break;
                case 3:
                    camera2D.transform.eulerAngles = new Vector3(90,90,0);
                break;
            }
            camera2D.depth = 1;
            stage2D = true;
            
            sound.editAudios[4].Play();
        }
    }

    // 視点変更(右)
    public void ChangeCameraRight()
    {
        if (stage2D) return;
        foreach (var value in virtualCamera) {
            value.Priority = 0;//全てオフ
        }
        cameraNumber++;
        if (cameraNumber >= 4) {
            cameraNumber = 0;
        }
        virtualCamera[cameraNumber].Priority = 1;
        sound.editAudios[6].Play();
    }

    // 視点変更(右)
    public void ChangeCameraLeft()
    {
        if (stage2D) return;
        foreach (var value in virtualCamera) {
            value.Priority = 0;//全てオフ
        }
        cameraNumber--;
        if (cameraNumber <= -1) {
            cameraNumber = 3;
        }
        virtualCamera[cameraNumber].Priority = 1;
        sound.editAudios[6].Play();
    }
}
