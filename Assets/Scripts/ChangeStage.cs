using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class ChangeStage : MonoBehaviour
{
    public List<GameObject> objs = new List<GameObject>();
    public List<MeshRenderer> meshs = new List<MeshRenderer>();
    private GameObject key;
    private Transform[] children;
    public List<GameObject> pickaxes;
    public List<Transform> pickaxeImage;
    public List<Transform> pickaxeObj;
    [SerializeField] Camera camera2D;
    public CinemachineVirtualCamera[] virtualCamera;
    public CinemachineBrain cinemachineBrain;
    private int cameraNumber;
    private bool stage2D = false;
    public int CameraNumber{get{return cameraNumber;}}
    public bool Stage2D{get{return stage2D;}}
    
    public void GetObj()
    {
        // keyのuiとオブジェを取得
        key = GameObject.Find("KEY(Clone)");
        children = new Transform[key.transform.childCount];
        for (int i = 0; i < key.transform.childCount; i++) {
            children[i] = key.transform.GetChild(i);
        }

        pickaxes.AddRange(GameObject.FindGameObjectsWithTag("pickaxe"));
        for (int i = 0; i < pickaxes.Count; i++) {
            pickaxeObj.Add(pickaxes[i].transform.GetChild(0));
            pickaxeImage.Add(pickaxes[i].transform.GetChild(1));
        }

        objs.AddRange(GameObject.FindGameObjectsWithTag("stage"));
        for(int i = 0; i < objs.Count; i++) {
            meshs.Add(objs[i].GetComponent<MeshRenderer>());
        }
        stage2D = false;
        camera2D.depth = -1;
        children[2].gameObject.SetActive(false);

        foreach (var value in pickaxeImage) {
            value.gameObject.SetActive(false);
        }
    }
    public void Change()
    {
        if(stage2D)
        {
            camera2D.depth = -1;
            stage2D = false;
            foreach(var mesh in meshs)
            {
                mesh.receiveShadows = true;
            }
            
            foreach (var value in children) {
                value.gameObject.SetActive(false);
            }
            children[0].gameObject.SetActive(true);
            children[1].gameObject.SetActive(true);

            foreach (var value in pickaxeImage) {
                value.gameObject.SetActive(false);
            }
            foreach (var value in pickaxeObj) {
                value.gameObject.SetActive(true);
            }

            Sounds.instance.se[5].Play();
        }
        else if(!stage2D)
        {
            camera2D.transform.eulerAngles = CameraSet(cameraNumber);
            
            camera2D.depth = 1;
            stage2D = true;
            foreach(var mesh in meshs)
            {
                mesh.receiveShadows = false;
            }

            foreach (var value in children) {
                value.gameObject.SetActive(false);
            }

            foreach (var value in pickaxeImage) {
                value.gameObject.SetActive(true);
            }
            foreach (var value in pickaxeObj) {
                value.gameObject.SetActive(false);
            }

            children[2].gameObject.SetActive(true);

            Sounds.instance.se[6].Play();
        }
    }

    private Vector3 CameraSet(int num)
    {
        switch (num)
        {
            case 0:
            return new Vector3(90,0,0);
            case 1:
            return new Vector3(90,90,0);
            case 2:
            return new Vector3(90,180,0);
            case 3:
            return new Vector3(90,-90,0);
        }
        return new Vector3(90,0,0);
    }

    public void ChangeCameraRight()//視点変更(右)
    {
        if(stage2D) return;
        foreach(var value in virtualCamera)
        {
            value.Priority = 0;//全てオフ
        }
        cameraNumber++;
        if(cameraNumber >= 4)
        {
            cameraNumber = 0;
        }
        virtualCamera[cameraNumber].Priority = 1;
        Sounds.instance.se[2].Play();
    }
    public void ChangeCameraLeft()//視点変更(右)
    {
        if(stage2D) return;
        foreach(var value in virtualCamera)
        {
            value.Priority = 0;//全てオフ
        }
        cameraNumber--;
        if(cameraNumber <= -1)
        {
            cameraNumber = 3;
        }
        virtualCamera[cameraNumber].Priority = 1;
        Sounds.instance.se[2].Play();
    }
}
